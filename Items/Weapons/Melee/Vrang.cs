using Aequus.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee
{
    public class Vrang : ModItem
    {
        public override void SetDefaults()
        {
            Item.SetWeaponValues(20, 3f, 8);
            Item.width = 24;
            Item.height = 20;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Melee;
            Item.value = ItemDefaults.GaleStreamsValue;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemDefaults.RarityGaleStreams - 1;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.shootSpeed = 22f;
            Item.shoot = ModContent.ProjectileType<VrangProj>();
        }

        public override bool? UseItem(Player player)
        {
            var aequus = player.GetModPlayer<AequusPlayer>();
            aequus.itemCombo = (ushort)(aequus.itemCombo == 0 ? Item.useAnimation * 2 : 0);
            return null;
        }
    }
}