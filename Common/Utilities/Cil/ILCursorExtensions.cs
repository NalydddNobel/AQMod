using MonoMod.Cil;
using System;

namespace Aequus.Common.Utilities.Cil;
public static class ILCursorExtensions {
    public record struct SwitchControlInfo(ILCursor Cursor, ILLabel[] Targets, int Offset) {
        public readonly bool HasCase(int index) {
            index -= Offset;
            return Targets.IndexInRange(index) && Targets[index] is ILLabel;
        }
        public readonly bool TryGoToCase(int index) {
            if (!HasCase(index)) {
                return false;
            }

            Cursor.GotoLabel(Targets[index]);
            return true;
        }
    }

    public static bool TryGoToSwitch(this ILCursor c, Func<SwitchControlInfo, bool> predicate) {
        ILLabel[]? targets = null;

        while (c.TryGotoNext(i => i.MatchSwitch(out targets))) {
            int offset = 0;
            if (c.Prev.MatchSub() && c.Prev.Previous.MatchLdcI4(out offset)) {; }

            SwitchControlInfo info = new SwitchControlInfo(c, targets!, offset);

            if (predicate(info)) {
                return true;
            }
        }

        return false;
    }
}
