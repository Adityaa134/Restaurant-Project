using Restaurent.Core.Domain.Identity;
using Restaurent.Core.DTO;

namespace Restaurent.Core.ServiceContracts
{
    public interface IJwtService
    {
        /// <summary>
        /// Creates the JWT Token using the given user's information and configuration settings 
        /// </summary>
        /// <param name="user">ApplicationUser object</param>
        /// <returns>Returns the access and refresh token</returns> 

        Task<TokenModel> CreateJwtToken(ApplicationUser user);
    }
}
