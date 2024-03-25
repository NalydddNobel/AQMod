using Aequus.Common.Buffs.Components;
using Aequus.Common.NPCs;
using Aequus.DataSets;
using Terraria.Audio;

namespace Aequus.Old.Content.Equipment.Accessories.OnHitDebuffs;

public class BoneRingDebuff : ModBuff, IOnAddBuff {
    public override string Texture => AequusTextures.Buff(BuffID.Weak);

    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
        Main.buffNoSave[Type] = true;
        BuffSets.GrantImmunityWith[Type].Add(BuffID.Slow);
        BuffDataSet.PlayerStatusDebuff.Add(Type);
    }

    public override void Update(NPC npc, ref int buffIndex) {
        if (npc.TryGetGlobalNPC(out BoneRingDebuffGlobalNPC boneRingNPC)) {
            boneRingNPC.debuffBoneRing = true;
        }

        if (npc.TryGetGlobalNPC(out AequusNPC aequusNPC)) {
            aequusNPC.statSpeedX *= BoneRing.MovementSpeedMultiplier;
            aequusNPC.statSpeedY *= BoneRing.MovementSpeedMultiplier;
        }
    }

    public void PostAddBuff(NPC npc, bool alreadyHasBuff, int duration, bool quiet) {
        if (alreadyHasBuff || !npc.HasBuff<BoneRingDebuff>()) {
            return;
        }

        SoundEngine.PlaySound(AequusSounds.InflictWeakness, npc.Center);

        for (int i = 0; i < 12; i++) {
            var v = Main.rand.NextVector2Unit();
            var d = Dust.NewDustPerfect(npc.Center + v * new Vector2(Main.rand.NextFloat(npc.width / 2f + 16f), Main.rand.NextFloat(npc.height / 2f + 16f)), DustID.AncientLight, v * 8f);
            d.noGravity = true;
            d.noLightEmittence = true;
        }
    }
}