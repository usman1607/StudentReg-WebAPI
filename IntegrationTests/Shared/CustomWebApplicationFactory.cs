using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;

namespace IntegrationTests.Shared
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IStudentService> StudentServiceMock { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove real service
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IStudentService));

                if (descriptor != null)
                    services.Remove(descriptor);

                // Add mock
                services.AddSingleton(StudentServiceMock.Object);
            });

            builder.ConfigureServices(services =>
            {
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

                services.PostConfigureAll<AuthenticationOptions>(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                });
            });
        }
    }
}
