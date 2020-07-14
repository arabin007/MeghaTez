using Application.Errors;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Activities
{
    public class Detail
    {
        public class Query: IRequest<ActivityDTO>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, ActivityDTO>
        {
            private DataContext _context;
            private readonly IMapper _autoMapper;

            public Handler(DataContext context, IMapper autoMapper)
            {
                _context = context;
                _autoMapper = autoMapper;
            }

            public async Task<ActivityDTO> Handle(Query request,
                                         CancellationToken cancellationToken)
            {

                var activity = await _context.tblActivities
                                                        //.Include(x => x.UserActivities)
                                                        //.ThenInclude(a => a.AppUser)
                                                        //.SingleOrDefaultAsync(s => s.Id == request.Id);   //Using SingleOrDefault since we cant use Find after Include
                                                        .FindAsync(request.Id);

                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, new { activity = "Not Found" });

                var returnActivityDTO = _autoMapper.Map<Activity, ActivityDTO>(activity);

                return returnActivityDTO;
            }
        }
    }
}
