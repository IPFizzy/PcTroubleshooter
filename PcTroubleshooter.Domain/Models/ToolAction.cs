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
/// Defines the approved tool actions that the application is allowed to run.
/// This works as an allowlist so the UI cannot send arbitrary commands.
/// </summary>
public enum ToolAction
{
    PingGoogle,
    PingGateway,
    ShowNetworkConfig,
    DnsLookup,
    OpenTaskManager,
    OpenDeviceManager,
    OpenEventViewer,
    OpenWindowsUpdate,
    OpenDiskCleanup,
    CreateBasicPcReport,
    OpenReportFolder,
    OpenChrisTitusWinUtil
}
