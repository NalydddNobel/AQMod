using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace Aequus.Content.TownNPCs.SkyMerchant.Emote;

public class SkyMerchantEmote : ModEmoteBubble {
    public override void SetStaticDefaults() {
        AddToCategory(EmoteID.Category.Town);
    }
}