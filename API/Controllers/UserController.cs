using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class UserController : BaseController
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(Login.Query query)
        {
            return await Mediator.Send(query);
        }

        [AllowAnonymous]
        [HttpPost("RegisterAdmin")]
        public async Task<ActionResult<AdminDTO>> RegisterAdmin(RegisterAdmin.Command command)
        {
            return await Mediator.Send(command);
        }

        [AllowAnonymous]
        [HttpPost("RegisterConsumer")]
        public async Task<ActionResult<ConsumerDTO>> RegisterConsumer(RegisterConsumer.Command command)
        {
            return await Mediator.Send(command);
        }

        [HttpGet]
        public async Task<ActionResult<User>> CurrentUser()
        {
            return await Mediator.Send(new CurrentUser.Query());
        }

        [HttpGet("ListConsumer")]
        public async Task<ActionResult<List<ConsumerDTO>>> ListConsumer([FromForm]ListConsumer.Query query)
        {
            query.Role = "Consumer";
            return await Mediator.Send(query);
        }

        [HttpGet("ListAdmin")]
        public async Task<ActionResult<List<AdminDTO>>> ListUser([FromForm]ListAdmin.Query query)
        {
            query.Role = "Admin";
            return await Mediator.Send(query);
        }
    }
}