/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/ToolGeodes
**
*************************************************/

using StardewValley;

namespace ToolGeodes
{
    public static class Extensions
    {
        public static int HasAdornment(this Farmer player, ToolType tool, int adornment)
        {
            //var data = Mod.Data;
            //if (player != Game1.player)
            //    data = Mod.instance.Helper.Data.ReadSaveData<SaveData>("spacechase0.ToolGeodes." + player.UniqueMultiplayerID) ?? new SaveData();

            int[] ids = null;
            if (tool == ToolType.Weapon) ids = Mod.Data.WeaponGeodes;
            if (tool == ToolType.Pickaxe) ids = Mod.Data.PickaxeGeodes;
            if (tool == ToolType.Axe) ids = Mod.Data.AxeGeodes;
            if (tool == ToolType.WateringCan) ids = Mod.Data.WaterCanGeodes;
            if (tool == ToolType.Hoe) ids = Mod.Data.HoeGeodes;

            int ret = 0;
            if (ids != null)
            {
                foreach (var id in ids)
                {
                    if (id == adornment)
                        ++ret;
                }
            }

            return ret;
        }
    }
}
