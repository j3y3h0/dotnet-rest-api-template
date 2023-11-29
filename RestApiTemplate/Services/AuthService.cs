using Microsoft.IdentityModel.Tokens;
using RestApiTemplate.Configurations;
using RestApiTemplate.Models.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestApiTemplate.Service
{
    public class AuthService
    {
        /// <summary>
        /// 토큰 발급 및 반환 함수
        /// </summary>
        /// <param name="username">유저 ID</param>
        public static string GenerateJSONWebToken(string username)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.jwtOption.secretKey));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            int expirationMinutes = AppSettings.jwtOption.expirationMins;

            JwtSecurityToken token = new JwtSecurityToken(
              claims: claims,
              expires: DateTime.Now.AddMinutes(expirationMinutes), // 만료시간 (분)
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
