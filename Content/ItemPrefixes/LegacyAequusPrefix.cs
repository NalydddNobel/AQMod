using Aequus.Common.Entities.ItemAffixes;

namespace Aequus.Content.ItemPrefixes;
public abstract class LegacyAequusPrefix : ModPrefix, IShimmerAffix {
    public virtual bool Shimmerable => false;

    public override void SetStaticDefaults() {
        // DisplayName.SetDefault("{$Mods.Aequus.PrefixName." + Name + "}");
    }

    public virtual void UpdateEquip(Item item, Player player) {
    }

    public virtual void UpdateAccessory(Item item, Player player, bool hideVisual) {
    }

    public virtual void OnShoot(Player player, Item item, Projectile projectile) {
    }

    bool IShimmerAffix.OnShimmer(Item item) {
        if (!Shimmerable) {
            return false;
        }

        IShimmerAffix.ClearPrefixOnShimmer(item);
        return true;
    }
}