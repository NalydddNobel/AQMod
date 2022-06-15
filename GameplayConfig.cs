using Aequus.Common;
using Aequus.Items.Accessories.Summon.Necro;
using Aequus.Items.Weapons.Melee;
using Aequus.Items.Weapons.Ranged;
using Aequus.Items.Weapons.Summon.Candles;
using Aequus.Items.Weapons.Summon.Necro;
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

        [Header(Key + "Gameplay.Headers.Drops")]

        [BackgroundColor(47, 29, 140, 180)]
        [Label(Key + "Gameplay.Drops.DungeonMisc_AngryBonesLabel")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool DungeonMisc_AngryBones { get; set; }
        [BackgroundColor(47, 29, 140, 180)]
        [Label(Key + "Gameplay.Drops.DungeonNecromancy_DarkCasterLabel")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool DungeonNecromancy_DarkCaster { get; set; }
        [BackgroundColor(47, 29, 140, 180)]
        [Label(Key + "Gameplay.Drops.DungeonNecromancy_NecromancerLabel")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool DungeonNecromancy_Necromancer { get; set; }
        [BackgroundColor(47, 29, 140, 180)]
        [Label(Key + "Gameplay.Drops.LavaCharm_LavaSlimeLabel")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool LavaCharm_LavaSlime { get; set; }
        [BackgroundColor(47, 29, 140, 180)]
        [Label(Key + "Gameplay.Drops.Skyware_HarpiesLabel")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool Skyware_Harpies { get; set; }
        [BackgroundColor(47, 29, 140, 180)]
        [Label(Key + "Gameplay.Drops.FrozenMisc_VikingsLabel")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool FrozenMisc_Vikings { get; set; }

        void IPostSetupContent.PostSetupContent(Aequus aequus)
        {
            AequusText.NewFromDict("Configuration.Gameplay.Drops.FrozenMisc_Vikings", "Label",
                new
                {
                    CrystalDagger = AequusText.ItemText<CrystalDagger>(),
                });
            AequusText.NewFromDict("Configuration.Gameplay.Drops.Skyware_Harpies", "Label",
                new
                {
                    Slingshot = AequusText.ItemText<Slingshot>(),
                });
            AequusText.NewFromDict("Configuration.Gameplay.Drops.LavaCharm_LavaSlime", "Label",
                new
                {
                    LavaCharm = AequusText.ItemText(ItemID.LavaCharm),
                });
            AequusText.NewFromDict("Configuration.Gameplay.Drops.DungeonNecromancy_Necromancer", "Label",
                new
                {
                    Revenant = AequusText.ItemText<Revenant>(),
                    WretchedCandle = AequusText.ItemText<WretchedCandle>(),
                    PandorasBox = AequusText.ItemText<PandorasBox>(),
                });
            AequusText.NewFromDict("Configuration.Gameplay.Drops.DungeonNecromancy_DarkCaster", "Label",
                new
                {
                    Revenant = AequusText.ItemText<Revenant>(),
                    WretchedCandle = AequusText.ItemText<WretchedCandle>(),
                    PandorasBox = AequusText.ItemText<PandorasBox>(),
                });
            AequusText.NewFromDict("Configuration.Gameplay.Drops.DungeonMisc_AngryBones", "Label",
                new
                {
                    Valari = AequusText.ItemText<Valari>(),
                });
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