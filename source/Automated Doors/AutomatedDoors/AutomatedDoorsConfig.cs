/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/azah/AutomatedDoors
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley.Buildings;
using StardewValley;
using StardewModdingAPI;
using System.Collections.Generic;
using System;

namespace AutomatedDoors
{
    public class AutomatedDoorsConfig
    {
        public int TimeDoorsOpen { get; set; } = 620;
        public int TimeDoorsClose { get; set; } = 1810;
        public bool OpenOnRainyDays { get; set; } = false;
        public bool OpenInWinter { get; set; } = false;
        public Dictionary<string, Dictionary<string, bool>> Buildings { get; set; } = new Dictionary<string, Dictionary<string, bool>>();
    }
}
