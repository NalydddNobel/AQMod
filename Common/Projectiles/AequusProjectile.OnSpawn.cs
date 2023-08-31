using Aequus.Common.Items.Components;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus;

public partial class AequusProjectile {
    public short parentNPCIndex;
    public int parentItemType;

    public override void OnSpawn(Projectile projectile, IEntitySource source) {
        if (source is EntitySource_Parent source_Parent) {
            if (source_Parent.Entity is Projectile projectile_Source && projectile_Source.TryGetGlobalProjectile<AequusProjectile>(out var parent_GlobalProjectile_Source)) {
                parentItemType = parent_GlobalProjectile_Source.parentItemType;
                parentNPCIndex = parent_GlobalProjectile_Source.parentNPCIndex;
            }
            else if (source_Parent.Entity is Item item_Source) {
                parentItemType = item_Source.type;
            }
            else if (source_Parent.Entity is NPC npc_Source) {
                parentNPCIndex = (short)npc_Source.whoAmI;
            }
        }
        if (source is EntitySource_ItemUse source_ItemUse) {
            if (source_ItemUse.Item != null) {
                parentItemType = source_ItemUse.Item.type;
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