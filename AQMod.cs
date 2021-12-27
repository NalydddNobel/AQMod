using Terraria.ModLoader;

namespace AQMod
{
    public class AQMod : Mod
    {
        public static bool IsLoading { get; private set; }

        public override void Load()
        {
            IsLoading = true;
        }

        public override void AddRecipeGroups()
        {
            IsLoading = false;
        }

        public override void Unload()
        {
            IsLoading = true;
        }
    }
}