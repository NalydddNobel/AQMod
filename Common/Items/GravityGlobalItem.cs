using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.Items;

public sealed class GravityGlobalItem : GlobalItem {
    public override bool InstancePerEntity => true;

    protected override bool CloneNewInstances => true;

    /// <summary>
    /// How long the item should float for. Set to 255 for permanent duration.
    /// </summary>
    public byte itemGravityCheck = 0;
    public float itemGravityMultiplier = 1f;

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return !ItemID.Sets.ItemNoGravity[entity.type];
    }

    public void SetNoGrav(Item item, byte duration) {
        var aequus = item.GetGlobalItem<GravityGlobalItem>();
        if (aequus.itemGravityCheck > Math.Max(duration - 15, 15)) {
            return;
        }
        CheckNoGravity(item);
        aequus.itemGravityCheck = duration;
    }

    private void CheckNoGravity(Item item) {
        itemGravityMultiplier = TileHelper.IsShimmerBelow(item.Center.ToTileCoordinates(), 12) ? 1f : 0f;
    }

    private void CheckNoGravity(Entity parent) {
    }

    public override void SetDefaults(Item entity) {
        itemGravityMultiplier = 1f;
    }

    public override void OnSpawn(Item item, IEntitySource source) {
        if (source is EntitySource_Parent entitySource_Parent) {
            //if (parent is not NPC npc || !npc.GetGlobalNPC<AequusNPC>().noGravityDrops || Helper.FindFloor(parent.Center, 20) != -1) {
            //    return;
            //}

            //itemGravityCheck = 255;
        }
    }

    public override void Update(Item item, ref float gravity, ref float maxFallSpeed) {
        if (gravity <= 0f || itemGravityCheck == 0) {
            return;
        }

        if (itemGravityCheck < 255) {
            itemGravityCheck--;
        }
        if (itemGravityCheck == 0 || (itemGravityCheck == 255 && item.timeSinceItemSpawned % 40 == 0)) {
            CheckNoGravity(item);
        }
        item.velocity.Y *= 0.95f + 0.05f * itemGravityMultiplier;
        gravity *= itemGravityMultiplier;
    }

    #region IO
    public override void NetSend(Item item, BinaryWriter writer) {
        writer.Write(itemGravityCheck > 0);
        if (itemGravityCheck > 0) {
            writer.Write(itemGravityCheck);
            writer.Write(itemGravityMultiplier);
        }
    }

    public override void NetReceive(Item item, BinaryReader reader) {
        if (reader.ReadBoolean()) {
            itemGravityCheck = reader.ReadByte();
            itemGravityMultiplier = reader.ReadSingle();
        }
    }
    #endregion
}