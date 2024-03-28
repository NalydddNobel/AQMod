using System.Collections.Generic;

namespace Aequus.Core;

public sealed class NewItemCache : ILoad {
    public static readonly List<Item> DroppedItems = new();
    public static bool QueueItems { get; private set; }

    public void Load(Mod mod) {
        On_Item.NewItem_Inner += On_Item_NewItem_Inner;
        QueueItems = false;
    }

    private static int On_Item_NewItem_Inner(On_Item.orig_NewItem_Inner orig, Terraria.DataStructures.IEntitySource source, int X, int Y, int Width, int Height, Item itemToClone, int Type, int Stack, bool noBroadcast, int pfix, bool noGrabDelay, bool reverseLookup) {
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