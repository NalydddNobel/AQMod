using Aequus.Items.Accessories.CrownOfBlood;
using System.ComponentModel;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace Aequus.Common.Preferences {
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
        [Name("Gameplay.General.EarlyWiring")]
        [Desc("Gameplay.General.EarlyWiring")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool EarlyWiring { get; set; }

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

        [MemberBGColor]
        [Name("Gameplay.General.EarlyGreenJellyfish")]
        [Desc("Gameplay.General.EarlyGreenJellyfish")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool EarlyGreenJellyfish { get; set; }

        [MemberBGColor]
        [Name("Gameplay.General.EarlyAnglerFish")]
        [Desc("Gameplay.General.EarlyAnglerFish")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool EarlyAnglerFish { get; set; }


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

        public static Condition ConditionEarlyPortalGun => new(TextHelper.GetOrRegister("Condition.EarlyPortalGun"), () => Instance.EarlyPortalGun);
        public static Condition ConditionEarlyGravityGlobe => new(TextHelper.GetOrRegister("Condition.EarlyGravityGlobe"), () => Instance.EarlyGravityGlobe);

        public override void AddCustomTranslations()
        {
            FixItemIcon("General.EarlyAnglerFish");
            FixItemIcon("General.EarlyGreenJellyfish");
            FixItemIcon("General.EarlyWiring");
            FixItemIcon("General.EyeOfCthulhuOreDecrease");
            FixItemIcon("World.EyeOfCthulhuOres");
            FixItemIcon("World.CaveVariety");
            FixItemIcon("World.HardmodeChests");
            FixItemIcon("General.EarlyMimics");
            FormatText("General.DamageReductionCap", new
            {
                Item = TextHelper.ItemCommand<CrownOfBloodItem>(),
            });
            FormatText("General.EarlyGravityGlobe", new
            {
                Item = TextHelper.ItemCommand(ItemID.GravityGlobe),
            });
            FormatText("General.EarlyPortalGun", new
            {
                Item = TextHelper.ItemCommand(ItemID.PortalGun),
            });
            FormatText("Recipes.VoidBag", new
            {
                Item1 = TextHelper.ItemCommand(ItemID.VoidVault),
                Item2 = TextHelper.ItemCommand(ItemID.VoidLens),
            });
        }
    }
}