using Aequus.Common.Items;
using Aequus.Content.DedicatedContent.TorraTh;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.DedicatedContent.TorraTh;

public class SwagLookingEye : PetItemBase, IDedicatedItem {
    public override int ProjId => ModContent.ProjectileType<TorraPet>();
    public override int BuffId => ModContent.BuffType<TorraBuff>();

    public string DisplayedDedicateeName => "torra th";
    public Color TextColor => new Color(80, 60, 255);

    public override void SetStaticDefaults() {
        ItemID.Sets.ShimmerTransformToItem[ItemID.SuspiciousLookingEye] = Type;
    }

    public override void SetDefaults() {
        base.SetDefaults();
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.value = Item.sellPrice(gold: 1);
    }
}