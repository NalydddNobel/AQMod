using Aequus.Content.DataSets;
using Aequus.Core.IO;
using Aequus.Core.Utilities;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

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
        CheckNameTag(item);
    }

    public override bool CanStack(Item destination, Item source) {
        return NametagStackCheck(destination, source);
    }

    public override bool CanStackInWorld(Item destination, Item source) {
        return NametagStackCheck(destination, source);
    }

    public override void SaveData(Item item, TagCompound tag) {
        SaveDataAttribute.SaveData(tag, this);
    }

    public override void LoadData(Item item, TagCompound tag) {
        SaveDataAttribute.LoadData(tag, this);
    }
}