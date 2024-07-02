using Aequus.Core.Components.Buffs;
using Aequus.DataSets;
using System;
using Terraria.Audio;

namespace Aequus.Old.Content.StatusEffects;

public class BlueFire : ModBuff, IOnAddBuff {
    public static int Damage { get; set; } = 20;
    public static int DamageNumber { get; set; } = 5;
    public static int VFXWearOffTime { get; set; } = 50;

    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
        Main.buffNoSave[Type] = true;
        BuffDataSet.PlayerDoTDebuff.Add(Type);
    }

    public override void Update(NPC npc, ref int buffIndex) {
        npc.GetGlobalNPC<BlueFireNPC>().debuffBlueFire = VFXWearOffTime;
    }

    public void PostAddBuff(NPC npc, bool alreadyHasBuff, int duration, bool quiet) {
        if (npc.HasBuff<BlueFire>()) {
            SoundEngine.PlaySound(AequusSounds.InflictFireDebuff, npc.Center);
        }
    }
}

public class BlueFireNPC : GlobalNPC {
    public int debuffBlueFire;

    public override bool InstancePerEntity => true;
    protected override bool CloneNewInstances => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
        return !entity.buffImmune[ModContent.BuffType<BlueFire>()];
    }

    public override void ResetEffects(NPC npc) {
        if (debuffBlueFire > 0) {
            debuffBlueFire--;
        }
    }

    public override void PostAI(NPC npc) {
        if (debuffBlueFire > 0 && Main.netMode != NetmodeID.Server && !Main.rand.NextBool(debuffBlueFire)) {
            int amt = Math.Max((int)(npc.Size.Length() / 64f), 1);
            for (int i = 0; i < amt; i++) {
                var p = BlueFireParticle.New();
                p.Location = Main.rand.NextVector2FromRectangle(npc.Hitbox);
                p.Velocity = -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                p.Color = new Color(45, 90, 205, 0);
                p.BloomColor = new Color(0, 0, 10, 0);
                p.Scale = Main.rand.NextFloat(1.5f, 3f);
                p.BloomScale = 0.3f;
                p.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }
        }
    }

    public override void UpdateLifeRegen(NPC npc, ref int damage) {
        if (debuffBlueFire > 0) {
            npc.lifeRegen -= BlueFire.Damage;
            damage = BlueFire.DamageNumber;

            if (npc.oiled) {
                npc.lifeRegen -= 50;
            }
        }
    }
}
