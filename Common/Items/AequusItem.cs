using Aequus.Content.DataSets;
using Aequus.Core.IO;
using Aequus.Core.Utilities;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
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

    public override bool ItemSpace(Item item, Player player) {
        if (!player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer) && aequusPlayer.ExtraInventoryCount > 0) {
            return false;
        }

        for (int i = 0; i < aequusPlayer.ExtraInventoryCount; i++) {
            if (player.CanItemSlotAccept(aequusPlayer.extraInventory[i], item)) {
                return true;
            }
        }
        return false;
    }

    public override bool OnPickup(Item item, Player player) {
        if (!player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer) && aequusPlayer.ExtraInventoryCount > 0) {
            return true;
        }

        int extraInventorySlots = aequusPlayer.extraInventorySlots;
        aequusPlayer.extraInventorySlots = 0;
        var itemSpace = player.ItemSpace(item);
        aequusPlayer.extraInventorySlots = extraInventorySlots;

        int transferredToBackpack = 0;
        for (int i = 0; i < aequusPlayer.ExtraInventoryCount; i++) {
            if (!player.CanItemSlotAccept(aequusPlayer.extraInventory[i], item)) {
                continue;
            }

            if (aequusPlayer.extraInventory[i].IsAir && (!itemSpace.CanTakeItem || itemSpace.ItemIsGoingToVoidVault)) {
                aequusPlayer.extraInventory[i] = item.Clone();
                transferredToBackpack = item.stack;
                item.stack = 0;
                break;
            }

            ItemLoader.StackItems(aequusPlayer.extraInventory[i], item, out int transferred);
            transferredToBackpack += transferred;
            if (item.stack <= 0) {
                break;
            }
        }
        if (transferredToBackpack > 0) {
            PopupText.NewText(PopupTextContext.RegularItemPickup, item, transferredToBackpack);
            SoundEngine.PlaySound(SoundID.Grab);
            if (item.stack <= 0) {
                item.TurnToAir();
                return false;
            }
        }
        return true;
    }

    public override void SaveData(Item item, TagCompound tag) {
        SaveDataAttribute.SaveData(tag, this);
    }

    public override void LoadData(Item item, TagCompound tag) {
        SaveDataAttribute.LoadData(tag, this);
    }
}