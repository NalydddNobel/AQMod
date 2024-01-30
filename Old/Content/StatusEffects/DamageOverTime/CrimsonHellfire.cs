using Aequus.Common.Buffs.Components;
using Aequus.Content.DataSets;
using Aequus.Core.DataSets;
using Aequus.Old.Content.Particles;
using System;
using Terraria.Audio;

namespace Aequus.Old.Content.StatusEffects.DamageOverTime;

public class CrimsonHellfire : ModBuff, IOnAddBuff {
    public static Int32 BaseDamage { get; set; } = 28;
    public static Int32 StackDamage { get; set; } = 4;
    public static Byte MaxStacks { get; set; } = 200;
    public static Int32 BaseDamageNumber { get; set; } = 14;
    public static Int32 StackDamageNumber { get; set; } = 2;
    public static Color FireColor { get; set; } = new Color(205, 15, 30, 60);
    public static Color BloomColor { get; set; } = new Color(175, 10, 2, 10);

    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
        Main.buffNoSave[Type] = true;
        BuffSets.DemonSiegeImmune.Add((BuffEntry)Type);
        BuffSets.PlayerDoTDebuff.Add((BuffEntry)Type);
    }

    public override Boolean ReApply(NPC npc, Int32 time, Int32 buffIndex) {
        if (npc.TryGetGlobalNPC(out CrimsonHellfireNPC crimsonHellfire)) {
            crimsonHellfire.stacks = (Byte)Math.Min(crimsonHellfire.stacks + 1, MaxStacks);
        }
        return false;
    }

    public override void Update(NPC npc, ref Int32 buffIndex) {
        if (npc.TryGetGlobalNPC(out CrimsonHellfireNPC crimsonHellfire)) {
            crimsonHellfire.stacks = Math.Max(crimsonHellfire.stacks, (Byte)1);
            crimsonHellfire.hasDebuff = true;
        }
    }

    public void PostAddBuff(NPC npc, Int32 duration, Boolean quiet) {
        if (npc.HasBuff<CrimsonHellfire>()) {
            SoundEngine.PlaySound(AequusSounds.InflictFireDebuff with { Pitch = -0.2f, PitchVariance = 0.1f }, npc.Center);
        }
    }
}

public class CrimsonHellfireNPC : GlobalNPC {
    public Boolean hasDebuff;
    public Byte stacks;

    public override Boolean InstancePerEntity => true;

    public override Boolean AppliesToEntity(NPC entity, Boolean lateInstantiation) {
        return !entity.buffImmune[ModContent.BuffType<CrimsonHellfire>()];
    }

    public override void ResetEffects(NPC npc) {
        hasDebuff = false;
    }

    public override void PostAI(NPC npc) {
        if (hasDebuff && Main.netMode != NetmodeID.Server) {
            Int32 amt = Math.Max((Int32)(npc.Size.Length() / 32f), 1);
            foreach (var p in ModContent.GetInstance<LegacyBloomParticle>().NewMultiple(amt)) {
                p.Location = Main.rand.NextVector2FromRectangle(npc.Hitbox);
                p.Velocity = -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                p.Color = CrimsonHellfire.FireColor;
                p.BloomColor = CrimsonHellfire.BloomColor * 0.07f;
                p.Scale = Main.rand.NextFloat(1.5f, 3f);
                p.BloomScale = 0.3f;
                p.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                p.dontEmitLight = false;
                p.Frame = (Byte)Main.rand.Next(3);
            }
        }
    }

    public override void UpdateLifeRegen(NPC npc, ref Int32 damage) {
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
