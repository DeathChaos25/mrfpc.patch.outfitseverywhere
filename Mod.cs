using Reloaded.Hooks.Definitions;
using Reloaded.Mod.Interfaces;
using mrfpc.patch.outfitseverywhere.Configuration;
using mrfpc.patch.outfitseverywhere.Template;
using static mrfpc.patch.outfitseverywhere.Utils;
using static mrfpc.patch.outfitseverywhere.UnitOutfitsRandom;
using IReloadedHooks = Reloaded.Hooks.ReloadedII.Interfaces.IReloadedHooks;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions.X64;
using System.Reflection;
using Reloaded.Memory;
using Reloaded.Memory.Interfaces;

namespace mrfpc.patch.outfitseverywhere
{
    /// <summary>
    /// Your mod logic goes here.
    /// </summary>
    public class Mod : ModBase // <= Do not Remove.
    {
        /// <summary>
        /// Provides access to the mod loader API.
        /// </summary>
        private readonly IModLoader _modLoader;

        /// <summary>
        /// Provides access to the Reloaded.Hooks API.
        /// </summary>
        /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
        private readonly IReloadedHooks? _hooks;

        /// <summary>
        /// Provides access to the Reloaded logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Entry point into the mod, instance that created this class.
        /// </summary>
        private readonly IMod _owner;

        /// <summary>
        /// Provides access to this mod's configuration.
        /// </summary>
        private Config _configuration;

        /// <summary>
        /// The configuration of the currently executing mod.
        /// </summary>
        private readonly IModConfig _modConfig;

        private unsafe delegate nint CalculateModelIDForCostumeDelegate(nint a1, short unit_type, short* player_id, ushort* model_id);

        private unsafe delegate bool isPlayerUnitAvailableDelegate(int player_id);

        private IHook<CalculateModelIDForCostumeDelegate> _calculateModelIDForCostumeHook;

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        private delegate nint getDatUnitFromIDDelegate(uint id);

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        private delegate uint datGetEquipItemDelegate(nint datUnit, ushort item_slot);

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        private delegate ushort GetCostumeModelIDFromItemDelegate(ushort item_id);

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        private delegate ushort GetBagItemDelegate(nint item_id);

        [Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        private delegate void SetBagItemDelegate(nint item_id, byte amount);

        private getDatUnitFromIDDelegate getDatUnitFromID;
        private datGetEquipItemDelegate datGetEquipItem;
        private GetCostumeModelIDFromItemDelegate GetCostumeModelIDFromItem;
        private GetBagItemDelegate GetBagItem;
        private SetBagItemDelegate SetBagItem;
        private isPlayerUnitAvailableDelegate isPlayerUnitAvailable;

        private ushort[] rndCostume = new ushort[22];
        private bool[] isAvailable = new bool[22];

        public Mod(ModContext context)
        {
            _modLoader = context.ModLoader;
            _hooks = context.Hooks;
            _logger = context.Logger;
            _owner = context.Owner;
            _configuration = context.Configuration;
            _modConfig = context.ModConfig;

            Initialise(_logger, _configuration, _modLoader);


            SigScan("48 63 C1 48 69 C8 A8 16 00 00 48 8B 05 ?? ?? ?? ?? 48 05 28 02 00 00 48 03 C1", "getDatUnitFromID", address =>
            {
                getDatUnitFromID = _hooks.CreateWrapper<getDatUnitFromIDDelegate>(address, out _);
            });

            SigScan("0F B7 C2 8B 84 ?? ?? ?? ?? ??", "datGetEquipItem", address =>
            {
                datGetEquipItem = _hooks.CreateWrapper<datGetEquipItemDelegate>(address, out _);
            });

            SigScan("4C 8B DC 48 83 EC 68 33 C0", "GetModelIDFromCostumeTBL", address =>
            {
                var funcAddress = GetGlobalAddress(address + 0x24);
                LogDebug($"Found GetModelIDFromCostumeTBL at 0x{funcAddress:X}");
                GetCostumeModelIDFromItem = _hooks.CreateWrapper<GetCostumeModelIDFromItemDelegate>((long)funcAddress, out _);
            });

            SigScan("E8 ?? ?? ?? ?? 03 E8 89 74 24 ??", "GetBagItem", address =>
            {
                var funcAddress = GetGlobalAddress(address + 1);
                LogDebug($"Found GetBagItem at 0x{funcAddress:X}");
                GetBagItem = _hooks.CreateWrapper<GetBagItemDelegate>((long)funcAddress, out _);
            });

            SigScan("49 89 E3 49 89 5B ?? 49 89 73 ?? 89 4C 24 ??", "SetBagItem", address =>
            {
                SetBagItem = _hooks.CreateWrapper<SetBagItemDelegate>(address, out _);
            });

            SigScan("48 83 EC 28 8D 41 ?? 83 F8 12", "isPlayerUnitAvailable", address =>
            {
                isPlayerUnitAvailable = _hooks.CreateWrapper<isPlayerUnitAvailableDelegate>(address, out _);
            });

            unsafe
            {
                SigScan("48 89 5C 24 ?? 56 57 41 56 48 81 EC 70 01 00 00", "datGetCostumeModelID", address =>
                {
                    _calculateModelIDForCostumeHook = _hooks.CreateHook<CalculateModelIDForCostumeDelegate>(DatCalculateModelIDForCostume, address).Activate();
                });
            }
        }

        unsafe nint DatCalculateModelIDForCostume(nint a1, short unit_type, short* player_id, ushort* model_id)
        {
            nint result = _calculateModelIDForCostumeHook.OriginalFunction(a1, unit_type, player_id, model_id);

            short major_id = *player_id;
            ushort minor_id = *model_id;

            if (isPlayerUnit_ReplaceModeValid(unit_type, major_id, minor_id))
            {
                ushort modelID = GetReplacementModelID(major_id);

                if (modelID > 1)
                {
                    *model_id = modelID;
                    LogDebug($"Model for player {major_id:D2} successfully redirected to {modelID:D3}");
                }
            }
            else if (unit_type == 0xe) // env load
            {
                GiveCostumeItems();
                RollRandomOutfits();
            }

            return result;
        }

        private bool isPlayerUnit_ReplaceModeValid(short unit_type, short major_id, ushort minor_id)
        {
            return (unit_type == 1 && isValidDLCCharacter((uint)major_id)) ||
                   (unit_type <= 5 && minor_id == 1 && isValidDLCCharacter((uint)major_id) && _configuration.FieldOutfits) ||
                   (unit_type == 0x23 && minor_id == 1 && isValidDLCCharacter((uint)major_id) && _configuration.EventOutfits);
        }

        private ushort GetReplacementModelID(short major_id)
        {
            if (_configuration.RandomOutfits)
            {
                return rndCostume[major_id];
            }
            else
            {
                ushort itemID = GetCostumeItemID(major_id);
                return GetCostumeModelIDFromItem(itemID);
            }
        }

        private ushort GetCostumeItemID(short major_id)
        {
            ushort itemID = (ushort)(datGetEquipItem(getDatUnitFromID((uint)major_id), 4) - 0x6000);
            if (major_id == 20)
            {
                itemID = (ushort)(datGetEquipItem(getDatUnitFromID(1), 4) - 0x6000);
            }
            return itemID;
        }

        bool isValidDLCCharacter(uint character_id)
        {
            if (character_id >= 1 && character_id <= 18)
            {
                return isPlayerUnitAvailable((int)character_id);
            }
            else if (character_id == 20)
            {
                return true;
            }
            else return false;
        }

        private void GiveCostumeItems()
        {
            for (int i = 0; i < 10; i++) // new costumes
            {
                int itemID = 0x6000 + 92 + i;
                if (GetBagItem(itemID) == 0) SetBagItem(itemID, 1);
            }
        }

        private void RollRandomOutfits()
        {
            for (int i = 1; i <= 20; i++)
            {
                rndCostume[i] = (ushort)GetRandomCostume(i);
            }
        }

        #region Standard Overrides
        public override void ConfigurationUpdated(Config configuration)
        {
            // Apply settings from configuration.
            // ... your code here.
            _configuration = configuration;
            _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
        }
        #endregion

        #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Mod() { }
#pragma warning restore CS8618
        #endregion
    }
}