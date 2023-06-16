using Aequus.Common.DataSets;
using Aequus.Items.Materials.Energies;
using Aequus.Projectiles.Melee.Swords;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
            Item.DefaultToDopeSword<MirrorsCallProj>(32);
            Item.SetWeaponValues(150, 6f, 6);
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
            Item.FixSwing(player);
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
                    position + spinningPoint * 2f * scale,
                    frame,
                    Color.Red.HueSet(f) with { A = 0},
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
            var texture = ModContent.Request<Texture2D>($"{Texture}_Glow", AssetRequestMode.ImmediateLoad).Value;
            Main.spriteBatch.Draw(texture, ItemDefaults.WorldDrawPos(Item, texture) + new Vector2(0f, -2f), frame, Helper.GetRainbowColor(Main.LocalPlayer, Main.GlobalTimeWrappedHourly).UseA(0) * 0.5f,
                rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.PiercingStarlight)
                .AddIngredient(ModContent.ItemType<Slice>())
                .AddIngredient(ItemID.LunarBar, 10)
                .AddIngredient(ModContent.ItemType<UltimateEnergy>(), 5)
                .AddIngredient(ItemID.WhitePearl)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}