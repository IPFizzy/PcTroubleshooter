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
/// Groups troubleshooting tools into sections that can be shown in the user interface.
/// </summary>
public enum ToolCategory
{
    Network,
    WindowsTools,
    Reports,
    Advanced
}
