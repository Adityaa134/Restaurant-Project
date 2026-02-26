using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Restaurent.Core.Domain.Identity;
using Restaurent.Core.DTO;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;

        public JwtService(IConfiguration configuration, IAuthService authService)
        {
            _configuration = configuration;
            _authService = authService;
        }

        public async Task<TokenModel> CreateJwtToken(ApplicationUser user)
        {
            if(user==null)
                throw new ArgumentNullException(nameof(user));

            //ExpirtaionTime of Jwt 
            DateTime expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"]));
            string userRole = await _authService.GetUserRole(user);

            //Claims
            Claim[] claims = new Claim[]
            {
                //userId
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),

                //JwtId
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),

                //Token Issued At Time
                new Claim(JwtRegisteredClaimNames.Iat,DateTimeOffset.Now.ToUnixTimeSeconds().ToString()),

                new Claim(JwtRegisteredClaimNames.Email,user.Email),

                new Claim(JwtRegisteredClaimNames.Name,user.UserName),

                new Claim("role", userRole.ToString()),
            };

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));


            //Hashing Algorithm
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
 
            JwtSecurityToken tokenGenerator = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expiration,
                signingCredentials: signingCredentials
                );

            //generating Jwt token
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string token = tokenHandler.WriteToken(tokenGenerator);

            return new TokenModel()
            {
                AccessToken = token,
                RefreshToken = GenerateRefreshToken(),
                RefershTokenExpirationDateTime = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["RefreshToken:EXPIRATION_MINUTES"]))
            };
        }
        private string GenerateRefreshToken()
        {
            byte[] bytes = new byte[64];
            var randomNumberGenerator = RandomNumberGenerator.Create();

            randomNumberGenerator.GetBytes(bytes);

            return Convert.ToBase64String(bytes);

        }

    }
}
