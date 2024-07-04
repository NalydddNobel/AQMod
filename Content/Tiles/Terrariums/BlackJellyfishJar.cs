using AequusRemake.Content.Fishing.Baits.BlackJellyfish;

namespace AequusRemake.Content.Tiles.Terrariums;

public class BlackJellyfishJar : JellyfishJarTemplate {
    public override void SetStaticDefaults() {
        base.SetStaticDefaults();

        JellyfishItem = ModContent.ItemType<BlackJellyfishBait>();
        AnimationShockChance /= 5;
        AddMapEntry(new Color(226, 206, 157), CreateMapEntryName());
    }
}
