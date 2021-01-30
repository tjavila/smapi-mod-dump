/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/iTile
**
*************************************************/

using StardewModdingAPI;

namespace iTile.Core
{
    public abstract class Manager : IInitializable
    {
        public IModHelper Helper
            => iTile.Instance.Helper;

        public virtual void Init()
        {
        }
    }
}