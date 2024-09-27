namespace Aequus.Common.ContentTemplates.Armor;

public abstract class UnifiedVanitySet : UnifiedArmorSet {
    public override string LocalizationCategory => "Items.Vanity";

    public override string Texture => $"{this.NamespacePath()}/{Name.Replace("Vanity", "")}";
}
