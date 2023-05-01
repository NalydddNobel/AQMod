using Aequus;
using Aequus.Items;
using Aequus.Projectiles.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items.DebugItems {
    public class PhysicsGun2 : ModItem {
        public Asset<Texture2D> GlowTexture => ModContent.Request<Texture2D>(Texture + "_Glow", AssetRequestMode.ImmediateLoad);

        public override bool IsLoadingEnabled(Mod mod) {
            return Aequus.DebugFeatures;
        }

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 0;
        }

        public override void SetDefaults() {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 20);
            Item.shoot = ModContent.ProjectileType<SuperPhysicsGunProj>();
            Item.shootSpeed = 2f;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            tooltips.Insert(tooltips.GetIndex("PickPower"), new TooltipLine(Mod, "PickPower", "Infinite" + Lang.tip[26].Value));
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            try {
                var texture = TextureAssets.Item[Type].Value;

                var coloring = Main.mouseColor;
                var glowTexture = GlowTexture.Value;

                spriteBatch.Draw(texture, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);

                foreach (var v in Helper.CircularVector(8, Main.GlobalTimeWrappedHourly)) {
                    spriteBatch.Draw(glowTexture, position + v * scale * 2f, frame, (coloring * Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.05f, 0.2f)).UseA(20), 0f, origin, scale, SpriteEffects.None, 0f);
                }

                foreach (var v in Helper.CircularVector(4)) {
                    spriteBatch.Draw(glowTexture, position + v * scale * 2f, frame, (coloring * Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.2f, 0.5f)).UseA(100), 0f, origin, scale, SpriteEffects.None, 0f);
                }
                spriteBatch.Draw(glowTexture, position, frame, coloring, 0f, origin, scale, SpriteEffects.None, 0f);
            }
            catch {
                return true;
            }
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
            try {
                var texture = TextureAssets.Item[Type].Value;
                var frame = texture.Frame();
                var origin = frame.Size() / 2f;
                var drawCoordinates = Item.position - Main.screenPosition + origin + new Vector2(Item.width / 2 - origin.X, Item.height - frame.Height);
                var itemOrigin = frame.Size() / 2f;

                var coloring = Main.mouseColor;
                var glowTexture = GlowTexture.Value;

                spriteBatch.Draw(texture, drawCoordinates, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);

                foreach (var v in Helper.CircularVector(8, Main.GlobalTimeWrappedHourly)) {
                    spriteBatch.Draw(glowTexture, drawCoordinates + v * scale * 2f, frame, (coloring * Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.05f, 0.2f)).UseA(20), rotation, origin, scale, SpriteEffects.None, 0f);
                }

                foreach (var v in Helper.CircularVector(4)) {
                    spriteBatch.Draw(glowTexture, drawCoordinates + v * scale * 2f, frame, (coloring * Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.2f, 0.5f)).UseA(100), rotation, origin, scale, SpriteEffects.None, 0f);
                }
                spriteBatch.Draw(glowTexture, drawCoordinates, frame, coloring, rotation, origin, scale, SpriteEffects.None, 0f);
            }
            catch {
                return true;
            }
            return false;
        }
    }
}