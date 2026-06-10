/*
 * Keon Bushman
 * PC Troubleshooter
 * Local Windows Troubleshooting Dashboard
 * Created: 2026
 *
 * This file is part of a local troubleshooting utility designed to run
 * approved diagnostic and support tools on a Windows computer with permission.
 */

using System.Diagnostics;
using PcTroubleshooter.Application.Interfaces;
using PcTroubleshooter.Application.Services;
using PcTroubleshooter.Domain.Models;
using PcTroubleshooter.Infrastructure.Windows.Services;

const string dashboardUrl = "http://localhost:5055";

LaunchMode launchMode = GetLaunchMode();

if (launchMode == LaunchMode.Exit)
{
    Console.WriteLine("PC Troubleshooter was closed before starting.");
    return;
}

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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

// Starts the local web server before opening the dashboard.
app.Urls.Add(dashboardUrl);
await app.StartAsync();

Console.WriteLine();
Console.WriteLine($"PC Troubleshooter is running at {dashboardUrl}");
Console.WriteLine("Keep this window open while using the dashboard.");
Console.WriteLine("Press Ctrl + C to stop the application.");
Console.WriteLine();

if (launchMode == LaunchMode.Browser)
{
    OpenBrowser(dashboardUrl);
}

// Keeps the local web server running until the user stops the program.
await app.WaitForShutdownAsync();

/// <summary>
/// Shows a startup menu and gets the user's preferred launch mode.
/// </summary>
/// <returns>The selected launch mode.</returns>
static LaunchMode GetLaunchMode()
{
    while (true)
    {
        Console.Clear();

        Console.WriteLine("PC Troubleshooter");
        Console.WriteLine("----------------");
        Console.WriteLine();
        Console.WriteLine("Choose launch mode:");
        Console.WriteLine("1. Open in web browser");
        Console.WriteLine("2. Start server only");
        Console.WriteLine("3. Exit");
        Console.WriteLine();
        Console.Write("Selection: ");

        string? input = Console.ReadLine();

        switch (input)
        {
            case "1":
                return LaunchMode.Browser;

            case "2":
                return LaunchMode.ServerOnly;

            case "3":
                return LaunchMode.Exit;

            default:
                Console.WriteLine();
                Console.WriteLine("Invalid selection. Press Enter to try again.");
                Console.ReadLine();
                break;
        }
    }
}

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
/// Defines the startup mode selected from the console menu.
/// </summary>
public enum LaunchMode
{
    Browser,
    ServerOnly,
    Exit
}
