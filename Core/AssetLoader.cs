﻿using Aequus.Common;
using System.Reflection;
using Terraria.ModLoader;

namespace Aequus.Core;

public class AssetLoader<AssetType, AssetValue> : ILoadable where AssetType : TemplateAsset<AssetValue> where AssetValue : class {
    public void Load(Mod mod) {
        OnLoad(mod);
    }

    protected virtual void OnLoad(Mod mod) {
    }

    public void Unload() {
        OnUnload();
        foreach (var f in GetType().GetFields(BindingFlags.Public | BindingFlags.Static)) {
            if (f.FieldType == typeof(AssetType)) {
                ((AssetType)f.GetValue(this))?.Unload();
            }
        }
    }

    protected virtual void OnUnload() {
    }
}