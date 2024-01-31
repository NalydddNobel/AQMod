using Aequus.Common.Buffs.Components;
using Aequus.Content.DataSets;
using Aequus.Core.DataSets;
using Aequus.Old.Content.Particles;
using System;
using Terraria.Audio;

namespace Aequus.Old.Content.Weapons.Demon;

public class CrimsonHellfire : ModBuff, IOnAddBuff {
    public static int BaseDamage { get; set; } = 28;
    public static int StackDamage { get; set; } = 4;
    public static byte MaxStacks { get; set; } = 200;
    public static int BaseDamageNumber { get; set; } = 14;
    public static int StackDamageNumber { get; set; } = 2;
    public static Color FireColor { get; set; } = new Color(205, 15, 30, 60);
    public static Color BloomColor { get; set; } = new Color(175, 10, 2, 10);

    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
        Main.buffNoSave[Type] = true;
        BuffSets.DemonSiegeImmune.Add((BuffEntry)Type);
        BuffSets.PlayerDoTDebuff.Add((BuffEntry)Type);
    }

    public override bool ReApply(NPC npc, int time, int buffIndex) {
        if (npc.TryGetGlobalNPC(out CrimsonHellfireNPC crimsonHellfire)) {
            crimsonHellfire.stacks = (byte)Math.Min(crimsonHellfire.stacks + 1, MaxStacks);
        }
        return false;
    }

    public override void Update(NPC npc, ref int buffIndex) {
        if (npc.TryGetGlobalNPC(out CrimsonHellfireNPC crimsonHellfire)) {
            crimsonHellfire.stacks = Math.Max(crimsonHellfire.stacks, (byte)1);
            crimsonHellfire.hasDebuff = true;
        }
    }

    public void PostAddBuff(NPC npc, int duration, bool quiet) {
        if (npc.HasBuff<CrimsonHellfire>()) {
            SoundEngine.PlaySound(AequusSounds.InflictFireDebuff with { Pitch = -0.2f, PitchVariance = 0.1f }, npc.Center);
        }
    }
}

public class CrimsonHellfireNPC : GlobalNPC {
    public bool hasDebuff;
    public byte stacks;

    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
        return !entity.buffImmune[ModContent.BuffType<CrimsonHellfire>()];
    }

    public override void ResetEffects(NPC npc) {
        hasDebuff = false;
    }

    public override void PostAI(NPC npc) {
        if (hasDebuff && Main.netMode != NetmodeID.Server) {
            int amt = Math.Max((int)(npc.Size.Length() / 32f), 1);
            foreach (var p in ModContent.GetInstance<LegacyBloomParticle>().NewMultipleReduced(amt)) {
                p.Location = Main.rand.NextVector2FromRectangle(npc.Hitbox);
                p.Velocity = -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                p.Color = CrimsonHellfire.FireColor;
                p.BloomColor = CrimsonHellfire.BloomColor * 0.07f;
                p.Scale = Main.rand.NextFloat(1.5f, 3f);
                p.BloomScale = 0.3f;
                p.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                p.dontEmitLight = false;
                p.Frame = (byte)Main.rand.Next(3);
            }
        }
    }

    public override void UpdateLifeRegen(NPC npc, ref int damage) {
        if (!hasDebuff) {
            stacks = 0;
            return;
        }

        npc.lifeRegen -= CrimsonHellfire.BaseDamage + CrimsonHellfire.StackDamage * stacks;
        damage = CrimsonHellfire.BaseDamageNumber + CrimsonHellfire.StackDamageNumber * stacks;

        if (npc.oiled) {
            npc.lifeRegen -= 50;
        }
    }
}
