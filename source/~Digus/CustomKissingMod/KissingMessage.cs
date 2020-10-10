/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomKissingMod
{
    public class KissingMessage
    {
        public long Kisser;
        public long Kissed;

        public KissingMessage(long kisser, long kissed)
        {
            Kisser = kisser;
            Kissed = kissed;
        }
    }
}
