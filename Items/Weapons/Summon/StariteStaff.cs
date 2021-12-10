using AQMod.Assets.LegacyItemOverlays;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Summon
{
    public class StariteStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[item.type] = true;
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new GlowmaskOverlay(this.GetPath("_Glow")), item.type);
        }

        public override void SetDefaults()
        {
            item.damage = 18;
            item.summon = true;
            item.mana = 10;
            item.width = 26;
            item.height = 28;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = AQItem.Prices.GlimmerWeaponValue;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item44;
            item.shoot = ModContent.ProjectileType<Projectiles.Summon.StariteMinion.StariteLeader>();
            item.buffType = ModContent.BuffType<Buffs.Summon.Starite>();
            item.autoReuse = true;
            item.shootSpeed = 8f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            player.AddBuff(item.buffType, 2);
            position = Main.MouseWorld;
            int stariteParent = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            int stariteChild = Projectile.NewProjectile(position + new Vector2(30f, 10f), new Vector2(speedX, speedY), ModContent.ProjectileType<Projectiles.Summon.StariteMinion.Starite>(), damage, knockBack, player.whoAmI, stariteParent + 1);
            Main.projectile[stariteChild].minionSlots = 0;
            stariteChild = Projectile.NewProjectile(position + new Vector2(-30f, 10f), new Vector2(speedX, speedY), ModContent.ProjectileType<Projectiles.Summon.StariteMinion.Starite>(), damage, knockBack, player.whoAmI, stariteParent + 1);
            Main.projectile[stariteChild].minionSlots = 0;
            return false;
        }
    }
}