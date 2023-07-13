using Aequus.Common.Buffs;
using Aequus.Common.ModPlayers;
using Aequus.Common.Net.Sounds;
using Aequus.Items.Accessories.Combat.OnHitAbility.BoneRing;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus;

public partial class AequusPlayer {
    public Accessory<IBoneRing> accBoneRing = new();

    private void OnHit_BoneRing(NPC target) {
        int stacks = accBoneRing.EquipmentStacks();
        if (!accBoneRing.TryGetItem(out var item, out var BoneRing)) {
            return;
        }
        AequusBuff.ApplyBuff<BoneRingDebuff>(target, BoneRing.DebuffDuration * stacks, out bool canPlaySound);

        if (canPlaySound) {
            ModContent.GetInstance<WeaknessDebuffSound>().Play(target.Center);
        }
        if (canPlaySound || target.HasBuff<BoneRingDebuff>()) {
            for (int i = 0; i < 12; i++) {
                var v = Main.rand.NextVector2Unit();
                var d = Dust.NewDustPerfect(target.Center + v * new Vector2(Main.rand.NextFloat(target.width / 2f + 16f), Main.rand.NextFloat(target.height / 2f + 16f)), DustID.AncientLight, v * 8f);
                d.noGravity = true;
                d.noLightEmittence = true;
            }
        }
    }
}