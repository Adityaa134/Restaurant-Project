using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Restaurent.Core.Domain.Identity;
using Restaurent.Core.DTO;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.WebAPI.Controllers
{
    [AllowAnonymous]
    public class AccountController : CustomControllerBase
    {
        private readonly IAuthService _authService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtService _jwtService;

        public AccountController(IAuthService authService,IJwtService jwtService, SignInManager<ApplicationUser> signInManager) 
        {
            _authService = authService;
            _jwtService = jwtService;
            _signInManager = signInManager;
        }


        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginRequestt loginRequest)
        {
            if (ModelState.IsValid == false)
            {
                string errorMessage = string.Join("|", ModelState.Values.SelectMany(value => value.Errors).Select(e => e.ErrorMessage));
                return Problem(errorMessage);
            }

           var result =  await _authService.Login(loginRequest);
           if(result.Succeeded)
           {
                var user = await _authService.FindUserByUserName(loginRequest.UserName);
                if (user == null)
                {
                    return Problem("Invalid Email Id");
                }
                TokenModel tokenModel = await _jwtService.CreateJwtToken(user);
                await _authService.UpdateRefreshTokenInTable(user, tokenModel);

                AuthenticationResponse authenticationResponse = new AuthenticationResponse()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = await _authService.GetUserRole(user),
                    ProfileImage = user.ProfileImagePath
                };

                await _authService.SetTokensInsideCookie(tokenModel, HttpContext);
                return Ok(authenticationResponse);
           }
           if (result.IsNotAllowed)
           { 
                return Problem("Please verify your emailId to login");
           }
           else
           {
                return Problem("Invalid Username or password");
           }
        }

        [HttpGet("logout")]

        public async Task<ActionResult> Logout()
        {
            await _authService.Logout(HttpContext);
            return NoContent();
        }

        [HttpPost("register")]
        public async Task<ActionResult> PostRegister(RegisterRequestt registerRequest)
        {
            if (ModelState.IsValid == false)
            {
                string errorMessage = string.Join("|", ModelState.Values.SelectMany(value => value.Errors).Select(e => e.ErrorMessage));
                return Problem(errorMessage);
            }

           
           IdentityResult result =  await _authService.Register(registerRequest);
            if (result.Succeeded)
            {
                ApplicationUser? user = await _authService.FindUserByEmail(registerRequest.Email);
                if (user==null)
                {
                    return Problem("Invalid Email Id");
                }

                if (!user.EmailConfirmed)
                {
                    return Problem("Please verify your emailId to login");
                }
                TokenModel tokenModel = await _jwtService.CreateJwtToken(user);
                await _authService.UpdateRefreshTokenInTable(user, tokenModel);

                AuthenticationResponse authenticationResponse = new AuthenticationResponse()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = await _authService.GetUserRole(user),
                    ProfileImage = user.ProfileImagePath
                };
                await _authService.SetTokensInsideCookie(tokenModel, HttpContext);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok(authenticationResponse);
            }
            else
            {
                string errorMesasage = string.Join("|", result.Errors.Select(err => err.Description));
                return Problem(errorMesasage);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> GetNewJwtAndRefreshToken()
        {
            HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);

            if (refreshToken == null)
                return Unauthorized();
            var user  = await _authService.GetUserByRefreshToken(refreshToken);

            
            //if user is null or refreshToken does not match with the refreshToken stored in db or the refreshTokenExpireTime is completed it means user has to login again. 
            if(user==null || user.RefreshToken!=refreshToken || user.RefershTokenExpirationDateTime <= DateTime.UtcNow)
                return Unauthorized("Invlaid refersh token");

            TokenModel token = await _jwtService.CreateJwtToken(user);
            await _authService.UpdateRefreshTokenInTable(user, token);
            AuthenticationResponse authenticationResponse = new AuthenticationResponse()
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = await _authService.GetUserRole(user),
                ProfileImage = user.ProfileImagePath
            };
            await _authService.SetTokensInsideCookie(token, HttpContext);
            return Ok(authenticationResponse);
        }

        [HttpGet("EmailExist")]
        public async Task<ActionResult> IsEmailAlreadyRegistered(string email)
        {
            bool result = await _authService.IsEmailAlereadyRegistered(email);
            if (!result)
            {
                return Ok(new { exists = result }); 
            }
            return Ok(new { exists = result }); 
        }

        [HttpGet("UserNameExist")]
        public async Task<ActionResult> IsUserNameAleradyExist(string userName)
        {
            bool result = await _authService.IsUserNameAleradyExist(userName);
            if (!result)
            {
                return Ok(new { exists = result }); 
            }
            return Ok(new { exists = result }); 
        }

        [HttpGet("confirm-email")]
        public async Task<ActionResult> ConfirmEmail(string email)
        {
            var user = await _authService.FindUserByEmail(email);
            if (user != null)
            {
                if (user.EmailConfirmed)
                {
                    return Ok("Your email has alerady confirmed.Please Login");   
                }
                else
                {
                    await _authService.GenerateEmailConfirmationToken(user);
                    return Ok("Email has been sent to your registered email id");
                }
            }
            else
            {
                return Problem("Something Went Wrong Try Again");
            }
           
        }

        [HttpPost("confirm-email-success")]
        public async Task<ActionResult> ConfirmEmail(string uid,string token)
        {
            if (!string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(token))
            {
                token = token.Replace(' ', '+');
                var result = await _authService.ConfirmEmail(uid, token);
                if (result.Succeeded)
                {
                    return Ok("Email Verified");
                }
                return BadRequest("Toke or Uid is not valid");
            }
            return BadRequest("Token or Uid can't be null ");
        }

        [HttpGet("forgot-password")]
        public async Task<ActionResult> ForgotPasswordEmail(string email)
        {
            if (email == null)
                return BadRequest("Email can't be null");
            ApplicationUser? user =  await _authService.FindUserByEmail(email);
            if (user == null)
                return BadRequest("Email Id is not linked with any acccount");
            await _authService.GenerateForgotPasswordToken(user);
            return Ok(true);
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            if (ModelState.IsValid == false)
            {
                string errorMessage = string.Join("|", ModelState.Values.SelectMany(value => value.Errors).Select(e => e.ErrorMessage));
                return Problem(errorMessage);
            }

            resetPasswordDTO.Token = resetPasswordDTO.Token.Replace(' ', '+');
            var result = await _authService.ResetPassword(resetPasswordDTO);

            if (result.Succeeded)
            {
                resetPasswordDTO.IsPasswordChangedSuccessfully = true;
                return Ok(true);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return BadRequest(resetPasswordDTO);
        }

        [Authorize]
        [HttpGet("restore-session")]
        public async Task<ActionResult> RestoreSession() 
        {
            var userIdClaim =  User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out Guid userId))
                return Unauthorized();

            UserDTO? user =  await _authService.GetUserByUserId(userId);
            if(user == null)
                return Unauthorized();
            AuthenticationResponse authenticationResponse = new AuthenticationResponse()
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                ProfileImage = user.ProfileImage,
            };
            return Ok(authenticationResponse);
        }
    }
}
