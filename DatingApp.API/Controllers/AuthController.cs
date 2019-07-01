using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;


namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        public AuthController(IAuthRepository repo)
        {
            this._repo = repo;
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
    }
}