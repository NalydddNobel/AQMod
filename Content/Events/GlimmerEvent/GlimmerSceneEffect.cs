using Aequus.Common.Effects;
using Aequus.Content.Events.GlimmerEvent.Sky;
using Aequus.Items.Weapons.Melee.Heavy;
using Aequus.NPCs.BossMonsters;
using Aequus.NPCs.BossMonsters.OmegaStarite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Events.GlimmerEvent {
    public class GlimmerSceneEffect : ModSceneEffect
    {
        public static LegacyMiscShaderWrap StarShader { get; private set; }

        public static bool renderedUltimateSword;
        public static int cantTouchThis;
        public static Vector2 ultimateSwordWorldDrawLocation;
        public static int EatenAlpha;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                StarShader = new LegacyMiscShaderWrap("Aequus/Assets/Effects/GlimmerBackgroundShaders", "Aequus:GlimmerBackgroundStars", "StarsPass", true);
                StarShader.ShaderData.UseImage1(ModContent.Request<Texture2D>("Terraria/Images/Misc/noise", AssetRequestMode.ImmediateLoad));
                SkyManager.Instance[GlimmerSky.Key] = new GlimmerSky() { checkDistance = true, };
            }
        }

        public override bool IsSceneEffectActive(Player player)
        {
            return player.Aequus().ZoneGlimmer || player.Aequus().ZonePeacefulGlimmer || GlimmerBiomeManager.omegaStarite != -1;
        }

        public override float GetWeight(Player player)
        {
            return GlimmerBiomeManager.omegaStarite != -1 || GlimmerSystem.CalcTiles(player) < GlimmerBiomeManager.UltraStariteSpawn ? 1f : 0.2f;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            bool monolithActive = SkyManager.Instance[CosmicMonolithScene.Key].IsActive();
            if (isActive && !monolithActive)
            {
                if (!SkyManager.Instance[GlimmerSky.Key].IsActive())
                {
                    SkyManager.Instance.Activate(GlimmerSky.Key);
                }
            }
            else
            {
                if (SkyManager.Instance[GlimmerSky.Key].IsActive())
                {
                    SkyManager.Instance.Deactivate(GlimmerSky.Key);
                }
            }
        }

        public static void DrawUltimateSword()
        {
            if (!GlimmerBiomeManager.EventActive || GlimmerBiomeManager.omegaStarite != -1 && (int)Main.npc[GlimmerBiomeManager.omegaStarite].ai[0] != AequusBoss.ACTION_INTRO && (int)Main.npc[GlimmerBiomeManager.omegaStarite].ai[0] != AequusBoss.ACTION_INIT)
            {
                EatenAlpha = 255;
                renderedUltimateSword = false;
                ultimateSwordWorldDrawLocation = Vector2.Zero;
                return;
            }
            var gotoPosition = GlimmerBiomeManager.TileLocation.ToWorldCoordinates() + new Vector2(8f, -60f);
            if (!renderedUltimateSword)
            {
                ultimateSwordWorldDrawLocation = gotoPosition;
            }
            else
            {
                if (ultimateSwordWorldDrawLocation.X > gotoPosition.X)
                {
                    ultimateSwordWorldDrawLocation.X--;
                    if (ultimateSwordWorldDrawLocation.X < gotoPosition.X)
                    {
                        ultimateSwordWorldDrawLocation.X = gotoPosition.X;
                    }
                }
                else if (ultimateSwordWorldDrawLocation.X < gotoPosition.X)
                {
                    ultimateSwordWorldDrawLocation.X++;
                    if (ultimateSwordWorldDrawLocation.X > gotoPosition.X)
                    {
                        ultimateSwordWorldDrawLocation.X = gotoPosition.X;
                    }
                }

                if (ultimateSwordWorldDrawLocation.Y > gotoPosition.Y)
                {
                    ultimateSwordWorldDrawLocation.Y -= 4;
                    if (ultimateSwordWorldDrawLocation.Y < gotoPosition.Y)
                    {
                        ultimateSwordWorldDrawLocation.Y = gotoPosition.Y;
                    }
                }
                else if (ultimateSwordWorldDrawLocation.Y < gotoPosition.Y)
                {
                    ultimateSwordWorldDrawLocation.Y += 4;
                    if (ultimateSwordWorldDrawLocation.Y > gotoPosition.Y)
                    {
                        ultimateSwordWorldDrawLocation.Y = gotoPosition.Y;
                    }
                }
            }
            renderedUltimateSword = false;
            ScreenCulling.Prepare(400);
            if (!ScreenCulling.OnScreenWorld(ultimateSwordWorldDrawLocation) && !ScreenCulling.OnScreenWorld(gotoPosition))
            {
                EatenAlpha = 0;
                return;
            }
            if (EatenAlpha > 0)
            {
                EatenAlpha -= 2;
            }

            var drawCoords = ultimateSwordWorldDrawLocation - Main.screenPosition + new Vector2(0f, Helper.Wave(Main.GlobalTimeWrappedHourly * 0.5f, -10f, 10f));
            Main.instance.LoadItem(ModContent.ItemType<UltimateSword>());
            var texture = TextureAssets.Item[ModContent.ItemType<UltimateSword>()].Value;

            var interactionRect = new Rectangle((int)drawCoords.X - 12, (int)drawCoords.Y - 70, 24, 70);
            if (Main.SmartCursorIsUsed)
            {
                interactionRect.X -= 45;
                interactionRect.Y -= 32;
                interactionRect.Width += 90;
                interactionRect.Height += 64;
            }
            var mouseScreen = Helper.ScaledMouseScreen;
            bool hovering = interactionRect.Contains(mouseScreen.ToPoint());
            //AequusHelpers.DrawRectangle(interactionRect, hovering ? Color.Yellow * 0.2f : Color.Red * 0.2f);
            float opacity = 1f - EatenAlpha / 255f;
            if (hovering)
            {
                if (Main.SmartCursorIsUsed)
                {
                    Main.SmartCursorX = 0;
                    Main.SmartCursorY = 0;
                }
                var mouseWorld = mouseScreen + Main.screenPosition;
                if (!NPC.AnyNPCs(ModContent.NPCType<OmegaStarite>()) && Aequus.GameWorldActive && Main.LocalPlayer.IsInTileInteractionRange((int)mouseWorld.X / 16, (int)mouseWorld.Y / 16))
                {
                    var plr = Main.LocalPlayer;
                    plr.noThrow = 2;
                    plr.cursorItemIconEnabled = true;
                    plr.cursorItemIconID = ModContent.ItemType<UltimateSword>();
                    //var highlightTexture = ModContent.GetTexture(AQUtils.GetPath<UltimateSword>("_Highlight"));
                    //Main.spriteBatch.Draw(highlightTexture, drawPos - Main.screenPosition, frame, new Color(255, 255, 255, 255), MathHelper.PiOver4 * 3f, origin, 1f, SpriteEffects.None, 0f);
                    if (Main.mouseRight && Main.mouseRightRelease && cantTouchThis <= 0)
                    {
                        cantTouchThis = 240;
                        plr.tileInteractAttempted = true;
                        plr.tileInteractionHappened = true;
                        plr.releaseUseTile = false;
                        drawCoords += Main.screenPosition;
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            NPC.SpawnBoss((int)drawCoords.X, (int)drawCoords.Y - 1600, ModContent.NPCType<OmegaStarite>(), Main.myPlayer);
                        }
                        else
                        {
                            PacketSystem.Send((p) =>
                            {
                                p.Write((int)drawCoords.X);
                                p.Write((int)drawCoords.Y);
                                p.Write(Main.myPlayer);
                            }, PacketType.SpawnOmegaStarite);
                        }
                        AequusWorld.downedEventCosmic = true;
                        SoundEngine.PlaySound(SoundID.Roar, Main.LocalPlayer.Center);
                        drawCoords -= Main.screenPosition;
                    }

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin_World(shader: true);
                    try
                    {
                        var s = GameShaders.Armor.GetSecondaryShader(ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex, Main.LocalPlayer);
                        var dd = new DrawData(texture, drawCoords, null, Color.White * opacity, MathHelper.PiOver4 * 3f, new Vector2(texture.Width, 0f), 1f, SpriteEffects.None, 0);
                        foreach (var c in Helper.CircularVector(4))
                        {
                            dd.position = drawCoords + c * 2f;
                            s.Apply(null, dd);
                            dd.Draw(Main.spriteBatch);
                        }
                    }
                    catch
                    {

                    }
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin_World(shader: false);
                }
            }
            Main.spriteBatch.Draw(texture, drawCoords, null, Color.White * opacity, MathHelper.PiOver4 * 3f, new Vector2(texture.Width, 0f), 1f, SpriteEffects.None, 0f);
            renderedUltimateSword = true;
        }
    }
}