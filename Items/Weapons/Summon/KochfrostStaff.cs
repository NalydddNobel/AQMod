using AQMod.Buffs.Summon;
using AQMod.Items.Materials.Energies;
using AQMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Summon
{
    public class KochfrostStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[item.type] = true;
            this.Glowmask();
        }

        public override void SetDefaults()
        {
            item.damage = 80;
            item.summon = true;
            item.mana = 10;
            item.width = 26;
            item.height = 28;
            item.useTime = 13;
            item.useAnimation = 13;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 3f;
            item.value = AQItem.Prices.GaleStreamsWeaponValue;
            item.rare = AQItem.Rarities.GaleStreamsRare;
            item.UseSound = SoundID.Item46;
            item.shoot = ModContent.ProjectileType<SnowsawMinion>();
            item.buffType = ModContent.BuffType<SnowsawBuff>();
            item.autoReuse = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            player.AddBuff(item.buffType, 2);
            position = Main.MouseWorld;
            AQPlayer.HeadMinionSummonCheck(player.whoAmI, type);
            return true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.HornetStaff);
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>());
            r.AddIngredient(ModContent.ItemType<Materials.SiphonTentacle>(), 12);
            r.AddIngredient(ItemID.SoulofFlight, 20);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}