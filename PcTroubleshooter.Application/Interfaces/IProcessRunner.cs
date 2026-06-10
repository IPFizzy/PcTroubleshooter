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
/// Defines the operations needed to run command-line tools and open Windows processes.
/// </summary>
public interface IProcessRunner
{
    /// <summary>
    /// Runs a command-line process and captures the output.
    /// </summary>
    /// <param name="fileName">The executable or command host to run.</param>
    /// <param name="arguments">The command-line arguments to pass into the process.</param>
    /// <returns>A CommandResult containing success status, messages, and output.</returns>
    Task<CommandResult> RunCommandAsync(string fileName, string arguments);

    /// <summary>
    /// Starts a visible Windows process, such as Task Manager or Device Manager.
    /// </summary>
    /// <param name="fileName">The executable, file, folder, URI, or tool to open.</param>
    /// <param name="arguments">Optional arguments to pass into the process.</param>
    /// <param name="runAsAdmin">Determines whether the process should request administrator permission.</param>
    /// <returns>A CommandResult containing the result of the start attempt.</returns>
    CommandResult StartProcess(string fileName, string arguments = "", bool runAsAdmin = false);
}
