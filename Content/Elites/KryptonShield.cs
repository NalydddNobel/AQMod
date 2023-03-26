using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Elites {
    public class KryptonShield : ModNPC {

        private const int STATE_DEFAULT = 0;
        private const int STATE_DEAD = 1;
        public int State { get => (int)NPC.ai[0]; set => NPC.ai[0] = value; }
        public int NPCOwner { get => (int)NPC.ai[1] - 1; set => NPC.ai[1] = value + 1; }

        private byte _hitEffect;

        public override void SetStaticDefaults() {
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = new(0) {
                Hide = true,
            };
            NPCID.Sets.CannotDropSouls[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.DebuffImmunitySets[Type] = new() {
                ImmuneToAllBuffsThatAreNotWhips = true,
            };
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
        }

        public override void SetDefaults() {
            NPC.width = 60;
            NPC.height = 60;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.lifeMax = 160;
            NPC.defense = 16;
            NPC.npcSlots = 0f;
            NPC.damage = 0;
            NPC.chaseable = true;
            NPC.knockBackResist = 0f;
            NPC.alpha = 250;
            NPC.scale = 1.33f;
            NPC.HitSound = AequusSounds.hit_OmegaStarite with { Pitch = -0.1f, PitchVariance = 0.1f, Volume = 0.5f, };
        }

        public override void HitEffect(int hitDirection, double damage) {
            _hitEffect = (byte)(Math.Min(Math.Max((int)(damage / 4f), _hitEffect), 8) + 4);
        }

        public override bool CheckDead() {

            bool dontTakeDamage = NPC.dontTakeDamage;
            State = STATE_DEAD;
            NPC.ai[2] = 0f;
            NPC.life = 1;
            NPC.dontTakeDamage = true;
            return dontTakeDamage;
        }

        public override bool CheckActive() {
            return false;
        }

        public override Color? GetAlpha(Color drawColor) {
            return Colors.RarityLime with { A = 180 } * NPC.Opacity;
        }

        public override void AI() {

            // Misc thing to disable a lot of weird effects
            NPC.realLife = NPC.whoAmI;

            if (_hitEffect > 0)
                _hitEffect--;

            if (NPCOwner == -1) {

                int closestNPC = -1;
                float closestDistance = float.MaxValue;
                for (int i = 0; i < Main.maxNPCs; i++) {
                    if (i == NPC.whoAmI || !Main.npc[i].active) {
                        continue;
                    }
                    float distance = NPC.Distance(Main.npc[i].Center);
                    if (distance > closestDistance) {
                        continue;
                    }

                    closestNPC = i;
                    closestDistance = distance;
                }

                if (closestNPC == -1) {
                    NPC.Kill();
                    return;
                }
                NPCOwner = closestNPC;
            }

            var owner = Main.npc[NPCOwner];
            if (!owner.active) {
                NPC.Kill();
                return;
            }
            if (State == STATE_DEFAULT) {

                NPC.dontTakeDamage = false;
                NPC.localAI[0] = 0f;
                bool spawnEffects = NPC.ai[2] == 0;

                if (NPC.alpha > 0) {
                    NPC.alpha -= 6;
                    if (NPC.alpha < 0) {
                        NPC.alpha = 0;
                    }
                }

                Spin(owner);

                if (spawnEffects) {
                    SoundEngine.PlaySound(SoundID.DD2_DefenseTowerSpawn with { Pitch = -0.5f, Volume = 0.5f, }, NPC.Center);

                    for (int i = 0; i < 10; i++) {
                        var d = Dust.NewDustDirect(
                            NPC.position, NPC.width, NPC.height,
                            ModContent.DustType<MonoDust>(),
                            newColor: Colors.RarityLime with { A = 0 }
                        );
                        d.noGravity = true;
                    }
                }
            }
            else if (State == STATE_DEAD) {

                NPC.dontTakeDamage = true;
                NPC.life = NPC.lifeMax;
                if (NPC.ai[2] == 0) {
                    SoundEngine.PlaySound(SoundID.Shatter, NPC.Center);

                    for (int i = 0; i < 20; i++) {
                        var d = Dust.NewDustDirect(
                            NPC.position, NPC.width, NPC.height, 
                            ModContent.DustType<MonoDust>(), 
                            newColor: Colors.RarityLime with { A = 0 },
                            Scale: Main.rand.NextFloat(0.75f, 1.5f)
                        );
                        d.noGravity = true;
                    }
                }

                if (NPC.alpha < 200) {
                    NPC.alpha += 6;
                    if (NPC.alpha > 200) {
                        NPC.alpha = 200;
                    }
                }

                NPC.localAI[0]++;
                int timer = (int)NPC.localAI[0];
                if (timer == 180 || timer == 200 || timer == 220) {
                    for (int i = 0; i < 10; i++) {
                        var d = Dust.NewDustDirect(
                            NPC.position - new Vector2(20f), NPC.width + 40, NPC.height + 40,
                            ModContent.DustType<MonoDust>(),
                            newColor: Colors.RarityLime with { A = 0 },
                            Scale: Main.rand.NextFloat(1f, 1.5f)
                        );
                        d.velocity = (NPC.Center - d.position) / 12f;
                        d.noGravity = true;
                    }
                }
                if (timer > 240) {
                    NPC.localAI[0] = 0f;
                    State = STATE_DEFAULT;
                }

                NPC.ai[2] *= 0.85f;
                Spin(owner);
            }
        }

        private void Spin(NPC owner) {

            float distance = Math.Max(owner.Size.Length(), 64f);
            NPC.Center = owner.Center + (NPC.ai[3] - MathHelper.PiOver2).ToRotationVector2() * distance;

            float maxSpeed = 0.075f;
            if (NPC.ai[2] < maxSpeed) {
                NPC.ai[2] += 0.0025f;
                if (NPC.ai[2] > maxSpeed) {
                    NPC.ai[2] = maxSpeed;
                }
            }

            NPC.ai[3] += NPC.ai[2];
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) {

            if (NPCOwner < 0) {
                return null;
            }

            int shake = Math.Min((int)_hitEffect, 4);
            position = Main.npc[NPCOwner].Bottom + new Vector2(Main.rand.Next(-shake, shake), 28f + Main.rand.Next(-shake, shake) + Main.npc[NPCOwner].gfxOffY);
            scale += 0.25f;
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {

            var texture = TextureAssets.Npc[Type].Value;
            var color = NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor));
            spriteBatch.Draw(
                texture, 
                NPC.Center - screenPos + new Vector2(Main.rand.Next(-_hitEffect, _hitEffect), Main.rand.NextFloat(-_hitEffect, _hitEffect)),
                NPC.frame, 
                color, 
                NPC.rotation, 
                NPC.frame.Size() / 2f, 
                NPC.scale, SpriteEffects.None, 0f
            );
            return false;
        }
    }
}