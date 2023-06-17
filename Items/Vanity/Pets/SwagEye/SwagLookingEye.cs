using Aequus.Common.DataSets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Vanity.Pets.SwagEye {
    public class SwagLookingEye : PetItemBase {
        public override int ProjId => ModContent.ProjectileType<TorraPet>();
        public override int BuffId => ModContent.BuffType<TorraBuff>();

        public override void SetStaticDefaults() {
            ItemSets.DedicatedContent[Type] = new("torra th", new Color(80, 60, 255, 255));
            ItemID.Sets.ShimmerTransformToItem[ItemID.SuspiciousLookingEye] = Type;
        }

        public override void SetDefaults() {
            base.SetDefaults();
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.sellPrice(gold: 1);
        }
    }
}