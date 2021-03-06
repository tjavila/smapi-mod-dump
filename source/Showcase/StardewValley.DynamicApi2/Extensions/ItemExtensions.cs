/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

using System.Text;
using StardewValley;

namespace Igorious.StardewValley.DynamicApi2.Extensions
{
    public static class ItemExtensions
    {
        public static string GetInfo(this Item item)
        {
            if (item == null) return "{Empty}";
            return item is Object o
                ? o.GetInfo() 
                : $"{item.Name} [ID={item.parentSheetIndex}, {item.GetType().Name}]";
        }

        private static string GetInfo(this Object o)
        {
            var buffer = new StringBuilder(o.Name);
            if (o.Stack != 1) buffer.Append(" x").Append(o.Stack);
            buffer.Append(" [ID=").Append(o.parentSheetIndex);
            if (o.GetType() != typeof(Object)) buffer.Append(", ").Append(o.GetType().Name);
            buffer.Append("]");
            return buffer.ToString();
        }
    }
}
