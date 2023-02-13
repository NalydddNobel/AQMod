using Aequus.Items.Accessories;
using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace Aequus.Common.Preferences
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

        [MemberBGColor]
        [Name("Gameplay.General.EyeOfCthulhuOreDecrease")]
        [Desc("Gameplay.General.EyeOfCthulhuOreDecrease")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool EyeOfCthulhuOreDropsDecrease { get; set; }

        [MemberBGColor]
        [Name("Gameplay.General.EarlyMimics")]
        [Desc("Gameplay.General.EarlyMimics")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool EarlyMimics { get; set; }


        [Header(Key + "Gameplay.World.Header")]

        [MemberBGColor]
        [Name("Gameplay.World.CaveVariety")]
        [Desc("Gameplay.World.CaveVariety")]
        [DefaultValue(1f)]
        [Range(0f, 1f)]
        public float CaveVariety { get; set; }

        [MemberBGColor]
        [Name("Gameplay.World.EyeOfCthulhuOres")]
        [Desc("Gameplay.World.EyeOfCthulhuOres")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool EyeOfCthulhuOres { get; set; }

        [MemberBGColor]
        [Name("Gameplay.World.HardmodeChests")]
        [Desc("Gameplay.World.HardmodeChests")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool HardmodeChests { get; set; }


        [Header(Key + "Gameplay.Recipes.Header")]

        [MemberBGColor]
        [Name("Gameplay.Recipes.VoidBag")]
        [Desc("Gameplay.Recipes.VoidBag")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool VoidBagRecipe { get; set; }

        public override void AddCustomTranslations()
        {
            Text("General.EyeOfCthulhuOreDecrease");
            Text("World.EyeOfCthulhuOres");
            Text("World.CaveVariety");
            Text("World.HardmodeChests");
            Text("General.EarlyMimics");
            Text("General.DamageReductionCap", new
            {
                Item = TextHelper.ItemCommand<CrownOfBlood>(),
            });
            Text("General.EarlyGravityGlobe", new
            {
                Item = TextHelper.ItemCommand(ItemID.GravityGlobe),
            });
            Text("General.EarlyPortalGun", new
            {
                Item = TextHelper.ItemCommand(ItemID.PortalGun),
            });
            Text("Recipes.VoidBag", new
            {
                Item1 = TextHelper.ItemCommand(ItemID.VoidVault),
                Item2 = TextHelper.ItemCommand(ItemID.VoidLens),
            });
        }
    }
}