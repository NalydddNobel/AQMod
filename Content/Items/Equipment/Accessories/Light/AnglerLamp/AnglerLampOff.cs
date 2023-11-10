using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Equipment.Accessories.Light.AnglerLamp;
public class AnglerLampOff : AnglerLampBase {
    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        ContentSamples.CreativeResearchItemPersistentIdOverride[Type] = ModContent.ItemType<AnglerLamp>();
        ItemID.Sets.ShimmerCountsAsItem[Type] = ModContent.ItemType<AnglerLamp>();
    }

    public override void Transform(Player player) {
        if (player.GetModPlayer<AequusPlayer>().accAnglerLamp != null || AnglerLamp.ConsumeGel(player)) {
            Item.Transform<AnglerLamp>();
            (Item.ModItem as AnglerLampBase).animation = 0.5f;
            player.GetModPlayer<AequusPlayer>().anglerLampTime = AnglerLamp.ConsumeRate;
            SoundEngine.PlaySound(SoundID.Item20);
        }
        else {
            animation = 1f;
            SoundEngine.PlaySound(SoundID.Dig);
        }
    }
}