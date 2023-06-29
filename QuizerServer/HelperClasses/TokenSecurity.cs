using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Quizer.Models;
using QuizerServer.HelperInterfaces;

namespace QuizerServer.HelperClasses
{
    public class TokenSecurity : ITokenCreator
    {
        private readonly JwtSettings _options;
        public ClaimsCreator _claimsCreator;
        private string username;
        public TokenSecurity(JwtSettings options, string username = "")
        {
            _options = options;
            this.username = username;
            _claimsCreator = new ClaimsCreator(this.username);
        }
        public string GetToken()
        {
            ClaimsIdentity identity = _claimsCreator.GetClaims();
            DateTime timeNow = DateTime.UtcNow;

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                notBefore: timeNow,
                claims: identity.Claims,
                expires: timeNow.Add(TimeSpan.FromDays(1)),
                signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options?.SecretKey!)),
                        SecurityAlgorithms.HmacSha256
                    )
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class ClaimsCreator
    {
        string username;
        public ClaimsCreator(string username)
        {
            this.username = username;
        }
        public ClaimsIdentity GetClaims()
        {
            List<Claim> claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, username) };
            ClaimsIdentity identity = new ClaimsIdentity(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            return identity;
        }
    }
}
