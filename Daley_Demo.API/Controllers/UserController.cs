using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Daley_Demo.API.Data;
using Daley_Demo.API.Models;
using Daley_Demo.API.Repositories;
using Daley_Demo.API.Services;

namespace Daley_Demo.API.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private readonly UserService _userService;

        public UsersController()
        {
            _userService = new UserService(new UserRepository(new ECommerceDbContext()));
        }

        // POST api/users/register
        [HttpPost]
        [Route("register")]
        public IHttpActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = _userService.GetUserByEmail(user.Email);
            if (existingUser != null)
            {
                return Conflict(); // Email already registered
            }

            _userService.CreateUser(user);
            return Ok(user);
        }

        // POST api/users/login
        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login(User loginUser)
        {
            var user = _userService.GetUserByEmailAndPassword(loginUser.Email, loginUser.Password);

            if (user == null)
            {
                return Unauthorized(); // Invalid credentials
            }

            return Ok(user);
        }
    }
}
