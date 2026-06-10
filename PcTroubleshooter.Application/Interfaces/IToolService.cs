/*
 * Keon Bushman
 * PC Troubleshooter
 * Local Windows Troubleshooting Dashboard
 * Created: 2026
 *
 * This file is part of a local troubleshooting utility designed to run
 * approved diagnostic and support tools on a Windows computer with permission.
 */

using PcTroubleshooter.Domain.Models;

namespace PcTroubleshooter.Application.Interfaces;

/// <summary>
/// Defines the main troubleshooting tool service used by the web user interface.
/// </summary>
public interface IToolService
{
    /// <summary>
    /// Gets the list of approved troubleshooting tools that can be displayed in the UI.
    /// </summary>
    /// <returns>A list of available tool definitions.</returns>
    List<ToolDefinition> GetAvailableTools();

    /// <summary>
    /// Runs one approved troubleshooting action.
    /// </summary>
    /// <param name="action">The allowed tool action selected by the user.</param>
    /// <returns>A CommandResult containing the result of the selected tool action.</returns>
    Task<CommandResult> RunToolAsync(ToolAction action);
}
