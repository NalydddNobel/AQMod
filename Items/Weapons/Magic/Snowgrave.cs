using AQMod.Buffs.Debuffs;
using AQMod.Items.Materials.Energies;
using AQMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public class Snowgrave : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 80;
            item.height = 80;
            item.damage = 1800;
            item.knockBack = 0f;
            item.magic = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 160;
            item.useAnimation = 160;
            item.rare = AQItem.RarityGaleStreams;
            item.shootSpeed = 32f;
            item.autoReuse = true;
            item.noMelee = true;
            item.value = AQItem.Prices.GaleStreamsWeaponValue;
            item.mana = 200;
            item.shoot = ModContent.ProjectileType<SnowgraveProjectileSpawner>();
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            reduce = MathHelper.Lerp(reduce, 1f, 0.8f); // mana cost is 80% less effective on this item!
            mult = MathHelper.Lerp(reduce, 1f, 0.8f);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            player.AddBuff(ModContent.BuffType<ManaDrain>(), 360);
            position.X = Main.MouseWorld.X;
            position.Y += 600f;
            speedX = 0f;
            speedY = 0f;
            return true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.WaterBolt);
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>());
            r.AddIngredient(ModContent.ItemType<Materials.SiphonTentacle>(), 12);
            r.AddIngredient(ItemID.SoulofFlight, 20);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}