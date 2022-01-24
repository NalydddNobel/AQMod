using AQMod.Common;
using AQMod.Common.Graphics;
using AQMod.Content.Players;
using AQMod.Content.World.Events.GlimmerEvent;
using AQMod.Dusts;
using AQMod.Effects.GoreNest;
using AQMod.Effects.Wind;
using AQMod.Effects.WorldEffects;
using AQMod.Items.Weapons.Melee;
using AQMod.NPCs.Bosses;
using AQMod.NPCs.Monsters.DemonSiege;
using AQMod.Tiles.Nature;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AQMod.Effects
{
    public class GameWorldRenders : ModWorld
    {
        internal static byte UltimateSwordEffectDelay;
        internal static UnifiedRandom EffectRand;

        public static class Hooks
        {
            internal static int LastScreenWidth;
            internal static int LastScreenHeight;

            internal static void Main_UpdateDisplaySettings(On.Terraria.Main.orig_UpdateDisplaySettings orig, Main self)
            {
                orig(self);
                if (!Main.gameMenu && Main.graphics.GraphicsDevice != null && !Main.graphics.GraphicsDevice.IsDisposed && Main.spriteBatch != null)
                {
                    if (LastScreenWidth != Main.screenWidth || LastScreenHeight != Main.screenHeight)
                    {
                        WindLayer.ResetTargets(Main.graphics.GraphicsDevice);
                    }
                    WindLayer.DrawTargets();
                    AQGraphics.SetCullPadding();
                    LastScreenWidth = Main.screenWidth;
                    LastScreenHeight = Main.screenHeight;
                }
            }

            public static void Main_DrawNPCs(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
            {
                if (behindTiles)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active)
                        {
                            if (Main.npc[i].type == ModContent.NPCType<JerryCrabson>())
                            {
                                var crabsonPosition = Main.npc[i].Center;
                                var chain = ModContent.GetTexture(AQUtils.GetPath<JerryClaw>("_Chain"));
                                AQGraphics.Rendering.JerryChain(chain, new Vector2(crabsonPosition.X - 24f, crabsonPosition.Y), Main.npc[(int)Main.npc[i].localAI[0]].Center);
                                AQGraphics.Rendering.JerryChain(chain, new Vector2(crabsonPosition.X + 24f, crabsonPosition.Y), Main.npc[(int)Main.npc[i].localAI[1]].Center);
                            }
                        }
                    }
                }
                else
                {
                    GoreNestRenderer.RenderGoreNests();

                    var screenCenter = AQGraphics.WorldScreenCenter;

                    if (OmegaStariteScenes.OmegaStariteIndexCache == -1 && GlimmerEvent.deactivationDelay <= 0)
                        OmegaStariteScenes.SceneType = 0;

                    AQGraphics.SetCullPadding(padding: 360);

                    if (AQGraphics.Cull(new Vector2(GlimmerEvent.tileX * 16f, GlimmerEvent.tileY * 16f) - Main.screenPosition) && OmegaStariteScenes.SceneType < 1)
                    {
                        float x = GlimmerEvent.tileX * 16f;
                        if (Framing.GetTileSafely(GlimmerEvent.tileX, GlimmerEvent.tileY).type == ModContent.TileType<GlimmeringStatue>())
                        {
                            x += 16f;
                        }
                        else
                        {
                            x += 8f;
                        }
                        float y = GlimmerEvent.tileY * 16 - 80f + (float)Math.Sin(Main.GameUpdateCount * 0.0157f) * 8f;
                        var drawPos = new Vector2(x, y);
                        var texture = Main.itemTexture[ModContent.ItemType<UltimateSword>()];
                        var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                        var origin = new Vector2(frame.Width, 0f);
                        Main.spriteBatch.Draw(texture, drawPos - Main.screenPosition, frame, new Color(255, 255, 255, 255), MathHelper.PiOver4 * 3f, origin, 1f, SpriteEffects.None, 0f);

                        var blurTexture = ModContent.GetTexture(AQUtils.GetPath<UltimateSword>("_Blur"));
                        var blurFrame = new Rectangle(0, 0, blurTexture.Width, blurTexture.Height);
                        var blurOrigin = new Vector2(origin.X, blurTexture.Height - texture.Height);
                        Main.spriteBatch.Draw(blurTexture, drawPos - Main.screenPosition, blurFrame, new Color(80 + Main.DiscoR / 60, 80 + Main.DiscoG / 60, 80 + Main.DiscoB / 60, 0) * (1f - (float)Math.Sin(Main.GameUpdateCount * 0.0157f)), MathHelper.PiOver4 * 3f, blurOrigin, 1f, SpriteEffects.None, 0f);

                        var hitbox = new Rectangle((int)drawPos.X - 10, (int)drawPos.Y - 60, 20, 60);
                        Vector2 trueMouseworld = AQUtils.TrueMouseworld;
                        if (hitbox.Contains((int)trueMouseworld.X, (int)trueMouseworld.Y) && GlimmerEvent.IsGlimmerEventCurrentlyActive())
                        {
                            if (OmegaStariteScenes.SceneType == 0 && !Main.gameMenu && !Main.gamePaused && Main.LocalPlayer.IsInTileInteractionRange((int)trueMouseworld.X / 16, (int)trueMouseworld.Y / 16))
                            {
                                var plr = Main.LocalPlayer;
                                plr.mouseInterface = true;
                                plr.noThrow = 2;
                                plr.showItemIcon = true;
                                plr.showItemIcon2 = ModContent.ItemType<UltimateSword>();
                                var highlightTexture = ModContent.GetTexture(AQUtils.GetPath<UltimateSword>("_Highlight"));
                                Main.spriteBatch.Draw(highlightTexture, drawPos - Main.screenPosition, frame, new Color(255, 255, 255, 255), MathHelper.PiOver4 * 3f, origin, 1f, SpriteEffects.None, 0f);
                                if (Main.mouseRight && Main.mouseRightRelease)
                                {
                                    plr.tileInteractAttempted = true;
                                    plr.tileInteractionHappened = true;
                                    plr.releaseUseTile = false;
                                    if (Main.netMode == NetmodeID.SinglePlayer)
                                    {
                                        AQMod.spawnStarite = true;
                                        WorldDefeats.OmegaStariteIntroduction = true;
                                    }
                                    else
                                    {
                                        NetHelper.RequestOmegaStarite();
                                    }
                                    Main.PlaySound(SoundID.Item, (int)drawPos.X, (int)drawPos.Y, 4, 0.5f, -2.5f);
                                }
                            }
                        }
                    }

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<Trapper>() && !AQGraphics.Cull(Main.npc[i].frame))
                        {
                            var chainTexture = ModContent.GetTexture(AQUtils.GetPath<Trapper>("_Chain"));
                            int npcOwner = (int)Main.npc[i].ai[1] - 1;
                            int height = chainTexture.Height - 2;
                            var npcCenter = Main.npc[i].Center;
                            var trapImpCenter = Main.npc[npcOwner].Center;
                            var velocity = npcCenter - trapImpCenter;
                            int length = (int)(velocity.Length() / height);
                            velocity.Normalize();
                            velocity *= height;
                            float rotation = velocity.ToRotation() + MathHelper.PiOver2;
                            var origin = new Vector2(chainTexture.Width / 2f, chainTexture.Height / 2f);
                            for (int j = 1; j < length; j++)
                            {
                                var position = trapImpCenter + velocity * j;
                                var color = Lighting.GetColor((int)(position.X / 16), (int)(position.Y / 16f));
                                if (j < 6)
                                    color *= 1f / 6f * j;
                                Main.spriteBatch.Draw(chainTexture, position - Main.screenPosition, null, color, rotation, origin, 1f, SpriteEffects.None, 0f);
                            }
                        }
                    }

                    WindLayer.DrawFinal();
                }
                orig(self, behindTiles);
                if (behindTiles)
                {
                    CustomRenderBehindTiles.Render();
                    //SceneLayersManager.DrawLayer(SceneLayering.BehindTiles_InfrontNPCs);
                }
                else
                {
                    //SceneLayersManager.DrawLayer(SceneLayering.InfrontNPCs);
                }
            }

            public static void Main_DrawTiles(On.Terraria.Main.orig_DrawTiles orig, Main self, bool solidOnly, int waterStyleOverride)
            {
                if (!solidOnly)
                {
                    GoreNestRenderer.RefreshCoordinates();
                }
                orig(self, solidOnly, waterStyleOverride);
            }
        }

        public override void Initialize()
        {
            UltimateSwordEffectDelay = 0;
            EffectRand = new UnifiedRandom();
        }

        public static void DoUpdate()
        {
            if (!GlimmerEvent.IsGlimmerEventCurrentlyActive() || OmegaStariteScenes.SceneType > 1 || AQGraphics.Cull(Utils.CenteredRectangle(new Vector2(GlimmerEvent.tileX, GlimmerEvent.tileY) - Main.screenPosition, new Vector2(80f, 160f))))
                return;
            float x = GlimmerEvent.tileX * 16f;
            if (Framing.GetTileSafely(GlimmerEvent.tileX, GlimmerEvent.tileY).type == ModContent.TileType<GlimmeringStatue>())
            {
                x += 16f;
            }
            else
            {
                x += 8f;
            }
            float y = GlimmerEvent.tileY * 16 - 80f + (float)Math.Sin(Main.GameUpdateCount * 0.0157f) * 8f;
            Lighting.AddLight(new Vector2(x, y), new Vector3(1f, 1f, 1f));
            if (EffectRand.NextBool(10))
            {
                int d = Dust.NewDust(new Vector2(x, y) + new Vector2(EffectRand.Next(-6, 6), -EffectRand.Next(60)), 2, 2, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(160, 160, 160, 80));
                Main.dust[d].velocity *= 0.1f;
                Main.dust[d].noGravity = true;
            }
            if (UltimateSwordEffectDelay > 0)
            {
                UltimateSwordEffectDelay--;
            }
            else if (EffectRand.NextBool(10 + (int)(20 * (1f - AQConfigClient.c_EffectIntensity))))
            {
                AQMod.WorldEffects.Add(new UltimateSwordEffect(EffectRand));
                UltimateSwordEffectDelay = (byte)(int)(8 * (1f - AQConfigClient.c_EffectIntensity));
            }
        }
    }
}