using System.Collections.Generic;

namespace Aequus.Common.Utilities.Sampling;

public class NewItemCache : LoadedType {
    public readonly List<Item> Items = [];
    public bool QueueItems { get; private set; }

    public static NewItemCache Instance => ModContent.GetInstance<NewItemCache>();

    public void Begin() {
        Items.Clear();
        QueueItems = true;
    }

    public void End() {
        QueueItems = false;
    }

    protected override void Load() {
        On_Item.NewItem_Inner += On_Item_NewItem_Inner;
    }

    private static int On_Item_NewItem_Inner(On_Item.orig_NewItem_Inner orig, Terraria.DataStructures.IEntitySource source, int X, int Y, int Width, int Height, Item itemToClone, int Type, int Stack, bool noBroadcast, int pfix, bool noGrabDelay, bool reverseLookup) {
        if (Instance.QueueItems) {
            Item resultItem;
            if (itemToClone != null) {
                resultItem = itemToClone.Clone();
            }
            else {
                resultItem = new(Type);
                resultItem.Prefix(pfix);
                resultItem.stack = Stack;
            }

            Instance.Items.Add(resultItem);
            return WorldGen.gen || reverseLookup ? 0 : Main.maxItems;
        }

        return orig(source, X, Y, Width, Height, itemToClone, Type, Stack, noBroadcast, pfix, noGrabDelay, reverseLookup);
    }
}