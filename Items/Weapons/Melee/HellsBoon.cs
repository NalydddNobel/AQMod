using Aequus.Biomes.DemonSiege;
using Aequus.Buffs.Debuffs;
using Aequus.Projectiles.Melee;
using Aequus.Projectiles.Monster;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee
{
    [GlowMask]
    public class HellsBoon : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DemonSiegeSystem.RegisterSacrifice(new SacrificeData(ItemID.LightsBane, Type, UpgradeProgressionType.PreHardmode));
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 46;
            Item.useTime = 38;
            Item.useAnimation = 19;
            Item.autoReuse = true;
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.value = ItemDefaults.DemonSiegeValue;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 3f;
            Item.shootSpeed = 35f;
            Item.shoot = ModContent.ProjectileType<HellsBoonSpawner>();
            Item.scale = 1.2f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor.MaxRGBA(50);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            CorruptionHellfire.AddBuff(target, 240);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            damage = (int)(damage * 0.5f);
            position = Main.MouseWorld;
            player.LimitPointToPlayerReachableArea(ref position);
        }
    }
}