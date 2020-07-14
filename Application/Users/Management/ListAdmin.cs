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
    public class ListAdmin
    {
       
        public class Query : IRequest<List<AdminDTO>>
        {
            public string Role { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<AdminDTO>>
        {
            private UserManager<AppUser> _userManager;

            public Handler( UserManager<AppUser> userManager)
            {
                _userManager = userManager;
            }
            public async Task<List<AdminDTO>> Handle(Query request,
                                               CancellationToken cancellationToken)
            {
                var users = await _userManager.GetUsersInRoleAsync(request.Role);
                if (users == null)
                    throw new RestException(HttpStatusCode.NotFound);
                List<AdminDTO> returnListUserDTO = new List<AdminDTO>();
                foreach (var user in users)
                {
                    var tmpUser = new AdminDTO()
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
