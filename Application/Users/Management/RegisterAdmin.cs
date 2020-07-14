using Application.CustomValidator;
using Application.Errors;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users
{
    public class RegisterAdmin
    {
        public class Command : IRequest<AdminDTO>
        {
            public string DisplayName { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.DisplayName).NotEmpty();
                RuleFor(x => x.Username).NotEmpty();
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).PasswordValidation();
            }
        }

        public class Handler : IRequestHandler<Command, AdminDTO>
        {
            private readonly DataContext _context;
            private readonly UserManager<AppUser> _userManager;
            private readonly IJwtGenerator _jwtGenerator;

            public Handler(DataContext context, UserManager<AppUser> userManager, IJwtGenerator jwtGenerator)
            {
                _context = context;
                _userManager = userManager;
                _jwtGenerator = jwtGenerator;
            }

            public async Task<AdminDTO> Handle(Command request,
                                     CancellationToken cancellationToken)
            {
                if (await _context.Users.Where(x => x.Email == request.Email).AnyAsync())
                    throw new RestException(HttpStatusCode.BadRequest, new { Email = "Email already in use." });

                if (await _context.Users.Where(x => x.UserName == request.Username).AnyAsync())
                    throw new RestException(HttpStatusCode.BadRequest, new { Username = "Username already in use." });

                var user = new AppUser
                {
                    DisplayName = request.DisplayName,
                    Email = request.Email,
                    UserName = request.Username

                };

                var UserCreationResult = await _userManager.CreateAsync(user, request.Password);
                var AssignRoleToUser = await _userManager.AddToRoleAsync(user, "Admin");

                if (UserCreationResult.Succeeded && AssignRoleToUser.Succeeded)
                {
                    try
                    {
                        AdminDTO tmpRegisteredUser = new AdminDTO
                        {
                            DisplayName = user.DisplayName,
                            Token = _jwtGenerator.CreateToken(user),
                            Username = user.UserName,
                            //Image = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
                            Image = null,
                            Roles = await _userManager.GetRolesAsync(user)
                        };
                        return tmpRegisteredUser;
                    }
                    catch (Exception exp)
                    {
                        throw new RestException(HttpStatusCode.BadRequest, "Problem Returning AdminDTO");
                    }
                }

                throw new RestException(HttpStatusCode.InternalServerError, "Problem Creating Admin");
            }
        }
    }
}
