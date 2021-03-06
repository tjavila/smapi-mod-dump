/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace HatsOnCats.Framework.Offsets
{
    internal class AggregateOffsetProvider
    {
        private readonly IEnumerable<IOffsetProvider> offsetProviders;

        public AggregateOffsetProvider(IEnumerable<IOffsetProvider> offsetProviders)
        {
            this.offsetProviders = offsetProviders;
        }


        public string Name => null;

        public bool GetOffset(string spriteName, Rectangle sourceRectangle, SpriteEffects effects, out Vector2 offset)
        {
            foreach (IOffsetProvider provider in this.offsetProviders)
            {
                if (provider.CanHandle(spriteName))
                {
                    return provider.GetOffset(sourceRectangle, effects, out offset);
                }
            }

            offset = Vector2.Zero;
            return false;
        }
    }
}
