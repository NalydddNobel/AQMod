using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Summon
{
    public class CursedKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[item.type] = true;
            this.Glowmask(() => new Color(200, 200, 200, 0) * AQUtils.Wave(Main.GlobalTime * 6f, 0.9f, 1f));
        }

        public override void SetDefaults()
        {
            item.damage = 26;
            item.summon = true;
            item.mana = 10;
            item.width = 26;
            item.height = 28;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.knockBack = 8f;
            item.value = AQItem.Prices.DemonSiegeWeaponValue;
            item.rare = AQItem.RarityDemonSiege;
            item.UseSound = SoundID.Item78;
            item.shoot = ModContent.ProjectileType<Projectiles.Summon.TrapperMinion>();
            item.buffType = ModContent.BuffType<Buffs.Summon.Trapper>();
            item.autoReuse = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            player.AddBuff(item.buffType, 2);
            return true;
        }
    }
}