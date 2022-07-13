using Aequus.Items.Misc.Energies;
using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Aequus.Items.Weapons.Ranged
{
    public class BoundBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.width = 30;
            Item.height = 30;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ProjectileID.UnholyArrow;
            Item.shootSpeed = 16.5f;
            Item.UseSound = SoundID.Item5;
            Item.value = Item.sellPrice(gold: 2, silver: 75);
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.knockBack = 4f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 80);
        }

        public override bool CanUseItem(Player player)
        {
            return player.Aequus().boundBowAmmo > 0;
        }

        public override bool? UseItem(Player player)
        {
            var aequus = player.Aequus();
            aequus.boundBowAmmo--;
            aequus.boundBowAmmoTimer = 120;
            return true;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Main.playerInventory)
                return;

            var center = ItemSlotRenderer.InventoryItemGetCorner(position, frame, scale);
            center.X -= TextureAssets.InventoryBack.Value.Width / 2f * Main.inventoryScale;
            center.Y += TextureAssets.InventoryBack.Value.Height / 2f * Main.inventoryScale;
            string ammo = Main.LocalPlayer.Aequus().boundBowAmmo.ToString();

            var color = Color.Lerp(Color.BlueViolet, Color.White, AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.45f, 1f));
            var font = FontAssets.MouseText.Value;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, ammo, center + new Vector2(8f, -24f) * Main.inventoryScale, color, 0f, Vector2.Zero, new Vector2(1f) * Main.inventoryScale * 0.8f, spread: Main.inventoryScale);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.EbonwoodBow)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddIngredient<DemonicEnergy>()
                .AddTile(TileID.Anvils)
                .RegisterBefore(ItemID.OnyxBlaster);
            CreateRecipe()
                .AddIngredient(ItemID.ShadewoodBow)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddIngredient<DemonicEnergy>()
                .AddTile(TileID.Anvils)
                .RegisterBefore(ItemID.OnyxBlaster);
        }
    }
}