using System.Collections.Generic;

namespace Aequus.Core;

public sealed class NewItemCache : ILoadable {
    public static readonly List<Item> DroppedItems = new();
    public static System.Boolean QueueItems { get; private set; }

    public void Load(Mod mod) {
        On_Item.NewItem_Inner += On_Item_NewItem_Inner;
        QueueItems = false;
    }

    private static System.Int32 On_Item_NewItem_Inner(On_Item.orig_NewItem_Inner orig, Terraria.DataStructures.IEntitySource source, System.Int32 X, System.Int32 Y, System.Int32 Width, System.Int32 Height, Item itemToClone, System.Int32 Type, System.Int32 Stack, System.Boolean noBroadcast, System.Int32 pfix, System.Boolean noGrabDelay, System.Boolean reverseLookup) {
        if (QueueItems) {
            Item resultItem;
            if (itemToClone != null) {
                resultItem = itemToClone.Clone();
            }
            else {
                resultItem = new(Type);
                resultItem.Prefix(pfix);
                resultItem.stack = Stack;
            }
            DroppedItems.Add(resultItem);
            return (WorldGen.gen || reverseLookup) ? 0 : Main.maxItems;
        }

        return orig(source, X, Y, Width, Height, itemToClone, Type, Stack, noBroadcast, pfix, noGrabDelay, reverseLookup);
    }

    public void Unload() {
    }

    public static void Begin() {
        DroppedItems.Clear();
        QueueItems = true;
    }

    public static void End() {
        QueueItems = false;
    }
}