using Aequus.Common.NPCs;
using Aequus.Core.Utilities;
using MonoMod.Utils;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Classless.StunGun;

public class StunGunDebuff : ModBuff/*, IAddRecipeGroups*/ {
    public override string Texture => AequusTextures.TemporaryDebuffIcon;

    public static readonly HashSet<int> StunnableOverrideByNPCID = new();
    public static readonly HashSet<int> StunnableOverrideByAIStyle = new();

    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
        StunnableOverrideByAIStyle.AddRange(new int[] {
             NPCAIStyleID.Caster,
            NPCAIStyleID.Antlion,
            NPCAIStyleID.Mimic,
            NPCAIStyleID.HoveringFighter,
            NPCAIStyleID.BiomeMimic,
            NPCAIStyleID.MourningWood,
            NPCAIStyleID.Flocko,
            NPCAIStyleID.IceQueen,
            NPCAIStyleID.SantaNK1,
            NPCAIStyleID.SandElemental,
            NPCAIStyleID.Fighter,
            NPCAIStyleID.FlyingFish,
            NPCAIStyleID.GiantTortoise,
            NPCAIStyleID.GraniteElemental,
            NPCAIStyleID.Herpling,
            NPCAIStyleID.EnchantedSword,
            NPCAIStyleID.FlowInvader,
            NPCAIStyleID.Flying,
            NPCAIStyleID.Jellyfish,
            NPCAIStyleID.ManEater,
            NPCAIStyleID.Mothron,
            NPCAIStyleID.MothronEgg,
            NPCAIStyleID.BabyMothron,
            NPCAIStyleID.Bat,
            NPCAIStyleID.AncientVision,
            NPCAIStyleID.Corite,
            NPCAIStyleID.Creeper,
            NPCAIStyleID.CursedSkull,
            NPCAIStyleID.Piranha,
            NPCAIStyleID.Slime,
            NPCAIStyleID.SandShark,
            NPCAIStyleID.Sharkron,
            NPCAIStyleID.Snowman,
            NPCAIStyleID.Unicorn,
            NPCAIStyleID.Vulture,
            NPCAIStyleID.TeslaTurret,
            NPCAIStyleID.StarCell,
        });
    }

    //public void AddRecipeGroups(Aequus aequus) {
    //    for (int i = NPCID.NegativeIDCount + 1; i < NPCLoader.NPCCount; i++) {
    //        NPC npc = new();
    //        npc.SetDefaults(i);
    //        if (npc.buffImmune[BuffID.Confused]) {
    //            NPCHelper.SetBuffImmune(i, Type);
    //        }
    //    }
    //}

    public static bool IsStunnable(NPC npc) {
        return !NPCID.Sets.BelongsToInvasionOldOnesArmy[npc.type] && (!npc.buffImmune[BuffID.Confused] || StunnableOverrideByNPCID.Contains(npc.type) || StunnableOverrideByAIStyle.Contains(npc.aiStyle));
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