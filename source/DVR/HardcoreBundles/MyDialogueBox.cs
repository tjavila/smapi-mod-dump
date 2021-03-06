/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/captncraig/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HardcoreBundles
{
    public class MyResponse : Response
    {
        public readonly Action a;

        public MyResponse(string responseText, Action a) : base("", responseText)
        {
            this.a = a;
        }
    }
    public class MyDialogueBox : DialogueBox
    {
        private readonly IModHelper helper;

        public MyDialogueBox(string dialogue, List<MyResponse> responses, IModHelper helper) : base(dialogue, responses.Select(x => x as Response).ToList(), 1200)
        {
            this.helper = helper;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            var selected = helper.Reflection.GetField<int>(this, "selectedResponse").GetValue();
            var responses = helper.Reflection.GetField<List<Response>>(this, "responses").GetValue().Cast<MyResponse>().ToList();
            if (selected != -1 && responses[selected].a != null)
            {
                responses[selected].a();
            }
            base.receiveLeftClick(x, y, playSound);
        }
    }
}
