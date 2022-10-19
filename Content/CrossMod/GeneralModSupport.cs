using Aequus.Common;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    public class GeneralModSupport : IPostSetupContent, IAddRecipes
    {
        public static Mod Census => CensusSupport.Census;
        public static Mod CalamityMod => CalamityModSupport.CalamityMod;
        public static Mod Polarities => PolaritiesSupport.Polarities;
        public static Mod BossChecklist => BossChecklistSupport.BossChecklist;
        public static Mod Fargowiltas { get; private set; }
        public static Mod ColoredDamageTypes { get; private set; }

        void ILoadable.Load(Mod mod)
        {
        }

        void IPostSetupContent.PostSetupContent(Aequus aequus)
        {
            Fargowiltas = FindMod(nameof(Fargowiltas));

            ColoredDamageTypes = FindMod(nameof(ColoredDamageTypes));

            if (ColoredDamageTypes != null)
            {
                ColoredDamageTypesSupport();
            }
        }
        private static Mod FindMod(string name)
        {
            return ModLoader.TryGetMod(name, out var value) ? value : null;
        }
        private static void ColoredDamageTypesSupport()
        {
            ColoredDamageTypes.Call("AddDamageType", NecromancyDamageClass.Instance,
                NecromancyDamageClass.NecromancyDamageColor, NecromancyDamageClass.NecromancyDamageColor, NecromancyDamageClass.NecromancyDamageColor * 1.25f);
        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
        }

        void ILoadable.Unload()
        {
            Fargowiltas = null;
            ColoredDamageTypes = null;
        }
    }
}