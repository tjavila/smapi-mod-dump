/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/ExpandedStorage
**
*************************************************/

using System.Collections;
using System.Collections.Generic;
using Harmony;
using StardewModdingAPI;

namespace Common.HarmonyPatches
{
    public class PatternPatches : IEnumerable<CodeInstruction>
    {
        private static IMonitor _monitor;
        private readonly Queue<PatternPatch> _patternPatches = new Queue<PatternPatch>();
        private readonly IEnumerable<CodeInstruction> _instructions;
        public bool Done => _patternPatches.Count == 0;
        public PatternPatches(IEnumerable<CodeInstruction> instructions, IMonitor monitor)
        {
            _instructions = instructions;
            _monitor = monitor;
        }
        public PatternPatch Find(params CodeInstruction[] pattern)
        {
            var operation = new PatternPatch(pattern);
            _patternPatches.Enqueue(operation);
            return operation;
        }
        public IEnumerator<CodeInstruction> GetEnumerator()
        {
            var currentOperation = _patternPatches.Dequeue();
            var rawStack = new LinkedList<CodeInstruction>();
            var skipped = 0;
            var done = false;

            foreach (var instruction in _instructions)
            {
                // Skipped instructions
                if (skipped > 0)
                {
                    skipped--;
                    continue;
                }

                // Pattern does not match or done matching patterns
                if (done || !currentOperation.Matches(instruction))
                {
                    rawStack.AddLast(instruction);
                    continue;
                }
                
                // Return patched code
                if (currentOperation.Text != null)
                    _monitor.Log(currentOperation.Text);
                rawStack.AddLast(instruction);
                currentOperation.Patches(rawStack);
                foreach (var patch in rawStack)
                {
                    yield return patch;
                }
                rawStack.Clear();
                skipped = currentOperation.Skipped;
                
                // Repeat
                if (currentOperation.Loop)
                    continue;
                
                // Next pattern
                if (_patternPatches.Count > 0)
                    currentOperation = _patternPatches.Dequeue();
                else
                    done = true;
            }

            foreach (var instruction in rawStack)
            {
                yield return instruction;
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}