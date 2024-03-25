using Aequus.Common.Buffs.Components;
using Aequus.Common.NPCs;
using Aequus.DataSets;
using Terraria.Audio;

namespace Aequus.Content.Weapons.Classless.StunGun;

public class StunGunDebuff : ModBuff, IOnAddBuff/*, IAddRecipeGroups*/ {
    public override string Texture => AequusTextures.TemporaryDebuffIcon;

    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
        BuffSets.GrantImmunityWith[Type].Add(BuffID.Slow);
    }

    #region On Add Buff
    public void PostAddBuff(NPC npc, bool alreadyHasBuff, int duration, bool quiet) {
        if (npc.HasBuff<StunGunDebuff>()) {
            SoundEngine.PlaySound(AequusSounds.InflictStunned with { Volume = 0.3f, Pitch = 0.175f, PitchVariance = 0.05f });
        }
        //else {
        //    npc.buffImmune[ModContent.BuffType<StunGunDebuff>()] = false;
        //    npc.AddBuff(ModContent.BuffType<StunGunDebuff>(), duration, quiet);
        //}
    }

    public void PostAddBuff(Player player, bool alreadyHasBuff, int duration, bool quiet, bool foodHack) {
        if (player.HasBuff<StunGunDebuff>()) {
            SoundEngine.PlaySound(AequusSounds.InflictStunned);
        }
    }
    #endregion

    public static bool IsStunnable(NPC npc) {
        //if (npc.ModNPC?.Mod?.Name == "CalamityMod") {
        //    return true;
        //}
        return !NPCSets.BelongsToInvasionOldOnesArmy[npc.type] && (!npc.buffImmune[BuffID.Confused] || NPCDataSet.StunnableByTypeId.Contains(npc.type) || NPCDataSet.StunnableByAI.Contains(npc.aiStyle));
    }

    private void EmitParticles(Entity entity, int[] buffTime, ref int buffIndex) {
        var dustSpotFront = entity.Center + StunGun.GetVisualOffset(entity.width, StunGun.GetVisualTime(StunGun.VisualTimer, front: true), entity.whoAmI);
        var dustSpotBack = entity.Center + StunGun.GetVisualOffset(entity.width, StunGun.GetVisualTime(StunGun.VisualTimer, front: false), entity.whoAmI);
        float dustScale = StunGun.GetVisualScale(entity.Size.Length());
        int dustSize = (int)(5 * dustScale);

        if (buffTime[buffIndex] <= 1) {
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

        EmitParticles(npc, npc.buffTime, ref buffIndex);
    }

    public override void Update(Player player, ref int buffIndex) {
        player.frozen = true;

        EmitParticles(player, player.buffTime, ref buffIndex);
    }
}