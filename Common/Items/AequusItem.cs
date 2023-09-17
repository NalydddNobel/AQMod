using Aequus.Core.Utilities;
using System.Reflection;
using Terraria.ModLoader;

namespace Aequus.Common.Items;

public partial class AequusItem : GlobalItem {
    public override void Load() {
        DetourHelper.AddHook(typeof(PrefixLoader).GetMethod(nameof(PrefixLoader.CanRoll)), typeof(AequusItem).GetMethod(nameof(On_PrefixLoader_CanRoll), BindingFlags.NonPublic | BindingFlags.Static));
    }
}