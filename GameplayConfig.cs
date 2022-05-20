using Aequus.Common;
using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace Aequus
{
    public class GameplayConfig : ConfigurationBase, IPostSetupContent
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public static GameplayConfig Instance;

        [Header(Key + "Gameplay.Headers.Recipes")]

        [BackgroundColor(47, 29, 140, 180)]
        [Label(Key + "Gameplay.VoidBagRecipeLabel")]
        [Tooltip(Key + "Gameplay.VoidBagRecipeTooltip")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool VoidBagRecipe { get; set; }

        void IPostSetupContent.PostSetupContent(Aequus aequus)
        {
            AequusText.NewFromDict("Configuration.Gameplay.VoidBagRecipe", "Label", 
                (s) => AequusText.ItemText(ItemID.VoidLens) + " " + AequusText.ItemText(ItemID.VoidVault) +  "  " + s);
        }

        void ILoadable.Load(Mod mod)
        {
        }

        void ILoadable.Unload()
        {
        }
    }
}