using Aequus.Common.Items;
using Aequus.Items.Materials.GaleStreams;
using Aequus.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Swords.GaleStreams.ThunderClap {
    public class ThunderClap : ModItem {
        public override void SetDefaults() {
            Item.SetWeaponValues(55, 32f, 16);
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 100;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemDefaults.RarityGaleStreams;
            Item.value = ItemDefaults.ValueGaleStreams;
            Item.DamageType = DamageClass.Melee;
            Item.shoot = ModContent.ProjectileType<ThunderClapProj>();
            Item.shootSpeed = 1f;
            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public override bool? UseItem(Player player) {
            Item.FixSwing(player);
            return null;
        }

        public override bool MeleePrefix() {
            return true;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.CobaltBar, 14)
                .AddIngredient<Fluorescence>(20)
                .AddTile(TileID.Anvils)
                .Register()
                .Clone().ReplaceItem(ItemID.CobaltBar, ItemID.PalladiumBar).Register();
        }
    }
}