using System.IO;
using Terraria.ModLoader.IO;

namespace Aequus.Items.Misc.PermanentUpgrades;

public class ShimmerCoinSystem : ModSystem {
    public const string TAG_NAME = "CoinStacks";

    public override void SaveWorldData(TagCompound tag) {
        if (ShimmerCoin.TimesUsed > 0) {
            tag[TAG_NAME] = ShimmerCoin.TimesUsed;
        }
    }

    public override void LoadWorldData(TagCompound tag) {
        if (tag.TryGet(TAG_NAME, out int timesUsed)) {
            ShimmerCoin.TimesUsed = timesUsed;
        }
    }

    public override void NetSend(BinaryWriter writer) {
        writer.Write((byte)ShimmerCoin.TimesUsed);
    }

    public override void NetReceive(BinaryReader reader) {
        ShimmerCoin.TimesUsed = reader.ReadByte();
    }
}
