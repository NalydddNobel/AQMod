using Aequus.Core.CodeGeneration;
using System;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public float accRabbitFootLuck;
    private static int _rabbitFootRerolls;

    internal void RerollLuckFull(ref int rolled, int range) {
        if (_rabbitFootRerolls == 0) {
            _rabbitFootRerolls++;
            try {
                rolled = RerollLuck(rolled, range);
            }
            catch {
            }
            _rabbitFootRerolls = 0;
        }
    }

    public int RerollLuck(int rolledAmt, int range) {
        for (float luckLeft = accRabbitFootLuck; luckLeft > 0f; luckLeft--) {
            if (luckLeft < 1f) {
                if (Main.rand.NextFloat(1f) > luckLeft) {
                    return rolledAmt;
                }
            }
            rolledAmt = Math.Min(rolledAmt, Player.RollLuck(range));
            if (rolledAmt <= 0) {
                return 0;
            }
        }
        return rolledAmt;
    }
}
