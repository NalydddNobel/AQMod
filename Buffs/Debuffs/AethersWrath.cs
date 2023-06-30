using Aequus.Buffs.Debuffs;
using Aequus.Common.DataSets;
using Aequus.Common.Particles;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs {
    public class AethersWrath : ModBuff, IPostAddRecipes {
        public static Color[] ParticleColors = new[] { new Color(80, 180, 255, 10), new Color(255, 255, 255, 10), new Color(100, 255, 100, 10), };
        public static int Damage = 50;
        public static int DamageNumber = 10;

        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            AequusBuff.SetImmune(NPCID.MoonLordCore, Type);
            AequusBuff.SetImmune(NPCID.MoonLordHand, Type);
            AequusBuff.SetImmune(NPCID.MoonLordHead, Type);
            AequusBuff.SetImmune(NPCID.CultistArcherBlue, Type);
            AequusBuff.SetImmune(NPCID.CultistArcherWhite, Type);
            AequusBuff.SetImmune(NPCID.CultistBoss, Type);
            AequusBuff.SetImmune(NPCID.CultistBossClone, Type);
            AequusBuff.SetImmune(NPCID.CultistDevote, Type);
            AequusBuff.SetImmune(NPCID.CultistDragonHead, Type);
            AequusBuff.SetImmune(NPCID.CultistDragonBody1, Type);
            AequusBuff.SetImmune(NPCID.CultistDragonBody2, Type);
            AequusBuff.SetImmune(NPCID.CultistDragonBody3, Type);
            AequusBuff.SetImmune(NPCID.CultistDragonBody4, Type);
            AequusBuff.SetImmune(NPCID.CultistDragonTail, Type);
            AequusBuff.SetImmune(NPCID.AncientCultistSquidhead, Type);
            AequusBuff.IsFire.Add(Type);
            AequusBuff.PlayerDoTBuff.Add(Type);
        }

        public void PostAddRecipes(Aequus aequus) {
            foreach (var i in NPCSets.IsHallow) {
                AequusBuff.SetImmune(i, Type);
            }
            foreach (var i in NPCSets.FromPillarEvent) {
                AequusBuff.SetImmune(i, Type);
            }
        }

        public override void Update(NPC npc, ref int buffIndex) {
            npc.Aequus().debuffAetherFire = true;
        }
    }
}

namespace Aequus.NPCs {
    partial class AequusNPC {
        public bool debuffAetherFire;

        private void DebuffEffect_AethersWrath(NPC npc) {
            if (!debuffAetherFire) {
                return;
            }

            int amt = (int)(npc.Size.Length() / 32f);
            for (int i = 0; i < amt; i++) {
                var spawnLocation = Main.rand.NextFromRect(npc.Hitbox);
                spawnLocation.Y -= 4f;
                var color = Helper.LerpBetween(AethersWrath.ParticleColors, spawnLocation.X / 32f + Main.GlobalTimeWrappedHourly * 4f);
                ParticleSystem.New<MonoBloomParticle>(ParticleLayer.BehindPlayers).Setup(
                    spawnLocation, 
                    -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-2f, 2f), -Main.rand.NextFloat(-2f, 2f)),
                    color, (color with { A = 0 }).HueAdd(Main.rand.NextFloat(0.02f)) * 0.01f,
                    Main.rand.NextFloat(1.5f, 3f), 0.3f, 
                    Main.rand.NextFloat(MathHelper.TwoPi)
                );
                if (i == 0 && Main.GameUpdateCount % 12 == 0) {
                    var velocity = -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-6f, 6f), -Main.rand.NextFloat(4f, 7f));
                    float scale = Main.rand.NextFloat(1.2f, 2.2f);
                    float rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    ParticleSystem.New<AethersWrathParticle>(ParticleLayer.BehindPlayers).Setup(spawnLocation, velocity, 10, color, scale * 1.2f, rotation);
                }
            }
        }

        private void UpdateLifeRegen_AethersWrath(NPC npc, ref LifeRegenModifiers modifiers) {
            if (debuffAetherFire) {
                modifiers.LifeRegen -= AethersWrath.Damage;
                modifiers.DamageNumber = AethersWrath.DamageNumber;
                modifiers.ApplyOiled = true;
            }
        }
    }
}