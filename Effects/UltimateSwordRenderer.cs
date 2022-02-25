using AQMod.Common;
using AQMod.Common.Graphics;
using AQMod.Content.Players;
using AQMod.Content.World.Events;
using AQMod.Dusts;
using AQMod.Items.Weapons.Melee;
using AQMod.NPCs.Bosses;
using AQMod.Tiles.Nature;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Effects
{
    public static class UltimateSwordRenderer
    {
        public class DustEffect
        {
            private float _time;
            private readonly float _maxWidth;
            private readonly float _maxHeight;
            private byte _existence;
            private readonly byte _lifespan;
            private readonly byte _reps;
            private Color _dustColor;

            public DustEffect()
            {
                _time = FX.Rand(0f, MathHelper.PiOver2 * 3f);
                _maxWidth = FX.Rand(10f, 35f);
                _maxHeight = FX.Rand(55f, 120f);
                _reps = (byte)(FX.Rand(4) + 1);
                _lifespan = (byte)(FX.Rand(50, 150) + 1);
                _existence = (byte)FX.Rand(_lifespan / 3);
                int colorType = (int)FX.Rand(9f);
                switch (colorType)
                {
                    default:
                        {
                            _dustColor = new Color(255, 255, 100, 0);
                        }
                        break;

                    case 1:
                        {
                            _dustColor = new Color(140, 50, 255, 0);
                        }
                        break;

                    case 2:
                        {
                            _dustColor = new Color(255, 255, 255, 0);
                        }
                        break;

                    case 3:
                        {
                            _dustColor = new Color(255, 100, 255, 0);
                        }
                        break;

                    case 4:
                        {
                            _dustColor = new Color(255, 160, 255, 0);
                        }
                        break;

                    case 5:
                        {
                            _dustColor = new Color(160, 255, 180, 0);
                        }
                        break;

                    case 6:
                        {
                            _dustColor = new Color(40, 255, 150, 0);
                        }
                        break;

                    case 7:
                        {
                            _dustColor = new Color(180, 255, 50, 0);
                        }
                        break;

                    case 8:
                        {
                            _dustColor = new Color(255, 100, 180, 0);
                        }
                        break;
                }
            }

            public bool Update()
            {
                for (int i = 0; i < _reps; i++)
                {
                    _existence++;
                    float progress = 1f - (_existence / (float)_lifespan);
                    if (_time < MathHelper.TwoPi)
                    {
                        _time += _existence * 0.0008f;
                    }
                    else
                    {
                        _time += _existence * (0.0005f * (1f - progress));
                    }
                    int x = (int)(Math.Sin(_time) * _maxWidth);
                    int y = (int)((1f - progress) * _maxHeight);
                    y += (int)(Glimmer.tileY * 16f - 80f + (float)Math.Sin(Main.GameUpdateCount * 0.0157f) * 8f);
                    x += Glimmer.tileX * 16;
                    if (Framing.GetTileSafely(Glimmer.tileX, Glimmer.tileY).type == ModContent.TileType<GlimmeringStatue>())
                    {
                        x += 16;
                    }
                    else
                    {
                        x += 8;
                    }
                    int d = Dust.NewDust(new Vector2(x, y), 2, 2, ModContent.DustType<MonoDust>(), 0f, 0f, 0, _dustColor);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0.1f;
                    if (AQConfigClient.c_EffectQuality >= 1f)
                    {
                        float p = 1f - progress * 0.5f;
                        Main.dust[d].scale *= p * p;
                        if (FX.RandChance(20))
                        {
                            Main.dust[d].scale *= FX.Rand(1.1f, 1.5f);
                        }
                        Main.dust[d].color *= p;
                        if (FX.RandChance(15))
                        {
                            d = Dust.NewDust(new Vector2(x, y), 2, 2, ModContent.DustType<MonoDust>(), 0f, 0f, 0, _dustColor);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 0.8f;
                        }
                    }
                    if (_existence >= _lifespan)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        internal static byte UltimateSwordEffectDelay;
        private static List<DustEffect> _fx;

        internal static void Initalize()
        {
            UltimateSwordEffectDelay = 0;
            if (_fx == null)
            {
                _fx = new List<DustEffect>();
            }
            else
            {
                _fx.Clear();
            }
        }

        internal static void Render()
        {
            bool renderUltimateSword = Glimmer.IsGlimmerEventCurrentlyActive() && Glimmer.deactivationDelay == -1;
            if (renderUltimateSword && Glimmer.omegaStarite != -1)
            {
                renderUltimateSword = (int)Main.npc[Glimmer.omegaStarite].ai[0] == OmegaStarite.PHASE_NOVA;
            }

            if (renderUltimateSword)
            {
                AQGraphics.SetCullPadding(padding: 360);
                renderUltimateSword = AQGraphics.Cull(new Vector2(Glimmer.tileX * 16f, Glimmer.tileY * 16f) - Main.screenPosition);
            }

            if (renderUltimateSword)
            {
                float x = Glimmer.tileX * 16f;
                if (Framing.GetTileSafely(Glimmer.tileX, Glimmer.tileY).type == ModContent.TileType<GlimmeringStatue>())
                {
                    x += 16f;
                }
                else
                {
                    x += 8f;
                }
                float y = Glimmer.tileY * 16 - 80f + (float)Math.Sin(Main.GameUpdateCount * 0.0157f) * 8f;
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
                if (AQPlayer._ImportantInteractionDelay() && hitbox.Contains((int)trueMouseworld.X, (int)trueMouseworld.Y))
                {
                    if (Glimmer.omegaStarite == -1 && !Main.gameMenu && !Main.gamePaused && Main.LocalPlayer.IsInTileInteractionRange((int)trueMouseworld.X / 16, (int)trueMouseworld.Y / 16))
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
                            AQPlayer._ImportantInteractionDelay(apply: 1800);
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
        }

        internal static void Update()
        {
            if (!Glimmer.IsGlimmerEventCurrentlyActive() || Glimmer.omegaStarite != -1 || Glimmer.deactivationDelay == -1 || AQGraphics.Cull(Utils.CenteredRectangle(new Vector2(Glimmer.tileX, Glimmer.tileY) - Main.screenPosition, new Vector2(80f, 160f))))
            {
                if (_fx != null)
                {
                    _fx.Clear();
                    _fx = null;
                }
                return;
            }
            float x = Glimmer.tileX * 16f;
            if (Framing.GetTileSafely(Glimmer.tileX, Glimmer.tileY).type == ModContent.TileType<GlimmeringStatue>())
            {
                x += 16f;
            }
            else
            {
                x += 8f;
            }
            float y = Glimmer.tileY * 16 - 80f + (float)Math.Sin(Main.GameUpdateCount * 0.0157f) * 8f;
            Lighting.AddLight(new Vector2(x, y), new Vector3(1f, 1f, 1f));
            if (FX.RandChance(10))
            {
                int d = Dust.NewDust(new Vector2(x, y) + new Vector2(FX.Rand(-6f, 6f), -FX.Rand(60)), 2, 2, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(160, 160, 160, 80));
                Main.dust[d].velocity *= 0.1f;
                Main.dust[d].noGravity = true;
            }
            if (UltimateSwordEffectDelay > 0)
            {
                UltimateSwordEffectDelay--;
            }
            else if (FX.RandChance(10 + (int)(20 * (1f - AQConfigClient.c_EffectIntensity))))
            {
                _fx.Add(new DustEffect());
                UltimateSwordEffectDelay = (byte)(int)(8 * (1f - AQConfigClient.c_EffectIntensity));
            }
        }
    }
}
