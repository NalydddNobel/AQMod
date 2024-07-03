using Aequu2.Core.Components.Prefixes;
using Aequu2.DataSets;
using ReLogic.Content;

namespace Aequu2.Old.Content.Items.Potions.Prefixes;

public abstract class PotionPrefix : ModPrefix, IRemovedByShimmerPrefix {
    public ModItem Item { get; private set; }
    public Asset<Texture2D> GlintTexture { get; private set; }

    public override PrefixCategory Category => PrefixCategory.Custom;

    public override void Load() {
        Item = new PotionPrefixItem(this);

        Mod.AddContent(Item);

        if (Main.netMode != NetmodeID.Server) {
            GlintTexture = ModContent.Request<Texture2D>($"{this.NamespaceFilePath()}/{Name}Glint");
        }
    }

    public override void Unload() {
        GlintTexture = null;
    }

    public override bool CanRoll(Item item) {
        return ItemDataSet.Potions.Contains(item.type);
    }
}