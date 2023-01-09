using Aequus.Biomes.DemonSiege;
using Aequus.Items.Weapons.Summon.Scepters;
using Aequus.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic
{
    public class Wabbajack : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            SacrificeTotal = 1;
            DemonSiegeSystem.RegisterSacrifice(new SacrificeData(ModContent.ItemType<Revenant>(), Type, UpgradeProgressionType.PreHardmode));
        }

        public override void SetDefaults()
        {
            Item.SetWeaponValues(1, 2f, 11);
            Item.DamageType = DamageClass.Magic;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.width = 32;
            Item.height = 32;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.shoot = ModContent.ProjectileType<WabbajackProj>();
            Item.shootSpeed = 8.5f;
            Item.mana = 30;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item43.WithVolume(1.33f).WithPitchOffset(-0.4f);
            Item.value = Item.buyPrice(gold: 12);
        }
    }
}