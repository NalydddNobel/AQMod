using Aequus.Common.Items;
using Aequus.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Thrown {
    public class Vrang : ModItem
    {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.SetWeaponValues(20, 3f, 8);
            Item.width = 24;
            Item.height = 20;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Melee;
            Item.value = ItemDefaults.ValueGaleStreams / 4;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemDefaults.RarityGaleStreams - 2;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.shootSpeed = 22f;
            Item.shoot = ModContent.ProjectileType<VrangProj>();
            Item.Aequus().itemGravityCheck = 255;
        }

        public override bool? UseItem(Player player)
        {
            var aequus = player.GetModPlayer<AequusPlayer>();
            aequus.itemCombo = (ushort)(aequus.itemCombo == 0 ? Item.useAnimation * 2 : 0);
            return null;
        }
    }
}