using Terraria.ModLoader;

namespace Aequus {
    internal class AutoloadSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            foreach (var t in Aequus.Instance.GetContent<IPostSetupContent>())
            {
                t.PostSetupContent(Aequus.Instance);
            }
        }

        public override void AddRecipeGroups()
        {
            foreach (var t in Aequus.Instance.GetContent<IAddRecipeGroups>())
            {
                t.AddRecipeGroups(Aequus.Instance);
            }
        }

        public override void AddRecipes()
        {
            foreach (var t in Aequus.Instance.GetContent<IAddRecipes>())
            {
                t.AddRecipes(Aequus.Instance);
            }
        }

        public override void PostAddRecipes()
        {
            foreach (var t in Aequus.Instance.GetContent<IPostAddRecipes>())
            {
                t.PostAddRecipes(Aequus.Instance);
            }
        }
    }

    internal interface IOnModLoad : ILoadable
    {
        void OnModLoad(Aequus aequus);
    }

    internal interface IPostSetupContent : ILoadable
    {
        void PostSetupContent(Aequus aequus);
    }

    internal interface IAddRecipeGroups : ILoadable
    {
        void AddRecipeGroups(Aequus aequus);
    }

    internal interface IAddRecipes : ILoadable
    {
        void AddRecipes(Aequus aequus);
    }

    internal interface IPostAddRecipes : ILoadable
    {
        void PostAddRecipes(Aequus aequus);
    }
}