using AQMod.Assets;
using AQMod.Common.NetCode;
using AQMod.Common.Utilities;
using AQMod.Content.Dusts;
using AQMod.Content.WorldEvents.CosmicEvent;
using AQMod.Effects.WorldEffects;
using AQMod.Items.Weapons.Melee;
using AQMod.NPCs.Boss.Starite;
using AQMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AQMod.Common.Graphics.SceneLayers
{
    public sealed class UltimateSwordWorldOverlay : SceneLayer
    {
        public static LayerKey Key { get; private set; }

        private byte _swordEffectDelay;
        private UnifiedRandom _rand;

        public override string Name => "UltimateSword";
        public override SceneLayering Layering => SceneLayering.BehindNPCs;

        protected override void OnRegister(LayerKey key)
        {
            _rand = new UnifiedRandom();
            Key = key;
        }

        protected override void Draw()
        {
            if (!closeEnoughToDraw() || OmegaStariteScene.SceneType > 1)
                return;
            var drawPos = swordPos();
            if (OmegaStariteScene.OmegaStariteIndexCache == -1)
                OmegaStariteScene.SceneType = 0;
            var texture = TextureCache.GetItem(ModContent.ItemType<UltimateSword>());
            var frame = new Rectangle(0, 0, texture.Width, texture.Height);
            var origin = new Vector2(frame.Width, 0f);
            Main.spriteBatch.Draw(texture, drawPos - Main.screenPosition, frame, new Color(255, 255, 255, 255), MathHelper.PiOver4 * 3f, origin, 1f, SpriteEffects.None, 0f);

            float bobbing = (bobbingSin() + 1f) / 2f;
            var blurTexture = ModContent.GetTexture(AQUtils.GetPath<UltimateSword>("_Blur"));
            var blurFrame = new Rectangle(0, 0, blurTexture.Width, blurTexture.Height);
            var blurOrigin = new Vector2(origin.X, blurTexture.Height - texture.Height);
            Main.spriteBatch.Draw(blurTexture, drawPos - Main.screenPosition, blurFrame, new Color(80 + Main.DiscoR / 60, 80 + Main.DiscoG / 60, 80 + Main.DiscoB / 60, 0) * (1f - bobbing), MathHelper.PiOver4 * 3f, blurOrigin, 1f, SpriteEffects.None, 0f);

            var hitbox = new Rectangle((int)drawPos.X - 10, (int)drawPos.Y - 60, 20, 60);
            Vector2 trueMouseworld = AQUtils.TrueMouseworld;
            if (hitbox.Contains((int)trueMouseworld.X, (int)trueMouseworld.Y) && AQMod.CosmicEvent.IsActive)
            {
                int omegaStariteID = ModContent.NPCType<OmegaStarite>();
                if (OmegaStariteScene.SceneType == 0 && !Main.gameMenu && !Main.gamePaused && Main.LocalPlayer.IsInTileInteractionRange((int)trueMouseworld.X / 16, (int)trueMouseworld.Y / 16))
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
                        NetworkingMethods.GlimmerEventNetSummonOmegaStarite();
                        Main.PlaySound(SoundID.Item, (int)drawPos.X, (int)drawPos.Y, 4, 0.5f, -2.5f);
                    }
                }
            }
        }

        public static float bobbingSin()
        {
            return (float)Math.Sin(Main.GameUpdateCount * 0.0157f);
        }

        public static Vector2 swordPos()
        {
            float x = AQMod.CosmicEvent.tileX * 16f;
            if (Framing.GetTileSafely(AQMod.CosmicEvent.tileX, AQMod.CosmicEvent.tileY).type == ModContent.TileType<GlimmeringStatue>())
                x += 16f;
            else
            {
                x += 8f;
            }
            float y = AQMod.CosmicEvent.tileY * 16 - 80f + bobbingSin() * 8f;
            return new Vector2(x, y);
        }

        private bool closeEnoughToDraw()
        {
            return Main.LocalPlayer.position.X - AQMod.CosmicEvent.tileX * 16f < Main.screenWidth + 200f;
        }

        public override void Update()
        {
            if (!AQMod.CosmicEvent.IsActive || OmegaStariteScene.SceneType > 1 || !closeEnoughToDraw())
                return;
            var position = swordPos();
            Lighting.AddLight(position, new Vector3(1f, 1f, 1f));
            if (_rand.NextBool(10))
            {
                int d = Dust.NewDust(position + new Vector2(_rand.Next(-6, 6), -_rand.Next(60)), 2, 2, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(160, 160, 160, 80));
                Main.dust[d].velocity *= 0.1f;
                Main.dust[d].noGravity = true;
            }
            if (_swordEffectDelay > 0)
                _swordEffectDelay--;
            else if (_rand.NextBool(10 + (int)(20 * AQMod.EffectIntensityMinus)))
            {
                AQMod.WorldEffects.Add(new UltimateSwordEffect(_rand));
                _swordEffectDelay = (byte)(int)(8 * AQMod.EffectIntensityMinus);
            }
        }
    }
}