using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CityInfo.Api.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController(IConfiguration configuration) : ControllerBase
    {
        readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        public class AuthenticationRequestBody
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }

        [HttpPost("Authenticate")]
        public ActionResult<string> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
            var user = ValidateUserCredentials(authenticationRequestBody.UserName, authenticationRequestBody.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(_configuration["Authentication:SecretForKey"] ?? ""));

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>
            {
                new("sub", user.UserId.ToString()),
                new("given_name", user.FirstName),
                new("family_name", user.LastName),
                new("city", user.City)
            };

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            return Ok(new JwtSecurityTokenHandler()
                .WriteToken(jwtSecurityToken));
        }

        private CityInfoUser ValidateUserCredentials(string? userName, string? password)
        {
            // obviously in a real application, the credentials would be checked here.
            return new CityInfoUser(
                1, userName ?? "", "Jayme", "Desrosiers", "Winnipeg");
        }

        private class CityInfoUser(int userId, string userName, string firstName, string lastName, string city)
        {
            public int UserId { get; set; } = userId;
            public string UserName { get; set; } = userName;
            public string FirstName { get; set; } = firstName;
            public string LastName { get; set; } = lastName;
            public string City { get; set; } = city;
        }
    }
}
