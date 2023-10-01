using Aequus.Common.Graphics;
using Aequus.Common.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.BossMonsters.Upriser {
    public class Upriser : ModNPC {
        public const string ShaderKey = "Aequus:UpriserWings";
        private static Ref<Effect> _wingShader;
        private float wingsRotation;

        public override void Load() {
            if (!Main.dedServ) {
                GameShaders.Misc[ShaderKey] = new MiscShaderData(AequusShaders.GlintMiscShader, "EnchantmentPass")
                    .UseOpacity(0.8f)
                    .UseImage1(AequusTextures.Upriser_Wings_Effect);
            }
        }

        public override void SetStaticDefaults() {
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Bleeding] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Ichor] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.ShadowFlame] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.NeutralHunger] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Hunger] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Starving] = true;
        }

        public override void SetDefaults() {
            NPC.width = 100;
            NPC.height = 100;
            NPC.lifeMax = 36000;
            NPC.defense = 24;
            NPC.damage = 50;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment) {
            NPC.lifeMax = (int)(NPC.lifeMax * balance * 0.6f);
        }

        public override void AI() {
            wingsRotation = Math.Clamp(NPC.velocity.X * 0.03f, -1f, 1f);
            switch ((int)NPC.ai[0]) {
                case -2: {
                        NPC.alpha += 5;
                        NPC.ai[1]++;
                        if (NPC.ai[1] > 30f) {
                            NPC.KillEffects();
                        }
                    }
                    break;

                case -1: {
                        NPC.ai[0] = -2f;
                        NPC.ai[1] = 0f;
                        NPC.velocity.Y = -12f;
                        NPC.netUpdate = true;
                    }
                    break;

                case 0: {
                        NPC.velocity.Y *= 0.92f;
                        if ((int)NPC.ai[1] == 0) {
                            NPC.velocity.Y = -8f;
                        }
                        NPC.ai[1]++;
                        if (NPC.ai[1] > 60f) {
                            NPC.ai[0] = 1f;
                            NPC.ai[1] = 0f;
                            NPC.TargetClosest(faceTarget: true);
                            NPC.netUpdate = true;
                        }
                    }
                    break;

                case 1: {
                        if (!NPC.HasValidTarget && !NPC.TryRetargeting()) {
                            NPC.ai[0] = -1f;
                            break;
                        }
                        var gotoPosition = Main.player[NPC.target].Center - new Vector2(0f, 240f);
                        if (NPC.Distance(gotoPosition) > 30f) {
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(gotoPosition - NPC.Center) * 20f, 0.04f);
                        }

                        NPC.spriteDirection = 0;
                    }
                    break;
            }
        }

        private void DrawWings(SpriteBatch spriteBatch, Vector2 drawCoordinates, SpriteEffects effects) {
            var wingsTexture = AequusTextures.Upriser_Wings;
            var wingsFrame = wingsTexture.Frame(verticalFrames: 5, frameY: (int)Main.GameUpdateCount / 6 % 5);
            var wingsOrigin = wingsFrame.Size() / 2f;
            DrawData drawData = new(AequusTextures.Upriser_Wings, drawCoordinates, wingsFrame, Color.White, NPC.rotation + wingsRotation, wingsOrigin, NPC.scale, effects, 0);
            drawData.Draw(spriteBatch);
            AequusDrawing.spriteBatchCache.Inherit(spriteBatch);
            spriteBatch.End();
            if (NPC.IsABestiaryIconDummy) {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, AequusDrawing.RasterizerState_BestiaryUI, null, Main.UIScaleMatrix);
            }
            else {
                spriteBatch.Begin_World(shader: true);
            }

            drawData.texture = AequusTextures.Upriser_Wings_Mask;
            GameShaders.Misc[ShaderKey].Apply(drawData);
            
            drawData.Draw(spriteBatch);

            spriteBatch.End();
            AequusDrawing.spriteBatchCache.Begin(spriteBatch);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            var drawCoordinates = NPC.Center - screenPos;
            var effects = NPC.spriteDirection.ToSpriteEffect();
            DrawWings(spriteBatch, drawCoordinates, effects);
            spriteBatch.Draw(TextureAssets.Npc[Type].Value, drawCoordinates, NPC.frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, TextureAssets.Npc[Type].Value.Size() / 2f, NPC.scale, effects, 0f);
            return false;
        }

        public override bool IsLoadingEnabled(Mod mod) {
            return Aequus.DevelopmentFeatures;
        }
    }
}