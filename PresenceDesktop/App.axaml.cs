using System;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PresenceHTTPClient.APIClients;
using PresenceHTTPClient.Interfaces;

namespace PresenceDesktop;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5193")
            };

            var factory = new SimpleHttpClientFactory(httpClient);
            var loggerFactory = NullLoggerFactory.Instance;

            var groupLogger = loggerFactory.CreateLogger<GroupAPIClient>();
            var userLogger = loggerFactory.CreateLogger<UserAPIClient>();
            var presenceLogger = loggerFactory.CreateLogger<PresenceAPIClient>();

            IGroupAPIClient groupApiClient = new GroupAPIClient(factory, groupLogger);
            IUserAPIClient userApiClient = new UserAPIClient(factory, userLogger);
            IPresenceAPIClient presenceApiClient = new PresenceAPIClient(factory, presenceLogger);

            var mainWindow = new MainWindow(groupApiClient, userApiClient, presenceApiClient);
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
public class SimpleHttpClientFactory : IHttpClientFactory
{
    private readonly HttpClient _httpClient;

    public SimpleHttpClientFactory(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public HttpClient CreateClient(string name) => _httpClient;
}