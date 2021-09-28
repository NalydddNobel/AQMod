using AQMod.Assets.ItemOverlays;
using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Items.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Summon
{
    public class MonoxiderStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[item.type] = true;
            if (!Main.dedServ)
                ItemOverlayLoader.Register(new Glowmask(GlowID.MonoxiderStaff), item.type);
        }

        public override void SetDefaults()
        {
            item.damage = 88;
            item.summon = true;
            item.mana = 10;
            item.width = 26;
            item.height = 28;
            item.useTime = 13;
            item.useAnimation = 13;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 3f;
            item.value = AQItem.PillarWeaponValue;
            item.rare = ItemRarityID.Red;
            item.UseSound = SoundID.Item44;
            item.shoot = ModContent.ProjectileType<Projectiles.Minions.Monoxider>();
            item.buffType = ModContent.BuffType<Buffs.Minion.Monoxider>();
            item.autoReuse = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            player.AddBuff(item.buffType, 2);
            position = Main.MouseWorld;
            return true;
        }

        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.RavenStaff);
            recipe.AddIngredient(ItemID.FragmentSolar, 18);
            recipe.AddIngredient(ItemID.HallowedBar, 20);
            recipe.AddIngredient(ModContent.ItemType<DemonicEnergy>(), 15);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}