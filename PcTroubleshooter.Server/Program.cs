/*
 * Keon Bushman
 * PC Troubleshooter
 * Local Windows Troubleshooting Dashboard
 * Created: 2026
 *
 * This file is part of a local troubleshooting utility designed to run
 * approved diagnostic and support tools on a Windows computer with permission.
 */

using PcTroubleshooter.Application.Interfaces;
using PcTroubleshooter.Application.Services;
using PcTroubleshooter.Domain.Models;
using PcTroubleshooter.Infrastructure.Windows.Services;
using PcTroubleshooter.Web.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

int port = GetAvailablePort(5055);
string dashboardUrl = $"http://localhost:{port}";

WebApplicationOptions options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
};

WebApplicationBuilder builder = WebApplication.CreateBuilder(options);

builder.Services.ConfigureHttpJsonOptions(jsonOptions =>
{
    jsonOptions.SerializerOptions.TypeInfoResolverChain.Insert(
        0,
        PcTroubleshooterJsonContext.Default);
});

// Register application and infrastructure services for dependency injection.
builder.Services.AddSingleton<IProcessRunner, WindowsProcessRunner>();
builder.Services.AddSingleton<IToolService, ToolService>();

WebApplication app = builder.Build();

// Allows the app to load index.html from wwwroot by default.
app.UseDefaultFiles();

// Allows the app to serve static files such as HTML, CSS, and JavaScript.
app.UseStaticFiles();

// Returns the list of approved troubleshooting tools to the web user interface.
app.MapGet("/api/tools", (IToolService toolService) =>
{
    return Results.Ok(toolService.GetAvailableTools());
});

// Runs one approved troubleshooting tool selected from the web user interface.
app.MapPost("/api/tools/{action}", async (string action, IToolService toolService) =>
{
    if (!Enum.TryParse(action, ignoreCase: true, out ToolAction toolAction))
    {
        return Results.BadRequest(CommandResult.Fail($"Invalid action: {action}"));
    }

    CommandResult result = await toolService.RunToolAsync(toolAction);

    if (!result.Success)
    {
        return Results.BadRequest(result);
    }

    return Results.Ok(result);
});

// Runs only on the local computer.
app.Urls.Add(dashboardUrl);

// Starts the local web server.
await app.StartAsync();

Console.Clear();
Console.WriteLine("PC Troubleshooter");
Console.WriteLine("----------------");
Console.WriteLine();
Console.WriteLine($"Running locally at: {dashboardUrl}");
Console.WriteLine("The dashboard should open in your browser automatically.");
Console.WriteLine();
Console.WriteLine("Keep this window open while using the tool.");
Console.WriteLine("Close this window or press Ctrl + C to stop.");
Console.WriteLine();

OpenBrowser(dashboardUrl);

// Keeps the local web server running until the user stops the program.
await app.WaitForShutdownAsync();

/// <summary>
/// Opens the dashboard URL in the user's default web browser.
/// </summary>
/// <param name="url">The local dashboard URL to open.</param>
static void OpenBrowser(string url)
{
    try
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        };

        Process.Start(startInfo);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unable to open browser automatically: {ex.Message}");
        Console.WriteLine($"Open this URL manually: {url}");
    }
}

/// <summary>
/// Gets the preferred port if available, otherwise asks Windows for a free local port.
/// </summary>
/// <param name="preferredPort">The preferred port number.</param>
/// <returns>An available port number.</returns>
static int GetAvailablePort(int preferredPort)
{
    if (IsPortAvailable(preferredPort))
    {
        return preferredPort;
    }

    TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
    listener.Start();

    int port = ((IPEndPoint)listener.LocalEndpoint).Port;

    listener.Stop();

    return port;
}

/// <summary>
/// Checks whether a local TCP port is available.
/// </summary>
/// <param name="port">The port number to check.</param>
/// <returns>True if the port is available; otherwise false.</returns>
static bool IsPortAvailable(int port)
{
    try
    {
        TcpListener listener = new TcpListener(IPAddress.Loopback, port);
        listener.Start();
        listener.Stop();

        return true;
    }
    catch
    {
        return false;
    }
}
