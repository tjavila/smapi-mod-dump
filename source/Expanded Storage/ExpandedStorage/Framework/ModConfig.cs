/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/ExpandedStorage
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ExpandedStorage.Framework
{
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public class ModConfig
    {
        /// <summary>Allow carried chests to be accessed while in inventory.</summary>
        public bool AllowAccessCarriedChest { get; set; } = true;
        
        /// <summary>Allow chests to be picked up and placed with items.</summary>
        public bool AllowCarryingChests { get; set; } = true;

        /// <summary>Whether to allow modded storage to have capacity other than 36 slots.</summary>
        public bool AllowModdedCapacity { get; set; } = true;

        /// <summary>Allows storages to accept specific items.</summary>
        public bool AllowRestrictedStorage { get; set; } = true;

        /// <summary>Allows storages to pull items directly into their inventory.</summary>
        public bool AllowVacuumItems { get; set; } = true;

        /// <summary>Adds three extra rows to the Inventory Menu.</summary>
        public bool ExpandInventoryMenu { get; set; } = true;
        
        /// <summary>Adds clickable arrows to indicate when there are more items in the chest.</summary>
        public bool ShowOverlayArrows { get; set; } = true;

        /// <summary>Allows filtering Inventory Menu by searching for the the item name.</summary>
        public bool ShowSearchBar { get; set; } = true;

        /// <summary>Allows showing tabs in the Chest Menu.</summary>
        public bool ShowTabs { get; set; } = true;

        /// <summary>Control scheme for Expanded Storage features.</summary>
        public ModConfigKeys Controls { get; set; } = new();

        public static void RegisterModConfig(IManifest modManifest, IGenericModConfigMenuAPI modConfigApi, ModConfig config)
        {
            modConfigApi.RegisterLabel(modManifest,
                "Controls",
                "Controller/Keyboard controls");
            modConfigApi.RegisterSimpleOption(modManifest,
                "Scroll Up",
                "Button for scrolling up",
                () => config.Controls.ScrollUp.Keybinds.Single(kb => kb.IsBound).Buttons.First(),
                value => config.Controls.ScrollUp = KeybindList.ForSingle(value));
            modConfigApi.RegisterSimpleOption(modManifest,
                "Scroll Down",
                "Button for scrolling down",
                () => config.Controls.ScrollDown.Keybinds.Single(kb => kb.IsBound).Buttons.First(),
                value => config.Controls.ScrollDown = KeybindList.ForSingle(value));
            modConfigApi.RegisterSimpleOption(modManifest,
                "Previous Tab",
                "Button for switching to the previous tab",
                () => config.Controls.PreviousTab.Keybinds.Single(kb => kb.IsBound).Buttons.First(),
                value => config.Controls.PreviousTab = KeybindList.ForSingle(value));
            modConfigApi.RegisterSimpleOption(modManifest,
                "Next Tab",
                "Button for switching to the next tab",
                () => config.Controls.NextTab.Keybinds.Single(kb => kb.IsBound).Buttons.First(),
                value => config.Controls.NextTab = KeybindList.ForSingle(value));
            modConfigApi.RegisterLabel(modManifest,
                "Global Toggles",
                "Enable/Disable features (restart to revert patches)");
            modConfigApi.RegisterSimpleOption(modManifest,
                "Carry Chest",
                "Uncheck to globally disable carrying chests",
                () => config.AllowCarryingChests,
                value => config.AllowCarryingChests = value);
            modConfigApi.RegisterSimpleOption(modManifest,
                "Access Carried Chest",
                "Uncheck to globally disable accessing chest items for carried chests",
                () => config.AllowAccessCarriedChest,
                value => config.AllowAccessCarriedChest = value);
            modConfigApi.RegisterSimpleOption(modManifest,
                "Expand Inventory Menu",
                "Uncheck to globally disable resizing the inventory menu",
                () => config.ExpandInventoryMenu,
                value => config.ExpandInventoryMenu = value);
            modConfigApi.RegisterSimpleOption(modManifest,
                "Modded Capacity",
                "Uncheck to globally disable non-vanilla capacity (36 item slots)",
                () => config.AllowModdedCapacity,
                value => config.AllowModdedCapacity = value);
            modConfigApi.RegisterSimpleOption(modManifest,
                "Restricted Storage",
                "Uncheck to globally disable allow/block lists for chest items",
                () => config.AllowRestrictedStorage,
                value => config.AllowRestrictedStorage = value);
            modConfigApi.RegisterSimpleOption(modManifest,
                "Show Overlay Arrows",
                "Uncheck to globally disable adding arrow buttons",
                () => config.ShowOverlayArrows,
                value => config.ShowOverlayArrows = value);
            modConfigApi.RegisterSimpleOption(modManifest,
                "Show Search Bar",
                "Uncheck to globally disable carrying chests",
                () => config.ShowSearchBar,
                value => config.ShowSearchBar = value);
            modConfigApi.RegisterSimpleOption(modManifest,
                "Show Tabs",
                "Uncheck to globally disable chest tabs",
                () => config.ShowTabs,
                value => config.ShowTabs = value);
            modConfigApi.RegisterSimpleOption(modManifest,
                "Vacuum Items",
                "Uncheck to globally disable chests picking up items",
                () => config.AllowVacuumItems,
                value => config.AllowVacuumItems = value);
        }
    }
}