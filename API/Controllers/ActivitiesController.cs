using Application.Activities;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class ActivitiesController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<List<ActivityDTO>>> List()
        {
            return await Mediator.Send(new List.Query());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityDTO>> Detail(Guid id)
        {
            var test = await Mediator.Send(new Detail.Query() { Id = id });
            return test;
        }

        //[HttpPost]
        //public async Task<ActionResult<Unit>> Create()
        //{
        //    return await Mediator.Send(new Create.Command());  // This creates the activity but values are not passed along from json body.
        //}

        [HttpPost]
        public async Task<ActionResult<Unit>> Create(Create.Command command)  // This is implemented by [ApiController] of Create([fromBody]Create.Command command), this takes values from body and initializes the command properties inside Create.cs
        {
            return await Mediator.Send(command);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "IsActivityHost")]
        public async Task<ActionResult<Unit>> Edit(Guid id, Edit.Command command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "IsActivityHost")]

        public async Task<ActionResult<Unit>> Delete(Guid id)
        {
            return await Mediator.Send(new Delete.Command() { Id = id });
        }

        [HttpPost("{id}/attend")]
        public async Task<ActionResult<Unit>> Attend(Guid id, Attend.Command command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        [HttpDelete("{id}/attend")]
        public async Task<ActionResult<Unit>> Unattend(Guid id, [FromForm]Unattend.Command command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }
    }
}