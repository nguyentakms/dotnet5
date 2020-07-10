namespace WebApi.Modules.Common
{
    using Domain.Security.Services;
    using Infrastructure.ExternalAuthentication;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    ///     Authentication Extensions.
    /// </summary>
    public static class AuthenticationExtensions
    {
        /// <summary>
        ///     Add Authentication Extensions.
        /// </summary>
        public static IServiceCollection AddAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            bool useFake = configuration.GetValue<bool>("AuthenticationModule:UseFake");

            if (useFake)
            {
                services.AddScoped<IUserService, TestUserService>();
            }
            else
            {
                services.AddScoped<IUserService, ExternalUserService>();
            }

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://localhost:7001";

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "api1");
                });
            });

            return services;
        }
    }
}
