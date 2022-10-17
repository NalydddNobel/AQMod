using Aequus.Common;
using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace Aequus
{
    public class GameplayConfig : ConfigurationBase, IPostSetupContent
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        protected override string ConfigKey => "Gameplay";

        public static GameplayConfig Instance;

        //[Header(Key + "Gameplay.General.Header")]

        [Header(Key + "Gameplay.Recipes.Header")]

        [MemberBGColor]
        [Name("Gameplay.Recipes.VoidBag")]
        [Desc("Gameplay.Recipes.VoidBag")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool VoidBagRecipe { get; set; }

        [MemberBGColor]
        [Name("Gameplay.Recipes.PhoenixBlaster")]
        [Desc("Gameplay.Recipes.PhoenixBlaster")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PhoenixBlasterRecipe { get; set; }

        //[Header(Key + "Gameplay.Drops.Header")]

        public override void AddCustomTranslations()
        {
            Text("Recipes.PhoenixBlaster", new
            {
                PhoenixBlaster = AequusText.ItemCommand(ItemID.PhoenixBlaster),
            });
            Text("Recipes.VoidBag", new
            {
                VoidVault = AequusText.ItemCommand(ItemID.VoidVault),
                VoidBag = AequusText.ItemCommand(ItemID.VoidLens),
            });
        }
    }
}