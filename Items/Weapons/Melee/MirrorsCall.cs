using Aequus.Common.GlobalItems;
using Aequus.Events.GaleStreams.Rewards;
using Aequus.Items.Misc.Energies;
using Aequus.Projectiles.Melee.Swords;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee
{
    public class MirrorsCall : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            AequusItem.Dedicated[Type] = new ItemDedication(new Color(110, 110, 128, 255));
        }

        public override void SetDefaults()
        {
            Item.DefaultToDopeSword<MirrorsCallProj>(32);
            Item.SetWeaponValues(150, 6f, 26);
            Item.width = 20;
            Item.height = 20;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 20);
            Item.scale = 1.2f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool? UseItem(Player player)
        {
            Item.FixSwing(player);
            return null;
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return true;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>($"{Texture}_Glow", AssetRequestMode.ImmediateLoad).Value, position, frame, AequusHelpers.GetRainbowColor(Main.LocalPlayer, Main.GlobalTimeWrappedHourly).UseA(0) * 0.5f,
                0f, origin, scale, SpriteEffects.None, 0f);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            AequusHelpers.GetItemDrawData(Item, out var frame);
            var texture = ModContent.Request<Texture2D>($"{Texture}_Glow", AssetRequestMode.ImmediateLoad).Value;
            Main.spriteBatch.Draw(texture, ItemDefaults.WorldDrawPos(Item, texture) + new Vector2(0f, -2f), frame, AequusHelpers.GetRainbowColor(Main.LocalPlayer, Main.GlobalTimeWrappedHourly).UseA(0) * 0.5f,
                rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
        }

        public override void AddRecipes()
        {
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