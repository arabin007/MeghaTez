using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<List<ActivityDTO>> { }

        public class Handler : IRequestHandler<Query, List<ActivityDTO>>
        {
            private  DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper _mapper)
            {
                _context = context;
                this._mapper = _mapper;
            }
            public async Task<List<ActivityDTO>> Handle(Query request,
                                               CancellationToken cancellationToken)
            {

                List<Activity> activitiesList = await _context.tblActivities
                                                            //.Include(x => x.UserActivities)       // Use 'Include' & 'ThenInclude' for Eager Loading(Makes single but big query to retrieve all relevant info.)
                                                            //.ThenInclude(a => a.AppUser)          // Here these are excluded since we are using Lazy Loading(Makes multiple small queries if required). For Lazy loading add 'Proxies' from Nuget in Persistence and 'UseLazyLoading' in DbContext of Startup.cs 
                                                            .ToListAsync();

                var returnListActivityDTO = _mapper.Map<List<Activity>, List<ActivityDTO>>(activitiesList);

                return returnListActivityDTO;
            }
        }
    }
}






















