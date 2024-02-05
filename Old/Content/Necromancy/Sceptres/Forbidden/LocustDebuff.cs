using Aequus.Content.DataSets;
using Aequus.Old.Content.Weapons.Demon;
using System;

namespace Aequus.Old.Content.Necromancy.Sceptres.Forbidden;

public class LocustDebuff : ModBuff {
    public static int BaseDamage { get; set; } = 2;
    public static int StackDamage { get; set; } = 2;
    public static byte MaxStacks { get; set; } = 200;
    public static int BaseDamageNumber { get; set; } = 1;
    public static int StackDamageNumber { get; set; } = 1;

    public override string Texture => AequusTextures.TemporaryDebuffIcon;

    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
        Main.buffNoSave[Type] = true;
        BuffSets.PlayerDoTDebuff.Add(Type);
    }

    public override bool ReApply(NPC npc, int time, int buffIndex) {
        if (npc.TryGetGlobalNPC(out LocustDebuffNPC locust)) {
            locust.stacks = (byte)Math.Min(locust.stacks + 1, MaxStacks);
        }
        return false;
    }

    public override void Update(NPC npc, ref int buffIndex) {
        if (npc.TryGetGlobalNPC(out LocustDebuffNPC locust)) {
            locust.stacks = Math.Max(locust.stacks, (byte)1);
            locust.hasDebuff = true;
        }
    }
}

public class LocustDebuffNPC : GlobalNPC {
    public bool hasDebuff;
    public byte stacks;

    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
        return !entity.buffImmune[ModContent.BuffType<CorruptionHellfire>()];
    }

    public override void ResetEffects(NPC npc) {
        hasDebuff = false;
    }

    public override void UpdateLifeRegen(NPC npc, ref int damage) {
        if (!hasDebuff) {
            stacks = 0;
            return;
        }

        npc.lifeRegen -= LocustDebuff.BaseDamage + LocustDebuff.StackDamage * stacks;
        damage = LocustDebuff.BaseDamageNumber + LocustDebuff.StackDamageNumber * stacks;
    }
}
