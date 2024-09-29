using Aequus.Common.Entities.ItemAffixes;
using ReLogic.Content;

namespace Aequus.Content.Entities.PotionAffixes;

public abstract class UnifiedPotionAffix : ModPrefix, IShimmerAffix {
    public readonly ModItem Item;

    public Asset<Texture2D>? GlintTexture { get; private set; }

    public UnifiedPotionAffix() {
        Item = new PotionAffixItem(this);
    }

    public override PrefixCategory Category => PrefixCategory.Custom;

    public override void Load() {
        Mod.AddContent(Item);

        GlintTexture = ModContent.Request<Texture2D>($"{this.NamespacePath()}/{Name}Glint");
    }

    bool IShimmerAffix.OnShimmer(Item item) {
        IShimmerAffix.ClearPrefixOnShimmer(item);
        return true;
    }
}
