using Aequus.Core.CodeGeneration;
using Aequus.Old.Content.Items.Accessories.OnHitDebuffs;
using System.Collections.Generic;
using System.Linq;
using Terraria.Audio;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public int accBlackPhial;

    private void ProcBlackPhial(NPC target) {
        if (accBlackPhial <= 0 || TimerActive(BlackPhial.COOLDOWN_KEY)) {
            return;
        }

        List<int> quickList = BlackPhial.DebuffsAfflicted.Where(buffId => !target.buffImmune[buffId] && !target.HasBuff(buffId)).ToList();
        if (quickList.Count <= 0) {
            return;
        }

        int chosenDebuff = Main.rand.Next(quickList);
        target.AddBuff(chosenDebuff, BlackPhial.DebuffDuration * accBlackPhial);

        if (target.HasBuff(chosenDebuff)) {
            SetTimer(BlackPhial.COOLDOWN_KEY, BlackPhial.CooldownDuration);
            SoundEngine.PlaySound(AequusSounds.InflictBlackPhial with { Volume = 0.33f, }, target.Center);
        }
    }
}