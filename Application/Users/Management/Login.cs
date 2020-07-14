using Application.Errors;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users
{
    public class Login
    {
        public class Query : IRequest<User> 
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class QueryValidator: AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Query, User>
        {
            private UserManager<AppUser> _userManager;
            private RoleManager<IdentityRole> _roleManager;
            private SignInManager<AppUser> _signInManager;
            private IJwtGenerator _jwtGenerator;

            public Handler(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, IJwtGenerator jwtGenerator)
            {
                _userManager = userManager;
                _roleManager = roleManager;
                _signInManager = signInManager;
               _jwtGenerator = jwtGenerator;
            }
            public async Task<User> Handle(Query request,
                                               CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                    throw new RestException(HttpStatusCode.Unauthorized);
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (result.Succeeded)
                {
                    return new User()
                    {
                        DisplayName = user.DisplayName,
                        Username = user.UserName,
                        Token = _jwtGenerator.CreateToken(user),
                        //Image = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
                        Image = null,
                        Roles = await _userManager.GetRolesAsync(user)
                    };
                }
                throw new RestException(HttpStatusCode.Unauthorized);
            }
        }
    }
}
