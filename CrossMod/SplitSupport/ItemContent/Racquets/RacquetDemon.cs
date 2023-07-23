using Aequus.Common;
using Aequus.Common.Items;

namespace Aequus.CrossMod.SplitSupport.ItemContent.Racquets;

[ModRequired("Split")]
[WorkInProgress()]
public class RacquetDemon : RacquetBase {
    public override int BallCount => 1;

    public override void SetDefaults() {
        base.SetDefaults();
        Item.SetWeaponValues(80, 2f, 0);
        Item.width = 32;
        Item.height = 32;
        Item.useTime = 40;
        Item.useAnimation = 40;
        Item.value = ItemDefaults.ValueDemonSiegeTier2;
        Item.rare = ItemDefaults.RarityDemonSiegeTier2;
        Item.autoReuse = true;
        //Item.shoot = ModContent.ProjectileType<BaseballDemon>();
    }
}