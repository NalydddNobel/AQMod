using Aequus.Common.DataSets;
using Aequus.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.MirrorsCall {
    public class MirrorsCall : ModItem {
        public override void SetStaticDefaults() {
            ItemSets.DedicatedContent[Type] = new("Mr. Gerd26", new Color(110, 110, 128, 255));
        }

        public override void SetDefaults() {
            Item.DefaultToAequusSword<MirrorsCallProj>(7);
            Item.SetWeaponValues(200, 6f, 6);
            Item.width = 24;
            Item.height = 24;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 20);
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override bool? UseItem(Player player) {
            return null;
        }

        public override bool MeleePrefix() {
            return true;
        }

        public override bool AltFunctionUse(Player player) {
            return false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            return true;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            for (float f = 0f; f < 1f; f += 0.125f) {
                var spinningPoint = (f * MathHelper.TwoPi).ToRotationVector2();
                Main.spriteBatch.Draw(
                    AequusTextures.MirrorsCall_Aura,
                    position + spinningPoint * 8f * scale,
                    frame,
                    Helper.GetRainbowColor(Main.LocalPlayer, f * 6f) with { A = 0 } * 0.3f,
                    0f,
                    origin,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) {
            Item.GetItemDrawData(out var frame);
            var texture = AequusTextures.MirrorsCall_Aura;
            var position = ItemDefaults.WorldDrawPos(Item, texture) + new Vector2(0f, -2f);
            var origin = frame.Size() / 2f;

            for (float f = 0f; f < 1f; f += 0.125f) {
                var spinningPoint = (f * MathHelper.TwoPi + Main.GlobalTimeWrappedHourly * 5f).ToRotationVector2();
                Main.spriteBatch.Draw(
                    AequusTextures.MirrorsCall_Aura,
                    position + spinningPoint * 4f * scale,
                    frame,
                    Helper.GetRainbowColor(Main.LocalPlayer, f * 6f) with { A = 0 } * 0.15f,
                    rotation,
                    origin,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.PiercingStarlight)
                .AddIngredient(ModContent.ItemType<Slice.Slice>())
                .AddIngredient(ItemID.LunarBar, 10)
                .AddIngredient(ModContent.ItemType<UltimateEnergy>(), 5)
                .AddIngredient(ItemID.WhitePearl)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}