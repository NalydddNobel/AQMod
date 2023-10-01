using Aequus;
using Aequus.Common.Buffs.Components;
using Aequus.Common.DataSets;
using Aequus.Common.NPCs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Weapons.Classless.StunGun;

public class StunGunDebuff : ModBuff, IOnAddBuff/*, IAddRecipeGroups*/ {
    public override string Texture => AequusTextures.TemporaryDebuffIcon;

    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
        BuffSets.ModifiesMoveSpeed.Add(Type);
    }

    #region On Add Buff
    public void PostAddBuff(NPC npc, int duration, bool quiet) {
        if (npc.HasBuff<StunGunDebuff>()) {
            SoundEngine.PlaySound(AequusSounds.InflictStunned with { Volume = 0.3f, Pitch = 0.175f, PitchVariance = 0.05f });
        }
        //else {
        //    npc.buffImmune[ModContent.BuffType<StunGunDebuff>()] = false;
        //    npc.AddBuff(ModContent.BuffType<StunGunDebuff>(), duration, quiet);
        //}
    }

    public void PostAddBuff(Player player, int duration, bool quiet, bool foodHack) {
        if (player.HasBuff<StunGunDebuff>()) {
            SoundEngine.PlaySound(AequusSounds.InflictStunned);
        }
    }
    #endregion

    public static bool IsStunnable(NPC npc) {
        //if (npc.ModNPC?.Mod?.Name == "CalamityMod") {
        //    return true;
        //}
        return !NPCID.Sets.BelongsToInvasionOldOnesArmy[npc.type] && (!npc.buffImmune[BuffID.Confused] || BuffSets.StunnableNPCIDs.Contains(npc.type) || BuffSets.StunnableAIStyles.Contains(npc.aiStyle));
    }

    public override void Update(NPC npc, ref int buffIndex) {
        if (!npc.TryGetGlobalNPC<AequusNPC>(out var aequusNPC)) {
            return;
        }

        aequusNPC.stunGunVisual = true;
        if (IsStunnable(npc)) {
            aequusNPC.stunGun = true;
        }
        else {
            aequusNPC.statSpeedX *= 0.5f;
            aequusNPC.statSpeedY *= 0.5f;
        }
        //npc.netOffset.X = Main.rand.NextFloat(-2f, 2f);

        var dustSpotFront = npc.Center + StunGun.GetVisualOffset(npc, StunGun.GetVisualTime(StunGun.VisualTimer, front: true));
        var dustSpotBack = npc.Center + StunGun.GetVisualOffset(npc, StunGun.GetVisualTime(StunGun.VisualTimer, front: false));
        float dustScale = StunGun.GetVisualScale(npc);
        int dustSize = (int)(5 * dustScale);

        if (npc.buffTime[buffIndex] <= 1) {
            for (int i = 0; i < 2; i++) {
                var d = Dust.NewDustPerfect(dustSpotFront + Main.rand.NextVector2Square(-dustSize, dustSize), DustID.Electric);
                d.velocity *= 2f;
                d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.2f);
                d.noGravity = true;

                d = Dust.NewDustPerfect(dustSpotBack + Main.rand.NextVector2Square(-dustSize, dustSize), DustID.Electric);
                d.velocity *= 0.5f;
                d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.2f);
                d.noGravity = true;
            }
        }
        if (Main.GameUpdateCount % 15 == 0) {
            var d = Dust.NewDustPerfect(dustSpotFront + Main.rand.NextVector2Square(-dustSize, dustSize), DustID.Electric, Scale: 0.6f * dustScale);
            d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.2f);
            d.noGravity = true;

            d = Dust.NewDustPerfect(dustSpotBack + Main.rand.NextVector2Square(-dustSize, dustSize), DustID.Electric, Scale: 0.6f * dustScale);
            d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.2f);
            d.noGravity = true;
        }
    }
}