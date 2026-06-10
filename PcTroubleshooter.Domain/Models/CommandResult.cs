/*
 * Keon Bushman
 * PC Troubleshooter
 * Local Windows Troubleshooting Dashboard
 * Created: 2026
 *
 * This file is part of a local troubleshooting utility designed to run
 * approved diagnostic and support tools on a Windows computer with permission.
 */

namespace PcTroubleshooter.Domain.Models;

/// <summary>
/// Represents the result of running a troubleshooting command or opening a Windows tool.
/// </summary>
public class CommandResult
{
    /// <summary>
    /// Shows whether the command or action completed successfully.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// A short message explaining the result.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Any command-line output returned by the tool.
    /// </summary>
    public string Output { get; set; } = string.Empty;

    /// <summary>
    /// Optional file path used when a tool creates a report or output file.
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// Creates a successful command result.
    /// </summary>
    /// <param name="message">The success message to show to the user.</param>
    /// <param name="output">Optional command output.</param>
    /// <param name="filePath">Optional path to a generated file.</param>
    /// <returns>A successful CommandResult object.</returns>
    public static CommandResult Ok(string message, string output = "", string? filePath = null)
    {
        return new CommandResult
        {
            Success = true,
            Message = message,
            Output = output,
            FilePath = filePath
        };
    }

    /// <summary>
    /// Creates a failed command result.
    /// </summary>
    /// <param name="message">The failure message to show to the user.</param>
    /// <param name="output">Optional command output or error details.</param>
    /// <returns>A failed CommandResult object.</returns>
    public static CommandResult Fail(string message, string output = "")
    {
        return new CommandResult
        {
            Success = false,
            Message = message,
            Output = output
        };
    }
}
