using AQMod.Assets.Textures;
using AQMod.Common.Utilities;
using AQMod.Content.Dusts;
using AQMod.Content.WorldEvents;
using AQMod.NPCs.Starite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Assets.SceneLayers
{
    public class UltimateSwordWorldOverlay : SceneLayer
    {
        public const string KEY = "AQMod:UltimateSword";

        private ushort _ultimateSwordChatTimer;
        private float _ultimateSwordOffsetY;

        public static int ItemType()
        {
            return ModContent.ItemType<Items.BossItems.Starite.UltimateSword>();
        }

        public float UltimateSwordOffsetY()
        {
            return _ultimateSwordOffsetY == 0f ? (float)Math.Sin(Main.GlobalTime) * 8f : _ultimateSwordOffsetY;
        }

        public static Vector2 GetPosition(float ultimateSwordOffsetY)
        {
            return new Vector2(GlimmerEvent.X * 16f, GlimmerEvent.Y * 16f - 120f + ultimateSwordOffsetY);
        }

        public UltimateSwordWorldOverlay()
        {
            _ultimateSwordChatTimer = 0;
            _ultimateSwordOffsetY = 0f;
        }

        public override void Draw()
        {
            if (!Visible())
            {
                return;
            }
            if (AQMod.omegaStariteIndexCache == -1)
            {
                AQMod.omegaStariteScene = 0;
                _ultimateSwordOffsetY = 0f;
            }
            var offset = UltimateSwordOffsetY();
            var position = GetPosition(offset);
            if (!GlimmerEvent.FakeActive)
                position.X += 8f;
            var ultimateSwordID = ItemType();
            var texture = Main.itemTexture[ultimateSwordID];
            var frame = new Rectangle(0, 0, texture.Width, texture.Height);
            var origin = frame.Size() / 2f;
            Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, new Color(255, 255, 255, 255), MathHelper.PiOver4 * 3f, origin, 1f, SpriteEffects.None, 0f);
            float x = (float)Math.Sin(Main.GlobalTime / 2f) * 4f;
            Main.spriteBatch.Draw(texture, position + new Vector2(x, 0f) - Main.screenPosition, frame, new Color(75, 75, 75, 0), MathHelper.PiOver4 * 3f, origin, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, position + new Vector2(-x, 0f) - Main.screenPosition, frame, new Color(75, 75, 75, 0), MathHelper.PiOver4 * 3f, origin, 1f, SpriteEffects.None, 0f);
            var hitbox = Utils.CenteredRectangle(position, new Vector2(30f, 80f));
            Vector2 trueMouseworld = AQUtils.TrueMouseworld;
            if (hitbox.Contains((int)trueMouseworld.X, (int)trueMouseworld.Y) && GlimmerEvent.ActuallyActive)
            {
                int omegaStariteID = ModContent.NPCType<OmegaStarite>();
                if (AQMod.omegaStariteScene == 0 && !Main.gameMenu && !Main.gamePaused && Main.LocalPlayer.IsInTileInteractionRange((int)position.X / 16, (int)position.Y / 16))
                {
                    var plr = Main.LocalPlayer;
                    plr.mouseInterface = true;
                    plr.noThrow = 2;
                    plr.showItemIcon = true;
                    plr.showItemIcon2 = ultimateSwordID;
                    texture = DrawUtils.Textures.Extras[ExtraID.UltimateSwordHighlight];
                    frame = new Rectangle(0, 0, texture.Width, texture.Height);
                    Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, new Color(255, 255, 255, 255), MathHelper.PiOver4 * 3f, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);
                    if (Main.mouseRight && Main.mouseRightRelease)
                    {
                        plr.tileInteractAttempted = true;
                        plr.tileInteractionHappened = true;
                        plr.releaseUseTile = false;
                        if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            AQMod.Instance.GetPacket().Write((byte)0);
                        }
                        else
                        {
                            AQMod.summonOmegaStarite = true;
                        }
                        Main.PlaySound(SoundID.Item, (int)plr.position.X + plr.width / 2, (int)plr.position.Y + plr.height / 2, 4, 0.5f, -2.5f);
                        _ultimateSwordOffsetY = offset;
                    }
                }
            }
        }

        public bool Visible()
        {
            //Main.NewText(!Main.dayTime && GlimmerEvent.IsActive && (AQMod.omegaStariteIndexCache == -1 || AQMod.omegaStariteScene == 1) && AQMod.omegaStariteScene != 3 && Vector2.Distance(SpriteUtils.WorldScreenCenter, GetPosition(UltimateSwordOffsetY())) < Main.screenWidth + Main.screenHeight);
            return !Main.dayTime && GlimmerEvent.IsActive && (AQMod.omegaStariteIndexCache == -1 || AQMod.omegaStariteScene == 1) && AQMod.omegaStariteScene != 3 && Vector2.Distance(DrawUtils.WorldScreenCenter, GetPosition(UltimateSwordOffsetY())) < Main.screenWidth + Main.screenHeight;
        }

        public override void Update()
        {
            if (!GlimmerEvent.IsActive)
            {
                return;
            }
            if (AQMod.omegaStariteScene == 1)
            {
                _ultimateSwordOffsetY = MathHelper.Lerp(_ultimateSwordOffsetY, -320f, 0.05f);
                float offsetY = UltimateSwordOffsetY();
                var dustPosition = GetPosition(UltimateSwordOffsetY());
                int dustChance = 10 - (int)(offsetY - _ultimateSwordOffsetY).Abs();
                if (dustChance < 2 || Main.rand.NextBool(dustChance))
                {
                    int d = Dust.NewDust(dustPosition + new Vector2(-8f, 24f), 16, 16, ModContent.DustType<UltimaDust>());
                    Main.dust[d].scale = 1.5f;
                    Main.dust[d].velocity.X *= 0.05f;
                    Main.dust[d].velocity.Y = Main.rand.NextFloat(5f, 9f);
                }
            }
            if (GlimmerEvent.ActuallyActive && Visible())
            {
                var position = GetPosition(UltimateSwordOffsetY());
                _ultimateSwordChatTimer++;
                if (_ultimateSwordChatTimer >= 2700)
                {
                    _ultimateSwordChatTimer = 0;
                    Main.NewText("what.");
                    Main.PlaySound(AQMod.Instance.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/StariteSpeech").WithVolume(0.2f * Main.ambientVolume).WithPitchVariance(1f), position);
                }
            }
        }
    }
}