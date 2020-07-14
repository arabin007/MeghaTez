using Application.Errors;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users
{
    public class ListConsumer
    {
       
        public class Query : IRequest<List<ConsumerDTO>>
        {
            public string Role { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<ConsumerDTO>>
        {
            private UserManager<AppUser> _userManager;

            public Handler( UserManager<AppUser> userManager)
            {
                _userManager = userManager;
            }
            public async Task<List<ConsumerDTO>> Handle(Query request,
                                               CancellationToken cancellationToken)
            {
                var users = await _userManager.GetUsersInRoleAsync(request.Role);
                if (users == null)
                    throw new RestException(HttpStatusCode.NotFound);
                List<ConsumerDTO> returnListUserDTO = new List<ConsumerDTO>();
                foreach (var user in users)
                {
                    var tmpUser = new ConsumerDTO()
                    {
                        DisplayName = user.DisplayName,
                        Username = user.UserName,
                        Image = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
                    };
                    returnListUserDTO.Add(tmpUser);
                }
                return returnListUserDTO;
            }
        }
    }
}
