/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/doncollins/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValleyMods.CategorizeChests.Interface.Widgets;

namespace StardewValleyMods.CategorizeChests.Interface
{
    class TooltipManager : ITooltipManager
    {
        private Widget Tooltip;

        public void ShowTooltipThisFrame(Widget tooltip)
        {
            Tooltip = tooltip;
        }

        public void Draw(SpriteBatch batch)
        {
            if (Tooltip != null)
            {
                var mousePosition = Game1.getMousePosition();

                Tooltip.Position = new Point(
                    mousePosition.X + 8 * Game1.pixelZoom,
                    mousePosition.Y + 8 * Game1.pixelZoom
                );

                Tooltip.Draw(batch);

                Tooltip = null;
            }
        }
    }
}