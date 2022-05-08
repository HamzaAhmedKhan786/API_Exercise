using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_Exercise.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Login : ControllerBase
    {
        private IConfiguration _config;
        private UserModel _user;

        public Login(IConfiguration config)
        {
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult LoginUser([FromBody] UserModel userlogin)
        {
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(userlogin);

            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(StatusCodes.Status200OK);
            }
            else
            {
                response = Ok(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        private string GenerateJSONWebToken(UserModel userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.email_address),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.password),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserModel AuthenticateUser(UserModel userlogin)
        {
            _user = null;
            if (!userlogin.email_address.Equals(string.Empty) && !userlogin.password.Equals(string.Empty))
            {
                _user = userlogin;
                Database db = new Database();
                db.Set_User(_user);
                if (db.User_Login() == null)
                {
                    _user = null;
                }
            }
            return _user;
        }

        [HttpGet("{user_email}")]
        public User_Model Get_User_Data(string user_email) 
        {
            User_Model user_data = new User_Model();
            Database db = new Database();
            user_data = db.Get_User_Data(user_email);
            return user_data;
        }

    }
}
