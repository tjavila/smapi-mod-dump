/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace Pathoschild.Stardew.NoclipMode.Framework
{
    /// <summary>The mod configuration.</summary>
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The keys which toggle noclip mode.</summary>
        public KeybindList ToggleKey { get; set; } = KeybindList.ForSingle(SButton.F11);

        /// <summary>Whether to show a confirmation message when noclip is enabled.</summary>
        public bool ShowEnabledMessage { get; set; } = true;

        /// <summary>Whether to show a confirmation message when noclip is disabled.</summary>
        public bool ShowDisabledMessage { get; set; } = false;
    }
}
