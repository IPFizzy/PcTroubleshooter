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
using PcTroubleshooter.Domain.Models;

namespace PcTroubleshooter.Infrastructure.Windows.Services;

/// <summary>
/// Runs command-line tools and opens Windows processes for the troubleshooting dashboard.
/// </summary>
public class WindowsProcessRunner : IProcessRunner
{
    /// <summary>
    /// Runs a command-line process and captures the standard output and error output.
    /// </summary>
    /// <param name="fileName">The executable or command host to run.</param>
    /// <param name="arguments">The command-line arguments to pass to the process.</param>
    /// <returns>A CommandResult containing the command output and success status.</returns>
    public async Task<CommandResult> RunCommandAsync(string fileName, string arguments)
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = new Process
            {
                StartInfo = startInfo
            };

            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            string combinedOutput = string.Join(
                Environment.NewLine,
                new[] { output, error }.Where(text => !string.IsNullOrWhiteSpace(text))
            );

            if (process.ExitCode == 0)
            {
                return CommandResult.Ok("Command completed successfully.", combinedOutput);
            }

            return CommandResult.Fail(
                $"Command failed with exit code {process.ExitCode}.",
                combinedOutput
            );
        }
        catch (Exception ex)
        {
            return CommandResult.Fail($"Error running command: {ex.Message}");
        }
    }

    /// <summary>
    /// Starts a visible Windows process, folder, system tool, or settings page.
    /// </summary>
    /// <param name="fileName">The process, file, folder, or URI to open.</param>
    /// <param name="arguments">Optional arguments passed to the process.</param>
    /// <param name="runAsAdmin">Determines whether the process should request administrator permission.</param>
    /// <returns>A CommandResult showing whether the process started successfully.</returns>
    public CommandResult StartProcess(string fileName, string arguments = "", bool runAsAdmin = false)
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = true
            };

            if (runAsAdmin)
            {
                startInfo.Verb = "runas";
            }

            Process.Start(startInfo);

            return CommandResult.Ok($"Started: {fileName}");
        }
        catch (Exception ex)
        {
            return CommandResult.Fail($"Error starting process: {ex.Message}");
        }
    }
}
