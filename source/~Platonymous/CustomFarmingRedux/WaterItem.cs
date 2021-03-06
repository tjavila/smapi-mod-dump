/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using PyTK.CustomElementHandler;
using StardewValley;
using System.Collections.Generic;

namespace CustomFarmingRedux
{
    class WaterItem : PySObject
    {
        public WaterItem()
        {

        }

        public WaterItem(CustomObjectData data)
            : base(data)
        {
        }

        public override string DisplayName { get => (stack / 10f).ToString() + "l " + base.DisplayName.ToLower(); set => base.DisplayName = value; }

        public override Item getOne()
        {
            return new WaterItem(data) { TileLocation = Vector2.Zero, name = name, Price = price, Quality = quality };
        }

        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            return new WaterItem(CustomObjectData.collection[additionalSaveData["id"]]);
        }
    }
}
