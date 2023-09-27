using Aequus.Common.DataSets;
using Aequus.Core.Utilities;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus;

public partial class AequusItem : GlobalItem {
    public override bool InstancePerEntity => true;
    protected override bool CloneNewInstances => true;

    public override void Load() {
        DetourHelper.AddHook(typeof(PrefixLoader).GetMethod(nameof(PrefixLoader.CanRoll)), typeof(AequusItem).GetMethod(nameof(On_PrefixLoader_CanRoll), BindingFlags.NonPublic | BindingFlags.Static));
    }

    public override void OnSpawn(Item item, IEntitySource source) {
        itemGravityMultiplier = 1f;
        //reversedGravity = false;
        //if (source is EntitySource_Loot) {
        //    naturallyDropped = true;
        //}

        if (item.IsACoin || ItemSets.IsPickup.Contains(item.type)) {
            return;
        }

        if (source is EntitySource_Parent entitySource_Parent) {
            CheckNoGravity(entitySource_Parent.Entity);
        }
    }

    public override void Update(Item item, ref float gravity, ref float maxFallSpeed) {
        UpdateZeroGravity(item, ref gravity);
    }
}