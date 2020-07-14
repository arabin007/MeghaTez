using Application.Errors;
using MediatR;
using Persistence;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Activities
{
    public class Delete
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private  DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Command request,
                                     CancellationToken cancellationToken)
            {
                var activity = await _context.tblActivities.FindAsync(request.Id);
                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, new { activity = "Not Found" } );

                _context.tblActivities.Remove(activity);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return Unit.Value;
                throw new Exception("Could not save change.");
            }
        }
    }
}
