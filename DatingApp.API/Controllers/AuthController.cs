using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            this._repo = repo;
            this._config = config;
        }

        //Registration is case insensitive
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto UserForRegisterDto)
        {
            //validation will be here
            UserForRegisterDto.Username = UserForRegisterDto.Username.ToLower();
            
            if(await _repo.UserExists(UserForRegisterDto.Username))
            {
                return BadRequest("User name already exists");
            }

            var userToCreate = new User()
            {
                Username = UserForRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCreate, UserForRegisterDto.Password);

            return StatusCode(201); //will be replaced with CreatedAtRoute, for now we do not have an User object to return as required by this method
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            //check whether user already exists
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if(userFromRepo == null)
            {
                return Unauthorized();
            }

            //token will have 2 claims
            var claims = new []{
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            //Server needs to sign the token, so
            // we create a key to sign the credentials;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //creating token information - descriptor using data above
            var tokenDescriptor = new SecurityTokenDescriptor(){
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            //Handler allows us to create a token based on descriptor information
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // we send back in response token that they will use futher for logging in 
            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}