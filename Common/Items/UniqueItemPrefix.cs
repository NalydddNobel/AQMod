using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Items;

public abstract class UniqueItemPrefix : ModPrefix {
    public virtual bool CanBeShimmeredAway => false;

    public override void SetStaticDefaults() {
        // DisplayName.SetDefault("{$Mods.Aequus.PrefixName." + Name + "}");
    }

    public virtual void UpdateEquip(Item item, Player player) {
    }

    public virtual void UpdateAccessory(Item item, Player player, bool hideVisual) {
    }

    public virtual void OnShoot(Player player, Item item, Projectile projectile) {
    }
}