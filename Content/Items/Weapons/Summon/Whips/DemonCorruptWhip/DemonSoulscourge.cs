using Aequus.Common;
using Aequus.Core.ContentGeneration;

namespace Aequus.Content.Items.Weapons.Summon.Whips.DemonCorruptWhip;

[WorkInProgress]
public class DemonSoulscourge : UnifiedWhipItem {
    public override void SetDefaults() {
        Item.DefaultToWhip(ProjectileID.BoneWhip, 32, 2f, 8f, animationTotalTime: 30);
        Item.rare = Commons.Rare.DemonSiegeLoot;
        Item.value = Commons.Cost.DemonSiegeLoot;
    }
}
