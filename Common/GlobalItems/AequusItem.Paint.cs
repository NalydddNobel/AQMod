using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items {
    public partial class AequusItem : GlobalItem, IPostSetupContent, IAddRecipes
    {
        public delegate bool CustomCoatingFunction(int x, int y, Player player);

        public static Dictionary<int, CustomCoatingFunction> ApplyCustomCoating { get; private set; }
        public static List<CustomCoatingFunction> RemoveCustomCoating { get; private set; }

        public void Load_Paint()
        {
            RemoveCustomCoating = new List<CustomCoatingFunction>();
            ApplyCustomCoating = new Dictionary<int, CustomCoatingFunction>();
        }

        public void Unload_Paint()
        {
            RemoveCustomCoating?.Clear();
            RemoveCustomCoating = null;
            ApplyCustomCoating?.Clear();
            ApplyCustomCoating = null;
        }
    }
}