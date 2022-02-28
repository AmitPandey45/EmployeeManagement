using EmployeeManagement.Models;
using EmployeeManagement.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement
{
    public class Startup
    {
        private const string AppSettings = "AppSettings";
        private const string GoogleSecretDetails = "GoogleSecretDetails";
        private const string FacebookSecretDetails = "FacebookSecretDetails";
        private const string ClientId = "ClientId";
        private const string ClientSecret = "ClientSecret";

        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string googleSecretDetailsSection = $"{AppSettings}:{GoogleSecretDetails}";
            string facebookSecretDetailsSection = $"{AppSettings}:{FacebookSecretDetails}";

            services.AddDbContextPool<AppDbContext>(
                options => options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 3;

                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = "AF_EmployeeManagement";
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/administration/accessdenied";
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "AI_EmployeeManagement";
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(ClaimsStore.DeleteRolePolicy,
                    policy => policy.RequireClaim(ClaimsStore.DeleteRole, ClaimsStore.ClaimValueYes));

                //options.AddPolicy(ClaimsStore.EditRolePolicy,
                //    policy => policy.RequireAssertion(context =>
                //    (context.User.IsInRole(ClaimsStore.AdminRole) &&
                //    context.User.HasClaim(claim => claim.Type.Equals(ClaimsStore.EditRole) && claim.Value.Equals(ClaimsStore.ClaimValueYes))) ||
                //    context.User.IsInRole(ClaimsStore.SuperAdminRole)
                //    ));

                options.AddPolicy(ClaimsStore.EditRolePolicy,
                    policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));

                options.AddPolicy(ClaimsStore.AdminRolePolicy,
                    policy => policy.RequireRole(ClaimsStore.AdminRole));
            });

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = _config.GetValue<string>($"{googleSecretDetailsSection}:{ClientId}");
                    options.ClientSecret = _config.GetValue<string>($"{googleSecretDetailsSection}:{ClientSecret}");
                })
                .AddFacebook(options =>
                {
                    options.AppId = _config.GetValue<string>($"{facebookSecretDetailsSection}:{ClientId}");
                    options.AppSecret = _config.GetValue<string>($"{facebookSecretDetailsSection}:{ClientSecret}");
                });

            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            })
                .AddXmlSerializerFormatters();

            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(route =>
            {
                route.MapRoute("default", "{controller=home}/{action=index}/{id?}");
            });
        }
    }
}
