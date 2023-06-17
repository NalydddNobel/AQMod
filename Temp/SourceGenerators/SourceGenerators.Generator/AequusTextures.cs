using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Aequus.Common;

namespace Aequus {
    /// <summary>(Amt Textures: 1210)</summary>
    [CompilerGenerated]
    public partial class AequusTextures : ILoadable {
        public void Load(Mod mod) {
        }

        public void Unload() {
            foreach (var f in GetType().GetFields()) {
                ((TextureAsset)f.GetValue(this))?.Unload();
            }
        }

        /// <summary>Full Path: Aequus/Tiles/Misc/Plants/AcornTile</summary>
        public static readonly TextureAsset AcornTile = new("Aequus/Tiles/Misc/Plants/AcornTile");
        /// <summary>Full Path: Aequus/NPCs/Town/CarpenterNPC/Quest/Bounties/Icons/ActuatorDoorBounty</summary>
        public static readonly TextureAsset ActuatorDoorBounty = new("Aequus/NPCs/Town/CarpenterNPC/Quest/Bounties/Icons/ActuatorDoorBounty");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/AdamantiteChest</summary>
        public static readonly TextureAsset AdamantiteChest = new("Aequus/Tiles/Misc/HardmodeChests/AdamantiteChest");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/AdamantiteChestTile</summary>
        public static readonly TextureAsset AdamantiteChestTile = new("Aequus/Tiles/Misc/HardmodeChests/AdamantiteChestTile");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/AdamantiteChestTile_Highlight</summary>
        public static readonly TextureAsset AdamantiteChestTile_Highlight = new("Aequus/Tiles/Misc/HardmodeChests/AdamantiteChestTile_Highlight");
        /// <summary>Full Path: Aequus/NPCs/Monsters/AdamantiteMimic</summary>
        public static readonly TextureAsset AdamantiteMimic = new("Aequus/NPCs/Monsters/AdamantiteMimic");
        /// <summary>Full Path: Aequus/Items/Tools/Building/AdvancedRuler</summary>
        public static readonly TextureAsset AdvancedRuler = new("Aequus/Items/Tools/Building/AdvancedRuler");
        /// <summary>Full Path: Aequus/Items/Armor/SetAetherial/AetherialCrown</summary>
        public static readonly TextureAsset AetherialCrown = new("Aequus/Items/Armor/SetAetherial/AetherialCrown");
        /// <summary>Full Path: Aequus/Items/Armor/SetAetherial/AetherialCrown_Head</summary>
        public static readonly TextureAsset AetherialCrown_Head = new("Aequus/Items/Armor/SetAetherial/AetherialCrown_Head");
        /// <summary>Full Path: Aequus/Buffs/Debuffs/AethersWrath</summary>
        public static readonly TextureAsset AethersWrath = new("Aequus/Buffs/Debuffs/AethersWrath");
        /// <summary>Full Path: Aequus/Tiles/Misc/BigGems/AmethystDeposit</summary>
        public static readonly TextureAsset AmethystDeposit = new("Aequus/Tiles/Misc/BigGems/AmethystDeposit");
        /// <summary>Full Path: Aequus/Assets/Gores/Gems/AmethystGore</summary>
        public static readonly TextureAsset AmethystGore = new("Aequus/Assets/Gores/Gems/AmethystGore");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Ranged/AmmoBackpack</summary>
        public static readonly TextureAsset AmmoBackpack = new("Aequus/Items/Accessories/Combat/Ranged/AmmoBackpack");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Ranged/AmmoBackpack_Back</summary>
        public static readonly TextureAsset AmmoBackpack_Back = new("Aequus/Items/Accessories/Combat/Ranged/AmmoBackpack_Back");
        /// <summary>Full Path: Aequus/Tiles/Blocks/GravityBlocks/Ancient/AncientAntiGravityBlock</summary>
        public static readonly TextureAsset AncientAntiGravityBlock = new("Aequus/Tiles/Blocks/GravityBlocks/Ancient/AncientAntiGravityBlock");
        /// <summary>Full Path: Aequus/Tiles/Blocks/GravityBlocks/Ancient/AncientAntiGravityBlockTile</summary>
        public static readonly TextureAsset AncientAntiGravityBlockTile = new("Aequus/Tiles/Blocks/GravityBlocks/Ancient/AncientAntiGravityBlockTile");
        /// <summary>Full Path: Aequus/Items/Misc/Dyes/Ancient/AncientBreakdownDye</summary>
        public static readonly TextureAsset AncientBreakdownDye = new("Aequus/Items/Misc/Dyes/Ancient/AncientBreakdownDye");
        /// <summary>Full Path: Aequus/Items/Misc/Dyes/Ancient/AncientFrostbiteDye</summary>
        public static readonly TextureAsset AncientFrostbiteDye = new("Aequus/Items/Misc/Dyes/Ancient/AncientFrostbiteDye");
        /// <summary>Full Path: Aequus/Tiles/Blocks/GravityBlocks/Ancient/AncientGravityBlock</summary>
        public static readonly TextureAsset AncientGravityBlock = new("Aequus/Tiles/Blocks/GravityBlocks/Ancient/AncientGravityBlock");
        /// <summary>Full Path: Aequus/Tiles/Blocks/GravityBlocks/Ancient/AncientGravityBlockTile</summary>
        public static readonly TextureAsset AncientGravityBlockTile = new("Aequus/Tiles/Blocks/GravityBlocks/Ancient/AncientGravityBlockTile");
        /// <summary>Full Path: Aequus/Items/Misc/Dyes/Ancient/AncientHellBeamDye</summary>
        public static readonly TextureAsset AncientHellBeamDye = new("Aequus/Items/Misc/Dyes/Ancient/AncientHellBeamDye");
        /// <summary>Full Path: Aequus/Items/Misc/Dyes/AncientHueshiftDye</summary>
        public static readonly TextureAsset AncientHueshiftDye = new("Aequus/Items/Misc/Dyes/AncientHueshiftDye");
        /// <summary>Full Path: Aequus/Items/Misc/Dyes/Ancient/AncientScorchingDye</summary>
        public static readonly TextureAsset AncientScorchingDye = new("Aequus/Items/Misc/Dyes/Ancient/AncientScorchingDye");
        /// <summary>Full Path: Aequus/Items/Misc/Dyes/Ancient/AncientTidalDye</summary>
        public static readonly TextureAsset AncientTidalDye = new("Aequus/Items/Misc/Dyes/Ancient/AncientTidalDye");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/Info/AnglerBroadcaster</summary>
        public static readonly TextureAsset AnglerBroadcaster = new("Aequus/Items/Accessories/Misc/Info/AnglerBroadcaster");
        /// <summary>Full Path: Aequus/UI/AnglerBroadcasterIcon</summary>
        public static readonly TextureAsset AnglerBroadcasterIcon = new("Aequus/UI/AnglerBroadcasterIcon");
        /// <summary>Full Path: Aequus/Tiles/Blocks/GravityBlocks/Auras/AntiGravityAura_0</summary>
        public static readonly TextureAsset AntiGravityAura_0 = new("Aequus/Tiles/Blocks/GravityBlocks/Auras/AntiGravityAura_0");
        /// <summary>Full Path: Aequus/Tiles/Blocks/GravityBlocks/Auras/AntiGravityAura_1</summary>
        public static readonly TextureAsset AntiGravityAura_1 = new("Aequus/Tiles/Blocks/GravityBlocks/Auras/AntiGravityAura_1");
        /// <summary>Full Path: Aequus/Tiles/Blocks/GravityBlocks/AntiGravityBlock</summary>
        public static readonly TextureAsset AntiGravityBlock = new("Aequus/Tiles/Blocks/GravityBlocks/AntiGravityBlock");
        /// <summary>Full Path: Aequus/Tiles/Blocks/GravityBlocks/AntiGravityBlockTile</summary>
        public static readonly TextureAsset AntiGravityBlockTile = new("Aequus/Tiles/Blocks/GravityBlocks/AntiGravityBlockTile");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Gravity/AntiGravityCandelabra</summary>
        public static readonly TextureAsset AntiGravityCandelabra = new("Aequus/Tiles/Furniture/Gravity/AntiGravityCandelabra");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Gravity/AntiGravityCandle</summary>
        public static readonly TextureAsset AntiGravityCandle = new("Aequus/Tiles/Furniture/Gravity/AntiGravityCandle");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Gravity/AntiGravityChair</summary>
        public static readonly TextureAsset AntiGravityChair = new("Aequus/Tiles/Furniture/Gravity/AntiGravityChair");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Gravity/AntiGravityChestTile</summary>
        public static readonly TextureAsset AntiGravityChestTile = new("Aequus/Tiles/Furniture/Gravity/AntiGravityChestTile");
        /// <summary>Full Path: Aequus/Tiles/Blocks/GravityBlocks/Auras/AntiGravityDust</summary>
        public static readonly TextureAsset AntiGravityDust = new("Aequus/Tiles/Blocks/GravityBlocks/Auras/AntiGravityDust");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Gravity/AntiGravityLamp</summary>
        public static readonly TextureAsset AntiGravityLamp = new("Aequus/Tiles/Furniture/Gravity/AntiGravityLamp");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Gravity/AntiGravityPiano</summary>
        public static readonly TextureAsset AntiGravityPiano = new("Aequus/Tiles/Furniture/Gravity/AntiGravityPiano");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Gravity/AntiGravitySofa</summary>
        public static readonly TextureAsset AntiGravitySofa = new("Aequus/Tiles/Furniture/Gravity/AntiGravitySofa");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Gravity/AntiGravityTable</summary>
        public static readonly TextureAsset AntiGravityTable = new("Aequus/Tiles/Furniture/Gravity/AntiGravityTable");
        /// <summary>Full Path: Aequus/Items/Materials/Energies/AquaticEnergy</summary>
        public static readonly TextureAsset AquaticEnergy = new("Aequus/Items/Materials/Energies/AquaticEnergy");
        /// <summary>Full Path: Aequus/Assets/Effects/Textures/AquaticEnergyGradient</summary>
        public static readonly TextureAsset AquaticEnergyGradient = new("Aequus/Assets/Effects/Textures/AquaticEnergyGradient");
        /// <summary>Full Path: Aequus/Items/Misc/LegendaryFish/ArgonFish</summary>
        public static readonly TextureAsset ArgonFish = new("Aequus/Items/Misc/LegendaryFish/ArgonFish");
        /// <summary>Full Path: Aequus/Items/Accessories/Life/Water/ArmFloaties</summary>
        public static readonly TextureAsset ArmFloaties = new("Aequus/Items/Accessories/Life/Water/ArmFloaties");
        /// <summary>Full Path: Aequus/Items/Materials/PillarFragments/ArtFragment</summary>
        public static readonly TextureAsset ArtFragment = new("Aequus/Items/Materials/PillarFragments/ArtFragment");
        /// <summary>Full Path: Aequus/Tiles/Misc/AshTombstones/AshCrossGraveMarker</summary>
        public static readonly TextureAsset AshCrossGraveMarker = new("Aequus/Tiles/Misc/AshTombstones/AshCrossGraveMarker");
        /// <summary>Full Path: Aequus/Tiles/Misc/AshTombstones/AshGraveMarker</summary>
        public static readonly TextureAsset AshGraveMarker = new("Aequus/Tiles/Misc/AshTombstones/AshGraveMarker");
        /// <summary>Full Path: Aequus/Tiles/Misc/AshTombstones/AshGravestone</summary>
        public static readonly TextureAsset AshGravestone = new("Aequus/Tiles/Misc/AshTombstones/AshGravestone");
        /// <summary>Full Path: Aequus/Tiles/Misc/AshTombstones/AshHeadstone</summary>
        public static readonly TextureAsset AshHeadstone = new("Aequus/Tiles/Misc/AshTombstones/AshHeadstone");
        /// <summary>Full Path: Aequus/Tiles/Misc/AshTombstones/AshObelisk</summary>
        public static readonly TextureAsset AshObelisk = new("Aequus/Tiles/Misc/AshTombstones/AshObelisk");
        /// <summary>Full Path: Aequus/Tiles/Misc/AshTombstones/AshTombstone</summary>
        public static readonly TextureAsset AshTombstone = new("Aequus/Tiles/Misc/AshTombstones/AshTombstone");
        /// <summary>Full Path: Aequus/Tiles/Misc/AshTombstones/AshTombstonesTile</summary>
        public static readonly TextureAsset AshTombstonesTile = new("Aequus/Tiles/Misc/AshTombstones/AshTombstonesTile");
        /// <summary>Full Path: Aequus/Tiles/Misc/AshTombstones/AshTombstonesTile_Glow</summary>
        public static readonly TextureAsset AshTombstonesTile_Glow = new("Aequus/Tiles/Misc/AshTombstones/AshTombstonesTile_Glow");
        /// <summary>Full Path: Aequus/Items/Consumables/Foods/AstralCookie</summary>
        public static readonly TextureAsset AstralCookie = new("Aequus/Items/Consumables/Foods/AstralCookie");
        /// <summary>Full Path: Aequus/Buffs/AstralCookieBuff</summary>
        public static readonly TextureAsset AstralCookieBuff = new("Aequus/Buffs/AstralCookieBuff");
        /// <summary>Full Path: Aequus/Items/Materials/Energies/AtmosphericEnergy</summary>
        public static readonly TextureAsset AtmosphericEnergy = new("Aequus/Items/Materials/Energies/AtmosphericEnergy");
        /// <summary>Full Path: Aequus/Assets/Effects/Textures/AtmosphericEnergyGradient</summary>
        public static readonly TextureAsset AtmosphericEnergyGradient = new("Aequus/Assets/Effects/Textures/AtmosphericEnergyGradient");
        /// <summary>Full Path: Aequus/Items/Consumables/Foods/Baguette</summary>
        public static readonly TextureAsset Baguette = new("Aequus/Items/Consumables/Foods/Baguette");
        /// <summary>Full Path: Aequus/Buffs/BaguetteBuff</summary>
        public static readonly TextureAsset BaguetteBuff = new("Aequus/Buffs/BaguetteBuff");
        /// <summary>Full Path: Aequus/Items/Misc/Mounts/BalloonKit</summary>
        public static readonly TextureAsset BalloonKit = new("Aequus/Items/Misc/Mounts/BalloonKit");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/Thrown/Baozhu</summary>
        public static readonly TextureAsset Baozhu = new("Aequus/Items/Weapons/Ranged/Thrown/Baozhu");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/Thrown/Baozhu_Glow</summary>
        public static readonly TextureAsset Baozhu_Glow = new("Aequus/Items/Weapons/Ranged/Thrown/Baozhu_Glow");
        /// <summary>Full Path: Aequus/Projectiles/Ranged/BaozhuProj</summary>
        public static readonly TextureAsset BaozhuProj = new("Aequus/Projectiles/Ranged/BaozhuProj");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/Misc/BarbedHarpoon</summary>
        public static readonly TextureAsset BarbedHarpoon = new("Aequus/Items/Weapons/Ranged/Misc/BarbedHarpoon");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/BattleAxe/BattleAxe</summary>
        public static readonly TextureAsset BattleAxe = new("Aequus/Items/Weapons/Melee/BattleAxe/BattleAxe");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/BattleAxe/BattleAxeProj</summary>
        public static readonly TextureAsset BattleAxeProj = new("Aequus/Items/Weapons/Melee/BattleAxe/BattleAxeProj");
        /// <summary>Full Path: Aequus/Projectiles/Misc/Bobbers/BeeBobber</summary>
        public static readonly TextureAsset BeeBobber = new("Aequus/Projectiles/Misc/Bobbers/BeeBobber");
        /// <summary>Full Path: Aequus/Tiles/Misc/Plants/BeetrootTile</summary>
        public static readonly TextureAsset BeetrootTile = new("Aequus/Tiles/Misc/Plants/BeetrootTile");
        /// <summary>Full Path: Aequus/Items/Tools/Bellows</summary>
        public static readonly TextureAsset Bellows = new("Aequus/Items/Tools/Bellows");
        /// <summary>Full Path: Aequus/Projectiles/Misc/BellowsProj</summary>
        public static readonly TextureAsset BellowsProj = new("Aequus/Projectiles/Misc/BellowsProj");
        /// <summary>Full Path: Aequus/Assets/UI/BestiaryNotebook</summary>
        public static readonly TextureAsset BestiaryNotebook = new("Aequus/Assets/UI/BestiaryNotebook");
        /// <summary>Full Path: Aequus/Tiles/Misc/BigGems/BigGemsTile</summary>
        public static readonly TextureAsset BigGemsTile = new("Aequus/Tiles/Misc/BigGems/BigGemsTile");
        /// <summary>Full Path: Aequus/NPCs/Town/CarpenterNPC/Quest/Bounties/Icons/BiomePaletteBounty</summary>
        public static readonly TextureAsset BiomePaletteBounty = new("Aequus/NPCs/Town/CarpenterNPC/Quest/Bounties/Icons/BiomePaletteBounty");
        /// <summary>Full Path: Aequus/Buffs/Debuffs/BitCrushedDebuff</summary>
        public static readonly TextureAsset BitCrushedDebuff = new("Aequus/Buffs/Debuffs/BitCrushedDebuff");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/OnHit/Debuff/BlackPhial</summary>
        public static readonly TextureAsset BlackPhial = new("Aequus/Items/Accessories/Combat/OnHit/Debuff/BlackPhial");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/OnHit/Debuff/BlackPhial_Waist</summary>
        public static readonly TextureAsset BlackPhial_Waist = new("Aequus/Items/Accessories/Combat/OnHit/Debuff/BlackPhial_Waist");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/OnHit/Debuff/BlackPlague</summary>
        public static readonly TextureAsset BlackPlague = new("Aequus/Items/Accessories/Combat/OnHit/Debuff/BlackPlague");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/OnHit/Debuff/BlackPlague_HandsOn</summary>
        public static readonly TextureAsset BlackPlague_HandsOn = new("Aequus/Items/Accessories/Combat/OnHit/Debuff/BlackPlague_HandsOn");
        /// <summary>Full Path: Aequus/Items/Misc/LegendaryFish/Blobfish</summary>
        public static readonly TextureAsset Blobfish = new("Aequus/Items/Misc/LegendaryFish/Blobfish");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/Thrown/BlockGlove</summary>
        public static readonly TextureAsset BlockGlove = new("Aequus/Items/Weapons/Ranged/Thrown/BlockGlove");
        /// <summary>Full Path: Aequus/Items/Accessories/Life/BloodCurcleav</summary>
        public static readonly TextureAsset BloodCurcleav = new("Aequus/Items/Accessories/Life/BloodCurcleav");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Necro/BloodiedBucket</summary>
        public static readonly TextureAsset BloodiedBucket = new("Aequus/Items/Accessories/Combat/Necro/BloodiedBucket");
        /// <summary>Full Path: Aequus/NPCs/Monsters/Event/BloodMoon/BloodMimic</summary>
        public static readonly TextureAsset BloodMimic = new("Aequus/NPCs/Monsters/Event/BloodMoon/BloodMimic");
        /// <summary>Full Path: Aequus/Assets/Gores/BloodMimic_0</summary>
        public static readonly TextureAsset BloodMimic_0 = new("Aequus/Assets/Gores/BloodMimic_0");
        /// <summary>Full Path: Aequus/Assets/Gores/BloodMimic_1</summary>
        public static readonly TextureAsset BloodMimic_1 = new("Aequus/Assets/Gores/BloodMimic_1");
        /// <summary>Full Path: Aequus/Tiles/Banners/Items/BloodMimicBanner</summary>
        public static readonly TextureAsset BloodMimicBanner = new("Aequus/Tiles/Banners/Items/BloodMimicBanner");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Candles/BloodMoonCandle</summary>
        public static readonly TextureAsset BloodMoonCandle = new("Aequus/Items/Weapons/Necromancy/Candles/BloodMoonCandle");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Candles/BloodMoonCandle_Flame</summary>
        public static readonly TextureAsset BloodMoonCandle_Flame = new("Aequus/Items/Weapons/Necromancy/Candles/BloodMoonCandle_Flame");
        /// <summary>Full Path: Aequus/Buffs/BloodthirstBuff</summary>
        public static readonly TextureAsset BloodthirstBuff = new("Aequus/Buffs/BloodthirstBuff");
        /// <summary>Full Path: Aequus/Items/Potions/BloodthirstPotion</summary>
        public static readonly TextureAsset BloodthirstPotion = new("Aequus/Items/Potions/BloodthirstPotion");
        /// <summary>Full Path: Aequus/Items/Materials/BloodyTearstone</summary>
        public static readonly TextureAsset BloodyTearstone = new("Aequus/Items/Materials/BloodyTearstone");
        /// <summary>Full Path: Aequus/Assets/Bloom_20x20</summary>
        public static readonly TextureAsset Bloom_20x20 = new("Aequus/Assets/Bloom_20x20");
        /// <summary>Full Path: Aequus/Assets/Bloom0</summary>
        public static readonly TextureAsset Bloom0 = new("Aequus/Assets/Bloom0");
        /// <summary>Full Path: Aequus/Assets/Bloom1</summary>
        public static readonly TextureAsset Bloom1 = new("Aequus/Assets/Bloom1");
        /// <summary>Full Path: Aequus/Assets/Bloom2</summary>
        public static readonly TextureAsset Bloom2 = new("Aequus/Assets/Bloom2");
        /// <summary>Full Path: Aequus/Assets/Bloom3</summary>
        public static readonly TextureAsset Bloom3 = new("Aequus/Assets/Bloom3");
        /// <summary>Full Path: Aequus/Assets/Bloom4</summary>
        public static readonly TextureAsset Bloom4 = new("Aequus/Assets/Bloom4");
        /// <summary>Full Path: Aequus/Assets/Bloom5</summary>
        public static readonly TextureAsset Bloom5 = new("Aequus/Assets/Bloom5");
        /// <summary>Full Path: Aequus/Assets/Bloom6</summary>
        public static readonly TextureAsset Bloom6 = new("Aequus/Assets/Bloom6");
        /// <summary>Full Path: Aequus/Buffs/Debuffs/BlueFire</summary>
        public static readonly TextureAsset BlueFire = new("Aequus/Buffs/Debuffs/BlueFire");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/Demon/BombarderRod</summary>
        public static readonly TextureAsset BombarderRod = new("Aequus/Items/Weapons/Magic/Demon/BombarderRod");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/Demon/BombarderRod_Glow</summary>
        public static readonly TextureAsset BombarderRod_Glow = new("Aequus/Items/Weapons/Magic/Demon/BombarderRod_Glow");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/OnHit/Anchor/BoneAnchor</summary>
        public static readonly TextureAsset BoneAnchor = new("Aequus/Items/Accessories/Combat/OnHit/Anchor/BoneAnchor");
        /// <summary>Full Path: Aequus/Items/Accessories/CrownOfBlood/Projectiles/BoneHelmMinion</summary>
        public static readonly TextureAsset BoneHelmMinion = new("Aequus/Items/Accessories/CrownOfBlood/Projectiles/BoneHelmMinion");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/OnHit/Debuff/BoneRing</summary>
        public static readonly TextureAsset BoneRing = new("Aequus/Items/Accessories/Combat/OnHit/Debuff/BoneRing");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/OnHit/Debuff/BoneRing_HandsOn</summary>
        public static readonly TextureAsset BoneRing_HandsOn = new("Aequus/Items/Accessories/Combat/OnHit/Debuff/BoneRing_HandsOn");
        /// <summary>Full Path: Aequus/Items/Accessories/CrownOfBlood/Projectiles/Bonesaw</summary>
        public static readonly TextureAsset Bonesaw = new("Aequus/Items/Accessories/CrownOfBlood/Projectiles/Bonesaw");
        /// <summary>Full Path: Aequus/Items/Accessories/CrownOfBlood/Projectiles/Bonesaw_Glow</summary>
        public static readonly TextureAsset Bonesaw_Glow = new("Aequus/Items/Accessories/CrownOfBlood/Projectiles/Bonesaw_Glow");
        /// <summary>Full Path: Aequus/Items/Accessories/CrownOfBlood/Projectiles/Bonesaw_Trail</summary>
        public static readonly TextureAsset Bonesaw_Trail = new("Aequus/Items/Accessories/CrownOfBlood/Projectiles/Bonesaw_Trail");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Paintings/Items/BongBongPainting</summary>
        public static readonly TextureAsset BongBongPainting = new("Aequus/Tiles/Furniture/Paintings/Items/BongBongPainting");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/BossRelics</summary>
        public static readonly TextureAsset BossRelics = new("Aequus/NPCs/BossMonsters/BossRelics");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/BossRelicsOrbs</summary>
        public static readonly TextureAsset BossRelicsOrbs = new("Aequus/NPCs/BossMonsters/BossRelicsOrbs");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/Misc/BoundBow</summary>
        public static readonly TextureAsset BoundBow = new("Aequus/Items/Weapons/Ranged/Misc/BoundBow");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/Misc/BoundBow_Glow</summary>
        public static readonly TextureAsset BoundBow_Glow = new("Aequus/Items/Weapons/Ranged/Misc/BoundBow_Glow");
        /// <summary>Full Path: Aequus/Projectiles/Ranged/BoundBowProj</summary>
        public static readonly TextureAsset BoundBowProj = new("Aequus/Projectiles/Ranged/BoundBowProj");
        /// <summary>Full Path: Aequus/Content/ItemPrefixes/Potions/BoundedGlint</summary>
        public static readonly TextureAsset BoundedGlint = new("Aequus/Content/ItemPrefixes/Potions/BoundedGlint");
        /// <summary>Full Path: Aequus/NPCs/Town/CarpenterNPC/Quest/Bounties/BountyUIArrow</summary>
        public static readonly TextureAsset BountyUIArrow = new("Aequus/NPCs/Town/CarpenterNPC/Quest/Bounties/BountyUIArrow");
        /// <summary>Full Path: Aequus/NPCs/Town/CarpenterNPC/Quest/Bounties/BountyUIArrow_2</summary>
        public static readonly TextureAsset BountyUIArrow_2 = new("Aequus/NPCs/Town/CarpenterNPC/Quest/Bounties/BountyUIArrow_2");
        /// <summary>Full Path: Aequus/Projectiles/Summon/BrainCauliflowerBlast</summary>
        public static readonly TextureAsset BrainCauliflowerBlast = new("Aequus/Projectiles/Summon/BrainCauliflowerBlast");
        /// <summary>Full Path: Aequus/Projectiles/Summon/BrainCauliflowerBlast_Aura</summary>
        public static readonly TextureAsset BrainCauliflowerBlast_Aura = new("Aequus/Projectiles/Summon/BrainCauliflowerBlast_Aura");
        /// <summary>Full Path: Aequus/Buffs/Minion/BrainCauliflowerBuff</summary>
        public static readonly TextureAsset BrainCauliflowerBuff = new("Aequus/Buffs/Minion/BrainCauliflowerBuff");
        /// <summary>Full Path: Aequus/Projectiles/Summon/BrainCauliflowerMinion</summary>
        public static readonly TextureAsset BrainCauliflowerMinion = new("Aequus/Projectiles/Summon/BrainCauliflowerMinion");
        /// <summary>Full Path: Aequus/Items/Weapons/Summon/Demon/BrainwaveStaff</summary>
        public static readonly TextureAsset BrainwaveStaff = new("Aequus/Items/Weapons/Summon/Demon/BrainwaveStaff");
        /// <summary>Full Path: Aequus/NPCs/Monsters/BreadOfCthulhu</summary>
        public static readonly TextureAsset BreadOfCthulhu = new("Aequus/NPCs/Monsters/BreadOfCthulhu");
        /// <summary>Full Path: Aequus/Assets/Gores/BreadOfCthulhu_0</summary>
        public static readonly TextureAsset BreadOfCthulhu_0 = new("Aequus/Assets/Gores/BreadOfCthulhu_0");
        /// <summary>Full Path: Aequus/Assets/Gores/BreadOfCthulhu_1</summary>
        public static readonly TextureAsset BreadOfCthulhu_1 = new("Aequus/Assets/Gores/BreadOfCthulhu_1");
        /// <summary>Full Path: Aequus/Assets/Gores/BreadOfCthulhu_2</summary>
        public static readonly TextureAsset BreadOfCthulhu_2 = new("Aequus/Assets/Gores/BreadOfCthulhu_2");
        /// <summary>Full Path: Aequus/Tiles/Banners/Items/BreadOfCthulhuBanner</summary>
        public static readonly TextureAsset BreadOfCthulhuBanner = new("Aequus/Tiles/Banners/Items/BreadOfCthulhuBanner");
        /// <summary>Full Path: Aequus/Items/Vanity/BreadOfCthulhuMask</summary>
        public static readonly TextureAsset BreadOfCthulhuMask = new("Aequus/Items/Vanity/BreadOfCthulhuMask");
        /// <summary>Full Path: Aequus/Items/Vanity/BreadOfCthulhuMask_Head</summary>
        public static readonly TextureAsset BreadOfCthulhuMask_Head = new("Aequus/Items/Vanity/BreadOfCthulhuMask_Head");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Paintings/Items/BreadRoachPainting</summary>
        public static readonly TextureAsset BreadRoachPainting = new("Aequus/Tiles/Furniture/Paintings/Items/BreadRoachPainting");
        /// <summary>Full Path: Aequus/Items/Misc/QuestFish/BrickFish</summary>
        public static readonly TextureAsset BrickFish = new("Aequus/Items/Misc/QuestFish/BrickFish");
        /// <summary>Full Path: Aequus/Content/Building/Bonuses/BridgeBountyBuff</summary>
        public static readonly TextureAsset BridgeBountyBuff = new("Aequus/Content/Building/Bonuses/BridgeBountyBuff");
        /// <summary>Full Path: Aequus/Content/Building/Bonuses/Buff</summary>
        public static readonly TextureAsset Buff_Bonuses = new("Aequus/Content/Building/Bonuses/Buff");
        /// <summary>Full Path: Aequus/Buffs/Buff</summary>
        public static readonly TextureAsset Buff_Buffs = new("Aequus/Buffs/Buff");
        /// <summary>Full Path: Aequus/Buffs/Misc/Empowered/Buff</summary>
        public static readonly TextureAsset Buff_Empowered = new("Aequus/Buffs/Misc/Empowered/Buff");
        /// <summary>Full Path: Aequus/Assets/UI/BuilderIcons</summary>
        public static readonly TextureAsset BuilderIcons = new("Aequus/Assets/UI/BuilderIcons");
        /// <summary>Full Path: Aequus/Assets/Bullet</summary>
        public static readonly TextureAsset Bullet = new("Aequus/Assets/Bullet");
        /// <summary>Full Path: Aequus/Tiles/Misc/Plants/Bush/Bush2x2</summary>
        public static readonly TextureAsset Bush2x2 = new("Aequus/Tiles/Misc/Plants/Bush/Bush2x2");
        /// <summary>Full Path: Aequus/Tiles/Misc/Plants/Bush/Bush2x3</summary>
        public static readonly TextureAsset Bush2x3 = new("Aequus/Tiles/Misc/Plants/Bush/Bush2x3");
        /// <summary>Full Path: Aequus/Tiles/Misc/Plants/Bush/BushCorrupt2x2</summary>
        public static readonly TextureAsset BushCorrupt2x2 = new("Aequus/Tiles/Misc/Plants/Bush/BushCorrupt2x2");
        /// <summary>Full Path: Aequus/Tiles/Misc/Plants/Bush/BushCorrupt2x3</summary>
        public static readonly TextureAsset BushCorrupt2x3 = new("Aequus/Tiles/Misc/Plants/Bush/BushCorrupt2x3");
        /// <summary>Full Path: Aequus/Tiles/Misc/Plants/Bush/BushCrimson2x2</summary>
        public static readonly TextureAsset BushCrimson2x2 = new("Aequus/Tiles/Misc/Plants/Bush/BushCrimson2x2");
        /// <summary>Full Path: Aequus/Tiles/Misc/Plants/Bush/BushCrimson2x3</summary>
        public static readonly TextureAsset BushCrimson2x3 = new("Aequus/Tiles/Misc/Plants/Bush/BushCrimson2x3");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/Money/BusinessCard</summary>
        public static readonly TextureAsset BusinessCard = new("Aequus/Items/Accessories/Misc/Money/BusinessCard");
        /// <summary>Full Path: Aequus/Items/Tools/Poles/Buzzer</summary>
        public static readonly TextureAsset Buzzer = new("Aequus/Items/Tools/Poles/Buzzer");
        /// <summary>Full Path: Aequus/NPCs/Town/CarpenterNPC/Carpenter</summary>
        public static readonly TextureAsset Carpenter = new("Aequus/NPCs/Town/CarpenterNPC/Carpenter");
        /// <summary>Full Path: Aequus/NPCs/Town/CarpenterNPC/Carpenter_Head</summary>
        public static readonly TextureAsset Carpenter_Head = new("Aequus/NPCs/Town/CarpenterNPC/Carpenter_Head");
        /// <summary>Full Path: Aequus/NPCs/Town/CarpenterNPC/Carpenter_Shimmer</summary>
        public static readonly TextureAsset Carpenter_Shimmer = new("Aequus/NPCs/Town/CarpenterNPC/Carpenter_Shimmer");
        /// <summary>Full Path: Aequus/NPCs/Town/CarpenterNPC/Carpenter_Shimmer_Head</summary>
        public static readonly TextureAsset Carpenter_Shimmer_Head = new("Aequus/NPCs/Town/CarpenterNPC/Carpenter_Shimmer_Head");
        /// <summary>Full Path: Aequus/Items/Misc/CarpenterBountyItem</summary>
        public static readonly TextureAsset CarpenterBountyItem = new("Aequus/Items/Misc/CarpenterBountyItem");
        /// <summary>Full Path: Aequus/Items/Consumables/CarpenterResetSheet</summary>
        public static readonly TextureAsset CarpenterResetSheet = new("Aequus/Items/Consumables/CarpenterResetSheet");
        /// <summary>Full Path: Aequus/Particles/Dusts/CarpenterSurpriseDust</summary>
        public static readonly TextureAsset CarpenterSurpriseDust = new("Aequus/Particles/Dusts/CarpenterSurpriseDust");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Necro/CartilageRing</summary>
        public static readonly TextureAsset CartilageRing = new("Aequus/Items/Accessories/Combat/Necro/CartilageRing");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Paintings/Items/CatalystPainting</summary>
        public static readonly TextureAsset CatalystPainting = new("Aequus/Tiles/Furniture/Paintings/Items/CatalystPainting");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/Demon/Cauterizer</summary>
        public static readonly TextureAsset Cauterizer = new("Aequus/Items/Weapons/Melee/Demon/Cauterizer");
        /// <summary>Full Path: Aequus/Projectiles/Melee/CauterizerSlash</summary>
        public static readonly TextureAsset CauterizerSlash = new("Aequus/Projectiles/Melee/CauterizerSlash");
        /// <summary>Full Path: Aequus/Items/Misc/CelesitalEightBall</summary>
        public static readonly TextureAsset CelesitalEightBall = new("Aequus/Items/Misc/CelesitalEightBall");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Passive/CelesteTorus</summary>
        public static readonly TextureAsset CelesteTorus = new("Aequus/Items/Accessories/Combat/Passive/CelesteTorus");
        /// <summary>Full Path: Aequus/Projectiles/Misc/Friendly/CelesteTorusProj</summary>
        public static readonly TextureAsset CelesteTorusProj = new("Aequus/Projectiles/Misc/Friendly/CelesteTorusProj");
        /// <summary>Full Path: Aequus/Items/Misc/Dyes/CensorDye</summary>
        public static readonly TextureAsset CensorDye = new("Aequus/Items/Misc/Dyes/CensorDye");
        /// <summary>Full Path: Aequus/Items/Consumables/Powders/ChlorophytePowder</summary>
        public static readonly TextureAsset ChlorophytePowder = new("Aequus/Items/Consumables/Powders/ChlorophytePowder");
        /// <summary>Full Path: Aequus/NPCs/Monsters/Event/DemonSiege/Cindera</summary>
        public static readonly TextureAsset Cindera = new("Aequus/NPCs/Monsters/Event/DemonSiege/Cindera");
        /// <summary>Full Path: Aequus/Tiles/Banners/Items/CinderaBanner</summary>
        public static readonly TextureAsset CinderaBanner = new("Aequus/Tiles/Banners/Items/CinderaBanner");
        /// <summary>Full Path: Aequus/Items/Consumables/Foods/CinnamonRoll</summary>
        public static readonly TextureAsset CinnamonRoll = new("Aequus/Items/Consumables/Foods/CinnamonRoll");
        /// <summary>Full Path: Aequus/Content/DronePylons/NPCs/CleanserDrone</summary>
        public static readonly TextureAsset CleanserDrone = new("Aequus/Content/DronePylons/NPCs/CleanserDrone");
        /// <summary>Full Path: Aequus/Content/DronePylons/NPCs/CleanserDrone_Glow</summary>
        public static readonly TextureAsset CleanserDrone_Glow = new("Aequus/Content/DronePylons/NPCs/CleanserDrone_Glow");
        /// <summary>Full Path: Aequus/Content/DronePylons/NPCs/CleanserDrone_Gun</summary>
        public static readonly TextureAsset CleanserDrone_Gun = new("Aequus/Content/DronePylons/NPCs/CleanserDrone_Gun");
        /// <summary>Full Path: Aequus/Content/DronePylons/NPCs/CleanserDrone_Gun_Glow</summary>
        public static readonly TextureAsset CleanserDrone_Gun_Glow = new("Aequus/Content/DronePylons/NPCs/CleanserDrone_Gun_Glow");
        /// <summary>Full Path: Aequus/Tiles/Misc/Plants/CloudFlower</summary>
        public static readonly TextureAsset CloudFlower = new("Aequus/Tiles/Misc/Plants/CloudFlower");
        /// <summary>Full Path: Aequus/NPCs/Monsters/CrabCrevice/CoconutCrab</summary>
        public static readonly TextureAsset CoconutCrab = new("Aequus/NPCs/Monsters/CrabCrevice/CoconutCrab");
        /// <summary>Full Path: Aequus/NPCs/Monsters/CrabCrevice/CoconutCrab_Glow</summary>
        public static readonly TextureAsset CoconutCrab_Glow = new("Aequus/NPCs/Monsters/CrabCrevice/CoconutCrab_Glow");
        /// <summary>Full Path: Aequus/Tiles/Banners/Items/CoconutCrabBanner</summary>
        public static readonly TextureAsset CoconutCrabBanner = new("Aequus/Tiles/Banners/Items/CoconutCrabBanner");
        /// <summary>Full Path: Aequus/Assets/UI/CooldownBack</summary>
        public static readonly TextureAsset CooldownBack = new("Aequus/Assets/UI/CooldownBack");
        /// <summary>Full Path: Aequus/Tiles/Misc/Plants/CorpseFlower</summary>
        public static readonly TextureAsset CorpseFlower = new("Aequus/Tiles/Misc/Plants/CorpseFlower");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Candles/CorruptionCandle</summary>
        public static readonly TextureAsset CorruptionCandle = new("Aequus/Items/Weapons/Necromancy/Candles/CorruptionCandle");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Candles/CorruptionCandle_Flame</summary>
        public static readonly TextureAsset CorruptionCandle_Flame = new("Aequus/Items/Weapons/Necromancy/Candles/CorruptionCandle_Flame");
        /// <summary>Full Path: Aequus/Buffs/Debuffs/CorruptionHellfire</summary>
        public static readonly TextureAsset CorruptionHellfire = new("Aequus/Buffs/Debuffs/CorruptionHellfire");
        /// <summary>Full Path: Aequus/Buffs/Minion/CorruptPlantBuff</summary>
        public static readonly TextureAsset CorruptPlantBuff = new("Aequus/Buffs/Minion/CorruptPlantBuff");
        /// <summary>Full Path: Aequus/Projectiles/Summon/CorruptPlantMinion</summary>
        public static readonly TextureAsset CorruptPlantMinion = new("Aequus/Projectiles/Summon/CorruptPlantMinion");
        /// <summary>Full Path: Aequus/Projectiles/Summon/CorruptPlantMinion_Chain</summary>
        public static readonly TextureAsset CorruptPlantMinion_Chain = new("Aequus/Projectiles/Summon/CorruptPlantMinion_Chain");
        /// <summary>Full Path: Aequus/Projectiles/Summon/CorruptPlantMinion_Large</summary>
        public static readonly TextureAsset CorruptPlantMinion_Large = new("Aequus/Projectiles/Summon/CorruptPlantMinion_Large");
        /// <summary>Full Path: Aequus/Projectiles/Summon/CorruptPlantMinion_Med</summary>
        public static readonly TextureAsset CorruptPlantMinion_Med = new("Aequus/Projectiles/Summon/CorruptPlantMinion_Med");
        /// <summary>Full Path: Aequus/Items/Weapons/Summon/Minion/CorruptPot</summary>
        public static readonly TextureAsset CorruptPot = new("Aequus/Items/Weapons/Summon/Minion/CorruptPot");
        /// <summary>Full Path: Aequus/Items/Consumables/Permanent/CosmicChest</summary>
        public static readonly TextureAsset CosmicChest = new("Aequus/Items/Consumables/Permanent/CosmicChest");
        /// <summary>Full Path: Aequus/Items/Materials/Energies/CosmicEnergy</summary>
        public static readonly TextureAsset CosmicEnergy = new("Aequus/Items/Materials/Energies/CosmicEnergy");
        /// <summary>Full Path: Aequus/Assets/Effects/Textures/CosmicEnergyGradient</summary>
        public static readonly TextureAsset CosmicEnergyGradient = new("Aequus/Assets/Effects/Textures/CosmicEnergyGradient");
        /// <summary>Full Path: Aequus/Content/Events/GlimmerEvent/Sky/CosmicMonolith</summary>
        public static readonly TextureAsset CosmicMonolith = new("Aequus/Content/Events/GlimmerEvent/Sky/CosmicMonolith");
        /// <summary>Full Path: Aequus/Content/Events/GlimmerEvent/Sky/CosmicMonolithTile</summary>
        public static readonly TextureAsset CosmicMonolithTile = new("Aequus/Content/Events/GlimmerEvent/Sky/CosmicMonolithTile");
        /// <summary>Full Path: Aequus/Unused/Items/Crabax</summary>
        public static readonly TextureAsset Crabax = new("Aequus/Unused/Items/Crabax");
        /// <summary>Full Path: Aequus/Projectiles/Misc/Bobbers/CrabBobber</summary>
        public static readonly TextureAsset CrabBobber = new("Aequus/Projectiles/Misc/Bobbers/CrabBobber");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Crab/CrabClock</summary>
        public static readonly TextureAsset CrabClock = new("Aequus/Tiles/Furniture/Crab/CrabClock");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Crab/CrabClockTile</summary>
        public static readonly TextureAsset CrabClockTile = new("Aequus/Tiles/Furniture/Crab/CrabClockTile");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Crab/CrabClockTile_Highlight</summary>
        public static readonly TextureAsset CrabClockTile_Highlight = new("Aequus/Tiles/Furniture/Crab/CrabClockTile_Highlight");
        /// <summary>Full Path: Aequus/Assets/UI/BestiaryIcons/CrabCrevice</summary>
        public static readonly TextureAsset CrabCrevice = new("Aequus/Assets/UI/BestiaryIcons/CrabCrevice");
        /// <summary>Full Path: Aequus/Items/Consumables/Crates/CrabCreviceCrate</summary>
        public static readonly TextureAsset CrabCreviceCrate = new("Aequus/Items/Consumables/Crates/CrabCreviceCrate");
        /// <summary>Full Path: Aequus/Items/Consumables/Crates/CrabCreviceCrateHard</summary>
        public static readonly TextureAsset CrabCreviceCrateHard = new("Aequus/Items/Consumables/Crates/CrabCreviceCrateHard");
        /// <summary>Full Path: Aequus/Content/Biomes/CrabCrevice/Water/CrabCreviceDroplet</summary>
        public static readonly TextureAsset CrabCreviceDroplet = new("Aequus/Content/Biomes/CrabCrevice/Water/CrabCreviceDroplet");
        /// <summary>Full Path: Aequus/Tiles/CrabCrevice/Ambient/CrabCrevicePot</summary>
        public static readonly TextureAsset CrabCrevicePot = new("Aequus/Tiles/CrabCrevice/Ambient/CrabCrevicePot");
        /// <summary>Full Path: Aequus/Assets/Gores/Pots/CrabCrevicePot_0_0</summary>
        public static readonly TextureAsset CrabCrevicePot_0_0 = new("Aequus/Assets/Gores/Pots/CrabCrevicePot_0_0");
        /// <summary>Full Path: Aequus/Assets/Gores/Pots/CrabCrevicePot_0_1</summary>
        public static readonly TextureAsset CrabCrevicePot_0_1 = new("Aequus/Assets/Gores/Pots/CrabCrevicePot_0_1");
        /// <summary>Full Path: Aequus/Assets/Gores/Pots/CrabCrevicePot_0_2</summary>
        public static readonly TextureAsset CrabCrevicePot_0_2 = new("Aequus/Assets/Gores/Pots/CrabCrevicePot_0_2");
        /// <summary>Full Path: Aequus/Assets/Gores/Pots/CrabCrevicePot_1_0</summary>
        public static readonly TextureAsset CrabCrevicePot_1_0 = new("Aequus/Assets/Gores/Pots/CrabCrevicePot_1_0");
        /// <summary>Full Path: Aequus/Assets/Gores/Pots/CrabCrevicePot_1_1</summary>
        public static readonly TextureAsset CrabCrevicePot_1_1 = new("Aequus/Assets/Gores/Pots/CrabCrevicePot_1_1");
        /// <summary>Full Path: Aequus/Assets/Gores/Pots/CrabCrevicePot_1_2</summary>
        public static readonly TextureAsset CrabCrevicePot_1_2 = new("Aequus/Assets/Gores/Pots/CrabCrevicePot_1_2");
        /// <summary>Full Path: Aequus/Particles/Dusts/CrabCreviceSplash</summary>
        public static readonly TextureAsset CrabCreviceSplash = new("Aequus/Particles/Dusts/CrabCreviceSplash");
        /// <summary>Full Path: Aequus/Content/Biomes/CrabCrevice/Background/CrabCreviceSurfaceBackground_0</summary>
        public static readonly TextureAsset CrabCreviceSurfaceBackground_0 = new("Aequus/Content/Biomes/CrabCrevice/Background/CrabCreviceSurfaceBackground_0");
        /// <summary>Full Path: Aequus/Content/Biomes/CrabCrevice/Background/CrabCreviceSurfaceBackground_1</summary>
        public static readonly TextureAsset CrabCreviceSurfaceBackground_1 = new("Aequus/Content/Biomes/CrabCrevice/Background/CrabCreviceSurfaceBackground_1");
        /// <summary>Full Path: Aequus/Content/Biomes/CrabCrevice/Background/CrabCreviceSurfaceBackground_2</summary>
        public static readonly TextureAsset CrabCreviceSurfaceBackground_2 = new("Aequus/Content/Biomes/CrabCrevice/Background/CrabCreviceSurfaceBackground_2");
        /// <summary>Full Path: Aequus/Content/Biomes/CrabCrevice/Water/CrabCreviceWater</summary>
        public static readonly TextureAsset CrabCreviceWater = new("Aequus/Content/Biomes/CrabCrevice/Water/CrabCreviceWater");
        /// <summary>Full Path: Aequus/Content/Biomes/CrabCrevice/Water/CrabCreviceWater_Block</summary>
        public static readonly TextureAsset CrabCreviceWater_Block = new("Aequus/Content/Biomes/CrabCrevice/Water/CrabCreviceWater_Block");
        /// <summary>Full Path: Aequus/Content/Biomes/CrabCrevice/Water/CrabCreviceWater_Slope</summary>
        public static readonly TextureAsset CrabCreviceWater_Slope = new("Aequus/Content/Biomes/CrabCrevice/Water/CrabCreviceWater_Slope");
        /// <summary>Full Path: Aequus/Content/Biomes/CrabCrevice/Water/CrabCreviceWaterfall</summary>
        public static readonly TextureAsset CrabCreviceWaterfall = new("Aequus/Content/Biomes/CrabCrevice/Water/CrabCreviceWaterfall");
        /// <summary>Full Path: Aequus/Items/Misc/LegendaryFish/CrabDaughter</summary>
        public static readonly TextureAsset CrabDaughter = new("Aequus/Items/Misc/LegendaryFish/CrabDaughter");
        /// <summary>Full Path: Aequus/NPCs/Monsters/CrabCrevice/CrabFish</summary>
        public static readonly TextureAsset CrabFish = new("Aequus/NPCs/Monsters/CrabCrevice/CrabFish");
        /// <summary>Full Path: Aequus/NPCs/Monsters/CrabCrevice/CrabFish_Glow</summary>
        public static readonly TextureAsset CrabFish_Glow = new("Aequus/NPCs/Monsters/CrabCrevice/CrabFish_Glow");
        /// <summary>Full Path: Aequus/Tiles/Banners/Items/CrabFishBanner</summary>
        public static readonly TextureAsset CrabFishBanner = new("Aequus/Tiles/Banners/Items/CrabFishBanner");
        /// <summary>Full Path: Aequus/Tiles/CrabCrevice/Ambient/CrabFloorPlants</summary>
        public static readonly TextureAsset CrabFloorPlants = new("Aequus/Tiles/CrabCrevice/Ambient/CrabFloorPlants");
        /// <summary>Full Path: Aequus/Tiles/CrabCrevice/Ambient/CrabGrassBig</summary>
        public static readonly TextureAsset CrabGrassBig = new("Aequus/Tiles/CrabCrevice/Ambient/CrabGrassBig");
        /// <summary>Full Path: Aequus/Tiles/CrabCrevice/Ambient/CrabHydrosailia</summary>
        public static readonly TextureAsset CrabHydrosailia = new("Aequus/Tiles/CrabCrevice/Ambient/CrabHydrosailia");
        /// <summary>Full Path: Aequus/Items/Tools/Poles/CrabRod</summary>
        public static readonly TextureAsset CrabRod = new("Aequus/Items/Tools/Poles/CrabRod");
        /// <summary>Full Path: Aequus/Assets/UI/BossChecklist/Crabson</summary>
        public static readonly TextureAsset Crabson_BossChecklist = new("Aequus/Assets/UI/BossChecklist/Crabson");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Crabson_Chain</summary>
        public static readonly TextureAsset Crabson_Chain = new("Aequus/NPCs/BossMonsters/Crabson/Crabson_Chain");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Crabson</summary>
        public static readonly TextureAsset Crabson_Crabson = new("Aequus/NPCs/BossMonsters/Crabson/Crabson");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/CrabsonOld/Crabson</summary>
        public static readonly TextureAsset Crabson_CrabsonOld = new("Aequus/NPCs/BossMonsters/Crabson/CrabsonOld/Crabson");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Crabson_Eyes</summary>
        public static readonly TextureAsset Crabson_Eyes = new("Aequus/NPCs/BossMonsters/Crabson/Crabson_Eyes");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Crabson_Head_Boss</summary>
        public static readonly TextureAsset Crabson_Head_Boss_Crabson = new("Aequus/NPCs/BossMonsters/Crabson/Crabson_Head_Boss");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/CrabsonOld/Crabson_Head_Boss</summary>
        public static readonly TextureAsset Crabson_Head_Boss_CrabsonOld = new("Aequus/NPCs/BossMonsters/Crabson/CrabsonOld/Crabson_Head_Boss");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Crabson_Legs</summary>
        public static readonly TextureAsset Crabson_Legs = new("Aequus/NPCs/BossMonsters/Crabson/Crabson_Legs");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Crabson_Pupil</summary>
        public static readonly TextureAsset Crabson_Pupil = new("Aequus/NPCs/BossMonsters/Crabson/Crabson_Pupil");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Rewards/CrabsonBag</summary>
        public static readonly TextureAsset CrabsonBag = new("Aequus/NPCs/BossMonsters/Crabson/Rewards/CrabsonBag");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Projectiles/CrabsonBubble</summary>
        public static readonly TextureAsset CrabsonBubble = new("Aequus/NPCs/BossMonsters/Crabson/Projectiles/CrabsonBubble");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/CrabsonOld/CrabsonClaw_Chain</summary>
        public static readonly TextureAsset CrabsonClaw_Chain = new("Aequus/NPCs/BossMonsters/Crabson/CrabsonOld/CrabsonClaw_Chain");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/CrabsonClaw</summary>
        public static readonly TextureAsset CrabsonClaw_Crabson = new("Aequus/NPCs/BossMonsters/Crabson/CrabsonClaw");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/CrabsonOld/CrabsonClaw</summary>
        public static readonly TextureAsset CrabsonClaw_CrabsonOld = new("Aequus/NPCs/BossMonsters/Crabson/CrabsonOld/CrabsonClaw");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/CrabsonClaw_Head_Boss</summary>
        public static readonly TextureAsset CrabsonClaw_Head_Boss_Crabson = new("Aequus/NPCs/BossMonsters/Crabson/CrabsonClaw_Head_Boss");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/CrabsonOld/CrabsonClaw_Head_Boss</summary>
        public static readonly TextureAsset CrabsonClaw_Head_Boss_CrabsonOld = new("Aequus/NPCs/BossMonsters/Crabson/CrabsonOld/CrabsonClaw_Head_Boss");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Gore/CrabsonGoreClawBottom</summary>
        public static readonly TextureAsset CrabsonGoreClawBottom = new("Aequus/NPCs/BossMonsters/Crabson/Gore/CrabsonGoreClawBottom");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Gore/CrabsonGoreClawTop</summary>
        public static readonly TextureAsset CrabsonGoreClawTop = new("Aequus/NPCs/BossMonsters/Crabson/Gore/CrabsonGoreClawTop");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Gore/CrabsonGoreHead</summary>
        public static readonly TextureAsset CrabsonGoreHead = new("Aequus/NPCs/BossMonsters/Crabson/Gore/CrabsonGoreHead");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Gore/CrabsonGoreHeadBottom</summary>
        public static readonly TextureAsset CrabsonGoreHeadBottom = new("Aequus/NPCs/BossMonsters/Crabson/Gore/CrabsonGoreHeadBottom");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Gore/CrabsonGoreLeg</summary>
        public static readonly TextureAsset CrabsonGoreLeg = new("Aequus/NPCs/BossMonsters/Crabson/Gore/CrabsonGoreLeg");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Rewards/CrabsonMask</summary>
        public static readonly TextureAsset CrabsonMask = new("Aequus/NPCs/BossMonsters/Crabson/Rewards/CrabsonMask");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Rewards/CrabsonMask_Head</summary>
        public static readonly TextureAsset CrabsonMask_Head = new("Aequus/NPCs/BossMonsters/Crabson/Rewards/CrabsonMask_Head");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Projectiles/CrabsonPearl</summary>
        public static readonly TextureAsset CrabsonPearl = new("Aequus/NPCs/BossMonsters/Crabson/Projectiles/CrabsonPearl");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Projectiles/CrabsonPearl_White</summary>
        public static readonly TextureAsset CrabsonPearl_White = new("Aequus/NPCs/BossMonsters/Crabson/Projectiles/CrabsonPearl_White");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Projectiles/CrabsonPearlShard</summary>
        public static readonly TextureAsset CrabsonPearlShard = new("Aequus/NPCs/BossMonsters/Crabson/Projectiles/CrabsonPearlShard");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Rewards/CrabsonRelic</summary>
        public static readonly TextureAsset CrabsonRelic = new("Aequus/NPCs/BossMonsters/Crabson/Rewards/CrabsonRelic");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Rewards/CrabsonTreasureChest</summary>
        public static readonly TextureAsset CrabsonTreasureChest = new("Aequus/NPCs/BossMonsters/Crabson/Rewards/CrabsonTreasureChest");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Rewards/CrabsonTrophy</summary>
        public static readonly TextureAsset CrabsonTrophy = new("Aequus/NPCs/BossMonsters/Crabson/Rewards/CrabsonTrophy");
        /// <summary>Full Path: Aequus/Items/Misc/Bait/CrateBait</summary>
        public static readonly TextureAsset CrateBait = new("Aequus/Items/Misc/Bait/CrateBait");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Candles/CrimsonCandle</summary>
        public static readonly TextureAsset CrimsonCandle = new("Aequus/Items/Weapons/Necromancy/Candles/CrimsonCandle");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Candles/CrimsonCandle_Flame</summary>
        public static readonly TextureAsset CrimsonCandle_Flame = new("Aequus/Items/Weapons/Necromancy/Candles/CrimsonCandle_Flame");
        /// <summary>Full Path: Aequus/Buffs/Debuffs/CrimsonHellfire</summary>
        public static readonly TextureAsset CrimsonHellfire = new("Aequus/Buffs/Debuffs/CrimsonHellfire");
        /// <summary>Full Path: Aequus/Items/Accessories/CrownOfBlood/Buffs/CrownOfBloodBuff</summary>
        public static readonly TextureAsset CrownOfBloodBuff = new("Aequus/Items/Accessories/CrownOfBlood/Buffs/CrownOfBloodBuff");
        /// <summary>Full Path: Aequus/Items/Accessories/CrownOfBlood/Buffs/CrownOfBloodCooldown</summary>
        public static readonly TextureAsset CrownOfBloodCooldown = new("Aequus/Items/Accessories/CrownOfBlood/Buffs/CrownOfBloodCooldown");
        /// <summary>Full Path: Aequus/Items/Accessories/CrownOfBlood/Buffs/CrownOfBloodDebuff</summary>
        public static readonly TextureAsset CrownOfBloodDebuff = new("Aequus/Items/Accessories/CrownOfBlood/Buffs/CrownOfBloodDebuff");
        /// <summary>Full Path: Aequus/Items/Accessories/CrownOfBlood/CrownOfBloodItem</summary>
        public static readonly TextureAsset CrownOfBloodItem = new("Aequus/Items/Accessories/CrownOfBlood/CrownOfBloodItem");
        /// <summary>Full Path: Aequus/Items/Accessories/CrownOfBlood/CrownOfBloodItem_Crown</summary>
        public static readonly TextureAsset CrownOfBloodItem_Crown = new("Aequus/Items/Accessories/CrownOfBlood/CrownOfBloodItem_Crown");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/CrownOfDarkness</summary>
        public static readonly TextureAsset CrownOfDarkness = new("Aequus/Items/Accessories/Combat/CrownOfDarkness");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/CrownOfDarkness_Crown</summary>
        public static readonly TextureAsset CrownOfDarkness_Crown = new("Aequus/Items/Accessories/Combat/CrownOfDarkness_Crown");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/CrownOfTheGrounded</summary>
        public static readonly TextureAsset CrownOfTheGrounded = new("Aequus/Items/Accessories/Combat/CrownOfTheGrounded");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/CrownOfTheGrounded_Crown</summary>
        public static readonly TextureAsset CrownOfTheGrounded_Crown = new("Aequus/Items/Accessories/Combat/CrownOfTheGrounded_Crown");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/CrusadersCrossbow</summary>
        public static readonly TextureAsset CrusadersCrossbow = new("Aequus/Items/Weapons/Ranged/CrusadersCrossbow");
        /// <summary>Full Path: Aequus/Projectiles/Ranged/CrusadersCrossbowBolt</summary>
        public static readonly TextureAsset CrusadersCrossbowBolt = new("Aequus/Projectiles/Ranged/CrusadersCrossbowBolt");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/CrystalDagger</summary>
        public static readonly TextureAsset CrystalDagger = new("Aequus/Items/Weapons/Melee/CrystalDagger");
        /// <summary>Full Path: Aequus/Buffs/Misc/CrystalDaggerBuff</summary>
        public static readonly TextureAsset CrystalDaggerBuff = new("Aequus/Buffs/Misc/CrystalDaggerBuff");
        /// <summary>Full Path: Aequus/Items/Misc/Bait/CursedPopper</summary>
        public static readonly TextureAsset CursedPopper = new("Aequus/Items/Misc/Bait/CursedPopper");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_10</summary>
        public static readonly TextureAsset Cursor_10_PumpkingCursor = new("Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_10");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_10</summary>
        public static readonly TextureAsset Cursor_10_XmasCursor = new("Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_10");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_2</summary>
        public static readonly TextureAsset Cursor_2_PumpkingCursor = new("Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_2");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_2</summary>
        public static readonly TextureAsset Cursor_2_XmasCursor = new("Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_2");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_3</summary>
        public static readonly TextureAsset Cursor_3_PumpkingCursor = new("Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_3");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_3</summary>
        public static readonly TextureAsset Cursor_3_XmasCursor = new("Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_3");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_6</summary>
        public static readonly TextureAsset Cursor_6_PumpkingCursor = new("Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_6");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_6</summary>
        public static readonly TextureAsset Cursor_6_XmasCursor = new("Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_6");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_7</summary>
        public static readonly TextureAsset Cursor_7_PumpkingCursor = new("Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_7");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_7</summary>
        public static readonly TextureAsset Cursor_7_XmasCursor = new("Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_7");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_8</summary>
        public static readonly TextureAsset Cursor_8_PumpkingCursor = new("Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_8");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_8</summary>
        public static readonly TextureAsset Cursor_8_XmasCursor = new("Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_8");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_9</summary>
        public static readonly TextureAsset Cursor_9_PumpkingCursor = new("Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_9");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_9</summary>
        public static readonly TextureAsset Cursor_9_XmasCursor = new("Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_9");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor</summary>
        public static readonly TextureAsset Cursor_PumpkingCursor = new("Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_Smart</summary>
        public static readonly TextureAsset Cursor_Smart_PumpkingCursor = new("Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_Smart");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_Smart</summary>
        public static readonly TextureAsset Cursor_Smart_XmasCursor = new("Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_Smart");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/XmasCursor/Cursor</summary>
        public static readonly TextureAsset Cursor_XmasCursor = new("Aequus/Content/CursorDyes/Items/XmasCursor/Cursor");
        /// <summary>Full Path: Aequus/Items/Armor/SetTrap/DartTrapHat</summary>
        public static readonly TextureAsset DartTrapHat = new("Aequus/Items/Armor/SetTrap/DartTrapHat");
        /// <summary>Full Path: Aequus/Items/Armor/SetTrap/DartTrapHat_Head</summary>
        public static readonly TextureAsset DartTrapHat_Head = new("Aequus/Items/Armor/SetTrap/DartTrapHat_Head");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/OnHit/Anchor/DavyJonesAnchor</summary>
        public static readonly TextureAsset DavyJonesAnchor = new("Aequus/Items/Accessories/Combat/OnHit/Anchor/DavyJonesAnchor");
        /// <summary>Full Path: Aequus/Projectiles/Misc/Friendly/DavyJonesAnchorProj</summary>
        public static readonly TextureAsset DavyJonesAnchorProj = new("Aequus/Projectiles/Misc/Friendly/DavyJonesAnchorProj");
        /// <summary>Full Path: Aequus/Items/Potions/Unique/DeathsEmbrace</summary>
        public static readonly TextureAsset DeathsEmbrace = new("Aequus/Items/Potions/Unique/DeathsEmbrace");
        /// <summary>Full Path: Aequus/Buffs/DeathsEmbraceBuff</summary>
        public static readonly TextureAsset DeathsEmbraceBuff = new("Aequus/Buffs/DeathsEmbraceBuff");
        /// <summary>Full Path: Aequus/Buffs/Debuffs/Debuff</summary>
        public static readonly TextureAsset Debuff = new("Aequus/Buffs/Debuffs/Debuff");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/Demon/Deltoid</summary>
        public static readonly TextureAsset Deltoid = new("Aequus/Items/Weapons/Ranged/Demon/Deltoid");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/Demon/Deltoid_Glow</summary>
        public static readonly TextureAsset Deltoid_Glow = new("Aequus/Items/Weapons/Ranged/Demon/Deltoid_Glow");
        /// <summary>Full Path: Aequus/Projectiles/Ranged/DeltoidArrow</summary>
        public static readonly TextureAsset DeltoidArrow = new("Aequus/Projectiles/Ranged/DeltoidArrow");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor_2</summary>
        public static readonly TextureAsset DemonCursor_2 = new("Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor_2");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor_3</summary>
        public static readonly TextureAsset DemonCursor_3 = new("Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor_3");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor_6</summary>
        public static readonly TextureAsset DemonCursor_6 = new("Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor_6");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor_6Outline</summary>
        public static readonly TextureAsset DemonCursor_6Outline = new("Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor_6Outline");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor</summary>
        public static readonly TextureAsset DemonCursor_DemonCursor = new("Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/DemonCursor</summary>
        public static readonly TextureAsset DemonCursor_Items = new("Aequus/Content/CursorDyes/Items/DemonCursor");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor_Smart</summary>
        public static readonly TextureAsset DemonCursor_Smart = new("Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor_Smart");
        /// <summary>Full Path: Aequus/Items/Materials/Energies/DemonicEnergy</summary>
        public static readonly TextureAsset DemonicEnergy = new("Aequus/Items/Materials/Energies/DemonicEnergy");
        /// <summary>Full Path: Aequus/Assets/Effects/Textures/DemonicEnergyGradient</summary>
        public static readonly TextureAsset DemonicEnergyGradient = new("Aequus/Assets/Effects/Textures/DemonicEnergyGradient");
        /// <summary>Full Path: Aequus/Assets/UI/BestiaryIcons/DemonSiege</summary>
        public static readonly TextureAsset DemonSiege_BestiaryIcons = new("Aequus/Assets/UI/BestiaryIcons/DemonSiege");
        /// <summary>Full Path: Aequus/Assets/UI/BossChecklist/DemonSiege</summary>
        public static readonly TextureAsset DemonSiege_BossChecklist = new("Aequus/Assets/UI/BossChecklist/DemonSiege");
        /// <summary>Full Path: Aequus/Assets/UI/EventIcons/DemonSiege</summary>
        public static readonly TextureAsset DemonSiege_EventIcons = new("Aequus/Assets/UI/EventIcons/DemonSiege");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/Demon/DemonSword</summary>
        public static readonly TextureAsset DemonSword = new("Aequus/Items/Weapons/Melee/Demon/DemonSword");
        /// <summary>Full Path: Aequus/Items/Materials/Fish/Depthscale</summary>
        public static readonly TextureAsset Depthscale = new("Aequus/Items/Materials/Fish/Depthscale");
        /// <summary>Full Path: Aequus/Unused/Items/SlotMachines/DesertRoulette</summary>
        public static readonly TextureAsset DesertRoulette = new("Aequus/Unused/Items/SlotMachines/DesertRoulette");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/Fishing/DevilsTongue</summary>
        public static readonly TextureAsset DevilsTongue = new("Aequus/Items/Accessories/Misc/Fishing/DevilsTongue");
        /// <summary>Full Path: Aequus/Tiles/Misc/BigGems/DiamondDeposit</summary>
        public static readonly TextureAsset DiamondDeposit = new("Aequus/Tiles/Misc/BigGems/DiamondDeposit");
        /// <summary>Full Path: Aequus/Assets/Gores/Gems/DiamondGore</summary>
        public static readonly TextureAsset DiamondGore = new("Aequus/Assets/Gores/Gems/DiamondGore");
        /// <summary>Full Path: Aequus/Items/Misc/Dyes/DiscoDye</summary>
        public static readonly TextureAsset DiscoDye = new("Aequus/Items/Misc/Dyes/DiscoDye");
        /// <summary>Full Path: Aequus/Items/Weapons/Summon/Minion/DisturbanceStaff</summary>
        public static readonly TextureAsset DisturbanceStaff = new("Aequus/Items/Weapons/Summon/Minion/DisturbanceStaff");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/OmegaStarite/DragonBall</summary>
        public static readonly TextureAsset DragonBall = new("Aequus/Items/Vanity/Pets/OmegaStarite/DragonBall");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Candles/DragonsBreath</summary>
        public static readonly TextureAsset DragonsBreath = new("Aequus/Items/Weapons/Necromancy/Candles/DragonsBreath");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Candles/DragonsBreath_Flame</summary>
        public static readonly TextureAsset DragonsBreath_Flame = new("Aequus/Items/Weapons/Necromancy/Candles/DragonsBreath_Flame");
        /// <summary>Full Path: Aequus/Items/Misc/Trash/Driftwood</summary>
        public static readonly TextureAsset Driftwood = new("Aequus/Items/Misc/Trash/Driftwood");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/Drone/DroneBuff</summary>
        public static readonly TextureAsset DroneBuff = new("Aequus/Items/Vanity/Pets/Drone/DroneBuff");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Candles/DungeonCandle</summary>
        public static readonly TextureAsset DungeonCandle = new("Aequus/Items/Weapons/Necromancy/Candles/DungeonCandle");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Candles/DungeonCandle_Flame</summary>
        public static readonly TextureAsset DungeonCandle_Flame = new("Aequus/Items/Weapons/Necromancy/Candles/DungeonCandle_Flame");
        /// <summary>Full Path: Aequus/Assets/UI/BossChecklist/DustDevil</summary>
        public static readonly TextureAsset DustDevil_BossChecklist = new("Aequus/Assets/UI/BossChecklist/DustDevil");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/DustDevil/DustDevil</summary>
        public static readonly TextureAsset DustDevil_DustDevil = new("Aequus/NPCs/BossMonsters/DustDevil/DustDevil");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/DustDevil/DustDevil_Head_Boss</summary>
        public static readonly TextureAsset DustDevil_Head_Boss = new("Aequus/NPCs/BossMonsters/DustDevil/DustDevil_Head_Boss");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/DustDevil/Rewards/DustDevilBag</summary>
        public static readonly TextureAsset DustDevilBag = new("Aequus/NPCs/BossMonsters/DustDevil/Rewards/DustDevilBag");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/DustDevil/Rewards/DustDevilRelic</summary>
        public static readonly TextureAsset DustDevilRelic = new("Aequus/NPCs/BossMonsters/DustDevil/Rewards/DustDevilRelic");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/DustDevil/Rewards/DustDevilTrophy</summary>
        public static readonly TextureAsset DustDevilTrophy = new("Aequus/NPCs/BossMonsters/DustDevil/Rewards/DustDevilTrophy");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/DustDevil/DustParticle</summary>
        public static readonly TextureAsset DustParticle = new("Aequus/NPCs/BossMonsters/DustDevil/DustParticle");
        /// <summary>Full Path: Aequus/NPCs/Critters/DwarfStarite</summary>
        public static readonly TextureAsset DwarfStarite = new("Aequus/NPCs/Critters/DwarfStarite");
        /// <summary>Full Path: Aequus/NPCs/Critters/DwarfStariteItem</summary>
        public static readonly TextureAsset DwarfStariteItem = new("Aequus/NPCs/Critters/DwarfStariteItem");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/DyableCursor</summary>
        public static readonly TextureAsset DyableCursor = new("Aequus/Content/CursorDyes/Items/DyableCursor");
        /// <summary>Full Path: Aequus/Assets/DyeSample</summary>
        public static readonly TextureAsset DyeSample = new("Aequus/Assets/DyeSample");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/DynaKnife/Dynaknife</summary>
        public static readonly TextureAsset Dynaknife = new("Aequus/Items/Weapons/Melee/DynaKnife/Dynaknife");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/DynaKnife/Dynaknife_Glow</summary>
        public static readonly TextureAsset Dynaknife_Glow = new("Aequus/Items/Weapons/Melee/DynaKnife/Dynaknife_Glow");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/DynaKnife/DynaknifeProj</summary>
        public static readonly TextureAsset DynaknifeProj = new("Aequus/Items/Weapons/Melee/DynaKnife/DynaknifeProj");
        /// <summary>Full Path: Aequus/Assets/Textures/EffectNoise</summary>
        public static readonly TextureAsset EffectNoise = new("Aequus/Assets/Textures/EffectNoise");
        /// <summary>Full Path: Aequus/Assets/Textures/EffectWaterRefraction</summary>
        public static readonly TextureAsset EffectWaterRefraction = new("Aequus/Assets/Textures/EffectWaterRefraction");
        /// <summary>Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/ElitePlants/ElitePlantArgon</summary>
        public static readonly TextureAsset ElitePlantArgon = new("Aequus/Content/Biomes/MossBiomes/Tiles/ElitePlants/ElitePlantArgon");
        /// <summary>Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/ElitePlants/ElitePlantKrypton</summary>
        public static readonly TextureAsset ElitePlantKrypton = new("Aequus/Content/Biomes/MossBiomes/Tiles/ElitePlants/ElitePlantKrypton");
        /// <summary>Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/ElitePlants/ElitePlantNeon</summary>
        public static readonly TextureAsset ElitePlantNeon = new("Aequus/Content/Biomes/MossBiomes/Tiles/ElitePlants/ElitePlantNeon");
        /// <summary>Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/ElitePlants/ElitePlantTile</summary>
        public static readonly TextureAsset ElitePlantTile = new("Aequus/Content/Biomes/MossBiomes/Tiles/ElitePlants/ElitePlantTile");
        /// <summary>Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/ElitePlants/ElitePlantXenon</summary>
        public static readonly TextureAsset ElitePlantXenon = new("Aequus/Content/Biomes/MossBiomes/Tiles/ElitePlants/ElitePlantXenon");
        /// <summary>Full Path: Aequus/Tiles/Blocks/EmancipationGrill</summary>
        public static readonly TextureAsset EmancipationGrill = new("Aequus/Tiles/Blocks/EmancipationGrill");
        /// <summary>Full Path: Aequus/Tiles/Blocks/EmancipationGrillTile</summary>
        public static readonly TextureAsset EmancipationGrillTile = new("Aequus/Tiles/Blocks/EmancipationGrillTile");
        /// <summary>Full Path: Aequus/Tiles/Misc/BigGems/EmeraldDeposit</summary>
        public static readonly TextureAsset EmeraldDeposit = new("Aequus/Tiles/Misc/BigGems/EmeraldDeposit");
        /// <summary>Full Path: Aequus/Assets/Gores/Gems/EmeraldGore</summary>
        public static readonly TextureAsset EmeraldGore = new("Aequus/Assets/Gores/Gems/EmeraldGore");
        /// <summary>Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredArchery</summary>
        public static readonly TextureAsset EmpoweredArchery = new("Aequus/Buffs/Misc/Empowered/EmpoweredArchery");
        /// <summary>Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredEndurance</summary>
        public static readonly TextureAsset EmpoweredEndurance = new("Aequus/Buffs/Misc/Empowered/EmpoweredEndurance");
        /// <summary>Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredFishing</summary>
        public static readonly TextureAsset EmpoweredFishing = new("Aequus/Buffs/Misc/Empowered/EmpoweredFishing");
        /// <summary>Full Path: Aequus/Content/ItemPrefixes/Potions/EmpoweredGlint</summary>
        public static readonly TextureAsset EmpoweredGlint = new("Aequus/Content/ItemPrefixes/Potions/EmpoweredGlint");
        /// <summary>Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredIronskin</summary>
        public static readonly TextureAsset EmpoweredIronskin = new("Aequus/Buffs/Misc/Empowered/EmpoweredIronskin");
        /// <summary>Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredMagicPower</summary>
        public static readonly TextureAsset EmpoweredMagicPower = new("Aequus/Buffs/Misc/Empowered/EmpoweredMagicPower");
        /// <summary>Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredManaRegeneration</summary>
        public static readonly TextureAsset EmpoweredManaRegeneration = new("Aequus/Buffs/Misc/Empowered/EmpoweredManaRegeneration");
        /// <summary>Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredMining</summary>
        public static readonly TextureAsset EmpoweredMining = new("Aequus/Buffs/Misc/Empowered/EmpoweredMining");
        /// <summary>Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredRage</summary>
        public static readonly TextureAsset EmpoweredRage = new("Aequus/Buffs/Misc/Empowered/EmpoweredRage");
        /// <summary>Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredRegeneration</summary>
        public static readonly TextureAsset EmpoweredRegeneration = new("Aequus/Buffs/Misc/Empowered/EmpoweredRegeneration");
        /// <summary>Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredShine</summary>
        public static readonly TextureAsset EmpoweredShine = new("Aequus/Buffs/Misc/Empowered/EmpoweredShine");
        /// <summary>Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredSummoning</summary>
        public static readonly TextureAsset EmpoweredSummoning = new("Aequus/Buffs/Misc/Empowered/EmpoweredSummoning");
        /// <summary>Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredSwiftness</summary>
        public static readonly TextureAsset EmpoweredSwiftness = new("Aequus/Buffs/Misc/Empowered/EmpoweredSwiftness");
        /// <summary>Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredThorns</summary>
        public static readonly TextureAsset EmpoweredThorns = new("Aequus/Buffs/Misc/Empowered/EmpoweredThorns");
        /// <summary>Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredWrath</summary>
        public static readonly TextureAsset EmpoweredWrath = new("Aequus/Buffs/Misc/Empowered/EmpoweredWrath");
        /// <summary>Full Path: Aequus/Items/Misc/Dyes/EnchantedDye</summary>
        public static readonly TextureAsset EnchantedDye = new("Aequus/Items/Misc/Dyes/EnchantedDye");
        /// <summary>Full Path: Aequus/Items/Misc/Dyes/EnchantedDyeEffect</summary>
        public static readonly TextureAsset EnchantedDyeEffect = new("Aequus/Items/Misc/Dyes/EnchantedDyeEffect");
        /// <summary>Full Path: Aequus/Particles/Dusts/EnergyDust</summary>
        public static readonly TextureAsset EnergyDust = new("Aequus/Particles/Dusts/EnergyDust");
        /// <summary>Full Path: Aequus/Unused/Items/DebugItems/EnthrallingScepter</summary>
        public static readonly TextureAsset EnthrallingScepter = new("Aequus/Unused/Items/DebugItems/EnthrallingScepter");
        /// <summary>Full Path: Aequus/Content/CrossMod/SplitSupport/Photography/EnvelopeGlimmer</summary>
        public static readonly TextureAsset EnvelopeGlimmer = new("Aequus/Content/CrossMod/SplitSupport/Photography/EnvelopeGlimmer");
        /// <summary>Full Path: Aequus/Content/CrossMod/SplitSupport/Photography/EnvelopeUndergroundOcean</summary>
        public static readonly TextureAsset EnvelopeUndergroundOcean = new("Aequus/Content/CrossMod/SplitSupport/Photography/EnvelopeUndergroundOcean");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Paintings/Items/ExLydSpacePainting</summary>
        public static readonly TextureAsset ExLydSpacePainting = new("Aequus/Tiles/Furniture/Paintings/Items/ExLydSpacePainting");
        /// <summary>Full Path: Aequus/Assets/Explosion0</summary>
        public static readonly TextureAsset Explosion0 = new("Aequus/Assets/Explosion0");
        /// <summary>Full Path: Aequus/Assets/Explosion1</summary>
        public static readonly TextureAsset Explosion1 = new("Aequus/Assets/Explosion1");
        /// <summary>Full Path: Aequus/NPCs/Town/ExporterNPC/Exporter</summary>
        public static readonly TextureAsset Exporter = new("Aequus/NPCs/Town/ExporterNPC/Exporter");
        /// <summary>Full Path: Aequus/Assets/Gores/Exporter_0</summary>
        public static readonly TextureAsset Exporter_0 = new("Aequus/Assets/Gores/Exporter_0");
        /// <summary>Full Path: Aequus/Assets/Gores/Exporter_1</summary>
        public static readonly TextureAsset Exporter_1 = new("Aequus/Assets/Gores/Exporter_1");
        /// <summary>Full Path: Aequus/Assets/Gores/Exporter_2</summary>
        public static readonly TextureAsset Exporter_2 = new("Aequus/Assets/Gores/Exporter_2");
        /// <summary>Full Path: Aequus/Assets/Gores/Exporter_3</summary>
        public static readonly TextureAsset Exporter_3 = new("Aequus/Assets/Gores/Exporter_3");
        /// <summary>Full Path: Aequus/Assets/Gores/Exporter_4</summary>
        public static readonly TextureAsset Exporter_4 = new("Aequus/Assets/Gores/Exporter_4");
        /// <summary>Full Path: Aequus/Assets/Gores/Exporter_5</summary>
        public static readonly TextureAsset Exporter_5 = new("Aequus/Assets/Gores/Exporter_5");
        /// <summary>Full Path: Aequus/NPCs/Town/ExporterNPC/Exporter_Head</summary>
        public static readonly TextureAsset Exporter_Head = new("Aequus/NPCs/Town/ExporterNPC/Exporter_Head");
        /// <summary>Full Path: Aequus/Items/Vanity/EyeGlint</summary>
        public static readonly TextureAsset EyeGlint = new("Aequus/Items/Vanity/EyeGlint");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/Familiar/FamiliarBuff</summary>
        public static readonly TextureAsset FamiliarBuff = new("Aequus/Items/Vanity/Pets/Familiar/FamiliarBuff");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/Familiar/FamiliarPickaxe</summary>
        public static readonly TextureAsset FamiliarPickaxe = new("Aequus/Items/Vanity/Pets/Familiar/FamiliarPickaxe");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/Money/FaultyCoin</summary>
        public static readonly TextureAsset FaultyCoin = new("Aequus/Items/Accessories/Misc/Money/FaultyCoin");
        /// <summary>Full Path: Aequus/Buffs/Misc/FaultyCoinBuff</summary>
        public static readonly TextureAsset FaultyCoinBuff = new("Aequus/Buffs/Misc/FaultyCoinBuff");
        /// <summary>Full Path: Aequus/Items/Consumables/Powders/FertilePowder</summary>
        public static readonly TextureAsset FertilePowder = new("Aequus/Items/Consumables/Powders/FertilePowder");
        /// <summary>Full Path: Aequus/Items/Consumables/Crates/FishingCratesTile</summary>
        public static readonly TextureAsset FishingCratesTile = new("Aequus/Items/Consumables/Crates/FishingCratesTile");
        /// <summary>Full Path: Aequus/Tiles/Misc/FishSign</summary>
        public static readonly TextureAsset FishSign = new("Aequus/Tiles/Misc/FishSign");
        /// <summary>Full Path: Aequus/Tiles/Misc/FishSignTile</summary>
        public static readonly TextureAsset FishSignTile = new("Aequus/Tiles/Misc/FishSignTile");
        /// <summary>Full Path: Aequus/Tiles/Misc/FishSignTile_Highlight</summary>
        public static readonly TextureAsset FishSignTile_Highlight = new("Aequus/Tiles/Misc/FishSignTile_Highlight");
        /// <summary>Full Path: Aequus/Items/Accessories/Life/Water/FishyFins</summary>
        public static readonly TextureAsset FishyFins = new("Aequus/Items/Accessories/Life/Water/FishyFins");
        /// <summary>Full Path: Aequus/Items/Accessories/Life/Water/FishyFins_Ears</summary>
        public static readonly TextureAsset FishyFins_Ears = new("Aequus/Items/Accessories/Life/Water/FishyFins_Ears");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/Flameblaster</summary>
        public static readonly TextureAsset Flameblaster = new("Aequus/Items/Weapons/Ranged/Flameblaster");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/Flameblaster_Glow</summary>
        public static readonly TextureAsset Flameblaster_Glow = new("Aequus/Items/Weapons/Ranged/Flameblaster_Glow");
        /// <summary>Full Path: Aequus/Projectiles/Ranged/FlameblasterProj</summary>
        public static readonly TextureAsset FlameblasterProj = new("Aequus/Projectiles/Ranged/FlameblasterProj");
        /// <summary>Full Path: Aequus/Projectiles/Ranged/FlameblasterWind</summary>
        public static readonly TextureAsset FlameblasterWind = new("Aequus/Projectiles/Ranged/FlameblasterWind");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/OnHit/Debuff/FlameCrystal</summary>
        public static readonly TextureAsset FlameCrystal = new("Aequus/Items/Accessories/Combat/OnHit/Debuff/FlameCrystal");
        /// <summary>Full Path: Aequus/Assets/Textures/Flare</summary>
        public static readonly TextureAsset Flare = new("Aequus/Assets/Textures/Flare");
        /// <summary>Full Path: Aequus/Items/Accessories/Life/Dodge/FlashwayNecklace</summary>
        public static readonly TextureAsset FlashwayNecklace = new("Aequus/Items/Accessories/Life/Dodge/FlashwayNecklace");
        /// <summary>Full Path: Aequus/Buffs/Cooldowns/FlashwayNecklaceCooldown</summary>
        public static readonly TextureAsset FlashwayNecklaceCooldown = new("Aequus/Buffs/Cooldowns/FlashwayNecklaceCooldown");
        /// <summary>Full Path: Aequus/Buffs/Misc/FlaskBuff</summary>
        public static readonly TextureAsset FlaskBuff = new("Aequus/Buffs/Misc/FlaskBuff");
        /// <summary>Full Path: Aequus/Items/Armor/FlowerCrown</summary>
        public static readonly TextureAsset FlowerCrown = new("Aequus/Items/Armor/FlowerCrown");
        /// <summary>Full Path: Aequus/Items/Armor/FlowerCrown_Head</summary>
        public static readonly TextureAsset FlowerCrown_Head = new("Aequus/Items/Armor/FlowerCrown_Head");
        /// <summary>Full Path: Aequus/Items/Materials/GaleStreams/Fluorescence</summary>
        public static readonly TextureAsset Fluorescence = new("Aequus/Items/Materials/GaleStreams/Fluorescence");
        /// <summary>Full Path: Aequus/Particles/FogParticle</summary>
        public static readonly TextureAsset FogParticle = new("Aequus/Particles/FogParticle");
        /// <summary>Full Path: Aequus/Particles/FogParticleHQ</summary>
        public static readonly TextureAsset FogParticleHQ = new("Aequus/Particles/FogParticleHQ");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/Money/FoolsGoldRing</summary>
        public static readonly TextureAsset FoolsGoldRing = new("Aequus/Items/Accessories/Misc/Money/FoolsGoldRing");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/Money/FoolsGoldRing_HandsOn</summary>
        public static readonly TextureAsset FoolsGoldRing_HandsOn = new("Aequus/Items/Accessories/Misc/Money/FoolsGoldRing_HandsOn");
        /// <summary>Full Path: Aequus/Buffs/Misc/FoolsGoldRingBuff</summary>
        public static readonly TextureAsset FoolsGoldRingBuff = new("Aequus/Buffs/Misc/FoolsGoldRingBuff");
        /// <summary>Full Path: Aequus/NPCs/Town/CarpenterNPC/Quest/Bounties/Icons/FountainBounty</summary>
        public static readonly TextureAsset FountainBounty = new("Aequus/NPCs/Town/CarpenterNPC/Quest/Bounties/Icons/FountainBounty");
        /// <summary>Full Path: Aequus/Content/Building/Bonuses/FountainBountyBuff</summary>
        public static readonly TextureAsset FountainBountyBuff = new("Aequus/Content/Building/Bonuses/FountainBountyBuff");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/Misc/FriendshipMagick</summary>
        public static readonly TextureAsset FriendshipMagick = new("Aequus/Items/Weapons/Magic/Misc/FriendshipMagick");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/Misc/FriendshipMagick_Glow</summary>
        public static readonly TextureAsset FriendshipMagick_Glow = new("Aequus/Items/Weapons/Magic/Misc/FriendshipMagick_Glow");
        /// <summary>Full Path: Aequus/Projectiles/Misc/Friendly/FriendshipMagickProj</summary>
        public static readonly TextureAsset FriendshipMagickProj = new("Aequus/Projectiles/Misc/Friendly/FriendshipMagickProj");
        /// <summary>Full Path: Aequus/Projectiles/Misc/Friendly/FriendshipMagickProj_Aura</summary>
        public static readonly TextureAsset FriendshipMagickProj_Aura = new("Aequus/Projectiles/Misc/Friendly/FriendshipMagickProj_Aura");
        /// <summary>Full Path: Aequus/Assets/Textures/FriendshipParticle</summary>
        public static readonly TextureAsset FriendshipParticle = new("Aequus/Assets/Textures/FriendshipParticle");
        /// <summary>Full Path: Aequus/Items/Misc/Dyes/FrostbiteDye</summary>
        public static readonly TextureAsset FrostbiteDye = new("Aequus/Items/Misc/Dyes/FrostbiteDye");
        /// <summary>Full Path: Aequus/Items/Misc/Dyes/FrostbiteDyeEffect</summary>
        public static readonly TextureAsset FrostbiteDyeEffect = new("Aequus/Items/Misc/Dyes/FrostbiteDyeEffect");
        /// <summary>Full Path: Aequus/Buffs/FrostBuff</summary>
        public static readonly TextureAsset FrostBuff = new("Aequus/Buffs/FrostBuff");
        /// <summary>Full Path: Aequus/NPCs/Monsters/FrostMimic</summary>
        public static readonly TextureAsset FrostMimic = new("Aequus/NPCs/Monsters/FrostMimic");
        /// <summary>Full Path: Aequus/Items/Potions/FrostPotion</summary>
        public static readonly TextureAsset FrostPotion = new("Aequus/Items/Potions/FrostPotion");
        /// <summary>Full Path: Aequus/Items/Materials/GaleStreams/FrozenTear</summary>
        public static readonly TextureAsset FrozenTear = new("Aequus/Items/Materials/GaleStreams/FrozenTear");
        /// <summary>Full Path: Aequus/Content/Events/GlimmerEvent/Misc/GalacticStarfruit</summary>
        public static readonly TextureAsset GalacticStarfruit = new("Aequus/Content/Events/GlimmerEvent/Misc/GalacticStarfruit");
        /// <summary>Full Path: Aequus/Unused/Items/GalaxyCommission</summary>
        public static readonly TextureAsset GalaxyCommission = new("Aequus/Unused/Items/GalaxyCommission");
        /// <summary>Full Path: Aequus/Assets/UI/BestiaryIcons/GaleStreams</summary>
        public static readonly TextureAsset GaleStreams_BestiaryIcons = new("Aequus/Assets/UI/BestiaryIcons/GaleStreams");
        /// <summary>Full Path: Aequus/Assets/UI/BossChecklist/GaleStreams</summary>
        public static readonly TextureAsset GaleStreams_BossChecklist = new("Aequus/Assets/UI/BossChecklist/GaleStreams");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/Gamestar</summary>
        public static readonly TextureAsset Gamestar = new("Aequus/Items/Weapons/Magic/Gamestar");
        /// <summary>Full Path: Aequus/Assets/Particles/GamestarParticle</summary>
        public static readonly TextureAsset GamestarParticle = new("Aequus/Assets/Particles/GamestarParticle");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/GhastlyBlaster</summary>
        public static readonly TextureAsset GhastlyBlaster = new("Aequus/Items/Weapons/Magic/GhastlyBlaster");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/GhastlyBlaster_Glow</summary>
        public static readonly TextureAsset GhastlyBlaster_Glow = new("Aequus/Items/Weapons/Magic/GhastlyBlaster_Glow");
        /// <summary>Full Path: Aequus/Projectiles/Magic/GhastlyBlasterProj</summary>
        public static readonly TextureAsset GhastlyBlasterProj = new("Aequus/Projectiles/Magic/GhastlyBlasterProj");
        /// <summary>Full Path: Aequus/Projectiles/Magic/GhastlyBlasterProj_Glow</summary>
        public static readonly TextureAsset GhastlyBlasterProj_Glow = new("Aequus/Projectiles/Magic/GhastlyBlasterProj_Glow");
        /// <summary>Full Path: Aequus/Projectiles/Magic/GhastlyBlasterProjLaser</summary>
        public static readonly TextureAsset GhastlyBlasterProjLaser = new("Aequus/Projectiles/Magic/GhastlyBlasterProjLaser");
        /// <summary>Full Path: Aequus/Particles/Dusts/GhostDrainDust</summary>
        public static readonly TextureAsset GhostDrainDust = new("Aequus/Particles/Dusts/GhostDrainDust");
        /// <summary>Full Path: Aequus/Items/Tools/GhostlyGrave</summary>
        public static readonly TextureAsset GhostlyGrave = new("Aequus/Items/Tools/GhostlyGrave");
        /// <summary>Full Path: Aequus/Items/Vanity/GiftingSpirit</summary>
        public static readonly TextureAsset GiftingSpirit = new("Aequus/Items/Vanity/GiftingSpirit");
        /// <summary>Full Path: Aequus/Tiles/Misc/Plants/GildedFlower</summary>
        public static readonly TextureAsset GildedFlower = new("Aequus/Tiles/Misc/Plants/GildedFlower");
        /// <summary>Full Path: Aequus/Assets/UI/BestiaryIcons/Glimmer</summary>
        public static readonly TextureAsset Glimmer_BestiaryIcons = new("Aequus/Assets/UI/BestiaryIcons/Glimmer");
        /// <summary>Full Path: Aequus/Assets/UI/BossChecklist/Glimmer</summary>
        public static readonly TextureAsset Glimmer_BossChecklist = new("Aequus/Assets/UI/BossChecklist/Glimmer");
        /// <summary>Full Path: Aequus/Assets/UI/EventIcons/Glimmer</summary>
        public static readonly TextureAsset Glimmer_EventIcons = new("Aequus/Assets/UI/EventIcons/Glimmer");
        /// <summary>Full Path: Aequus/Assets/UI/MapBGs/Glimmer</summary>
        public static readonly TextureAsset Glimmer_MapBGs = new("Aequus/Assets/UI/MapBGs/Glimmer");
        /// <summary>Full Path: Aequus/Content/MainMenu/GlimmerMenu</summary>
        public static readonly TextureAsset GlimmerMenu = new("Aequus/Content/MainMenu/GlimmerMenu");
        /// <summary>Full Path: Aequus/Content/Events/GlimmerEvent/Sky/GlimmerSky</summary>
        public static readonly TextureAsset GlimmerSky = new("Aequus/Content/Events/GlimmerEvent/Sky/GlimmerSky");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/GlowCore</summary>
        public static readonly TextureAsset GlowCore = new("Aequus/Items/Accessories/Misc/GlowCore");
        /// <summary>Full Path: Aequus/Items/Materials/GlowLichen</summary>
        public static readonly TextureAsset GlowLichen = new("Aequus/Items/Materials/GlowLichen");
        /// <summary>Full Path: Aequus/Tiles/Misc/Plants/GoblinFlower</summary>
        public static readonly TextureAsset GoblinFlower = new("Aequus/Tiles/Misc/Plants/GoblinFlower");
        /// <summary>Full Path: Aequus/Unused/Items/SlotMachines/GoldenRoulette</summary>
        public static readonly TextureAsset GoldenRoulette = new("Aequus/Unused/Items/SlotMachines/GoldenRoulette");
        /// <summary>Full Path: Aequus/Items/Misc/LegendaryFish/GoreFish</summary>
        public static readonly TextureAsset GoreFish = new("Aequus/Items/Misc/LegendaryFish/GoreFish");
        /// <summary>Full Path: Aequus/Content/Biomes/GoreNest/Tiles/GoreNest</summary>
        public static readonly TextureAsset GoreNest = new("Aequus/Content/Biomes/GoreNest/Tiles/GoreNest");
        /// <summary>Full Path: Aequus/Content/Events/DemonSiege/GoreNestAura</summary>
        public static readonly TextureAsset GoreNestAura = new("Aequus/Content/Events/DemonSiege/GoreNestAura");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Paintings/Items/GoreNestPainting</summary>
        public static readonly TextureAsset GoreNestPainting = new("Aequus/Tiles/Furniture/Paintings/Items/GoreNestPainting");
        /// <summary>Full Path: Aequus/Content/Biomes/GoreNest/Tiles/GoreNestStalagmite</summary>
        public static readonly TextureAsset GoreNestStalagmite = new("Aequus/Content/Biomes/GoreNest/Tiles/GoreNestStalagmite");
        /// <summary>Full Path: Aequus/Content/Biomes/GoreNest/Tiles/GoreNestTile</summary>
        public static readonly TextureAsset GoreNestTile = new("Aequus/Content/Biomes/GoreNest/Tiles/GoreNestTile");
        /// <summary>Full Path: Aequus/Content/Biomes/GoreNest/Tiles/GoreNestTile_Highlight</summary>
        public static readonly TextureAsset GoreNestTile_Highlight = new("Aequus/Content/Biomes/GoreNest/Tiles/GoreNestTile_Highlight");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/Luck/GrandReward</summary>
        public static readonly TextureAsset GrandReward = new("Aequus/Items/Accessories/Misc/Luck/GrandReward");
        /// <summary>Full Path: Aequus/Items/Armor/SetGravetender/GravetenderHood</summary>
        public static readonly TextureAsset GravetenderHood = new("Aequus/Items/Armor/SetGravetender/GravetenderHood");
        /// <summary>Full Path: Aequus/Items/Armor/SetGravetender/GravetenderHood_Head</summary>
        public static readonly TextureAsset GravetenderHood_Head = new("Aequus/Items/Armor/SetGravetender/GravetenderHood_Head");
        /// <summary>Full Path: Aequus/Buffs/Minion/GravetenderMinionBuff</summary>
        public static readonly TextureAsset GravetenderMinionBuff = new("Aequus/Buffs/Minion/GravetenderMinionBuff");
        /// <summary>Full Path: Aequus/Items/Armor/SetGravetender/GravetenderRobes</summary>
        public static readonly TextureAsset GravetenderRobes = new("Aequus/Items/Armor/SetGravetender/GravetenderRobes");
        /// <summary>Full Path: Aequus/Items/Armor/SetGravetender/GravetenderRobes_Body</summary>
        public static readonly TextureAsset GravetenderRobes_Body = new("Aequus/Items/Armor/SetGravetender/GravetenderRobes_Body");
        /// <summary>Full Path: Aequus/Tiles/Blocks/GravityBlocks/Auras/GravityAura_0</summary>
        public static readonly TextureAsset GravityAura_0 = new("Aequus/Tiles/Blocks/GravityBlocks/Auras/GravityAura_0");
        /// <summary>Full Path: Aequus/Tiles/Blocks/GravityBlocks/Auras/GravityAura_1</summary>
        public static readonly TextureAsset GravityAura_1 = new("Aequus/Tiles/Blocks/GravityBlocks/Auras/GravityAura_1");
        /// <summary>Full Path: Aequus/Tiles/Blocks/GravityBlocks/GravityBlock</summary>
        public static readonly TextureAsset GravityBlock = new("Aequus/Tiles/Blocks/GravityBlocks/GravityBlock");
        /// <summary>Full Path: Aequus/Tiles/Blocks/GravityBlocks/GravityBlockTile</summary>
        public static readonly TextureAsset GravityBlockTile = new("Aequus/Tiles/Blocks/GravityBlocks/GravityBlockTile");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Gravity/GravityCandelabra</summary>
        public static readonly TextureAsset GravityCandelabra = new("Aequus/Tiles/Furniture/Gravity/GravityCandelabra");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Gravity/GravityCandle</summary>
        public static readonly TextureAsset GravityCandle = new("Aequus/Tiles/Furniture/Gravity/GravityCandle");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Gravity/GravityChair</summary>
        public static readonly TextureAsset GravityChair = new("Aequus/Tiles/Furniture/Gravity/GravityChair");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Gravity/GravityChest</summary>
        public static readonly TextureAsset GravityChest = new("Aequus/Tiles/Furniture/Gravity/GravityChest");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Gravity/GravityChestTile</summary>
        public static readonly TextureAsset GravityChestTile = new("Aequus/Tiles/Furniture/Gravity/GravityChestTile");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Gravity/GravityChestTile_Glow</summary>
        public static readonly TextureAsset GravityChestTile_Glow = new("Aequus/Tiles/Furniture/Gravity/GravityChestTile_Glow");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Gravity/GravityChestTile_Highlight</summary>
        public static readonly TextureAsset GravityChestTile_Highlight = new("Aequus/Tiles/Furniture/Gravity/GravityChestTile_Highlight");
        /// <summary>Full Path: Aequus/Tiles/Blocks/GravityBlocks/Auras/GravityDust</summary>
        public static readonly TextureAsset GravityDust = new("Aequus/Tiles/Blocks/GravityBlocks/Auras/GravityDust");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Gravity/GravityLamp</summary>
        public static readonly TextureAsset GravityLamp = new("Aequus/Tiles/Furniture/Gravity/GravityLamp");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Gravity/GravityPiano</summary>
        public static readonly TextureAsset GravityPiano = new("Aequus/Tiles/Furniture/Gravity/GravityPiano");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Gravity/GravitySofa</summary>
        public static readonly TextureAsset GravitySofa = new("Aequus/Tiles/Furniture/Gravity/GravitySofa");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Gravity/GravityTable</summary>
        public static readonly TextureAsset GravityTable = new("Aequus/Tiles/Furniture/Gravity/GravityTable");
        /// <summary>Full Path: Aequus/Content/DronePylons/NPCs/GunnerDrone</summary>
        public static readonly TextureAsset GunnerDrone = new("Aequus/Content/DronePylons/NPCs/GunnerDrone");
        /// <summary>Full Path: Aequus/Content/DronePylons/NPCs/GunnerDrone_Extras</summary>
        public static readonly TextureAsset GunnerDrone_Extras = new("Aequus/Content/DronePylons/NPCs/GunnerDrone_Extras");
        /// <summary>Full Path: Aequus/Content/DronePylons/NPCs/GunnerDrone_Extras_Glow</summary>
        public static readonly TextureAsset GunnerDrone_Extras_Glow = new("Aequus/Content/DronePylons/NPCs/GunnerDrone_Extras_Glow");
        /// <summary>Full Path: Aequus/Content/DronePylons/NPCs/GunnerDrone_Glow</summary>
        public static readonly TextureAsset GunnerDrone_Glow = new("Aequus/Content/DronePylons/NPCs/GunnerDrone_Glow");
        /// <summary>Full Path: Aequus/Items/Misc/Dyes/GustDye</summary>
        public static readonly TextureAsset GustDye = new("Aequus/Items/Misc/Dyes/GustDye");
        /// <summary>Full Path: Aequus/Items/Weapons/Summon/Minion/HailstormStaff</summary>
        public static readonly TextureAsset HailstormStaff = new("Aequus/Items/Weapons/Summon/Minion/HailstormStaff");
        /// <summary>Full Path: Aequus/Unused/Items/HalloweenEnergy</summary>
        public static readonly TextureAsset HalloweenEnergy = new("Aequus/Unused/Items/HalloweenEnergy");
        /// <summary>Full Path: Aequus/Assets/Effects/Textures/HalloweenEnergyGradient</summary>
        public static readonly TextureAsset HalloweenEnergyGradient = new("Aequus/Assets/Effects/Textures/HalloweenEnergyGradient");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/ItemReach/HaltingMachine</summary>
        public static readonly TextureAsset HaltingMachine = new("Aequus/Items/Accessories/Misc/ItemReach/HaltingMachine");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/ItemReach/HaltingMagnet</summary>
        public static readonly TextureAsset HaltingMagnet = new("Aequus/Items/Accessories/Misc/ItemReach/HaltingMagnet");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/Demon/HamaYumi</summary>
        public static readonly TextureAsset HamaYumi = new("Aequus/Items/Weapons/Ranged/Demon/HamaYumi");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/Demon/HamaYumi_Glow</summary>
        public static readonly TextureAsset HamaYumi_Glow = new("Aequus/Items/Weapons/Ranged/Demon/HamaYumi_Glow");
        /// <summary>Full Path: Aequus/Projectiles/Ranged/HamaYumiArrow</summary>
        public static readonly TextureAsset HamaYumiArrow = new("Aequus/Projectiles/Ranged/HamaYumiArrow");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/HardFrozenChest</summary>
        public static readonly TextureAsset HardFrozenChest = new("Aequus/Tiles/Misc/HardmodeChests/HardFrozenChest");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/HardFrozenChestTile</summary>
        public static readonly TextureAsset HardFrozenChestTile = new("Aequus/Tiles/Misc/HardmodeChests/HardFrozenChestTile");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/HardFrozenChestTile_Highlight</summary>
        public static readonly TextureAsset HardFrozenChestTile_Highlight = new("Aequus/Tiles/Misc/HardmodeChests/HardFrozenChestTile_Highlight");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/HardGraniteChest</summary>
        public static readonly TextureAsset HardGraniteChest = new("Aequus/Tiles/Misc/HardmodeChests/HardGraniteChest");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/HardGraniteChestTile</summary>
        public static readonly TextureAsset HardGraniteChestTile = new("Aequus/Tiles/Misc/HardmodeChests/HardGraniteChestTile");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/HardGraniteChestTile_Glow</summary>
        public static readonly TextureAsset HardGraniteChestTile_Glow = new("Aequus/Tiles/Misc/HardmodeChests/HardGraniteChestTile_Glow");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/HardGraniteChestTile_Highlight</summary>
        public static readonly TextureAsset HardGraniteChestTile_Highlight = new("Aequus/Tiles/Misc/HardmodeChests/HardGraniteChestTile_Highlight");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/HardJungleChest</summary>
        public static readonly TextureAsset HardJungleChest = new("Aequus/Tiles/Misc/HardmodeChests/HardJungleChest");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/HardJungleChestTile</summary>
        public static readonly TextureAsset HardJungleChestTile = new("Aequus/Tiles/Misc/HardmodeChests/HardJungleChestTile");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/HardJungleChestTile_Highlight</summary>
        public static readonly TextureAsset HardJungleChestTile_Highlight = new("Aequus/Tiles/Misc/HardmodeChests/HardJungleChestTile_Highlight");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/HardMarbleChest</summary>
        public static readonly TextureAsset HardMarbleChest = new("Aequus/Tiles/Misc/HardmodeChests/HardMarbleChest");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/HardMarbleChestTile</summary>
        public static readonly TextureAsset HardMarbleChestTile = new("Aequus/Tiles/Misc/HardmodeChests/HardMarbleChestTile");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/HardMarbleChestTile_Highlight</summary>
        public static readonly TextureAsset HardMarbleChestTile_Highlight = new("Aequus/Tiles/Misc/HardmodeChests/HardMarbleChestTile_Highlight");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/HardMushroomChest</summary>
        public static readonly TextureAsset HardMushroomChest = new("Aequus/Tiles/Misc/HardmodeChests/HardMushroomChest");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/HardMushroomChestTile</summary>
        public static readonly TextureAsset HardMushroomChestTile = new("Aequus/Tiles/Misc/HardmodeChests/HardMushroomChestTile");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/HardMushroomChestTile_Glow</summary>
        public static readonly TextureAsset HardMushroomChestTile_Glow = new("Aequus/Tiles/Misc/HardmodeChests/HardMushroomChestTile_Glow");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/HardMushroomChestTile_Highlight</summary>
        public static readonly TextureAsset HardMushroomChestTile_Highlight = new("Aequus/Tiles/Misc/HardmodeChests/HardMushroomChestTile_Highlight");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/HardSandstoneChest</summary>
        public static readonly TextureAsset HardSandstoneChest = new("Aequus/Tiles/Misc/HardmodeChests/HardSandstoneChest");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/HardSandstoneChestTile</summary>
        public static readonly TextureAsset HardSandstoneChestTile = new("Aequus/Tiles/Misc/HardmodeChests/HardSandstoneChestTile");
        /// <summary>Full Path: Aequus/Tiles/Misc/HardmodeChests/HardSandstoneChestTile_Highlight</summary>
        public static readonly TextureAsset HardSandstoneChestTile_Highlight = new("Aequus/Tiles/Misc/HardmodeChests/HardSandstoneChestTile_Highlight");
        /// <summary>Full Path: Aequus/Projectiles/Monster/HardwoodProj</summary>
        public static readonly TextureAsset HardwoodProj = new("Aequus/Projectiles/Monster/HardwoodProj");
        /// <summary>Full Path: Aequus/Items/Vanity/Headless</summary>
        public static readonly TextureAsset Headless = new("Aequus/Items/Vanity/Headless");
        /// <summary>Full Path: Aequus/Items/Vanity/Headless_Head</summary>
        public static readonly TextureAsset Headless_Head = new("Aequus/Items/Vanity/Headless_Head");
        /// <summary>Full Path: Aequus/Content/DronePylons/NPCs/HealerDrone</summary>
        public static readonly TextureAsset HealerDrone = new("Aequus/Content/DronePylons/NPCs/HealerDrone");
        /// <summary>Full Path: Aequus/Content/DronePylons/NPCs/HealerDrone_Glow</summary>
        public static readonly TextureAsset HealerDrone_Glow = new("Aequus/Content/DronePylons/NPCs/HealerDrone_Glow");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/HealthCursor</summary>
        public static readonly TextureAsset HealthCursor = new("Aequus/Content/CursorDyes/Items/HealthCursor");
        /// <summary>Full Path: Aequus/Items/Accessories/CrownOfBlood/Hearts/Heart</summary>
        public static readonly TextureAsset Heart = new("Aequus/Items/Accessories/CrownOfBlood/Hearts/Heart");
        /// <summary>Full Path: Aequus/Items/Accessories/CrownOfBlood/Hearts/Heart_Template</summary>
        public static readonly TextureAsset Heart_Template = new("Aequus/Items/Accessories/CrownOfBlood/Hearts/Heart_Template");
        /// <summary>Full Path: Aequus/Items/Accessories/CrownOfBlood/Hearts/Heart2</summary>
        public static readonly TextureAsset Heart2 = new("Aequus/Items/Accessories/CrownOfBlood/Hearts/Heart2");
        /// <summary>Full Path: Aequus/Items/Accessories/Life/Dodge/HeartshatterNecklace</summary>
        public static readonly TextureAsset HeartshatterNecklace = new("Aequus/Items/Accessories/Life/Dodge/HeartshatterNecklace");
        /// <summary>Full Path: Aequus/NPCs/Monsters/Heckto</summary>
        public static readonly TextureAsset Heckto = new("Aequus/NPCs/Monsters/Heckto");
        /// <summary>Full Path: Aequus/Unused/Items/Heliosis</summary>
        public static readonly TextureAsset Heliosis = new("Aequus/Unused/Items/Heliosis");
        /// <summary>Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/HeliumPlant</summary>
        public static readonly TextureAsset HeliumPlant = new("Aequus/Content/Biomes/MossBiomes/Tiles/HeliumPlant");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/Demon/HellsBoon</summary>
        public static readonly TextureAsset HellsBoon = new("Aequus/Items/Weapons/Melee/Demon/HellsBoon");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/Demon/HellsBoon_Glow</summary>
        public static readonly TextureAsset HellsBoon_Glow = new("Aequus/Items/Weapons/Melee/Demon/HellsBoon_Glow");
        /// <summary>Full Path: Aequus/Projectiles/Melee/HellsBoonProj</summary>
        public static readonly TextureAsset HellsBoonProj = new("Aequus/Projectiles/Melee/HellsBoonProj");
        /// <summary>Full Path: Aequus/Items/Materials/Hexoplasm</summary>
        public static readonly TextureAsset Hexoplasm = new("Aequus/Items/Materials/Hexoplasm");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/OnHit/CriticalStrike/HighSteaks</summary>
        public static readonly TextureAsset HighSteaks = new("Aequus/Items/Accessories/Combat/OnHit/CriticalStrike/HighSteaks");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/OnHit/CriticalStrike/HighSteaks_Waist</summary>
        public static readonly TextureAsset HighSteaks_Waist = new("Aequus/Items/Accessories/Combat/OnHit/CriticalStrike/HighSteaks_Waist");
        /// <summary>Full Path: Aequus/Tiles/Banners/Items/HijivarchCrabBanner</summary>
        public static readonly TextureAsset HijivarchCrabBanner = new("Aequus/Tiles/Banners/Items/HijivarchCrabBanner");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/Hitscanner</summary>
        public static readonly TextureAsset Hitscanner = new("Aequus/Items/Weapons/Ranged/Hitscanner");
        /// <summary>Full Path: Aequus/Items/Accessories/CrownOfBlood/Projectiles/HivePackMinion</summary>
        public static readonly TextureAsset HivePackMinion = new("Aequus/Items/Accessories/CrownOfBlood/Projectiles/HivePackMinion");
        /// <summary>Full Path: Aequus/Items/Consumables/Foods/HolographicMeatloaf</summary>
        public static readonly TextureAsset HolographicMeatloaf = new("Aequus/Items/Consumables/Foods/HolographicMeatloaf");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/Info/HoloLens</summary>
        public static readonly TextureAsset HoloLens = new("Aequus/Items/Accessories/Misc/Info/HoloLens");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Paintings/Items/HomeworldPainting</summary>
        public static readonly TextureAsset HomeworldPainting = new("Aequus/Tiles/Furniture/Paintings/Items/HomeworldPainting");
        /// <summary>Full Path: Aequus/Buffs/Mounts/HotAirBalloonBuff</summary>
        public static readonly TextureAsset HotAirBalloonBuff = new("Aequus/Buffs/Mounts/HotAirBalloonBuff");
        /// <summary>Full Path: Aequus/Items/Misc/Mounts/HotAirBalloonMount</summary>
        public static readonly TextureAsset HotAirBalloonMount = new("Aequus/Items/Misc/Mounts/HotAirBalloonMount");
        /// <summary>Full Path: Aequus/Items/Misc/Mounts/HotAirBalloonMount_Back</summary>
        public static readonly TextureAsset HotAirBalloonMount_Back = new("Aequus/Items/Misc/Mounts/HotAirBalloonMount_Back");
        /// <summary>Full Path: Aequus/Items/Misc/Mounts/HotAirBalloonMount_Front</summary>
        public static readonly TextureAsset HotAirBalloonMount_Front = new("Aequus/Items/Misc/Mounts/HotAirBalloonMount_Front");
        /// <summary>Full Path: Aequus/Items/Misc/Dyes/HueshiftDye</summary>
        public static readonly TextureAsset HueshiftDye = new("Aequus/Items/Misc/Dyes/HueshiftDye");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/HyperCrystal</summary>
        public static readonly TextureAsset HyperCrystal = new("Aequus/Items/Accessories/Combat/HyperCrystal");
        /// <summary>Full Path: Aequus/Projectiles/Misc/Friendly/HyperCrystalProj</summary>
        public static readonly TextureAsset HyperCrystalProj = new("Aequus/Projectiles/Misc/Friendly/HyperCrystalProj");
        /// <summary>Full Path: Aequus/Items/Accessories/Life/HyperJet</summary>
        public static readonly TextureAsset HyperJet = new("Aequus/Items/Accessories/Life/HyperJet");
        /// <summary>Full Path: Aequus/NPCs/Monsters/Event/Glimmer/HyperStarite</summary>
        public static readonly TextureAsset HyperStarite = new("Aequus/NPCs/Monsters/Event/Glimmer/HyperStarite");
        /// <summary>Full Path: Aequus/Tiles/Banners/Items/HyperStariteBanner</summary>
        public static readonly TextureAsset HyperStariteBanner = new("Aequus/Tiles/Banners/Items/HyperStariteBanner");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Crabson/Misc/HypnoticPearl</summary>
        public static readonly TextureAsset HypnoticPearl = new("Aequus/NPCs/BossMonsters/Crabson/Misc/HypnoticPearl");
        /// <summary>Full Path: Aequus/Items/Materials/Fish/IcebergFish</summary>
        public static readonly TextureAsset IcebergFish = new("Aequus/Items/Materials/Fish/IcebergFish");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Sentry/IcebergKraken</summary>
        public static readonly TextureAsset IcebergKraken = new("Aequus/Items/Accessories/Combat/Sentry/IcebergKraken");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Sentry/IcebergKraken_Hat</summary>
        public static readonly TextureAsset IcebergKraken_Hat = new("Aequus/Items/Accessories/Combat/Sentry/IcebergKraken_Hat");
        /// <summary>Full Path: Aequus/Items/Misc/Bait/IchorPopper</summary>
        public static readonly TextureAsset IchorPopper = new("Aequus/Items/Misc/Bait/IchorPopper");
        /// <summary>Full Path: Aequus/icon</summary>
        public static readonly TextureAsset icon = new("Aequus/icon");
        /// <summary>Full Path: Aequus/icon_small</summary>
        public static readonly TextureAsset icon_small = new("Aequus/icon_small");
        /// <summary>Full Path: Aequus/icon_workshop</summary>
        public static readonly TextureAsset icon_workshop = new("Aequus/icon_workshop");
        /// <summary>Full Path: Aequus/Content/CrossMod/SplitSupport/Photography/Icons</summary>
        public static readonly TextureAsset Icons = new("Aequus/Content/CrossMod/SplitSupport/Photography/Icons");
        /// <summary>Full Path: Aequus/Unused/Items/ImpenetrableCoating</summary>
        public static readonly TextureAsset ImpenetrableCoating = new("Aequus/Unused/Items/ImpenetrableCoating");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/InspirationCursor</summary>
        public static readonly TextureAsset InspirationCursor = new("Aequus/Content/CursorDyes/Items/InspirationCursor");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Scepters/Insurgency</summary>
        public static readonly TextureAsset Insurgency = new("Aequus/Items/Weapons/Necromancy/Scepters/Insurgency");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Scepters/Insurgency_Glow</summary>
        public static readonly TextureAsset Insurgency_Glow = new("Aequus/Items/Weapons/Necromancy/Scepters/Insurgency_Glow");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Paintings/Items/InsurgentPainting</summary>
        public static readonly TextureAsset InsurgentPainting = new("Aequus/Tiles/Furniture/Paintings/Items/InsurgentPainting");
        /// <summary>Full Path: Aequus/Items/Accessories/CrownOfBlood/InventoryBack</summary>
        public static readonly TextureAsset InventoryBack_CrownOfBlood = new("Aequus/Items/Accessories/CrownOfBlood/InventoryBack");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Sentry/EquipmentChips/InventoryBack</summary>
        public static readonly TextureAsset InventoryBack_EquipmentChips = new("Aequus/Items/Accessories/Combat/Sentry/EquipmentChips/InventoryBack");
        /// <summary>Full Path: Aequus/Assets/UI/InventoryBack</summary>
        public static readonly TextureAsset InventoryBack_UI = new("Aequus/Assets/UI/InventoryBack");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/Heavy/IronLotus</summary>
        public static readonly TextureAsset IronLotus = new("Aequus/Items/Weapons/Melee/Heavy/IronLotus");
        /// <summary>Full Path: Aequus/Projectiles/Melee/IronLotusProj</summary>
        public static readonly TextureAsset IronLotusProj = new("Aequus/Projectiles/Melee/IronLotusProj");
        /// <summary>Full Path: Aequus/Unused/Items/ItemScrap</summary>
        public static readonly TextureAsset ItemScrap = new("Aequus/Unused/Items/ItemScrap");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Jeweled/JeweledCandelabra</summary>
        public static readonly TextureAsset JeweledCandelabra = new("Aequus/Tiles/Furniture/Jeweled/JeweledCandelabra");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Jeweled/JeweledCandelabraTile</summary>
        public static readonly TextureAsset JeweledCandelabraTile = new("Aequus/Tiles/Furniture/Jeweled/JeweledCandelabraTile");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Jeweled/JeweledCandelabraTile_Flame</summary>
        public static readonly TextureAsset JeweledCandelabraTile_Flame = new("Aequus/Tiles/Furniture/Jeweled/JeweledCandelabraTile_Flame");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Jeweled/JeweledChalice</summary>
        public static readonly TextureAsset JeweledChalice = new("Aequus/Tiles/Furniture/Jeweled/JeweledChalice");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Jeweled/JeweledChaliceTile</summary>
        public static readonly TextureAsset JeweledChaliceTile = new("Aequus/Tiles/Furniture/Jeweled/JeweledChaliceTile");
        /// <summary>Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/ArgonJumpshroom/JumpMushroom</summary>
        public static readonly TextureAsset JumpMushroom = new("Aequus/Content/Biomes/MossBiomes/Tiles/ArgonJumpshroom/JumpMushroom");
        /// <summary>Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/ArgonJumpshroom/JumpMushroomTile</summary>
        public static readonly TextureAsset JumpMushroomTile = new("Aequus/Content/Biomes/MossBiomes/Tiles/ArgonJumpshroom/JumpMushroomTile");
        /// <summary>Full Path: Aequus/Assets/UI/BestiaryIcons/JungleEvent</summary>
        public static readonly TextureAsset JungleEvent = new("Aequus/Assets/UI/BestiaryIcons/JungleEvent");
        /// <summary>Full Path: Aequus/Unused/Items/SlotMachines/JungleSlotMachine</summary>
        public static readonly TextureAsset JungleSlotMachine = new("Aequus/Unused/Items/SlotMachines/JungleSlotMachine");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/Misc/JunkJet</summary>
        public static readonly TextureAsset JunkJet = new("Aequus/Items/Weapons/Ranged/Misc/JunkJet");
        /// <summary>Full Path: Aequus/Items/Misc/LegendaryFish/KryptonFish</summary>
        public static readonly TextureAsset KryptonFish = new("Aequus/Items/Misc/LegendaryFish/KryptonFish");
        /// <summary>Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/KryptonHealPlant</summary>
        public static readonly TextureAsset KryptonHealPlant = new("Aequus/Content/Biomes/MossBiomes/Tiles/KryptonHealPlant");
        /// <summary>Full Path: Aequus/Content/Elites/Misc/KryptonShield</summary>
        public static readonly TextureAsset KryptonShield = new("Aequus/Content/Elites/Misc/KryptonShield");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/LanternCat/LanternCatBuff</summary>
        public static readonly TextureAsset LanternCatBuff = new("Aequus/Items/Vanity/Pets/LanternCat/LanternCatBuff");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/LanternCat/LanternCatPet</summary>
        public static readonly TextureAsset LanternCatPet = new("Aequus/Items/Vanity/Pets/LanternCat/LanternCatPet");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/LanternCat/LanternCatPet_Lantern</summary>
        public static readonly TextureAsset LanternCatPet_Lantern = new("Aequus/Items/Vanity/Pets/LanternCat/LanternCatPet_Lantern");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/LanternCat/LanternCatSpawner</summary>
        public static readonly TextureAsset LanternCatSpawner = new("Aequus/Items/Vanity/Pets/LanternCat/LanternCatSpawner");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Ranged/LaserReticle</summary>
        public static readonly TextureAsset LaserReticle = new("Aequus/Items/Accessories/Combat/Ranged/LaserReticle");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Ranged/LaserReticle_Glow</summary>
        public static readonly TextureAsset LaserReticle_Glow = new("Aequus/Items/Accessories/Combat/Ranged/LaserReticle_Glow");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/Building/LavaproofMitten</summary>
        public static readonly TextureAsset LavaproofMitten = new("Aequus/Items/Accessories/Misc/Building/LavaproofMitten");
        /// <summary>Full Path: Aequus/Items/Materials/Fish/Leecheel</summary>
        public static readonly TextureAsset Leecheel = new("Aequus/Items/Materials/Fish/Leecheel");
        /// <summary>Full Path: Aequus/Items/Tools/GrapplingHooks/LeechHook</summary>
        public static readonly TextureAsset LeechHook = new("Aequus/Items/Tools/GrapplingHooks/LeechHook");
        /// <summary>Full Path: Aequus/Projectiles/Misc/GrapplingHooks/LeechHookProj</summary>
        public static readonly TextureAsset LeechHookProj = new("Aequus/Projectiles/Misc/GrapplingHooks/LeechHookProj");
        /// <summary>Full Path: Aequus/Projectiles/Misc/GrapplingHooks/LeechHookProj_Chain</summary>
        public static readonly TextureAsset LeechHookProj_Chain = new("Aequus/Projectiles/Misc/GrapplingHooks/LeechHookProj_Chain");
        /// <summary>Full Path: Aequus/Items/Misc/Bait/LegendberryBait</summary>
        public static readonly TextureAsset LegendberryBait = new("Aequus/Items/Misc/Bait/LegendberryBait");
        /// <summary>Full Path: Aequus/Items/Weapons/Summon/Minion/LifeFruitStaff</summary>
        public static readonly TextureAsset LifeFruitStaff = new("Aequus/Items/Weapons/Summon/Minion/LifeFruitStaff");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/RedSprite/LightningRod</summary>
        public static readonly TextureAsset LightningRod = new("Aequus/Items/Vanity/Pets/RedSprite/LightningRod");
        /// <summary>Full Path: Aequus/Assets/LightRay</summary>
        public static readonly TextureAsset LightRay = new("Aequus/Assets/LightRay");
        /// <summary>Full Path: Aequus/Assets/LightRay0</summary>
        public static readonly TextureAsset LightRay0 = new("Aequus/Assets/LightRay0");
        /// <summary>Full Path: Aequus/Assets/LightRay1</summary>
        public static readonly TextureAsset LightRay1 = new("Aequus/Assets/LightRay1");
        /// <summary>Full Path: Aequus/Assets/LightRay2</summary>
        public static readonly TextureAsset LightRay2 = new("Aequus/Assets/LightRay2");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/LihzahrdKusariyari/LihzahrdKusariyari</summary>
        public static readonly TextureAsset LihzahrdKusariyari = new("Aequus/Items/Weapons/Melee/LihzahrdKusariyari/LihzahrdKusariyari");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/LihzahrdKusariyari/LihzahrdKusariyari_Chain</summary>
        public static readonly TextureAsset LihzahrdKusariyari_Chain = new("Aequus/Items/Weapons/Melee/LihzahrdKusariyari/LihzahrdKusariyari_Chain");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/LihzahrdKusariyari/LihzahrdKusariyariAltProj</summary>
        public static readonly TextureAsset LihzahrdKusariyariAltProj = new("Aequus/Items/Weapons/Melee/LihzahrdKusariyari/LihzahrdKusariyariAltProj");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/LihzahrdKusariyari/LihzahrdKusariyariProj</summary>
        public static readonly TextureAsset LihzahrdKusariyariProj = new("Aequus/Items/Weapons/Melee/LihzahrdKusariyari/LihzahrdKusariyariProj");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/LihzahrdKusariyari/LihzahrdKusariyariProj_Glow</summary>
        public static readonly TextureAsset LihzahrdKusariyariProj_Glow = new("Aequus/Items/Weapons/Melee/LihzahrdKusariyari/LihzahrdKusariyariProj_Glow");
        /// <summary>Full Path: Aequus/Unused/Items/LiquidGun</summary>
        public static readonly TextureAsset LiquidGun = new("Aequus/Unused/Items/LiquidGun");
        /// <summary>Full Path: Aequus/Unused/Items/LiquidGunHoney</summary>
        public static readonly TextureAsset LiquidGunHoney = new("Aequus/Unused/Items/LiquidGunHoney");
        /// <summary>Full Path: Aequus/Unused/Items/LiquidGunLava</summary>
        public static readonly TextureAsset LiquidGunLava = new("Aequus/Unused/Items/LiquidGunLava");
        /// <summary>Full Path: Aequus/Unused/Items/LiquidGunLava_Glow</summary>
        public static readonly TextureAsset LiquidGunLava_Glow = new("Aequus/Unused/Items/LiquidGunLava_Glow");
        /// <summary>Full Path: Aequus/Unused/Items/LiquidGunShimmer</summary>
        public static readonly TextureAsset LiquidGunShimmer = new("Aequus/Unused/Items/LiquidGunShimmer");
        /// <summary>Full Path: Aequus/Unused/Items/LiquidGunShimmer_Glow</summary>
        public static readonly TextureAsset LiquidGunShimmer_Glow = new("Aequus/Unused/Items/LiquidGunShimmer_Glow");
        /// <summary>Full Path: Aequus/Unused/Items/LiquidGunWater</summary>
        public static readonly TextureAsset LiquidGunWater = new("Aequus/Unused/Items/LiquidGunWater");
        /// <summary>Full Path: Aequus/Unused/Items/LittleInferno</summary>
        public static readonly TextureAsset LittleInferno = new("Aequus/Unused/Items/LittleInferno");
        /// <summary>Full Path: Aequus/NPCs/Monsters/Event/DemonSiege/Magmabubble</summary>
        public static readonly TextureAsset Magmabubble = new("Aequus/NPCs/Monsters/Event/DemonSiege/Magmabubble");
        /// <summary>Full Path: Aequus/Assets/Gores/Magmabubble_0</summary>
        public static readonly TextureAsset Magmabubble_0 = new("Aequus/Assets/Gores/Magmabubble_0");
        /// <summary>Full Path: Aequus/Assets/Gores/Magmabubble_1</summary>
        public static readonly TextureAsset Magmabubble_1 = new("Aequus/Assets/Gores/Magmabubble_1");
        /// <summary>Full Path: Aequus/Tiles/Banners/Items/MagmabubbleBanner</summary>
        public static readonly TextureAsset MagmabubbleBanner = new("Aequus/Tiles/Banners/Items/MagmabubbleBanner");
        /// <summary>Full Path: Aequus/Projectiles/Monster/MagmabubbleBullet</summary>
        public static readonly TextureAsset MagmabubbleBullet = new("Aequus/Projectiles/Monster/MagmabubbleBullet");
        /// <summary>Full Path: Aequus/NPCs/Monsters/Event/DemonSiege/MagmabubbleLegs</summary>
        public static readonly TextureAsset MagmabubbleLegs = new("Aequus/NPCs/Monsters/Event/DemonSiege/MagmabubbleLegs");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Necro/Malediction</summary>
        public static readonly TextureAsset Malediction = new("Aequus/Items/Accessories/Combat/Necro/Malediction");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/Misc/Mallet</summary>
        public static readonly TextureAsset Mallet = new("Aequus/Items/Weapons/Melee/Misc/Mallet");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/Misc/MalletCursor_0</summary>
        public static readonly TextureAsset MalletCursor_0 = new("Aequus/Items/Weapons/Melee/Misc/MalletCursor_0");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/Misc/MalletCursor_0_Outline</summary>
        public static readonly TextureAsset MalletCursor_0_Outline = new("Aequus/Items/Weapons/Melee/Misc/MalletCursor_0_Outline");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/Misc/MalletCursor_1</summary>
        public static readonly TextureAsset MalletCursor_1 = new("Aequus/Items/Weapons/Melee/Misc/MalletCursor_1");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/Misc/MalletCursor_1_Outline</summary>
        public static readonly TextureAsset MalletCursor_1_Outline = new("Aequus/Items/Weapons/Melee/Misc/MalletCursor_1_Outline");
        /// <summary>Full Path: Aequus/Items/Potions/Pollen/ManaclePollen</summary>
        public static readonly TextureAsset ManaclePollen = new("Aequus/Items/Potions/Pollen/ManaclePollen");
        /// <summary>Full Path: Aequus/Items/Potions/Pollen/ManacleSeeds</summary>
        public static readonly TextureAsset ManacleSeeds = new("Aequus/Items/Potions/Pollen/ManacleSeeds");
        /// <summary>Full Path: Aequus/Tiles/Misc/Herbs/ManacleTile</summary>
        public static readonly TextureAsset ManacleTile = new("Aequus/Tiles/Misc/Herbs/ManacleTile");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/ManaCursor</summary>
        public static readonly TextureAsset ManaCursor = new("Aequus/Content/CursorDyes/Items/ManaCursor");
        /// <summary>Full Path: Aequus/Buffs/ManathirstBuff</summary>
        public static readonly TextureAsset ManathirstBuff = new("Aequus/Buffs/ManathirstBuff");
        /// <summary>Full Path: Aequus/Items/Potions/ManathirstPotion</summary>
        public static readonly TextureAsset ManathirstPotion = new("Aequus/Items/Potions/ManathirstPotion");
        /// <summary>Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/MannequinArmorOverlay</summary>
        public static readonly TextureAsset MannequinArmorOverlay = new("Aequus/Content/Biomes/MossBiomes/Tiles/MannequinArmorOverlay");
        /// <summary>Full Path: Aequus/Items/Tools/GrapplingHooks/Meathook</summary>
        public static readonly TextureAsset Meathook = new("Aequus/Items/Tools/GrapplingHooks/Meathook");
        /// <summary>Full Path: Aequus/Projectiles/Misc/GrapplingHooks/MeathookProj</summary>
        public static readonly TextureAsset MeathookProj = new("Aequus/Projectiles/Misc/GrapplingHooks/MeathookProj");
        /// <summary>Full Path: Aequus/Projectiles/Misc/GrapplingHooks/MeathookProj_Chain</summary>
        public static readonly TextureAsset MeathookProj_Chain = new("Aequus/Projectiles/Misc/GrapplingHooks/MeathookProj_Chain");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/MechPetCombo/MechPetComboBuff</summary>
        public static readonly TextureAsset MechPetComboBuff = new("Aequus/Items/Vanity/Pets/MechPetCombo/MechPetComboBuff");
        /// <summary>Full Path: Aequus/Unused/Items/Mendshroom</summary>
        public static readonly TextureAsset Mendshroom = new("Aequus/Unused/Items/Mendshroom");
        /// <summary>Full Path: Aequus/Particles/Dusts/MendshroomDustSpore</summary>
        public static readonly TextureAsset MendshroomDustSpore = new("Aequus/Particles/Dusts/MendshroomDustSpore");
        /// <summary>Full Path: Aequus/Items/Potions/Unique/MercerTonic</summary>
        public static readonly TextureAsset MercerTonic = new("Aequus/Items/Potions/Unique/MercerTonic");
        /// <summary>Full Path: Aequus/NPCs/Monsters/Meteor</summary>
        public static readonly TextureAsset Meteor = new("Aequus/NPCs/Monsters/Meteor");
        /// <summary>Full Path: Aequus/Buffs/Minion/MindfungusBuff</summary>
        public static readonly TextureAsset MindfungusBuff = new("Aequus/Buffs/Minion/MindfungusBuff");
        /// <summary>Full Path: Aequus/Items/Weapons/Summon/Minion/MindfungusStaff</summary>
        public static readonly TextureAsset MindfungusStaff = new("Aequus/Items/Weapons/Summon/Minion/MindfungusStaff");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/Miner/MiningPet</summary>
        public static readonly TextureAsset MiningPet = new("Aequus/Items/Vanity/Pets/Miner/MiningPet");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/Miner/MiningPetBuff</summary>
        public static readonly TextureAsset MiningPetBuff = new("Aequus/Items/Vanity/Pets/Miner/MiningPetBuff");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/Miner/MiningPetSpawner</summary>
        public static readonly TextureAsset MiningPetSpawner = new("Aequus/Items/Vanity/Pets/Miner/MiningPetSpawner");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/MirrorsCall/MirrorsCall</summary>
        public static readonly TextureAsset MirrorsCall = new("Aequus/Items/Weapons/Melee/MirrorsCall/MirrorsCall");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/MirrorsCall/MirrorsCall_Aura</summary>
        public static readonly TextureAsset MirrorsCall_Aura = new("Aequus/Items/Weapons/Melee/MirrorsCall/MirrorsCall_Aura");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/MirrorsCall/MirrorsCall_Edges</summary>
        public static readonly TextureAsset MirrorsCall_Edges = new("Aequus/Items/Weapons/Melee/MirrorsCall/MirrorsCall_Edges");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/MirrorsCall/MirrorsCall_Edges_Aura</summary>
        public static readonly TextureAsset MirrorsCall_Edges_Aura = new("Aequus/Items/Weapons/Melee/MirrorsCall/MirrorsCall_Edges_Aura");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/MirrorsCall/MirrorsCall_resprite</summary>
        public static readonly TextureAsset MirrorsCall_resprite = new("Aequus/Items/Weapons/Melee/MirrorsCall/MirrorsCall_resprite");
        /// <summary>Full Path: Aequus/Items/Potions/Pollen/MistralPollen</summary>
        public static readonly TextureAsset MistralPollen = new("Aequus/Items/Potions/Pollen/MistralPollen");
        /// <summary>Full Path: Aequus/Items/Potions/Pollen/MistralSeeds</summary>
        public static readonly TextureAsset MistralSeeds = new("Aequus/Items/Potions/Pollen/MistralSeeds");
        /// <summary>Full Path: Aequus/Tiles/Misc/Herbs/MistralTile</summary>
        public static readonly TextureAsset MistralTile = new("Aequus/Tiles/Misc/Herbs/MistralTile");
        /// <summary>Full Path: Aequus/Tiles/Misc/Herbs/MistralTile_Pinwheel</summary>
        public static readonly TextureAsset MistralTile_Pinwheel = new("Aequus/Tiles/Misc/Herbs/MistralTile_Pinwheel");
        /// <summary>Full Path: Aequus/Items/Consumables/Permanent/MoneyTrashcan</summary>
        public static readonly TextureAsset MoneyTrashcan = new("Aequus/Items/Consumables/Permanent/MoneyTrashcan");
        /// <summary>Full Path: Aequus/Items/Consumables/Permanent/MoneyTrashcan_UI</summary>
        public static readonly TextureAsset MoneyTrashcan_UI = new("Aequus/Items/Consumables/Permanent/MoneyTrashcan_UI");
        /// <summary>Full Path: Aequus/Particles/Dusts/MonoDust</summary>
        public static readonly TextureAsset MonoDust = new("Aequus/Particles/Dusts/MonoDust");
        /// <summary>Full Path: Aequus/Items/Materials/Gems/MonoGem</summary>
        public static readonly TextureAsset MonoGem = new("Aequus/Items/Materials/Gems/MonoGem");
        /// <summary>Full Path: Aequus/Items/Materials/Gems/MonoGemTile</summary>
        public static readonly TextureAsset MonoGemTile = new("Aequus/Items/Materials/Gems/MonoGemTile");
        /// <summary>Full Path: Aequus/Particles/Dusts/MonoSparkleDust</summary>
        public static readonly TextureAsset MonoSparkleDust = new("Aequus/Particles/Dusts/MonoSparkleDust");
        /// <summary>Full Path: Aequus/Tiles/Banners/MonsterBanners</summary>
        public static readonly TextureAsset MonsterBanners = new("Aequus/Tiles/Banners/MonsterBanners");
        /// <summary>Full Path: Aequus/Items/Potions/Pollen/MoonflowerPollen</summary>
        public static readonly TextureAsset MoonflowerPollen = new("Aequus/Items/Potions/Pollen/MoonflowerPollen");
        /// <summary>Full Path: Aequus/Items/Potions/Pollen/MoonflowerSeeds</summary>
        public static readonly TextureAsset MoonflowerSeeds = new("Aequus/Items/Potions/Pollen/MoonflowerSeeds");
        /// <summary>Full Path: Aequus/Tiles/Misc/Herbs/MoonflowerTile</summary>
        public static readonly TextureAsset MoonflowerTile = new("Aequus/Tiles/Misc/Herbs/MoonflowerTile");
        /// <summary>Full Path: Aequus/Tiles/Misc/Herbs/MoonflowerTileEffect</summary>
        public static readonly TextureAsset MoonflowerTileEffect = new("Aequus/Tiles/Misc/Herbs/MoonflowerTileEffect");
        /// <summary>Full Path: Aequus/Items/Armor/SetWizard/MoonlunaHat</summary>
        public static readonly TextureAsset MoonlunaHat = new("Aequus/Items/Armor/SetWizard/MoonlunaHat");
        /// <summary>Full Path: Aequus/Items/Armor/SetWizard/MoonlunaHat_Glow</summary>
        public static readonly TextureAsset MoonlunaHat_Glow = new("Aequus/Items/Armor/SetWizard/MoonlunaHat_Glow");
        /// <summary>Full Path: Aequus/Items/Armor/SetWizard/MoonlunaHat_Head</summary>
        public static readonly TextureAsset MoonlunaHat_Head = new("Aequus/Items/Armor/SetWizard/MoonlunaHat_Head");
        /// <summary>Full Path: Aequus/Projectiles/Summon/Misc/MoonlunaHatProj</summary>
        public static readonly TextureAsset MoonlunaHatProj = new("Aequus/Projectiles/Summon/Misc/MoonlunaHatProj");
        /// <summary>Full Path: Aequus/Items/Potions/Pollen/MorayPollen</summary>
        public static readonly TextureAsset MorayPollen = new("Aequus/Items/Potions/Pollen/MorayPollen");
        /// <summary>Full Path: Aequus/Items/Potions/Pollen/MoraySeeds</summary>
        public static readonly TextureAsset MoraySeeds = new("Aequus/Items/Potions/Pollen/MoraySeeds");
        /// <summary>Full Path: Aequus/Items/Potions/Pollen/MoraySeeds_Glow</summary>
        public static readonly TextureAsset MoraySeeds_Glow = new("Aequus/Items/Potions/Pollen/MoraySeeds_Glow");
        /// <summary>Full Path: Aequus/Tiles/Misc/Herbs/MorayTile</summary>
        public static readonly TextureAsset MorayTile = new("Aequus/Tiles/Misc/Herbs/MorayTile");
        /// <summary>Full Path: Aequus/Tiles/Misc/Herbs/MorayTile_Glow</summary>
        public static readonly TextureAsset MorayTile_Glow = new("Aequus/Tiles/Misc/Herbs/MorayTile_Glow");
        /// <summary>Full Path: Aequus/Unused/Items/Moro</summary>
        public static readonly TextureAsset Moro = new("Aequus/Unused/Items/Moro");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/MothmanMask</summary>
        public static readonly TextureAsset MothmanMask = new("Aequus/Items/Accessories/Combat/MothmanMask");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/MothmanMask_Mask</summary>
        public static readonly TextureAsset MothmanMask_Mask = new("Aequus/Items/Accessories/Combat/MothmanMask_Mask");
        /// <summary>Full Path: Aequus/Items/Misc/Bait/MysticPopper</summary>
        public static readonly TextureAsset MysticPopper = new("Aequus/Items/Misc/Bait/MysticPopper");
        /// <summary>Full Path: Aequus/Items/Misc/NameTag</summary>
        public static readonly TextureAsset NameTag = new("Aequus/Items/Misc/NameTag");
        /// <summary>Full Path: Aequus/Items/Accessories/CrownOfBlood/Projectiles/NaniteSpore</summary>
        public static readonly TextureAsset NaniteSpore = new("Aequus/Items/Accessories/CrownOfBlood/Projectiles/NaniteSpore");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/Narrizuul</summary>
        public static readonly TextureAsset Narrizuul = new("Aequus/Items/Weapons/Magic/Narrizuul");
        /// <summary>Full Path: Aequus/Assets/UI/NarrizuulBloom</summary>
        public static readonly TextureAsset NarrizuulBloom = new("Aequus/Assets/UI/NarrizuulBloom");
        /// <summary>Full Path: Aequus/Projectiles/Magic/NarrizuulProj</summary>
        public static readonly TextureAsset NarrizuulProj = new("Aequus/Projectiles/Magic/NarrizuulProj");
        /// <summary>Full Path: Aequus/Projectiles/Magic/NarrizuulProj_Glow</summary>
        public static readonly TextureAsset NarrizuulProj_Glow = new("Aequus/Projectiles/Magic/NarrizuulProj_Glow");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Necro/NaturesCruelty</summary>
        public static readonly TextureAsset NaturesCruelty = new("Aequus/Items/Accessories/Combat/Necro/NaturesCruelty");
        /// <summary>Full Path: Aequus/Items/Materials/PillarFragments/NecroFragment</summary>
        public static readonly TextureAsset NecroFragment = new("Aequus/Items/Materials/PillarFragments/NecroFragment");
        /// <summary>Full Path: Aequus/Items/Armor/SetNecromancer/NecromancerHood</summary>
        public static readonly TextureAsset NecromancerHood = new("Aequus/Items/Armor/SetNecromancer/NecromancerHood");
        /// <summary>Full Path: Aequus/Items/Armor/SetNecromancer/NecromancerHood_Head</summary>
        public static readonly TextureAsset NecromancerHood_Head = new("Aequus/Items/Armor/SetNecromancer/NecromancerHood_Head");
        /// <summary>Full Path: Aequus/Items/Armor/SetNecromancer/NecromancerHood_Head_Glow</summary>
        public static readonly TextureAsset NecromancerHood_Head_Glow = new("Aequus/Items/Armor/SetNecromancer/NecromancerHood_Head_Glow");
        /// <summary>Full Path: Aequus/Items/Armor/SetNecromancer/NecromancerRobe</summary>
        public static readonly TextureAsset NecromancerRobe = new("Aequus/Items/Armor/SetNecromancer/NecromancerRobe");
        /// <summary>Full Path: Aequus/Items/Armor/SetNecromancer/NecromancerRobe_Body</summary>
        public static readonly TextureAsset NecromancerRobe_Body = new("Aequus/Items/Armor/SetNecromancer/NecromancerRobe_Body");
        /// <summary>Full Path: Aequus/Items/Armor/SetNecromancer/NecromancerRobe_Legs</summary>
        public static readonly TextureAsset NecromancerRobe_Legs = new("Aequus/Items/Armor/SetNecromancer/NecromancerRobe_Legs");
        /// <summary>Full Path: Aequus/Items/Armor/SetNecromancer/NecromancerRobe_Legs_Glow</summary>
        public static readonly TextureAsset NecromancerRobe_Legs_Glow = new("Aequus/Items/Armor/SetNecromancer/NecromancerRobe_Legs_Glow");
        /// <summary>Full Path: Aequus/Buffs/Misc/NecromancyOwnerBuff</summary>
        public static readonly TextureAsset NecromancyOwnerBuff = new("Aequus/Buffs/Misc/NecromancyOwnerBuff");
        /// <summary>Full Path: Aequus/Unused/Items/NecromancyPotion</summary>
        public static readonly TextureAsset NecromancyPotion = new("Aequus/Unused/Items/NecromancyPotion");
        /// <summary>Full Path: Aequus/Assets/UI/NecromancySelectionCursor</summary>
        public static readonly TextureAsset NecromancySelectionCursor = new("Aequus/Assets/UI/NecromancySelectionCursor");
        /// <summary>Full Path: Aequus/Content/Elites/Misc/NeonAttack</summary>
        public static readonly TextureAsset NeonAttack = new("Aequus/Content/Elites/Misc/NeonAttack");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/Fishing/NeonGenesis</summary>
        public static readonly TextureAsset NeonGenesis = new("Aequus/Items/Accessories/Misc/Fishing/NeonGenesis");
        /// <summary>Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/NeonHealPlant</summary>
        public static readonly TextureAsset NeonHealPlant = new("Aequus/Content/Biomes/MossBiomes/Tiles/NeonHealPlant");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/Heavy/Nettlebane</summary>
        public static readonly TextureAsset Nettlebane = new("Aequus/Items/Weapons/Melee/Heavy/Nettlebane");
        /// <summary>Full Path: Aequus/Buffs/Misc/NettlebaneBuffTier1</summary>
        public static readonly TextureAsset NettlebaneBuffTier1 = new("Aequus/Buffs/Misc/NettlebaneBuffTier1");
        /// <summary>Full Path: Aequus/Buffs/Misc/NettlebaneBuffTier2</summary>
        public static readonly TextureAsset NettlebaneBuffTier2 = new("Aequus/Buffs/Misc/NettlebaneBuffTier2");
        /// <summary>Full Path: Aequus/Buffs/Misc/NettlebaneBuffTier3</summary>
        public static readonly TextureAsset NettlebaneBuffTier3 = new("Aequus/Buffs/Misc/NettlebaneBuffTier3");
        /// <summary>Full Path: Aequus/Projectiles/Melee/Swords/NettlebaneProj</summary>
        public static readonly TextureAsset NettlebaneProj = new("Aequus/Projectiles/Melee/Swords/NettlebaneProj");
        /// <summary>Full Path: Aequus/Items/Potions/NeutronYogurt</summary>
        public static readonly TextureAsset NeutronYogurt = new("Aequus/Items/Potions/NeutronYogurt");
        /// <summary>Full Path: Aequus/Buffs/NeutronYogurtBuff</summary>
        public static readonly TextureAsset NeutronYogurtBuff = new("Aequus/Buffs/NeutronYogurtBuff");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/Nightfall</summary>
        public static readonly TextureAsset Nightfall = new("Aequus/Items/Weapons/Magic/Nightfall");
        /// <summary>Full Path: Aequus/Projectiles/Magic/NightfallGlint</summary>
        public static readonly TextureAsset NightfallGlint = new("Aequus/Projectiles/Magic/NightfallGlint");
        /// <summary>Full Path: Aequus/Projectiles/Magic/NightfallProj</summary>
        public static readonly TextureAsset NightfallProj = new("Aequus/Projectiles/Magic/NightfallProj");
        /// <summary>Full Path: Aequus/Items/Tools/Poles/Nimrod</summary>
        public static readonly TextureAsset Nimrod = new("Aequus/Items/Tools/Poles/Nimrod");
        /// <summary>Full Path: Aequus/Items/Tools/Poles/Nimrod_Glow</summary>
        public static readonly TextureAsset Nimrod_Glow = new("Aequus/Items/Tools/Poles/Nimrod_Glow");
        /// <summary>Full Path: Aequus/Projectiles/Misc/Bobbers/NimrodBobber</summary>
        public static readonly TextureAsset NimrodBobber = new("Aequus/Projectiles/Misc/Bobbers/NimrodBobber");
        /// <summary>Full Path: Aequus/Assets/None</summary>
        public static readonly TextureAsset None = new("Aequus/Assets/None");
        /// <summary>Full Path: Aequus/Buffs/NoonBuff</summary>
        public static readonly TextureAsset NoonBuff = new("Aequus/Buffs/NoonBuff");
        /// <summary>Full Path: Aequus/Items/Potions/NoonPotion</summary>
        public static readonly TextureAsset NoonPotion = new("Aequus/Items/Potions/NoonPotion");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionBed</summary>
        public static readonly TextureAsset OblivionBed = new("Aequus/Tiles/Furniture/Oblivion/OblivionBed");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionBookcase</summary>
        public static readonly TextureAsset OblivionBookcase = new("Aequus/Tiles/Furniture/Oblivion/OblivionBookcase");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionCandelabra</summary>
        public static readonly TextureAsset OblivionCandelabra = new("Aequus/Tiles/Furniture/Oblivion/OblivionCandelabra");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionCandle</summary>
        public static readonly TextureAsset OblivionCandle = new("Aequus/Tiles/Furniture/Oblivion/OblivionCandle");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionChair</summary>
        public static readonly TextureAsset OblivionChair = new("Aequus/Tiles/Furniture/Oblivion/OblivionChair");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionChandelier</summary>
        public static readonly TextureAsset OblivionChandelier = new("Aequus/Tiles/Furniture/Oblivion/OblivionChandelier");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionChest</summary>
        public static readonly TextureAsset OblivionChest = new("Aequus/Tiles/Furniture/Oblivion/OblivionChest");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionChestTile</summary>
        public static readonly TextureAsset OblivionChestTile = new("Aequus/Tiles/Furniture/Oblivion/OblivionChestTile");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionChestTile_Glow</summary>
        public static readonly TextureAsset OblivionChestTile_Glow = new("Aequus/Tiles/Furniture/Oblivion/OblivionChestTile_Glow");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionChestTile_Highlight</summary>
        public static readonly TextureAsset OblivionChestTile_Highlight = new("Aequus/Tiles/Furniture/Oblivion/OblivionChestTile_Highlight");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionClock</summary>
        public static readonly TextureAsset OblivionClock = new("Aequus/Tiles/Furniture/Oblivion/OblivionClock");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionDresser</summary>
        public static readonly TextureAsset OblivionDresser = new("Aequus/Tiles/Furniture/Oblivion/OblivionDresser");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionLamp</summary>
        public static readonly TextureAsset OblivionLamp = new("Aequus/Tiles/Furniture/Oblivion/OblivionLamp");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionLantern</summary>
        public static readonly TextureAsset OblivionLantern = new("Aequus/Tiles/Furniture/Oblivion/OblivionLantern");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionPiano</summary>
        public static readonly TextureAsset OblivionPiano = new("Aequus/Tiles/Furniture/Oblivion/OblivionPiano");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionSink</summary>
        public static readonly TextureAsset OblivionSink = new("Aequus/Tiles/Furniture/Oblivion/OblivionSink");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionSofa</summary>
        public static readonly TextureAsset OblivionSofa = new("Aequus/Tiles/Furniture/Oblivion/OblivionSofa");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionTable</summary>
        public static readonly TextureAsset OblivionTable = new("Aequus/Tiles/Furniture/Oblivion/OblivionTable");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionWorkbench</summary>
        public static readonly TextureAsset OblivionWorkbench = new("Aequus/Tiles/Furniture/Oblivion/OblivionWorkbench");
        /// <summary>Full Path: Aequus/NPCs/Critters/Oblivision</summary>
        public static readonly TextureAsset Oblivision = new("Aequus/NPCs/Critters/Oblivision");
        /// <summary>Full Path: Aequus/Assets/Gores/Occultist_0</summary>
        public static readonly TextureAsset Occultist_0 = new("Aequus/Assets/Gores/Occultist_0");
        /// <summary>Full Path: Aequus/Assets/Gores/Occultist_1</summary>
        public static readonly TextureAsset Occultist_1 = new("Aequus/Assets/Gores/Occultist_1");
        /// <summary>Full Path: Aequus/Assets/Gores/Occultist_2</summary>
        public static readonly TextureAsset Occultist_2 = new("Aequus/Assets/Gores/Occultist_2");
        /// <summary>Full Path: Aequus/NPCs/Town/OccultistNPC/Occultist_Glow</summary>
        public static readonly TextureAsset Occultist_Glow = new("Aequus/NPCs/Town/OccultistNPC/Occultist_Glow");
        /// <summary>Full Path: Aequus/NPCs/Town/OccultistNPC/Occultist_Head</summary>
        public static readonly TextureAsset Occultist_Head = new("Aequus/NPCs/Town/OccultistNPC/Occultist_Head");
        /// <summary>Full Path: Aequus/NPCs/Town/OccultistNPC/Occultist</summary>
        public static readonly TextureAsset Occultist_OccultistNPC = new("Aequus/NPCs/Town/OccultistNPC/Occultist");
        /// <summary>Full Path: Aequus/NPCs/Town/OccultistNPC/Shimmer/Occultist</summary>
        public static readonly TextureAsset Occultist_Shimmer = new("Aequus/NPCs/Town/OccultistNPC/Shimmer/Occultist");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Candles/OccultistCandle</summary>
        public static readonly TextureAsset OccultistCandle = new("Aequus/Items/Weapons/Necromancy/Candles/OccultistCandle");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Candles/OccultistCandle_Flame</summary>
        public static readonly TextureAsset OccultistCandle_Flame = new("Aequus/Items/Weapons/Necromancy/Candles/OccultistCandle_Flame");
        /// <summary>Full Path: Aequus/NPCs/Town/OccultistNPC/OccultistHostile</summary>
        public static readonly TextureAsset OccultistHostile = new("Aequus/NPCs/Town/OccultistNPC/OccultistHostile");
        /// <summary>Full Path: Aequus/NPCs/Town/OccultistNPC/OccultistHostile_Head</summary>
        public static readonly TextureAsset OccultistHostile_Head = new("Aequus/NPCs/Town/OccultistNPC/OccultistHostile_Head");
        /// <summary>Full Path: Aequus/NPCs/Town/OccultistNPC/OccultistHostile_Sit</summary>
        public static readonly TextureAsset OccultistHostile_Sit = new("Aequus/NPCs/Town/OccultistNPC/OccultistHostile_Sit");
        /// <summary>Full Path: Aequus/NPCs/Town/OccultistNPC/OccultistHostile_Sit_Glow</summary>
        public static readonly TextureAsset OccultistHostile_Sit_Glow = new("Aequus/NPCs/Town/OccultistNPC/OccultistHostile_Sit_Glow");
        /// <summary>Full Path: Aequus/NPCs/Town/OccultistNPC/OccultistRune</summary>
        public static readonly TextureAsset OccultistRune = new("Aequus/NPCs/Town/OccultistNPC/OccultistRune");
        /// <summary>Full Path: Aequus/NPCs/Town/OccultistNPC/OccultistSleep</summary>
        public static readonly TextureAsset OccultistSleep = new("Aequus/NPCs/Town/OccultistNPC/OccultistSleep");
        /// <summary>Full Path: Aequus/NPCs/Town/OccultistNPC/OccultistSleep_Glow</summary>
        public static readonly TextureAsset OccultistSleep_Glow = new("Aequus/NPCs/Town/OccultistNPC/OccultistSleep_Glow");
        /// <summary>Full Path: Aequus/Unused/Items/SlotMachines/OceanSlotMachine</summary>
        public static readonly TextureAsset OceanSlotMachine = new("Aequus/Unused/Items/SlotMachines/OceanSlotMachine");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Paintings/Items/OliverPainting</summary>
        public static readonly TextureAsset OliverPainting = new("Aequus/Tiles/Furniture/Paintings/Items/OliverPainting");
        /// <summary>Full Path: Aequus/Assets/UI/BossChecklist/OmegaStarite</summary>
        public static readonly TextureAsset OmegaStarite_BossChecklist = new("Aequus/Assets/UI/BossChecklist/OmegaStarite");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/OmegaStarite/OmegaStarite_Head_Boss</summary>
        public static readonly TextureAsset OmegaStarite_Head_Boss = new("Aequus/NPCs/BossMonsters/OmegaStarite/OmegaStarite_Head_Boss");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/OmegaStarite/OmegaStarite</summary>
        public static readonly TextureAsset OmegaStarite_OmegaStarite = new("Aequus/NPCs/BossMonsters/OmegaStarite/OmegaStarite");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/OmegaStarite/Rewards/OmegaStariteBag</summary>
        public static readonly TextureAsset OmegaStariteBag = new("Aequus/NPCs/BossMonsters/OmegaStarite/Rewards/OmegaStariteBag");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/OmegaStarite/OmegaStariteBuff</summary>
        public static readonly TextureAsset OmegaStariteBuff = new("Aequus/Items/Vanity/Pets/OmegaStarite/OmegaStariteBuff");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/OmegaStarite/Projectiles/OmegaStariteBullet</summary>
        public static readonly TextureAsset OmegaStariteBullet = new("Aequus/NPCs/BossMonsters/OmegaStarite/Projectiles/OmegaStariteBullet");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/OmegaStarite/Projectiles/OmegaStariteDeathray</summary>
        public static readonly TextureAsset OmegaStariteDeathray = new("Aequus/NPCs/BossMonsters/OmegaStarite/Projectiles/OmegaStariteDeathray");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/OmegaStarite/Rewards/OmegaStariteMask</summary>
        public static readonly TextureAsset OmegaStariteMask = new("Aequus/NPCs/BossMonsters/OmegaStarite/Rewards/OmegaStariteMask");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/OmegaStarite/Rewards/OmegaStariteMask_Head</summary>
        public static readonly TextureAsset OmegaStariteMask_Head = new("Aequus/NPCs/BossMonsters/OmegaStarite/Rewards/OmegaStariteMask_Head");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Paintings/Items/OmegaStaritePainting</summary>
        public static readonly TextureAsset OmegaStaritePainting = new("Aequus/Tiles/Furniture/Paintings/Items/OmegaStaritePainting");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/OmegaStarite/OmegaStaritePet</summary>
        public static readonly TextureAsset OmegaStaritePet = new("Aequus/Items/Vanity/Pets/OmegaStarite/OmegaStaritePet");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/OmegaStarite/Projectiles/OmegaStariteProj</summary>
        public static readonly TextureAsset OmegaStariteProj = new("Aequus/NPCs/BossMonsters/OmegaStarite/Projectiles/OmegaStariteProj");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/OmegaStarite/Rewards/OmegaStariteRelic</summary>
        public static readonly TextureAsset OmegaStariteRelic = new("Aequus/NPCs/BossMonsters/OmegaStarite/Rewards/OmegaStariteRelic");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/OmegaStarite/Rewards/OmegaStariteTrophy</summary>
        public static readonly TextureAsset OmegaStariteTrophy = new("Aequus/NPCs/BossMonsters/OmegaStarite/Rewards/OmegaStariteTrophy");
        /// <summary>Full Path: Aequus/Items/Misc/Bait/Omnibait</summary>
        public static readonly TextureAsset Omnibait = new("Aequus/Items/Misc/Bait/Omnibait");
        /// <summary>Full Path: Aequus/Items/Materials/Gems/OmniGem</summary>
        public static readonly TextureAsset OmniGem = new("Aequus/Items/Materials/Gems/OmniGem");
        /// <summary>Full Path: Aequus/Items/Materials/Gems/OmniGem_Mask</summary>
        public static readonly TextureAsset OmniGem_Mask = new("Aequus/Items/Materials/Gems/OmniGem_Mask");
        /// <summary>Full Path: Aequus/Items/Materials/Gems/OmniGemTile</summary>
        public static readonly TextureAsset OmniGemTile = new("Aequus/Items/Materials/Gems/OmniGemTile");
        /// <summary>Full Path: Aequus/Items/Materials/Gems/OmniGemTile_Mask</summary>
        public static readonly TextureAsset OmniGemTile_Mask = new("Aequus/Items/Materials/Gems/OmniGemTile_Mask");
        /// <summary>Full Path: Aequus/Items/Tools/Building/OmniPaint</summary>
        public static readonly TextureAsset OmniPaint = new("Aequus/Items/Tools/Building/OmniPaint");
        /// <summary>Full Path: Aequus/NPCs/Town/ExporterNPC/RerollSystem/OpenButton</summary>
        public static readonly TextureAsset OpenButton = new("Aequus/NPCs/Town/ExporterNPC/RerollSystem/OpenButton");
        /// <summary>Full Path: Aequus/Items/Materials/Energies/OrganicEnergy</summary>
        public static readonly TextureAsset OrganicEnergy = new("Aequus/Items/Materials/Energies/OrganicEnergy");
        /// <summary>Full Path: Aequus/Assets/Effects/Textures/OrganicEnergyGradient</summary>
        public static readonly TextureAsset OrganicEnergyGradient = new("Aequus/Assets/Effects/Textures/OrganicEnergyGradient");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Paintings/Items/OriginPainting</summary>
        public static readonly TextureAsset OriginPainting = new("Aequus/Tiles/Furniture/Paintings/Items/OriginPainting");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Scepters/Osiris</summary>
        public static readonly TextureAsset Osiris = new("Aequus/Items/Weapons/Necromancy/Scepters/Osiris");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Scepters/Osiris_Glow</summary>
        public static readonly TextureAsset Osiris_Glow = new("Aequus/Items/Weapons/Necromancy/Scepters/Osiris_Glow");
        /// <summary>Full Path: Aequus/Items/Misc/Dyes/OutlineDye</summary>
        public static readonly TextureAsset OutlineDye = new("Aequus/Items/Misc/Dyes/OutlineDye");
        /// <summary>Full Path: Aequus/Items/Consumables/PalePufferfish</summary>
        public static readonly TextureAsset PalePufferfish = new("Aequus/Items/Consumables/PalePufferfish");
        /// <summary>Full Path: Aequus/Content/Building/Bonuses/PaletteBountyBuff</summary>
        public static readonly TextureAsset PaletteBountyBuff = new("Aequus/Content/Building/Bonuses/PaletteBountyBuff");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Necro/PandorasBox</summary>
        public static readonly TextureAsset PandorasBox = new("Aequus/Items/Accessories/Combat/Necro/PandorasBox");
        /// <summary>Full Path: Aequus/NPCs/Town/CarpenterNPC/Quest/Bounties/Panel</summary>
        public static readonly TextureAsset Panel_Bounties = new("Aequus/NPCs/Town/CarpenterNPC/Quest/Bounties/Panel");
        /// <summary>Full Path: Aequus/NPCs/Town/ExporterNPC/RerollSystem/Panel</summary>
        public static readonly TextureAsset Panel_RerollSystem = new("Aequus/NPCs/Town/ExporterNPC/RerollSystem/Panel");
        /// <summary>Full Path: Aequus/Assets/Particles/Particle</summary>
        public static readonly TextureAsset Particle = new("Aequus/Assets/Particles/Particle");
        /// <summary>Full Path: Aequus/Items/Materials/PearlShards/PearlShardBlack</summary>
        public static readonly TextureAsset PearlShardBlack = new("Aequus/Items/Materials/PearlShards/PearlShardBlack");
        /// <summary>Full Path: Aequus/Items/Materials/PearlShards/PearlShardBlack_Dropped</summary>
        public static readonly TextureAsset PearlShardBlack_Dropped = new("Aequus/Items/Materials/PearlShards/PearlShardBlack_Dropped");
        /// <summary>Full Path: Aequus/Items/Materials/PearlShards/PearlShardPink</summary>
        public static readonly TextureAsset PearlShardPink = new("Aequus/Items/Materials/PearlShards/PearlShardPink");
        /// <summary>Full Path: Aequus/Items/Materials/PearlShards/PearlShardPink_Dropped</summary>
        public static readonly TextureAsset PearlShardPink_Dropped = new("Aequus/Items/Materials/PearlShards/PearlShardPink_Dropped");
        /// <summary>Full Path: Aequus/Items/Materials/PearlShards/PearlShardWhite</summary>
        public static readonly TextureAsset PearlShardWhite = new("Aequus/Items/Materials/PearlShards/PearlShardWhite");
        /// <summary>Full Path: Aequus/Items/Materials/PearlShards/PearlShardWhite_Dropped</summary>
        public static readonly TextureAsset PearlShardWhite_Dropped = new("Aequus/Items/Materials/PearlShards/PearlShardWhite_Dropped");
        /// <summary>Full Path: Aequus/Items/Materials/PearlShards/PearlsTileBlack</summary>
        public static readonly TextureAsset PearlsTileBlack = new("Aequus/Items/Materials/PearlShards/PearlsTileBlack");
        /// <summary>Full Path: Aequus/Items/Materials/PearlShards/PearlsTileHypnotic</summary>
        public static readonly TextureAsset PearlsTileHypnotic = new("Aequus/Items/Materials/PearlShards/PearlsTileHypnotic");
        /// <summary>Full Path: Aequus/Items/Materials/PearlShards/PearlsTilePink</summary>
        public static readonly TextureAsset PearlsTilePink = new("Aequus/Items/Materials/PearlShards/PearlsTilePink");
        /// <summary>Full Path: Aequus/Items/Materials/PearlShards/PearlsTileWhite</summary>
        public static readonly TextureAsset PearlsTileWhite = new("Aequus/Items/Materials/PearlShards/PearlsTileWhite");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/PentalScythe</summary>
        public static readonly TextureAsset PentalScythe = new("Aequus/Items/Weapons/Magic/PentalScythe");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/PentalScythe_Glow</summary>
        public static readonly TextureAsset PentalScythe_Glow = new("Aequus/Items/Weapons/Magic/PentalScythe_Glow");
        /// <summary>Full Path: Aequus/Projectiles/Magic/PentalScytheProj</summary>
        public static readonly TextureAsset PentalScytheProj = new("Aequus/Projectiles/Magic/PentalScytheProj");
        /// <summary>Full Path: Aequus/Unused/Items/Photobook/PeonyPhotobook</summary>
        public static readonly TextureAsset PeonyPhotobook = new("Aequus/Unused/Items/Photobook/PeonyPhotobook");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/Drone/PersonalDronePack</summary>
        public static readonly TextureAsset PersonalDronePack = new("Aequus/Items/Vanity/Pets/Drone/PersonalDronePack");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Necro/PharaohsCurse</summary>
        public static readonly TextureAsset PharaohsCurse = new("Aequus/Items/Accessories/Combat/Necro/PharaohsCurse");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/Thrown/PhaseDisc</summary>
        public static readonly TextureAsset PhaseDisc = new("Aequus/Items/Weapons/Melee/Thrown/PhaseDisc");
        /// <summary>Full Path: Aequus/Projectiles/Melee/PhaseDiscProj</summary>
        public static readonly TextureAsset PhaseDiscProj = new("Aequus/Projectiles/Melee/PhaseDiscProj");
        /// <summary>Full Path: Aequus/Items/Tools/PhaseMirror</summary>
        public static readonly TextureAsset PhaseMirror = new("Aequus/Items/Tools/PhaseMirror");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/OnHit/Debuff/PhoenixRing</summary>
        public static readonly TextureAsset PhoenixRing = new("Aequus/Items/Accessories/Combat/OnHit/Debuff/PhoenixRing");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/OnHit/Debuff/PhoenixRing_HandsOn</summary>
        public static readonly TextureAsset PhoenixRing_HandsOn = new("Aequus/Items/Accessories/Combat/OnHit/Debuff/PhoenixRing_HandsOn");
        /// <summary>Full Path: Aequus/Unused/Items/Photobook/PhotobookItem</summary>
        public static readonly TextureAsset PhotobookItem = new("Aequus/Unused/Items/Photobook/PhotobookItem");
        /// <summary>Full Path: Aequus/NPCs/Town/PhysicistNPC/Shimmer/Physicist_Glow</summary>
        public static readonly TextureAsset Physicist_Glow = new("Aequus/NPCs/Town/PhysicistNPC/Shimmer/Physicist_Glow");
        /// <summary>Full Path: Aequus/NPCs/Town/PhysicistNPC/Physicist_Head</summary>
        public static readonly TextureAsset Physicist_Head_PhysicistNPC = new("Aequus/NPCs/Town/PhysicistNPC/Physicist_Head");
        /// <summary>Full Path: Aequus/NPCs/Town/PhysicistNPC/Shimmer/Physicist_Head</summary>
        public static readonly TextureAsset Physicist_Head_Shimmer = new("Aequus/NPCs/Town/PhysicistNPC/Shimmer/Physicist_Head");
        /// <summary>Full Path: Aequus/NPCs/Town/PhysicistNPC/Physicist</summary>
        public static readonly TextureAsset Physicist_PhysicistNPC = new("Aequus/NPCs/Town/PhysicistNPC/Physicist");
        /// <summary>Full Path: Aequus/NPCs/Town/PhysicistNPC/Shimmer/Physicist</summary>
        public static readonly TextureAsset Physicist_Shimmer = new("Aequus/NPCs/Town/PhysicistNPC/Shimmer/Physicist");
        /// <summary>Full Path: Aequus/NPCs/Town/PhysicistNPC/PhysicistPet_Glow</summary>
        public static readonly TextureAsset PhysicistPet_Glow_PhysicistNPC = new("Aequus/NPCs/Town/PhysicistNPC/PhysicistPet_Glow");
        /// <summary>Full Path: Aequus/NPCs/Town/PhysicistNPC/Shimmer/PhysicistPet_Glow</summary>
        public static readonly TextureAsset PhysicistPet_Glow_Shimmer = new("Aequus/NPCs/Town/PhysicistNPC/Shimmer/PhysicistPet_Glow");
        /// <summary>Full Path: Aequus/NPCs/Town/PhysicistNPC/PhysicistPet</summary>
        public static readonly TextureAsset PhysicistPet_PhysicistNPC = new("Aequus/NPCs/Town/PhysicistNPC/PhysicistPet");
        /// <summary>Full Path: Aequus/NPCs/Town/PhysicistNPC/Shimmer/PhysicistPet</summary>
        public static readonly TextureAsset PhysicistPet_Shimmer = new("Aequus/NPCs/Town/PhysicistNPC/Shimmer/PhysicistPet");
        /// <summary>Full Path: Aequus/Items/Weapons/Sentry/PhysicistSentry/PhysicistSentry</summary>
        public static readonly TextureAsset PhysicistSentry = new("Aequus/Items/Weapons/Sentry/PhysicistSentry/PhysicistSentry");
        /// <summary>Full Path: Aequus/Items/Weapons/Sentry/PhysicistSentry/PhysicistSentryProj</summary>
        public static readonly TextureAsset PhysicistSentryProj = new("Aequus/Items/Weapons/Sentry/PhysicistSentry/PhysicistSentryProj");
        /// <summary>Full Path: Aequus/Items/Weapons/Sentry/PhysicistSentry/PhysicistSentryProj_Glow</summary>
        public static readonly TextureAsset PhysicistSentryProj_Glow = new("Aequus/Items/Weapons/Sentry/PhysicistSentry/PhysicistSentryProj_Glow");
        /// <summary>Full Path: Aequus/Tiles/Blocks/PhysicsBlock</summary>
        public static readonly TextureAsset PhysicsBlock = new("Aequus/Tiles/Blocks/PhysicsBlock");
        /// <summary>Full Path: Aequus/Tiles/Blocks/PhysicsBlockTile</summary>
        public static readonly TextureAsset PhysicsBlockTile = new("Aequus/Tiles/Blocks/PhysicsBlockTile");
        /// <summary>Full Path: Aequus/Items/Tools/PhysicsGun</summary>
        public static readonly TextureAsset PhysicsGun = new("Aequus/Items/Tools/PhysicsGun");
        /// <summary>Full Path: Aequus/Items/Tools/PhysicsGun_Glow</summary>
        public static readonly TextureAsset PhysicsGun_Glow = new("Aequus/Items/Tools/PhysicsGun_Glow");
        /// <summary>Full Path: Aequus/Unused/Items/DebugItems/PhysicsGun2</summary>
        public static readonly TextureAsset PhysicsGun2 = new("Aequus/Unused/Items/DebugItems/PhysicsGun2");
        /// <summary>Full Path: Aequus/Unused/Items/DebugItems/PhysicsGun2_Glow</summary>
        public static readonly TextureAsset PhysicsGun2_Glow = new("Aequus/Unused/Items/DebugItems/PhysicsGun2_Glow");
        /// <summary>Full Path: Aequus/Projectiles/Misc/PhysicsGunProj</summary>
        public static readonly TextureAsset PhysicsGunProj = new("Aequus/Projectiles/Misc/PhysicsGunProj");
        /// <summary>Full Path: Aequus/Buffs/Debuffs/PickBreak</summary>
        public static readonly TextureAsset PickBreak = new("Aequus/Buffs/Debuffs/PickBreak");
        /// <summary>Full Path: Aequus/Buffs/Minion/PiranhaPlantBuff</summary>
        public static readonly TextureAsset PiranhaPlantBuff = new("Aequus/Buffs/Minion/PiranhaPlantBuff");
        /// <summary>Full Path: Aequus/Projectiles/Summon/PiranhaPlantFireball</summary>
        public static readonly TextureAsset PiranhaPlantFireball = new("Aequus/Projectiles/Summon/PiranhaPlantFireball");
        /// <summary>Full Path: Aequus/Projectiles/Summon/PiranhaPlantMinion</summary>
        public static readonly TextureAsset PiranhaPlantMinion = new("Aequus/Projectiles/Summon/PiranhaPlantMinion");
        /// <summary>Full Path: Aequus/Items/Weapons/Summon/Demon/PiranhaPot</summary>
        public static readonly TextureAsset PiranhaPot = new("Aequus/Items/Weapons/Summon/Demon/PiranhaPot");
        /// <summary>Full Path: Aequus/Content/Building/Bonuses/PirateBountyBuff</summary>
        public static readonly TextureAsset PirateBountyBuff = new("Aequus/Content/Building/Bonuses/PirateBountyBuff");
        /// <summary>Full Path: Aequus/NPCs/Town/CarpenterNPC/Quest/Bounties/Icons/PirateShipBounty</summary>
        public static readonly TextureAsset PirateShipBounty = new("Aequus/NPCs/Town/CarpenterNPC/Quest/Bounties/Icons/PirateShipBounty");
        /// <summary>Full Path: Aequus/Assets/Pixel</summary>
        public static readonly TextureAsset Pixel = new("Aequus/Assets/Pixel");
        /// <summary>Full Path: Aequus/Items/Tools/MapCamera/PixelCamera</summary>
        public static readonly TextureAsset PixelCamera = new("Aequus/Items/Tools/MapCamera/PixelCamera");
        /// <summary>Full Path: Aequus/Items/Tools/MapCamera/PixelCameraClip</summary>
        public static readonly TextureAsset PixelCameraClip = new("Aequus/Items/Tools/MapCamera/PixelCameraClip");
        /// <summary>Full Path: Aequus/Projectiles/Misc/PixelCameraProj</summary>
        public static readonly TextureAsset PixelCameraProj = new("Aequus/Projectiles/Misc/PixelCameraProj");
        /// <summary>Full Path: Aequus/Tiles/Misc/PixelPaintingTile</summary>
        public static readonly TextureAsset PixelPaintingTile = new("Aequus/Tiles/Misc/PixelPaintingTile");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Candles/PixieCandle</summary>
        public static readonly TextureAsset PixieCandle = new("Aequus/Items/Weapons/Necromancy/Candles/PixieCandle");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Candles/PixieCandle_Flame</summary>
        public static readonly TextureAsset PixieCandle_Flame = new("Aequus/Items/Weapons/Necromancy/Candles/PixieCandle_Flame");
        /// <summary>Full Path: Aequus/Items/Misc/Trash/PlasticBottle</summary>
        public static readonly TextureAsset PlasticBottle = new("Aequus/Items/Misc/Trash/PlasticBottle");
        /// <summary>Full Path: Aequus/NPCs/Town/CarpenterNPC/Quest/Bounties/Icons/PondBridgeBounty</summary>
        public static readonly TextureAsset PondBridgeBounty = new("Aequus/NPCs/Town/CarpenterNPC/Quest/Bounties/Icons/PondBridgeBounty");
        /// <summary>Full Path: Aequus/Items/Materials/PossessedShard</summary>
        public static readonly TextureAsset PossessedShard = new("Aequus/Items/Materials/PossessedShard");
        /// <summary>Full Path: Aequus/Content/CrossMod/SplitSupport/Photography/PosterBloodMimic</summary>
        public static readonly TextureAsset PosterBloodMimic = new("Aequus/Content/CrossMod/SplitSupport/Photography/PosterBloodMimic");
        /// <summary>Full Path: Aequus/Content/CrossMod/SplitSupport/Photography/PosterBreadOfCthulhu</summary>
        public static readonly TextureAsset PosterBreadOfCthulhu = new("Aequus/Content/CrossMod/SplitSupport/Photography/PosterBreadOfCthulhu");
        /// <summary>Full Path: Aequus/Content/CrossMod/SplitSupport/Photography/PosterHeckto</summary>
        public static readonly TextureAsset PosterHeckto = new("Aequus/Content/CrossMod/SplitSupport/Photography/PosterHeckto");
        /// <summary>Full Path: Aequus/Content/CrossMod/SplitSupport/Photography/PosterOblivision</summary>
        public static readonly TextureAsset PosterOblivision = new("Aequus/Content/CrossMod/SplitSupport/Photography/PosterOblivision");
        /// <summary>Full Path: Aequus/Content/CrossMod/SplitSupport/Photography/PosterSkyMerchant</summary>
        public static readonly TextureAsset PosterSkyMerchant = new("Aequus/Content/CrossMod/SplitSupport/Photography/PosterSkyMerchant");
        /// <summary>Full Path: Aequus/Content/CrossMod/SplitSupport/Photography/PosterUltraStarite</summary>
        public static readonly TextureAsset PosterUltraStarite = new("Aequus/Content/CrossMod/SplitSupport/Photography/PosterUltraStarite");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/PotionCanteen</summary>
        public static readonly TextureAsset PotionCanteen = new("Aequus/Items/Accessories/Misc/PotionCanteen");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/PotionCanteen_Mask</summary>
        public static readonly TextureAsset PotionCanteen_Mask = new("Aequus/Items/Accessories/Misc/PotionCanteen_Mask");
        /// <summary>Full Path: Aequus/Items/Potions/Unique/PotionOfResurrection</summary>
        public static readonly TextureAsset PotionOfResurrection = new("Aequus/Items/Potions/Unique/PotionOfResurrection");
        /// <summary>Full Path: Aequus/Content/CrossMod/SplitSupport/Photography/PrintsTile</summary>
        public static readonly TextureAsset PrintsTile = new("Aequus/Content/CrossMod/SplitSupport/Photography/PrintsTile");
        /// <summary>Full Path: Aequus/Projectiles/Misc/ProtectiveProbe</summary>
        public static readonly TextureAsset ProtectiveProbe = new("Aequus/Projectiles/Misc/ProtectiveProbe");
        /// <summary>Full Path: Aequus/Projectiles/Misc/ProtectiveProbe_Glow</summary>
        public static readonly TextureAsset ProtectiveProbe_Glow = new("Aequus/Projectiles/Misc/ProtectiveProbe_Glow");
        /// <summary>Full Path: Aequus/Items/Tools/Pumpinator</summary>
        public static readonly TextureAsset Pumpinator = new("Aequus/Items/Tools/Pumpinator");
        /// <summary>Full Path: Aequus/Items/Tools/Pumpinator_Glow</summary>
        public static readonly TextureAsset Pumpinator_Glow = new("Aequus/Items/Tools/Pumpinator_Glow");
        /// <summary>Full Path: Aequus/Projectiles/Misc/PumpinatorProj</summary>
        public static readonly TextureAsset PumpinatorProj = new("Aequus/Projectiles/Misc/PumpinatorProj");
        /// <summary>Full Path: Aequus/Items/Vanity/PumpkingCloak</summary>
        public static readonly TextureAsset PumpkingCloak = new("Aequus/Items/Vanity/PumpkingCloak");
        /// <summary>Full Path: Aequus/Items/Vanity/PumpkingCloak_Back</summary>
        public static readonly TextureAsset PumpkingCloak_Back = new("Aequus/Items/Vanity/PumpkingCloak_Back");
        /// <summary>Full Path: Aequus/Items/Vanity/PumpkingCloak_Front</summary>
        public static readonly TextureAsset PumpkingCloak_Front = new("Aequus/Items/Vanity/PumpkingCloak_Front");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/PumpkingCursor</summary>
        public static readonly TextureAsset PumpkingCursor = new("Aequus/Content/CursorDyes/Items/PumpkingCursor");
        /// <summary>Full Path: Aequus/Content/DronePylons/Items/PylonCleanserItem</summary>
        public static readonly TextureAsset PylonCleanserItem = new("Aequus/Content/DronePylons/Items/PylonCleanserItem");
        /// <summary>Full Path: Aequus/Content/DronePylons/Items/PylonGunnerItem</summary>
        public static readonly TextureAsset PylonGunnerItem = new("Aequus/Content/DronePylons/Items/PylonGunnerItem");
        /// <summary>Full Path: Aequus/Content/DronePylons/Items/PylonHealerItem</summary>
        public static readonly TextureAsset PylonHealerItem = new("Aequus/Content/DronePylons/Items/PylonHealerItem");
        /// <summary>Full Path: Aequus/Content/Biomes/Pyramid/PyramidStatue</summary>
        public static readonly TextureAsset PyramidStatue = new("Aequus/Content/Biomes/Pyramid/PyramidStatue");
        /// <summary>Full Path: Aequus/Content/Biomes/Pyramid/PyramidStatueBroken1Tile</summary>
        public static readonly TextureAsset PyramidStatueBroken1Tile = new("Aequus/Content/Biomes/Pyramid/PyramidStatueBroken1Tile");
        /// <summary>Full Path: Aequus/Content/Biomes/Pyramid/PyramidStatueBroken2Tile</summary>
        public static readonly TextureAsset PyramidStatueBroken2Tile = new("Aequus/Content/Biomes/Pyramid/PyramidStatueBroken2Tile");
        /// <summary>Full Path: Aequus/Content/Biomes/Pyramid/PyramidStatueBuff</summary>
        public static readonly TextureAsset PyramidStatueBuff = new("Aequus/Content/Biomes/Pyramid/PyramidStatueBuff");
        /// <summary>Full Path: Aequus/Content/Biomes/Pyramid/PyramidStatueBuffDark</summary>
        public static readonly TextureAsset PyramidStatueBuffDark = new("Aequus/Content/Biomes/Pyramid/PyramidStatueBuffDark");
        /// <summary>Full Path: Aequus/Content/Biomes/Pyramid/PyramidStatueBuffLight</summary>
        public static readonly TextureAsset PyramidStatueBuffLight = new("Aequus/Content/Biomes/Pyramid/PyramidStatueBuffLight");
        /// <summary>Full Path: Aequus/Content/Biomes/Pyramid/PyramidStatueTile</summary>
        public static readonly TextureAsset PyramidStatueTile = new("Aequus/Content/Biomes/Pyramid/PyramidStatueTile");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/Luck/RabbitsFoot</summary>
        public static readonly TextureAsset RabbitsFoot = new("Aequus/Items/Accessories/Misc/Luck/RabbitsFoot");
        /// <summary>Full Path: Aequus/Items/Misc/LegendaryFish/RadonFish</summary>
        public static readonly TextureAsset RadonFish = new("Aequus/Items/Misc/LegendaryFish/RadonFish");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/Fishing/RadonFishingBobber</summary>
        public static readonly TextureAsset RadonFishingBobber = new("Aequus/Items/Accessories/Misc/Fishing/RadonFishingBobber");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/Fishing/RadonFishingBobberBuff</summary>
        public static readonly TextureAsset RadonFishingBobberBuff = new("Aequus/Items/Accessories/Misc/Fishing/RadonFishingBobberBuff");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/Fishing/RadonFishingBobberProj</summary>
        public static readonly TextureAsset RadonFishingBobberProj = new("Aequus/Items/Accessories/Misc/Fishing/RadonFishingBobberProj");
        /// <summary>Full Path: Aequus/Content/Biomes/RadonBiome/Tiles/RadonMoss</summary>
        public static readonly TextureAsset RadonMoss = new("Aequus/Content/Biomes/RadonBiome/Tiles/RadonMoss");
        /// <summary>Full Path: Aequus/Content/Biomes/RadonBiome/Tiles/RadonMossBrickTile</summary>
        public static readonly TextureAsset RadonMossBrickTile = new("Aequus/Content/Biomes/RadonBiome/Tiles/RadonMossBrickTile");
        /// <summary>Full Path: Aequus/Content/Biomes/RadonBiome/Tiles/RadonMossGrass</summary>
        public static readonly TextureAsset RadonMossGrass = new("Aequus/Content/Biomes/RadonBiome/Tiles/RadonMossGrass");
        /// <summary>Full Path: Aequus/Content/Biomes/RadonBiome/Tiles/RadonMossTile</summary>
        public static readonly TextureAsset RadonMossTile = new("Aequus/Content/Biomes/RadonBiome/Tiles/RadonMossTile");
        /// <summary>Full Path: Aequus/Content/Biomes/RadonBiome/Tiles/RadonPlantTile</summary>
        public static readonly TextureAsset RadonPlantTile = new("Aequus/Content/Biomes/RadonBiome/Tiles/RadonPlantTile");
        /// <summary>Full Path: Aequus/Items/Misc/Dyes/RainbowOutlineDye</summary>
        public static readonly TextureAsset RainbowOutlineDye = new("Aequus/Items/Misc/Dyes/RainbowOutlineDye");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/Fishing/Ramishroom</summary>
        public static readonly TextureAsset Ramishroom = new("Aequus/Items/Accessories/Misc/Fishing/Ramishroom");
        /// <summary>Full Path: Aequus/Projectiles/Misc/Bobbers/RamishroomBobber</summary>
        public static readonly TextureAsset RamishroomBobber = new("Aequus/Projectiles/Misc/Bobbers/RamishroomBobber");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/Raygun</summary>
        public static readonly TextureAsset Raygun = new("Aequus/Items/Weapons/Ranged/Raygun");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/BattleAxe/RecordBreaker</summary>
        public static readonly TextureAsset RecordBreaker = new("Aequus/Items/Weapons/Melee/BattleAxe/RecordBreaker");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/BattleAxe/RecordBreakerProj</summary>
        public static readonly TextureAsset RecordBreakerProj = new("Aequus/Items/Weapons/Melee/BattleAxe/RecordBreakerProj");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/BattleAxe/RecordBreakerProj_Trail</summary>
        public static readonly TextureAsset RecordBreakerProj_Trail = new("Aequus/Items/Weapons/Melee/BattleAxe/RecordBreakerProj_Trail");
        /// <summary>Full Path: Aequus/Tiles/CraftingStations/RecyclingMachine</summary>
        public static readonly TextureAsset RecyclingMachine = new("Aequus/Tiles/CraftingStations/RecyclingMachine");
        /// <summary>Full Path: Aequus/Tiles/CraftingStations/RecyclingMachineTile</summary>
        public static readonly TextureAsset RecyclingMachineTile = new("Aequus/Tiles/CraftingStations/RecyclingMachineTile");
        /// <summary>Full Path: Aequus/Tiles/CraftingStations/RecyclingMachineTile_Highlight</summary>
        public static readonly TextureAsset RecyclingMachineTile_Highlight = new("Aequus/Tiles/CraftingStations/RecyclingMachineTile_Highlight");
        /// <summary>Full Path: Aequus/Assets/UI/BossChecklist/RedSprite</summary>
        public static readonly TextureAsset RedSprite_BossChecklist = new("Aequus/Assets/UI/BossChecklist/RedSprite");
        /// <summary>Full Path: Aequus/NPCs/RedSprite/RedSprite_Glow</summary>
        public static readonly TextureAsset RedSprite_Glow = new("Aequus/NPCs/RedSprite/RedSprite_Glow");
        /// <summary>Full Path: Aequus/NPCs/RedSprite/RedSprite_Head_Boss</summary>
        public static readonly TextureAsset RedSprite_Head_Boss = new("Aequus/NPCs/RedSprite/RedSprite_Head_Boss");
        /// <summary>Full Path: Aequus/NPCs/RedSprite/RedSprite</summary>
        public static readonly TextureAsset RedSprite_RedSprite = new("Aequus/NPCs/RedSprite/RedSprite");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/RedSprite/RedSpriteBuff</summary>
        public static readonly TextureAsset RedSpriteBuff = new("Aequus/Items/Vanity/Pets/RedSprite/RedSpriteBuff");
        /// <summary>Full Path: Aequus/NPCs/RedSprite/Projectiles/RedSpriteCloud</summary>
        public static readonly TextureAsset RedSpriteCloud = new("Aequus/NPCs/RedSprite/Projectiles/RedSpriteCloud");
        /// <summary>Full Path: Aequus/NPCs/RedSprite/Projectiles/RedSpriteCloudLightning</summary>
        public static readonly TextureAsset RedSpriteCloudLightning = new("Aequus/NPCs/RedSprite/Projectiles/RedSpriteCloudLightning");
        /// <summary>Full Path: Aequus/Particles/Dusts/RedSpriteDust</summary>
        public static readonly TextureAsset RedSpriteDust = new("Aequus/Particles/Dusts/RedSpriteDust");
        /// <summary>Full Path: Aequus/NPCs/RedSprite/RedSpriteFriendly</summary>
        public static readonly TextureAsset RedSpriteFriendly = new("Aequus/NPCs/RedSprite/RedSpriteFriendly");
        /// <summary>Full Path: Aequus/NPCs/RedSprite/Rewards/RedSpriteMask</summary>
        public static readonly TextureAsset RedSpriteMask = new("Aequus/NPCs/RedSprite/Rewards/RedSpriteMask");
        /// <summary>Full Path: Aequus/NPCs/RedSprite/Rewards/RedSpriteMask_Head</summary>
        public static readonly TextureAsset RedSpriteMask_Head = new("Aequus/NPCs/RedSprite/Rewards/RedSpriteMask_Head");
        /// <summary>Full Path: Aequus/Projectiles/Summon/RedSpriteMinion</summary>
        public static readonly TextureAsset RedSpriteMinion = new("Aequus/Projectiles/Summon/RedSpriteMinion");
        /// <summary>Full Path: Aequus/Buffs/Minion/RedSpriteMinionBuff</summary>
        public static readonly TextureAsset RedSpriteMinionBuff = new("Aequus/Buffs/Minion/RedSpriteMinionBuff");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/RedSprite/RedSpritePet</summary>
        public static readonly TextureAsset RedSpritePet = new("Aequus/Items/Vanity/Pets/RedSprite/RedSpritePet");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/RedSprite/RedSpritePet_Glow</summary>
        public static readonly TextureAsset RedSpritePet_Glow = new("Aequus/Items/Vanity/Pets/RedSprite/RedSpritePet_Glow");
        /// <summary>Full Path: Aequus/NPCs/RedSprite/Rewards/RedSpriteRelic</summary>
        public static readonly TextureAsset RedSpriteRelic = new("Aequus/NPCs/RedSprite/Rewards/RedSpriteRelic");
        /// <summary>Full Path: Aequus/NPCs/RedSprite/Projectiles/RedSpriteThunderClap</summary>
        public static readonly TextureAsset RedSpriteThunderClap = new("Aequus/NPCs/RedSprite/Projectiles/RedSpriteThunderClap");
        /// <summary>Full Path: Aequus/NPCs/RedSprite/Rewards/RedSpriteTrophy</summary>
        public static readonly TextureAsset RedSpriteTrophy = new("Aequus/NPCs/RedSprite/Rewards/RedSpriteTrophy");
        /// <summary>Full Path: Aequus/NPCs/RedSprite/Projectiles/RedSpriteWindFire</summary>
        public static readonly TextureAsset RedSpriteWindFire = new("Aequus/NPCs/RedSprite/Projectiles/RedSpriteWindFire");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/Fishing/RegrowingBait</summary>
        public static readonly TextureAsset RegrowingBait = new("Aequus/Items/Accessories/Misc/Fishing/RegrowingBait");
        /// <summary>Full Path: Aequus/Particles/Dusts/ReliefshroomDustSpore</summary>
        public static readonly TextureAsset ReliefshroomDustSpore = new("Aequus/Particles/Dusts/ReliefshroomDustSpore");
        /// <summary>Full Path: Aequus/NPCs/Town/SkyMerchantNPC/NameTags/RenameBackIcon</summary>
        public static readonly TextureAsset RenameBackIcon = new("Aequus/NPCs/Town/SkyMerchantNPC/NameTags/RenameBackIcon");
        /// <summary>Full Path: Aequus/NPCs/Town/ExporterNPC/RerollSystem/RerollButton</summary>
        public static readonly TextureAsset RerollButton = new("Aequus/NPCs/Town/ExporterNPC/RerollSystem/RerollButton");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/Thrown/Resonance</summary>
        public static readonly TextureAsset Resonance = new("Aequus/Items/Weapons/Melee/Thrown/Resonance");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Scepters/Revenant</summary>
        public static readonly TextureAsset Revenant = new("Aequus/Items/Weapons/Necromancy/Scepters/Revenant");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Scepters/Revenant_Glow</summary>
        public static readonly TextureAsset Revenant_Glow = new("Aequus/Items/Weapons/Necromancy/Scepters/Revenant_Glow");
        /// <summary>Full Path: Aequus/Items/Accessories/Misc/Money/RichMansMonocle</summary>
        public static readonly TextureAsset RichMansMonocle = new("Aequus/Items/Accessories/Misc/Money/RichMansMonocle");
        /// <summary>Full Path: Aequus/Buffs/Misc/RitualBuff</summary>
        public static readonly TextureAsset RitualBuff = new("Aequus/Buffs/Misc/RitualBuff");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Necro/RitualisticSkull</summary>
        public static readonly TextureAsset RitualisticSkull = new("Aequus/Items/Accessories/Combat/Necro/RitualisticSkull");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/RockMan</summary>
        public static readonly TextureAsset RockMan = new("Aequus/Items/Weapons/Melee/RockMan");
        /// <summary>Full Path: Aequus/Unused/Items/SlotMachines/Roulette</summary>
        public static readonly TextureAsset Roulette = new("Aequus/Unused/Items/SlotMachines/Roulette");
        /// <summary>Full Path: Aequus/Tiles/Misc/BigGems/RubyDeposit</summary>
        public static readonly TextureAsset RubyDeposit = new("Aequus/Tiles/Misc/BigGems/RubyDeposit");
        /// <summary>Full Path: Aequus/Assets/Gores/Gems/RubyGore</summary>
        public static readonly TextureAsset RubyGore = new("Aequus/Assets/Gores/Gems/RubyGore");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/SandstormStaff</summary>
        public static readonly TextureAsset SandstormStaff = new("Aequus/Items/Weapons/Magic/SandstormStaff");
        /// <summary>Full Path: Aequus/Tiles/Misc/BigGems/SapphireDeposit</summary>
        public static readonly TextureAsset SapphireDeposit = new("Aequus/Tiles/Misc/BigGems/SapphireDeposit");
        /// <summary>Full Path: Aequus/Assets/Gores/Gems/SapphireGore</summary>
        public static readonly TextureAsset SapphireGore = new("Aequus/Assets/Gores/Gems/SapphireGore");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/Healer/SavingGrace</summary>
        public static readonly TextureAsset SavingGrace = new("Aequus/Items/Weapons/Magic/Healer/SavingGrace");
        /// <summary>Full Path: Aequus/Projectiles/Magic/SavingGraceProj</summary>
        public static readonly TextureAsset SavingGraceProj = new("Aequus/Projectiles/Magic/SavingGraceProj");
        /// <summary>Full Path: Aequus/Projectiles/Magic/SavingGraceProj_Aura</summary>
        public static readonly TextureAsset SavingGraceProj_Aura = new("Aequus/Projectiles/Magic/SavingGraceProj_Aura");
        /// <summary>Full Path: Aequus/Projectiles/Magic/SavingGraceProj_GlowIndicator</summary>
        public static readonly TextureAsset SavingGraceProj_GlowIndicator = new("Aequus/Projectiles/Magic/SavingGraceProj_GlowIndicator");
        /// <summary>Full Path: Aequus/Items/Consumables/ScarabDynamite</summary>
        public static readonly TextureAsset ScarabDynamite = new("Aequus/Items/Consumables/ScarabDynamite");
        /// <summary>Full Path: Aequus/Items/Misc/Dyes/ScorchingDye</summary>
        public static readonly TextureAsset ScorchingDye = new("Aequus/Items/Misc/Dyes/ScorchingDye");
        /// <summary>Full Path: Aequus/Items/Weapons/Summon/Minion/ScribbleNotebook</summary>
        public static readonly TextureAsset ScribbleNotebook = new("Aequus/Items/Weapons/Summon/Minion/ScribbleNotebook");
        /// <summary>Full Path: Aequus/Items/Weapons/Summon/Minion/ScribbleNotebook_Glow</summary>
        public static readonly TextureAsset ScribbleNotebook_Glow = new("Aequus/Items/Weapons/Summon/Minion/ScribbleNotebook_Glow");
        /// <summary>Full Path: Aequus/Buffs/Minion/ScribbleNotebookBuff</summary>
        public static readonly TextureAsset ScribbleNotebookBuff = new("Aequus/Buffs/Minion/ScribbleNotebookBuff");
        /// <summary>Full Path: Aequus/Projectiles/Summon/ScribbleNotebookMinion</summary>
        public static readonly TextureAsset ScribbleNotebookMinion = new("Aequus/Projectiles/Summon/ScribbleNotebookMinion");
        /// <summary>Full Path: Aequus/Items/Misc/Dyes/ScrollDye</summary>
        public static readonly TextureAsset ScrollDye = new("Aequus/Items/Misc/Dyes/ScrollDye");
        /// <summary>Full Path: Aequus/Tiles/CrabCrevice/SeaPickleItem</summary>
        public static readonly TextureAsset SeaPickleItem = new("Aequus/Tiles/CrabCrevice/SeaPickleItem");
        /// <summary>Full Path: Aequus/Tiles/CrabCrevice/SeaPickleTile</summary>
        public static readonly TextureAsset SeaPickleTile = new("Aequus/Tiles/CrabCrevice/SeaPickleTile");
        /// <summary>Full Path: Aequus/Content/Building/Bonuses/SecretEntranceBuff</summary>
        public static readonly TextureAsset SecretEntranceBuff = new("Aequus/Content/Building/Bonuses/SecretEntranceBuff");
        /// <summary>Full Path: Aequus/Tiles/CrabCrevice/SedimentaryRockItem</summary>
        public static readonly TextureAsset SedimentaryRockItem = new("Aequus/Tiles/CrabCrevice/SedimentaryRockItem");
        /// <summary>Full Path: Aequus/Tiles/CrabCrevice/SedimentaryRockTile</summary>
        public static readonly TextureAsset SedimentaryRockTile = new("Aequus/Tiles/CrabCrevice/SedimentaryRockTile");
        /// <summary>Full Path: Aequus/Tiles/CrabCrevice/SedimentaryRockWallItem</summary>
        public static readonly TextureAsset SedimentaryRockWallItem = new("Aequus/Tiles/CrabCrevice/SedimentaryRockWallItem");
        /// <summary>Full Path: Aequus/Tiles/CrabCrevice/SedimentaryRockWallPlaced</summary>
        public static readonly TextureAsset SedimentaryRockWallPlaced = new("Aequus/Tiles/CrabCrevice/SedimentaryRockWallPlaced");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Sentry/EquipmentChips/Sentinel6510</summary>
        public static readonly TextureAsset Sentinel6510 = new("Aequus/Items/Accessories/Combat/Sentry/EquipmentChips/Sentinel6510");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Sentry/EquipmentChips/Sentinel6510_Eyes</summary>
        public static readonly TextureAsset Sentinel6510_Eyes = new("Aequus/Items/Accessories/Combat/Sentry/EquipmentChips/Sentinel6510_Eyes");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Sentry/EquipmentChips/Sentry6502</summary>
        public static readonly TextureAsset Sentry6502 = new("Aequus/Items/Accessories/Combat/Sentry/EquipmentChips/Sentry6502");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Sentry/EquipmentChips/Sentry6502_Eyes</summary>
        public static readonly TextureAsset Sentry6502_Eyes = new("Aequus/Items/Accessories/Combat/Sentry/EquipmentChips/Sentry6502_Eyes");
        /// <summary>Full Path: Aequus/Unused/Items/SentryPotion</summary>
        public static readonly TextureAsset SentryPotion = new("Aequus/Unused/Items/SentryPotion");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Sentry/SentrySquid</summary>
        public static readonly TextureAsset SentrySquid = new("Aequus/Items/Accessories/Combat/Sentry/SentrySquid");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Sentry/SentrySquid_Hat</summary>
        public static readonly TextureAsset SentrySquid_Hat = new("Aequus/Items/Accessories/Combat/Sentry/SentrySquid_Hat");
        /// <summary>Full Path: Aequus/Items/Armor/SetSeraphim/SeraphimHood</summary>
        public static readonly TextureAsset SeraphimHood = new("Aequus/Items/Armor/SetSeraphim/SeraphimHood");
        /// <summary>Full Path: Aequus/Items/Armor/SetSeraphim/SeraphimHood_Head</summary>
        public static readonly TextureAsset SeraphimHood_Head = new("Aequus/Items/Armor/SetSeraphim/SeraphimHood_Head");
        /// <summary>Full Path: Aequus/Items/Armor/SetSeraphim/SeraphimRobes</summary>
        public static readonly TextureAsset SeraphimRobes = new("Aequus/Items/Armor/SetSeraphim/SeraphimRobes");
        /// <summary>Full Path: Aequus/Items/Armor/SetSeraphim/SeraphimRobes_Body</summary>
        public static readonly TextureAsset SeraphimRobes_Body = new("Aequus/Items/Armor/SetSeraphim/SeraphimRobes_Body");
        /// <summary>Full Path: Aequus/Unused/Items/SlotMachines/ShadowRoulette</summary>
        public static readonly TextureAsset ShadowRoulette = new("Aequus/Unused/Items/SlotMachines/ShadowRoulette");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Necro/ShadowVeer</summary>
        public static readonly TextureAsset ShadowVeer = new("Aequus/Items/Accessories/Combat/Necro/ShadowVeer");
        /// <summary>Full Path: Aequus/Assets/Shatter</summary>
        public static readonly TextureAsset Shatter = new("Aequus/Assets/Shatter");
        /// <summary>Full Path: Aequus/Items/Consumables/ShimmerFish</summary>
        public static readonly TextureAsset ShimmerFish = new("Aequus/Items/Consumables/ShimmerFish");
        /// <summary>Full Path: Aequus/Items/Consumables/ShimmerSundialCharge</summary>
        public static readonly TextureAsset ShimmerSundialCharge = new("Aequus/Items/Consumables/ShimmerSundialCharge");
        /// <summary>Full Path: Aequus/Particles/ShinyFlashParticle</summary>
        public static readonly TextureAsset ShinyFlashParticle = new("Aequus/Particles/ShinyFlashParticle");
        /// <summary>Full Path: Aequus/Items/Tools/CarpenterCamera/Shutterstocker</summary>
        public static readonly TextureAsset Shutterstocker = new("Aequus/Items/Tools/CarpenterCamera/Shutterstocker");
        /// <summary>Full Path: Aequus/Items/Tools/CarpenterCamera/ShutterstockerCameraProj</summary>
        public static readonly TextureAsset ShutterstockerCameraProj = new("Aequus/Items/Tools/CarpenterCamera/ShutterstockerCameraProj");
        /// <summary>Full Path: Aequus/Unused/Items/ShutterstockerClip</summary>
        public static readonly TextureAsset ShutterstockerClip = new("Aequus/Unused/Items/ShutterstockerClip");
        /// <summary>Full Path: Aequus/Items/Tools/CarpenterCamera/ShutterstockerHeldProj</summary>
        public static readonly TextureAsset ShutterstockerHeldProj = new("Aequus/Items/Tools/CarpenterCamera/ShutterstockerHeldProj");
        /// <summary>Full Path: Aequus/Items/Tools/CarpenterCamera/ShutterstockerIcons</summary>
        public static readonly TextureAsset ShutterstockerIcons = new("Aequus/Items/Tools/CarpenterCamera/ShutterstockerIcons");
        /// <summary>Full Path: Aequus/Assets/UI/ShutterstockerInterface</summary>
        public static readonly TextureAsset ShutterstockerInterface = new("Aequus/Assets/UI/ShutterstockerInterface");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/Thrown/SickBeat</summary>
        public static readonly TextureAsset SickBeat = new("Aequus/Items/Weapons/Melee/Thrown/SickBeat");
        /// <summary>Full Path: Aequus/Unused/Items/SilkHammer</summary>
        public static readonly TextureAsset SilkHammer = new("Aequus/Unused/Items/SilkHammer");
        /// <summary>Full Path: Aequus/Unused/Items/SilkPickaxe</summary>
        public static readonly TextureAsset SilkPickaxe = new("Aequus/Unused/Items/SilkPickaxe");
        /// <summary>Full Path: Aequus/Items/Misc/Dyes/SimplifiedDye</summary>
        public static readonly TextureAsset SimplifiedDye = new("Aequus/Items/Misc/Dyes/SimplifiedDye");
        /// <summary>Full Path: Aequus/Items/Tools/SkeletonKey</summary>
        public static readonly TextureAsset SkeletonKey = new("Aequus/Items/Tools/SkeletonKey");
        /// <summary>Full Path: Aequus/Assets/UI/SkeletonMerchantHead</summary>
        public static readonly TextureAsset SkeletonMerchantHead = new("Aequus/Assets/UI/SkeletonMerchantHead");
        /// <summary>Full Path: Aequus/NPCs/Town/SkyMerchantNPC/SkyMerchant</summary>
        public static readonly TextureAsset SkyMerchant = new("Aequus/NPCs/Town/SkyMerchantNPC/SkyMerchant");
        /// <summary>Full Path: Aequus/NPCs/Town/SkyMerchantNPC/SkyMerchant_Head</summary>
        public static readonly TextureAsset SkyMerchant_Head = new("Aequus/NPCs/Town/SkyMerchantNPC/SkyMerchant_Head");
        /// <summary>Full Path: Aequus/NPCs/Town/SkyMerchantNPC/SkyMerchant_Resprite</summary>
        public static readonly TextureAsset SkyMerchant_Resprite = new("Aequus/NPCs/Town/SkyMerchantNPC/SkyMerchant_Resprite");
        /// <summary>Full Path: Aequus/NPCs/Town/SkyMerchantNPC/SkyMerchantBalloon</summary>
        public static readonly TextureAsset SkyMerchantBalloon = new("Aequus/NPCs/Town/SkyMerchantNPC/SkyMerchantBalloon");
        /// <summary>Full Path: Aequus/NPCs/Town/SkyMerchantNPC/SkyMerchantBasket</summary>
        public static readonly TextureAsset SkyMerchantBasket = new("Aequus/NPCs/Town/SkyMerchantNPC/SkyMerchantBasket");
        /// <summary>Full Path: Aequus/NPCs/Town/SkyMerchantNPC/SkyMerchantFlee</summary>
        public static readonly TextureAsset SkyMerchantFlee = new("Aequus/NPCs/Town/SkyMerchantNPC/SkyMerchantFlee");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Paintings/Items/SkyrimRock1</summary>
        public static readonly TextureAsset SkyrimRock1 = new("Aequus/Tiles/Furniture/Paintings/Items/SkyrimRock1");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Paintings/Items/SkyrimRock2</summary>
        public static readonly TextureAsset SkyrimRock2 = new("Aequus/Tiles/Furniture/Paintings/Items/SkyrimRock2");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Paintings/Items/SkyrimRock3</summary>
        public static readonly TextureAsset SkyrimRock3 = new("Aequus/Tiles/Furniture/Paintings/Items/SkyrimRock3");
        /// <summary>Full Path: Aequus/Unused/Items/SlotMachines/SkyRoulette</summary>
        public static readonly TextureAsset SkyRoulette = new("Aequus/Unused/Items/SlotMachines/SkyRoulette");
        /// <summary>Full Path: Aequus/Assets/Textures/SlamEffect0</summary>
        public static readonly TextureAsset SlamEffect0 = new("Aequus/Assets/Textures/SlamEffect0");
        /// <summary>Full Path: Aequus/Assets/Textures/SlamEffect1</summary>
        public static readonly TextureAsset SlamEffect1 = new("Aequus/Assets/Textures/SlamEffect1");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/Slice</summary>
        public static readonly TextureAsset Slice = new("Aequus/Items/Weapons/Melee/Slice");
        /// <summary>Full Path: Aequus/Projectiles/Melee/Swords/SliceBulletProj</summary>
        public static readonly TextureAsset SliceBulletProj = new("Aequus/Projectiles/Melee/Swords/SliceBulletProj");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/Misc/Slingsaber</summary>
        public static readonly TextureAsset Slingsaber = new("Aequus/Items/Weapons/Ranged/Misc/Slingsaber");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/Misc/Slingsaber_Glow</summary>
        public static readonly TextureAsset Slingsaber_Glow = new("Aequus/Items/Weapons/Ranged/Misc/Slingsaber_Glow");
        /// <summary>Full Path: Aequus/Projectiles/Ranged/SlingsaberProj</summary>
        public static readonly TextureAsset SlingsaberProj = new("Aequus/Projectiles/Ranged/SlingsaberProj");
        /// <summary>Full Path: Aequus/Projectiles/Ranged/SlingsaberProj_Glow</summary>
        public static readonly TextureAsset SlingsaberProj_Glow = new("Aequus/Projectiles/Ranged/SlingsaberProj_Glow");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/Misc/Slingshot</summary>
        public static readonly TextureAsset Slingshot = new("Aequus/Items/Weapons/Ranged/Misc/Slingshot");
        /// <summary>Full Path: Aequus/Projectiles/Ranged/SlingshotProj</summary>
        public static readonly TextureAsset SlingshotProj = new("Aequus/Projectiles/Ranged/SlingshotProj");
        /// <summary>Full Path: Aequus/NPCs/Critters/Snobster</summary>
        public static readonly TextureAsset Snobster = new("Aequus/NPCs/Critters/Snobster");
        /// <summary>Full Path: Aequus/NPCs/Critters/SnobsterItem</summary>
        public static readonly TextureAsset SnobsterItem = new("Aequus/NPCs/Critters/SnobsterItem");
        /// <summary>Full Path: Aequus/Buffs/Minion/SnowflakeBuff</summary>
        public static readonly TextureAsset SnowflakeBuff = new("Aequus/Buffs/Minion/SnowflakeBuff");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/SnowflakeCannon</summary>
        public static readonly TextureAsset SnowflakeCannon = new("Aequus/Items/Weapons/Ranged/SnowflakeCannon");
        /// <summary>Full Path: Aequus/Projectiles/Summon/SnowflakeMinion</summary>
        public static readonly TextureAsset SnowflakeMinion = new("Aequus/Projectiles/Summon/SnowflakeMinion");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/Snowgrave</summary>
        public static readonly TextureAsset Snowgrave = new("Aequus/Items/Weapons/Magic/Snowgrave");
        /// <summary>Full Path: Aequus/Projectiles/Magic/SnowgraveProj</summary>
        public static readonly TextureAsset SnowgraveProj = new("Aequus/Projectiles/Magic/SnowgraveProj");
        /// <summary>Full Path: Aequus/Unused/Items/SlotMachines/SnowRoulette</summary>
        public static readonly TextureAsset SnowRoulette = new("Aequus/Unused/Items/SlotMachines/SnowRoulette");
        /// <summary>Full Path: Aequus/NPCs/Monsters/CrabCrevice/SoldierCrab</summary>
        public static readonly TextureAsset SoldierCrab = new("Aequus/NPCs/Monsters/CrabCrevice/SoldierCrab");
        /// <summary>Full Path: Aequus/Tiles/Banners/Items/SoldierCrabBanner</summary>
        public static readonly TextureAsset SoldierCrabBanner = new("Aequus/Tiles/Banners/Items/SoldierCrabBanner");
        /// <summary>Full Path: Aequus/Projectiles/Monster/SoldierCrabProj</summary>
        public static readonly TextureAsset SoldierCrabProj = new("Aequus/Projectiles/Monster/SoldierCrabProj");
        /// <summary>Full Path: Aequus/Assets/SoulChains</summary>
        public static readonly TextureAsset SoulChains = new("Aequus/Assets/SoulChains");
        /// <summary>Full Path: Aequus/Items/Materials/Gems/SoulGem</summary>
        public static readonly TextureAsset SoulGem = new("Aequus/Items/Materials/Gems/SoulGem");
        /// <summary>Full Path: Aequus/Items/Materials/Gems/SoulGemTile</summary>
        public static readonly TextureAsset SoulGemTile = new("Aequus/Items/Materials/Gems/SoulGemTile");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Necro/SouljointCuffs</summary>
        public static readonly TextureAsset SouljointCuffs = new("Aequus/Items/Accessories/Combat/Necro/SouljointCuffs");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/SoulNeglectite</summary>
        public static readonly TextureAsset SoulNeglectite = new("Aequus/Items/Accessories/Combat/SoulNeglectite");
        /// <summary>Full Path: Aequus/Assets/UI/BossChecklist/SpaceSquid</summary>
        public static readonly TextureAsset SpaceSquid_BossChecklist = new("Aequus/Assets/UI/BossChecklist/SpaceSquid");
        /// <summary>Full Path: Aequus/NPCs/SpaceSquid/SpaceSquid_Glow</summary>
        public static readonly TextureAsset SpaceSquid_Glow = new("Aequus/NPCs/SpaceSquid/SpaceSquid_Glow");
        /// <summary>Full Path: Aequus/NPCs/SpaceSquid/SpaceSquid_Head_Boss</summary>
        public static readonly TextureAsset SpaceSquid_Head_Boss = new("Aequus/NPCs/SpaceSquid/SpaceSquid_Head_Boss");
        /// <summary>Full Path: Aequus/NPCs/SpaceSquid/SpaceSquid</summary>
        public static readonly TextureAsset SpaceSquid_SpaceSquid = new("Aequus/NPCs/SpaceSquid/SpaceSquid");
        /// <summary>Full Path: Aequus/Particles/Dusts/SpaceSquidBlood</summary>
        public static readonly TextureAsset SpaceSquidBlood = new("Aequus/Particles/Dusts/SpaceSquidBlood");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/SpaceSquid/SpaceSquidBuff</summary>
        public static readonly TextureAsset SpaceSquidBuff = new("Aequus/Items/Vanity/Pets/SpaceSquid/SpaceSquidBuff");
        /// <summary>Full Path: Aequus/NPCs/SpaceSquid/Projectiles/SpaceSquidDeathray</summary>
        public static readonly TextureAsset SpaceSquidDeathray = new("Aequus/NPCs/SpaceSquid/Projectiles/SpaceSquidDeathray");
        /// <summary>Full Path: Aequus/NPCs/SpaceSquid/SpaceSquidDefeated</summary>
        public static readonly TextureAsset SpaceSquidDefeated = new("Aequus/NPCs/SpaceSquid/SpaceSquidDefeated");
        /// <summary>Full Path: Aequus/NPCs/SpaceSquid/SpaceSquidDefeated_Glow</summary>
        public static readonly TextureAsset SpaceSquidDefeated_Glow = new("Aequus/NPCs/SpaceSquid/SpaceSquidDefeated_Glow");
        /// <summary>Full Path: Aequus/NPCs/SpaceSquid/SpaceSquidFriendly</summary>
        public static readonly TextureAsset SpaceSquidFriendly = new("Aequus/NPCs/SpaceSquid/SpaceSquidFriendly");
        /// <summary>Full Path: Aequus/NPCs/SpaceSquid/Projectiles/SpaceSquidLaser</summary>
        public static readonly TextureAsset SpaceSquidLaser = new("Aequus/NPCs/SpaceSquid/Projectiles/SpaceSquidLaser");
        /// <summary>Full Path: Aequus/NPCs/SpaceSquid/Rewards/SpaceSquidMask</summary>
        public static readonly TextureAsset SpaceSquidMask = new("Aequus/NPCs/SpaceSquid/Rewards/SpaceSquidMask");
        /// <summary>Full Path: Aequus/NPCs/SpaceSquid/Rewards/SpaceSquidMask_Head</summary>
        public static readonly TextureAsset SpaceSquidMask_Head = new("Aequus/NPCs/SpaceSquid/Rewards/SpaceSquidMask_Head");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/SpaceSquid/SpaceSquidPet</summary>
        public static readonly TextureAsset SpaceSquidPet = new("Aequus/Items/Vanity/Pets/SpaceSquid/SpaceSquidPet");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/SpaceSquid/SpaceSquidPet_Glow</summary>
        public static readonly TextureAsset SpaceSquidPet_Glow = new("Aequus/Items/Vanity/Pets/SpaceSquid/SpaceSquidPet_Glow");
        /// <summary>Full Path: Aequus/NPCs/SpaceSquid/Rewards/SpaceSquidRelic</summary>
        public static readonly TextureAsset SpaceSquidRelic = new("Aequus/NPCs/SpaceSquid/Rewards/SpaceSquidRelic");
        /// <summary>Full Path: Aequus/NPCs/SpaceSquid/Projectiles/SpaceSquidSnowflake</summary>
        public static readonly TextureAsset SpaceSquidSnowflake = new("Aequus/NPCs/SpaceSquid/Projectiles/SpaceSquidSnowflake");
        /// <summary>Full Path: Aequus/NPCs/SpaceSquid/Rewards/SpaceSquidTrophy</summary>
        public static readonly TextureAsset SpaceSquidTrophy = new("Aequus/NPCs/SpaceSquid/Rewards/SpaceSquidTrophy");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/SpamMail</summary>
        public static readonly TextureAsset SpamMail = new("Aequus/Items/Weapons/Magic/SpamMail");
        /// <summary>Full Path: Aequus/Items/Consumables/Foods/SpicyEel</summary>
        public static readonly TextureAsset SpicyEel = new("Aequus/Items/Consumables/Foods/SpicyEel");
        /// <summary>Full Path: Aequus/Buffs/SpicyEelBuff</summary>
        public static readonly TextureAsset SpicyEelBuff = new("Aequus/Buffs/SpicyEelBuff");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Necro/SpiritBottle</summary>
        public static readonly TextureAsset SpiritBottle = new("Aequus/Items/Accessories/Combat/Necro/SpiritBottle");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Necro/SpiritKeg</summary>
        public static readonly TextureAsset SpiritKeg = new("Aequus/Items/Accessories/Combat/Necro/SpiritKeg");
        /// <summary>Full Path: Aequus/Content/ItemPrefixes/Potions/SplashGlint</summary>
        public static readonly TextureAsset SplashGlint = new("Aequus/Content/ItemPrefixes/Potions/SplashGlint");
        /// <summary>Full Path: Aequus/Items/Tools/Poles/Starcatcher</summary>
        public static readonly TextureAsset Starcatcher = new("Aequus/Items/Tools/Poles/Starcatcher");
        /// <summary>Full Path: Aequus/Projectiles/Misc/Bobbers/StarcatcherBobber</summary>
        public static readonly TextureAsset StarcatcherBobber = new("Aequus/Projectiles/Misc/Bobbers/StarcatcherBobber");
        /// <summary>Full Path: Aequus/NPCs/Monsters/Event/Glimmer/Starite</summary>
        public static readonly TextureAsset Starite = new("Aequus/NPCs/Monsters/Event/Glimmer/Starite");
        /// <summary>Full Path: Aequus/Tiles/Banners/Items/StariteBanner</summary>
        public static readonly TextureAsset StariteBanner = new("Aequus/Tiles/Banners/Items/StariteBanner");
        /// <summary>Full Path: Aequus/Tiles/Misc/StariteBottle</summary>
        public static readonly TextureAsset StariteBottle = new("Aequus/Tiles/Misc/StariteBottle");
        /// <summary>Full Path: Aequus/Buffs/StariteBottleBuff</summary>
        public static readonly TextureAsset StariteBottleBuff = new("Aequus/Buffs/StariteBottleBuff");
        /// <summary>Full Path: Aequus/Tiles/Misc/StariteBottleTile</summary>
        public static readonly TextureAsset StariteBottleTile = new("Aequus/Tiles/Misc/StariteBottleTile");
        /// <summary>Full Path: Aequus/Tiles/Misc/StariteBottleTile_Glow</summary>
        public static readonly TextureAsset StariteBottleTile_Glow = new("Aequus/Tiles/Misc/StariteBottleTile_Glow");
        /// <summary>Full Path: Aequus/Buffs/Minion/StariteBuff</summary>
        public static readonly TextureAsset StariteBuff = new("Aequus/Buffs/Minion/StariteBuff");
        /// <summary>Full Path: Aequus/Items/Materials/Glimmer/StariteMaterial</summary>
        public static readonly TextureAsset StariteMaterial = new("Aequus/Items/Materials/Glimmer/StariteMaterial");
        /// <summary>Full Path: Aequus/Projectiles/Summon/StariteMinion</summary>
        public static readonly TextureAsset StariteMinion = new("Aequus/Projectiles/Summon/StariteMinion");
        /// <summary>Full Path: Aequus/Items/Weapons/Summon/Minion/StariteStaff</summary>
        public static readonly TextureAsset StariteStaff = new("Aequus/Items/Weapons/Summon/Minion/StariteStaff");
        /// <summary>Full Path: Aequus/Items/Weapons/Summon/Minion/StariteStaff_Glow</summary>
        public static readonly TextureAsset StariteStaff_Glow = new("Aequus/Items/Weapons/Summon/Minion/StariteStaff_Glow");
        /// <summary>Full Path: Aequus/Items/Materials/Glimmer/StariteStrongMaterial</summary>
        public static readonly TextureAsset StariteStrongMaterial = new("Aequus/Items/Materials/Glimmer/StariteStrongMaterial");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/StarPhish</summary>
        public static readonly TextureAsset StarPhish = new("Aequus/Items/Weapons/Ranged/StarPhish");
        /// <summary>Full Path: Aequus/Assets/UI/StatusBubble</summary>
        public static readonly TextureAsset StatusBubble = new("Aequus/Assets/UI/StatusBubble");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/OnHit/CriticalStrike/SteakEyes</summary>
        public static readonly TextureAsset SteakEyes = new("Aequus/Items/Accessories/Combat/OnHit/CriticalStrike/SteakEyes");
        /// <summary>Full Path: Aequus/Items/Accessories/Life/Dodge/Stormcloak</summary>
        public static readonly TextureAsset Stormcloak = new("Aequus/Items/Accessories/Life/Dodge/Stormcloak");
        /// <summary>Full Path: Aequus/Items/Accessories/Life/Dodge/Stormcloak_Back</summary>
        public static readonly TextureAsset Stormcloak_Back = new("Aequus/Items/Accessories/Life/Dodge/Stormcloak_Back");
        /// <summary>Full Path: Aequus/Items/Accessories/Life/Dodge/Stormcloak_Front</summary>
        public static readonly TextureAsset Stormcloak_Front = new("Aequus/Items/Accessories/Life/Dodge/Stormcloak_Front");
        /// <summary>Full Path: Aequus/Buffs/Cooldowns/StormcloakCooldown</summary>
        public static readonly TextureAsset StormcloakCooldown = new("Aequus/Buffs/Cooldowns/StormcloakCooldown");
        /// <summary>Full Path: Aequus/NPCs/Monsters/Event/GaleStreams/StreamingBalloon</summary>
        public static readonly TextureAsset StreamingBalloon = new("Aequus/NPCs/Monsters/Event/GaleStreams/StreamingBalloon");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/StudiesOfTheInkblot</summary>
        public static readonly TextureAsset StudiesOfTheInkblot = new("Aequus/Items/Weapons/Magic/StudiesOfTheInkblot");
        /// <summary>Full Path: Aequus/Content/ItemPrefixes/Potions/StuffedGlint</summary>
        public static readonly TextureAsset StuffedGlint = new("Aequus/Content/ItemPrefixes/Potions/StuffedGlint");
        /// <summary>Full Path: Aequus/NPCs/Monsters/CrabCrevice/SummonerCrab</summary>
        public static readonly TextureAsset SummonerCrab = new("Aequus/NPCs/Monsters/CrabCrevice/SummonerCrab");
        /// <summary>Full Path: Aequus/NPCs/Monsters/CrabCrevice/SummonerCrab_Glow</summary>
        public static readonly TextureAsset SummonerCrab_Glow = new("Aequus/NPCs/Monsters/CrabCrevice/SummonerCrab_Glow");
        /// <summary>Full Path: Aequus/NPCs/Monsters/CrabCrevice/SummonerCrabMinion</summary>
        public static readonly TextureAsset SummonerCrabMinion = new("Aequus/NPCs/Monsters/CrabCrevice/SummonerCrabMinion");
        /// <summary>Full Path: Aequus/NPCs/Monsters/CrabCrevice/SummonerCrabMinion_Glow</summary>
        public static readonly TextureAsset SummonerCrabMinion_Glow = new("Aequus/NPCs/Monsters/CrabCrevice/SummonerCrabMinion_Glow");
        /// <summary>Full Path: Aequus/Items/Armor/SetTrap/SuperDartTrapHat</summary>
        public static readonly TextureAsset SuperDartTrapHat = new("Aequus/Items/Armor/SetTrap/SuperDartTrapHat");
        /// <summary>Full Path: Aequus/Items/Armor/SetTrap/SuperDartTrapHat_Head</summary>
        public static readonly TextureAsset SuperDartTrapHat_Head = new("Aequus/Items/Armor/SetTrap/SuperDartTrapHat_Head");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/OmegaStarite/Misc/SupernovaFruit</summary>
        public static readonly TextureAsset SupernovaFruit = new("Aequus/NPCs/BossMonsters/OmegaStarite/Misc/SupernovaFruit");
        /// <summary>Full Path: Aequus/NPCs/Monsters/Event/Glimmer/SuperStarite</summary>
        public static readonly TextureAsset SuperStarite = new("Aequus/NPCs/Monsters/Event/Glimmer/SuperStarite");
        /// <summary>Full Path: Aequus/Tiles/Banners/Items/SuperStariteBanner</summary>
        public static readonly TextureAsset SuperStariteBanner = new("Aequus/Tiles/Banners/Items/SuperStariteBanner");
        /// <summary>Full Path: Aequus/Projectiles/Monster/SuperStariteBullet</summary>
        public static readonly TextureAsset SuperStariteBullet = new("Aequus/Projectiles/Monster/SuperStariteBullet");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/Heavy/SuperStarSword</summary>
        public static readonly TextureAsset SuperStarSword = new("Aequus/Items/Weapons/Melee/Heavy/SuperStarSword");
        /// <summary>Full Path: Aequus/Projectiles/Melee/SuperStarSwordSlash</summary>
        public static readonly TextureAsset SuperStarSwordSlash = new("Aequus/Projectiles/Melee/SuperStarSwordSlash");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/SurgeRod</summary>
        public static readonly TextureAsset SurgeRod = new("Aequus/Items/Weapons/Magic/SurgeRod");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/SurgeRod_Glow</summary>
        public static readonly TextureAsset SurgeRod_Glow = new("Aequus/Items/Weapons/Magic/SurgeRod_Glow");
        /// <summary>Full Path: Aequus/Projectiles/Magic/SurgeRodProj</summary>
        public static readonly TextureAsset SurgeRodProj = new("Aequus/Projectiles/Magic/SurgeRodProj");
        /// <summary>Full Path: Aequus/Items/Consumables/Foods/SuspiciousLookingSteak</summary>
        public static readonly TextureAsset SuspiciousLookingSteak = new("Aequus/Items/Consumables/Foods/SuspiciousLookingSteak");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/SwagEye/SwagLookingEye</summary>
        public static readonly TextureAsset SwagLookingEye = new("Aequus/Items/Vanity/Pets/SwagEye/SwagLookingEye");
        /// <summary>Full Path: Aequus/Projectiles/Melee/Swords/Swish</summary>
        public static readonly TextureAsset Swish = new("Aequus/Projectiles/Melee/Swords/Swish");
        /// <summary>Full Path: Aequus/Projectiles/Melee/Swords/Swish2</summary>
        public static readonly TextureAsset Swish2 = new("Aequus/Projectiles/Melee/Swords/Swish2");
        /// <summary>Full Path: Aequus/Projectiles/Melee/Swords/SwishNoEdit</summary>
        public static readonly TextureAsset SwishNoEdit = new("Aequus/Projectiles/Melee/Swords/SwishNoEdit");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_10</summary>
        public static readonly TextureAsset SwordCursor_10 = new("Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_10");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_2</summary>
        public static readonly TextureAsset SwordCursor_2 = new("Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_2");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_3</summary>
        public static readonly TextureAsset SwordCursor_3 = new("Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_3");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_6</summary>
        public static readonly TextureAsset SwordCursor_6 = new("Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_6");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_7</summary>
        public static readonly TextureAsset SwordCursor_7 = new("Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_7");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_8</summary>
        public static readonly TextureAsset SwordCursor_8 = new("Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_8");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_9</summary>
        public static readonly TextureAsset SwordCursor_9 = new("Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_9");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/SwordCursor</summary>
        public static readonly TextureAsset SwordCursor_Items = new("Aequus/Content/CursorDyes/Items/SwordCursor");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_Smart</summary>
        public static readonly TextureAsset SwordCursor_Smart = new("Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_Smart");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor</summary>
        public static readonly TextureAsset SwordCursor_SwordCursor = new("Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor");
        /// <summary>Full Path: Aequus/Items/Accessories/CrownOfBlood/Symbol_Blood</summary>
        public static readonly TextureAsset Symbol_Blood = new("Aequus/Items/Accessories/CrownOfBlood/Symbol_Blood");
        /// <summary>Full Path: Aequus/Items/Accessories/CrownOfBlood/Symbol_Remains</summary>
        public static readonly TextureAsset Symbol_Remains = new("Aequus/Items/Accessories/CrownOfBlood/Symbol_Remains");
        /// <summary>Full Path: Aequus/Items/Misc/Trash/TatteredDemonHorn</summary>
        public static readonly TextureAsset TatteredDemonHorn = new("Aequus/Items/Misc/Trash/TatteredDemonHorn");
        /// <summary>Full Path: Aequus/Assets/UI/TextSparkle</summary>
        public static readonly TextureAsset TextSparkle = new("Aequus/Assets/UI/TextSparkle");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/MechPetCombo/TheReconstruction</summary>
        public static readonly TextureAsset TheReconstruction = new("Aequus/Items/Vanity/Pets/MechPetCombo/TheReconstruction");
        /// <summary>Full Path: Aequus/Items/Accessories/CrownOfBlood/Projectiles/ThermiteGel</summary>
        public static readonly TextureAsset ThermiteGel = new("Aequus/Items/Accessories/CrownOfBlood/Projectiles/ThermiteGel");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/ThrashBag/ThrashBag</summary>
        public static readonly TextureAsset ThrashBag = new("Aequus/Items/Weapons/Melee/ThrashBag/ThrashBag");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/ThrashBag/ThrashBag_Grabber</summary>
        public static readonly TextureAsset ThrashBag_Grabber = new("Aequus/Items/Weapons/Melee/ThrashBag/ThrashBag_Grabber");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/Misc/ThunderClap</summary>
        public static readonly TextureAsset ThunderClap = new("Aequus/Items/Weapons/Melee/Misc/ThunderClap");
        /// <summary>Full Path: Aequus/Projectiles/Melee/ThunderClapProj</summary>
        public static readonly TextureAsset ThunderClapProj = new("Aequus/Projectiles/Melee/ThunderClapProj");
        /// <summary>Full Path: Aequus/Projectiles/Melee/ThunderClapProjChain</summary>
        public static readonly TextureAsset ThunderClapProjChain = new("Aequus/Projectiles/Melee/ThunderClapProjChain");
        /// <summary>Full Path: Aequus/Items/Misc/Dyes/TidalDye</summary>
        public static readonly TextureAsset TidalDye_Dyes = new("Aequus/Items/Misc/Dyes/TidalDye");
        /// <summary>Full Path: Aequus/Assets/Effects/Textures/TidalDye</summary>
        public static readonly TextureAsset TidalDye_Textures = new("Aequus/Assets/Effects/Textures/TidalDye");
        /// <summary>Full Path: Aequus/Items/Consumables/Permanent/TinkerersGuidebook</summary>
        public static readonly TextureAsset TinkerersGuidebook = new("Aequus/Items/Consumables/Permanent/TinkerersGuidebook");
        /// <summary>Full Path: Aequus/Buffs/Misc/TonicSpawnratesBuff</summary>
        public static readonly TextureAsset TonicSpawnratesBuff = new("Aequus/Buffs/Misc/TonicSpawnratesBuff");
        /// <summary>Full Path: Aequus/Buffs/Misc/TonicSpawnratesDebuff</summary>
        public static readonly TextureAsset TonicSpawnratesDebuff = new("Aequus/Buffs/Misc/TonicSpawnratesDebuff");
        /// <summary>Full Path: Aequus/Tiles/Misc/BigGems/TopazDeposit</summary>
        public static readonly TextureAsset TopazDeposit = new("Aequus/Tiles/Misc/BigGems/TopazDeposit");
        /// <summary>Full Path: Aequus/Assets/Gores/Gems/TopazGore</summary>
        public static readonly TextureAsset TopazGore = new("Aequus/Assets/Gores/Gems/TopazGore");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/DustDevil/Misc/TornadoInABottle</summary>
        public static readonly TextureAsset TornadoInABottle = new("Aequus/NPCs/BossMonsters/DustDevil/Misc/TornadoInABottle");
        /// <summary>Full Path: Aequus/Projectiles/Summon/Whip/TornadoWhipProj</summary>
        public static readonly TextureAsset TornadoWhipProj = new("Aequus/Projectiles/Summon/Whip/TornadoWhipProj");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/SwagEye/TorraBuff</summary>
        public static readonly TextureAsset TorraBuff = new("Aequus/Items/Vanity/Pets/SwagEye/TorraBuff");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/SwagEye/TorraPet</summary>
        public static readonly TextureAsset TorraPet = new("Aequus/Items/Vanity/Pets/SwagEye/TorraPet");
        /// <summary>Full Path: Aequus/Projectiles/Magic/TouhouBullet</summary>
        public static readonly TextureAsset TouhouBullet = new("Aequus/Projectiles/Magic/TouhouBullet");
        /// <summary>Full Path: Aequus/Assets/TownNPCExclamation</summary>
        public static readonly TextureAsset TownNPCExclamation = new("Aequus/Assets/TownNPCExclamation");
        /// <summary>Full Path: Aequus/Items/Vanity/Pets/SpaceSquid/ToySpaceGun</summary>
        public static readonly TextureAsset ToySpaceGun = new("Aequus/Items/Vanity/Pets/SpaceSquid/ToySpaceGun");
        /// <summary>Full Path: Aequus/Assets/Effects/Prims/Trail0</summary>
        public static readonly TextureAsset Trail0 = new("Aequus/Assets/Effects/Prims/Trail0");
        /// <summary>Full Path: Aequus/Assets/Effects/Prims/Trail1</summary>
        public static readonly TextureAsset Trail1 = new("Aequus/Assets/Effects/Prims/Trail1");
        /// <summary>Full Path: Aequus/Assets/Effects/Prims/Trail2</summary>
        public static readonly TextureAsset Trail2 = new("Aequus/Assets/Effects/Prims/Trail2");
        /// <summary>Full Path: Aequus/Assets/Effects/Prims/Trail3</summary>
        public static readonly TextureAsset Trail3 = new("Aequus/Assets/Effects/Prims/Trail3");
        /// <summary>Full Path: Aequus/Assets/Effects/Prims/Trail4</summary>
        public static readonly TextureAsset Trail4 = new("Aequus/Assets/Effects/Prims/Trail4");
        /// <summary>Full Path: Aequus/NPCs/Monsters/Event/DemonSiege/Trapper</summary>
        public static readonly TextureAsset Trapper = new("Aequus/NPCs/Monsters/Event/DemonSiege/Trapper");
        /// <summary>Full Path: Aequus/Assets/Gores/Trapper_0</summary>
        public static readonly TextureAsset Trapper_0 = new("Aequus/Assets/Gores/Trapper_0");
        /// <summary>Full Path: Aequus/Assets/Gores/Trapper_1</summary>
        public static readonly TextureAsset Trapper_1 = new("Aequus/Assets/Gores/Trapper_1");
        /// <summary>Full Path: Aequus/Assets/Gores/Trapper_2</summary>
        public static readonly TextureAsset Trapper_2 = new("Aequus/Assets/Gores/Trapper_2");
        /// <summary>Full Path: Aequus/Assets/Gores/Trapper_3</summary>
        public static readonly TextureAsset Trapper_3 = new("Aequus/Assets/Gores/Trapper_3");
        /// <summary>Full Path: Aequus/Assets/Gores/Trapper_4</summary>
        public static readonly TextureAsset Trapper_4 = new("Aequus/Assets/Gores/Trapper_4");
        /// <summary>Full Path: Aequus/NPCs/Monsters/Event/DemonSiege/Trapper_Chain</summary>
        public static readonly TextureAsset Trapper_Chain = new("Aequus/NPCs/Monsters/Event/DemonSiege/Trapper_Chain");
        /// <summary>Full Path: Aequus/Projectiles/Monster/TrapperBullet</summary>
        public static readonly TextureAsset TrapperBullet = new("Aequus/Projectiles/Monster/TrapperBullet");
        /// <summary>Full Path: Aequus/NPCs/Monsters/Event/DemonSiege/TrapperImp</summary>
        public static readonly TextureAsset TrapperImp = new("Aequus/NPCs/Monsters/Event/DemonSiege/TrapperImp");
        /// <summary>Full Path: Aequus/Assets/Gores/TrapperImp_0</summary>
        public static readonly TextureAsset TrapperImp_0 = new("Aequus/Assets/Gores/TrapperImp_0");
        /// <summary>Full Path: Aequus/Assets/Gores/TrapperImp_1</summary>
        public static readonly TextureAsset TrapperImp_1 = new("Aequus/Assets/Gores/TrapperImp_1");
        /// <summary>Full Path: Aequus/Assets/Gores/TrapperImp_2</summary>
        public static readonly TextureAsset TrapperImp_2 = new("Aequus/Assets/Gores/TrapperImp_2");
        /// <summary>Full Path: Aequus/Assets/Gores/TrapperImp_3</summary>
        public static readonly TextureAsset TrapperImp_3 = new("Aequus/Assets/Gores/TrapperImp_3");
        /// <summary>Full Path: Aequus/NPCs/Monsters/Event/DemonSiege/TrapperImp_Glow</summary>
        public static readonly TextureAsset TrapperImp_Glow = new("Aequus/NPCs/Monsters/Event/DemonSiege/TrapperImp_Glow");
        /// <summary>Full Path: Aequus/Tiles/Banners/Items/TrapperImpBanner</summary>
        public static readonly TextureAsset TrapperImpBanner = new("Aequus/Tiles/Banners/Items/TrapperImpBanner");
        /// <summary>Full Path: Aequus/NPCs/Monsters/Event/DemonSiege/TrapperImpTail</summary>
        public static readonly TextureAsset TrapperImpTail = new("Aequus/NPCs/Monsters/Event/DemonSiege/TrapperImpTail");
        /// <summary>Full Path: Aequus/NPCs/Monsters/Event/DemonSiege/TrapperImpWings</summary>
        public static readonly TextureAsset TrapperImpWings = new("Aequus/NPCs/Monsters/Event/DemonSiege/TrapperImpWings");
        /// <summary>Full Path: Aequus/NPCs/Monsters/TrapSkeleton</summary>
        public static readonly TextureAsset TrapSkeleton = new("Aequus/NPCs/Monsters/TrapSkeleton");
        /// <summary>Full Path: Aequus/Tiles/Banners/Items/TrapSkeletonBanner</summary>
        public static readonly TextureAsset TrapSkeletonBanner = new("Aequus/Tiles/Banners/Items/TrapSkeletonBanner");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/Demon/Triacanthorn</summary>
        public static readonly TextureAsset Triacanthorn = new("Aequus/Items/Weapons/Magic/Demon/Triacanthorn");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/Demon/Triacanthorn_Glow</summary>
        public static readonly TextureAsset Triacanthorn_Glow = new("Aequus/Items/Weapons/Magic/Demon/Triacanthorn_Glow");
        /// <summary>Full Path: Aequus/Projectiles/Magic/TriacanthornBolt</summary>
        public static readonly TextureAsset TriacanthornBolt = new("Aequus/Projectiles/Magic/TriacanthornBolt");
        /// <summary>Full Path: Aequus/Projectiles/Magic/TriacanthornProj</summary>
        public static readonly TextureAsset TriacanthornProj = new("Aequus/Projectiles/Magic/TriacanthornProj");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Trophies</summary>
        public static readonly TextureAsset Trophies = new("Aequus/NPCs/BossMonsters/Trophies");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/Trophy</summary>
        public static readonly TextureAsset Trophy = new("Aequus/NPCs/BossMonsters/Trophy");
        /// <summary>Full Path: Aequus/Unused/Items/TubOfCookieDough</summary>
        public static readonly TextureAsset TubOfCookieDough = new("Aequus/Unused/Items/TubOfCookieDough");
        /// <summary>Full Path: Aequus/Items/Materials/Energies/UltimateEnergy</summary>
        public static readonly TextureAsset UltimateEnergy = new("Aequus/Items/Materials/Energies/UltimateEnergy");
        /// <summary>Full Path: Aequus/Assets/Effects/Textures/UltimateEnergyGradient</summary>
        public static readonly TextureAsset UltimateEnergyGradient = new("Aequus/Assets/Effects/Textures/UltimateEnergyGradient");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/Heavy/UltimateSword</summary>
        public static readonly TextureAsset UltimateSword = new("Aequus/Items/Weapons/Melee/Heavy/UltimateSword");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/Heavy/UltimateSword_Glow</summary>
        public static readonly TextureAsset UltimateSword_Glow = new("Aequus/Items/Weapons/Melee/Heavy/UltimateSword_Glow");
        /// <summary>Full Path: Aequus/Buffs/Misc/UltimateSwordBuff</summary>
        public static readonly TextureAsset UltimateSwordBuff = new("Aequus/Buffs/Misc/UltimateSwordBuff");
        /// <summary>Full Path: Aequus/Assets/UI/BossChecklist/UltraStarite</summary>
        public static readonly TextureAsset UltraStarite_BossChecklist = new("Aequus/Assets/UI/BossChecklist/UltraStarite");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/UltraStarite/UltraStarite_Head_Boss</summary>
        public static readonly TextureAsset UltraStarite_Head_Boss = new("Aequus/NPCs/BossMonsters/UltraStarite/UltraStarite_Head_Boss");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/UltraStarite/UltraStarite</summary>
        public static readonly TextureAsset UltraStarite_UltraStarite = new("Aequus/NPCs/BossMonsters/UltraStarite/UltraStarite");
        /// <summary>Full Path: Aequus/Unused/Items/UltraStariteBanner</summary>
        public static readonly TextureAsset UltraStariteBanner = new("Aequus/Unused/Items/UltraStariteBanner");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/UltraStarite/Rewards/UltraStariteRelic</summary>
        public static readonly TextureAsset UltraStariteRelic = new("Aequus/NPCs/BossMonsters/UltraStarite/Rewards/UltraStariteRelic");
        /// <summary>Full Path: Aequus/NPCs/BossMonsters/UltraStarite/Rewards/UltraStariteTrophy</summary>
        public static readonly TextureAsset UltraStariteTrophy = new("Aequus/NPCs/BossMonsters/UltraStarite/Rewards/UltraStariteTrophy");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/Umystick</summary>
        public static readonly TextureAsset Umystick = new("Aequus/Items/Weapons/Magic/Umystick");
        /// <summary>Full Path: Aequus/Projectiles/Magic/UmystickBullet</summary>
        public static readonly TextureAsset UmystickBullet = new("Aequus/Projectiles/Magic/UmystickBullet");
        /// <summary>Full Path: Aequus/Projectiles/Magic/UmystickProj</summary>
        public static readonly TextureAsset UmystickProj = new("Aequus/Projectiles/Magic/UmystickProj");
        /// <summary>Full Path: Aequus/Content/Events/DemonSiege/Misc/UnholyCore</summary>
        public static readonly TextureAsset UnholyCore = new("Aequus/Content/Events/DemonSiege/Misc/UnholyCore");
        /// <summary>Full Path: Aequus/Content/Events/DemonSiege/Misc/UnholyCoreSmall</summary>
        public static readonly TextureAsset UnholyCoreSmall = new("Aequus/Content/Events/DemonSiege/Misc/UnholyCoreSmall");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/Thrown/Valari</summary>
        public static readonly TextureAsset Valari = new("Aequus/Items/Weapons/Melee/Thrown/Valari");
        /// <summary>Full Path: Aequus/Content/Vampirism/Items/VampireSquid</summary>
        public static readonly TextureAsset VampireSquid = new("Aequus/Content/Vampirism/Items/VampireSquid");
        /// <summary>Full Path: Aequus/Content/Vampirism/Buffs/VampirismBuff</summary>
        public static readonly TextureAsset VampirismBuff = new("Aequus/Content/Vampirism/Buffs/VampirismBuff");
        /// <summary>Full Path: Aequus/Content/Vampirism/Buffs/VampirismDay</summary>
        public static readonly TextureAsset VampirismDay = new("Aequus/Content/Vampirism/Buffs/VampirismDay");
        /// <summary>Full Path: Aequus/Content/Vampirism/Buffs/VampirismDayRain</summary>
        public static readonly TextureAsset VampirismDayRain = new("Aequus/Content/Vampirism/Buffs/VampirismDayRain");
        /// <summary>Full Path: Aequus/Content/Vampirism/Buffs/VampirismNight</summary>
        public static readonly TextureAsset VampirismNight = new("Aequus/Content/Vampirism/Buffs/VampirismNight");
        /// <summary>Full Path: Aequus/Content/Vampirism/Buffs/VampirismNightEclipse</summary>
        public static readonly TextureAsset VampirismNightEclipse = new("Aequus/Content/Vampirism/Buffs/VampirismNightEclipse");
        /// <summary>Full Path: Aequus/Buffs/VeinminerBuff</summary>
        public static readonly TextureAsset VeinminerBuff = new("Aequus/Buffs/VeinminerBuff");
        /// <summary>Full Path: Aequus/Buffs/VeinminerBuffEmpowered</summary>
        public static readonly TextureAsset VeinminerBuffEmpowered = new("Aequus/Buffs/VeinminerBuffEmpowered");
        /// <summary>Full Path: Aequus/Items/Potions/VeinminerPotion</summary>
        public static readonly TextureAsset VeinminerPotion = new("Aequus/Items/Potions/VeinminerPotion");
        /// <summary>Full Path: Aequus/Items/Armor/SetTrap/VenomDartTrapHat</summary>
        public static readonly TextureAsset VenomDartTrapHat = new("Aequus/Items/Armor/SetTrap/VenomDartTrapHat");
        /// <summary>Full Path: Aequus/Items/Armor/SetTrap/VenomDartTrapHat_Head</summary>
        public static readonly TextureAsset VenomDartTrapHat_Head = new("Aequus/Items/Armor/SetTrap/VenomDartTrapHat_Head");
        /// <summary>Full Path: Aequus/Projectiles/Summon/Misc/VenomDartTrapHatProj</summary>
        public static readonly TextureAsset VenomDartTrapHatProj = new("Aequus/Projectiles/Summon/Misc/VenomDartTrapHatProj");
        /// <summary>Full Path: Aequus/Items/Consumables/Permanent/VictorsReward</summary>
        public static readonly TextureAsset VictorsReward = new("Aequus/Items/Consumables/Permanent/VictorsReward");
        /// <summary>Full Path: Aequus/Assets/VignetteSmall</summary>
        public static readonly TextureAsset VignetteSmall = new("Aequus/Assets/VignetteSmall");
        /// <summary>Full Path: Aequus/Particles/Dusts/VoidDust</summary>
        public static readonly TextureAsset VoidDust = new("Aequus/Particles/Dusts/VoidDust");
        /// <summary>Full Path: Aequus/NPCs/Monsters/Event/GaleStreams/Vraine</summary>
        public static readonly TextureAsset Vraine = new("Aequus/NPCs/Monsters/Event/GaleStreams/Vraine");
        /// <summary>Full Path: Aequus/NPCs/Monsters/Event/GaleStreams/Vraine_Cold</summary>
        public static readonly TextureAsset Vraine_Cold = new("Aequus/NPCs/Monsters/Event/GaleStreams/Vraine_Cold");
        /// <summary>Full Path: Aequus/NPCs/Monsters/Event/GaleStreams/Vraine_Hot</summary>
        public static readonly TextureAsset Vraine_Hot = new("Aequus/NPCs/Monsters/Event/GaleStreams/Vraine_Hot");
        /// <summary>Full Path: Aequus/Tiles/Banners/Items/VraineBanner</summary>
        public static readonly TextureAsset VraineBanner = new("Aequus/Tiles/Banners/Items/VraineBanner");
        /// <summary>Full Path: Aequus/Items/Weapons/Melee/Thrown/Vrang</summary>
        public static readonly TextureAsset Vrang = new("Aequus/Items/Weapons/Melee/Thrown/Vrang");
        /// <summary>Full Path: Aequus/Projectiles/Melee/VrangProj</summary>
        public static readonly TextureAsset VrangProj = new("Aequus/Projectiles/Melee/VrangProj");
        /// <summary>Full Path: Aequus/Projectiles/Melee/VrangProj_Cold</summary>
        public static readonly TextureAsset VrangProj_Cold = new("Aequus/Projectiles/Melee/VrangProj_Cold");
        /// <summary>Full Path: Aequus/Projectiles/Melee/VrangProj_Hot</summary>
        public static readonly TextureAsset VrangProj_Hot = new("Aequus/Projectiles/Melee/VrangProj_Hot");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/Misc/Wabbajack</summary>
        public static readonly TextureAsset Wabbajack = new("Aequus/Items/Weapons/Magic/Misc/Wabbajack");
        /// <summary>Full Path: Aequus/Projectiles/Magic/WabbajackProj</summary>
        public static readonly TextureAsset WabbajackProj = new("Aequus/Projectiles/Magic/WabbajackProj");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Paintings/WallPaintings</summary>
        public static readonly TextureAsset WallPaintings = new("Aequus/Tiles/Furniture/Paintings/WallPaintings");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Paintings/WallPaintings2x2</summary>
        public static readonly TextureAsset WallPaintings2x2 = new("Aequus/Tiles/Furniture/Paintings/WallPaintings2x2");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Paintings/WallPaintings3x2</summary>
        public static readonly TextureAsset WallPaintings3x2 = new("Aequus/Tiles/Furniture/Paintings/WallPaintings3x2");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Paintings/WallPaintings6x4</summary>
        public static readonly TextureAsset WallPaintings6x4 = new("Aequus/Tiles/Furniture/Paintings/WallPaintings6x4");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Summon/WarHorn</summary>
        public static readonly TextureAsset WarHorn = new("Aequus/Items/Accessories/Combat/Summon/WarHorn");
        /// <summary>Full Path: Aequus/Items/Accessories/Combat/Summon/WarHornCooldown</summary>
        public static readonly TextureAsset WarHornCooldown = new("Aequus/Items/Accessories/Combat/Summon/WarHornCooldown");
        /// <summary>Full Path: Aequus/Items/Tools/WhiteFlag</summary>
        public static readonly TextureAsset WhiteFlag = new("Aequus/Items/Tools/WhiteFlag");
        /// <summary>Full Path: Aequus/Unused/Items/WhitePhial</summary>
        public static readonly TextureAsset WhitePhial = new("Aequus/Unused/Items/WhitePhial");
        /// <summary>Full Path: Aequus/NPCs/Monsters/Event/GaleStreams/WhiteSlime</summary>
        public static readonly TextureAsset WhiteSlime = new("Aequus/NPCs/Monsters/Event/GaleStreams/WhiteSlime");
        /// <summary>Full Path: Aequus/Tiles/Banners/Items/WhiteSlimeBanner</summary>
        public static readonly TextureAsset WhiteSlimeBanner = new("Aequus/Tiles/Banners/Items/WhiteSlimeBanner");
        /// <summary>Full Path: Aequus/Assets/Wind</summary>
        public static readonly TextureAsset Wind = new("Aequus/Assets/Wind");
        /// <summary>Full Path: Aequus/Assets/Wind2</summary>
        public static readonly TextureAsset Wind2 = new("Aequus/Assets/Wind2");
        /// <summary>Full Path: Aequus/Items/Weapons/Magic/Misc/WindFan</summary>
        public static readonly TextureAsset WindFan = new("Aequus/Items/Weapons/Magic/Misc/WindFan");
        /// <summary>Full Path: Aequus/Items/Accessories/CrownOfBlood/Projectiles/WormScarfLaser</summary>
        public static readonly TextureAsset WormScarfLaser = new("Aequus/Items/Accessories/CrownOfBlood/Projectiles/WormScarfLaser");
        /// <summary>Full Path: Aequus/Items/Misc/Bait/XenonBait</summary>
        public static readonly TextureAsset XenonBait = new("Aequus/Items/Misc/Bait/XenonBait");
        /// <summary>Full Path: Aequus/Items/Misc/LegendaryFish/XenonFish</summary>
        public static readonly TextureAsset XenonFish = new("Aequus/Items/Misc/LegendaryFish/XenonFish");
        /// <summary>Full Path: Aequus/Content/Elites/Misc/XenonSpore</summary>
        public static readonly TextureAsset XenonSpore = new("Aequus/Content/Elites/Misc/XenonSpore");
        /// <summary>Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/XenonSummonPlant</summary>
        public static readonly TextureAsset XenonSummonPlant = new("Aequus/Content/Biomes/MossBiomes/Tiles/XenonSummonPlant");
        /// <summary>Full Path: Aequus/Content/CursorDyes/Items/XmasCursor</summary>
        public static readonly TextureAsset XmasCursor = new("Aequus/Content/CursorDyes/Items/XmasCursor");
        /// <summary>Full Path: Aequus/Unused/Items/XmasEnergy</summary>
        public static readonly TextureAsset XmasEnergy = new("Aequus/Unused/Items/XmasEnergy");
        /// <summary>Full Path: Aequus/Assets/Effects/Textures/XmasEnergyGradient</summary>
        public static readonly TextureAsset XmasEnergyGradient = new("Aequus/Assets/Effects/Textures/XmasEnergyGradient");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Paintings/Items/YangPainting</summary>
        public static readonly TextureAsset YangPainting = new("Aequus/Tiles/Furniture/Paintings/Items/YangPainting");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Paintings/Items/YinPainting</summary>
        public static readonly TextureAsset YinPainting = new("Aequus/Tiles/Furniture/Paintings/Items/YinPainting");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Paintings/Items/YinYangPainting</summary>
        public static readonly TextureAsset YinYangPainting = new("Aequus/Tiles/Furniture/Paintings/Items/YinYangPainting");
        /// <summary>Full Path: Aequus/Tiles/Furniture/Paintings/Items/YinYangXmasPainting</summary>
        public static readonly TextureAsset YinYangXmasPainting = new("Aequus/Tiles/Furniture/Paintings/Items/YinYangXmasPainting");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Scepters/ZombieScepter</summary>
        public static readonly TextureAsset ZombieScepter = new("Aequus/Items/Weapons/Necromancy/Scepters/ZombieScepter");
        /// <summary>Full Path: Aequus/Items/Weapons/Necromancy/Scepters/ZombieScepter_Glow</summary>
        public static readonly TextureAsset ZombieScepter_Glow = new("Aequus/Items/Weapons/Necromancy/Scepters/ZombieScepter_Glow");
        /// <summary>Full Path: Aequus/Assets/UI/ZoologistAltHead</summary>
        public static readonly TextureAsset ZoologistAltHead = new("Aequus/Assets/UI/ZoologistAltHead");
    }
}