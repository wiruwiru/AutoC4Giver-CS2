using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace AutoC4Giver.Config;

public class BaseConfigs : BasePluginConfig
{
    [JsonPropertyName("SpawnDuration")]
    public int SpawnDuration { get; set; } = 20;

    [JsonPropertyName("TransferDelay")]
    public float TransferDelay { get; set; } = 0.5f;

    [JsonPropertyName("EnableDebug")]
    public bool EnableDebug { get; set; } = false;

    [JsonPropertyName("ConfigVersion")]
    public override int Version { get; set; } = 1;
}