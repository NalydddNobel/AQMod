using Aequus.Common;
using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Zombie.Corruption {
    [AutoloadGlowMask]
    [WorkInProgress]
    public class ZombieSceptre : SceptreBase {
        public override Color GlowColor => Color.Blue;
        public override int DustSpawn => ModContent.DustType<ZombieSceptreParticle>();

        public override void SetDefaults() {
            base.SetDefaults();
            Item.DefaultToNecromancy(10);
            Item.SetWeaponValues(4, 1f, 0);
            Item.shootSpeed = 2f;
            Item.shoot = ModContent.ProjectileType<ZombieScepterMinion>();
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 50);
            Item.mana = 6;
            Item.autoReuse = true;
        }

        public override void HoldItem(Player player) {
            if (player.ownedProjectileCounts[Item.shoot] <= 0) {
                player.ownedProjectileCounts[Item.shoot]++;
                if (Main.myPlayer == player.whoAmI) {
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Main.rand.NextVector2Circular(1f, 1f), Item.shoot, Item.damage, Item.knockBack, player.whoAmI);
                }
            }
        }

        public override void AddRecipes() {
#if DEBUG
            CreateRecipe()
                .AddIngredient(ItemID.DemoniteBar, 6)
                .AddIngredient(ItemID.LifeCrystal)
                .AddTile(TileID.Anvils)
                .TryRegisterAfter(ItemID.RainbowRod);
#endif
        }
    }
}