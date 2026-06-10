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

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Register application and infrastructure services for dependency injection.
builder.Services.AddSingleton<IProcessRunner, WindowsProcessRunner>();
builder.Services.AddSingleton<IToolService, ToolService>();

WebApplication app = builder.Build();

// Allows the app to load index.html from wwwroot by default.
app.UseDefaultFiles();

// Allows the app to serve static files such as HTML, CSS, and JavaScript.
app.UseStaticFiles();

/// <summary>
/// Returns the list of approved troubleshooting tools to the web user interface.
/// </summary>
app.MapGet("/api/tools", (IToolService toolService) =>
{
    return Results.Ok(toolService.GetAvailableTools());
});

/// <summary>
/// Runs one approved troubleshooting tool selected from the web user interface.
/// </summary>
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

// Runs the troubleshooting dashboard only on this computer.
app.Run("http://localhost:5055");
