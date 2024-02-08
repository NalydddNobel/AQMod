using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Aequus.Common.Tiles;

public class RandomTick : GlobalTile {
    /// <summary>Lookup for update tick actions to perform less type checks.</summary>
    public static readonly Dictionary<int, OnUpdateDelegate> UpdateByType = new();

    public static void AddUpdateByType(OnUpdateDelegate onUpdate, params int[] types) {
        foreach (int t in types) {
            AddUpdateByType(onUpdate, t);
        }
    }
    public static void AddUpdateByType(OnUpdateDelegate onUpdate, int type) {
        CollectionsMarshal.GetValueRefOrAddDefault(UpdateByType, type, out _) += onUpdate;
    }

    public override void RandomUpdate(int i, int j, int type) {
        if (UpdateByType.TryGetValue(type, out OnUpdateDelegate action)) {
            action.Invoke(i, j, type);
        }
    }

    public delegate void OnUpdateDelegate(int i, int j, int type);

    public override void Unload() {
        UpdateByType.Clear();
    }
}
