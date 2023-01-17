using Aequus.Common;
using Aequus.Items.Accessories;
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

        [Header(Key + "Gameplay.General.Header")]

        [MemberBGColor]
        [Name("Gameplay.General.DamageReductionCap")]
        [Desc("Gameplay.General.DamageReductionCap")]
        [DefaultValue(0.6f)]
        [Range(0.6f, 1f)]
        [ReloadRequired]
        public float DamageReductionCap { get; set; }

        [MemberBGColor]
        [Name("Gameplay.General.EarlyGravityGlobe")]
        [Desc("Gameplay.General.EarlyGravityGlobe")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool EarlyGravityGlobe { get; set; }

        [MemberBGColor]
        [Name("Gameplay.General.EarlyPortalGun")]
        [Desc("Gameplay.General.EarlyPortalGun")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool EarlyPortalGun { get; set; }

        [Header(Key + "Gameplay.Recipes.Header")]

        [MemberBGColor]
        [Name("Gameplay.Recipes.VoidBag")]
        [Desc("Gameplay.Recipes.VoidBag")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool VoidBagRecipe { get; set; }

        public override void AddCustomTranslations()
        {
            Text("General.DamageReductionCap", new
            {
                Item = AequusText.ItemCommand<CrownOfBlood>(),
            });
            Text("General.EarlyGravityGlobe", new
            {
                Item = AequusText.ItemCommand(ItemID.GravityGlobe),
            });
            Text("General.EarlyPortalGun", new
            {
                Item = AequusText.ItemCommand(ItemID.PortalGun),
            });
            Text("Recipes.VoidBag", new
            {
                Item1 = AequusText.ItemCommand(ItemID.VoidVault),
                Item2 = AequusText.ItemCommand(ItemID.VoidLens),
            });
        }
    }
}