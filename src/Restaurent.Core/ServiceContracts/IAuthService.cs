using System;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Restaurent.Core.Domain.Identity;
using Restaurent.Core.DTO;

namespace Restaurent.Core.ServiceContracts
{
    public interface IAuthService
    {
        /// <summary>
        /// Log in the user in application
        /// </summary>
        /// <param name="loginRequest">User details to Login</param>
        /// <returns>Returns SignInResult</returns>
        Task<SignInResult> Login(LoginRequestt loginRequest);

        /// <summary>
        /// Register the user   
        /// </summary>
        /// <param name="registerRequest">User details to register</param>
        /// <returns>Returns IdenityResult object which tells user creation is successfull or not</returns>

        Task<IdentityResult> Register(RegisterRequestt registerRequest);

        /// <summary>
        /// Log out the user 
        /// </summary>
        /// <returns></returns>
        Task Logout();

        /// <summary>
        /// Checks if the email alerady exist or not 
        /// </summary>
        /// <param name="email">email to check</param>
        /// <returns>Returns true if exists; otherwise false</returns>
        Task<bool> IsEmailAlereadyRegistered(string email);

        /// <summary>
        /// Checks if the userName alerady exist or not
        /// </summary>
        /// <param name="userName">userName to check</param>
        /// <returns>Returns true if exists; otherwise false</returns>
        Task<bool> IsUserNameAleradyExist(string userName);

        /// <summary>
        /// Returns the role of user
        /// </summary>
        /// <param name="user">The user to check</param>
        /// <returns>Returns the user role</returns>

        Task<string> GetUserRole(ApplicationUser user);

        /// <summary>
        /// Returns the user by searching through email
        /// </summary>
        /// <param name="email">The email on which user will be searched</param>
        /// <returns>Returns the user if the user is found based on email otherwise returns null</returns>
        Task<ApplicationUser?> FindUserByEmail(string email);

        /// <summary>
        /// Returns the user by searching through userName
        /// </summary>
        /// <param name="userName">The userName on which user will be searched</param>
        /// <returns>Returns the user if the user is found based on userName otherwise returns null</returns>
        Task<ApplicationUser?> FindUserByUserName(string userName);

        /// <summary>
        /// Search for user based on id
        /// </summary>
        /// <param name="userId">the user to be search</param>
        /// <returns>Retuns user based on id</returns>
        Task<UserDTO?> GetUserByUserId(Guid userId);

        /// <summary>
        /// Updates User detail's
        /// </summary>
        /// <param name="personalDetailsDTO">Updated User details</param>
        /// <returns>Returns updated User details</returns>
        Task<UserDTO?> UpdatePersonalDetails(UserPersonalDetailsDTO personalDetailsDTO);

        /// <summary>
        /// Updates the token information in user's table
        /// </summary>
        /// <param name="user">The user information in which token information needs to be updated</param>
        /// <param name="authenticationResponse">The token information</param>
        /// <returns></returns>
        Task UpdateRefreshTokenInTable(ApplicationUser user, AuthenticationResponse authenticationResponse);

        /// <summary>
        /// Confirms the user's email address
        /// </summary>
        /// <param name="uid">The user ID whose email needs to be confirmed</param>
        /// <param name="token">The email confirmation token generated during registration</param>
        /// <returns>IdentityResult indicating success or failure of the confirmation</returns>
        Task<IdentityResult> ConfirmEmail(string uid, string token);

        /// <summary>
        /// Genrates email confirmation token 
        /// </summary>
        /// <param name="user">The user for which email confirmation token will be generated</param>
        /// <returns></returns>
        Task GenerateEmailConfirmationToken(ApplicationUser user);

        /// <summary>
        /// Genrates forgot password token
        /// <param name="user">The user for which forgot password token will be generated</param>
        /// <returns></returns>
        Task GenerateForgotPasswordToken(ApplicationUser user);

        /// <summary>
        /// Resets the password
        /// </summary>
        /// <param name="resetPasswordDTO">The new password details</param>
        /// <returns>Returns the result if updation of password is succesfull or not </returns>
        Task<IdentityResult> ResetPassword(ResetPasswordDTO resetPasswordDTO);

        /// <summary>
        /// Creates a new user account from Google OAuth payload.
        /// </summary>
        /// <param name="payload">Google authentication payload with user details.</param>
        /// <returns>IdentityResult indicating success/failure, or null if user's account is alerady created.</returns>
        Task<IdentityResult?> Register(GoogleJsonWebSignature.Payload payload);

    }
}
