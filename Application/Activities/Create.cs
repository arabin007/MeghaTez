using Domain;
using MediatR;
using Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using System.Linq;

namespace Application.Activities
{
    public class Create
    {
        public class Command: IRequest
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
            public DateTime Date { get; set; }
            public string City { get; set; }
            public string Venue { get; set; }
        }

        public class CommandValidator: AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Title).NotEmpty();
                RuleFor(x => x.Description).NotEmpty();
                RuleFor(x => x.Category).NotEmpty();
                RuleFor(x => x.City).NotEmpty();
                RuleFor(x => x.Venue).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Unit> Handle(Command request,
                                     CancellationToken cancellationToken)
            {
                var activity = new Activity()
                {
                    Id = request.Id,
                    Title = request.Title,
                    City = request.City,
                    Category = request.Category,
                    Date = request.Date,
                    Description = request.Description,
                    Venue = request.Venue
                };

                _context.tblActivities.Add(activity);

                var user = _context.Users.SingleOrDefault(x => x.UserName == _userAccessor.GetCurrentUsername());

                var attendee = new UserActivity()
                {
                    AppUser = user,
                    Activity = activity,
                    DateJoined = DateTime.Now,
                    IsHost = true
                };

                _context.tblUsersActivities.Add(attendee);

                var result = await _context.SaveChangesAsync();  //Returns the number of changes made.
                if(result > 0)
                {
                    return Unit.Value;
                }
                throw new Exception("Problem Saving Changes.");
            }
        }
    }
}
