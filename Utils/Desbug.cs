using AutoC4Giver.Config;

namespace AutoC4Giver.Utils;

public static class Debug
{
    public static BaseConfigs? Config { get; set; }

    public static void DebugMessage(string message)
    {
        if (Config?.EnableDebug != true) return;
        Console.WriteLine($"================================= [ AutoC4Giver ] =================================");
        Console.WriteLine(message);
        Console.WriteLine("=============================================================================");
    }

    public static void DebugInfo(string category, string message)
    {
        if (Config?.EnableDebug != true) return;
        Console.WriteLine($"================================= [ AutoC4Giver - {category} ] =================================");
        Console.WriteLine(message);
        Console.WriteLine("=============================================================================");
    }

    public static void DebugError(string error)
    {
        if (Config?.EnableDebug != true) return;
        Console.WriteLine($"================================= [ AutoC4Giver - ERROR ] =================================");
        Console.WriteLine($"ERROR: {error}");
        Console.WriteLine("======================================================================================");
    }

    public static void DebugWarning(string warning)
    {
        if (Config?.EnableDebug != true) return;
        Console.WriteLine($"================================= [ AutoC4Giver - WARNING ] =================================");
        Console.WriteLine($"WARNING: {warning}");
        Console.WriteLine("========================================================================================");
    }

    public static void DebugC4(string operation, string message)
    {
        if (Config?.EnableDebug != true) return;
        Console.WriteLine($"================================= [ AutoC4Giver - C4 {operation} ] =================================");
        Console.WriteLine(message);
        Console.WriteLine("======================================================================================");
    }

    public static void DebugPlayer(string playerName, string operation, string message)
    {
        if (Config?.EnableDebug != true) return;
        Console.WriteLine($"================================= [ AutoC4Giver - Player: {playerName} ] =================================");
        Console.WriteLine($"{operation}: {message}");
        Console.WriteLine("=========================================================================================");
    }
}