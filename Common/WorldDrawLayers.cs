using AQMod.Assets.Textures;
using AQMod.Common.WorldEvents;
using AQMod.Items.BossItems.Starite;
using AQMod.NPCs.Glimmer.OmegaStar;
using AQMod.NPCs.Ocean.Crabson;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common
{
    public class WorldDrawLayers
    {
        internal static void Setup()
        {
            On.Terraria.Main.DrawNPCs += Main_DrawNPCs;
        }

        public static void Reset()
        {
        }


        private static void Main_DrawNPCs(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            if (!behindTiles)
            {
                if (GlimmerEvent.IsActive)
                {
                    float offset = GlimmerEvent._ultimateSwordOffsetY == 0f ? (float)Math.Sin(Main.GlobalTime) * 8f : GlimmerEvent._ultimateSwordOffsetY;
                    var position = new Vector2(GlimmerEvent.X * 16f, GlimmerEvent.Y * 16f - 120f + offset);
                    if (!GlimmerEvent.FakeActive)
                        position.X += 8f;
                    if (Vector2.Distance(SpriteUtils.WorldScreenCenter, position) < Main.screenWidth + Main.screenHeight)
                    {
                        if ((AQMod.OmegaStariteIndexCache == -1 || AQMod.omegaStariteScene == 1) && AQMod.omegaStariteScene != 3)
                        {
                            if (AQMod.OmegaStariteIndexCache == -1)
                            {
                                AQMod.omegaStariteScene = 0;
                                GlimmerEvent._ultimateSwordOffsetY = 0f;
                            }
                            GlimmerEvent._ultimateSwordChatTimer++;
                            if (GlimmerEvent._ultimateSwordChatTimer >= 2700)
                            {
                                GlimmerEvent._ultimateSwordChatTimer = 0;
                                Main.PlaySound(AQMod.Instance.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/StariteSpeech").WithVolume(0.2f * Main.ambientVolume).WithPitchVariance(1f), position);
                            }
                            var ultimateSwordID = ModContent.ItemType<UltimateSword>();
                            var texture = Main.itemTexture[ultimateSwordID];
                            var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                            var origin = frame.Size() / 2f;
                            Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, new Color(255, 255, 255, 255), MathHelper.PiOver4 * 3f, origin, 1f, SpriteEffects.None, 0f);
                            float x = (float)Math.Sin(Main.GlobalTime / 2f) * 4f;
                            Main.spriteBatch.Draw(texture, position + new Vector2(x, 0f) - Main.screenPosition, frame, new Color(75, 75, 75, 0), MathHelper.PiOver4 * 3f, origin, 1f, SpriteEffects.None, 0f);
                            Main.spriteBatch.Draw(texture, position + new Vector2(-x, 0f) - Main.screenPosition, frame, new Color(75, 75, 75, 0), MathHelper.PiOver4 * 3f, origin, 1f, SpriteEffects.None, 0f);
                            var hitbox = Utils.CenteredRectangle(position, new Vector2(30f, 80f));
                            Vector2 trueMouseworld = AQUtils.TrueMouseworld;
                            if (AQMod.omegaStariteScene == 1)
                            {
                                GlimmerEvent._ultimateSwordOffsetY = MathHelper.Lerp(GlimmerEvent._ultimateSwordOffsetY, -320f, 0.05f);
                                int dustChance = 10 - (int)(offset - GlimmerEvent._ultimateSwordOffsetY).Abs();
                                if (dustChance < 2 || Main.rand.NextBool(dustChance))
                                {
                                    int d = Dust.NewDust(position + new Vector2(-8f, texture.Height + -24f), 16, 16, ModContent.DustType<Dusts.UltimaDust>());
                                    Main.dust[d].scale = 1.5f;
                                    Main.dust[d].velocity.X *= 0.05f;
                                    Main.dust[d].velocity.Y = Main.rand.NextFloat(5f, 9f);
                                }
                            }
                            if (hitbox.Contains((int)trueMouseworld.X, (int)trueMouseworld.Y) && GlimmerEvent.ActuallyActive)
                            {
                                int omegaStariteID = ModContent.NPCType<OmegaStarite>();
                                if (AQMod.omegaStariteScene == 0 && !Main.gameMenu & !Main.gamePaused & Main.LocalPlayer.IsInTileInteractionRange((int)position.X / 16, (int)position.Y / 16))
                                {
                                    var plr = Main.LocalPlayer;
                                    plr.mouseInterface = true;
                                    plr.noThrow = 2;
                                    plr.showItemIcon = true;
                                    plr.showItemIcon2 = ultimateSwordID;
                                    texture = SpriteUtils.Textures.Extras[ExtraID.UltimateSwordHighlight];
                                    frame = new Rectangle(0, 0, texture.Width, texture.Height);
                                    Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, new Color(255, 255, 255, 255), MathHelper.PiOver4 * 3f, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);
                                    if (Main.mouseRight && Main.mouseRightRelease)
                                    {
                                        plr.tileInteractAttempted = true;
                                        plr.tileInteractionHappened = true;
                                        plr.releaseUseTile = false;
                                        Main.PlaySound(SoundID.Item, (int)plr.position.X + plr.width / 2, (int)plr.position.Y + plr.height / 2, 4, 0.5f, -2.5f);
                                        AQMod.OmegaStariteIndexCache = (short)NPC.NewNPC(GlimmerEvent.X * 16 + 8, GlimmerEvent.Y * 16 - 1600, omegaStariteID, 0, OmegaStarite.PHASE_NOVA, 0f, 0f, 0f, Main.myPlayer);
                                        Main.npc[AQMod.OmegaStariteIndexCache].netUpdate = true;
                                        AQMod.omegaStariteScene = 1;
                                        GlimmerEvent._ultimateSwordOffsetY = offset;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                int crabsonType = ModContent.NPCType<JerryCrabson>();
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].type == crabsonType)
                    {
                        var crabsonPosition = Main.npc[i].Center;
                        DrawMethods.DrawJerryChain(new Vector2(crabsonPosition.X - 24f, crabsonPosition.Y), Main.npc[(int)Main.npc[i].localAI[0]].Center);
                        DrawMethods.DrawJerryChain(new Vector2(crabsonPosition.X + 24f, crabsonPosition.Y), Main.npc[(int)Main.npc[i].localAI[1]].Center);
                    }
                }
            }
            orig(self, behindTiles);
        }
    }
}