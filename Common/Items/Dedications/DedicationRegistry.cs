using Aequus.Content.Dedicated;
using System;
using System.Collections.Generic;

namespace Aequus.Common.Items.Dedications;

public class DedicationRegistry : ModSystem {
    private static readonly Dictionary<ModItem, IDedicationInfo> _fromItem = new();
    private static bool _doneAcceptingRegistries;

    internal static void Register(ModItem modItem, IDedicationInfo info) {
        if (_fromItem.ContainsValue(info)) {
            throw new Exception($"Dedication has already been registered for an item.");
        }

        RegisterInner(modItem, info);
    }

    internal static void RegisterSubItem(ModItem parentItem, ModItem childItem) {
        if (!_fromItem.TryGetValue(parentItem, out IDedicationInfo info)) {
            throw new Exception($"{parentItem.FullName} cannot register {childItem.FullName} since it does not have dedicated item info.");
        }

        RegisterInner(childItem, info);
    }

    private static void RegisterInner(ModItem modItem, IDedicationInfo info) {
        if (_doneAcceptingRegistries) {
            throw new Exception($"{modItem.FullName} attempted to register dedicated item info after Aequus has been loaded.");
        }

        if (info.Faeling == null) {
            // Register a colored faeling aswell.
            info.Faeling = new DedicatedFaeling.FaelingItem(modItem, info);
            ModContent.GetInstance<Aequus>().AddContent(info.Faeling);
        }

        _fromItem[modItem] = info;
    }

    public static IDedicationInfo Get(ModItem item) {
        return _fromItem[item];
    }

    public static bool TryGet(ModItem item, out IDedicationInfo info) {
        if (item == null) {
            info = default;
            return false;
        }

        return _fromItem.TryGetValue(item, out info);
    }

    public static IDedicationInfo Get(int id) {
        return Get(ItemLoader.GetItem(id));
    }

    public static bool TryGet(int id, out IDedicationInfo info) {
        return TryGet(ItemLoader.GetItem(id), out info);
    }

    public override void OnModLoad() {
        _doneAcceptingRegistries = true;
    }

    public override void Unload() {
        _doneAcceptingRegistries = false;
        _fromItem.Clear();
    }
}
