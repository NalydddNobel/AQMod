using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Aequus.Common;

namespace Aequus {
    /// <summary>(Amt Textures: 220)</summary>
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
        /// <summary>Full Path: Aequus/Content/Weapons/Melee/AncientCutlass/AncientCutlass</summary>
        public static readonly TextureAsset AncientCutlass = new("Aequus/Content/Weapons/Melee/AncientCutlass/AncientCutlass");
        /// <summary>Full Path: Aequus/Content/Items/Tools/AnglerLamp/AnglerLamp</summary>
        public static readonly TextureAsset AnglerLamp = new("Aequus/Content/Items/Tools/AnglerLamp/AnglerLamp");
        /// <summary>Full Path: Aequus/Content/Items/Tools/AnglerLamp/AnglerLamp_Glow</summary>
        public static readonly TextureAsset AnglerLamp_Glow = new("Aequus/Content/Items/Tools/AnglerLamp/AnglerLamp_Glow");
        /// <summary>Full Path: Aequus/Content/Items/Tools/AnglerLamp/AnglerLampOff</summary>
        public static readonly TextureAsset AnglerLampOff = new("Aequus/Content/Items/Tools/AnglerLamp/AnglerLampOff");
        /// <summary>Full Path: Aequus/Content/DedicatedContent/Baguette/Baguette</summary>
        public static readonly TextureAsset Baguette = new("Aequus/Content/DedicatedContent/Baguette/Baguette");
        /// <summary>Full Path: Aequus/Content/DedicatedContent/Baguette/BaguetteBuff</summary>
        public static readonly TextureAsset BaguetteBuff = new("Aequus/Content/DedicatedContent/Baguette/BaguetteBuff");
        /// <summary>Full Path: Aequus/Content/TownNPCs/SkyMerchant/Shimmer/Balloon</summary>
        public static readonly TextureAsset Balloon_Shimmer = new("Aequus/Content/TownNPCs/SkyMerchant/Shimmer/Balloon");
        /// <summary>Full Path: Aequus/Content/TownNPCs/SkyMerchant/Balloon</summary>
        public static readonly TextureAsset Balloon_SkyMerchant = new("Aequus/Content/TownNPCs/SkyMerchant/Balloon");
        /// <summary>Full Path: Aequus/Content/Equipment/Mounts/HotAirBalloon/BalloonKit</summary>
        public static readonly TextureAsset BalloonKit = new("Aequus/Content/Equipment/Mounts/HotAirBalloon/BalloonKit");
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
        /// <summary>Full Path: Aequus/Content/Enemies/PollutedOcean/BlackJellyfish/BlackJellyfish</summary>
        public static readonly TextureAsset BlackJellyfish = new("Aequus/Content/Enemies/PollutedOcean/BlackJellyfish/BlackJellyfish");
        /// <summary>Full Path: Aequus/Content/Enemies/PollutedOcean/BlackJellyfish/BlackJellyfish_Bag</summary>
        public static readonly TextureAsset BlackJellyfish_Bag = new("Aequus/Content/Enemies/PollutedOcean/BlackJellyfish/BlackJellyfish_Bag");
        /// <summary>Full Path: Aequus/Content/Enemies/PollutedOcean/BlackJellyfish/BlackJellyfishBanner</summary>
        public static readonly TextureAsset BlackJellyfishBanner = new("Aequus/Content/Enemies/PollutedOcean/BlackJellyfish/BlackJellyfishBanner");
        /// <summary>Full Path: Aequus/Content/Enemies/PollutedOcean/BlackJellyfish/BlackJellyfishBannerItem</summary>
        public static readonly TextureAsset BlackJellyfishBannerItem = new("Aequus/Content/Enemies/PollutedOcean/BlackJellyfish/BlackJellyfishBannerItem");
        /// <summary>Full Path: Aequus/Content/Enemies/PollutedOcean/BlackJellyfish/BlackJellyfishVertexStrip</summary>
        public static readonly TextureAsset BlackJellyfishVertexStrip = new("Aequus/Content/Enemies/PollutedOcean/BlackJellyfish/BlackJellyfishVertexStrip");
        /// <summary>Full Path: Aequus/Assets/Textures/Bloom</summary>
        public static readonly TextureAsset Bloom = new("Aequus/Assets/Textures/Bloom");
        /// <summary>Full Path: Aequus/Assets/Textures/BloomStrong</summary>
        public static readonly TextureAsset BloomStrong = new("Aequus/Assets/Textures/BloomStrong");
        /// <summary>Full Path: Aequus/Content/Equipment/Armor/Vanity/BreadOfCthulhuMask</summary>
        public static readonly TextureAsset BreadOfCthulhuMask = new("Aequus/Content/Equipment/Armor/Vanity/BreadOfCthulhuMask");
        /// <summary>Full Path: Aequus/Content/Equipment/Armor/Vanity/BreadOfCthulhuMask_Head</summary>
        public static readonly TextureAsset BreadOfCthulhuMask_Head = new("Aequus/Content/Equipment/Armor/Vanity/BreadOfCthulhuMask_Head");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/Informational/Calendar/Calendar</summary>
        public static readonly TextureAsset Calendar = new("Aequus/Content/Equipment/Accessories/Informational/Calendar/Calendar");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/Informational/Calendar/CalendarInfoDisplay</summary>
        public static readonly TextureAsset CalendarInfoDisplay = new("Aequus/Content/Equipment/Accessories/Informational/Calendar/CalendarInfoDisplay");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/Informational/Calendar/CalendarTile</summary>
        public static readonly TextureAsset CalendarTile = new("Aequus/Content/Equipment/Accessories/Informational/Calendar/CalendarTile");
        /// <summary>Full Path: Aequus/Content/Dyes/Items/CensorDye</summary>
        public static readonly TextureAsset CensorDye = new("Aequus/Content/Dyes/Items/CensorDye");
        /// <summary>Full Path: Aequus/Content/Weapons/Ranged/Bows/SkyHunterCrossbow/Chain</summary>
        public static readonly TextureAsset Chain = new("Aequus/Content/Weapons/Ranged/Bows/SkyHunterCrossbow/Chain");
        /// <summary>Full Path: Aequus/Content/Items/Material/CompressedTrash</summary>
        public static readonly TextureAsset CompressedTrash = new("Aequus/Content/Items/Material/CompressedTrash");
        /// <summary>Full Path: Aequus/Content/Tiles/Conductive/ConductiveBlock</summary>
        public static readonly TextureAsset ConductiveBlock = new("Aequus/Content/Tiles/Conductive/ConductiveBlock");
        /// <summary>Full Path: Aequus/Content/Tiles/Conductive/ConductiveBlockItem</summary>
        public static readonly TextureAsset ConductiveBlockItem = new("Aequus/Content/Tiles/Conductive/ConductiveBlockItem");
        /// <summary>Full Path: Aequus/Content/Tiles/Conductive/ConductiveBlockTin</summary>
        public static readonly TextureAsset ConductiveBlockTin = new("Aequus/Content/Tiles/Conductive/ConductiveBlockTin");
        /// <summary>Full Path: Aequus/Content/Tiles/Conductive/ConductiveBlockTinItem</summary>
        public static readonly TextureAsset ConductiveBlockTinItem = new("Aequus/Content/Tiles/Conductive/ConductiveBlockTinItem");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/GrandReward/CosmicChest</summary>
        public static readonly TextureAsset CosmicChest = new("Aequus/Content/Equipment/Accessories/GrandReward/CosmicChest");
        /// <summary>Full Path: Aequus/Content/Fishing/CrabPots/CrabPot</summary>
        public static readonly TextureAsset CrabPot = new("Aequus/Content/Fishing/CrabPots/CrabPot");
        /// <summary>Full Path: Aequus/Content/Fishing/CrabPots/CrabPot_Back</summary>
        public static readonly TextureAsset CrabPot_Back = new("Aequus/Content/Fishing/CrabPots/CrabPot_Back");
        /// <summary>Full Path: Aequus/Content/Fishing/CrabPots/CrabPot_Highlight</summary>
        public static readonly TextureAsset CrabPot_Highlight = new("Aequus/Content/Fishing/CrabPots/CrabPot_Highlight");
        /// <summary>Full Path: Aequus/Content/Fishing/CrabPots/Items/CrabPotCopperItem</summary>
        public static readonly TextureAsset CrabPotCopperItem = new("Aequus/Content/Fishing/CrabPots/Items/CrabPotCopperItem");
        /// <summary>Full Path: Aequus/Content/Fishing/CrabPots/Items/CrabPotTinItem</summary>
        public static readonly TextureAsset CrabPotTinItem = new("Aequus/Content/Fishing/CrabPots/Items/CrabPotTinItem");
        /// <summary>Full Path: Aequus/Content/Bosses/TreasureBags/CrabsonBag</summary>
        public static readonly TextureAsset CrabsonBag = new("Aequus/Content/Bosses/TreasureBags/CrabsonBag");
        /// <summary>Full Path: Aequus/Content/Bosses/BossMasks/CrabsonMask</summary>
        public static readonly TextureAsset CrabsonMask = new("Aequus/Content/Bosses/BossMasks/CrabsonMask");
        /// <summary>Full Path: Aequus/Content/Bosses/BossMasks/CrabsonMask_Head</summary>
        public static readonly TextureAsset CrabsonMask_Head = new("Aequus/Content/Bosses/BossMasks/CrabsonMask_Head");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/CrabsonRelic</summary>
        public static readonly TextureAsset CrabsonRelic = new("Aequus/Content/Bosses/Trophies/CrabsonRelic");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/CrabsonRelicItem</summary>
        public static readonly TextureAsset CrabsonRelicItem = new("Aequus/Content/Bosses/Trophies/CrabsonRelicItem");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/CrabsonTrophy</summary>
        public static readonly TextureAsset CrabsonTrophy = new("Aequus/Content/Bosses/Trophies/CrabsonTrophy");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/CrabsonTrophyItem</summary>
        public static readonly TextureAsset CrabsonTrophyItem = new("Aequus/Content/Bosses/Trophies/CrabsonTrophyItem");
        /// <summary>Full Path: Aequus/Content/DedicatedContent/DeathsEmbrace/DeathsEmbrace</summary>
        public static readonly TextureAsset DeathsEmbrace = new("Aequus/Content/DedicatedContent/DeathsEmbrace/DeathsEmbrace");
        /// <summary>Full Path: Aequus/Content/DedicatedContent/DeathsEmbrace/DeathsEmbraceBuff</summary>
        public static readonly TextureAsset DeathsEmbraceBuff = new("Aequus/Content/DedicatedContent/DeathsEmbrace/DeathsEmbraceBuff");
        /// <summary>Full Path: Aequus/Assets/Textures/Debuff</summary>
        public static readonly TextureAsset Debuff = new("Aequus/Assets/Textures/Debuff");
        /// <summary>Full Path: Aequus/Content/DedicatedContent/DedicatedFaelingItem</summary>
        public static readonly TextureAsset DedicatedFaelingItem = new("Aequus/Content/DedicatedContent/DedicatedFaelingItem");
        /// <summary>Full Path: Aequus/Content/DedicatedContent/DedicatedFaelingItem_Mask</summary>
        public static readonly TextureAsset DedicatedFaelingItem_Mask = new("Aequus/Content/DedicatedContent/DedicatedFaelingItem_Mask");
        /// <summary>Full Path: Aequus/Content/Dyes/Items/DiscoDye</summary>
        public static readonly TextureAsset DiscoDye = new("Aequus/Content/Dyes/Items/DiscoDye");
        /// <summary>Full Path: Aequus/Content/Bosses/TreasureBags/DustDevilBag</summary>
        public static readonly TextureAsset DustDevilBag = new("Aequus/Content/Bosses/TreasureBags/DustDevilBag");
        /// <summary>Full Path: Aequus/Content/Bosses/BossMasks/DustDevilMask</summary>
        public static readonly TextureAsset DustDevilMask = new("Aequus/Content/Bosses/BossMasks/DustDevilMask");
        /// <summary>Full Path: Aequus/Content/Bosses/BossMasks/DustDevilMaskFire_Head</summary>
        public static readonly TextureAsset DustDevilMaskFire_Head = new("Aequus/Content/Bosses/BossMasks/DustDevilMaskFire_Head");
        /// <summary>Full Path: Aequus/Content/Bosses/BossMasks/DustDevilMaskIce_Head</summary>
        public static readonly TextureAsset DustDevilMaskIce_Head = new("Aequus/Content/Bosses/BossMasks/DustDevilMaskIce_Head");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/DustDevilRelic</summary>
        public static readonly TextureAsset DustDevilRelic = new("Aequus/Content/Bosses/Trophies/DustDevilRelic");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/DustDevilRelicItem</summary>
        public static readonly TextureAsset DustDevilRelicItem = new("Aequus/Content/Bosses/Trophies/DustDevilRelicItem");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/DustDevilTrophy</summary>
        public static readonly TextureAsset DustDevilTrophy = new("Aequus/Content/Bosses/Trophies/DustDevilTrophy");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/DustDevilTrophyItem</summary>
        public static readonly TextureAsset DustDevilTrophyItem = new("Aequus/Content/Bosses/Trophies/DustDevilTrophyItem");
        /// <summary>Full Path: Aequus/Content/Weapons/Melee/DynaKnife/Dynaknife</summary>
        public static readonly TextureAsset Dynaknife = new("Aequus/Content/Weapons/Melee/DynaKnife/Dynaknife");
        /// <summary>Full Path: Aequus/Content/Weapons/Melee/DynaKnife/Dynaknife_Glow</summary>
        public static readonly TextureAsset Dynaknife_Glow = new("Aequus/Content/Weapons/Melee/DynaKnife/Dynaknife_Glow");
        /// <summary>Full Path: Aequus/Content/Weapons/Melee/DynaKnife/DynaknifeProj</summary>
        public static readonly TextureAsset DynaknifeProj = new("Aequus/Content/Weapons/Melee/DynaKnife/DynaknifeProj");
        /// <summary>Full Path: Aequus/Content/Weapons/Melee/DynaKnife/DynaknifeProj_Glow</summary>
        public static readonly TextureAsset DynaknifeProj_Glow = new("Aequus/Content/Weapons/Melee/DynaKnife/DynaknifeProj_Glow");
        /// <summary>Full Path: Aequus/Assets/Textures/EffectNoise</summary>
        public static readonly TextureAsset EffectNoise = new("Aequus/Assets/Textures/EffectNoise");
        /// <summary>Full Path: Aequus/Assets/Textures/EffectPerlin</summary>
        public static readonly TextureAsset EffectPerlin = new("Aequus/Assets/Textures/EffectPerlin");
        /// <summary>Full Path: Aequus/Assets/Textures/EffectWaterRefraction</summary>
        public static readonly TextureAsset EffectWaterRefraction = new("Aequus/Assets/Textures/EffectWaterRefraction");
        /// <summary>Full Path: Aequus/Content/Dyes/Items/EnchantedDye</summary>
        public static readonly TextureAsset EnchantedDye = new("Aequus/Content/Dyes/Items/EnchantedDye");
        /// <summary>Full Path: Aequus/Content/Dyes/EnchantedDyeEffect</summary>
        public static readonly TextureAsset EnchantedDyeEffect = new("Aequus/Content/Dyes/EnchantedDyeEffect");
        /// <summary>Full Path: Aequus/Assets/Textures/Exclamation</summary>
        public static readonly TextureAsset Exclamation = new("Aequus/Assets/Textures/Exclamation");
        /// <summary>Full Path: Aequus/Content/DedicatedContent/Familiar/FamiliarPetBuff</summary>
        public static readonly TextureAsset FamiliarPetBuff = new("Aequus/Content/DedicatedContent/Familiar/FamiliarPetBuff");
        /// <summary>Full Path: Aequus/Content/DedicatedContent/Familiar/FamiliarPetItem</summary>
        public static readonly TextureAsset FamiliarPetItem = new("Aequus/Content/DedicatedContent/Familiar/FamiliarPetItem");
        /// <summary>Full Path: Aequus/Assets/Textures/Flare</summary>
        public static readonly TextureAsset Flare = new("Aequus/Assets/Textures/Flare");
        /// <summary>Full Path: Aequus/Assets/Textures/Flare2</summary>
        public static readonly TextureAsset Flare2 = new("Aequus/Assets/Textures/Flare2");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/FlashwayShield/FlashwayShield</summary>
        public static readonly TextureAsset FlashwayShield = new("Aequus/Content/Equipment/Accessories/FlashwayShield/FlashwayShield");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/FlashwayShield/FlashwayShield_Shield</summary>
        public static readonly TextureAsset FlashwayShield_Shield = new("Aequus/Content/Equipment/Accessories/FlashwayShield/FlashwayShield_Shield");
        /// <summary>Full Path: Aequus/Assets/Textures/Fog</summary>
        public static readonly TextureAsset Fog = new("Aequus/Assets/Textures/Fog");
        /// <summary>Full Path: Aequus/Content/Dyes/Items/FrostbiteDye</summary>
        public static readonly TextureAsset FrostbiteDye = new("Aequus/Content/Dyes/Items/FrostbiteDye");
        /// <summary>Full Path: Aequus/Content/Dyes/FrostbiteDyeEffect</summary>
        public static readonly TextureAsset FrostbiteDyeEffect = new("Aequus/Content/Dyes/FrostbiteDyeEffect");
        /// <summary>Full Path: Aequus/Content/Weapons/Magic/Furystar/Furystar</summary>
        public static readonly TextureAsset Furystar = new("Aequus/Content/Weapons/Magic/Furystar/Furystar");
        /// <summary>Full Path: Aequus/Assets/Textures/GenericExplosion</summary>
        public static readonly TextureAsset GenericExplosion = new("Aequus/Assets/Textures/GenericExplosion");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/GoldenFeather/GoldenFeather</summary>
        public static readonly TextureAsset GoldenFeather = new("Aequus/Content/Equipment/Accessories/GoldenFeather/GoldenFeather");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/GoldenFeather/GoldenFeatherBuff</summary>
        public static readonly TextureAsset GoldenFeatherBuff = new("Aequus/Content/Equipment/Accessories/GoldenFeather/GoldenFeatherBuff");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/GoldenFeather/GoldenWind</summary>
        public static readonly TextureAsset GoldenWind = new("Aequus/Content/Equipment/Accessories/GoldenFeather/GoldenWind");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/GoldenFeather/GoldenWindBuff</summary>
        public static readonly TextureAsset GoldenWindBuff = new("Aequus/Content/Equipment/Accessories/GoldenFeather/GoldenWindBuff");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/GrandReward/GrandReward</summary>
        public static readonly TextureAsset GrandReward = new("Aequus/Content/Equipment/Accessories/GrandReward/GrandReward");
        /// <summary>Full Path: Aequus/Content/Items/Potions/Healing/Restoration/GreaterRestorationPotion</summary>
        public static readonly TextureAsset GreaterRestorationPotion = new("Aequus/Content/Items/Potions/Healing/Restoration/GreaterRestorationPotion");
        /// <summary>Full Path: Aequus/Content/Equipment/Mounts/HotAirBalloon/HotAirBalloonBuff</summary>
        public static readonly TextureAsset HotAirBalloonBuff = new("Aequus/Content/Equipment/Mounts/HotAirBalloon/HotAirBalloonBuff");
        /// <summary>Full Path: Aequus/Content/Equipment/Mounts/HotAirBalloon/HotAirBalloonMount</summary>
        public static readonly TextureAsset HotAirBalloonMount = new("Aequus/Content/Equipment/Mounts/HotAirBalloon/HotAirBalloonMount");
        /// <summary>Full Path: Aequus/Content/Equipment/Mounts/HotAirBalloon/HotAirBalloonMount_Back</summary>
        public static readonly TextureAsset HotAirBalloonMount_Back = new("Aequus/Content/Equipment/Mounts/HotAirBalloon/HotAirBalloonMount_Back");
        /// <summary>Full Path: Aequus/Content/Equipment/Mounts/HotAirBalloon/HotAirBalloonMount_Front</summary>
        public static readonly TextureAsset HotAirBalloonMount_Front = new("Aequus/Content/Equipment/Mounts/HotAirBalloon/HotAirBalloonMount_Front");
        /// <summary>Full Path: Aequus/Content/Equipment/Mounts/HotAirBalloon/HotAirBalloonMount_Glow</summary>
        public static readonly TextureAsset HotAirBalloonMount_Glow = new("Aequus/Content/Equipment/Mounts/HotAirBalloon/HotAirBalloonMount_Glow");
        /// <summary>Full Path: Aequus/Content/Dyes/Items/HueshiftDye</summary>
        public static readonly TextureAsset HueshiftDye = new("Aequus/Content/Dyes/Items/HueshiftDye");
        /// <summary>Full Path: Aequus/icon</summary>
        public static readonly TextureAsset icon = new("Aequus/icon");
        /// <summary>Full Path: Aequus/icon_small</summary>
        public static readonly TextureAsset icon_small = new("Aequus/icon_small");
        /// <summary>Full Path: Aequus/icon_workshop</summary>
        public static readonly TextureAsset icon_workshop = new("Aequus/icon_workshop");
        /// <summary>Full Path: Aequus/Content/Weapons/Ranged/Misc/JunkJet/JunkJet</summary>
        public static readonly TextureAsset JunkJet = new("Aequus/Content/Weapons/Ranged/Misc/JunkJet/JunkJet");
        /// <summary>Full Path: Aequus/Content/DedicatedContent/LanternCat/LanternCatPet</summary>
        public static readonly TextureAsset LanternCatPet = new("Aequus/Content/DedicatedContent/LanternCat/LanternCatPet");
        /// <summary>Full Path: Aequus/Content/DedicatedContent/LanternCat/LanternCatPet_Lantern</summary>
        public static readonly TextureAsset LanternCatPet_Lantern = new("Aequus/Content/DedicatedContent/LanternCat/LanternCatPet_Lantern");
        /// <summary>Full Path: Aequus/Content/DedicatedContent/LanternCat/LanternCatPetBuff</summary>
        public static readonly TextureAsset LanternCatPetBuff = new("Aequus/Content/DedicatedContent/LanternCat/LanternCatPetBuff");
        /// <summary>Full Path: Aequus/Content/DedicatedContent/LanternCat/LanternCatPetItem</summary>
        public static readonly TextureAsset LanternCatPetItem = new("Aequus/Content/DedicatedContent/LanternCat/LanternCatPetItem");
        /// <summary>Full Path: Aequus/Assets/Textures/LightRayCircular</summary>
        public static readonly TextureAsset LightRayCircular = new("Aequus/Assets/Textures/LightRayCircular");
        /// <summary>Full Path: Aequus/Assets/Textures/LightRayFlat</summary>
        public static readonly TextureAsset LightRayFlat = new("Aequus/Assets/Textures/LightRayFlat");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/Informational/Monocle/MonocleBuilderToggle</summary>
        public static readonly TextureAsset MonocleBuilderToggle = new("Aequus/Content/Equipment/Accessories/Informational/Monocle/MonocleBuilderToggle");
        /// <summary>Full Path: Aequus/Assets/Textures/Movement</summary>
        public static readonly TextureAsset Movement = new("Aequus/Assets/Textures/Movement");
        /// <summary>Full Path: Aequus/Content/Items/Tools/NameTag/NameTag</summary>
        public static readonly TextureAsset NameTag = new("Aequus/Content/Items/Tools/NameTag/NameTag");
        /// <summary>Full Path: Aequus/Content/Items/Tools/NameTag/NameTagBlank</summary>
        public static readonly TextureAsset NameTagBlank = new("Aequus/Content/Items/Tools/NameTag/NameTagBlank");
        /// <summary>Full Path: Aequus/Content/UI/Map/NameTagBlip</summary>
        public static readonly TextureAsset NameTagBlip = new("Aequus/Content/UI/Map/NameTagBlip");
        /// <summary>Full Path: Aequus/Content/Items/PermaPowerups/NetherStar/NetherStar</summary>
        public static readonly TextureAsset NetherStar = new("Aequus/Content/Items/PermaPowerups/NetherStar/NetherStar");
        /// <summary>Full Path: Aequus/Content/Items/PermaPowerups/NetherStar/NetherStar_Glow</summary>
        public static readonly TextureAsset NetherStar_Glow = new("Aequus/Content/Items/PermaPowerups/NetherStar/NetherStar_Glow");
        /// <summary>Full Path: Aequus/Content/Items/PermaPowerups/NetherStar/NetherStarBackground</summary>
        public static readonly TextureAsset NetherStarBackground = new("Aequus/Content/Items/PermaPowerups/NetherStar/NetherStarBackground");
        /// <summary>Full Path: Aequus/Content/Items/PermaPowerups/NetherStar/NetherStarMask</summary>
        public static readonly TextureAsset NetherStarMask = new("Aequus/Content/Items/PermaPowerups/NetherStar/NetherStarMask");
        /// <summary>Full Path: Aequus/Content/Items/Potions/Buffs/NeutronYogurt/NeutronYogurt</summary>
        public static readonly TextureAsset NeutronYogurt = new("Aequus/Content/Items/Potions/Buffs/NeutronYogurt/NeutronYogurt");
        /// <summary>Full Path: Aequus/Content/Items/Potions/Buffs/NeutronYogurt/NeutronYogurtBuff</summary>
        public static readonly TextureAsset NeutronYogurtBuff = new("Aequus/Content/Items/Potions/Buffs/NeutronYogurt/NeutronYogurtBuff");
        /// <summary>Full Path: Aequus/Content/Fishing/CrabPots/ObsidianCrabPot</summary>
        public static readonly TextureAsset ObsidianCrabPot = new("Aequus/Content/Fishing/CrabPots/ObsidianCrabPot");
        /// <summary>Full Path: Aequus/Content/Fishing/CrabPots/ObsidianCrabPot_Back</summary>
        public static readonly TextureAsset ObsidianCrabPot_Back = new("Aequus/Content/Fishing/CrabPots/ObsidianCrabPot_Back");
        /// <summary>Full Path: Aequus/Content/Fishing/CrabPots/ObsidianCrabPot_Highlight</summary>
        public static readonly TextureAsset ObsidianCrabPot_Highlight = new("Aequus/Content/Fishing/CrabPots/ObsidianCrabPot_Highlight");
        /// <summary>Full Path: Aequus/Content/Fishing/CrabPots/Items/ObsidianCrabPotItem</summary>
        public static readonly TextureAsset ObsidianCrabPotItem = new("Aequus/Content/Fishing/CrabPots/Items/ObsidianCrabPotItem");
        /// <summary>Full Path: Aequus/Content/Tiles/Banners/OldMonsterBanners</summary>
        public static readonly TextureAsset OldMonsterBanners = new("Aequus/Content/Tiles/Banners/OldMonsterBanners");
        /// <summary>Full Path: Aequus/Content/Bosses/TreasureBags/OmegaStariteBag</summary>
        public static readonly TextureAsset OmegaStariteBag = new("Aequus/Content/Bosses/TreasureBags/OmegaStariteBag");
        /// <summary>Full Path: Aequus/Content/Bosses/BossMasks/OmegaStariteMask</summary>
        public static readonly TextureAsset OmegaStariteMask = new("Aequus/Content/Bosses/BossMasks/OmegaStariteMask");
        /// <summary>Full Path: Aequus/Content/Bosses/BossMasks/OmegaStariteMask_Head</summary>
        public static readonly TextureAsset OmegaStariteMask_Head = new("Aequus/Content/Bosses/BossMasks/OmegaStariteMask_Head");
        /// <summary>Full Path: Aequus/Content/Pets/OmegaStarite/OmegaStaritePet</summary>
        public static readonly TextureAsset OmegaStaritePet = new("Aequus/Content/Pets/OmegaStarite/OmegaStaritePet");
        /// <summary>Full Path: Aequus/Content/Pets/OmegaStarite/OmegaStaritePetBuff</summary>
        public static readonly TextureAsset OmegaStaritePetBuff = new("Aequus/Content/Pets/OmegaStarite/OmegaStaritePetBuff");
        /// <summary>Full Path: Aequus/Content/Pets/OmegaStarite/OmegaStaritePetItem</summary>
        public static readonly TextureAsset OmegaStaritePetItem = new("Aequus/Content/Pets/OmegaStarite/OmegaStaritePetItem");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/OmegaStariteRelic</summary>
        public static readonly TextureAsset OmegaStariteRelic = new("Aequus/Content/Bosses/Trophies/OmegaStariteRelic");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/OmegaStariteRelicItem</summary>
        public static readonly TextureAsset OmegaStariteRelicItem = new("Aequus/Content/Bosses/Trophies/OmegaStariteRelicItem");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/OmegaStariteTrophy</summary>
        public static readonly TextureAsset OmegaStariteTrophy = new("Aequus/Content/Bosses/Trophies/OmegaStariteTrophy");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/OmegaStariteTrophyItem</summary>
        public static readonly TextureAsset OmegaStariteTrophyItem = new("Aequus/Content/Bosses/Trophies/OmegaStariteTrophyItem");
        /// <summary>Full Path: Aequus/Assets/Textures/OnlineLink</summary>
        public static readonly TextureAsset OnlineLink = new("Aequus/Assets/Textures/OnlineLink");
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
        /// <summary>Full Path: Aequus/Content/Weapons/Ranged/Darts/Ammo/PlasticDart</summary>
        public static readonly TextureAsset PlasticDart = new("Aequus/Content/Weapons/Ranged/Darts/Ammo/PlasticDart");
        /// <summary>Full Path: Aequus/Content/Weapons/Ranged/Darts/Ammo/PlasticDart_Outline</summary>
        public static readonly TextureAsset PlasticDart_Outline = new("Aequus/Content/Weapons/Ranged/Darts/Ammo/PlasticDart_Outline");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/Balloons/PurpleBalloon</summary>
        public static readonly TextureAsset PurpleBalloon = new("Aequus/Content/Equipment/Accessories/Balloons/PurpleBalloon");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/Balloons/PurpleBalloon_Balloon</summary>
        public static readonly TextureAsset PurpleBalloon_Balloon = new("Aequus/Content/Equipment/Accessories/Balloons/PurpleBalloon_Balloon");
        /// <summary>Full Path: Aequus/Content/Bosses/BossMasks/RedSpriteMask</summary>
        public static readonly TextureAsset RedSpriteMask = new("Aequus/Content/Bosses/BossMasks/RedSpriteMask");
        /// <summary>Full Path: Aequus/Content/Bosses/BossMasks/RedSpriteMask_Head</summary>
        public static readonly TextureAsset RedSpriteMask_Head = new("Aequus/Content/Bosses/BossMasks/RedSpriteMask_Head");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/RedSpriteRelic</summary>
        public static readonly TextureAsset RedSpriteRelic = new("Aequus/Content/Bosses/Trophies/RedSpriteRelic");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/RedSpriteRelicItem</summary>
        public static readonly TextureAsset RedSpriteRelicItem = new("Aequus/Content/Bosses/Trophies/RedSpriteRelicItem");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/RedSpriteTrophy</summary>
        public static readonly TextureAsset RedSpriteTrophy = new("Aequus/Content/Bosses/Trophies/RedSpriteTrophy");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/RedSpriteTrophyItem</summary>
        public static readonly TextureAsset RedSpriteTrophyItem = new("Aequus/Content/Bosses/Trophies/RedSpriteTrophyItem");
        /// <summary>Full Path: Aequus/Content/TownNPCs/SkyMerchant/UI/RenameBackIcon</summary>
        public static readonly TextureAsset RenameBackIcon = new("Aequus/Content/TownNPCs/SkyMerchant/UI/RenameBackIcon");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/Informational/Monocle/RichMansMonocle</summary>
        public static readonly TextureAsset RichMansMonocle = new("Aequus/Content/Equipment/Accessories/Informational/Monocle/RichMansMonocle");
        /// <summary>Full Path: Aequus/Content/Enemies/PollutedOcean/Scavenger/Scavenger</summary>
        public static readonly TextureAsset Scavenger = new("Aequus/Content/Enemies/PollutedOcean/Scavenger/Scavenger");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/ScavengerBag/ScavengerBag</summary>
        public static readonly TextureAsset ScavengerBag = new("Aequus/Content/Equipment/Accessories/ScavengerBag/ScavengerBag");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/ScavengerBag/ScavengerBag_Back</summary>
        public static readonly TextureAsset ScavengerBag_Back = new("Aequus/Content/Equipment/Accessories/ScavengerBag/ScavengerBag_Back");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/ScavengerBag/ScavengerBag_Strap</summary>
        public static readonly TextureAsset ScavengerBag_Strap = new("Aequus/Content/Equipment/Accessories/ScavengerBag/ScavengerBag_Strap");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/ScavengerBag/ScavengerBagBuilderToggle</summary>
        public static readonly TextureAsset ScavengerBagBuilderToggle = new("Aequus/Content/Equipment/Accessories/ScavengerBag/ScavengerBagBuilderToggle");
        /// <summary>Full Path: Aequus/Content/Enemies/PollutedOcean/Scavenger/ScavengerBody</summary>
        public static readonly TextureAsset ScavengerBody = new("Aequus/Content/Enemies/PollutedOcean/Scavenger/ScavengerBody");
        /// <summary>Full Path: Aequus/Content/Enemies/PollutedOcean/Scavenger/Gores/ScavengerGoreHead</summary>
        public static readonly TextureAsset ScavengerGoreHead = new("Aequus/Content/Enemies/PollutedOcean/Scavenger/Gores/ScavengerGoreHead");
        /// <summary>Full Path: Aequus/Content/Enemies/PollutedOcean/Scavenger/ScavengerHead</summary>
        public static readonly TextureAsset ScavengerHead = new("Aequus/Content/Enemies/PollutedOcean/Scavenger/ScavengerHead");
        /// <summary>Full Path: Aequus/Content/Enemies/PollutedOcean/Scavenger/ScavengerLootBag</summary>
        public static readonly TextureAsset ScavengerLootBag = new("Aequus/Content/Enemies/PollutedOcean/Scavenger/ScavengerLootBag");
        /// <summary>Full Path: Aequus/Content/Enemies/PollutedOcean/Scavenger/ScavengerLootBag_Outline</summary>
        public static readonly TextureAsset ScavengerLootBag_Outline = new("Aequus/Content/Enemies/PollutedOcean/Scavenger/ScavengerLootBag_Outline");
        /// <summary>Full Path: Aequus/Content/Dyes/Items/ScorchingDye</summary>
        public static readonly TextureAsset ScorchingDye = new("Aequus/Content/Dyes/Items/ScorchingDye");
        /// <summary>Full Path: Aequus/Assets/Textures/Shatter</summary>
        public static readonly TextureAsset Shatter = new("Aequus/Assets/Textures/Shatter");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/Informational/Monocle/ShimmerMonocle</summary>
        public static readonly TextureAsset ShimmerMonocle = new("Aequus/Content/Equipment/Accessories/Informational/Monocle/ShimmerMonocle");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/Informational/Monocle/ShimmerMonocleBuilderToggle</summary>
        public static readonly TextureAsset ShimmerMonocleBuilderToggle = new("Aequus/Content/Equipment/Accessories/Informational/Monocle/ShimmerMonocleBuilderToggle");
        /// <summary>Full Path: Aequus/Content/Weapons/Ranged/Bows/SkyHunterCrossbow/SkyHunterCrossbow</summary>
        public static readonly TextureAsset SkyHunterCrossbow = new("Aequus/Content/Weapons/Ranged/Bows/SkyHunterCrossbow/SkyHunterCrossbow");
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
        /// <summary>Full Path: Aequus/Content/Weapons/Melee/Slice/Slice</summary>
        public static readonly TextureAsset Slice = new("Aequus/Content/Weapons/Melee/Slice/Slice");
        /// <summary>Full Path: Aequus/Content/Weapons/Melee/Slice/SliceBulletProj</summary>
        public static readonly TextureAsset SliceBulletProj = new("Aequus/Content/Weapons/Melee/Slice/SliceBulletProj");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/Balloons/SlimyBlueBalloon</summary>
        public static readonly TextureAsset SlimyBlueBalloon = new("Aequus/Content/Equipment/Accessories/Balloons/SlimyBlueBalloon");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/Balloons/SlimyBlueBalloon_Balloon</summary>
        public static readonly TextureAsset SlimyBlueBalloon_Balloon = new("Aequus/Content/Equipment/Accessories/Balloons/SlimyBlueBalloon_Balloon");
        /// <summary>Full Path: Aequus/Content/Bosses/BossMasks/SpaceSquidMask</summary>
        public static readonly TextureAsset SpaceSquidMask = new("Aequus/Content/Bosses/BossMasks/SpaceSquidMask");
        /// <summary>Full Path: Aequus/Content/Bosses/BossMasks/SpaceSquidMask_Head</summary>
        public static readonly TextureAsset SpaceSquidMask_Head = new("Aequus/Content/Bosses/BossMasks/SpaceSquidMask_Head");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/SpaceSquidRelic</summary>
        public static readonly TextureAsset SpaceSquidRelic = new("Aequus/Content/Bosses/Trophies/SpaceSquidRelic");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/SpaceSquidRelicItem</summary>
        public static readonly TextureAsset SpaceSquidRelicItem = new("Aequus/Content/Bosses/Trophies/SpaceSquidRelicItem");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/SpaceSquidTrophy</summary>
        public static readonly TextureAsset SpaceSquidTrophy = new("Aequus/Content/Bosses/Trophies/SpaceSquidTrophy");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/SpaceSquidTrophyItem</summary>
        public static readonly TextureAsset SpaceSquidTrophyItem = new("Aequus/Content/Bosses/Trophies/SpaceSquidTrophyItem");
        /// <summary>Full Path: Aequus/Assets/Textures/Sparkles</summary>
        public static readonly TextureAsset Sparkles = new("Aequus/Assets/Textures/Sparkles");
        /// <summary>Full Path: Aequus/Content/Weapons/Ranged/Darts/StarPhish/StarPhish</summary>
        public static readonly TextureAsset StarPhish = new("Aequus/Content/Weapons/Ranged/Darts/StarPhish/StarPhish");
        /// <summary>Full Path: Aequus/Content/Fishing/FishingPoles/Steampunker/SteampunkerFishingPole</summary>
        public static readonly TextureAsset SteampunkerFishingPole = new("Aequus/Content/Fishing/FishingPoles/Steampunker/SteampunkerFishingPole");
        /// <summary>Full Path: Aequus/Content/Fishing/FishingPoles/Steampunker/SteampunkerFishingPoleBobber</summary>
        public static readonly TextureAsset SteampunkerFishingPoleBobber = new("Aequus/Content/Fishing/FishingPoles/Steampunker/SteampunkerFishingPoleBobber");
        /// <summary>Full Path: Aequus/Content/Weapons/Classless/StunGun/StunEffect</summary>
        public static readonly TextureAsset StunEffect = new("Aequus/Content/Weapons/Classless/StunGun/StunEffect");
        /// <summary>Full Path: Aequus/Content/Weapons/Classless/StunGun/StunGun</summary>
        public static readonly TextureAsset StunGun = new("Aequus/Content/Weapons/Classless/StunGun/StunGun");
        /// <summary>Full Path: Aequus/Content/Items/Potions/Healing/Restoration/SuperRestorationPotion</summary>
        public static readonly TextureAsset SuperRestorationPotion = new("Aequus/Content/Items/Potions/Healing/Restoration/SuperRestorationPotion");
        /// <summary>Full Path: Aequus/Content/DedicatedContent/SwagEye/SwagEyePet</summary>
        public static readonly TextureAsset SwagEyePet = new("Aequus/Content/DedicatedContent/SwagEye/SwagEyePet");
        /// <summary>Full Path: Aequus/Content/DedicatedContent/SwagEye/SwagEyePetBuff</summary>
        public static readonly TextureAsset SwagEyePetBuff = new("Aequus/Content/DedicatedContent/SwagEye/SwagEyePetBuff");
        /// <summary>Full Path: Aequus/Content/DedicatedContent/SwagEye/SwagEyePetItem</summary>
        public static readonly TextureAsset SwagEyePetItem = new("Aequus/Content/DedicatedContent/SwagEye/SwagEyePetItem");
        /// <summary>Full Path: Aequus/Content/VanillaChanges/ToolbeltBuilderToggle</summary>
        public static readonly TextureAsset ToolbeltBuilderToggle = new("Aequus/Content/VanillaChanges/ToolbeltBuilderToggle");
        /// <summary>Full Path: Aequus/Assets/Textures/TownNPCExclamation</summary>
        public static readonly TextureAsset TownNPCExclamation = new("Aequus/Assets/Textures/TownNPCExclamation");
        /// <summary>Full Path: Aequus/Assets/Textures/Trail</summary>
        public static readonly TextureAsset Trail = new("Aequus/Assets/Textures/Trail");
        /// <summary>Full Path: Aequus/Assets/Textures/Trail2</summary>
        public static readonly TextureAsset Trail2 = new("Aequus/Assets/Textures/Trail2");
        /// <summary>Full Path: Aequus/Assets/Textures/Trail3</summary>
        public static readonly TextureAsset Trail3 = new("Aequus/Assets/Textures/Trail3");
        /// <summary>Full Path: Aequus/Content/Tiles/CraftingStations/TrashCompactor/TrashCompactor</summary>
        public static readonly TextureAsset TrashCompactor = new("Aequus/Content/Tiles/CraftingStations/TrashCompactor/TrashCompactor");
        /// <summary>Full Path: Aequus/Content/Tiles/CraftingStations/TrashCompactor/TrashCompactorItem</summary>
        public static readonly TextureAsset TrashCompactorItem = new("Aequus/Content/Tiles/CraftingStations/TrashCompactor/TrashCompactorItem");
        /// <summary>Full Path: Aequus/Content/Weapons/Magic/TrashStaff/TrashStaff</summary>
        public static readonly TextureAsset TrashStaff = new("Aequus/Content/Weapons/Magic/TrashStaff/TrashStaff");
        /// <summary>Full Path: Aequus/Content/Weapons/Magic/TrashStaff/TrashStaffCritEffect</summary>
        public static readonly TextureAsset TrashStaffCritEffect = new("Aequus/Content/Weapons/Magic/TrashStaff/TrashStaffCritEffect");
        /// <summary>Full Path: Aequus/Content/Weapons/Magic/TrashStaff/TrashStaffCritEffectStrip</summary>
        public static readonly TextureAsset TrashStaffCritEffectStrip = new("Aequus/Content/Weapons/Magic/TrashStaff/TrashStaffCritEffectStrip");
        /// <summary>Full Path: Aequus/Content/Weapons/Magic/TrashStaff/TrashStaffProj</summary>
        public static readonly TextureAsset TrashStaffProj = new("Aequus/Content/Weapons/Magic/TrashStaff/TrashStaffProj");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/Trophy</summary>
        public static readonly TextureAsset Trophy = new("Aequus/Content/Bosses/Trophies/Trophy");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/UltraStariteRelic</summary>
        public static readonly TextureAsset UltraStariteRelic = new("Aequus/Content/Bosses/Trophies/UltraStariteRelic");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/UltraStariteRelicItem</summary>
        public static readonly TextureAsset UltraStariteRelicItem = new("Aequus/Content/Bosses/Trophies/UltraStariteRelicItem");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/UltraStariteTrophy</summary>
        public static readonly TextureAsset UltraStariteTrophy = new("Aequus/Content/Bosses/Trophies/UltraStariteTrophy");
        /// <summary>Full Path: Aequus/Content/Bosses/Trophies/UltraStariteTrophyItem</summary>
        public static readonly TextureAsset UltraStariteTrophyItem = new("Aequus/Content/Bosses/Trophies/UltraStariteTrophyItem");
        /// <summary>Full Path: Aequus/Content/Pets/Miner/UndeadMinerPet</summary>
        public static readonly TextureAsset UndeadMinerPet = new("Aequus/Content/Pets/Miner/UndeadMinerPet");
        /// <summary>Full Path: Aequus/Content/Pets/Miner/UndeadMinerPetBuff</summary>
        public static readonly TextureAsset UndeadMinerPetBuff = new("Aequus/Content/Pets/Miner/UndeadMinerPetBuff");
        /// <summary>Full Path: Aequus/Content/Pets/Miner/UndeadMinerPetItem</summary>
        public static readonly TextureAsset UndeadMinerPetItem = new("Aequus/Content/Pets/Miner/UndeadMinerPetItem");
        /// <summary>Full Path: Aequus/Assets/Textures/VignetteSmall</summary>
        public static readonly TextureAsset VignetteSmall = new("Aequus/Assets/Textures/VignetteSmall");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/WeightedHorseshoe/WeightedHorseshoe</summary>
        public static readonly TextureAsset WeightedHorseshoe = new("Aequus/Content/Equipment/Accessories/WeightedHorseshoe/WeightedHorseshoe");
        /// <summary>Full Path: Aequus/Content/Equipment/Accessories/WeightedHorseshoe/WeightedHorseshoeVisual</summary>
        public static readonly TextureAsset WeightedHorseshoeVisual = new("Aequus/Content/Equipment/Accessories/WeightedHorseshoe/WeightedHorseshoeVisual");
        /// <summary>Full Path: Aequus/Content/Items/Tools/AnglerLamp/WispLantern</summary>
        public static readonly TextureAsset WispLantern = new("Aequus/Content/Items/Tools/AnglerLamp/WispLantern");
        /// <summary>Full Path: Aequus/Content/Items/Tools/AnglerLamp/WispLantern_Glow</summary>
        public static readonly TextureAsset WispLantern_Glow = new("Aequus/Content/Items/Tools/AnglerLamp/WispLantern_Glow");
    }
}