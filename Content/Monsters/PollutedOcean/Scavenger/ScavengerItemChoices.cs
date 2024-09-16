using Aequus.Common;
using System.Collections.Generic;

namespace Aequus.Content.Monsters.PollutedOcean.Scavenger;

public class ScavengerItemChoices : LoadedType {
    public readonly List<int> Weapons = [ItemID.FalconBlade, ItemID.GoldBow, ItemID.PlatinumBow];
    public readonly List<int> Head = [ItemID.SilverHelmet, ItemID.TungstenHelmet, ItemID.GoldHelmet, ItemID.PlatinumHelmet, ItemID.DivingHelmet, ItemID.MagicHat];
    public readonly List<int> Body = [ItemID.SilverChainmail, ItemID.TungstenChainmail, ItemID.GoldChainmail, ItemID.PlatinumChainmail, ItemID.Gi, ItemID.GypsyRobe];
    public readonly List<int> Legs = [ItemID.SilverGreaves, ItemID.TungstenGreaves, ItemID.GoldGreaves, ItemID.PlatinumGreaves];
    public readonly List<int> Accs = [ItemID.FrogLeg, ItemID.BalloonPufferfish, ItemID.TsunamiInABottle, ItemID.SailfishBoots];
}
