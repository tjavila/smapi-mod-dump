/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/JsonAssets
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace JsonAssets.Data
{
    public abstract class DataNeedsIdWithTexture : DataNeedsId
    {
        [JsonIgnore]
        public Texture2D texture;

        // The following is mainly data for the Content Patcher integration.

        [JsonIgnore]
        public string tilesheet;

        [JsonIgnore]
        public int tilesheetX;

        [JsonIgnore]
        public int tilesheetY;
    }
}
