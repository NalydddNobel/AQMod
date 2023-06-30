using Aequus.Common;
using Aequus.Common.Items;
using Aequus.Content;
using Aequus.Projectiles.Misc;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Unused.Items {
    [UnusedContent]
    public class Mendshroom : ModItem, ItemHooks.IUpdateItemDye {
        public override void SetStaticDefaults() {
            SentryAccessoriesDatabase.OnAI.Add(Type, SentryAccessoriesDatabase.ApplyEquipFunctional_AI);
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory();
            Item.rare = ItemDefaults.RarityCrabCrevice;
            Item.value = ItemDefaults.ValueCrabCrevice;
            Item.shoot = ModContent.ProjectileType<MendshroomProj>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().accMendshroom = Item;
        }

        public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
            player.Aequus().cMendshroom = dyeItem.dye;
        }
    }
}