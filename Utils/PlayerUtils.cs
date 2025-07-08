using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;

namespace AutoC4Giver.Utils;

public static class PlayerUtils
{
    public static bool IsValidPlayer(CCSPlayerController? player)
    {
        return player != null &&
               player.IsValid &&
               !player.IsBot &&
               player.Connected == PlayerConnectedState.PlayerConnected;
    }

    public static bool IsPlayerAlive(CCSPlayerController player)
    {
        return player.PlayerPawn?.Value != null &&
               player.PlayerPawn.Value.IsValid &&
               player.PlayerPawn.Value.Health > 0 &&
               player.PawnIsAlive;
    }

    public static float GetDistance(Vector pos1, Vector pos2)
    {
        var dx = pos1.X - pos2.X;
        var dy = pos1.Y - pos2.Y;
        var dz = pos1.Z - pos2.Z;

        return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    public static float GetDistance2D(Vector pos1, Vector pos2)
    {
        var dx = pos1.X - pos2.X;
        var dy = pos1.Y - pos2.Y;

        return (float)Math.Sqrt(dx * dx + dy * dy);
    }

    public static void GiveC4ToPlayer(CCSPlayerController player)
    {
        if (!IsValidPlayer(player))
        {
            Debug.DebugWarning($"Attempted to give C4 to invalid player");
            return;
        }

        try
        {
            Debug.DebugInfo("GiveC4", $"Giving C4 to player {player.PlayerName}");

            player.GiveNamedItem(CsItem.Bomb);

            Server.NextFrame(() =>
            {
                if (IsValidPlayer(player))
                {
                    player.ExecuteClientCommand("slot5");
                    Debug.DebugInfo("GiveC4", $"C4 given to {player.PlayerName} and switched to slot 5");
                }
            });
        }
        catch (Exception ex)
        {
            Debug.DebugError($"Error giving C4 to player {player.PlayerName}: {ex.Message}");
        }
    }

    public static List<CCSPlayerController> GetAliveTerrorists(HashSet<CCSPlayerController>? excludePlayers = null)
    {
        excludePlayers ??= new HashSet<CCSPlayerController>();

        return Utilities.GetPlayers()
            .Where(p => IsValidPlayer(p) &&
                        p.Team == CsTeam.Terrorist &&
                        IsPlayerAlive(p) &&
                        !excludePlayers.Contains(p))
            .ToList();
    }

    public static CCSPlayerController? FindNearestAliveTerrorist(Vector position, HashSet<CCSPlayerController>? excludePlayers = null, bool use2D = false)
    {
        var terrorists = GetAliveTerrorists(excludePlayers);

        if (!terrorists.Any())
        {
            Debug.DebugWarning("No eligible terrorists found");
            return null;
        }

        CCSPlayerController? nearestPlayer = null;
        float nearestDistance = float.MaxValue;

        Debug.DebugInfo("FindNearest", $"Looking for terrorists near position: {position.X:F2}, {position.Y:F2}, {position.Z:F2}");
        Debug.DebugInfo("FindNearest", $"Found {terrorists.Count} eligible terrorists");

        foreach (var terrorist in terrorists)
        {
            if (terrorist.PlayerPawn?.Value?.AbsOrigin == null)
                continue;

            var distance = use2D
                ? GetDistance2D(position, terrorist.PlayerPawn.Value.AbsOrigin)
                : GetDistance(position, terrorist.PlayerPawn.Value.AbsOrigin);

            Debug.DebugInfo("FindNearest", $"Player {terrorist.PlayerName} is {distance:F2} units away");

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestPlayer = terrorist;
            }
        }

        if (nearestPlayer != null)
        {
            Debug.DebugInfo("FindNearest", $"Nearest player: {nearestPlayer.PlayerName} at {nearestDistance:F2} units");
        }

        return nearestPlayer;
    }
    public static bool HasC4(CCSPlayerController player)
    {
        if (!IsValidPlayer(player) || player.PlayerPawn?.Value == null)
            return false;

        var weapons = player.PlayerPawn.Value.WeaponServices?.MyWeapons;
        if (weapons == null)
            return false;

        return weapons.Any(weapon => weapon?.Value?.DesignerName == "weapon_c4");
    }

    public static Vector? GetPlayerPosition(CCSPlayerController player)
    {
        if (!IsValidPlayer(player))
            return null;

        return player.PlayerPawn?.Value?.AbsOrigin;
    }

}