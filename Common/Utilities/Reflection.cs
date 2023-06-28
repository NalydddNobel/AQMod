using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Utilities {
    public partial class Reflection : ILoadable {
        public static MethodInfo Item_Variant_Set { get; private set; }

        public void Load(Mod mod) {
            var variantProperty = typeof(Item).GetProperty("Variant", BindingFlags.Public | BindingFlags.Instance);
            Item_Variant_Set = variantProperty.GetSetMethod(nonPublic: true);
        }

        public void Unload() {
            Item_Variant_Set = null;
        }
    }
}