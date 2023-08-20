using Aequus.Common.DataSets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.PetsVanity.Familiar {
    public class FamiliarPickaxe : PetItemBase {
        public override int ProjId => ModContent.ProjectileType<FamiliarPet>();
        public override int BuffId => ModContent.BuffType<FamiliarBuff>();

        public override void SetStaticDefaults() {
            ItemSets.DedicatedContent[Type] = new("Crabs", new Color(200, 65, 70, 255)) { UseAnonymousText = true,};
        }

        public override void SetDefaults() {
            base.SetDefaults();
            Item.value = Item.buyPrice(gold: 20);
        }
    }
}