using System;
using ECommerce.Core.Enums;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Restaurent.Core.Domain.Identity;
using Restaurent.Core.DTO;
using Restaurent.Core.Helpers;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IEmailSenderService _emailSenderService;
        private readonly IImageAdderService _imageAdderService;
        private readonly IImageUpdateService _imageUpdateService; 

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IEmailSenderService emailSenderService, IImageAdderService imageAdderService, IImageUpdateService imageUpdateService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailSenderService = emailSenderService;
            _imageAdderService = imageAdderService;
            _imageUpdateService = imageUpdateService;
        }


        public async Task<bool> IsEmailAlereadyRegistered(string email)
        {
           ApplicationUser? user =  await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            return true;
        }

        public async Task<bool> IsUserNameAleradyExist(string userName)
        {
            ApplicationUser? user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return false;
            return true;
        }

        public async Task<string> GetUserRole(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            //checking does user object is correct or not 
            ApplicationUser? appUser = await _userManager.FindByNameAsync(user.UserName);

            if (appUser != null)
            {
                //checking user role 
                if (await _userManager.IsInRoleAsync(appUser, "admin"))
                {
                    return "admin";
                }
            }
            return "user";
        }

        public async Task<SignInResult> Login(LoginRequestt loginRequest)
        {
            if (loginRequest == null)
                throw new ArgumentNullException(nameof(loginRequest));

            if (loginRequest.Password == null || loginRequest.UserName == null)
                throw new ArgumentException(nameof(loginRequest.Password), nameof(loginRequest.UserName));

            ValidationHelper.ModelValidator(loginRequest);

            //sign in the user 
            SignInResult result = await _signInManager.PasswordSignInAsync(loginRequest.UserName, loginRequest.Password, isPersistent: false, lockoutOnFailure: false);

            return result;
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> Register(RegisterRequestt registerRequest)
        {
            if (registerRequest == null)
                throw new ArgumentNullException(nameof(registerRequest));
            if (registerRequest.Password == null || registerRequest.Email == null)
                throw new ArgumentException(nameof(registerRequest.Password), nameof(registerRequest.Email));

            ValidationHelper.ModelValidator(registerRequest);

            ApplicationUser user = registerRequest.ToApplicationUser();

            //Creating user
            IdentityResult result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (result.Succeeded)
            {
                //checking if user role is created or not in ASPNetRoles Table
                if (await _roleManager.FindByNameAsync(UserTypeOptions.User.ToString()) is null)
                {
                    ApplicationRole role = new ApplicationRole()
                    {
                        Name = UserTypeOptions.User.ToString()
                    };

                    //Creating role in ASPNetRoles Table as User role is not exist
                    await _roleManager.CreateAsync(role);

                    // inserting user and it's role in ASPNetUserRoles Table
                    await _userManager.AddToRoleAsync(user, UserTypeOptions.User.ToString());

                    //generating emailConfirmationToken 
                    await GenerateEmailConfirmationToken(user);
                    return result;
                }
                else
                {
                    // inserting user and it's role in ASPNetUserRoles Table
                    await _userManager.AddToRoleAsync(user, UserTypeOptions.User.ToString());

                    //generating emailConfirmationToken 
                    await GenerateEmailConfirmationToken(user);
                }
            }
            return result;
        }

        public async Task GenerateEmailConfirmationToken(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            if (token != null)
            {
                // sending email verification link to user's email id 
                await _emailSenderService.SendEmailConfirmation(user, token);
            }
        }

        public async Task<IdentityResult> ConfirmEmail(string uid, string token)
        {
            return await _userManager.ConfirmEmailAsync(await _userManager.FindByIdAsync(uid), token);
        }

        public async Task<ApplicationUser?> FindUserByEmail(string email)
        {
          ApplicationUser? user =  await _userManager.FindByEmailAsync(email);
            if(user == null)
                return null;
            return user;
        }

        public async Task<ApplicationUser?> FindUserByUserName(string userName)
        {
            ApplicationUser? user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return null;
            return user;
        }

        public async Task<UserDTO?> GetUserByUserId(Guid userId)
        {
            if(userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId));
            string userID = userId.ToString();
            ApplicationUser? user = await _userManager.FindByIdAsync(userID);
            if (user == null)
                return null;
            UserDTO userDTO = new UserDTO()
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ProfileImagePath
            };
            return userDTO;
        }

        public async Task UpdateRefreshTokenInTable(ApplicationUser user ,AuthenticationResponse authenticationResponse)
        {
            user.RefreshToken = authenticationResponse.RefreshToken;
            user.RefershTokenExpirationDateTime = authenticationResponse.RefershTokenExpirationDateTime;
            await _userManager.UpdateAsync(user);
        }

        public async Task GenerateForgotPasswordToken(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            if (token != null)
            {
                // sending reset-password link to user's email id 
                await _emailSenderService.SendForgotPasswordEmail(user, token);
            }
        }

        public async Task<IdentityResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            return await _userManager.ResetPasswordAsync(await _userManager.FindByIdAsync(resetPasswordDTO.Uid), resetPasswordDTO.Token, resetPasswordDTO.Password);
        }

        public async Task<IdentityResult?> Register(GoogleJsonWebSignature.Payload payload)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            ApplicationUser? user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                ApplicationUser newUser = new ApplicationUser()
                {
                    Email = payload.Email,
                    UserName = payload.Name,
                    EmailConfirmed = payload.EmailVerified
                };

                IdentityResult result = await _userManager.CreateAsync(newUser);

                if (result.Succeeded)
                {
                    if (await _roleManager.FindByNameAsync(UserTypeOptions.User.ToString()) is null)
                    {
                        ApplicationRole role = new ApplicationRole()
                        {
                            Name = UserTypeOptions.User.ToString()
                        };

                        await _roleManager.CreateAsync(role);

                        await _userManager.AddToRoleAsync(user, UserTypeOptions.User.ToString());

                        return result;
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(newUser, UserTypeOptions.User.ToString());
                        return result;
                    }
                }

                return result;
            }
            return null;
        }

        public async Task<UserDTO?> UpdatePersonalDetails(UserPersonalDetailsDTO personalDetailsDTO)
        {
            if (personalDetailsDTO == null)
                throw new ArgumentNullException(nameof(personalDetailsDTO));
            if(personalDetailsDTO.UserId==null)
                throw new ArgumentNullException(nameof(personalDetailsDTO));

            ValidationHelper.ModelValidator(personalDetailsDTO);

            ApplicationUser? user =  await _userManager.FindByIdAsync(personalDetailsDTO.UserId.ToString());
            if(user != null)
            {
                user.PhoneNumber = personalDetailsDTO.PhoneNumber;
                user.UserName = personalDetailsDTO.UserName;
                if(personalDetailsDTO.ProfileImage !=null && personalDetailsDTO.ProfileImage.Length > 0)
                {
                    if (user.ProfileImagePath == null)
                    {
                       string imagePath =  await _imageAdderService.ImageAdder(personalDetailsDTO.ProfileImage);
                        user.ProfileImagePath = imagePath;
                        await _userManager.UpdateAsync(user);
                        UserDTO? userDetails = await GetUserByUserId(user.Id);
                        return userDetails;
                    }
                    else
                    {
                        string imagePath =  await _imageUpdateService.ImageUpdater(personalDetailsDTO.ProfileImage, user.ProfileImagePath);
                            user.ProfileImagePath = imagePath;
                            await _userManager.UpdateAsync(user);
                            UserDTO? userDetailsUpdate = await GetUserByUserId(user.Id);
                            return userDetailsUpdate;
                    }   
                }
                user.ProfileImagePath = personalDetailsDTO.ProfileImagePath;
                await _userManager.UpdateAsync(user);
                UserDTO? userDTO = await GetUserByUserId(user.Id);
                return userDTO;
            }
            return null;
        }

    }
}
