using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyAPI.Common;
using MyAPI.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyAPI.Services.Services
{
    public class JwtService : IJwtService
    {
        private readonly SiteSettings _siteSettings;
        public JwtService(IOptionsSnapshot<SiteSettings> settins)
        {
            _siteSettings = settins.Value;
        }
        public string Generate(User user)
        {
            var secretKey = Encoding.UTF8.GetBytes(_siteSettings.JwtSettings.SecretKey);//longer than 16 character
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

            var encryptionkey = Encoding.UTF8.GetBytes("16CharEncryptKey"); //must be 16 character
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionkey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);
            var claims = _getClaims(user);
            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _siteSettings.JwtSettings.Issuer,
                Audience = _siteSettings.JwtSettings.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now.AddMinutes(_siteSettings.JwtSettings.NotBeforeMinutes),
                Expires = DateTime.Now.AddHours(_siteSettings.JwtSettings.ExpirationMinutes),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(claims)

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securiryToken = tokenHandler.CreateToken(descriptor);
            var jwt = tokenHandler.WriteToken(securiryToken);
            return jwt;
        }

        private IEnumerable<Claim> _getClaims(User user)
        {
            var secutiryStampClaimType = new ClaimsIdentityOptions().SecurityStampClaimType;
            var list = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.MobilePhone, "09123456987"),
                new Claim(secutiryStampClaimType , user.SecurityStamp.ToString()),
            };

            var roles = new Role[] { new Role { Name = "Admin" } };
            foreach (var role in roles)
                list.Add(new Claim(ClaimTypes.Role, role.Name));

            return list;
        }
    }
}
