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
/// Stores information about a troubleshooting tool that can be displayed and run from the UI.
/// </summary>
public class ToolDefinition
{
    /// <summary>
    /// The internal action that will be executed when the user clicks the tool.
    /// </summary>
    public ToolAction Action { get; set; }

    /// <summary>
    /// The button or card title shown to the user.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// A short explanation of what the tool does.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The text shown inside the action button for this tool.
    /// </summary>
    public string ButtonText { get; set; } = "Run / Open";

    /// <summary>
    /// The UI section where this tool should appear.
    /// </summary>
    public ToolCategory Category { get; set; }

    /// <summary>
    /// Identifies whether the tool needs administrator permission to work correctly.
    /// </summary>
    public bool RequiresAdmin { get; set; }

    /// <summary>
    /// Identifies whether the tool should be treated as advanced or higher risk.
    /// </summary>
    public bool IsAdvanced { get; set; }
}
