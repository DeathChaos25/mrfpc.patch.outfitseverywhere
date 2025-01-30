using mrfpc.patch.outfitseverywhere.Configuration;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using System.Diagnostics;
using Reloaded.Memory;
using Reloaded.Memory.Interfaces;

namespace mrfpc.patch.outfitseverywhere;
internal unsafe class Utils
{
    private static ILogger _logger;
    private static Config _config;
    private static IStartupScanner _startupScanner;
    internal static nint BaseAddress { get; private set; }

    internal static bool DEBUG = false;

    internal static bool Initialise(ILogger logger, Config config, IModLoader modLoader)
    {
        _logger = logger;
        _config = config;
        using var thisProcess = Process.GetCurrentProcess();
        BaseAddress = thisProcess.MainModule!.BaseAddress;

        var startupScannerController = modLoader.GetController<IStartupScanner>();
        if (startupScannerController == null || !startupScannerController.TryGetTarget(out _startupScanner))
        {
            LogError($"Unable to get controller for Reloaded SigScan Library, stuff won't work :(");
            return false;
        }

        return true;
    }

    internal static void LogDebug(string message)
    {
        if (DEBUG)
            _logger.WriteLineAsync($"[Outfits Everywhere] {message}");
    }

    internal static void Log(string message)
    {
        _logger.WriteLineAsync($"[Outfits Everywhere] {message}");
    }

    internal static void LogError(string message, Exception e)
    {
        _logger.WriteLineAsync($"[Outfits Everywhere] {message}: {e.Message}", System.Drawing.Color.Red);
    }

    internal static void LogError(string message)
    {
        _logger.WriteLineAsync($"[Outfits Everywhere] {message}", System.Drawing.Color.Red);
    }

    internal static void SigScan(string pattern, string name, Action<nint> action)
    {
        if (pattern != null)
        {
            _startupScanner.AddMainModuleScan(pattern, result =>
            {
                if (!result.Found)
                {
                    LogError($"1 or more Signature scan has failed, make sure you are using latest game version/update");
                    action(-1);
                    return;
                }
                LogDebug($"Found {name} at 0x{result.Offset + BaseAddress:X}");

                action(result.Offset + BaseAddress);
            });
        }
    }

    /// <summary>
    /// Gets the address of a global from something that references it
    /// </summary>
    /// <param name="ptrAddress">The address to the pointer to the global (like in a mov instruction or something)</param>
    /// <returns>The address of the global</returns>
    internal static unsafe nuint GetGlobalAddress(nint ptrAddress)
    {
        return (nuint)((*(int*)ptrAddress) + ptrAddress + 4);
    }

    internal static unsafe int ReplaceFilePathWithMod(nint target, string newString)
    {
        // Get an instance of Reloaded-II's memory patching module
        var memory = Memory.Instance;

        // Convert the string to a byte array (ANSI encoding)
        byte[] strBytes = System.Text.Encoding.ASCII.GetBytes(newString);

        // Write the string bytes to the target memory location
        memory.SafeWrite((nuint)target, strBytes);

        // Write the null terminator at the end
        memory.SafeWrite((nuint)(target + strBytes.Length), new byte[] { 0 });

        return strBytes.Length + 1;
    }

}
