using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged.Misc.Slingshot {
    public class SlingshotAmmos : GlobalItem {
        public static int BirdAmmo;

        public override void Load() {
            BirdAmmo = ItemID.Bird;
        }

        public override void SetDefaults(Item item) {
            if (item.type == ItemID.Bird || item.type == ItemID.Cardinal || item.type == ItemID.BlueJay ||
                item.type == ItemID.Duck || item.type == ItemID.MallardDuck || item.type == ItemID.Seagull ||
                item.type == ItemID.Grebe || item.type == ItemID.GoldBird) {
                item.ammo = BirdAmmo;
            }
        }
    }
}