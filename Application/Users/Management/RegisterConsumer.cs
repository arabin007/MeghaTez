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
    public class RegisterConsumer
    {
        public class Command : IRequest<ConsumerDTO>
        {
            //UserInfo
            public string DisplayName { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }

            //ConsumerInfo
            public string ConsumerId { get; set; }
            public string TypeOfConsumer { get; set; }
            public string NationalId { get; set; }
            public string MeterId { get; set; }
            public string MeterCapacity { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.DisplayName).NotEmpty();
                RuleFor(x => x.Username).NotEmpty();
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).PasswordValidation();
                RuleFor(x => x.ConsumerId).NotEmpty();
                RuleFor(x => x.TypeOfConsumer).NotEmpty();
                RuleFor(x => x.NationalId).NotEmpty();
                RuleFor(x => x.MeterId).NotEmpty();
                RuleFor(x => x.MeterCapacity).NotEmpty();

            }
        }

        public class Handler : IRequestHandler<Command, ConsumerDTO>
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

            public async Task<ConsumerDTO> Handle(Command request,
                                     CancellationToken cancellationToken)
            {
                if (await _context.Users.Where(x => x.Email == request.Email).AnyAsync())
                    throw new RestException(HttpStatusCode.BadRequest, new { Email = "Email already in use." });

                if (await _context.Users.Where(x => x.UserName == request.Username).AnyAsync())
                    throw new RestException(HttpStatusCode.BadRequest, new { Username = "Username already in use." });

                if (await _context.tblConsumers.Where(x => x.MeterId == request.MeterId).AnyAsync())
                    throw new RestException(HttpStatusCode.BadRequest, new { MeterId = "Meter Id already in use." });

                if (await _context.tblConsumers.Where(x => x.ConsumerId == request.ConsumerId).AnyAsync())
                    throw new RestException(HttpStatusCode.BadRequest, new { ConsumerId = "Consumer Id already in use." });

                // Creating Basic User

                var user = new AppUser
                {
                    DisplayName = request.DisplayName,
                    Email = request.Email,
                    UserName = request.Username
                };

                var UserCreationResult = await _userManager.CreateAsync(user, request.Password);
                var AssignRoleToUser = await _userManager.AddToRoleAsync(user, "Consumer");

                if (UserCreationResult.Succeeded && AssignRoleToUser.Succeeded)
                {
                    //Adding Consumer Information to User

                    var consumer = new Consumer()
                    {
                        ConsumerId = request.ConsumerId,
                        NationalId = request.NationalId,
                        MeterCapacity = request.MeterCapacity,
                        MeterId = request.MeterId,
                        TypeOfConsumer = request.TypeOfConsumer,
                        AppIdFK = await _userManager.GetUserIdAsync(user)
                    };

                    await _context.tblConsumers.AddAsync(consumer);
                }

                var ConsumerCreationResult = await _context.SaveChangesAsync() > 0;

                if (ConsumerCreationResult)
                {
                    try
                    {
                        return new ConsumerDTO
                        {
                            DisplayName = user.DisplayName,
                            Token = _jwtGenerator.CreateToken(user),
                            Username = user.UserName,
                            Image = null,
                            ConsumerId = user.Consumer.ConsumerId,
                            MeterCapacity = user.Consumer.MeterCapacity,
                            MeterId = user.Consumer.MeterId,
                            TypeOfConsumer = user.Consumer.TypeOfConsumer,  
                            Roles = await _userManager.GetRolesAsync(user)
                        };

                    }
                    catch (Exception exp)
                    {
                        throw new RestException(HttpStatusCode.BadRequest, "Problem Returning ConsumerDTO");
                    }
                }

                throw new RestException(HttpStatusCode.InternalServerError, "Problem Creating Consumer");
            }
        }
    }
}
