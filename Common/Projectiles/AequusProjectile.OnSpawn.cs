using Aequus.Common.Items.Components;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.Projectiles;

public partial class AequusProjectile {
    public short parentNPCIndex;
    public int parentItemType;
    public int parentAmmoType;
    /// <summary>
    /// Whether this projectile was spawned by another projectile. Use this to prevent effects occuring multiple times (Like ammo retrival)
    /// </summary>
    public bool isProjectileChild;

    public override void OnSpawn(Projectile projectile, IEntitySource source) {
        if (source is EntitySource_Parent parentSource) {
            if (parentSource.Entity is Projectile parentProjectile && parentProjectile.TryGetGlobalProjectile<AequusProjectile>(out var parentAequusProjectile)) {
                parentItemType = parentAequusProjectile.parentItemType;
                parentNPCIndex = parentAequusProjectile.parentNPCIndex;
                parentAmmoType = parentAequusProjectile.parentAmmoType;
                isProjectileChild = true;
            }
            else if (parentSource.Entity is Item parentItem) {
                parentItemType = parentItem.type;
            }
            else if (parentSource.Entity is NPC parentNPC) {
                parentNPCIndex = (short)parentNPC.whoAmI;
            }
        }
        if (source is EntitySource_ItemUse_WithAmmo withAmmo) {
            parentAmmoType = withAmmo.AmmoItemIdUsed;
        }
        if (source is IEntitySource_WithStatsFromItem withItem) {
            if (withItem.Item != null) {
                parentItemType = withItem.Item.type;
            }
        }
    }

    private void PreAI_UpdateSources(Projectile projectile) {
        if (parentNPCIndex > -1 && !Main.npc[parentNPCIndex].active) {
            parentNPCIndex = -1;
        }
        if (parentItemType > ItemID.Count && ItemLoader.GetItem(parentItemType) is IManageProjectile manageProjectile) {
            _projectileManager = manageProjectile;
        }
    }
}