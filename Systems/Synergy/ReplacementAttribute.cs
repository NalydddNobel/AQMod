using AequusRemake.Core.Util.Helpers;
using System;
using tModLoaderExtended.Terraria.ModLoader;

namespace AequusRemake.Systems.Synergy;

/// <summary>Notifies a piece of content is a replacement for old Aequus content.</summary>
/// <param name="Name"></param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
internal class ReplacementAttribute(string Name = null) : Attribute, IAttributeOnModLoad {
    private readonly string _name = Name;

    void IAttributeOnModLoad.OnModLoad(Mod mod, ILoad Content) {
        if (AequusRemake.Aequus == null) {
            return;
        }

        string name = _name ?? ((dynamic)Content).Name;

        if (Content is ModItem ModItem) {
            if (AequusRemake.Aequus.TryFind(name, out ModItem oldModItem)) {
                ReplaceItems.Replace.Add(oldModItem.Type, ModItem.Type);

                ILTool.DeleteMethod(oldModItem.GetType(), "AddRecipes");
                AequusRemake.OnPostSetupContent += () => ItemID.Sets.Deprecated[oldModItem.Type] = true;
            }
            else {
                Log.Info($"Item Type of \"{name}\" was not found.");
            }
        }
    }
}
