using Aequus.Common.Items;
using Aequus.Content.Events.DemonSiege;
using Aequus.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic.Demon.Triacanthorn {
    [AutoloadGlowMask]
    public class Triacanthorn : ModItem {
        public override void SetStaticDefaults() {
            Item.staff[Type] = true;
            DemonSiegeSystem.RegisterSacrifice(new SacrificeData(ItemID.Vilethorn, Type, EventTier.PreHardmode));
        }

        public override void SetDefaults() {
            Item.width = 28;
            Item.height = 28;
            Item.damage = 17;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.autoReuse = true;
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item83;
            Item.value = ItemDefaults.ValueDemonSiege;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.knockBack = 1f;
            Item.shoot = ModContent.ProjectileType<TriacanthornProj>();
            Item.ArmorPenetration = 10;
            Item.shootSpeed = 16f;
            Item.noMelee = true;
        }

        public override Color? GetAlpha(Color lightColor) {
            return lightColor.MaxRGBA(200);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
            position += Vector2.Normalize(velocity) * 34f;
        }
    }
}