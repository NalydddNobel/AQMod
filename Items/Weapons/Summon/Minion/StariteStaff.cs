using Aequus.Buffs.Minion;
using Aequus.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Minion
{
    [AutoloadGlowMask]
    public class StariteStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.SetWeaponValues(20, 5f);
            Item.mana = 10;
            Item.DamageType = DamageClass.Summon;
            Item.rare = ItemDefaults.RarityGlimmer;
            Item.value = ItemDefaults.ValueGlimmer;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item44;
            Item.shoot = ModContent.ProjectileType<StariteMinion>();
            Item.shootSpeed = 8f;
            Item.buffType = ModContent.BuffType<StariteBuff>();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            player.SpawnMinionOnCursor(source, player.whoAmI, type, damage, knockback);
            return false;
        }
    }
}