/*
 * Keon Bushman
 * PC Troubleshooter
 * Local Windows Troubleshooting Dashboard
 * Created: 2026
 *
 * This file is part of a local troubleshooting utility designed to run
 * approved diagnostic and support tools on a Windows computer with permission.
 */

using System.Text.Json.Serialization;
using PcTroubleshooter.Domain.Models;

namespace PcTroubleshooter.Web.Json;

/// <summary>
/// Provides JSON serialization metadata for published and trimmed builds.
/// </summary>
[JsonSerializable(typeof(List<ToolDefinition>))]
[JsonSerializable(typeof(ToolDefinition))]
[JsonSerializable(typeof(CommandResult))]
public partial class PcTroubleshooterJsonContext : JsonSerializerContext
{
}
