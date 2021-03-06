/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using StardewValley.Buildings;

namespace CustomWarpLocations.WarpOverrides
{
    internal class ObeliskWarpOverride : WarpOverride
    {
        private readonly string obeliskType;

        internal ObeliskWarpOverride(object target)
        {
            this.obeliskType = ((Building)target).buildingType.Value;
        }

        internal override WarpLocation GetWarpLocation()
        {
            switch (this.obeliskType)
            {
                case "Earth Obelisk":
                    return WarpLocations.MountainWarpLocation_Obelisk;
                case "Water Obelisk":
                    return WarpLocations.BeachWarpLocation_Obelisk;
                default:
                case "Desert Obelisk":
                    return WarpLocations.DesertWarpLocation_Obelisk;
            }
        }
    }
}
