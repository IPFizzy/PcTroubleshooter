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
using PcTroubleshooter.Domain.Models;

namespace PcTroubleshooter.Application.Services;

/// <summary>
/// Contains the main business logic for listing and running approved troubleshooting tools.
/// </summary>
public class ToolService : IToolService
{
    private readonly IProcessRunner _processRunner;
    private readonly string _reportFolder;

    /// <summary>
    /// Initializes the tool service and creates the troubleshooting report folder if needed.
    /// </summary>
    /// <param name="processRunner">The process runner used to execute Windows commands and tools.</param>
    public ToolService(IProcessRunner processRunner)
    {
        _processRunner = processRunner;

        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        _reportFolder = Path.Combine(desktop, "TroubleshootingReports");

        Directory.CreateDirectory(_reportFolder);
    }

    /// <summary>
    /// Gets the approved list of tools that can appear in the web dashboard.
    /// </summary>
    /// <returns>A list of tool definitions grouped by category.</returns>
    public List<ToolDefinition> GetAvailableTools()
    {
        return new List<ToolDefinition>
        {
            new ToolDefinition
            {
                Action = ToolAction.PingGoogle,
                DisplayName = "Ping Google",
                Description = "Checks basic internet connectivity and DNS resolution.",
                ButtonText = "Run Check",
                Category = ToolCategory.Network
            },
            new ToolDefinition
            {
                Action = ToolAction.PingGateway,
                DisplayName = "Ping Gateway",
                Description = "Checks whether the computer can reach the local router.",
                ButtonText = "Run Check",
                Category = ToolCategory.Network
            },
            new ToolDefinition
            {
                Action = ToolAction.ShowNetworkConfig,
                DisplayName = "Show Network Config",
                Description = "Displays IP, DNS, adapter, and gateway information.",
                ButtonText = "Show Config",
                Category = ToolCategory.Network
            },
            new ToolDefinition
            {
                Action = ToolAction.DnsLookup,
                DisplayName = "DNS Lookup",
                Description = "Checks whether DNS can resolve google.com.",
                ButtonText = "Run Check",
                Category = ToolCategory.Network
            },
            new ToolDefinition
            {
                Action = ToolAction.OpenTaskManager,
                DisplayName = "Open Task Manager",
                Description = "Opens Windows Task Manager.",
                ButtonText = "Open Tool",
                Category = ToolCategory.WindowsTools
            },
            new ToolDefinition
            {
                Action = ToolAction.OpenDeviceManager,
                DisplayName = "Open Device Manager",
                Description = "Opens Device Manager.",
                ButtonText = "Open Tool",
                Category = ToolCategory.WindowsTools
            },
            new ToolDefinition
            {
                Action = ToolAction.OpenEventViewer,
                DisplayName = "Open Event Viewer",
                Description = "Opens Windows Event Viewer.",
                ButtonText = "Open Tool",
                Category = ToolCategory.WindowsTools
            },
            new ToolDefinition
            {
                Action = ToolAction.OpenWindowsUpdate,
                DisplayName = "Open Windows Update",
                Description = "Opens the Windows Update settings page.",
                ButtonText = "Open Settings",
                Category = ToolCategory.WindowsTools
            },
            new ToolDefinition
            {
                Action = ToolAction.OpenDiskCleanup,
                DisplayName = "Open Disk Cleanup",
                Description = "Opens the built-in Windows Disk Cleanup tool.",
                ButtonText = "Open Tool",
                Category = ToolCategory.WindowsTools
            },
            new ToolDefinition
            {
                Action = ToolAction.CreateBasicPcReport,
                DisplayName = "Create Basic PC Report",
                Description = "Creates a local report on the desktop with basic PC, disk, and network information.",
                ButtonText = "Create Report",
                Category = ToolCategory.Reports
            },
            new ToolDefinition
            {
                Action = ToolAction.OpenReportFolder,
                DisplayName = "Open Report Folder",
                Description = "Opens the local report folder on the desktop.",
                ButtonText = "Open Folder",
                Category = ToolCategory.Reports
            },
            new ToolDefinition
            {
                Action = ToolAction.OpenChrisTitusWinUtil,
                DisplayName = "Open Chris Titus WinUtil",
                Description = "Opens PowerShell with the WinUtil command. Use carefully.",
                ButtonText = "Open Admin Tool",
                Category = ToolCategory.Advanced,
                RequiresAdmin = true,
                IsAdvanced = true
            }
        };
    }

    /// <summary>
    /// Runs one approved troubleshooting tool based on the selected action.
    /// </summary>
    /// <param name="action">The selected tool action from the dashboard.</param>
    /// <returns>A CommandResult containing success status, messages, and output.</returns>
    public async Task<CommandResult> RunToolAsync(ToolAction action)
    {
        return action switch
        {
            ToolAction.PingGoogle => await _processRunner.RunCommandAsync("cmd.exe", "/c ping google.com"),
            ToolAction.PingGateway => await PingGatewayAsync(),
            ToolAction.ShowNetworkConfig => await _processRunner.RunCommandAsync("cmd.exe", "/c ipconfig /all"),
            ToolAction.DnsLookup => await _processRunner.RunCommandAsync("cmd.exe", "/c nslookup google.com"),

            ToolAction.OpenTaskManager => _processRunner.StartProcess("taskmgr.exe"),
            ToolAction.OpenDeviceManager => _processRunner.StartProcess("devmgmt.msc"),
            ToolAction.OpenEventViewer => _processRunner.StartProcess("eventvwr.msc"),
            ToolAction.OpenWindowsUpdate => _processRunner.StartProcess("ms-settings:windowsupdate"),
            ToolAction.OpenDiskCleanup => _processRunner.StartProcess("cleanmgr.exe"),

            ToolAction.CreateBasicPcReport => await CreateBasicPcReportAsync(),
            ToolAction.OpenReportFolder => _processRunner.StartProcess(_reportFolder),

            ToolAction.OpenChrisTitusWinUtil => OpenChrisTitusWinUtil(),

            _ => CommandResult.Fail("Unknown tool action.")
        };
    }

    /// <summary>
    /// Finds the default gateway and pings it to test local router connectivity.
    /// </summary>
    /// <returns>A CommandResult with the gateway ping output.</returns>
    private async Task<CommandResult> PingGatewayAsync()
    {
        CommandResult gatewayResult = await _processRunner.RunCommandAsync(
            "powershell.exe",
            "-NoProfile -Command \"(Get-NetRoute -DestinationPrefix '0.0.0.0/0' | Select-Object -First 1).NextHop\""
        );

        string gateway = gatewayResult.Output.Trim();

        if (string.IsNullOrWhiteSpace(gateway))
        {
            return CommandResult.Fail("No default gateway was found.");
        }

        CommandResult pingResult = await _processRunner.RunCommandAsync("cmd.exe", $"/c ping {gateway}");
        pingResult.Message = $"Gateway: {gateway}";

        return pingResult;
    }

    /// <summary>
    /// Creates a local troubleshooting report with basic computer, disk, network, and error information.
    /// </summary>
    /// <returns>A CommandResult with the created report path.</returns>
    private async Task<CommandResult> CreateBasicPcReportAsync()
    {
        string reportPath = Path.Combine(_reportFolder, $"Basic_PC_Report_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

        string script = $"""
        $report = "{reportPath}"

        "Basic PC Report" | Out-File $report
        "Generated: $(Get-Date)" | Out-File $report -Append
        "" | Out-File $report -Append

        "=== Computer Info ===" | Out-File $report -Append
        Get-ComputerInfo |
            Select-Object CsName, WindowsProductName, WindowsVersion, OsBuildNumber, CsManufacturer, CsModel, CsTotalPhysicalMemory |
            Format-List |
            Out-File $report -Append

        "=== Disk Space ===" | Out-File $report -Append
        Get-PSDrive -PSProvider FileSystem |
            Select-Object Name, Used, Free |
            Format-Table |
            Out-File $report -Append

        "=== Network Config ===" | Out-File $report -Append
        ipconfig /all | Out-File $report -Append

        "=== Recent System Errors ===" | Out-File $report -Append
        Get-EventLog -LogName System -EntryType Error -Newest 20 |
            Select-Object TimeGenerated, Source, EventID, Message |
            Format-List |
            Out-File $report -Append
        """;

        string encodedCommand = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(script));

        CommandResult result = await _processRunner.RunCommandAsync(
            "powershell.exe",
            $"-NoProfile -EncodedCommand {encodedCommand}"
        );

        if (!result.Success)
        {
            return result;
        }

        _processRunner.StartProcess("notepad.exe", $"\"{reportPath}\"");

        return CommandResult.Ok(
            "Basic PC report created and opened.",
            $"Report saved to: {reportPath}",
            reportPath
        );
    }

    /// <summary>
    /// Opens Chris Titus Tech's WinUtil in a visible administrator PowerShell window.
    /// </summary>
    /// <returns>A CommandResult showing whether PowerShell was opened successfully.</returns>
    private CommandResult OpenChrisTitusWinUtil()
    {
        string command = "irm christitus.com/win | iex";

        return _processRunner.StartProcess(
            "powershell.exe",
            $"-NoExit -Command \"{command}\"",
            runAsAdmin: true
        );
    }
}
