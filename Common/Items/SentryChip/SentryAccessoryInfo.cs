using Aequus.Common.Projectiles.SentryChip;
using Terraria;

namespace Aequus.Common.Items.SentryChip {
    public record struct SentryAccessoryInfo(Projectile Projectile, SentryAccessoriesGlobalProj SentryAccessoriesGlobalProj, Item Accessory) {
        public Player DummyPlayer => SentryAccessoriesGlobalProj.dummyPlayer;
        public int Owner => Projectile.owner;
    }
}