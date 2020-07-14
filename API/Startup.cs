using Domain;
using MediatR;
using AutoMapper;
using Persistence;
using System.Text;
using API.CustomMiddleware;
using Application.Interfaces;
using Infrastructure.Security;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Photos;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _conf = configuration;
        }

        public IConfiguration _conf { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseSqlServer(_conf.GetConnectionString("NewDbConnection"));
            }
            );
            services.AddMediatR(typeof(Application.Activities.List.Handler).Assembly);
            services.AddAutoMapper(typeof(Application.Activities.List.Handler));

            services.AddCors(opt =>
            {
                opt.AddPolicy("CORS_Policy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000");
                });
            });

            services.AddMvc()
            //(opt =>
            //{
            //    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            //    opt.Filters.Add(new AuthorizeFilter(policy));
            //})
                .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<Application.Activities.Create>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //For Identity User
            var builder = services.AddIdentity<AppUser, IdentityRole>();
            var identityBuilder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            identityBuilder.AddEntityFrameworkStores<DataContext>();
            identityBuilder.AddSignInManager<SignInManager<AppUser>>();
            identityBuilder.AddRoleManager<RoleManager<IdentityRole>>();

            //For Authorization
            services.AddAuthorization(opt =>                                                            //For Custom Authorization of edit and Delete Activity
            {
                opt.AddPolicy("IsActivityHost", policy =>
                {
                    policy.Requirements.Add(new IsHostRequirement());
                });
            });
            services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();


            //For Authentication
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Most powerful secret key"));     //We are generating same key from JwtGenerator too. In there is to create token and here to autheticate it.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(opt =>
                    {
                        opt.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = key,
                            ValidateAudience = false,
                            ValidateIssuer = false
                        };
                    });

            //Registering Interfaces
            services.AddScoped<IJwtGenerator, JwtGenerator>();
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();

            //Setting configuration to the class from appSetting
            services.Configure<CloudinarySettings>(_conf.GetSection("CloudinarySettings"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();  //Custom Error Handling Middleware
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            //app.UseAuthentication();
            app.UseCors("CORS_Policy");
            app.UseMvc();
        }
    }
}
