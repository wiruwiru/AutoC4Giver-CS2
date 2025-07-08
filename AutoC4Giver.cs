using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Attributes;
using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;

using AutoC4Giver.Config;
using AutoC4Giver.Utils;

namespace AutoC4Giver;

[MinimumApiVersion(305)]
public class AutoC4Giver : BasePlugin, IPluginConfig<BaseConfigs>
{
	public override string ModuleName => "AutoC4Giver";
	public override string ModuleVersion => "1.0.0";
	public override string ModuleAuthor => "luca.uy";
	public override string ModuleDescription => "Automatically transfers the C4 to a nearby alive teammate if it's dropped shortly after spawn";

	public required BaseConfigs Config { get; set; }

	private Timer? _spawnTimer;
	private bool _isInSpawnPeriod = false;
	private readonly HashSet<CCSPlayerController> _playersWhoDroppedC4 = new();

	public void OnConfigParsed(BaseConfigs config)
	{
		Config = config;
		Debug.Config = config;
	}

	public override void Load(bool hotReload)
	{
		Debug.DebugMessage("Plugin loaded successfully!");
		Debug.DebugInfo("Config", $"Spawn Duration: {Config.SpawnDuration}s");
		Debug.DebugInfo("Config", $"Transfer Delay: {Config.TransferDelay}s");
		Debug.DebugInfo("Config", $"Debug Enabled: {Config.EnableDebug}");
	}

	[GameEventHandler]
	public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
	{
		Debug.DebugInfo("RoundStart", "Round started, initializing spawn period timer");

		_isInSpawnPeriod = true;
		_playersWhoDroppedC4.Clear();
		_spawnTimer?.Kill();

		_spawnTimer = AddTimer(Config.SpawnDuration, () =>
		{
			_isInSpawnPeriod = false;
			Debug.DebugInfo("Timer", $"Spawn period ended after {Config.SpawnDuration} seconds");
		});

		return HookResult.Continue;
	}

	[GameEventHandler]
	public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
	{
		Debug.DebugInfo("RoundEnd", "Round ended, cleaning up");

		_spawnTimer?.Kill();
		_spawnTimer = null;
		_isInSpawnPeriod = false;
		_playersWhoDroppedC4.Clear();

		return HookResult.Continue;
	}

	[GameEventHandler]
	public HookResult OnItemPickup(EventItemPickup @event, GameEventInfo info)
	{
		var player = @event.Userid;
		var item = @event.Item;

		if (!PlayerUtils.IsValidPlayer(player) || item != "weapon_c4" || player?.Team != CsTeam.Terrorist)
			return HookResult.Continue;

		Debug.DebugInfo("ItemPickup", $"Terrorist {player?.PlayerName} picked up C4");

		if (player != null)
		{
			_playersWhoDroppedC4.Remove(player);
		}

		return HookResult.Continue;
	}

	[GameEventHandler]
	public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
	{
		var player = @event.Userid;
		if (player != null && player.Team == CsTeam.Terrorist)
		{
			_playersWhoDroppedC4.Remove(player);
		}

		return HookResult.Continue;
	}

	[GameEventHandler]
	public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
	{
		var player = @event.Userid;
		if (player != null && player.Team == CsTeam.Terrorist)
		{
			_playersWhoDroppedC4.Remove(player);
		}

		Debug.DebugInfo("PlayerDeath", $"Terrorist {player?.PlayerName} died - C4 will remain on ground if dropped");

		return HookResult.Continue;
	}

	private void CheckForDroppedC4()
	{
		if (!_isInSpawnPeriod)
			return;

		Debug.DebugInfo("CheckDroppedC4", "Checking for dropped C4 on the ground");

		var c4Entities = Utilities.FindAllEntitiesByDesignerName<CC4>("weapon_c4");
		foreach (var c4 in c4Entities)
		{
			if (c4?.OwnerEntity?.Value == null && c4?.AbsOrigin != null)
			{
				Debug.DebugInfo("CheckDroppedC4", "Found C4 on the ground, looking for recipient");

				var nearestTerrorist = PlayerUtils.FindNearestAliveTerrorist(c4.AbsOrigin, _playersWhoDroppedC4);
				if (nearestTerrorist != null)
				{
					Debug.DebugInfo("CheckDroppedC4", $"Transferring C4 to {nearestTerrorist.PlayerName}");

					c4.Remove();

					PlayerUtils.GiveC4ToPlayer(nearestTerrorist);
					nearestTerrorist.PrintToChat($"{Localizer["prefix"]} {Localizer["autoc4giver.c4_received"]}");

					Debug.DebugInfo("CheckDroppedC4", $"C4 successfully transferred to {nearestTerrorist.PlayerName}");
				}
				else
				{
					Debug.DebugWarning("No eligible terrorists found to transfer C4 to (excluding players who dropped it)");
				}

				break;
			}
		}
	}

	[GameEventHandler]
	public HookResult OnBombDropped(EventBombDropped @event, GameEventInfo info)
	{
		var player = @event.Userid;
		if (!PlayerUtils.IsValidPlayer(player))
			return HookResult.Continue;

		Debug.DebugInfo("BombDropped", $"Player {player?.PlayerName} dropped the bomb");

		if (player != null && _isInSpawnPeriod)
		{
			_playersWhoDroppedC4.Add(player);
			Debug.DebugInfo("BombDropped", $"Added {player.PlayerName} to exclusion list for C4 transfer");

			AddTimer(Config.TransferDelay, CheckForDroppedC4);
		}

		return HookResult.Continue;
	}

	public override void Unload(bool hotReload)
	{
		_spawnTimer?.Kill();
		_playersWhoDroppedC4.Clear();
		Debug.DebugMessage("Plugin unloaded");
	}
}