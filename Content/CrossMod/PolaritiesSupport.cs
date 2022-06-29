using Aequus.Common;
using Aequus.Tiles.Furniture;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    internal class PolaritiesSupport : IPostSetupContent, IAddRecipes
    {
        public static ModData Polarities;

        void ILoadable.Load(Mod mod)
        {
        }

        void IPostSetupContent.PostSetupContent(Aequus aequus)
        {
            Polarities = new ModData("Polarities");
        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
            if (Polarities.Enabled)
            {
                MonsterBanners.BannerWindHack.Add(TileID.Search.GetId("Polarities/BannerTile"));
            }
        }

        void ILoadable.Unload()
        {
            Polarities.Clear();
        }
    }
}