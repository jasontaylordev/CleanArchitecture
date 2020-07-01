using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Infrastructure.Cloud;
using CleanArchitecture.Infrastructure.Email;
using CleanArchitecture.Infrastructure.Files;
using CleanArchitecture.Infrastructure.Identity;
using CleanArchitecture.Infrastructure.PDF;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.QrCode;
using CleanArchitecture.Infrastructure.Services;
using DinkToPdf;
using DinkToPdf.Contracts;
using IdentityServer4.Services;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("DefaultDb"));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            }

            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());

            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<ApplicationRole>()
                .AddRoleManager<RoleManager<ApplicationRole>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            //.AddClaimsPrincipalFactory<UserClaimPrincipalFactory>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                // Default Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
                options.User.RequireUniqueEmail = true;
            });

            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<ICsvFileBuilder, CsvFileBuilder>();
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            services.AddTransient<IPDFConverter, PDFConverterHelper>();
            services.AddTransient<IQRGenerator, QRGenerator>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ICloudUploader, CloudUploader>();
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimPrincipalFactory>();
            services.AddScoped<IProfileService, ProfileService>();

            services.AddAuthentication()
                 .AddGoogle(googleOptions =>
                 {
                     googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
                     googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
                     googleOptions.SignInScheme = IdentityConstants.ExternalScheme;
                 })
                .AddIdentityServerJwt();
            return services;
        }
    }
}
