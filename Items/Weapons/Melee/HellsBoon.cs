using AQMod.Assets.LegacyItemOverlays;
using AQMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class HellsBoon : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.Glowmask(() => new Color(200, 200, 200, 0) * AQUtils.Wave(Main.GlobalTime * 6f, 0.9f, 1f));
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 40;
            item.damage = 48;
            item.useTime = 38;
            item.useAnimation = 19;
            item.autoReuse = true;
            item.rare = AQItem.Rarities.GoreNestRare;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(gold: 1);
            item.melee = true;
            item.knockBack = 3f;
            item.shootSpeed = 35f;
            item.shoot = ModContent.ProjectileType<HellsBoonSpike>();
            item.scale = 1.2f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return AQItem.Commons.DemonSiegeItem_GetAlpha(lightColor);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            Buffs.Debuffs.CorruptionHellfire.Inflict(target, 240);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            damage = (int)(damage * 0.5f);
            HellsBoonSpike.SpawnCluster(Main.MouseWorld, (int)(item.shootSpeed / player.meleeSpeed), damage, knockBack, player);
            return false;
        }
    }
}