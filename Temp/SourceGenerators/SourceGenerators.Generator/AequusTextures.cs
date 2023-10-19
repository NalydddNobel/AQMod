using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Aequus.Common;

namespace Aequus {
    /// <summary>(Amt Textures: 188)</summary>
    [CompilerGenerated]
    public partial class AequusTextures : ILoadable {
        public void Load(Mod mod) {
        }

        public void Unload() {
            foreach (var f in GetType().GetFields()) {
                ((TextureAsset)f.GetValue(this))?.Unload();
            }
        }

        /// <summary>Full Path: Aequus/Assets/Textures/Achievements</summary>
        public static readonly TextureAsset Achievements = new("Aequus/Assets/Textures/Achievements");
        /// <summary>Full Path: Aequus/Content/Items/Misc/Dyes/Breakdown/AncientBreakdownDye</summary>
        public static readonly TextureAsset AncientBreakdownDye = new("Aequus/Content/Items/Misc/Dyes/Breakdown/AncientBreakdownDye");
        /// <summary>Full Path: Aequus/Content/Items/Misc/Dyes/Frostbite/AncientFrostbiteDye</summary>
        public static readonly TextureAsset AncientFrostbiteDye = new("Aequus/Content/Items/Misc/Dyes/Frostbite/AncientFrostbiteDye");
        /// <summary>Full Path: Aequus/Content/Items/Misc/Dyes/HellBeam/AncientHellBeamDye</summary>
        public static readonly TextureAsset AncientHellBeamDye = new("Aequus/Content/Items/Misc/Dyes/HellBeam/AncientHellBeamDye");
        /// <summary>Full Path: Aequus/Content/Items/Misc/Dyes/Scorching/AncientScorchingDye</summary>
        public static readonly TextureAsset AncientScorchingDye = new("Aequus/Content/Items/Misc/Dyes/Scorching/AncientScorchingDye");
        /// <summary>Full Path: Aequus/Content/Items/Misc/Dyes/Tidal/AncientTidalDye</summary>
        public static readonly TextureAsset AncientTidalDye = new("Aequus/Content/Items/Misc/Dyes/Tidal/AncientTidalDye");
        /// <summary>Full Path: Aequus/Content/Items/Material/Energy/Aquatic/AquaticEnergy</summary>
        public static readonly TextureAsset AquaticEnergy = new("Aequus/Content/Items/Material/Energy/Aquatic/AquaticEnergy");
        /// <summary>Full Path: Aequus/Content/Items/Material/Energy/Aquatic/AquaticEnergyParticle</summary>
        public static readonly TextureAsset AquaticEnergyParticle = new("Aequus/Content/Items/Material/Energy/Aquatic/AquaticEnergyParticle");
        /// <summary>Full Path: Aequus/Content/Items/Material/Energy/Atmospheric/AtmosphericEnergy</summary>
        public static readonly TextureAsset AtmosphericEnergy = new("Aequus/Content/Items/Material/Energy/Atmospheric/AtmosphericEnergy");
        /// <summary>Full Path: Aequus/Content/Items/Material/Energy/Atmospheric/AtmosphericEnergyParticle</summary>
        public static readonly TextureAsset AtmosphericEnergyParticle = new("Aequus/Content/Items/Material/Energy/Atmospheric/AtmosphericEnergyParticle");
        /// <summary>Full Path: Aequus/Content/TownNPCs/SkyMerchant/Shimmer/Balloon</summary>
        public static readonly TextureAsset Balloon_Shimmer = new("Aequus/Content/TownNPCs/SkyMerchant/Shimmer/Balloon");
        /// <summary>Full Path: Aequus/Content/TownNPCs/SkyMerchant/Balloon</summary>
        public static readonly TextureAsset Balloon_SkyMerchant = new("Aequus/Content/TownNPCs/SkyMerchant/Balloon");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Mounts/HotAirBalloon/BalloonKit</summary>
        public static readonly TextureAsset BalloonKit = new("Aequus/Content/Items/Equipment/Mounts/HotAirBalloon/BalloonKit");
        /// <summary>Full Path: Aequus/Assets/Textures/BaseParticleTexture</summary>
        public static readonly TextureAsset BaseParticleTexture = new("Aequus/Assets/Textures/BaseParticleTexture");
        /// <summary>Full Path: Aequus/Content/TownNPCs/SkyMerchant/Shimmer/Basket</summary>
        public static readonly TextureAsset Basket_Shimmer = new("Aequus/Content/TownNPCs/SkyMerchant/Shimmer/Basket");
        /// <summary>Full Path: Aequus/Content/TownNPCs/SkyMerchant/Basket</summary>
        public static readonly TextureAsset Basket_SkyMerchant = new("Aequus/Content/TownNPCs/SkyMerchant/Basket");
        /// <summary>Full Path: Aequus/Content/Items/Tools/Bellows/Bellows</summary>
        public static readonly TextureAsset Bellows = new("Aequus/Content/Items/Tools/Bellows/Bellows");
        /// <summary>Full Path: Aequus/Content/Items/Tools/Bellows/BellowsProj</summary>
        public static readonly TextureAsset BellowsProj = new("Aequus/Content/Items/Tools/Bellows/BellowsProj");
        /// <summary>Full Path: Aequus/Assets/Textures/Bloom</summary>
        public static readonly TextureAsset Bloom = new("Aequus/Assets/Textures/Bloom");
        /// <summary>Full Path: Aequus/Assets/Textures/BloomStrong</summary>
        public static readonly TextureAsset BloomStrong = new("Aequus/Assets/Textures/BloomStrong");
        /// <summary>Full Path: Aequus/Content/Tiles/Furniture/Boss/BossRelicsTile</summary>
        public static readonly TextureAsset BossRelicsTile = new("Aequus/Content/Tiles/Furniture/Boss/BossRelicsTile");
        /// <summary>Full Path: Aequus/Content/Tiles/Furniture/Boss/BossTrophiesTile</summary>
        public static readonly TextureAsset BossTrophiesTile = new("Aequus/Content/Tiles/Furniture/Boss/BossTrophiesTile");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Armor/Vanity/BreadOfCthulhuMask</summary>
        public static readonly TextureAsset BreadOfCthulhuMask = new("Aequus/Content/Items/Equipment/Armor/Vanity/BreadOfCthulhuMask");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Armor/Vanity/BreadOfCthulhuMask_Head</summary>
        public static readonly TextureAsset BreadOfCthulhuMask_Head = new("Aequus/Content/Items/Equipment/Armor/Vanity/BreadOfCthulhuMask_Head");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Accessories/Informational/Calendar/Calendar</summary>
        public static readonly TextureAsset Calendar = new("Aequus/Content/Items/Equipment/Accessories/Informational/Calendar/Calendar");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Accessories/Informational/Calendar/CalendarInfoDisplay</summary>
        public static readonly TextureAsset CalendarInfoDisplay = new("Aequus/Content/Items/Equipment/Accessories/Informational/Calendar/CalendarInfoDisplay");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Accessories/Informational/Calendar/CalendarTile</summary>
        public static readonly TextureAsset CalendarTile = new("Aequus/Content/Items/Equipment/Accessories/Informational/Calendar/CalendarTile");
        /// <summary>Full Path: Aequus/Content/Items/Misc/Dyes/Censor/CensorDye</summary>
        public static readonly TextureAsset CensorDye = new("Aequus/Content/Items/Misc/Dyes/Censor/CensorDye");
        /// <summary>Full Path: Aequus/Content/Items/Weapons/Ranged/Bows/SkyHunterCrossbow/Chain</summary>
        public static readonly TextureAsset Chain = new("Aequus/Content/Items/Weapons/Ranged/Bows/SkyHunterCrossbow/Chain");
        /// <summary>Full Path: Aequus/Content/Tiles/Conductive/Items/ConductiveBlock</summary>
        public static readonly TextureAsset ConductiveBlock = new("Aequus/Content/Tiles/Conductive/Items/ConductiveBlock");
        /// <summary>Full Path: Aequus/Content/Tiles/Conductive/ConductiveBlockTile</summary>
        public static readonly TextureAsset ConductiveBlockTile = new("Aequus/Content/Tiles/Conductive/ConductiveBlockTile");
        /// <summary>Full Path: Aequus/Content/Tiles/Conductive/ConductiveBlockTileElectricity</summary>
        public static readonly TextureAsset ConductiveBlockTileElectricity = new("Aequus/Content/Tiles/Conductive/ConductiveBlockTileElectricity");
        /// <summary>Full Path: Aequus/Content/Tiles/Conductive/ConductiveBlockTileTin</summary>
        public static readonly TextureAsset ConductiveBlockTileTin = new("Aequus/Content/Tiles/Conductive/ConductiveBlockTileTin");
        /// <summary>Full Path: Aequus/Content/Tiles/Conductive/Items/ConductiveBlockTin</summary>
        public static readonly TextureAsset ConductiveBlockTin = new("Aequus/Content/Tiles/Conductive/Items/ConductiveBlockTin");
        /// <summary>Full Path: Aequus/Content/Items/Material/Energy/Cosmic/CosmicEnergy</summary>
        public static readonly TextureAsset CosmicEnergy = new("Aequus/Content/Items/Material/Energy/Cosmic/CosmicEnergy");
        /// <summary>Full Path: Aequus/Content/Items/Material/Energy/Cosmic/CosmicEnergyParticle</summary>
        public static readonly TextureAsset CosmicEnergyParticle = new("Aequus/Content/Items/Material/Energy/Cosmic/CosmicEnergyParticle");
        /// <summary>Full Path: Aequus/Content/Items/Misc/GrabBags/TreasureBags/CrabsonBag</summary>
        public static readonly TextureAsset CrabsonBag = new("Aequus/Content/Items/Misc/GrabBags/TreasureBags/CrabsonBag");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Armor/Vanity/CrabsonMask</summary>
        public static readonly TextureAsset CrabsonMask = new("Aequus/Content/Items/Equipment/Armor/Vanity/CrabsonMask");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Armor/Vanity/CrabsonMask_Head</summary>
        public static readonly TextureAsset CrabsonMask_Head = new("Aequus/Content/Items/Equipment/Armor/Vanity/CrabsonMask_Head");
        /// <summary>Full Path: Aequus/Content/Tiles/Furniture/Boss/Relics/CrabsonRelic</summary>
        public static readonly TextureAsset CrabsonRelic = new("Aequus/Content/Tiles/Furniture/Boss/Relics/CrabsonRelic");
        /// <summary>Full Path: Aequus/Content/Tiles/Furniture/Boss/Trophies/CrabsonTrophy</summary>
        public static readonly TextureAsset CrabsonTrophy = new("Aequus/Content/Tiles/Furniture/Boss/Trophies/CrabsonTrophy");
        /// <summary>Full Path: Aequus/Content/Items/Material/Energy/Demonic/DemonicEnergy</summary>
        public static readonly TextureAsset DemonicEnergy = new("Aequus/Content/Items/Material/Energy/Demonic/DemonicEnergy");
        /// <summary>Full Path: Aequus/Content/Items/Material/Energy/Demonic/DemonicEnergyParticle</summary>
        public static readonly TextureAsset DemonicEnergyParticle = new("Aequus/Content/Items/Material/Energy/Demonic/DemonicEnergyParticle");
        /// <summary>Full Path: Aequus/Content/Items/Misc/Dyes/Disco/DiscoDye</summary>
        public static readonly TextureAsset DiscoDye = new("Aequus/Content/Items/Misc/Dyes/Disco/DiscoDye");
        /// <summary>Full Path: Aequus/Content/Items/Misc/GrabBags/TreasureBags/DustDevilBag</summary>
        public static readonly TextureAsset DustDevilBag = new("Aequus/Content/Items/Misc/GrabBags/TreasureBags/DustDevilBag");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Armor/Vanity/DustDevilMaskFire</summary>
        public static readonly TextureAsset DustDevilMaskFire = new("Aequus/Content/Items/Equipment/Armor/Vanity/DustDevilMaskFire");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Armor/Vanity/DustDevilMaskFire_Head</summary>
        public static readonly TextureAsset DustDevilMaskFire_Head = new("Aequus/Content/Items/Equipment/Armor/Vanity/DustDevilMaskFire_Head");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Armor/Vanity/DustDevilMaskIce</summary>
        public static readonly TextureAsset DustDevilMaskIce = new("Aequus/Content/Items/Equipment/Armor/Vanity/DustDevilMaskIce");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Armor/Vanity/DustDevilMaskIce_Head</summary>
        public static readonly TextureAsset DustDevilMaskIce_Head = new("Aequus/Content/Items/Equipment/Armor/Vanity/DustDevilMaskIce_Head");
        /// <summary>Full Path: Aequus/Content/Tiles/Furniture/Boss/Relics/DustDevilRelic</summary>
        public static readonly TextureAsset DustDevilRelic = new("Aequus/Content/Tiles/Furniture/Boss/Relics/DustDevilRelic");
        /// <summary>Full Path: Aequus/Content/Tiles/Furniture/Boss/Trophies/DustDevilTrophy</summary>
        public static readonly TextureAsset DustDevilTrophy = new("Aequus/Content/Tiles/Furniture/Boss/Trophies/DustDevilTrophy");
        /// <summary>Full Path: Aequus/Assets/Textures/EffectNoise</summary>
        public static readonly TextureAsset EffectNoise = new("Aequus/Assets/Textures/EffectNoise");
        /// <summary>Full Path: Aequus/Assets/Textures/EffectPerlin</summary>
        public static readonly TextureAsset EffectPerlin = new("Aequus/Assets/Textures/EffectPerlin");
        /// <summary>Full Path: Aequus/Assets/Textures/EffectWaterRefraction</summary>
        public static readonly TextureAsset EffectWaterRefraction = new("Aequus/Assets/Textures/EffectWaterRefraction");
        /// <summary>Full Path: Aequus/Content/Items/Misc/Dyes/Enchanted/EnchantedDye</summary>
        public static readonly TextureAsset EnchantedDye = new("Aequus/Content/Items/Misc/Dyes/Enchanted/EnchantedDye");
        /// <summary>Full Path: Aequus/Content/Items/Misc/Dyes/Enchanted/EnchantedDyeEffect</summary>
        public static readonly TextureAsset EnchantedDyeEffect = new("Aequus/Content/Items/Misc/Dyes/Enchanted/EnchantedDyeEffect");
        /// <summary>Full Path: Aequus/Content/Items/Material/Energy/EnergyParticle</summary>
        public static readonly TextureAsset EnergyParticle = new("Aequus/Content/Items/Material/Energy/EnergyParticle");
        /// <summary>Full Path: Aequus/Assets/Textures/Exclamation</summary>
        public static readonly TextureAsset Exclamation = new("Aequus/Assets/Textures/Exclamation");
        /// <summary>Full Path: Aequus/Content/DedicatedContent/Crabs/FamiliarBuff</summary>
        public static readonly TextureAsset FamiliarBuff = new("Aequus/Content/DedicatedContent/Crabs/FamiliarBuff");
        /// <summary>Full Path: Aequus/Content/DedicatedContent/Crabs/FamiliarPickaxe</summary>
        public static readonly TextureAsset FamiliarPickaxe = new("Aequus/Content/DedicatedContent/Crabs/FamiliarPickaxe");
        /// <summary>Full Path: Aequus/Assets/Textures/Flare</summary>
        public static readonly TextureAsset Flare = new("Aequus/Assets/Textures/Flare");
        /// <summary>Full Path: Aequus/Assets/Textures/Flare2</summary>
        public static readonly TextureAsset Flare2 = new("Aequus/Assets/Textures/Flare2");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Accessories/Movement/FlashwayShield/FlashwayShield</summary>
        public static readonly TextureAsset FlashwayShield = new("Aequus/Content/Items/Equipment/Accessories/Movement/FlashwayShield/FlashwayShield");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Accessories/Movement/FlashwayShield/FlashwayShield_Shield</summary>
        public static readonly TextureAsset FlashwayShield_Shield = new("Aequus/Content/Items/Equipment/Accessories/Movement/FlashwayShield/FlashwayShield_Shield");
        /// <summary>Full Path: Aequus/Content/Items/Material/Fluorescence</summary>
        public static readonly TextureAsset Fluorescence = new("Aequus/Content/Items/Material/Fluorescence");
        /// <summary>Full Path: Aequus/Assets/Textures/Fog</summary>
        public static readonly TextureAsset Fog = new("Aequus/Assets/Textures/Fog");
        /// <summary>Full Path: Aequus/Content/Items/Misc/Dyes/Frostbite/FrostbiteDye</summary>
        public static readonly TextureAsset FrostbiteDye = new("Aequus/Content/Items/Misc/Dyes/Frostbite/FrostbiteDye");
        /// <summary>Full Path: Aequus/Content/Items/Misc/Dyes/Frostbite/FrostbiteDyeEffect</summary>
        public static readonly TextureAsset FrostbiteDyeEffect = new("Aequus/Content/Items/Misc/Dyes/Frostbite/FrostbiteDyeEffect");
        /// <summary>Full Path: Aequus/Content/Items/Material/FrozenTear</summary>
        public static readonly TextureAsset FrozenTear = new("Aequus/Content/Items/Material/FrozenTear");
        /// <summary>Full Path: Aequus/Content/Items/Weapons/Magic/Furystar/Furystar</summary>
        public static readonly TextureAsset Furystar = new("Aequus/Content/Items/Weapons/Magic/Furystar/Furystar");
        /// <summary>Full Path: Aequus/Assets/Textures/GenericExplosion</summary>
        public static readonly TextureAsset GenericExplosion = new("Aequus/Assets/Textures/GenericExplosion");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Accessories/Restoration/GoldenFeather/GoldenFeather</summary>
        public static readonly TextureAsset GoldenFeather = new("Aequus/Content/Items/Equipment/Accessories/Restoration/GoldenFeather/GoldenFeather");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Accessories/Restoration/GoldenFeather/GoldenFeatherBuff</summary>
        public static readonly TextureAsset GoldenFeatherBuff = new("Aequus/Content/Items/Equipment/Accessories/Restoration/GoldenFeather/GoldenFeatherBuff");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Accessories/Restoration/GoldenFeather/GoldenWind</summary>
        public static readonly TextureAsset GoldenWind = new("Aequus/Content/Items/Equipment/Accessories/Restoration/GoldenFeather/GoldenWind");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Accessories/Restoration/GoldenFeather/GoldenWindBuff</summary>
        public static readonly TextureAsset GoldenWindBuff = new("Aequus/Content/Items/Equipment/Accessories/Restoration/GoldenFeather/GoldenWindBuff");
        /// <summary>Full Path: Aequus/Content/Items/Potions/Healing/Restoration/GreaterRestorationPotion</summary>
        public static readonly TextureAsset GreaterRestorationPotion = new("Aequus/Content/Items/Potions/Healing/Restoration/GreaterRestorationPotion");
        /// <summary>Full Path: Aequus/Content/Items/Misc/Dyes/Gust/GustDye</summary>
        public static readonly TextureAsset GustDye = new("Aequus/Content/Items/Misc/Dyes/Gust/GustDye");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Mounts/HotAirBalloon/HotAirBalloonBuff</summary>
        public static readonly TextureAsset HotAirBalloonBuff = new("Aequus/Content/Items/Equipment/Mounts/HotAirBalloon/HotAirBalloonBuff");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Mounts/HotAirBalloon/HotAirBalloonMount</summary>
        public static readonly TextureAsset HotAirBalloonMount = new("Aequus/Content/Items/Equipment/Mounts/HotAirBalloon/HotAirBalloonMount");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Mounts/HotAirBalloon/HotAirBalloonMount_Back</summary>
        public static readonly TextureAsset HotAirBalloonMount_Back = new("Aequus/Content/Items/Equipment/Mounts/HotAirBalloon/HotAirBalloonMount_Back");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Mounts/HotAirBalloon/HotAirBalloonMount_Front</summary>
        public static readonly TextureAsset HotAirBalloonMount_Front = new("Aequus/Content/Items/Equipment/Mounts/HotAirBalloon/HotAirBalloonMount_Front");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Mounts/HotAirBalloon/HotAirBalloonMount_Glow</summary>
        public static readonly TextureAsset HotAirBalloonMount_Glow = new("Aequus/Content/Items/Equipment/Mounts/HotAirBalloon/HotAirBalloonMount_Glow");
        /// <summary>Full Path: Aequus/Content/Items/Misc/Dyes/Hueshift/HueshiftDye</summary>
        public static readonly TextureAsset HueshiftDye = new("Aequus/Content/Items/Misc/Dyes/Hueshift/HueshiftDye");
        /// <summary>Full Path: Aequus/icon</summary>
        public static readonly TextureAsset icon = new("Aequus/icon");
        /// <summary>Full Path: Aequus/icon_small</summary>
        public static readonly TextureAsset icon_small = new("Aequus/icon_small");
        /// <summary>Full Path: Aequus/icon_workshop</summary>
        public static readonly TextureAsset icon_workshop = new("Aequus/icon_workshop");
        /// <summary>Full Path: Aequus/Assets/Textures/LightRayCircular</summary>
        public static readonly TextureAsset LightRayCircular = new("Aequus/Assets/Textures/LightRayCircular");
        /// <summary>Full Path: Aequus/Assets/Textures/LightRayFlat</summary>
        public static readonly TextureAsset LightRayFlat = new("Aequus/Assets/Textures/LightRayFlat");
        /// <summary>Full Path: Aequus/Content/Items/Material/MonoGem/MonoGem</summary>
        public static readonly TextureAsset MonoGem = new("Aequus/Content/Items/Material/MonoGem/MonoGem");
        /// <summary>Full Path: Aequus/Content/Items/Material/MonoGem/MonoGemTile</summary>
        public static readonly TextureAsset MonoGemTile = new("Aequus/Content/Items/Material/MonoGem/MonoGemTile");
        /// <summary>Full Path: Aequus/Content/Items/Misc/PermaPowerups/NetherStar/NetherStar</summary>
        public static readonly TextureAsset NetherStar = new("Aequus/Content/Items/Misc/PermaPowerups/NetherStar/NetherStar");
        /// <summary>Full Path: Aequus/Content/Items/Misc/PermaPowerups/NetherStar/NetherStar_Glow</summary>
        public static readonly TextureAsset NetherStar_Glow = new("Aequus/Content/Items/Misc/PermaPowerups/NetherStar/NetherStar_Glow");
        /// <summary>Full Path: Aequus/Content/Items/Misc/PermaPowerups/NetherStar/NetherStarBackground</summary>
        public static readonly TextureAsset NetherStarBackground = new("Aequus/Content/Items/Misc/PermaPowerups/NetherStar/NetherStarBackground");
        /// <summary>Full Path: Aequus/Content/Items/Misc/PermaPowerups/NetherStar/NetherStarMask</summary>
        public static readonly TextureAsset NetherStarMask = new("Aequus/Content/Items/Misc/PermaPowerups/NetherStar/NetherStarMask");
        /// <summary>Full Path: Aequus/Content/Items/Potions/Buffs/NeutronYogurt/NeutronYogurt</summary>
        public static readonly TextureAsset NeutronYogurt = new("Aequus/Content/Items/Potions/Buffs/NeutronYogurt/NeutronYogurt");
        /// <summary>Full Path: Aequus/Content/Items/Potions/Buffs/NeutronYogurt/NeutronYogurtBuff</summary>
        public static readonly TextureAsset NeutronYogurtBuff = new("Aequus/Content/Items/Potions/Buffs/NeutronYogurt/NeutronYogurtBuff");
        /// <summary>Full Path: Aequus/Content/Items/Misc/GrabBags/TreasureBags/OmegaStariteBag</summary>
        public static readonly TextureAsset OmegaStariteBag = new("Aequus/Content/Items/Misc/GrabBags/TreasureBags/OmegaStariteBag");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Armor/Vanity/OmegaStariteMask</summary>
        public static readonly TextureAsset OmegaStariteMask = new("Aequus/Content/Items/Equipment/Armor/Vanity/OmegaStariteMask");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Armor/Vanity/OmegaStariteMask_Glow</summary>
        public static readonly TextureAsset OmegaStariteMask_Glow = new("Aequus/Content/Items/Equipment/Armor/Vanity/OmegaStariteMask_Glow");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Armor/Vanity/OmegaStariteMask_Head</summary>
        public static readonly TextureAsset OmegaStariteMask_Head = new("Aequus/Content/Items/Equipment/Armor/Vanity/OmegaStariteMask_Head");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Armor/Vanity/OmegaStariteMask_Head_Glow</summary>
        public static readonly TextureAsset OmegaStariteMask_Head_Glow = new("Aequus/Content/Items/Equipment/Armor/Vanity/OmegaStariteMask_Head_Glow");
        /// <summary>Full Path: Aequus/Content/Tiles/Furniture/Boss/Relics/OmegaStariteRelic</summary>
        public static readonly TextureAsset OmegaStariteRelic = new("Aequus/Content/Tiles/Furniture/Boss/Relics/OmegaStariteRelic");
        /// <summary>Full Path: Aequus/Content/Tiles/Furniture/Boss/Trophies/OmegaStariteTrophy</summary>
        public static readonly TextureAsset OmegaStariteTrophy = new("Aequus/Content/Tiles/Furniture/Boss/Trophies/OmegaStariteTrophy");
        /// <summary>Full Path: Aequus/Content/Items/Material/OmniGem/OmniGem</summary>
        public static readonly TextureAsset OmniGem = new("Aequus/Content/Items/Material/OmniGem/OmniGem");
        /// <summary>Full Path: Aequus/Content/Items/Material/OmniGem/OmniGem_Mask</summary>
        public static readonly TextureAsset OmniGem_Mask = new("Aequus/Content/Items/Material/OmniGem/OmniGem_Mask");
        /// <summary>Full Path: Aequus/Content/Items/Material/OmniGem/OmniGemTile</summary>
        public static readonly TextureAsset OmniGemTile = new("Aequus/Content/Items/Material/OmniGem/OmniGemTile");
        /// <summary>Full Path: Aequus/Content/Items/Material/OmniGem/OmniGemTile_Mask</summary>
        public static readonly TextureAsset OmniGemTile_Mask = new("Aequus/Content/Items/Material/OmniGem/OmniGemTile_Mask");
        /// <summary>Full Path: Aequus/Content/Items/Material/Energy/Organic/OrganicEnergy</summary>
        public static readonly TextureAsset OrganicEnergy = new("Aequus/Content/Items/Material/Energy/Organic/OrganicEnergy");
        /// <summary>Full Path: Aequus/Content/Items/Material/Energy/Organic/OrganicEnergyParticle</summary>
        public static readonly TextureAsset OrganicEnergyParticle = new("Aequus/Content/Items/Material/Energy/Organic/OrganicEnergyParticle");
        /// <summary>Full Path: Aequus/Content/Items/Misc/Dyes/Outline/OutlineDye</summary>
        public static readonly TextureAsset OutlineDye = new("Aequus/Content/Items/Misc/Dyes/Outline/OutlineDye");
        /// <summary>Full Path: Aequus/Content/Items/Tools/MagicMirrors/PhaseMirror/PhaseMirror</summary>
        public static readonly TextureAsset PhaseMirror = new("Aequus/Content/Items/Tools/MagicMirrors/PhaseMirror/PhaseMirror");
        /// <summary>Full Path: Aequus/Content/Items/Tools/MagicMirrors/PhasePhone/PhasePhone</summary>
        public static readonly TextureAsset PhasePhone = new("Aequus/Content/Items/Tools/MagicMirrors/PhasePhone/PhasePhone");
        /// <summary>Full Path: Aequus/Content/Items/Tools/MagicMirrors/PhasePhone/PhasePhoneHome</summary>
        public static readonly TextureAsset PhasePhoneHome = new("Aequus/Content/Items/Tools/MagicMirrors/PhasePhone/PhasePhoneHome");
        /// <summary>Full Path: Aequus/Content/Items/Tools/MagicMirrors/PhasePhone/PhasePhoneOcean</summary>
        public static readonly TextureAsset PhasePhoneOcean = new("Aequus/Content/Items/Tools/MagicMirrors/PhasePhone/PhasePhoneOcean");
        /// <summary>Full Path: Aequus/Content/Items/Tools/MagicMirrors/PhasePhone/PhasePhoneSpawn</summary>
        public static readonly TextureAsset PhasePhoneSpawn = new("Aequus/Content/Items/Tools/MagicMirrors/PhasePhone/PhasePhoneSpawn");
        /// <summary>Full Path: Aequus/Content/Items/Tools/MagicMirrors/PhasePhone/PhasePhoneUnderworld</summary>
        public static readonly TextureAsset PhasePhoneUnderworld = new("Aequus/Content/Items/Tools/MagicMirrors/PhasePhone/PhasePhoneUnderworld");
        /// <summary>Full Path: Aequus/Content/Items/Weapons/Melee/Yoyo/Pistachiyo/Pistachiyo</summary>
        public static readonly TextureAsset Pistachiyo = new("Aequus/Content/Items/Weapons/Melee/Yoyo/Pistachiyo/Pistachiyo");
        /// <summary>Full Path: Aequus/Content/Items/Weapons/Melee/Yoyo/Pistachiyo/PistachiyoProj</summary>
        public static readonly TextureAsset PistachiyoProj = new("Aequus/Content/Items/Weapons/Melee/Yoyo/Pistachiyo/PistachiyoProj");
        /// <summary>Full Path: Aequus/Content/Items/Weapons/Melee/Yoyo/Pistachiyo/PistachiyoProj_Shells</summary>
        public static readonly TextureAsset PistachiyoProj_Shells = new("Aequus/Content/Items/Weapons/Melee/Yoyo/Pistachiyo/PistachiyoProj_Shells");
        /// <summary>Full Path: Aequus/Content/Items/Tools/Pumpinator/Pumpinator</summary>
        public static readonly TextureAsset Pumpinator = new("Aequus/Content/Items/Tools/Pumpinator/Pumpinator");
        /// <summary>Full Path: Aequus/Content/Items/Tools/Pumpinator/Pumpinator_Glow</summary>
        public static readonly TextureAsset Pumpinator_Glow = new("Aequus/Content/Items/Tools/Pumpinator/Pumpinator_Glow");
        /// <summary>Full Path: Aequus/Content/Items/Tools/Pumpinator/PumpinatorProj</summary>
        public static readonly TextureAsset PumpinatorProj = new("Aequus/Content/Items/Tools/Pumpinator/PumpinatorProj");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Accessories/Fishing/RadonFishingBobber/RadonFishingBobber</summary>
        public static readonly TextureAsset RadonFishingBobber = new("Aequus/Content/Items/Equipment/Accessories/Fishing/RadonFishingBobber/RadonFishingBobber");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Accessories/Fishing/RadonFishingBobber/RadonFishingBobberBuff</summary>
        public static readonly TextureAsset RadonFishingBobberBuff = new("Aequus/Content/Items/Equipment/Accessories/Fishing/RadonFishingBobber/RadonFishingBobberBuff");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Accessories/Fishing/RadonFishingBobber/RadonFishingBobberProj</summary>
        public static readonly TextureAsset RadonFishingBobberProj = new("Aequus/Content/Items/Equipment/Accessories/Fishing/RadonFishingBobber/RadonFishingBobberProj");
        /// <summary>Full Path: Aequus/Content/Tiles/Radon/RadonMoss</summary>
        public static readonly TextureAsset RadonMoss = new("Aequus/Content/Tiles/Radon/RadonMoss");
        /// <summary>Full Path: Aequus/Content/Tiles/Radon/Brick/RadonMossBrickCraftedTile</summary>
        public static readonly TextureAsset RadonMossBrickCraftedTile = new("Aequus/Content/Tiles/Radon/Brick/RadonMossBrickCraftedTile");
        /// <summary>Full Path: Aequus/Content/Tiles/Radon/Brick/RadonMossBrickItem</summary>
        public static readonly TextureAsset RadonMossBrickItem = new("Aequus/Content/Tiles/Radon/Brick/RadonMossBrickItem");
        /// <summary>Full Path: Aequus/Content/Tiles/Radon/RadonMossBrickTile</summary>
        public static readonly TextureAsset RadonMossBrickTile = new("Aequus/Content/Tiles/Radon/RadonMossBrickTile");
        /// <summary>Full Path: Aequus/Content/Tiles/Radon/Brick/RadonMossBrickWallItem</summary>
        public static readonly TextureAsset RadonMossBrickWallItem = new("Aequus/Content/Tiles/Radon/Brick/RadonMossBrickWallItem");
        /// <summary>Full Path: Aequus/Content/Tiles/Radon/Brick/RadonMossBrickWallPlaced</summary>
        public static readonly TextureAsset RadonMossBrickWallPlaced = new("Aequus/Content/Tiles/Radon/Brick/RadonMossBrickWallPlaced");
        /// <summary>Full Path: Aequus/Content/Tiles/Radon/RadonMossGrass</summary>
        public static readonly TextureAsset RadonMossGrass = new("Aequus/Content/Tiles/Radon/RadonMossGrass");
        /// <summary>Full Path: Aequus/Content/Tiles/Radon/RadonMossTile</summary>
        public static readonly TextureAsset RadonMossTile = new("Aequus/Content/Tiles/Radon/RadonMossTile");
        /// <summary>Full Path: Aequus/Content/Items/Misc/Dyes/Outline/RainbowOutlineDye</summary>
        public static readonly TextureAsset RainbowOutlineDye = new("Aequus/Content/Items/Misc/Dyes/Outline/RainbowOutlineDye");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Armor/Vanity/RedSpriteMask</summary>
        public static readonly TextureAsset RedSpriteMask = new("Aequus/Content/Items/Equipment/Armor/Vanity/RedSpriteMask");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Armor/Vanity/RedSpriteMask_Glow</summary>
        public static readonly TextureAsset RedSpriteMask_Glow = new("Aequus/Content/Items/Equipment/Armor/Vanity/RedSpriteMask_Glow");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Armor/Vanity/RedSpriteMask_Head</summary>
        public static readonly TextureAsset RedSpriteMask_Head = new("Aequus/Content/Items/Equipment/Armor/Vanity/RedSpriteMask_Head");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Armor/Vanity/RedSpriteMask_Head_Glow</summary>
        public static readonly TextureAsset RedSpriteMask_Head_Glow = new("Aequus/Content/Items/Equipment/Armor/Vanity/RedSpriteMask_Head_Glow");
        /// <summary>Full Path: Aequus/Content/Tiles/Furniture/Boss/Relics/RedSpriteRelic</summary>
        public static readonly TextureAsset RedSpriteRelic = new("Aequus/Content/Tiles/Furniture/Boss/Relics/RedSpriteRelic");
        /// <summary>Full Path: Aequus/Content/Tiles/Furniture/Boss/Trophies/RedSpriteTrophy</summary>
        public static readonly TextureAsset RedSpriteTrophy = new("Aequus/Content/Tiles/Furniture/Boss/Trophies/RedSpriteTrophy");
        /// <summary>Full Path: Aequus/Content/TownNPCs/SkyMerchant/UI/RenameBackIcon</summary>
        public static readonly TextureAsset RenameBackIcon = new("Aequus/Content/TownNPCs/SkyMerchant/UI/RenameBackIcon");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Accessories/Informational/Monocle/RichMansMonocle</summary>
        public static readonly TextureAsset RichMansMonocle = new("Aequus/Content/Items/Equipment/Accessories/Informational/Monocle/RichMansMonocle");
        /// <summary>Full Path: Aequus/Content/Items/Misc/Dyes/Scorching/ScorchingDye</summary>
        public static readonly TextureAsset ScorchingDye = new("Aequus/Content/Items/Misc/Dyes/Scorching/ScorchingDye");
        /// <summary>Full Path: Aequus/Content/Items/Misc/Dyes/Scroll/ScrollDye</summary>
        public static readonly TextureAsset ScrollDye = new("Aequus/Content/Items/Misc/Dyes/Scroll/ScrollDye");
        /// <summary>Full Path: Aequus/Assets/Textures/Shatter</summary>
        public static readonly TextureAsset Shatter = new("Aequus/Assets/Textures/Shatter");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Accessories/Informational/Monocle/ShimmerMonocle</summary>
        public static readonly TextureAsset ShimmerMonocle = new("Aequus/Content/Items/Equipment/Accessories/Informational/Monocle/ShimmerMonocle");
        /// <summary>Full Path: Aequus/Content/Items/Misc/Dyes/Simplified/SimplifiedDye</summary>
        public static readonly TextureAsset SimplifiedDye = new("Aequus/Content/Items/Misc/Dyes/Simplified/SimplifiedDye");
        /// <summary>Full Path: Aequus/Content/Items/Weapons/Ranged/Bows/SkyHunterCrossbow/SkyHunterCrossbow</summary>
        public static readonly TextureAsset SkyHunterCrossbow = new("Aequus/Content/Items/Weapons/Ranged/Bows/SkyHunterCrossbow/SkyHunterCrossbow");
        /// <summary>Full Path: Aequus/Content/TownNPCs/SkyMerchant/Shimmer/SkyMerchant_Aiming</summary>
        public static readonly TextureAsset SkyMerchant_Aiming_Shimmer = new("Aequus/Content/TownNPCs/SkyMerchant/Shimmer/SkyMerchant_Aiming");
        /// <summary>Full Path: Aequus/Content/TownNPCs/SkyMerchant/SkyMerchant_Aiming</summary>
        public static readonly TextureAsset SkyMerchant_Aiming_SkyMerchant = new("Aequus/Content/TownNPCs/SkyMerchant/SkyMerchant_Aiming");
        /// <summary>Full Path: Aequus/Content/TownNPCs/SkyMerchant/Shimmer/SkyMerchant_CrossbowArm</summary>
        public static readonly TextureAsset SkyMerchant_CrossbowArm_Shimmer = new("Aequus/Content/TownNPCs/SkyMerchant/Shimmer/SkyMerchant_CrossbowArm");
        /// <summary>Full Path: Aequus/Content/TownNPCs/SkyMerchant/SkyMerchant_CrossbowArm</summary>
        public static readonly TextureAsset SkyMerchant_CrossbowArm_SkyMerchant = new("Aequus/Content/TownNPCs/SkyMerchant/SkyMerchant_CrossbowArm");
        /// <summary>Full Path: Aequus/Content/TownNPCs/SkyMerchant/SkyMerchant_CustomHead</summary>
        public static readonly TextureAsset SkyMerchant_CustomHead = new("Aequus/Content/TownNPCs/SkyMerchant/SkyMerchant_CustomHead");
        /// <summary>Full Path: Aequus/Content/TownNPCs/SkyMerchant/Shimmer/SkyMerchant_Head</summary>
        public static readonly TextureAsset SkyMerchant_Head_Shimmer = new("Aequus/Content/TownNPCs/SkyMerchant/Shimmer/SkyMerchant_Head");
        /// <summary>Full Path: Aequus/Content/TownNPCs/SkyMerchant/SkyMerchant_Head</summary>
        public static readonly TextureAsset SkyMerchant_Head_SkyMerchant = new("Aequus/Content/TownNPCs/SkyMerchant/SkyMerchant_Head");
        /// <summary>Full Path: Aequus/Content/TownNPCs/SkyMerchant/Shimmer/SkyMerchant</summary>
        public static readonly TextureAsset SkyMerchant_Shimmer = new("Aequus/Content/TownNPCs/SkyMerchant/Shimmer/SkyMerchant");
        /// <summary>Full Path: Aequus/Content/TownNPCs/SkyMerchant/SkyMerchant</summary>
        public static readonly TextureAsset SkyMerchant_SkyMerchant = new("Aequus/Content/TownNPCs/SkyMerchant/SkyMerchant");
        /// <summary>Full Path: Aequus/Content/TownNPCs/SkyMerchant/Emote/SkyMerchantEmote</summary>
        public static readonly TextureAsset SkyMerchantEmote = new("Aequus/Content/TownNPCs/SkyMerchant/Emote/SkyMerchantEmote");
        /// <summary>Full Path: Aequus/Assets/Textures/SlashVanilla</summary>
        public static readonly TextureAsset SlashVanilla = new("Aequus/Assets/Textures/SlashVanilla");
        /// <summary>Full Path: Aequus/Assets/Textures/SlashVanillaSmall</summary>
        public static readonly TextureAsset SlashVanillaSmall = new("Aequus/Assets/Textures/SlashVanillaSmall");
        /// <summary>Full Path: Aequus/Content/Items/Weapons/Melee/Swords/Slice/Slice</summary>
        public static readonly TextureAsset Slice = new("Aequus/Content/Items/Weapons/Melee/Swords/Slice/Slice");
        /// <summary>Full Path: Aequus/Content/Items/Weapons/Melee/Swords/Slice/SliceBulletProj</summary>
        public static readonly TextureAsset SliceBulletProj = new("Aequus/Content/Items/Weapons/Melee/Swords/Slice/SliceBulletProj");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Accessories/Movement/SlimyBlueBalloon/SlimyBlueBalloon</summary>
        public static readonly TextureAsset SlimyBlueBalloon = new("Aequus/Content/Items/Equipment/Accessories/Movement/SlimyBlueBalloon/SlimyBlueBalloon");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Accessories/Movement/SlimyBlueBalloon/SlimyBlueBalloon_Balloon</summary>
        public static readonly TextureAsset SlimyBlueBalloon_Balloon = new("Aequus/Content/Items/Equipment/Accessories/Movement/SlimyBlueBalloon/SlimyBlueBalloon_Balloon");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Armor/Vanity/SpaceSquidMask</summary>
        public static readonly TextureAsset SpaceSquidMask = new("Aequus/Content/Items/Equipment/Armor/Vanity/SpaceSquidMask");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Armor/Vanity/SpaceSquidMask_Glow</summary>
        public static readonly TextureAsset SpaceSquidMask_Glow = new("Aequus/Content/Items/Equipment/Armor/Vanity/SpaceSquidMask_Glow");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Armor/Vanity/SpaceSquidMask_Head</summary>
        public static readonly TextureAsset SpaceSquidMask_Head = new("Aequus/Content/Items/Equipment/Armor/Vanity/SpaceSquidMask_Head");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Armor/Vanity/SpaceSquidMask_Head_Glow</summary>
        public static readonly TextureAsset SpaceSquidMask_Head_Glow = new("Aequus/Content/Items/Equipment/Armor/Vanity/SpaceSquidMask_Head_Glow");
        /// <summary>Full Path: Aequus/Content/Tiles/Furniture/Boss/Relics/SpaceSquidRelic</summary>
        public static readonly TextureAsset SpaceSquidRelic = new("Aequus/Content/Tiles/Furniture/Boss/Relics/SpaceSquidRelic");
        /// <summary>Full Path: Aequus/Content/Tiles/Furniture/Boss/Trophies/SpaceSquidTrophy</summary>
        public static readonly TextureAsset SpaceSquidTrophy = new("Aequus/Content/Tiles/Furniture/Boss/Trophies/SpaceSquidTrophy");
        /// <summary>Full Path: Aequus/Assets/Textures/Sparkles</summary>
        public static readonly TextureAsset Sparkles = new("Aequus/Assets/Textures/Sparkles");
        /// <summary>Full Path: Aequus/Content/Items/Weapons/Classless/StunGun/StunEffect</summary>
        public static readonly TextureAsset StunEffect = new("Aequus/Content/Items/Weapons/Classless/StunGun/StunEffect");
        /// <summary>Full Path: Aequus/Content/Items/Weapons/Classless/StunGun/StunGun</summary>
        public static readonly TextureAsset StunGun = new("Aequus/Content/Items/Weapons/Classless/StunGun/StunGun");
        /// <summary>Full Path: Aequus/Content/Items/Potions/Healing/Restoration/SuperRestorationPotion</summary>
        public static readonly TextureAsset SuperRestorationPotion = new("Aequus/Content/Items/Potions/Healing/Restoration/SuperRestorationPotion");
        /// <summary>Full Path: Aequus/Content/DedicatedContent/TorraTh/SwagLookingEye</summary>
        public static readonly TextureAsset SwagLookingEye = new("Aequus/Content/DedicatedContent/TorraTh/SwagLookingEye");
        /// <summary>Full Path: Aequus/Content/Items/Misc/Dyes/Tidal/TidalDye</summary>
        public static readonly TextureAsset TidalDye = new("Aequus/Content/Items/Misc/Dyes/Tidal/TidalDye");
        /// <summary>Full Path: Aequus/Content/Items/Misc/Dyes/Tidal/TidalDyeEffect</summary>
        public static readonly TextureAsset TidalDyeEffect = new("Aequus/Content/Items/Misc/Dyes/Tidal/TidalDyeEffect");
        /// <summary>Full Path: Aequus/Content/DedicatedContent/TorraTh/TorraBuff</summary>
        public static readonly TextureAsset TorraBuff = new("Aequus/Content/DedicatedContent/TorraTh/TorraBuff");
        /// <summary>Full Path: Aequus/Content/DedicatedContent/TorraTh/TorraPet</summary>
        public static readonly TextureAsset TorraPet = new("Aequus/Content/DedicatedContent/TorraTh/TorraPet");
        /// <summary>Full Path: Aequus/Assets/Textures/TownNPCExclamation</summary>
        public static readonly TextureAsset TownNPCExclamation = new("Aequus/Assets/Textures/TownNPCExclamation");
        /// <summary>Full Path: Aequus/Content/Tiles/Furniture/Boss/Trophy</summary>
        public static readonly TextureAsset Trophy = new("Aequus/Content/Tiles/Furniture/Boss/Trophy");
        /// <summary>Full Path: Aequus/Content/Items/Material/Energy/Ultimate/UltimateEnergy</summary>
        public static readonly TextureAsset UltimateEnergy = new("Aequus/Content/Items/Material/Energy/Ultimate/UltimateEnergy");
        /// <summary>Full Path: Aequus/Content/Tiles/Furniture/Boss/Relics/UltraStariteRelic</summary>
        public static readonly TextureAsset UltraStariteRelic = new("Aequus/Content/Tiles/Furniture/Boss/Relics/UltraStariteRelic");
        /// <summary>Full Path: Aequus/Content/Tiles/Furniture/Boss/Trophies/UltraStariteTrophy</summary>
        public static readonly TextureAsset UltraStariteTrophy = new("Aequus/Content/Tiles/Furniture/Boss/Trophies/UltraStariteTrophy");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Accessories/ValentinesRing/ValentinesRing</summary>
        public static readonly TextureAsset ValentinesRing = new("Aequus/Content/Items/Equipment/Accessories/ValentinesRing/ValentinesRing");
        /// <summary>Full Path: Aequus/Assets/Textures/VignetteSmall</summary>
        public static readonly TextureAsset VignetteSmall = new("Aequus/Assets/Textures/VignetteSmall");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Accessories/Movement/WeightedHorseshoe/WeightedHorseshoe</summary>
        public static readonly TextureAsset WeightedHorseshoe = new("Aequus/Content/Items/Equipment/Accessories/Movement/WeightedHorseshoe/WeightedHorseshoe");
        /// <summary>Full Path: Aequus/Content/Items/Equipment/Accessories/Movement/WeightedHorseshoe/WeightedHorseshoeVisual</summary>
        public static readonly TextureAsset WeightedHorseshoeVisual = new("Aequus/Content/Items/Equipment/Accessories/Movement/WeightedHorseshoe/WeightedHorseshoeVisual");
    }
}