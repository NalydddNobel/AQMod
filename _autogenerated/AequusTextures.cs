using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Aequus.Common;

#if DEBUG
#else
namespace Aequus {
    /// <summary>
    /// (Amt Textures: 1176)
    /// </summary>
    [CompilerGenerated]
    public class AequusTextures : ILoadable {
        public void Load(Mod mod) {
        }

        public void Unload() {
            foreach (var f in GetType().GetFields()) {
                ((TextureAsset)f.GetValue(this))?.Unload();
            }
        }

        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Quest/Bounties/Icons/ActuatorDoorBounty
        /// </summary>
        public static readonly TextureAsset ActuatorDoorBounty = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Quest/Bounties/Icons/ActuatorDoorBounty");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/AdamantiteChest
        /// </summary>
        public static readonly TextureAsset AdamantiteChest = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/AdamantiteChest");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/AdamantiteChestTile
        /// </summary>
        public static readonly TextureAsset AdamantiteChestTile = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/AdamantiteChestTile");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/AdamantiteChestTile_Highlight
        /// </summary>
        public static readonly TextureAsset AdamantiteChestTile_Highlight = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/AdamantiteChestTile_Highlight");
        /// <summary>
        /// Full Path: Aequus/NPCs/Vanilla/AdamantiteMimic
        /// </summary>
        public static readonly TextureAsset AdamantiteMimic = new TextureAsset("Aequus/NPCs/Vanilla/AdamantiteMimic");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Rewards/AdvancedRuler
        /// </summary>
        public static readonly TextureAsset AdvancedRuler = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Rewards/AdvancedRuler");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetAetherial/AetherialCrown
        /// </summary>
        public static readonly TextureAsset AetherialCrown = new TextureAsset("Aequus/Items/Armor/SetAetherial/AetherialCrown");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetAetherial/AetherialCrown_Head
        /// </summary>
        public static readonly TextureAsset AetherialCrown_Head = new TextureAsset("Aequus/Items/Armor/SetAetherial/AetherialCrown_Head");
        /// <summary>
        /// Full Path: Aequus/Buffs/Debuffs/AethersWrath
        /// </summary>
        public static readonly TextureAsset AethersWrath = new TextureAsset("Aequus/Buffs/Debuffs/AethersWrath");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/BigGems/AmethystDeposit
        /// </summary>
        public static readonly TextureAsset AmethystDeposit = new TextureAsset("Aequus/Tiles/Misc/BigGems/AmethystDeposit");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Gems/AmethystGore
        /// </summary>
        public static readonly TextureAsset AmethystGore = new TextureAsset("Aequus/Assets/Gores/Gems/AmethystGore");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Utility/AmmoBackpack
        /// </summary>
        public static readonly TextureAsset AmmoBackpack = new TextureAsset("Aequus/Items/Accessories/Utility/AmmoBackpack");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Utility/AmmoBackpack_Back
        /// </summary>
        public static readonly TextureAsset AmmoBackpack_Back = new TextureAsset("Aequus/Items/Accessories/Utility/AmmoBackpack_Back");
        /// <summary>
        /// Full Path: Aequus/Tiles/Blocks/GravityBlocks/Ancient/AncientAntiGravityBlock
        /// </summary>
        public static readonly TextureAsset AncientAntiGravityBlock = new TextureAsset("Aequus/Tiles/Blocks/GravityBlocks/Ancient/AncientAntiGravityBlock");
        /// <summary>
        /// Full Path: Aequus/Tiles/Blocks/GravityBlocks/Ancient/AncientAntiGravityBlockTile
        /// </summary>
        public static readonly TextureAsset AncientAntiGravityBlockTile = new TextureAsset("Aequus/Tiles/Blocks/GravityBlocks/Ancient/AncientAntiGravityBlockTile");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Dyes/Ancient/AncientBreakdownDye
        /// </summary>
        public static readonly TextureAsset AncientBreakdownDye = new TextureAsset("Aequus/Items/Misc/Dyes/Ancient/AncientBreakdownDye");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Dyes/Ancient/AncientFrostbiteDye
        /// </summary>
        public static readonly TextureAsset AncientFrostbiteDye = new TextureAsset("Aequus/Items/Misc/Dyes/Ancient/AncientFrostbiteDye");
        /// <summary>
        /// Full Path: Aequus/Tiles/Blocks/GravityBlocks/Ancient/AncientGravityBlock
        /// </summary>
        public static readonly TextureAsset AncientGravityBlock = new TextureAsset("Aequus/Tiles/Blocks/GravityBlocks/Ancient/AncientGravityBlock");
        /// <summary>
        /// Full Path: Aequus/Tiles/Blocks/GravityBlocks/Ancient/AncientGravityBlockTile
        /// </summary>
        public static readonly TextureAsset AncientGravityBlockTile = new TextureAsset("Aequus/Tiles/Blocks/GravityBlocks/Ancient/AncientGravityBlockTile");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Dyes/Ancient/AncientHellBeamDye
        /// </summary>
        public static readonly TextureAsset AncientHellBeamDye = new TextureAsset("Aequus/Items/Misc/Dyes/Ancient/AncientHellBeamDye");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Dyes/AncientHueshiftDye
        /// </summary>
        public static readonly TextureAsset AncientHueshiftDye = new TextureAsset("Aequus/Items/Misc/Dyes/AncientHueshiftDye");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Dyes/Ancient/AncientScorchingDye
        /// </summary>
        public static readonly TextureAsset AncientScorchingDye = new TextureAsset("Aequus/Items/Misc/Dyes/Ancient/AncientScorchingDye");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Dyes/Ancient/AncientTidalDye
        /// </summary>
        public static readonly TextureAsset AncientTidalDye = new TextureAsset("Aequus/Items/Misc/Dyes/Ancient/AncientTidalDye");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Utility/AnglerBroadcaster
        /// </summary>
        public static readonly TextureAsset AnglerBroadcaster = new TextureAsset("Aequus/Items/Accessories/Utility/AnglerBroadcaster");
        /// <summary>
        /// Full Path: Aequus/UI/AnglerBroadcasterIcon
        /// </summary>
        public static readonly TextureAsset AnglerBroadcasterIcon = new TextureAsset("Aequus/UI/AnglerBroadcasterIcon");
        /// <summary>
        /// Full Path: Aequus/Tiles/Blocks/GravityBlocks/Auras/AntiGravityAura_0
        /// </summary>
        public static readonly TextureAsset AntiGravityAura_0 = new TextureAsset("Aequus/Tiles/Blocks/GravityBlocks/Auras/AntiGravityAura_0");
        /// <summary>
        /// Full Path: Aequus/Tiles/Blocks/GravityBlocks/Auras/AntiGravityAura_1
        /// </summary>
        public static readonly TextureAsset AntiGravityAura_1 = new TextureAsset("Aequus/Tiles/Blocks/GravityBlocks/Auras/AntiGravityAura_1");
        /// <summary>
        /// Full Path: Aequus/Tiles/Blocks/GravityBlocks/AntiGravityBlock
        /// </summary>
        public static readonly TextureAsset AntiGravityBlock = new TextureAsset("Aequus/Tiles/Blocks/GravityBlocks/AntiGravityBlock");
        /// <summary>
        /// Full Path: Aequus/Tiles/Blocks/GravityBlocks/AntiGravityBlockTile
        /// </summary>
        public static readonly TextureAsset AntiGravityBlockTile = new TextureAsset("Aequus/Tiles/Blocks/GravityBlocks/AntiGravityBlockTile");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Gravity/AntiGravityCandelabra
        /// </summary>
        public static readonly TextureAsset AntiGravityCandelabra = new TextureAsset("Aequus/Tiles/Furniture/Gravity/AntiGravityCandelabra");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Gravity/AntiGravityCandle
        /// </summary>
        public static readonly TextureAsset AntiGravityCandle = new TextureAsset("Aequus/Tiles/Furniture/Gravity/AntiGravityCandle");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Gravity/AntiGravityChair
        /// </summary>
        public static readonly TextureAsset AntiGravityChair = new TextureAsset("Aequus/Tiles/Furniture/Gravity/AntiGravityChair");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Gravity/AntiGravityChestTile
        /// </summary>
        public static readonly TextureAsset AntiGravityChestTile = new TextureAsset("Aequus/Tiles/Furniture/Gravity/AntiGravityChestTile");
        /// <summary>
        /// Full Path: Aequus/Tiles/Blocks/GravityBlocks/Auras/AntiGravityDust
        /// </summary>
        public static readonly TextureAsset AntiGravityDust = new TextureAsset("Aequus/Tiles/Blocks/GravityBlocks/Auras/AntiGravityDust");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Gravity/AntiGravityLamp
        /// </summary>
        public static readonly TextureAsset AntiGravityLamp = new TextureAsset("Aequus/Tiles/Furniture/Gravity/AntiGravityLamp");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Gravity/AntiGravityPiano
        /// </summary>
        public static readonly TextureAsset AntiGravityPiano = new TextureAsset("Aequus/Tiles/Furniture/Gravity/AntiGravityPiano");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Gravity/AntiGravitySofa
        /// </summary>
        public static readonly TextureAsset AntiGravitySofa = new TextureAsset("Aequus/Tiles/Furniture/Gravity/AntiGravitySofa");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Gravity/AntiGravityTable
        /// </summary>
        public static readonly TextureAsset AntiGravityTable = new TextureAsset("Aequus/Tiles/Furniture/Gravity/AntiGravityTable");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/Energies/AquaticEnergy
        /// </summary>
        public static readonly TextureAsset AquaticEnergy = new TextureAsset("Aequus/Items/Materials/Energies/AquaticEnergy");
        /// <summary>
        /// Full Path: Aequus/Assets/Effects/Textures/AquaticEnergyGradient
        /// </summary>
        public static readonly TextureAsset AquaticEnergyGradient = new TextureAsset("Aequus/Assets/Effects/Textures/AquaticEnergyGradient");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/LegendaryFish/ArgonFish
        /// </summary>
        public static readonly TextureAsset ArgonFish = new TextureAsset("Aequus/Content/Fishing/LegendaryFish/ArgonFish");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Utility/ArmFloaties
        /// </summary>
        public static readonly TextureAsset ArmFloaties = new TextureAsset("Aequus/Items/Accessories/Utility/ArmFloaties");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/AshTombstones/AshCrossGraveMarker
        /// </summary>
        public static readonly TextureAsset AshCrossGraveMarker = new TextureAsset("Aequus/Tiles/Misc/AshTombstones/AshCrossGraveMarker");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/AshTombstones/AshGraveMarker
        /// </summary>
        public static readonly TextureAsset AshGraveMarker = new TextureAsset("Aequus/Tiles/Misc/AshTombstones/AshGraveMarker");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/AshTombstones/AshGravestone
        /// </summary>
        public static readonly TextureAsset AshGravestone = new TextureAsset("Aequus/Tiles/Misc/AshTombstones/AshGravestone");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/AshTombstones/AshHeadstone
        /// </summary>
        public static readonly TextureAsset AshHeadstone = new TextureAsset("Aequus/Tiles/Misc/AshTombstones/AshHeadstone");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/AshTombstones/AshObelisk
        /// </summary>
        public static readonly TextureAsset AshObelisk = new TextureAsset("Aequus/Tiles/Misc/AshTombstones/AshObelisk");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/AshTombstones/AshTombstone
        /// </summary>
        public static readonly TextureAsset AshTombstone = new TextureAsset("Aequus/Tiles/Misc/AshTombstones/AshTombstone");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/AshTombstones/AshTombstonesTile
        /// </summary>
        public static readonly TextureAsset AshTombstonesTile = new TextureAsset("Aequus/Tiles/Misc/AshTombstones/AshTombstonesTile");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/AshTombstones/AshTombstonesTile_Glow
        /// </summary>
        public static readonly TextureAsset AshTombstonesTile_Glow = new TextureAsset("Aequus/Tiles/Misc/AshTombstones/AshTombstonesTile_Glow");
        /// <summary>
        /// Full Path: Aequus/Items/Consumables/Foods/AstralCookie
        /// </summary>
        public static readonly TextureAsset AstralCookie = new TextureAsset("Aequus/Items/Consumables/Foods/AstralCookie");
        /// <summary>
        /// Full Path: Aequus/Buffs/AstralCookieBuff
        /// </summary>
        public static readonly TextureAsset AstralCookieBuff = new TextureAsset("Aequus/Buffs/AstralCookieBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/Energies/AtmosphericEnergy
        /// </summary>
        public static readonly TextureAsset AtmosphericEnergy = new TextureAsset("Aequus/Items/Materials/Energies/AtmosphericEnergy");
        /// <summary>
        /// Full Path: Aequus/Assets/Effects/Textures/AtmosphericEnergyGradient
        /// </summary>
        public static readonly TextureAsset AtmosphericEnergyGradient = new TextureAsset("Aequus/Assets/Effects/Textures/AtmosphericEnergyGradient");
        /// <summary>
        /// Full Path: Aequus/Items/Consumables/Foods/Baguette
        /// </summary>
        public static readonly TextureAsset Baguette = new TextureAsset("Aequus/Items/Consumables/Foods/Baguette");
        /// <summary>
        /// Full Path: Aequus/Buffs/BaguetteBuff
        /// </summary>
        public static readonly TextureAsset BaguetteBuff = new TextureAsset("Aequus/Buffs/BaguetteBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Mounts/BalloonKit
        /// </summary>
        public static readonly TextureAsset BalloonKit = new TextureAsset("Aequus/Items/Misc/Mounts/BalloonKit");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Ranged/Thrown/Baozhu
        /// </summary>
        public static readonly TextureAsset Baozhu = new TextureAsset("Aequus/Items/Weapons/Ranged/Thrown/Baozhu");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Ranged/Thrown/Baozhu_Glow
        /// </summary>
        public static readonly TextureAsset Baozhu_Glow = new TextureAsset("Aequus/Items/Weapons/Ranged/Thrown/Baozhu_Glow");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Ranged/BaozhuProj
        /// </summary>
        public static readonly TextureAsset BaozhuProj = new TextureAsset("Aequus/Projectiles/Ranged/BaozhuProj");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Ranged/Misc/BarbedHarpoon
        /// </summary>
        public static readonly TextureAsset BarbedHarpoon = new TextureAsset("Aequus/Items/Weapons/Ranged/Misc/BarbedHarpoon");
        /// <summary>
        /// Full Path: Aequus/Items/Tools/BattleAxe
        /// </summary>
        public static readonly TextureAsset BattleAxe = new TextureAsset("Aequus/Items/Tools/BattleAxe");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Melee/Swords/BattleAxeProj
        /// </summary>
        public static readonly TextureAsset BattleAxeProj = new TextureAsset("Aequus/Projectiles/Melee/Swords/BattleAxeProj");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/Bobbers/BeeBobber
        /// </summary>
        public static readonly TextureAsset BeeBobber = new TextureAsset("Aequus/Projectiles/Misc/Bobbers/BeeBobber");
        /// <summary>
        /// Full Path: Aequus/Items/Tools/Bellows
        /// </summary>
        public static readonly TextureAsset Bellows = new TextureAsset("Aequus/Items/Tools/Bellows");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/BellowsProj
        /// </summary>
        public static readonly TextureAsset BellowsProj = new TextureAsset("Aequus/Projectiles/Misc/BellowsProj");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/BestiaryNotebook
        /// </summary>
        public static readonly TextureAsset BestiaryNotebook = new TextureAsset("Aequus/Assets/UI/BestiaryNotebook");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/BigGems/BigGemsTile
        /// </summary>
        public static readonly TextureAsset BigGemsTile = new TextureAsset("Aequus/Tiles/Misc/BigGems/BigGemsTile");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Quest/Bounties/Icons/BiomePaletteBounty
        /// </summary>
        public static readonly TextureAsset BiomePaletteBounty = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Quest/Bounties/Icons/BiomePaletteBounty");
        /// <summary>
        /// Full Path: Aequus/Buffs/Debuffs/BitCrushedDebuff
        /// </summary>
        public static readonly TextureAsset BitCrushedDebuff = new TextureAsset("Aequus/Buffs/Debuffs/BitCrushedDebuff");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Debuff/BlackPhial
        /// </summary>
        public static readonly TextureAsset BlackPhial = new TextureAsset("Aequus/Items/Accessories/Debuff/BlackPhial");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Debuff/BlackPhial_Waist
        /// </summary>
        public static readonly TextureAsset BlackPhial_Waist = new TextureAsset("Aequus/Items/Accessories/Debuff/BlackPhial_Waist");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Debuff/BlackPlague
        /// </summary>
        public static readonly TextureAsset BlackPlague = new TextureAsset("Aequus/Items/Accessories/Debuff/BlackPlague");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Debuff/BlackPlague_HandsOn
        /// </summary>
        public static readonly TextureAsset BlackPlague_HandsOn = new TextureAsset("Aequus/Items/Accessories/Debuff/BlackPlague_HandsOn");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/LegendaryFish/Blobfish
        /// </summary>
        public static readonly TextureAsset Blobfish = new TextureAsset("Aequus/Content/Fishing/LegendaryFish/Blobfish");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Ranged/Thrown/BlockGlove
        /// </summary>
        public static readonly TextureAsset BlockGlove = new TextureAsset("Aequus/Items/Weapons/Ranged/Thrown/BlockGlove");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Debuff/BloodCurcleav
        /// </summary>
        public static readonly TextureAsset BloodCurcleav = new TextureAsset("Aequus/Items/Accessories/Debuff/BloodCurcleav");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Necro/BloodiedBucket
        /// </summary>
        public static readonly TextureAsset BloodiedBucket = new TextureAsset("Aequus/Items/Accessories/Offense/Necro/BloodiedBucket");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Night/BloodMimic
        /// </summary>
        public static readonly TextureAsset BloodMimic = new TextureAsset("Aequus/NPCs/Monsters/Night/BloodMimic");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/BloodMimic_0
        /// </summary>
        public static readonly TextureAsset BloodMimic_0 = new TextureAsset("Aequus/Assets/Gores/BloodMimic_0");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/BloodMimic_1
        /// </summary>
        public static readonly TextureAsset BloodMimic_1 = new TextureAsset("Aequus/Assets/Gores/BloodMimic_1");
        /// <summary>
        /// Full Path: Aequus/Tiles/Banners/Items/BloodMimicBanner
        /// </summary>
        public static readonly TextureAsset BloodMimicBanner = new TextureAsset("Aequus/Tiles/Banners/Items/BloodMimicBanner");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Candles/BloodMoonCandle
        /// </summary>
        public static readonly TextureAsset BloodMoonCandle = new TextureAsset("Aequus/Items/Weapons/Necromancy/Candles/BloodMoonCandle");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Candles/BloodMoonCandle_Flame
        /// </summary>
        public static readonly TextureAsset BloodMoonCandle_Flame = new TextureAsset("Aequus/Items/Weapons/Necromancy/Candles/BloodMoonCandle_Flame");
        /// <summary>
        /// Full Path: Aequus/Buffs/BloodthirstBuff
        /// </summary>
        public static readonly TextureAsset BloodthirstBuff = new TextureAsset("Aequus/Buffs/BloodthirstBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Potions/BloodthirstPotion
        /// </summary>
        public static readonly TextureAsset BloodthirstPotion = new TextureAsset("Aequus/Items/Potions/BloodthirstPotion");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/BloodyTearstone
        /// </summary>
        public static readonly TextureAsset BloodyTearstone = new TextureAsset("Aequus/Items/Materials/BloodyTearstone");
        /// <summary>
        /// Full Path: Aequus/Assets/Bloom_20x20
        /// </summary>
        public static readonly TextureAsset Bloom_20x20 = new TextureAsset("Aequus/Assets/Bloom_20x20");
        /// <summary>
        /// Full Path: Aequus/Assets/Bloom0
        /// </summary>
        public static readonly TextureAsset Bloom0 = new TextureAsset("Aequus/Assets/Bloom0");
        /// <summary>
        /// Full Path: Aequus/Assets/Bloom1
        /// </summary>
        public static readonly TextureAsset Bloom1 = new TextureAsset("Aequus/Assets/Bloom1");
        /// <summary>
        /// Full Path: Aequus/Assets/Bloom2
        /// </summary>
        public static readonly TextureAsset Bloom2 = new TextureAsset("Aequus/Assets/Bloom2");
        /// <summary>
        /// Full Path: Aequus/Assets/Bloom3
        /// </summary>
        public static readonly TextureAsset Bloom3 = new TextureAsset("Aequus/Assets/Bloom3");
        /// <summary>
        /// Full Path: Aequus/Assets/Bloom4
        /// </summary>
        public static readonly TextureAsset Bloom4 = new TextureAsset("Aequus/Assets/Bloom4");
        /// <summary>
        /// Full Path: Aequus/Buffs/Debuffs/BlueFire
        /// </summary>
        public static readonly TextureAsset BlueFire = new TextureAsset("Aequus/Buffs/Debuffs/BlueFire");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Magic/BombarderRod
        /// </summary>
        public static readonly TextureAsset BombarderRod = new TextureAsset("Aequus/Items/Weapons/Magic/BombarderRod");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Magic/BombarderRod_Glow
        /// </summary>
        public static readonly TextureAsset BombarderRod_Glow = new TextureAsset("Aequus/Items/Weapons/Magic/BombarderRod_Glow");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Debuff/BoneHawkRing
        /// </summary>
        public static readonly TextureAsset BoneHawkRing = new TextureAsset("Aequus/Items/Accessories/Debuff/BoneHawkRing");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Debuff/BoneHawkRing_HandsOn
        /// </summary>
        public static readonly TextureAsset BoneHawkRing_HandsOn = new TextureAsset("Aequus/Items/Accessories/Debuff/BoneHawkRing_HandsOn");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/CrownOfBlood/Projectiles/Bonesaw
        /// </summary>
        public static readonly TextureAsset Bonesaw = new TextureAsset("Aequus/Items/Accessories/CrownOfBlood/Projectiles/Bonesaw");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/CrownOfBlood/Projectiles/Bonesaw_Glow
        /// </summary>
        public static readonly TextureAsset Bonesaw_Glow = new TextureAsset("Aequus/Items/Accessories/CrownOfBlood/Projectiles/Bonesaw_Glow");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/CrownOfBlood/Projectiles/Bonesaw_Trail
        /// </summary>
        public static readonly TextureAsset Bonesaw_Trail = new TextureAsset("Aequus/Items/Accessories/CrownOfBlood/Projectiles/Bonesaw_Trail");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Paintings/Items/BongBongPainting
        /// </summary>
        public static readonly TextureAsset BongBongPainting = new TextureAsset("Aequus/Tiles/Furniture/Paintings/Items/BongBongPainting");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/BossRelics
        /// </summary>
        public static readonly TextureAsset BossRelics = new TextureAsset("Aequus/Content/Boss/BossRelics");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/BossRelicsOrbs
        /// </summary>
        public static readonly TextureAsset BossRelicsOrbs = new TextureAsset("Aequus/Content/Boss/BossRelicsOrbs");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Ranged/Misc/BoundBow
        /// </summary>
        public static readonly TextureAsset BoundBow = new TextureAsset("Aequus/Items/Weapons/Ranged/Misc/BoundBow");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Ranged/Misc/BoundBow_Glow
        /// </summary>
        public static readonly TextureAsset BoundBow_Glow = new TextureAsset("Aequus/Items/Weapons/Ranged/Misc/BoundBow_Glow");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Ranged/BoundBowProj
        /// </summary>
        public static readonly TextureAsset BoundBowProj = new TextureAsset("Aequus/Projectiles/Ranged/BoundBowProj");
        /// <summary>
        /// Full Path: Aequus/Content/ItemPrefixes/Potions/BoundedGlint
        /// </summary>
        public static readonly TextureAsset BoundedGlint = new TextureAsset("Aequus/Content/ItemPrefixes/Potions/BoundedGlint");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Quest/Bounties/BountyUIArrow
        /// </summary>
        public static readonly TextureAsset BountyUIArrow = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Quest/Bounties/BountyUIArrow");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Quest/Bounties/BountyUIArrow_2
        /// </summary>
        public static readonly TextureAsset BountyUIArrow_2 = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Quest/Bounties/BountyUIArrow_2");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/BrainCauliflowerBlast
        /// </summary>
        public static readonly TextureAsset BrainCauliflowerBlast = new TextureAsset("Aequus/Projectiles/Summon/BrainCauliflowerBlast");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/BrainCauliflowerBlast_Aura
        /// </summary>
        public static readonly TextureAsset BrainCauliflowerBlast_Aura = new TextureAsset("Aequus/Projectiles/Summon/BrainCauliflowerBlast_Aura");
        /// <summary>
        /// Full Path: Aequus/Buffs/Minion/BrainCauliflowerBuff
        /// </summary>
        public static readonly TextureAsset BrainCauliflowerBuff = new TextureAsset("Aequus/Buffs/Minion/BrainCauliflowerBuff");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/BrainCauliflowerMinion
        /// </summary>
        public static readonly TextureAsset BrainCauliflowerMinion = new TextureAsset("Aequus/Projectiles/Summon/BrainCauliflowerMinion");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Summon/Minion/BrainwaveStaff
        /// </summary>
        public static readonly TextureAsset BrainwaveStaff = new TextureAsset("Aequus/Items/Weapons/Summon/Minion/BrainwaveStaff");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/BreadOfCthulhu
        /// </summary>
        public static readonly TextureAsset BreadOfCthulhu = new TextureAsset("Aequus/NPCs/Monsters/BreadOfCthulhu");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/BreadOfCthulhu_0
        /// </summary>
        public static readonly TextureAsset BreadOfCthulhu_0 = new TextureAsset("Aequus/Assets/Gores/BreadOfCthulhu_0");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/BreadOfCthulhu_1
        /// </summary>
        public static readonly TextureAsset BreadOfCthulhu_1 = new TextureAsset("Aequus/Assets/Gores/BreadOfCthulhu_1");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/BreadOfCthulhu_2
        /// </summary>
        public static readonly TextureAsset BreadOfCthulhu_2 = new TextureAsset("Aequus/Assets/Gores/BreadOfCthulhu_2");
        /// <summary>
        /// Full Path: Aequus/Tiles/Banners/Items/BreadOfCthulhuBanner
        /// </summary>
        public static readonly TextureAsset BreadOfCthulhuBanner = new TextureAsset("Aequus/Tiles/Banners/Items/BreadOfCthulhuBanner");
        /// <summary>
        /// Full Path: Aequus/Items/Vanity/BreadOfCthulhuMask
        /// </summary>
        public static readonly TextureAsset BreadOfCthulhuMask = new TextureAsset("Aequus/Items/Vanity/BreadOfCthulhuMask");
        /// <summary>
        /// Full Path: Aequus/Items/Vanity/BreadOfCthulhuMask_Head
        /// </summary>
        public static readonly TextureAsset BreadOfCthulhuMask_Head = new TextureAsset("Aequus/Items/Vanity/BreadOfCthulhuMask_Head");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Paintings/Items/BreadRoachPainting
        /// </summary>
        public static readonly TextureAsset BreadRoachPainting = new TextureAsset("Aequus/Tiles/Furniture/Paintings/Items/BreadRoachPainting");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/QuestFish/BrickFish
        /// </summary>
        public static readonly TextureAsset BrickFish = new TextureAsset("Aequus/Content/Fishing/QuestFish/BrickFish");
        /// <summary>
        /// Full Path: Aequus/Buffs/Buildings/BridgeBountyBuff
        /// </summary>
        public static readonly TextureAsset BridgeBountyBuff = new TextureAsset("Aequus/Buffs/Buildings/BridgeBountyBuff");
        /// <summary>
        /// Full Path: Aequus/Buffs/Buff
        /// </summary>
        public static readonly TextureAsset Buff_Buffs = new TextureAsset("Aequus/Buffs/Buff");
        /// <summary>
        /// Full Path: Aequus/Buffs/Buildings/Buff
        /// </summary>
        public static readonly TextureAsset Buff_Buildings = new TextureAsset("Aequus/Buffs/Buildings/Buff");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/Empowered/Buff
        /// </summary>
        public static readonly TextureAsset Buff_Empowered = new TextureAsset("Aequus/Buffs/Misc/Empowered/Buff");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/BuilderIcons
        /// </summary>
        public static readonly TextureAsset BuilderIcons = new TextureAsset("Aequus/Assets/UI/BuilderIcons");
        /// <summary>
        /// Full Path: Aequus/Assets/Bullet
        /// </summary>
        public static readonly TextureAsset Bullet = new TextureAsset("Aequus/Assets/Bullet");
        /// <summary>
        /// Full Path: Aequus/Content/Town/BusinessCard
        /// </summary>
        public static readonly TextureAsset BusinessCard = new TextureAsset("Aequus/Content/Town/BusinessCard");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Poles/Buzzer
        /// </summary>
        public static readonly TextureAsset Buzzer = new TextureAsset("Aequus/Content/Fishing/Poles/Buzzer");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Carpenter
        /// </summary>
        public static readonly TextureAsset Carpenter = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Carpenter");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Carpenter_Head
        /// </summary>
        public static readonly TextureAsset Carpenter_Head = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Carpenter_Head");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Carpenter_Shimmer
        /// </summary>
        public static readonly TextureAsset Carpenter_Shimmer = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Carpenter_Shimmer");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Carpenter_Shimmer_Head
        /// </summary>
        public static readonly TextureAsset Carpenter_Shimmer_Head = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Carpenter_Shimmer_Head");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Misc/CarpenterBountyItem
        /// </summary>
        public static readonly TextureAsset CarpenterBountyItem = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Misc/CarpenterBountyItem");
        /// <summary>
        /// Full Path: Aequus/Items/Consumables/CarpenterResetSheet
        /// </summary>
        public static readonly TextureAsset CarpenterResetSheet = new TextureAsset("Aequus/Items/Consumables/CarpenterResetSheet");
        /// <summary>
        /// Full Path: Aequus/Particles/Dusts/CarpenterSurpriseDust
        /// </summary>
        public static readonly TextureAsset CarpenterSurpriseDust = new TextureAsset("Aequus/Particles/Dusts/CarpenterSurpriseDust");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Necro/CartilageRing
        /// </summary>
        public static readonly TextureAsset CartilageRing = new TextureAsset("Aequus/Items/Accessories/Offense/Necro/CartilageRing");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Paintings/Items/CatalystPainting
        /// </summary>
        public static readonly TextureAsset CatalystPainting = new TextureAsset("Aequus/Tiles/Furniture/Paintings/Items/CatalystPainting");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/Heavy/Cauterizer
        /// </summary>
        public static readonly TextureAsset Cauterizer = new TextureAsset("Aequus/Items/Weapons/Melee/Heavy/Cauterizer");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Melee/Swords/CauterizerProj
        /// </summary>
        public static readonly TextureAsset CauterizerProj = new TextureAsset("Aequus/Projectiles/Melee/Swords/CauterizerProj");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Melee/CauterizerSlash
        /// </summary>
        public static readonly TextureAsset CauterizerSlash = new TextureAsset("Aequus/Projectiles/Melee/CauterizerSlash");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/CelesitalEightBall
        /// </summary>
        public static readonly TextureAsset CelesitalEightBall = new TextureAsset("Aequus/Items/Misc/CelesitalEightBall");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/CelesitalEightBall_Glow
        /// </summary>
        public static readonly TextureAsset CelesitalEightBall_Glow = new TextureAsset("Aequus/Items/Misc/CelesitalEightBall_Glow");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Passive/CelesteTorus
        /// </summary>
        public static readonly TextureAsset CelesteTorus = new TextureAsset("Aequus/Items/Accessories/Passive/CelesteTorus");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/Friendly/CelesteTorusProj
        /// </summary>
        public static readonly TextureAsset CelesteTorusProj = new TextureAsset("Aequus/Projectiles/Misc/Friendly/CelesteTorusProj");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Dyes/CensorDye
        /// </summary>
        public static readonly TextureAsset CensorDye = new TextureAsset("Aequus/Items/Misc/Dyes/CensorDye");
        /// <summary>
        /// Full Path: Aequus/Items/Consumables/ChlorophytePowder
        /// </summary>
        public static readonly TextureAsset ChlorophytePowder = new TextureAsset("Aequus/Items/Consumables/ChlorophytePowder");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Underworld/Cindera
        /// </summary>
        public static readonly TextureAsset Cindera = new TextureAsset("Aequus/NPCs/Monsters/Underworld/Cindera");
        /// <summary>
        /// Full Path: Aequus/Tiles/Banners/Items/CinderaBanner
        /// </summary>
        public static readonly TextureAsset CinderaBanner = new TextureAsset("Aequus/Tiles/Banners/Items/CinderaBanner");
        /// <summary>
        /// Full Path: Aequus/Items/Consumables/Foods/CinnamonRoll
        /// </summary>
        public static readonly TextureAsset CinnamonRoll = new TextureAsset("Aequus/Items/Consumables/Foods/CinnamonRoll");
        /// <summary>
        /// Full Path: Aequus/Content/DronePylons/NPCs/CleanserDrone
        /// </summary>
        public static readonly TextureAsset CleanserDrone = new TextureAsset("Aequus/Content/DronePylons/NPCs/CleanserDrone");
        /// <summary>
        /// Full Path: Aequus/Content/DronePylons/NPCs/CleanserDrone_Glow
        /// </summary>
        public static readonly TextureAsset CleanserDrone_Glow = new TextureAsset("Aequus/Content/DronePylons/NPCs/CleanserDrone_Glow");
        /// <summary>
        /// Full Path: Aequus/Content/DronePylons/NPCs/CleanserDrone_Gun
        /// </summary>
        public static readonly TextureAsset CleanserDrone_Gun = new TextureAsset("Aequus/Content/DronePylons/NPCs/CleanserDrone_Gun");
        /// <summary>
        /// Full Path: Aequus/Content/DronePylons/NPCs/CleanserDrone_Gun_Glow
        /// </summary>
        public static readonly TextureAsset CleanserDrone_Gun_Glow = new TextureAsset("Aequus/Content/DronePylons/NPCs/CleanserDrone_Gun_Glow");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/CrabCrevice/CoconutCrab
        /// </summary>
        public static readonly TextureAsset CoconutCrab = new TextureAsset("Aequus/NPCs/Monsters/CrabCrevice/CoconutCrab");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/CrabCrevice/CoconutCrab_Glow
        /// </summary>
        public static readonly TextureAsset CoconutCrab_Glow = new TextureAsset("Aequus/NPCs/Monsters/CrabCrevice/CoconutCrab_Glow");
        /// <summary>
        /// Full Path: Aequus/Tiles/Banners/Items/CoconutCrabBanner
        /// </summary>
        public static readonly TextureAsset CoconutCrabBanner = new TextureAsset("Aequus/Tiles/Banners/Items/CoconutCrabBanner");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/CooldownBack
        /// </summary>
        public static readonly TextureAsset CooldownBack = new TextureAsset("Aequus/Assets/UI/CooldownBack");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Candles/CorruptionCandle
        /// </summary>
        public static readonly TextureAsset CorruptionCandle = new TextureAsset("Aequus/Items/Weapons/Necromancy/Candles/CorruptionCandle");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Candles/CorruptionCandle_Flame
        /// </summary>
        public static readonly TextureAsset CorruptionCandle_Flame = new TextureAsset("Aequus/Items/Weapons/Necromancy/Candles/CorruptionCandle_Flame");
        /// <summary>
        /// Full Path: Aequus/Buffs/Debuffs/CorruptionHellfire
        /// </summary>
        public static readonly TextureAsset CorruptionHellfire = new TextureAsset("Aequus/Buffs/Debuffs/CorruptionHellfire");
        /// <summary>
        /// Full Path: Aequus/Buffs/Minion/CorruptPlantBuff
        /// </summary>
        public static readonly TextureAsset CorruptPlantBuff = new TextureAsset("Aequus/Buffs/Minion/CorruptPlantBuff");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/CorruptPlantMinion
        /// </summary>
        public static readonly TextureAsset CorruptPlantMinion = new TextureAsset("Aequus/Projectiles/Summon/CorruptPlantMinion");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/CorruptPlantMinion_Chain
        /// </summary>
        public static readonly TextureAsset CorruptPlantMinion_Chain = new TextureAsset("Aequus/Projectiles/Summon/CorruptPlantMinion_Chain");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/CorruptPlantMinion_Large
        /// </summary>
        public static readonly TextureAsset CorruptPlantMinion_Large = new TextureAsset("Aequus/Projectiles/Summon/CorruptPlantMinion_Large");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/CorruptPlantMinion_Med
        /// </summary>
        public static readonly TextureAsset CorruptPlantMinion_Med = new TextureAsset("Aequus/Projectiles/Summon/CorruptPlantMinion_Med");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Summon/Minion/CorruptPot
        /// </summary>
        public static readonly TextureAsset CorruptPot = new TextureAsset("Aequus/Items/Weapons/Summon/Minion/CorruptPot");
        /// <summary>
        /// Full Path: Aequus/Items/Consumables/Permanent/CosmicChest
        /// </summary>
        public static readonly TextureAsset CosmicChest = new TextureAsset("Aequus/Items/Consumables/Permanent/CosmicChest");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/Energies/CosmicEnergy
        /// </summary>
        public static readonly TextureAsset CosmicEnergy = new TextureAsset("Aequus/Items/Materials/Energies/CosmicEnergy");
        /// <summary>
        /// Full Path: Aequus/Assets/Effects/Textures/CosmicEnergyGradient
        /// </summary>
        public static readonly TextureAsset CosmicEnergyGradient = new TextureAsset("Aequus/Assets/Effects/Textures/CosmicEnergyGradient");
        /// <summary>
        /// Full Path: Aequus/Content/Events/GlimmerEvent/Sky/CosmicMonolith
        /// </summary>
        public static readonly TextureAsset CosmicMonolith = new TextureAsset("Aequus/Content/Events/GlimmerEvent/Sky/CosmicMonolith");
        /// <summary>
        /// Full Path: Aequus/Content/Events/GlimmerEvent/Sky/CosmicMonolithTile
        /// </summary>
        public static readonly TextureAsset CosmicMonolithTile = new TextureAsset("Aequus/Content/Events/GlimmerEvent/Sky/CosmicMonolithTile");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/Crabax
        /// </summary>
        public static readonly TextureAsset Crabax = new TextureAsset("Aequus/Unused/Items/Crabax");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/Bobbers/CrabBobber
        /// </summary>
        public static readonly TextureAsset CrabBobber = new TextureAsset("Aequus/Projectiles/Misc/Bobbers/CrabBobber");
        /// <summary>
        /// Full Path: Aequus/Items/Placeable/Furniture/Interactable/CrabClock
        /// </summary>
        public static readonly TextureAsset CrabClock = new TextureAsset("Aequus/Items/Placeable/Furniture/Interactable/CrabClock");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/BestiaryIcons/CrabCrevice
        /// </summary>
        public static readonly TextureAsset CrabCrevice = new TextureAsset("Aequus/Assets/UI/BestiaryIcons/CrabCrevice");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Crates/CrabCreviceCrate
        /// </summary>
        public static readonly TextureAsset CrabCreviceCrate = new TextureAsset("Aequus/Content/Fishing/Crates/CrabCreviceCrate");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Crates/CrabCreviceCrateHard
        /// </summary>
        public static readonly TextureAsset CrabCreviceCrateHard = new TextureAsset("Aequus/Content/Fishing/Crates/CrabCreviceCrateHard");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/CrabCrevice/Water/CrabCreviceDroplet
        /// </summary>
        public static readonly TextureAsset CrabCreviceDroplet = new TextureAsset("Aequus/Content/Biomes/CrabCrevice/Water/CrabCreviceDroplet");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/CrabCrevice/Tiles/CrabCrevicePot
        /// </summary>
        public static readonly TextureAsset CrabCrevicePot = new TextureAsset("Aequus/Content/Biomes/CrabCrevice/Tiles/CrabCrevicePot");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Pots/CrabCrevicePot_0_0
        /// </summary>
        public static readonly TextureAsset CrabCrevicePot_0_0 = new TextureAsset("Aequus/Assets/Gores/Pots/CrabCrevicePot_0_0");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Pots/CrabCrevicePot_0_1
        /// </summary>
        public static readonly TextureAsset CrabCrevicePot_0_1 = new TextureAsset("Aequus/Assets/Gores/Pots/CrabCrevicePot_0_1");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Pots/CrabCrevicePot_0_2
        /// </summary>
        public static readonly TextureAsset CrabCrevicePot_0_2 = new TextureAsset("Aequus/Assets/Gores/Pots/CrabCrevicePot_0_2");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Pots/CrabCrevicePot_1_0
        /// </summary>
        public static readonly TextureAsset CrabCrevicePot_1_0 = new TextureAsset("Aequus/Assets/Gores/Pots/CrabCrevicePot_1_0");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Pots/CrabCrevicePot_1_1
        /// </summary>
        public static readonly TextureAsset CrabCrevicePot_1_1 = new TextureAsset("Aequus/Assets/Gores/Pots/CrabCrevicePot_1_1");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Pots/CrabCrevicePot_1_2
        /// </summary>
        public static readonly TextureAsset CrabCrevicePot_1_2 = new TextureAsset("Aequus/Assets/Gores/Pots/CrabCrevicePot_1_2");
        /// <summary>
        /// Full Path: Aequus/Particles/Dusts/CrabCreviceSplash
        /// </summary>
        public static readonly TextureAsset CrabCreviceSplash = new TextureAsset("Aequus/Particles/Dusts/CrabCreviceSplash");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/CrabCrevice/Background/CrabCreviceSurfaceBackground_0
        /// </summary>
        public static readonly TextureAsset CrabCreviceSurfaceBackground_0 = new TextureAsset("Aequus/Content/Biomes/CrabCrevice/Background/CrabCreviceSurfaceBackground_0");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/CrabCrevice/Background/CrabCreviceSurfaceBackground_1
        /// </summary>
        public static readonly TextureAsset CrabCreviceSurfaceBackground_1 = new TextureAsset("Aequus/Content/Biomes/CrabCrevice/Background/CrabCreviceSurfaceBackground_1");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/CrabCrevice/Background/CrabCreviceSurfaceBackground_2
        /// </summary>
        public static readonly TextureAsset CrabCreviceSurfaceBackground_2 = new TextureAsset("Aequus/Content/Biomes/CrabCrevice/Background/CrabCreviceSurfaceBackground_2");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/CrabCrevice/Water/CrabCreviceWater
        /// </summary>
        public static readonly TextureAsset CrabCreviceWater = new TextureAsset("Aequus/Content/Biomes/CrabCrevice/Water/CrabCreviceWater");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/CrabCrevice/Water/CrabCreviceWater_Block
        /// </summary>
        public static readonly TextureAsset CrabCreviceWater_Block = new TextureAsset("Aequus/Content/Biomes/CrabCrevice/Water/CrabCreviceWater_Block");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/CrabCrevice/Water/CrabCreviceWaterfall
        /// </summary>
        public static readonly TextureAsset CrabCreviceWaterfall = new TextureAsset("Aequus/Content/Biomes/CrabCrevice/Water/CrabCreviceWaterfall");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/LegendaryFish/CrabDaughter
        /// </summary>
        public static readonly TextureAsset CrabDaughter = new TextureAsset("Aequus/Content/Fishing/LegendaryFish/CrabDaughter");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/CrabCrevice/CrabFish
        /// </summary>
        public static readonly TextureAsset CrabFish = new TextureAsset("Aequus/NPCs/Monsters/CrabCrevice/CrabFish");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/CrabCrevice/CrabFish_Glow
        /// </summary>
        public static readonly TextureAsset CrabFish_Glow = new TextureAsset("Aequus/NPCs/Monsters/CrabCrevice/CrabFish_Glow");
        /// <summary>
        /// Full Path: Aequus/Tiles/Banners/Items/CrabFishBanner
        /// </summary>
        public static readonly TextureAsset CrabFishBanner = new TextureAsset("Aequus/Tiles/Banners/Items/CrabFishBanner");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/CrabCrevice/Tiles/CrabFloorPlants
        /// </summary>
        public static readonly TextureAsset CrabFloorPlants = new TextureAsset("Aequus/Content/Biomes/CrabCrevice/Tiles/CrabFloorPlants");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/CrabCrevice/Tiles/CrabGrassBig
        /// </summary>
        public static readonly TextureAsset CrabGrassBig = new TextureAsset("Aequus/Content/Biomes/CrabCrevice/Tiles/CrabGrassBig");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/CrabCrevice/Tiles/CrabHydrosailia
        /// </summary>
        public static readonly TextureAsset CrabHydrosailia = new TextureAsset("Aequus/Content/Biomes/CrabCrevice/Tiles/CrabHydrosailia");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Poles/CrabRod
        /// </summary>
        public static readonly TextureAsset CrabRod = new TextureAsset("Aequus/Content/Fishing/Poles/CrabRod");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/BossChecklist/Crabson
        /// </summary>
        public static readonly TextureAsset Crabson_BossChecklist = new TextureAsset("Aequus/Assets/UI/BossChecklist/Crabson");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Crabson_Chain
        /// </summary>
        public static readonly TextureAsset Crabson_Chain = new TextureAsset("Aequus/Content/Boss/Crabson/Crabson_Chain");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Crabson
        /// </summary>
        public static readonly TextureAsset Crabson_Crabson = new TextureAsset("Aequus/Content/Boss/Crabson/Crabson");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/CrabsonOld/Crabson
        /// </summary>
        public static readonly TextureAsset Crabson_CrabsonOld = new TextureAsset("Aequus/Content/Boss/CrabsonOld/Crabson");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Crabson_Eyes
        /// </summary>
        public static readonly TextureAsset Crabson_Eyes = new TextureAsset("Aequus/Content/Boss/Crabson/Crabson_Eyes");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Crabson_Head_Boss
        /// </summary>
        public static readonly TextureAsset Crabson_Head_Boss_Crabson = new TextureAsset("Aequus/Content/Boss/Crabson/Crabson_Head_Boss");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/CrabsonOld/Crabson_Head_Boss
        /// </summary>
        public static readonly TextureAsset Crabson_Head_Boss_CrabsonOld = new TextureAsset("Aequus/Content/Boss/CrabsonOld/Crabson_Head_Boss");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Crabson_Legs
        /// </summary>
        public static readonly TextureAsset Crabson_Legs = new TextureAsset("Aequus/Content/Boss/Crabson/Crabson_Legs");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Crabson_Pupil
        /// </summary>
        public static readonly TextureAsset Crabson_Pupil = new TextureAsset("Aequus/Content/Boss/Crabson/Crabson_Pupil");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Rewards/CrabsonBag
        /// </summary>
        public static readonly TextureAsset CrabsonBag = new TextureAsset("Aequus/Content/Boss/Crabson/Rewards/CrabsonBag");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Projectiles/CrabsonBubble
        /// </summary>
        public static readonly TextureAsset CrabsonBubble = new TextureAsset("Aequus/Content/Boss/Crabson/Projectiles/CrabsonBubble");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/CrabsonOld/CrabsonClaw_Chain
        /// </summary>
        public static readonly TextureAsset CrabsonClaw_Chain = new TextureAsset("Aequus/Content/Boss/CrabsonOld/CrabsonClaw_Chain");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/CrabsonClaw
        /// </summary>
        public static readonly TextureAsset CrabsonClaw_Crabson = new TextureAsset("Aequus/Content/Boss/Crabson/CrabsonClaw");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/CrabsonOld/CrabsonClaw
        /// </summary>
        public static readonly TextureAsset CrabsonClaw_CrabsonOld = new TextureAsset("Aequus/Content/Boss/CrabsonOld/CrabsonClaw");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/CrabsonClaw_Head_Boss
        /// </summary>
        public static readonly TextureAsset CrabsonClaw_Head_Boss_Crabson = new TextureAsset("Aequus/Content/Boss/Crabson/CrabsonClaw_Head_Boss");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/CrabsonOld/CrabsonClaw_Head_Boss
        /// </summary>
        public static readonly TextureAsset CrabsonClaw_Head_Boss_CrabsonOld = new TextureAsset("Aequus/Content/Boss/CrabsonOld/CrabsonClaw_Head_Boss");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Gore/CrabsonGoreClawBottom
        /// </summary>
        public static readonly TextureAsset CrabsonGoreClawBottom = new TextureAsset("Aequus/Content/Boss/Crabson/Gore/CrabsonGoreClawBottom");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Gore/CrabsonGoreClawTop
        /// </summary>
        public static readonly TextureAsset CrabsonGoreClawTop = new TextureAsset("Aequus/Content/Boss/Crabson/Gore/CrabsonGoreClawTop");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Gore/CrabsonGoreHead
        /// </summary>
        public static readonly TextureAsset CrabsonGoreHead = new TextureAsset("Aequus/Content/Boss/Crabson/Gore/CrabsonGoreHead");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Gore/CrabsonGoreHeadBottom
        /// </summary>
        public static readonly TextureAsset CrabsonGoreHeadBottom = new TextureAsset("Aequus/Content/Boss/Crabson/Gore/CrabsonGoreHeadBottom");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Gore/CrabsonGoreLeg
        /// </summary>
        public static readonly TextureAsset CrabsonGoreLeg = new TextureAsset("Aequus/Content/Boss/Crabson/Gore/CrabsonGoreLeg");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Rewards/CrabsonMask
        /// </summary>
        public static readonly TextureAsset CrabsonMask = new TextureAsset("Aequus/Content/Boss/Crabson/Rewards/CrabsonMask");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Rewards/CrabsonMask_Head
        /// </summary>
        public static readonly TextureAsset CrabsonMask_Head = new TextureAsset("Aequus/Content/Boss/Crabson/Rewards/CrabsonMask_Head");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Projectiles/CrabsonPearl
        /// </summary>
        public static readonly TextureAsset CrabsonPearl = new TextureAsset("Aequus/Content/Boss/Crabson/Projectiles/CrabsonPearl");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Projectiles/CrabsonPearl_White
        /// </summary>
        public static readonly TextureAsset CrabsonPearl_White = new TextureAsset("Aequus/Content/Boss/Crabson/Projectiles/CrabsonPearl_White");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Projectiles/CrabsonPearlShard
        /// </summary>
        public static readonly TextureAsset CrabsonPearlShard = new TextureAsset("Aequus/Content/Boss/Crabson/Projectiles/CrabsonPearlShard");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Rewards/CrabsonRelic
        /// </summary>
        public static readonly TextureAsset CrabsonRelic = new TextureAsset("Aequus/Content/Boss/Crabson/Rewards/CrabsonRelic");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Rewards/CrabsonTreasureChest
        /// </summary>
        public static readonly TextureAsset CrabsonTreasureChest = new TextureAsset("Aequus/Content/Boss/Crabson/Rewards/CrabsonTreasureChest");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Rewards/CrabsonTrophy
        /// </summary>
        public static readonly TextureAsset CrabsonTrophy = new TextureAsset("Aequus/Content/Boss/Crabson/Rewards/CrabsonTrophy");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Bait/CrateBait
        /// </summary>
        public static readonly TextureAsset CrateBait = new TextureAsset("Aequus/Content/Fishing/Bait/CrateBait");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Candles/CrimsonCandle
        /// </summary>
        public static readonly TextureAsset CrimsonCandle = new TextureAsset("Aequus/Items/Weapons/Necromancy/Candles/CrimsonCandle");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Candles/CrimsonCandle_Flame
        /// </summary>
        public static readonly TextureAsset CrimsonCandle_Flame = new TextureAsset("Aequus/Items/Weapons/Necromancy/Candles/CrimsonCandle_Flame");
        /// <summary>
        /// Full Path: Aequus/Buffs/Debuffs/CrimsonHellfire
        /// </summary>
        public static readonly TextureAsset CrimsonHellfire = new TextureAsset("Aequus/Buffs/Debuffs/CrimsonHellfire");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/CrownOfBlood/CrownOfBloodBuff
        /// </summary>
        public static readonly TextureAsset CrownOfBloodBuff = new TextureAsset("Aequus/Items/Accessories/CrownOfBlood/CrownOfBloodBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/CrownOfBlood/CrownOfBloodDebuff
        /// </summary>
        public static readonly TextureAsset CrownOfBloodDebuff = new TextureAsset("Aequus/Items/Accessories/CrownOfBlood/CrownOfBloodDebuff");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/CrownOfBlood/CrownOfBloodItem
        /// </summary>
        public static readonly TextureAsset CrownOfBloodItem = new TextureAsset("Aequus/Items/Accessories/CrownOfBlood/CrownOfBloodItem");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/CrownOfBlood/CrownOfBloodItem_Crown
        /// </summary>
        public static readonly TextureAsset CrownOfBloodItem_Crown = new TextureAsset("Aequus/Items/Accessories/CrownOfBlood/CrownOfBloodItem_Crown");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/CrownOfDarkness
        /// </summary>
        public static readonly TextureAsset CrownOfDarkness = new TextureAsset("Aequus/Items/Accessories/Offense/CrownOfDarkness");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/CrownOfDarkness_Crown
        /// </summary>
        public static readonly TextureAsset CrownOfDarkness_Crown = new TextureAsset("Aequus/Items/Accessories/Offense/CrownOfDarkness_Crown");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Crit/CrownOfTheGrounded
        /// </summary>
        public static readonly TextureAsset CrownOfTheGrounded = new TextureAsset("Aequus/Items/Accessories/Offense/Crit/CrownOfTheGrounded");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Crit/CrownOfTheGrounded_Crown
        /// </summary>
        public static readonly TextureAsset CrownOfTheGrounded_Crown = new TextureAsset("Aequus/Items/Accessories/Offense/Crit/CrownOfTheGrounded_Crown");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Ranged/CrusadersCrossbow
        /// </summary>
        public static readonly TextureAsset CrusadersCrossbow = new TextureAsset("Aequus/Items/Weapons/Ranged/CrusadersCrossbow");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Ranged/CrusadersCrossbowBolt
        /// </summary>
        public static readonly TextureAsset CrusadersCrossbowBolt = new TextureAsset("Aequus/Projectiles/Ranged/CrusadersCrossbowBolt");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/CrystalDagger
        /// </summary>
        public static readonly TextureAsset CrystalDagger = new TextureAsset("Aequus/Items/Weapons/Melee/CrystalDagger");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/CrystalDaggerBuff
        /// </summary>
        public static readonly TextureAsset CrystalDaggerBuff = new TextureAsset("Aequus/Buffs/Misc/CrystalDaggerBuff");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Bait/CursedPopper
        /// </summary>
        public static readonly TextureAsset CursedPopper = new TextureAsset("Aequus/Content/Fishing/Bait/CursedPopper");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_10
        /// </summary>
        public static readonly TextureAsset Cursor_10_PumpkingCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_10");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_10
        /// </summary>
        public static readonly TextureAsset Cursor_10_XmasCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_10");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_2
        /// </summary>
        public static readonly TextureAsset Cursor_2_PumpkingCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_2");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_2
        /// </summary>
        public static readonly TextureAsset Cursor_2_XmasCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_2");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_3
        /// </summary>
        public static readonly TextureAsset Cursor_3_PumpkingCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_3");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_3
        /// </summary>
        public static readonly TextureAsset Cursor_3_XmasCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_3");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_6
        /// </summary>
        public static readonly TextureAsset Cursor_6_PumpkingCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_6");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_6
        /// </summary>
        public static readonly TextureAsset Cursor_6_XmasCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_6");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_7
        /// </summary>
        public static readonly TextureAsset Cursor_7_PumpkingCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_7");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_7
        /// </summary>
        public static readonly TextureAsset Cursor_7_XmasCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_7");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_8
        /// </summary>
        public static readonly TextureAsset Cursor_8_PumpkingCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_8");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_8
        /// </summary>
        public static readonly TextureAsset Cursor_8_XmasCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_8");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_9
        /// </summary>
        public static readonly TextureAsset Cursor_9_PumpkingCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_9");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_9
        /// </summary>
        public static readonly TextureAsset Cursor_9_XmasCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_9");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor
        /// </summary>
        public static readonly TextureAsset Cursor_PumpkingCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_Smart
        /// </summary>
        public static readonly TextureAsset Cursor_Smart_PumpkingCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/PumpkingCursor/Cursor_Smart");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_Smart
        /// </summary>
        public static readonly TextureAsset Cursor_Smart_XmasCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/XmasCursor/Cursor_Smart");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/XmasCursor/Cursor
        /// </summary>
        public static readonly TextureAsset Cursor_XmasCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/XmasCursor/Cursor");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetTrap/DartTrapHat
        /// </summary>
        public static readonly TextureAsset DartTrapHat = new TextureAsset("Aequus/Items/Armor/SetTrap/DartTrapHat");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetTrap/DartTrapHat_Head
        /// </summary>
        public static readonly TextureAsset DartTrapHat_Head = new TextureAsset("Aequus/Items/Armor/SetTrap/DartTrapHat_Head");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/DavyJonesAnchor
        /// </summary>
        public static readonly TextureAsset DavyJonesAnchor = new TextureAsset("Aequus/Items/Accessories/Offense/DavyJonesAnchor");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/Friendly/DavyJonesAnchorProj
        /// </summary>
        public static readonly TextureAsset DavyJonesAnchorProj = new TextureAsset("Aequus/Projectiles/Misc/Friendly/DavyJonesAnchorProj");
        /// <summary>
        /// Full Path: Aequus/Items/Potions/Unique/DeathsEmbrace
        /// </summary>
        public static readonly TextureAsset DeathsEmbrace = new TextureAsset("Aequus/Items/Potions/Unique/DeathsEmbrace");
        /// <summary>
        /// Full Path: Aequus/Buffs/DeathsEmbraceBuff
        /// </summary>
        public static readonly TextureAsset DeathsEmbraceBuff = new TextureAsset("Aequus/Buffs/DeathsEmbraceBuff");
        /// <summary>
        /// Full Path: Aequus/Buffs/Debuffs/Debuff
        /// </summary>
        public static readonly TextureAsset Debuff = new TextureAsset("Aequus/Buffs/Debuffs/Debuff");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Ranged/Deltoid
        /// </summary>
        public static readonly TextureAsset Deltoid = new TextureAsset("Aequus/Items/Weapons/Ranged/Deltoid");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Ranged/Deltoid_Glow
        /// </summary>
        public static readonly TextureAsset Deltoid_Glow = new TextureAsset("Aequus/Items/Weapons/Ranged/Deltoid_Glow");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Ranged/DeltoidArrow
        /// </summary>
        public static readonly TextureAsset DeltoidArrow = new TextureAsset("Aequus/Projectiles/Ranged/DeltoidArrow");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor_2
        /// </summary>
        public static readonly TextureAsset DemonCursor_2 = new TextureAsset("Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor_2");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor_3
        /// </summary>
        public static readonly TextureAsset DemonCursor_3 = new TextureAsset("Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor_3");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor_6
        /// </summary>
        public static readonly TextureAsset DemonCursor_6 = new TextureAsset("Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor_6");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor_6Outline
        /// </summary>
        public static readonly TextureAsset DemonCursor_6Outline = new TextureAsset("Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor_6Outline");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor
        /// </summary>
        public static readonly TextureAsset DemonCursor_DemonCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/DemonCursor
        /// </summary>
        public static readonly TextureAsset DemonCursor_Items = new TextureAsset("Aequus/Content/CursorDyes/Items/DemonCursor");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor_Smart
        /// </summary>
        public static readonly TextureAsset DemonCursor_Smart = new TextureAsset("Aequus/Content/CursorDyes/Items/DemonCursor/DemonCursor_Smart");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/Energies/DemonicEnergy
        /// </summary>
        public static readonly TextureAsset DemonicEnergy = new TextureAsset("Aequus/Items/Materials/Energies/DemonicEnergy");
        /// <summary>
        /// Full Path: Aequus/Assets/Effects/Textures/DemonicEnergyGradient
        /// </summary>
        public static readonly TextureAsset DemonicEnergyGradient = new TextureAsset("Aequus/Assets/Effects/Textures/DemonicEnergyGradient");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/BestiaryIcons/DemonSiege
        /// </summary>
        public static readonly TextureAsset DemonSiege_BestiaryIcons = new TextureAsset("Aequus/Assets/UI/BestiaryIcons/DemonSiege");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/BossChecklist/DemonSiege
        /// </summary>
        public static readonly TextureAsset DemonSiege_BossChecklist = new TextureAsset("Aequus/Assets/UI/BossChecklist/DemonSiege");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/EventIcons/DemonSiege
        /// </summary>
        public static readonly TextureAsset DemonSiege_EventIcons = new TextureAsset("Aequus/Assets/UI/EventIcons/DemonSiege");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Misc/Depthscale
        /// </summary>
        public static readonly TextureAsset Depthscale = new TextureAsset("Aequus/Content/Fishing/Misc/Depthscale");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/SlotMachines/DesertRoulette
        /// </summary>
        public static readonly TextureAsset DesertRoulette = new TextureAsset("Aequus/Unused/Items/SlotMachines/DesertRoulette");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Equipment/DevilsTongue
        /// </summary>
        public static readonly TextureAsset DevilsTongue = new TextureAsset("Aequus/Content/Fishing/Equipment/DevilsTongue");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/BigGems/DiamondDeposit
        /// </summary>
        public static readonly TextureAsset DiamondDeposit = new TextureAsset("Aequus/Tiles/Misc/BigGems/DiamondDeposit");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Gems/DiamondGore
        /// </summary>
        public static readonly TextureAsset DiamondGore = new TextureAsset("Aequus/Assets/Gores/Gems/DiamondGore");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Dyes/DiscoDye
        /// </summary>
        public static readonly TextureAsset DiscoDye = new TextureAsset("Aequus/Items/Misc/Dyes/DiscoDye");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Summon/Minion/DisturbanceStaff
        /// </summary>
        public static readonly TextureAsset DisturbanceStaff = new TextureAsset("Aequus/Items/Weapons/Summon/Minion/DisturbanceStaff");
        /// <summary>
        /// Full Path: Aequus/Items/Vanity/Pets/Light/DragonBall
        /// </summary>
        public static readonly TextureAsset DragonBall = new TextureAsset("Aequus/Items/Vanity/Pets/Light/DragonBall");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Candles/DragonsBreath
        /// </summary>
        public static readonly TextureAsset DragonsBreath = new TextureAsset("Aequus/Items/Weapons/Necromancy/Candles/DragonsBreath");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Candles/DragonsBreath_Flame
        /// </summary>
        public static readonly TextureAsset DragonsBreath_Flame = new TextureAsset("Aequus/Items/Weapons/Necromancy/Candles/DragonsBreath_Flame");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Misc/Driftwood
        /// </summary>
        public static readonly TextureAsset Driftwood = new TextureAsset("Aequus/Content/Fishing/Misc/Driftwood");
        /// <summary>
        /// Full Path: Aequus/Buffs/Pets/DroneBuff
        /// </summary>
        public static readonly TextureAsset DroneBuff = new TextureAsset("Aequus/Buffs/Pets/DroneBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Candles/DungeonCandle
        /// </summary>
        public static readonly TextureAsset DungeonCandle = new TextureAsset("Aequus/Items/Weapons/Necromancy/Candles/DungeonCandle");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Candles/DungeonCandle_Flame
        /// </summary>
        public static readonly TextureAsset DungeonCandle_Flame = new TextureAsset("Aequus/Items/Weapons/Necromancy/Candles/DungeonCandle_Flame");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/BossChecklist/DustDevil
        /// </summary>
        public static readonly TextureAsset DustDevil_BossChecklist = new TextureAsset("Aequus/Assets/UI/BossChecklist/DustDevil");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/DustDevil/DustDevil
        /// </summary>
        public static readonly TextureAsset DustDevil_DustDevil = new TextureAsset("Aequus/Content/Boss/DustDevil/DustDevil");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/DustDevil/DustDevil_Head_Boss
        /// </summary>
        public static readonly TextureAsset DustDevil_Head_Boss = new TextureAsset("Aequus/Content/Boss/DustDevil/DustDevil_Head_Boss");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/DustDevil/Rewards/DustDevilBag
        /// </summary>
        public static readonly TextureAsset DustDevilBag = new TextureAsset("Aequus/Content/Boss/DustDevil/Rewards/DustDevilBag");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/DustDevil/Rewards/DustDevilRelic
        /// </summary>
        public static readonly TextureAsset DustDevilRelic = new TextureAsset("Aequus/Content/Boss/DustDevil/Rewards/DustDevilRelic");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/DustDevil/Rewards/DustDevilTrophy
        /// </summary>
        public static readonly TextureAsset DustDevilTrophy = new TextureAsset("Aequus/Content/Boss/DustDevil/Rewards/DustDevilTrophy");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/DustDevil/DustParticle
        /// </summary>
        public static readonly TextureAsset DustParticle = new TextureAsset("Aequus/Content/Boss/DustDevil/DustParticle");
        /// <summary>
        /// Full Path: Aequus/Content/Critters/DwarfStarite
        /// </summary>
        public static readonly TextureAsset DwarfStarite = new TextureAsset("Aequus/Content/Critters/DwarfStarite");
        /// <summary>
        /// Full Path: Aequus/Content/Critters/DwarfStariteItem
        /// </summary>
        public static readonly TextureAsset DwarfStariteItem = new TextureAsset("Aequus/Content/Critters/DwarfStariteItem");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/DyableCursor
        /// </summary>
        public static readonly TextureAsset DyableCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/DyableCursor");
        /// <summary>
        /// Full Path: Aequus/Assets/DyeSample
        /// </summary>
        public static readonly TextureAsset DyeSample = new TextureAsset("Aequus/Assets/DyeSample");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/Dynaknife
        /// </summary>
        public static readonly TextureAsset Dynaknife = new TextureAsset("Aequus/Items/Weapons/Melee/Dynaknife");
        /// <summary>
        /// Full Path: Aequus/Assets/Textures/EffectNoise
        /// </summary>
        public static readonly TextureAsset EffectNoise = new TextureAsset("Aequus/Assets/Textures/EffectNoise");
        /// <summary>
        /// Full Path: Aequus/Assets/Textures/EffectWaterRefraction
        /// </summary>
        public static readonly TextureAsset EffectWaterRefraction = new TextureAsset("Aequus/Assets/Textures/EffectWaterRefraction");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/ElitePlants/ElitePlantArgon
        /// </summary>
        public static readonly TextureAsset ElitePlantArgon = new TextureAsset("Aequus/Content/Biomes/MossBiomes/Tiles/ElitePlants/ElitePlantArgon");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/ElitePlants/ElitePlantKrypton
        /// </summary>
        public static readonly TextureAsset ElitePlantKrypton = new TextureAsset("Aequus/Content/Biomes/MossBiomes/Tiles/ElitePlants/ElitePlantKrypton");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/ElitePlants/ElitePlantNeon
        /// </summary>
        public static readonly TextureAsset ElitePlantNeon = new TextureAsset("Aequus/Content/Biomes/MossBiomes/Tiles/ElitePlants/ElitePlantNeon");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/ElitePlants/ElitePlantTile
        /// </summary>
        public static readonly TextureAsset ElitePlantTile = new TextureAsset("Aequus/Content/Biomes/MossBiomes/Tiles/ElitePlants/ElitePlantTile");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/ElitePlants/ElitePlantXenon
        /// </summary>
        public static readonly TextureAsset ElitePlantXenon = new TextureAsset("Aequus/Content/Biomes/MossBiomes/Tiles/ElitePlants/ElitePlantXenon");
        /// <summary>
        /// Full Path: Aequus/Tiles/Blocks/EmancipationGrill
        /// </summary>
        public static readonly TextureAsset EmancipationGrill = new TextureAsset("Aequus/Tiles/Blocks/EmancipationGrill");
        /// <summary>
        /// Full Path: Aequus/Tiles/Blocks/EmancipationGrillTile
        /// </summary>
        public static readonly TextureAsset EmancipationGrillTile = new TextureAsset("Aequus/Tiles/Blocks/EmancipationGrillTile");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/BigGems/EmeraldDeposit
        /// </summary>
        public static readonly TextureAsset EmeraldDeposit = new TextureAsset("Aequus/Tiles/Misc/BigGems/EmeraldDeposit");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Gems/EmeraldGore
        /// </summary>
        public static readonly TextureAsset EmeraldGore = new TextureAsset("Aequus/Assets/Gores/Gems/EmeraldGore");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredArchery
        /// </summary>
        public static readonly TextureAsset EmpoweredArchery = new TextureAsset("Aequus/Buffs/Misc/Empowered/EmpoweredArchery");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredEndurance
        /// </summary>
        public static readonly TextureAsset EmpoweredEndurance = new TextureAsset("Aequus/Buffs/Misc/Empowered/EmpoweredEndurance");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredFishing
        /// </summary>
        public static readonly TextureAsset EmpoweredFishing = new TextureAsset("Aequus/Buffs/Misc/Empowered/EmpoweredFishing");
        /// <summary>
        /// Full Path: Aequus/Content/ItemPrefixes/Potions/EmpoweredGlint
        /// </summary>
        public static readonly TextureAsset EmpoweredGlint = new TextureAsset("Aequus/Content/ItemPrefixes/Potions/EmpoweredGlint");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredIronskin
        /// </summary>
        public static readonly TextureAsset EmpoweredIronskin = new TextureAsset("Aequus/Buffs/Misc/Empowered/EmpoweredIronskin");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredMagicPower
        /// </summary>
        public static readonly TextureAsset EmpoweredMagicPower = new TextureAsset("Aequus/Buffs/Misc/Empowered/EmpoweredMagicPower");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredManaRegeneration
        /// </summary>
        public static readonly TextureAsset EmpoweredManaRegeneration = new TextureAsset("Aequus/Buffs/Misc/Empowered/EmpoweredManaRegeneration");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredMining
        /// </summary>
        public static readonly TextureAsset EmpoweredMining = new TextureAsset("Aequus/Buffs/Misc/Empowered/EmpoweredMining");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredNecromancy
        /// </summary>
        public static readonly TextureAsset EmpoweredNecromancy = new TextureAsset("Aequus/Buffs/Misc/Empowered/EmpoweredNecromancy");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredRage
        /// </summary>
        public static readonly TextureAsset EmpoweredRage = new TextureAsset("Aequus/Buffs/Misc/Empowered/EmpoweredRage");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredRegeneration
        /// </summary>
        public static readonly TextureAsset EmpoweredRegeneration = new TextureAsset("Aequus/Buffs/Misc/Empowered/EmpoweredRegeneration");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredSentry
        /// </summary>
        public static readonly TextureAsset EmpoweredSentry = new TextureAsset("Aequus/Buffs/Misc/Empowered/EmpoweredSentry");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredShine
        /// </summary>
        public static readonly TextureAsset EmpoweredShine = new TextureAsset("Aequus/Buffs/Misc/Empowered/EmpoweredShine");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredSummoning
        /// </summary>
        public static readonly TextureAsset EmpoweredSummoning = new TextureAsset("Aequus/Buffs/Misc/Empowered/EmpoweredSummoning");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredSwiftness
        /// </summary>
        public static readonly TextureAsset EmpoweredSwiftness = new TextureAsset("Aequus/Buffs/Misc/Empowered/EmpoweredSwiftness");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredThorns
        /// </summary>
        public static readonly TextureAsset EmpoweredThorns = new TextureAsset("Aequus/Buffs/Misc/Empowered/EmpoweredThorns");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/Empowered/EmpoweredWrath
        /// </summary>
        public static readonly TextureAsset EmpoweredWrath = new TextureAsset("Aequus/Buffs/Misc/Empowered/EmpoweredWrath");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Dyes/EnchantedDye
        /// </summary>
        public static readonly TextureAsset EnchantedDye = new TextureAsset("Aequus/Items/Misc/Dyes/EnchantedDye");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Dyes/EnchantedDyeEffect
        /// </summary>
        public static readonly TextureAsset EnchantedDyeEffect = new TextureAsset("Aequus/Items/Misc/Dyes/EnchantedDyeEffect");
        /// <summary>
        /// Full Path: Aequus/Particles/Dusts/EnergyDust
        /// </summary>
        public static readonly TextureAsset EnergyDust = new TextureAsset("Aequus/Particles/Dusts/EnergyDust");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/DebugItems/EnthrallingScepter
        /// </summary>
        public static readonly TextureAsset EnthrallingScepter = new TextureAsset("Aequus/Unused/Items/DebugItems/EnthrallingScepter");
        /// <summary>
        /// Full Path: Aequus/Content/CrossMod/SplitSupport/Photography/EnvelopeGlimmer
        /// </summary>
        public static readonly TextureAsset EnvelopeGlimmer = new TextureAsset("Aequus/Content/CrossMod/SplitSupport/Photography/EnvelopeGlimmer");
        /// <summary>
        /// Full Path: Aequus/Content/CrossMod/SplitSupport/Photography/EnvelopeUndergroundOcean
        /// </summary>
        public static readonly TextureAsset EnvelopeUndergroundOcean = new TextureAsset("Aequus/Content/CrossMod/SplitSupport/Photography/EnvelopeUndergroundOcean");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Paintings/Items/ExLydSpacePainting
        /// </summary>
        public static readonly TextureAsset ExLydSpacePainting = new TextureAsset("Aequus/Tiles/Furniture/Paintings/Items/ExLydSpacePainting");
        /// <summary>
        /// Full Path: Aequus/Assets/Explosion0
        /// </summary>
        public static readonly TextureAsset Explosion0 = new TextureAsset("Aequus/Assets/Explosion0");
        /// <summary>
        /// Full Path: Aequus/Assets/Explosion1
        /// </summary>
        public static readonly TextureAsset Explosion1 = new TextureAsset("Aequus/Assets/Explosion1");
        /// <summary>
        /// Full Path: Aequus/Content/Town/ExporterNPC/Exporter
        /// </summary>
        public static readonly TextureAsset Exporter = new TextureAsset("Aequus/Content/Town/ExporterNPC/Exporter");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Exporter_0
        /// </summary>
        public static readonly TextureAsset Exporter_0 = new TextureAsset("Aequus/Assets/Gores/Exporter_0");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Exporter_1
        /// </summary>
        public static readonly TextureAsset Exporter_1 = new TextureAsset("Aequus/Assets/Gores/Exporter_1");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Exporter_2
        /// </summary>
        public static readonly TextureAsset Exporter_2 = new TextureAsset("Aequus/Assets/Gores/Exporter_2");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Exporter_3
        /// </summary>
        public static readonly TextureAsset Exporter_3 = new TextureAsset("Aequus/Assets/Gores/Exporter_3");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Exporter_4
        /// </summary>
        public static readonly TextureAsset Exporter_4 = new TextureAsset("Aequus/Assets/Gores/Exporter_4");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Exporter_5
        /// </summary>
        public static readonly TextureAsset Exporter_5 = new TextureAsset("Aequus/Assets/Gores/Exporter_5");
        /// <summary>
        /// Full Path: Aequus/Content/Town/ExporterNPC/Exporter_Head
        /// </summary>
        public static readonly TextureAsset Exporter_Head = new TextureAsset("Aequus/Content/Town/ExporterNPC/Exporter_Head");
        /// <summary>
        /// Full Path: Aequus/Items/Vanity/EyeGlint
        /// </summary>
        public static readonly TextureAsset EyeGlint = new TextureAsset("Aequus/Items/Vanity/EyeGlint");
        /// <summary>
        /// Full Path: Aequus/Buffs/Pets/FamiliarBuff
        /// </summary>
        public static readonly TextureAsset FamiliarBuff = new TextureAsset("Aequus/Buffs/Pets/FamiliarBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Vanity/Pets/FamiliarPickaxe
        /// </summary>
        public static readonly TextureAsset FamiliarPickaxe = new TextureAsset("Aequus/Items/Vanity/Pets/FamiliarPickaxe");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Misc/FaultyCoin
        /// </summary>
        public static readonly TextureAsset FaultyCoin = new TextureAsset("Aequus/Items/Accessories/Misc/FaultyCoin");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/FaultyCoinBuff
        /// </summary>
        public static readonly TextureAsset FaultyCoinBuff = new TextureAsset("Aequus/Buffs/Misc/FaultyCoinBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Consumables/FertilePowder
        /// </summary>
        public static readonly TextureAsset FertilePowder = new TextureAsset("Aequus/Items/Consumables/FertilePowder");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Crates/FishingCratesTile
        /// </summary>
        public static readonly TextureAsset FishingCratesTile = new TextureAsset("Aequus/Content/Fishing/Crates/FishingCratesTile");
        /// <summary>
        /// Full Path: Aequus/Items/Placeable/Furniture/Interactable/FishSign
        /// </summary>
        public static readonly TextureAsset FishSign = new TextureAsset("Aequus/Items/Placeable/Furniture/Interactable/FishSign");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/FishSignTile
        /// </summary>
        public static readonly TextureAsset FishSignTile = new TextureAsset("Aequus/Tiles/Misc/FishSignTile");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/FishSignTile_Highlight
        /// </summary>
        public static readonly TextureAsset FishSignTile_Highlight = new TextureAsset("Aequus/Tiles/Misc/FishSignTile_Highlight");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Utility/FishyFins
        /// </summary>
        public static readonly TextureAsset FishyFins = new TextureAsset("Aequus/Items/Accessories/Utility/FishyFins");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Utility/FishyFins_Ears
        /// </summary>
        public static readonly TextureAsset FishyFins_Ears = new TextureAsset("Aequus/Items/Accessories/Utility/FishyFins_Ears");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Ranged/Flameblaster
        /// </summary>
        public static readonly TextureAsset Flameblaster = new TextureAsset("Aequus/Items/Weapons/Ranged/Flameblaster");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Ranged/Flameblaster_Glow
        /// </summary>
        public static readonly TextureAsset Flameblaster_Glow = new TextureAsset("Aequus/Items/Weapons/Ranged/Flameblaster_Glow");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Ranged/FlameblasterProj
        /// </summary>
        public static readonly TextureAsset FlameblasterProj = new TextureAsset("Aequus/Projectiles/Ranged/FlameblasterProj");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Ranged/FlameblasterWind
        /// </summary>
        public static readonly TextureAsset FlameblasterWind = new TextureAsset("Aequus/Projectiles/Ranged/FlameblasterWind");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Debuff/FlameCrystal
        /// </summary>
        public static readonly TextureAsset FlameCrystal = new TextureAsset("Aequus/Items/Accessories/Debuff/FlameCrystal");
        /// <summary>
        /// Full Path: Aequus/Assets/Textures/Flare
        /// </summary>
        public static readonly TextureAsset Flare = new TextureAsset("Aequus/Assets/Textures/Flare");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Defense/FlashwayNecklace
        /// </summary>
        public static readonly TextureAsset FlashwayNecklace = new TextureAsset("Aequus/Items/Accessories/Defense/FlashwayNecklace");
        /// <summary>
        /// Full Path: Aequus/Buffs/Cooldowns/FlashwayNecklaceCooldown
        /// </summary>
        public static readonly TextureAsset FlashwayNecklaceCooldown = new TextureAsset("Aequus/Buffs/Cooldowns/FlashwayNecklaceCooldown");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/FlaskBuff
        /// </summary>
        public static readonly TextureAsset FlaskBuff = new TextureAsset("Aequus/Buffs/Misc/FlaskBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/FlowerCrown
        /// </summary>
        public static readonly TextureAsset FlowerCrown = new TextureAsset("Aequus/Items/Armor/FlowerCrown");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/FlowerCrown_Head
        /// </summary>
        public static readonly TextureAsset FlowerCrown_Head = new TextureAsset("Aequus/Items/Armor/FlowerCrown_Head");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/Fluorescence
        /// </summary>
        public static readonly TextureAsset Fluorescence = new TextureAsset("Aequus/Items/Materials/Fluorescence");
        /// <summary>
        /// Full Path: Aequus/Particles/FogParticle
        /// </summary>
        public static readonly TextureAsset FogParticle = new TextureAsset("Aequus/Particles/FogParticle");
        /// <summary>
        /// Full Path: Aequus/Particles/FogParticleHQ
        /// </summary>
        public static readonly TextureAsset FogParticleHQ = new TextureAsset("Aequus/Particles/FogParticleHQ");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Misc/FoolsGoldRing
        /// </summary>
        public static readonly TextureAsset FoolsGoldRing = new TextureAsset("Aequus/Items/Accessories/Misc/FoolsGoldRing");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Misc/FoolsGoldRing_HandsOn
        /// </summary>
        public static readonly TextureAsset FoolsGoldRing_HandsOn = new TextureAsset("Aequus/Items/Accessories/Misc/FoolsGoldRing_HandsOn");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/FoolsGoldRingBuff
        /// </summary>
        public static readonly TextureAsset FoolsGoldRingBuff = new TextureAsset("Aequus/Buffs/Misc/FoolsGoldRingBuff");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Quest/Bounties/Icons/FountainBounty
        /// </summary>
        public static readonly TextureAsset FountainBounty = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Quest/Bounties/Icons/FountainBounty");
        /// <summary>
        /// Full Path: Aequus/Buffs/Buildings/FountainBountyBuff
        /// </summary>
        public static readonly TextureAsset FountainBountyBuff = new TextureAsset("Aequus/Buffs/Buildings/FountainBountyBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Tools/FriendshipMagick
        /// </summary>
        public static readonly TextureAsset FriendshipMagick = new TextureAsset("Aequus/Items/Tools/FriendshipMagick");
        /// <summary>
        /// Full Path: Aequus/Items/Tools/FriendshipMagick_Glow
        /// </summary>
        public static readonly TextureAsset FriendshipMagick_Glow = new TextureAsset("Aequus/Items/Tools/FriendshipMagick_Glow");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/Friendly/FriendshipMagickProj
        /// </summary>
        public static readonly TextureAsset FriendshipMagickProj = new TextureAsset("Aequus/Projectiles/Misc/Friendly/FriendshipMagickProj");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/Friendly/FriendshipMagickProj_Aura
        /// </summary>
        public static readonly TextureAsset FriendshipMagickProj_Aura = new TextureAsset("Aequus/Projectiles/Misc/Friendly/FriendshipMagickProj_Aura");
        /// <summary>
        /// Full Path: Aequus/Assets/Textures/FriendshipParticle
        /// </summary>
        public static readonly TextureAsset FriendshipParticle = new TextureAsset("Aequus/Assets/Textures/FriendshipParticle");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Dyes/FrostbiteDye
        /// </summary>
        public static readonly TextureAsset FrostbiteDye = new TextureAsset("Aequus/Items/Misc/Dyes/FrostbiteDye");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Dyes/FrostbiteDyeEffect
        /// </summary>
        public static readonly TextureAsset FrostbiteDyeEffect = new TextureAsset("Aequus/Items/Misc/Dyes/FrostbiteDyeEffect");
        /// <summary>
        /// Full Path: Aequus/Buffs/FrostBuff
        /// </summary>
        public static readonly TextureAsset FrostBuff = new TextureAsset("Aequus/Buffs/FrostBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Potions/FrostPotion
        /// </summary>
        public static readonly TextureAsset FrostPotion = new TextureAsset("Aequus/Items/Potions/FrostPotion");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/FrozenTear
        /// </summary>
        public static readonly TextureAsset FrozenTear = new TextureAsset("Aequus/Items/Materials/FrozenTear");
        /// <summary>
        /// Full Path: Aequus/Content/Events/GlimmerEvent/Misc/GalacticStarfruit
        /// </summary>
        public static readonly TextureAsset GalacticStarfruit = new TextureAsset("Aequus/Content/Events/GlimmerEvent/Misc/GalacticStarfruit");
        /// <summary>
        /// Full Path: Aequus/Items/Consumables/Permanent/GalaxyCommission
        /// </summary>
        public static readonly TextureAsset GalaxyCommission = new TextureAsset("Aequus/Items/Consumables/Permanent/GalaxyCommission");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/BestiaryIcons/GaleStreams
        /// </summary>
        public static readonly TextureAsset GaleStreams_BestiaryIcons = new TextureAsset("Aequus/Assets/UI/BestiaryIcons/GaleStreams");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/BossChecklist/GaleStreams
        /// </summary>
        public static readonly TextureAsset GaleStreams_BossChecklist = new TextureAsset("Aequus/Assets/UI/BossChecklist/GaleStreams");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Magic/Gamestar
        /// </summary>
        public static readonly TextureAsset Gamestar = new TextureAsset("Aequus/Items/Weapons/Magic/Gamestar");
        /// <summary>
        /// Full Path: Aequus/Assets/Particles/GamestarParticle
        /// </summary>
        public static readonly TextureAsset GamestarParticle = new TextureAsset("Aequus/Assets/Particles/GamestarParticle");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Magic/GhastlyBlaster
        /// </summary>
        public static readonly TextureAsset GhastlyBlaster = new TextureAsset("Aequus/Items/Weapons/Magic/GhastlyBlaster");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Magic/GhastlyBlaster_Glow
        /// </summary>
        public static readonly TextureAsset GhastlyBlaster_Glow = new TextureAsset("Aequus/Items/Weapons/Magic/GhastlyBlaster_Glow");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Magic/GhastlyBlasterProj
        /// </summary>
        public static readonly TextureAsset GhastlyBlasterProj = new TextureAsset("Aequus/Projectiles/Magic/GhastlyBlasterProj");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Magic/GhastlyBlasterProj_Glow
        /// </summary>
        public static readonly TextureAsset GhastlyBlasterProj_Glow = new TextureAsset("Aequus/Projectiles/Magic/GhastlyBlasterProj_Glow");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Magic/GhastlyBlasterProjLaser
        /// </summary>
        public static readonly TextureAsset GhastlyBlasterProjLaser = new TextureAsset("Aequus/Projectiles/Magic/GhastlyBlasterProjLaser");
        /// <summary>
        /// Full Path: Aequus/Particles/Dusts/GhostDrainDust
        /// </summary>
        public static readonly TextureAsset GhostDrainDust = new TextureAsset("Aequus/Particles/Dusts/GhostDrainDust");
        /// <summary>
        /// Full Path: Aequus/Items/Tools/GhostlyGrave
        /// </summary>
        public static readonly TextureAsset GhostlyGrave = new TextureAsset("Aequus/Items/Tools/GhostlyGrave");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/GiftingSpirit
        /// </summary>
        public static readonly TextureAsset GiftingSpirit = new TextureAsset("Aequus/Items/Misc/GiftingSpirit");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/BestiaryIcons/Glimmer
        /// </summary>
        public static readonly TextureAsset Glimmer_BestiaryIcons = new TextureAsset("Aequus/Assets/UI/BestiaryIcons/Glimmer");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/BossChecklist/Glimmer
        /// </summary>
        public static readonly TextureAsset Glimmer_BossChecklist = new TextureAsset("Aequus/Assets/UI/BossChecklist/Glimmer");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/EventIcons/Glimmer
        /// </summary>
        public static readonly TextureAsset Glimmer_EventIcons = new TextureAsset("Aequus/Assets/UI/EventIcons/Glimmer");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/MapBGs/Glimmer
        /// </summary>
        public static readonly TextureAsset Glimmer_MapBGs = new TextureAsset("Aequus/Assets/UI/MapBGs/Glimmer");
        /// <summary>
        /// Full Path: Aequus/Content/Events/GlimmerEvent/Sky/GlimmerSky
        /// </summary>
        public static readonly TextureAsset GlimmerSky = new TextureAsset("Aequus/Content/Events/GlimmerEvent/Sky/GlimmerSky");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Utility/GlowCore
        /// </summary>
        public static readonly TextureAsset GlowCore = new TextureAsset("Aequus/Items/Accessories/Utility/GlowCore");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/GlowLichen
        /// </summary>
        public static readonly TextureAsset GlowLichen = new TextureAsset("Aequus/Items/Materials/GlowLichen");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/SlotMachines/GoldenRoulette
        /// </summary>
        public static readonly TextureAsset GoldenRoulette = new TextureAsset("Aequus/Unused/Items/SlotMachines/GoldenRoulette");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/LegendaryFish/GoreFish
        /// </summary>
        public static readonly TextureAsset GoreFish = new TextureAsset("Aequus/Content/Fishing/LegendaryFish/GoreFish");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/GoreNest/Tiles/GoreNest
        /// </summary>
        public static readonly TextureAsset GoreNest = new TextureAsset("Aequus/Content/Biomes/GoreNest/Tiles/GoreNest");
        /// <summary>
        /// Full Path: Aequus/Content/Events/DemonSiege/GoreNestAura
        /// </summary>
        public static readonly TextureAsset GoreNestAura = new TextureAsset("Aequus/Content/Events/DemonSiege/GoreNestAura");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Paintings/Items/GoreNestPainting
        /// </summary>
        public static readonly TextureAsset GoreNestPainting = new TextureAsset("Aequus/Tiles/Furniture/Paintings/Items/GoreNestPainting");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/GoreNest/Tiles/GoreNestStalagmite
        /// </summary>
        public static readonly TextureAsset GoreNestStalagmite = new TextureAsset("Aequus/Content/Biomes/GoreNest/Tiles/GoreNestStalagmite");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/GoreNest/Tiles/GoreNestTile
        /// </summary>
        public static readonly TextureAsset GoreNestTile = new TextureAsset("Aequus/Content/Biomes/GoreNest/Tiles/GoreNestTile");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/GoreNest/Tiles/GoreNestTile_Highlight
        /// </summary>
        public static readonly TextureAsset GoreNestTile_Highlight = new TextureAsset("Aequus/Content/Biomes/GoreNest/Tiles/GoreNestTile_Highlight");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Utility/GrandReward
        /// </summary>
        public static readonly TextureAsset GrandReward = new TextureAsset("Aequus/Items/Accessories/Utility/GrandReward");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetGravetender/GravetenderHood
        /// </summary>
        public static readonly TextureAsset GravetenderHood = new TextureAsset("Aequus/Items/Armor/SetGravetender/GravetenderHood");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetGravetender/GravetenderHood_Head
        /// </summary>
        public static readonly TextureAsset GravetenderHood_Head = new TextureAsset("Aequus/Items/Armor/SetGravetender/GravetenderHood_Head");
        /// <summary>
        /// Full Path: Aequus/Buffs/Minion/GravetenderMinionBuff
        /// </summary>
        public static readonly TextureAsset GravetenderMinionBuff = new TextureAsset("Aequus/Buffs/Minion/GravetenderMinionBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetGravetender/GravetenderRobes
        /// </summary>
        public static readonly TextureAsset GravetenderRobes = new TextureAsset("Aequus/Items/Armor/SetGravetender/GravetenderRobes");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetGravetender/GravetenderRobes_Body
        /// </summary>
        public static readonly TextureAsset GravetenderRobes_Body = new TextureAsset("Aequus/Items/Armor/SetGravetender/GravetenderRobes_Body");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/Misc/GravetenderWisp
        /// </summary>
        public static readonly TextureAsset GravetenderWisp = new TextureAsset("Aequus/Projectiles/Summon/Misc/GravetenderWisp");
        /// <summary>
        /// Full Path: Aequus/Tiles/Blocks/GravityBlocks/Auras/GravityAura_0
        /// </summary>
        public static readonly TextureAsset GravityAura_0 = new TextureAsset("Aequus/Tiles/Blocks/GravityBlocks/Auras/GravityAura_0");
        /// <summary>
        /// Full Path: Aequus/Tiles/Blocks/GravityBlocks/Auras/GravityAura_1
        /// </summary>
        public static readonly TextureAsset GravityAura_1 = new TextureAsset("Aequus/Tiles/Blocks/GravityBlocks/Auras/GravityAura_1");
        /// <summary>
        /// Full Path: Aequus/Tiles/Blocks/GravityBlocks/GravityBlock
        /// </summary>
        public static readonly TextureAsset GravityBlock = new TextureAsset("Aequus/Tiles/Blocks/GravityBlocks/GravityBlock");
        /// <summary>
        /// Full Path: Aequus/Tiles/Blocks/GravityBlocks/GravityBlockTile
        /// </summary>
        public static readonly TextureAsset GravityBlockTile = new TextureAsset("Aequus/Tiles/Blocks/GravityBlocks/GravityBlockTile");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Gravity/GravityCandelabra
        /// </summary>
        public static readonly TextureAsset GravityCandelabra = new TextureAsset("Aequus/Tiles/Furniture/Gravity/GravityCandelabra");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Gravity/GravityCandle
        /// </summary>
        public static readonly TextureAsset GravityCandle = new TextureAsset("Aequus/Tiles/Furniture/Gravity/GravityCandle");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Gravity/GravityChair
        /// </summary>
        public static readonly TextureAsset GravityChair = new TextureAsset("Aequus/Tiles/Furniture/Gravity/GravityChair");
        /// <summary>
        /// Full Path: Aequus/Items/Placeable/Furniture/Gravity/GravityChest
        /// </summary>
        public static readonly TextureAsset GravityChest = new TextureAsset("Aequus/Items/Placeable/Furniture/Gravity/GravityChest");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Gravity/GravityChestTile
        /// </summary>
        public static readonly TextureAsset GravityChestTile = new TextureAsset("Aequus/Tiles/Furniture/Gravity/GravityChestTile");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Gravity/GravityChestTile_Glow
        /// </summary>
        public static readonly TextureAsset GravityChestTile_Glow = new TextureAsset("Aequus/Tiles/Furniture/Gravity/GravityChestTile_Glow");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Gravity/GravityChestTile_Highlight
        /// </summary>
        public static readonly TextureAsset GravityChestTile_Highlight = new TextureAsset("Aequus/Tiles/Furniture/Gravity/GravityChestTile_Highlight");
        /// <summary>
        /// Full Path: Aequus/Tiles/Blocks/GravityBlocks/Auras/GravityDust
        /// </summary>
        public static readonly TextureAsset GravityDust = new TextureAsset("Aequus/Tiles/Blocks/GravityBlocks/Auras/GravityDust");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Gravity/GravityLamp
        /// </summary>
        public static readonly TextureAsset GravityLamp = new TextureAsset("Aequus/Tiles/Furniture/Gravity/GravityLamp");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Gravity/GravityPiano
        /// </summary>
        public static readonly TextureAsset GravityPiano = new TextureAsset("Aequus/Tiles/Furniture/Gravity/GravityPiano");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Gravity/GravitySofa
        /// </summary>
        public static readonly TextureAsset GravitySofa = new TextureAsset("Aequus/Tiles/Furniture/Gravity/GravitySofa");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Gravity/GravityTable
        /// </summary>
        public static readonly TextureAsset GravityTable = new TextureAsset("Aequus/Tiles/Furniture/Gravity/GravityTable");
        /// <summary>
        /// Full Path: Aequus/Content/DronePylons/NPCs/GunnerDrone
        /// </summary>
        public static readonly TextureAsset GunnerDrone = new TextureAsset("Aequus/Content/DronePylons/NPCs/GunnerDrone");
        /// <summary>
        /// Full Path: Aequus/Content/DronePylons/NPCs/GunnerDrone_Extras
        /// </summary>
        public static readonly TextureAsset GunnerDrone_Extras = new TextureAsset("Aequus/Content/DronePylons/NPCs/GunnerDrone_Extras");
        /// <summary>
        /// Full Path: Aequus/Content/DronePylons/NPCs/GunnerDrone_Extras_Glow
        /// </summary>
        public static readonly TextureAsset GunnerDrone_Extras_Glow = new TextureAsset("Aequus/Content/DronePylons/NPCs/GunnerDrone_Extras_Glow");
        /// <summary>
        /// Full Path: Aequus/Content/DronePylons/NPCs/GunnerDrone_Glow
        /// </summary>
        public static readonly TextureAsset GunnerDrone_Glow = new TextureAsset("Aequus/Content/DronePylons/NPCs/GunnerDrone_Glow");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Dyes/GustDye
        /// </summary>
        public static readonly TextureAsset GustDye = new TextureAsset("Aequus/Items/Misc/Dyes/GustDye");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Summon/Minion/HailstormStaff
        /// </summary>
        public static readonly TextureAsset HailstormStaff = new TextureAsset("Aequus/Items/Weapons/Summon/Minion/HailstormStaff");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/HalloweenEnergy
        /// </summary>
        public static readonly TextureAsset HalloweenEnergy = new TextureAsset("Aequus/Unused/Items/HalloweenEnergy");
        /// <summary>
        /// Full Path: Aequus/Assets/Effects/Textures/HalloweenEnergyGradient
        /// </summary>
        public static readonly TextureAsset HalloweenEnergyGradient = new TextureAsset("Aequus/Assets/Effects/Textures/HalloweenEnergyGradient");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Utility/HaltingMachine
        /// </summary>
        public static readonly TextureAsset HaltingMachine = new TextureAsset("Aequus/Items/Accessories/Utility/HaltingMachine");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Utility/HaltingMagnet
        /// </summary>
        public static readonly TextureAsset HaltingMagnet = new TextureAsset("Aequus/Items/Accessories/Utility/HaltingMagnet");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Ranged/HamaYumi
        /// </summary>
        public static readonly TextureAsset HamaYumi = new TextureAsset("Aequus/Items/Weapons/Ranged/HamaYumi");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Ranged/HamaYumi_Glow
        /// </summary>
        public static readonly TextureAsset HamaYumi_Glow = new TextureAsset("Aequus/Items/Weapons/Ranged/HamaYumi_Glow");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Ranged/HamaYumiArrow
        /// </summary>
        public static readonly TextureAsset HamaYumiArrow = new TextureAsset("Aequus/Projectiles/Ranged/HamaYumiArrow");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/HardFrozenChest
        /// </summary>
        public static readonly TextureAsset HardFrozenChest = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/HardFrozenChest");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/HardFrozenChestTile
        /// </summary>
        public static readonly TextureAsset HardFrozenChestTile = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/HardFrozenChestTile");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/HardFrozenChestTile_Highlight
        /// </summary>
        public static readonly TextureAsset HardFrozenChestTile_Highlight = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/HardFrozenChestTile_Highlight");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/HardGraniteChest
        /// </summary>
        public static readonly TextureAsset HardGraniteChest = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/HardGraniteChest");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/HardGraniteChestTile
        /// </summary>
        public static readonly TextureAsset HardGraniteChestTile = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/HardGraniteChestTile");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/HardGraniteChestTile_Glow
        /// </summary>
        public static readonly TextureAsset HardGraniteChestTile_Glow = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/HardGraniteChestTile_Glow");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/HardGraniteChestTile_Highlight
        /// </summary>
        public static readonly TextureAsset HardGraniteChestTile_Highlight = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/HardGraniteChestTile_Highlight");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/HardJungleChest
        /// </summary>
        public static readonly TextureAsset HardJungleChest = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/HardJungleChest");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/HardJungleChestTile
        /// </summary>
        public static readonly TextureAsset HardJungleChestTile = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/HardJungleChestTile");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/HardJungleChestTile_Highlight
        /// </summary>
        public static readonly TextureAsset HardJungleChestTile_Highlight = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/HardJungleChestTile_Highlight");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/HardMarbleChest
        /// </summary>
        public static readonly TextureAsset HardMarbleChest = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/HardMarbleChest");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/HardMarbleChestTile
        /// </summary>
        public static readonly TextureAsset HardMarbleChestTile = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/HardMarbleChestTile");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/HardMarbleChestTile_Highlight
        /// </summary>
        public static readonly TextureAsset HardMarbleChestTile_Highlight = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/HardMarbleChestTile_Highlight");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/HardMushroomChest
        /// </summary>
        public static readonly TextureAsset HardMushroomChest = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/HardMushroomChest");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/HardMushroomChestTile
        /// </summary>
        public static readonly TextureAsset HardMushroomChestTile = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/HardMushroomChestTile");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/HardMushroomChestTile_Glow
        /// </summary>
        public static readonly TextureAsset HardMushroomChestTile_Glow = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/HardMushroomChestTile_Glow");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/HardMushroomChestTile_Highlight
        /// </summary>
        public static readonly TextureAsset HardMushroomChestTile_Highlight = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/HardMushroomChestTile_Highlight");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/HardSandstoneChest
        /// </summary>
        public static readonly TextureAsset HardSandstoneChest = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/HardSandstoneChest");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/HardSandstoneChestTile
        /// </summary>
        public static readonly TextureAsset HardSandstoneChestTile = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/HardSandstoneChestTile");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/HardmodeChests/HardSandstoneChestTile_Highlight
        /// </summary>
        public static readonly TextureAsset HardSandstoneChestTile_Highlight = new TextureAsset("Aequus/Tiles/Misc/HardmodeChests/HardSandstoneChestTile_Highlight");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Jungle/Might/Hardwood
        /// </summary>
        public static readonly TextureAsset Hardwood = new TextureAsset("Aequus/NPCs/Monsters/Jungle/Might/Hardwood");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Jungle/Might/Hardwood_Glow
        /// </summary>
        public static readonly TextureAsset Hardwood_Glow = new TextureAsset("Aequus/NPCs/Monsters/Jungle/Might/Hardwood_Glow");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Monster/HardwoodProj
        /// </summary>
        public static readonly TextureAsset HardwoodProj = new TextureAsset("Aequus/Projectiles/Monster/HardwoodProj");
        /// <summary>
        /// Full Path: Aequus/Items/Vanity/Headless
        /// </summary>
        public static readonly TextureAsset Headless = new TextureAsset("Aequus/Items/Vanity/Headless");
        /// <summary>
        /// Full Path: Aequus/Items/Vanity/Headless_Head
        /// </summary>
        public static readonly TextureAsset Headless_Head = new TextureAsset("Aequus/Items/Vanity/Headless_Head");
        /// <summary>
        /// Full Path: Aequus/Content/DronePylons/NPCs/HealerDrone
        /// </summary>
        public static readonly TextureAsset HealerDrone = new TextureAsset("Aequus/Content/DronePylons/NPCs/HealerDrone");
        /// <summary>
        /// Full Path: Aequus/Content/DronePylons/NPCs/HealerDrone_Glow
        /// </summary>
        public static readonly TextureAsset HealerDrone_Glow = new TextureAsset("Aequus/Content/DronePylons/NPCs/HealerDrone_Glow");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/HealthCursor
        /// </summary>
        public static readonly TextureAsset HealthCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/HealthCursor");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/CrownOfBlood/Hearts/Heart
        /// </summary>
        public static readonly TextureAsset Heart = new TextureAsset("Aequus/Items/Accessories/CrownOfBlood/Hearts/Heart");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/CrownOfBlood/Hearts/Heart_Template
        /// </summary>
        public static readonly TextureAsset Heart_Template = new TextureAsset("Aequus/Items/Accessories/CrownOfBlood/Hearts/Heart_Template");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/CrownOfBlood/Hearts/Heart2
        /// </summary>
        public static readonly TextureAsset Heart2 = new TextureAsset("Aequus/Items/Accessories/CrownOfBlood/Hearts/Heart2");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Defense/HeartshatterNecklace
        /// </summary>
        public static readonly TextureAsset HeartshatterNecklace = new TextureAsset("Aequus/Items/Accessories/Defense/HeartshatterNecklace");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Heckto
        /// </summary>
        public static readonly TextureAsset Heckto = new TextureAsset("Aequus/NPCs/Monsters/Heckto");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/Heliosis
        /// </summary>
        public static readonly TextureAsset Heliosis = new TextureAsset("Aequus/Unused/Items/Heliosis");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/HeliumPlant
        /// </summary>
        public static readonly TextureAsset HeliumPlant = new TextureAsset("Aequus/Content/Biomes/MossBiomes/Tiles/HeliumPlant");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/HellsBoon
        /// </summary>
        public static readonly TextureAsset HellsBoon = new TextureAsset("Aequus/Items/Weapons/Melee/HellsBoon");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/HellsBoon_Glow
        /// </summary>
        public static readonly TextureAsset HellsBoon_Glow = new TextureAsset("Aequus/Items/Weapons/Melee/HellsBoon_Glow");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Melee/HellsBoonProj
        /// </summary>
        public static readonly TextureAsset HellsBoonProj = new TextureAsset("Aequus/Projectiles/Melee/HellsBoonProj");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/Hexoplasm
        /// </summary>
        public static readonly TextureAsset Hexoplasm = new TextureAsset("Aequus/Items/Materials/Hexoplasm");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Crit/HighSteaks
        /// </summary>
        public static readonly TextureAsset HighSteaks = new TextureAsset("Aequus/Items/Accessories/Offense/Crit/HighSteaks");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Crit/HighSteaks_Waist
        /// </summary>
        public static readonly TextureAsset HighSteaks_Waist = new TextureAsset("Aequus/Items/Accessories/Offense/Crit/HighSteaks_Waist");
        /// <summary>
        /// Full Path: Aequus/Tiles/Banners/Items/HijivarchCrabBanner
        /// </summary>
        public static readonly TextureAsset HijivarchCrabBanner = new TextureAsset("Aequus/Tiles/Banners/Items/HijivarchCrabBanner");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Ranged/Hitscanner
        /// </summary>
        public static readonly TextureAsset Hitscanner = new TextureAsset("Aequus/Items/Weapons/Ranged/Hitscanner");
        /// <summary>
        /// Full Path: Aequus/Items/Consumables/Foods/HolographicMeatloaf
        /// </summary>
        public static readonly TextureAsset HolographicMeatloaf = new TextureAsset("Aequus/Items/Consumables/Foods/HolographicMeatloaf");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Utility/HoloLens
        /// </summary>
        public static readonly TextureAsset HoloLens = new TextureAsset("Aequus/Items/Accessories/Utility/HoloLens");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Paintings/Items/HomeworldPainting
        /// </summary>
        public static readonly TextureAsset HomeworldPainting = new TextureAsset("Aequus/Tiles/Furniture/Paintings/Items/HomeworldPainting");
        /// <summary>
        /// Full Path: Aequus/Buffs/Mounts/HotAirBalloonBuff
        /// </summary>
        public static readonly TextureAsset HotAirBalloonBuff = new TextureAsset("Aequus/Buffs/Mounts/HotAirBalloonBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Mounts/HotAirBalloonMount
        /// </summary>
        public static readonly TextureAsset HotAirBalloonMount = new TextureAsset("Aequus/Items/Misc/Mounts/HotAirBalloonMount");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Mounts/HotAirBalloonMount_Back
        /// </summary>
        public static readonly TextureAsset HotAirBalloonMount_Back = new TextureAsset("Aequus/Items/Misc/Mounts/HotAirBalloonMount_Back");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Mounts/HotAirBalloonMount_Front
        /// </summary>
        public static readonly TextureAsset HotAirBalloonMount_Front = new TextureAsset("Aequus/Items/Misc/Mounts/HotAirBalloonMount_Front");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Dyes/HueshiftDye
        /// </summary>
        public static readonly TextureAsset HueshiftDye = new TextureAsset("Aequus/Items/Misc/Dyes/HueshiftDye");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/HyperCrystal
        /// </summary>
        public static readonly TextureAsset HyperCrystal = new TextureAsset("Aequus/Items/Accessories/Offense/HyperCrystal");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/Friendly/HyperCrystalProj
        /// </summary>
        public static readonly TextureAsset HyperCrystalProj = new TextureAsset("Aequus/Projectiles/Misc/Friendly/HyperCrystalProj");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Utility/HyperJet
        /// </summary>
        public static readonly TextureAsset HyperJet = new TextureAsset("Aequus/Items/Accessories/Utility/HyperJet");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Night/Glimmer/HyperStarite
        /// </summary>
        public static readonly TextureAsset HyperStarite = new TextureAsset("Aequus/NPCs/Monsters/Night/Glimmer/HyperStarite");
        /// <summary>
        /// Full Path: Aequus/Tiles/Banners/Items/HyperStariteBanner
        /// </summary>
        public static readonly TextureAsset HyperStariteBanner = new TextureAsset("Aequus/Tiles/Banners/Items/HyperStariteBanner");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Crabson/Misc/HypnoticPearl
        /// </summary>
        public static readonly TextureAsset HypnoticPearl = new TextureAsset("Aequus/Content/Boss/Crabson/Misc/HypnoticPearl");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Misc/IcebergFish
        /// </summary>
        public static readonly TextureAsset IcebergFish = new TextureAsset("Aequus/Content/Fishing/Misc/IcebergFish");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Sentry/IcebergKraken
        /// </summary>
        public static readonly TextureAsset IcebergKraken = new TextureAsset("Aequus/Items/Accessories/Offense/Sentry/IcebergKraken");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Sentry/IcebergKraken_Hat
        /// </summary>
        public static readonly TextureAsset IcebergKraken_Hat = new TextureAsset("Aequus/Items/Accessories/Offense/Sentry/IcebergKraken_Hat");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Bait/IchorPopper
        /// </summary>
        public static readonly TextureAsset IchorPopper = new TextureAsset("Aequus/Content/Fishing/Bait/IchorPopper");
        /// <summary>
        /// Full Path: Aequus/icon
        /// </summary>
        public static readonly TextureAsset icon = new TextureAsset("Aequus/icon");
        /// <summary>
        /// Full Path: Aequus/icon_small
        /// </summary>
        public static readonly TextureAsset icon_small = new TextureAsset("Aequus/icon_small");
        /// <summary>
        /// Full Path: Aequus/icon_workshop
        /// </summary>
        public static readonly TextureAsset icon_workshop = new TextureAsset("Aequus/icon_workshop");
        /// <summary>
        /// Full Path: Aequus/Content/CrossMod/SplitSupport/Photography/Icons
        /// </summary>
        public static readonly TextureAsset Icons = new TextureAsset("Aequus/Content/CrossMod/SplitSupport/Photography/Icons");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Paint/ImpenetrableCoating
        /// </summary>
        public static readonly TextureAsset ImpenetrableCoating = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Paint/ImpenetrableCoating");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/InspirationCursor
        /// </summary>
        public static readonly TextureAsset InspirationCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/InspirationCursor");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Scepters/Insurgency
        /// </summary>
        public static readonly TextureAsset Insurgency = new TextureAsset("Aequus/Items/Weapons/Necromancy/Scepters/Insurgency");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Scepters/Insurgency_Glow
        /// </summary>
        public static readonly TextureAsset Insurgency_Glow = new TextureAsset("Aequus/Items/Weapons/Necromancy/Scepters/Insurgency_Glow");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Paintings/Items/InsurgentPainting
        /// </summary>
        public static readonly TextureAsset InsurgentPainting = new TextureAsset("Aequus/Tiles/Furniture/Paintings/Items/InsurgentPainting");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/Necro/InsurgentSkull
        /// </summary>
        public static readonly TextureAsset InsurgentSkull = new TextureAsset("Aequus/Projectiles/Summon/Necro/InsurgentSkull");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/CrownOfBlood/InventoryBack
        /// </summary>
        public static readonly TextureAsset InventoryBack_CrownOfBlood = new TextureAsset("Aequus/Items/Accessories/CrownOfBlood/InventoryBack");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/SentryInheriters/InventoryBack
        /// </summary>
        public static readonly TextureAsset InventoryBack_SentryInheriters = new TextureAsset("Aequus/Items/Accessories/SentryInheriters/InventoryBack");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/InventoryBack
        /// </summary>
        public static readonly TextureAsset InventoryBack_UI = new TextureAsset("Aequus/Assets/UI/InventoryBack");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/Heavy/IronLotus
        /// </summary>
        public static readonly TextureAsset IronLotus = new TextureAsset("Aequus/Items/Weapons/Melee/Heavy/IronLotus");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Melee/IronLotusProj
        /// </summary>
        public static readonly TextureAsset IronLotusProj = new TextureAsset("Aequus/Projectiles/Melee/IronLotusProj");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/ItemScrap
        /// </summary>
        public static readonly TextureAsset ItemScrap = new TextureAsset("Aequus/Unused/Items/ItemScrap");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Jeweled/JeweledCandelabra
        /// </summary>
        public static readonly TextureAsset JeweledCandelabra = new TextureAsset("Aequus/Tiles/Furniture/Jeweled/JeweledCandelabra");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Jeweled/JeweledCandelabraTile
        /// </summary>
        public static readonly TextureAsset JeweledCandelabraTile = new TextureAsset("Aequus/Tiles/Furniture/Jeweled/JeweledCandelabraTile");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Jeweled/JeweledCandelabraTile_Flame
        /// </summary>
        public static readonly TextureAsset JeweledCandelabraTile_Flame = new TextureAsset("Aequus/Tiles/Furniture/Jeweled/JeweledCandelabraTile_Flame");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Jeweled/JeweledChalice
        /// </summary>
        public static readonly TextureAsset JeweledChalice = new TextureAsset("Aequus/Tiles/Furniture/Jeweled/JeweledChalice");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Jeweled/JeweledChaliceTile
        /// </summary>
        public static readonly TextureAsset JeweledChaliceTile = new TextureAsset("Aequus/Tiles/Furniture/Jeweled/JeweledChaliceTile");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/ArgonJumpshroom/JumpMushroom
        /// </summary>
        public static readonly TextureAsset JumpMushroom = new TextureAsset("Aequus/Content/Biomes/MossBiomes/Tiles/ArgonJumpshroom/JumpMushroom");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/ArgonJumpshroom/JumpMushroomTile
        /// </summary>
        public static readonly TextureAsset JumpMushroomTile = new TextureAsset("Aequus/Content/Biomes/MossBiomes/Tiles/ArgonJumpshroom/JumpMushroomTile");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/BestiaryIcons/JungleEvent
        /// </summary>
        public static readonly TextureAsset JungleEvent = new TextureAsset("Aequus/Assets/UI/BestiaryIcons/JungleEvent");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/SlotMachines/JungleSlotMachine
        /// </summary>
        public static readonly TextureAsset JungleSlotMachine = new TextureAsset("Aequus/Unused/Items/SlotMachines/JungleSlotMachine");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Ranged/Misc/JunkJet
        /// </summary>
        public static readonly TextureAsset JunkJet = new TextureAsset("Aequus/Items/Weapons/Ranged/Misc/JunkJet");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/LegendaryFish/KryptonFish
        /// </summary>
        public static readonly TextureAsset KryptonFish = new TextureAsset("Aequus/Content/Fishing/LegendaryFish/KryptonFish");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/KryptonHealPlant
        /// </summary>
        public static readonly TextureAsset KryptonHealPlant = new TextureAsset("Aequus/Content/Biomes/MossBiomes/Tiles/KryptonHealPlant");
        /// <summary>
        /// Full Path: Aequus/Content/Elites/Misc/KryptonShield
        /// </summary>
        public static readonly TextureAsset KryptonShield = new TextureAsset("Aequus/Content/Elites/Misc/KryptonShield");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Ranged/LaserReticle
        /// </summary>
        public static readonly TextureAsset LaserReticle = new TextureAsset("Aequus/Items/Accessories/Offense/Ranged/LaserReticle");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Ranged/LaserReticle_Glow
        /// </summary>
        public static readonly TextureAsset LaserReticle_Glow = new TextureAsset("Aequus/Items/Accessories/Offense/Ranged/LaserReticle_Glow");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Utility/LavaproofMitten
        /// </summary>
        public static readonly TextureAsset LavaproofMitten = new TextureAsset("Aequus/Items/Accessories/Utility/LavaproofMitten");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Misc/Leecheel
        /// </summary>
        public static readonly TextureAsset Leecheel = new TextureAsset("Aequus/Content/Fishing/Misc/Leecheel");
        /// <summary>
        /// Full Path: Aequus/Items/Tools/GrapplingHooks/LeechHook
        /// </summary>
        public static readonly TextureAsset LeechHook = new TextureAsset("Aequus/Items/Tools/GrapplingHooks/LeechHook");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/GrapplingHooks/LeechHookProj
        /// </summary>
        public static readonly TextureAsset LeechHookProj = new TextureAsset("Aequus/Projectiles/Misc/GrapplingHooks/LeechHookProj");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/GrapplingHooks/LeechHookProj_Chain
        /// </summary>
        public static readonly TextureAsset LeechHookProj_Chain = new TextureAsset("Aequus/Projectiles/Misc/GrapplingHooks/LeechHookProj_Chain");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Bait/LegendberryBait
        /// </summary>
        public static readonly TextureAsset LegendberryBait = new TextureAsset("Aequus/Content/Fishing/Bait/LegendberryBait");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Summon/Minion/LifeFruitStaff
        /// </summary>
        public static readonly TextureAsset LifeFruitStaff = new TextureAsset("Aequus/Items/Weapons/Summon/Minion/LifeFruitStaff");
        /// <summary>
        /// Full Path: Aequus/Items/Vanity/Pets/Light/LightningRod
        /// </summary>
        public static readonly TextureAsset LightningRod = new TextureAsset("Aequus/Items/Vanity/Pets/Light/LightningRod");
        /// <summary>
        /// Full Path: Aequus/Assets/LightRay
        /// </summary>
        public static readonly TextureAsset LightRay = new TextureAsset("Aequus/Assets/LightRay");
        /// <summary>
        /// Full Path: Aequus/Assets/LightRay0
        /// </summary>
        public static readonly TextureAsset LightRay0 = new TextureAsset("Aequus/Assets/LightRay0");
        /// <summary>
        /// Full Path: Aequus/Assets/LightRay1
        /// </summary>
        public static readonly TextureAsset LightRay1 = new TextureAsset("Aequus/Assets/LightRay1");
        /// <summary>
        /// Full Path: Aequus/Assets/LightRay2
        /// </summary>
        public static readonly TextureAsset LightRay2 = new TextureAsset("Aequus/Assets/LightRay2");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/LiquidGun
        /// </summary>
        public static readonly TextureAsset LiquidGun = new TextureAsset("Aequus/Unused/Items/LiquidGun");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/LiquidGunHoney
        /// </summary>
        public static readonly TextureAsset LiquidGunHoney = new TextureAsset("Aequus/Unused/Items/LiquidGunHoney");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/LiquidGunLava
        /// </summary>
        public static readonly TextureAsset LiquidGunLava = new TextureAsset("Aequus/Unused/Items/LiquidGunLava");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/LiquidGunLava_Glow
        /// </summary>
        public static readonly TextureAsset LiquidGunLava_Glow = new TextureAsset("Aequus/Unused/Items/LiquidGunLava_Glow");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/LiquidGunShimmer
        /// </summary>
        public static readonly TextureAsset LiquidGunShimmer = new TextureAsset("Aequus/Unused/Items/LiquidGunShimmer");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/LiquidGunShimmer_Glow
        /// </summary>
        public static readonly TextureAsset LiquidGunShimmer_Glow = new TextureAsset("Aequus/Unused/Items/LiquidGunShimmer_Glow");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/LiquidGunWater
        /// </summary>
        public static readonly TextureAsset LiquidGunWater = new TextureAsset("Aequus/Unused/Items/LiquidGunWater");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/LittleInferno
        /// </summary>
        public static readonly TextureAsset LittleInferno = new TextureAsset("Aequus/Unused/Items/LittleInferno");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Summon/Sentry/LivingWoodSentry
        /// </summary>
        public static readonly TextureAsset LivingWoodSentry = new TextureAsset("Aequus/Items/Weapons/Summon/Sentry/LivingWoodSentry");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/Necro/LocustLarge
        /// </summary>
        public static readonly TextureAsset LocustLarge = new TextureAsset("Aequus/Projectiles/Summon/Necro/LocustLarge");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/Necro/LocustSmall
        /// </summary>
        public static readonly TextureAsset LocustSmall = new TextureAsset("Aequus/Projectiles/Summon/Necro/LocustSmall");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Underworld/Magmabubble
        /// </summary>
        public static readonly TextureAsset Magmabubble = new TextureAsset("Aequus/NPCs/Monsters/Underworld/Magmabubble");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Magmabubble_0
        /// </summary>
        public static readonly TextureAsset Magmabubble_0 = new TextureAsset("Aequus/Assets/Gores/Magmabubble_0");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Magmabubble_1
        /// </summary>
        public static readonly TextureAsset Magmabubble_1 = new TextureAsset("Aequus/Assets/Gores/Magmabubble_1");
        /// <summary>
        /// Full Path: Aequus/Tiles/Banners/Items/MagmabubbleBanner
        /// </summary>
        public static readonly TextureAsset MagmabubbleBanner = new TextureAsset("Aequus/Tiles/Banners/Items/MagmabubbleBanner");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Monster/MagmabubbleBullet
        /// </summary>
        public static readonly TextureAsset MagmabubbleBullet = new TextureAsset("Aequus/Projectiles/Monster/MagmabubbleBullet");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Underworld/MagmabubbleLegs
        /// </summary>
        public static readonly TextureAsset MagmabubbleLegs = new TextureAsset("Aequus/NPCs/Monsters/Underworld/MagmabubbleLegs");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Necro/Malediction
        /// </summary>
        public static readonly TextureAsset Malediction = new TextureAsset("Aequus/Items/Accessories/Offense/Necro/Malediction");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/Misc/Mallet
        /// </summary>
        public static readonly TextureAsset Mallet = new TextureAsset("Aequus/Items/Weapons/Melee/Misc/Mallet");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/Misc/MalletCursor_0
        /// </summary>
        public static readonly TextureAsset MalletCursor_0 = new TextureAsset("Aequus/Items/Weapons/Melee/Misc/MalletCursor_0");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/Misc/MalletCursor_0_Outline
        /// </summary>
        public static readonly TextureAsset MalletCursor_0_Outline = new TextureAsset("Aequus/Items/Weapons/Melee/Misc/MalletCursor_0_Outline");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/Misc/MalletCursor_1
        /// </summary>
        public static readonly TextureAsset MalletCursor_1 = new TextureAsset("Aequus/Items/Weapons/Melee/Misc/MalletCursor_1");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/Misc/MalletCursor_1_Outline
        /// </summary>
        public static readonly TextureAsset MalletCursor_1_Outline = new TextureAsset("Aequus/Items/Weapons/Melee/Misc/MalletCursor_1_Outline");
        /// <summary>
        /// Full Path: Aequus/Items/Potions/Pollen/ManaclePollen
        /// </summary>
        public static readonly TextureAsset ManaclePollen = new TextureAsset("Aequus/Items/Potions/Pollen/ManaclePollen");
        /// <summary>
        /// Full Path: Aequus/Items/Potions/Pollen/ManacleSeeds
        /// </summary>
        public static readonly TextureAsset ManacleSeeds = new TextureAsset("Aequus/Items/Potions/Pollen/ManacleSeeds");
        /// <summary>
        /// Full Path: Aequus/Tiles/Ambience/ManacleTile
        /// </summary>
        public static readonly TextureAsset ManacleTile = new TextureAsset("Aequus/Tiles/Ambience/ManacleTile");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/ManaCursor
        /// </summary>
        public static readonly TextureAsset ManaCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/ManaCursor");
        /// <summary>
        /// Full Path: Aequus/Buffs/ManathirstBuff
        /// </summary>
        public static readonly TextureAsset ManathirstBuff = new TextureAsset("Aequus/Buffs/ManathirstBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Potions/ManathirstPotion
        /// </summary>
        public static readonly TextureAsset ManathirstPotion = new TextureAsset("Aequus/Items/Potions/ManathirstPotion");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/MannequinArmorOverlay
        /// </summary>
        public static readonly TextureAsset MannequinArmorOverlay = new TextureAsset("Aequus/Content/Biomes/MossBiomes/Tiles/MannequinArmorOverlay");
        /// <summary>
        /// Full Path: Aequus/Items/Tools/GrapplingHooks/Meathook
        /// </summary>
        public static readonly TextureAsset Meathook = new TextureAsset("Aequus/Items/Tools/GrapplingHooks/Meathook");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/GrapplingHooks/MeathookProj
        /// </summary>
        public static readonly TextureAsset MeathookProj = new TextureAsset("Aequus/Projectiles/Misc/GrapplingHooks/MeathookProj");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/GrapplingHooks/MeathookProj_Chain
        /// </summary>
        public static readonly TextureAsset MeathookProj_Chain = new TextureAsset("Aequus/Projectiles/Misc/GrapplingHooks/MeathookProj_Chain");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/Mendshroom
        /// </summary>
        public static readonly TextureAsset Mendshroom = new TextureAsset("Aequus/Unused/Items/Mendshroom");
        /// <summary>
        /// Full Path: Aequus/Particles/Dusts/MendshroomDustSpore
        /// </summary>
        public static readonly TextureAsset MendshroomDustSpore = new TextureAsset("Aequus/Particles/Dusts/MendshroomDustSpore");
        /// <summary>
        /// Full Path: Aequus/Items/Potions/Unique/MercerTonic
        /// </summary>
        public static readonly TextureAsset MercerTonic = new TextureAsset("Aequus/Items/Potions/Unique/MercerTonic");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Sky/Meteor
        /// </summary>
        public static readonly TextureAsset Meteor = new TextureAsset("Aequus/NPCs/Monsters/Sky/Meteor");
        /// <summary>
        /// Full Path: Aequus/Buffs/Minion/MindfungusBuff
        /// </summary>
        public static readonly TextureAsset MindfungusBuff = new TextureAsset("Aequus/Buffs/Minion/MindfungusBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Summon/Minion/MindfungusStaff
        /// </summary>
        public static readonly TextureAsset MindfungusStaff = new TextureAsset("Aequus/Items/Weapons/Summon/Minion/MindfungusStaff");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/Pets/MiningPet
        /// </summary>
        public static readonly TextureAsset MiningPet = new TextureAsset("Aequus/Projectiles/Misc/Pets/MiningPet");
        /// <summary>
        /// Full Path: Aequus/Buffs/Pets/MiningPetBuff
        /// </summary>
        public static readonly TextureAsset MiningPetBuff = new TextureAsset("Aequus/Buffs/Pets/MiningPetBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Vanity/Pets/Light/MiningPetSpawner
        /// </summary>
        public static readonly TextureAsset MiningPetSpawner = new TextureAsset("Aequus/Items/Vanity/Pets/Light/MiningPetSpawner");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/Heavy/MirrorsCall
        /// </summary>
        public static readonly TextureAsset MirrorsCall = new TextureAsset("Aequus/Items/Weapons/Melee/Heavy/MirrorsCall");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/Heavy/MirrorsCall_Glow
        /// </summary>
        public static readonly TextureAsset MirrorsCall_Glow = new TextureAsset("Aequus/Items/Weapons/Melee/Heavy/MirrorsCall_Glow");
        /// <summary>
        /// Full Path: Aequus/Items/Potions/Pollen/MistralPollen
        /// </summary>
        public static readonly TextureAsset MistralPollen = new TextureAsset("Aequus/Items/Potions/Pollen/MistralPollen");
        /// <summary>
        /// Full Path: Aequus/Items/Potions/Pollen/MistralSeeds
        /// </summary>
        public static readonly TextureAsset MistralSeeds = new TextureAsset("Aequus/Items/Potions/Pollen/MistralSeeds");
        /// <summary>
        /// Full Path: Aequus/Tiles/Ambience/MistralTile
        /// </summary>
        public static readonly TextureAsset MistralTile = new TextureAsset("Aequus/Tiles/Ambience/MistralTile");
        /// <summary>
        /// Full Path: Aequus/Tiles/Ambience/MistralTile_Pinwheel
        /// </summary>
        public static readonly TextureAsset MistralTile_Pinwheel = new TextureAsset("Aequus/Tiles/Ambience/MistralTile_Pinwheel");
        /// <summary>
        /// Full Path: Aequus/Items/Consumables/Permanent/MoneyTrashcan
        /// </summary>
        public static readonly TextureAsset MoneyTrashcan = new TextureAsset("Aequus/Items/Consumables/Permanent/MoneyTrashcan");
        /// <summary>
        /// Full Path: Aequus/Items/Consumables/Permanent/MoneyTrashcan_UI
        /// </summary>
        public static readonly TextureAsset MoneyTrashcan_UI = new TextureAsset("Aequus/Items/Consumables/Permanent/MoneyTrashcan_UI");
        /// <summary>
        /// Full Path: Aequus/Particles/Dusts/MonoDust
        /// </summary>
        public static readonly TextureAsset MonoDust = new TextureAsset("Aequus/Particles/Dusts/MonoDust");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/Gems/MonoGem
        /// </summary>
        public static readonly TextureAsset MonoGem = new TextureAsset("Aequus/Items/Materials/Gems/MonoGem");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/Gems/MonoGemTile
        /// </summary>
        public static readonly TextureAsset MonoGemTile = new TextureAsset("Aequus/Items/Materials/Gems/MonoGemTile");
        /// <summary>
        /// Full Path: Aequus/Particles/Dusts/MonoSparkleDust
        /// </summary>
        public static readonly TextureAsset MonoSparkleDust = new TextureAsset("Aequus/Particles/Dusts/MonoSparkleDust");
        /// <summary>
        /// Full Path: Aequus/Tiles/Banners/MonsterBanners
        /// </summary>
        public static readonly TextureAsset MonsterBanners = new TextureAsset("Aequus/Tiles/Banners/MonsterBanners");
        /// <summary>
        /// Full Path: Aequus/Items/Potions/Pollen/MoonflowerPollen
        /// </summary>
        public static readonly TextureAsset MoonflowerPollen = new TextureAsset("Aequus/Items/Potions/Pollen/MoonflowerPollen");
        /// <summary>
        /// Full Path: Aequus/Items/Potions/Pollen/MoonflowerSeeds
        /// </summary>
        public static readonly TextureAsset MoonflowerSeeds = new TextureAsset("Aequus/Items/Potions/Pollen/MoonflowerSeeds");
        /// <summary>
        /// Full Path: Aequus/Tiles/Ambience/MoonflowerTile
        /// </summary>
        public static readonly TextureAsset MoonflowerTile = new TextureAsset("Aequus/Tiles/Ambience/MoonflowerTile");
        /// <summary>
        /// Full Path: Aequus/Tiles/Ambience/MoonflowerTileEffect
        /// </summary>
        public static readonly TextureAsset MoonflowerTileEffect = new TextureAsset("Aequus/Tiles/Ambience/MoonflowerTileEffect");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetWizard/MoonlunaHat
        /// </summary>
        public static readonly TextureAsset MoonlunaHat = new TextureAsset("Aequus/Items/Armor/SetWizard/MoonlunaHat");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetWizard/MoonlunaHat_Glow
        /// </summary>
        public static readonly TextureAsset MoonlunaHat_Glow = new TextureAsset("Aequus/Items/Armor/SetWizard/MoonlunaHat_Glow");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetWizard/MoonlunaHat_Head
        /// </summary>
        public static readonly TextureAsset MoonlunaHat_Head = new TextureAsset("Aequus/Items/Armor/SetWizard/MoonlunaHat_Head");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/Misc/MoonlunaHatProj
        /// </summary>
        public static readonly TextureAsset MoonlunaHatProj = new TextureAsset("Aequus/Projectiles/Summon/Misc/MoonlunaHatProj");
        /// <summary>
        /// Full Path: Aequus/Items/Potions/Pollen/MorayPollen
        /// </summary>
        public static readonly TextureAsset MorayPollen = new TextureAsset("Aequus/Items/Potions/Pollen/MorayPollen");
        /// <summary>
        /// Full Path: Aequus/Items/Potions/Pollen/MoraySeeds
        /// </summary>
        public static readonly TextureAsset MoraySeeds = new TextureAsset("Aequus/Items/Potions/Pollen/MoraySeeds");
        /// <summary>
        /// Full Path: Aequus/Items/Potions/Pollen/MoraySeeds_Glow
        /// </summary>
        public static readonly TextureAsset MoraySeeds_Glow = new TextureAsset("Aequus/Items/Potions/Pollen/MoraySeeds_Glow");
        /// <summary>
        /// Full Path: Aequus/Tiles/Ambience/MorayTile
        /// </summary>
        public static readonly TextureAsset MorayTile = new TextureAsset("Aequus/Tiles/Ambience/MorayTile");
        /// <summary>
        /// Full Path: Aequus/Tiles/Ambience/MorayTile_Glow
        /// </summary>
        public static readonly TextureAsset MorayTile_Glow = new TextureAsset("Aequus/Tiles/Ambience/MorayTile_Glow");
        /// <summary>
        /// Full Path: Aequus/Items/Consumables/Permanent/Moro
        /// </summary>
        public static readonly TextureAsset Moro = new TextureAsset("Aequus/Items/Consumables/Permanent/Moro");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/MothmanMask
        /// </summary>
        public static readonly TextureAsset MothmanMask = new TextureAsset("Aequus/Items/Accessories/Offense/MothmanMask");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/MothmanMask_Mask
        /// </summary>
        public static readonly TextureAsset MothmanMask_Mask = new TextureAsset("Aequus/Items/Accessories/Offense/MothmanMask_Mask");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Bait/MysticPopper
        /// </summary>
        public static readonly TextureAsset MysticPopper = new TextureAsset("Aequus/Content/Fishing/Bait/MysticPopper");
        /// <summary>
        /// Full Path: Aequus/Content/Town/SkyMerchantNPC/NameTags/NameTag
        /// </summary>
        public static readonly TextureAsset NameTag = new TextureAsset("Aequus/Content/Town/SkyMerchantNPC/NameTags/NameTag");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/CrownOfBlood/Projectiles/NaniteSpore
        /// </summary>
        public static readonly TextureAsset NaniteSpore = new TextureAsset("Aequus/Items/Accessories/CrownOfBlood/Projectiles/NaniteSpore");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/Unobtainable/Narrizuul
        /// </summary>
        public static readonly TextureAsset Narrizuul = new TextureAsset("Aequus/Unused/Items/Unobtainable/Narrizuul");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/NarrizuulBloom
        /// </summary>
        public static readonly TextureAsset NarrizuulBloom = new TextureAsset("Aequus/Assets/UI/NarrizuulBloom");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Magic/NarrizuulProj
        /// </summary>
        public static readonly TextureAsset NarrizuulProj = new TextureAsset("Aequus/Projectiles/Magic/NarrizuulProj");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Magic/NarrizuulProj_Glow
        /// </summary>
        public static readonly TextureAsset NarrizuulProj_Glow = new TextureAsset("Aequus/Projectiles/Magic/NarrizuulProj_Glow");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Necro/NaturesCruelty
        /// </summary>
        public static readonly TextureAsset NaturesCruelty = new TextureAsset("Aequus/Items/Accessories/Offense/Necro/NaturesCruelty");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetNecromancer/NecromancerHood
        /// </summary>
        public static readonly TextureAsset NecromancerHood = new TextureAsset("Aequus/Items/Armor/SetNecromancer/NecromancerHood");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetNecromancer/NecromancerHood_Head
        /// </summary>
        public static readonly TextureAsset NecromancerHood_Head = new TextureAsset("Aequus/Items/Armor/SetNecromancer/NecromancerHood_Head");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetNecromancer/NecromancerHood_Head_Glow
        /// </summary>
        public static readonly TextureAsset NecromancerHood_Head_Glow = new TextureAsset("Aequus/Items/Armor/SetNecromancer/NecromancerHood_Head_Glow");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetNecromancer/NecromancerRobe
        /// </summary>
        public static readonly TextureAsset NecromancerRobe = new TextureAsset("Aequus/Items/Armor/SetNecromancer/NecromancerRobe");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetNecromancer/NecromancerRobe_Body
        /// </summary>
        public static readonly TextureAsset NecromancerRobe_Body = new TextureAsset("Aequus/Items/Armor/SetNecromancer/NecromancerRobe_Body");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetNecromancer/NecromancerRobe_Legs
        /// </summary>
        public static readonly TextureAsset NecromancerRobe_Legs = new TextureAsset("Aequus/Items/Armor/SetNecromancer/NecromancerRobe_Legs");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetNecromancer/NecromancerRobe_Legs_Glow
        /// </summary>
        public static readonly TextureAsset NecromancerRobe_Legs_Glow = new TextureAsset("Aequus/Items/Armor/SetNecromancer/NecromancerRobe_Legs_Glow");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/NecromancyOwnerBuff
        /// </summary>
        public static readonly TextureAsset NecromancyOwnerBuff = new TextureAsset("Aequus/Buffs/Misc/NecromancyOwnerBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Potions/NecromancyPotion
        /// </summary>
        public static readonly TextureAsset NecromancyPotion = new TextureAsset("Aequus/Items/Potions/NecromancyPotion");
        /// <summary>
        /// Full Path: Aequus/Buffs/NecromancyPotionBuff
        /// </summary>
        public static readonly TextureAsset NecromancyPotionBuff = new TextureAsset("Aequus/Buffs/NecromancyPotionBuff");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/NecromancySelectionCursor
        /// </summary>
        public static readonly TextureAsset NecromancySelectionCursor = new TextureAsset("Aequus/Assets/UI/NecromancySelectionCursor");
        /// <summary>
        /// Full Path: Aequus/Content/Elites/Misc/NeonAttack
        /// </summary>
        public static readonly TextureAsset NeonAttack = new TextureAsset("Aequus/Content/Elites/Misc/NeonAttack");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Equipment/NeonGenesis
        /// </summary>
        public static readonly TextureAsset NeonGenesis = new TextureAsset("Aequus/Content/Fishing/Equipment/NeonGenesis");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/NeonHealPlant
        /// </summary>
        public static readonly TextureAsset NeonHealPlant = new TextureAsset("Aequus/Content/Biomes/MossBiomes/Tiles/NeonHealPlant");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/Heavy/Nettlebane
        /// </summary>
        public static readonly TextureAsset Nettlebane = new TextureAsset("Aequus/Items/Weapons/Melee/Heavy/Nettlebane");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/NettlebaneBuffTier1
        /// </summary>
        public static readonly TextureAsset NettlebaneBuffTier1 = new TextureAsset("Aequus/Buffs/Misc/NettlebaneBuffTier1");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/NettlebaneBuffTier2
        /// </summary>
        public static readonly TextureAsset NettlebaneBuffTier2 = new TextureAsset("Aequus/Buffs/Misc/NettlebaneBuffTier2");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/NettlebaneBuffTier3
        /// </summary>
        public static readonly TextureAsset NettlebaneBuffTier3 = new TextureAsset("Aequus/Buffs/Misc/NettlebaneBuffTier3");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Melee/Swords/NettlebaneProj
        /// </summary>
        public static readonly TextureAsset NettlebaneProj = new TextureAsset("Aequus/Projectiles/Melee/Swords/NettlebaneProj");
        /// <summary>
        /// Full Path: Aequus/Items/Potions/NeutronYogurt
        /// </summary>
        public static readonly TextureAsset NeutronYogurt = new TextureAsset("Aequus/Items/Potions/NeutronYogurt");
        /// <summary>
        /// Full Path: Aequus/Buffs/NeutronYogurtBuff
        /// </summary>
        public static readonly TextureAsset NeutronYogurtBuff = new TextureAsset("Aequus/Buffs/NeutronYogurtBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Magic/Nightfall
        /// </summary>
        public static readonly TextureAsset Nightfall = new TextureAsset("Aequus/Items/Weapons/Magic/Nightfall");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Magic/NightfallGlint
        /// </summary>
        public static readonly TextureAsset NightfallGlint = new TextureAsset("Aequus/Projectiles/Magic/NightfallGlint");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Magic/NightfallProj
        /// </summary>
        public static readonly TextureAsset NightfallProj = new TextureAsset("Aequus/Projectiles/Magic/NightfallProj");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Poles/Nimrod
        /// </summary>
        public static readonly TextureAsset Nimrod = new TextureAsset("Aequus/Content/Fishing/Poles/Nimrod");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Poles/Nimrod_Glow
        /// </summary>
        public static readonly TextureAsset Nimrod_Glow = new TextureAsset("Aequus/Content/Fishing/Poles/Nimrod_Glow");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/Bobbers/NimrodBobber
        /// </summary>
        public static readonly TextureAsset NimrodBobber = new TextureAsset("Aequus/Projectiles/Misc/Bobbers/NimrodBobber");
        /// <summary>
        /// Full Path: Aequus/Assets/None
        /// </summary>
        public static readonly TextureAsset None = new TextureAsset("Aequus/Assets/None");
        /// <summary>
        /// Full Path: Aequus/Buffs/NoonBuff
        /// </summary>
        public static readonly TextureAsset NoonBuff = new TextureAsset("Aequus/Buffs/NoonBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Potions/NoonPotion
        /// </summary>
        public static readonly TextureAsset NoonPotion = new TextureAsset("Aequus/Items/Potions/NoonPotion");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionBed
        /// </summary>
        public static readonly TextureAsset OblivionBed = new TextureAsset("Aequus/Tiles/Furniture/Oblivion/OblivionBed");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionBookcase
        /// </summary>
        public static readonly TextureAsset OblivionBookcase = new TextureAsset("Aequus/Tiles/Furniture/Oblivion/OblivionBookcase");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionCandelabra
        /// </summary>
        public static readonly TextureAsset OblivionCandelabra = new TextureAsset("Aequus/Tiles/Furniture/Oblivion/OblivionCandelabra");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionCandle
        /// </summary>
        public static readonly TextureAsset OblivionCandle = new TextureAsset("Aequus/Tiles/Furniture/Oblivion/OblivionCandle");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionChair
        /// </summary>
        public static readonly TextureAsset OblivionChair = new TextureAsset("Aequus/Tiles/Furniture/Oblivion/OblivionChair");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionChandelier
        /// </summary>
        public static readonly TextureAsset OblivionChandelier = new TextureAsset("Aequus/Tiles/Furniture/Oblivion/OblivionChandelier");
        /// <summary>
        /// Full Path: Aequus/Items/Placeable/Furniture/Oblivion/OblivionChest
        /// </summary>
        public static readonly TextureAsset OblivionChest = new TextureAsset("Aequus/Items/Placeable/Furniture/Oblivion/OblivionChest");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionChestTile
        /// </summary>
        public static readonly TextureAsset OblivionChestTile = new TextureAsset("Aequus/Tiles/Furniture/Oblivion/OblivionChestTile");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionChestTile_Glow
        /// </summary>
        public static readonly TextureAsset OblivionChestTile_Glow = new TextureAsset("Aequus/Tiles/Furniture/Oblivion/OblivionChestTile_Glow");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionChestTile_Highlight
        /// </summary>
        public static readonly TextureAsset OblivionChestTile_Highlight = new TextureAsset("Aequus/Tiles/Furniture/Oblivion/OblivionChestTile_Highlight");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionClock
        /// </summary>
        public static readonly TextureAsset OblivionClock = new TextureAsset("Aequus/Tiles/Furniture/Oblivion/OblivionClock");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionDresser
        /// </summary>
        public static readonly TextureAsset OblivionDresser = new TextureAsset("Aequus/Tiles/Furniture/Oblivion/OblivionDresser");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionLamp
        /// </summary>
        public static readonly TextureAsset OblivionLamp = new TextureAsset("Aequus/Tiles/Furniture/Oblivion/OblivionLamp");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionLantern
        /// </summary>
        public static readonly TextureAsset OblivionLantern = new TextureAsset("Aequus/Tiles/Furniture/Oblivion/OblivionLantern");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionPiano
        /// </summary>
        public static readonly TextureAsset OblivionPiano = new TextureAsset("Aequus/Tiles/Furniture/Oblivion/OblivionPiano");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionSink
        /// </summary>
        public static readonly TextureAsset OblivionSink = new TextureAsset("Aequus/Tiles/Furniture/Oblivion/OblivionSink");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionSofa
        /// </summary>
        public static readonly TextureAsset OblivionSofa = new TextureAsset("Aequus/Tiles/Furniture/Oblivion/OblivionSofa");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionTable
        /// </summary>
        public static readonly TextureAsset OblivionTable = new TextureAsset("Aequus/Tiles/Furniture/Oblivion/OblivionTable");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Oblivion/OblivionWorkbench
        /// </summary>
        public static readonly TextureAsset OblivionWorkbench = new TextureAsset("Aequus/Tiles/Furniture/Oblivion/OblivionWorkbench");
        /// <summary>
        /// Full Path: Aequus/Content/Critters/Oblivision
        /// </summary>
        public static readonly TextureAsset Oblivision = new TextureAsset("Aequus/Content/Critters/Oblivision");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Occultist_0
        /// </summary>
        public static readonly TextureAsset Occultist_0 = new TextureAsset("Aequus/Assets/Gores/Occultist_0");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Occultist_1
        /// </summary>
        public static readonly TextureAsset Occultist_1 = new TextureAsset("Aequus/Assets/Gores/Occultist_1");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Occultist_2
        /// </summary>
        public static readonly TextureAsset Occultist_2 = new TextureAsset("Aequus/Assets/Gores/Occultist_2");
        /// <summary>
        /// Full Path: Aequus/Content/Town/OccultistNPC/Occultist_Glow
        /// </summary>
        public static readonly TextureAsset Occultist_Glow = new TextureAsset("Aequus/Content/Town/OccultistNPC/Occultist_Glow");
        /// <summary>
        /// Full Path: Aequus/Content/Town/OccultistNPC/Occultist_Head
        /// </summary>
        public static readonly TextureAsset Occultist_Head = new TextureAsset("Aequus/Content/Town/OccultistNPC/Occultist_Head");
        /// <summary>
        /// Full Path: Aequus/Content/Town/OccultistNPC/Occultist
        /// </summary>
        public static readonly TextureAsset Occultist_OccultistNPC = new TextureAsset("Aequus/Content/Town/OccultistNPC/Occultist");
        /// <summary>
        /// Full Path: Aequus/Content/Town/OccultistNPC/Shimmer/Occultist
        /// </summary>
        public static readonly TextureAsset Occultist_Shimmer = new TextureAsset("Aequus/Content/Town/OccultistNPC/Shimmer/Occultist");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Candles/OccultistCandle
        /// </summary>
        public static readonly TextureAsset OccultistCandle = new TextureAsset("Aequus/Items/Weapons/Necromancy/Candles/OccultistCandle");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Candles/OccultistCandle_Flame
        /// </summary>
        public static readonly TextureAsset OccultistCandle_Flame = new TextureAsset("Aequus/Items/Weapons/Necromancy/Candles/OccultistCandle_Flame");
        /// <summary>
        /// Full Path: Aequus/Content/Town/OccultistNPC/OccultistHostile
        /// </summary>
        public static readonly TextureAsset OccultistHostile = new TextureAsset("Aequus/Content/Town/OccultistNPC/OccultistHostile");
        /// <summary>
        /// Full Path: Aequus/Content/Town/OccultistNPC/OccultistHostile_Head
        /// </summary>
        public static readonly TextureAsset OccultistHostile_Head = new TextureAsset("Aequus/Content/Town/OccultistNPC/OccultistHostile_Head");
        /// <summary>
        /// Full Path: Aequus/Content/Town/OccultistNPC/OccultistHostile_Sit
        /// </summary>
        public static readonly TextureAsset OccultistHostile_Sit = new TextureAsset("Aequus/Content/Town/OccultistNPC/OccultistHostile_Sit");
        /// <summary>
        /// Full Path: Aequus/Content/Town/OccultistNPC/OccultistHostile_Sit_Glow
        /// </summary>
        public static readonly TextureAsset OccultistHostile_Sit_Glow = new TextureAsset("Aequus/Content/Town/OccultistNPC/OccultistHostile_Sit_Glow");
        /// <summary>
        /// Full Path: Aequus/Content/Town/OccultistNPC/OccultistRune
        /// </summary>
        public static readonly TextureAsset OccultistRune = new TextureAsset("Aequus/Content/Town/OccultistNPC/OccultistRune");
        /// <summary>
        /// Full Path: Aequus/Content/Town/OccultistNPC/OccultistSleep
        /// </summary>
        public static readonly TextureAsset OccultistSleep = new TextureAsset("Aequus/Content/Town/OccultistNPC/OccultistSleep");
        /// <summary>
        /// Full Path: Aequus/Content/Town/OccultistNPC/OccultistSleep_Glow
        /// </summary>
        public static readonly TextureAsset OccultistSleep_Glow = new TextureAsset("Aequus/Content/Town/OccultistNPC/OccultistSleep_Glow");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/SlotMachines/OceanSlotMachine
        /// </summary>
        public static readonly TextureAsset OceanSlotMachine = new TextureAsset("Aequus/Unused/Items/SlotMachines/OceanSlotMachine");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Paintings/Items/OliverPainting
        /// </summary>
        public static readonly TextureAsset OliverPainting = new TextureAsset("Aequus/Tiles/Furniture/Paintings/Items/OliverPainting");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/BossChecklist/OmegaStarite
        /// </summary>
        public static readonly TextureAsset OmegaStarite_BossChecklist = new TextureAsset("Aequus/Assets/UI/BossChecklist/OmegaStarite");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/OmegaStarite/OmegaStarite_Head_Boss
        /// </summary>
        public static readonly TextureAsset OmegaStarite_Head_Boss = new TextureAsset("Aequus/Content/Boss/OmegaStarite/OmegaStarite_Head_Boss");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/OmegaStarite/OmegaStarite
        /// </summary>
        public static readonly TextureAsset OmegaStarite_OmegaStarite = new TextureAsset("Aequus/Content/Boss/OmegaStarite/OmegaStarite");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/OmegaStarite/Rewards/OmegaStariteBag
        /// </summary>
        public static readonly TextureAsset OmegaStariteBag = new TextureAsset("Aequus/Content/Boss/OmegaStarite/Rewards/OmegaStariteBag");
        /// <summary>
        /// Full Path: Aequus/Buffs/Pets/OmegaStariteBuff
        /// </summary>
        public static readonly TextureAsset OmegaStariteBuff = new TextureAsset("Aequus/Buffs/Pets/OmegaStariteBuff");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/OmegaStarite/Projectiles/OmegaStariteBullet
        /// </summary>
        public static readonly TextureAsset OmegaStariteBullet = new TextureAsset("Aequus/Content/Boss/OmegaStarite/Projectiles/OmegaStariteBullet");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/OmegaStarite/Projectiles/OmegaStariteDeathray
        /// </summary>
        public static readonly TextureAsset OmegaStariteDeathray = new TextureAsset("Aequus/Content/Boss/OmegaStarite/Projectiles/OmegaStariteDeathray");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/OmegaStarite/Rewards/OmegaStariteMask
        /// </summary>
        public static readonly TextureAsset OmegaStariteMask = new TextureAsset("Aequus/Content/Boss/OmegaStarite/Rewards/OmegaStariteMask");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/OmegaStarite/Rewards/OmegaStariteMask_Head
        /// </summary>
        public static readonly TextureAsset OmegaStariteMask_Head = new TextureAsset("Aequus/Content/Boss/OmegaStarite/Rewards/OmegaStariteMask_Head");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Paintings/Items/OmegaStaritePainting
        /// </summary>
        public static readonly TextureAsset OmegaStaritePainting = new TextureAsset("Aequus/Tiles/Furniture/Paintings/Items/OmegaStaritePainting");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/Pets/OmegaStaritePet
        /// </summary>
        public static readonly TextureAsset OmegaStaritePet = new TextureAsset("Aequus/Projectiles/Misc/Pets/OmegaStaritePet");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/OmegaStarite/Projectiles/OmegaStariteProj
        /// </summary>
        public static readonly TextureAsset OmegaStariteProj = new TextureAsset("Aequus/Content/Boss/OmegaStarite/Projectiles/OmegaStariteProj");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/OmegaStarite/Rewards/OmegaStariteRelic
        /// </summary>
        public static readonly TextureAsset OmegaStariteRelic = new TextureAsset("Aequus/Content/Boss/OmegaStarite/Rewards/OmegaStariteRelic");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/OmegaStarite/Rewards/OmegaStariteTrophy
        /// </summary>
        public static readonly TextureAsset OmegaStariteTrophy = new TextureAsset("Aequus/Content/Boss/OmegaStarite/Rewards/OmegaStariteTrophy");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Bait/Omnibait
        /// </summary>
        public static readonly TextureAsset Omnibait = new TextureAsset("Aequus/Content/Fishing/Bait/Omnibait");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/Gems/OmniGem
        /// </summary>
        public static readonly TextureAsset OmniGem = new TextureAsset("Aequus/Items/Materials/Gems/OmniGem");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/Gems/OmniGem_Mask
        /// </summary>
        public static readonly TextureAsset OmniGem_Mask = new TextureAsset("Aequus/Items/Materials/Gems/OmniGem_Mask");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/Gems/OmniGemTile
        /// </summary>
        public static readonly TextureAsset OmniGemTile = new TextureAsset("Aequus/Items/Materials/Gems/OmniGemTile");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/Gems/OmniGemTile_Mask
        /// </summary>
        public static readonly TextureAsset OmniGemTile_Mask = new TextureAsset("Aequus/Items/Materials/Gems/OmniGemTile_Mask");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Paint/OmniPaint
        /// </summary>
        public static readonly TextureAsset OmniPaint = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Paint/OmniPaint");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Paint/OmniPaintUI
        /// </summary>
        public static readonly TextureAsset OmniPaintUI = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Paint/OmniPaintUI");
        /// <summary>
        /// Full Path: Aequus/Content/Town/ExporterNPC/RerollSystem/OpenButton
        /// </summary>
        public static readonly TextureAsset OpenButton = new TextureAsset("Aequus/Content/Town/ExporterNPC/RerollSystem/OpenButton");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/Energies/OrganicEnergy
        /// </summary>
        public static readonly TextureAsset OrganicEnergy = new TextureAsset("Aequus/Items/Materials/Energies/OrganicEnergy");
        /// <summary>
        /// Full Path: Aequus/Assets/Effects/Textures/OrganicEnergyGradient
        /// </summary>
        public static readonly TextureAsset OrganicEnergyGradient = new TextureAsset("Aequus/Assets/Effects/Textures/OrganicEnergyGradient");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Paintings/Items/OriginPainting
        /// </summary>
        public static readonly TextureAsset OriginPainting = new TextureAsset("Aequus/Tiles/Furniture/Paintings/Items/OriginPainting");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Scepters/Osiris
        /// </summary>
        public static readonly TextureAsset Osiris = new TextureAsset("Aequus/Items/Weapons/Necromancy/Scepters/Osiris");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Scepters/Osiris_Glow
        /// </summary>
        public static readonly TextureAsset Osiris_Glow = new TextureAsset("Aequus/Items/Weapons/Necromancy/Scepters/Osiris_Glow");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Dyes/OutlineDye
        /// </summary>
        public static readonly TextureAsset OutlineDye = new TextureAsset("Aequus/Items/Misc/Dyes/OutlineDye");
        /// <summary>
        /// Full Path: Aequus/Items/Consumables/PalePufferfish
        /// </summary>
        public static readonly TextureAsset PalePufferfish = new TextureAsset("Aequus/Items/Consumables/PalePufferfish");
        /// <summary>
        /// Full Path: Aequus/Buffs/Buildings/PaletteBountyBuff
        /// </summary>
        public static readonly TextureAsset PaletteBountyBuff = new TextureAsset("Aequus/Buffs/Buildings/PaletteBountyBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Necro/PandorasBox
        /// </summary>
        public static readonly TextureAsset PandorasBox = new TextureAsset("Aequus/Items/Accessories/Offense/Necro/PandorasBox");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Quest/Bounties/Panel
        /// </summary>
        public static readonly TextureAsset Panel_Bounties = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Quest/Bounties/Panel");
        /// <summary>
        /// Full Path: Aequus/Content/Town/ExporterNPC/RerollSystem/Panel
        /// </summary>
        public static readonly TextureAsset Panel_RerollSystem = new TextureAsset("Aequus/Content/Town/ExporterNPC/RerollSystem/Panel");
        /// <summary>
        /// Full Path: Aequus/Assets/Particles/Particle
        /// </summary>
        public static readonly TextureAsset Particle = new TextureAsset("Aequus/Assets/Particles/Particle");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/PearlShards/PearlShardBlack
        /// </summary>
        public static readonly TextureAsset PearlShardBlack = new TextureAsset("Aequus/Items/Materials/PearlShards/PearlShardBlack");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/PearlShards/PearlShardBlack_Dropped
        /// </summary>
        public static readonly TextureAsset PearlShardBlack_Dropped = new TextureAsset("Aequus/Items/Materials/PearlShards/PearlShardBlack_Dropped");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/PearlShards/PearlShardPink
        /// </summary>
        public static readonly TextureAsset PearlShardPink = new TextureAsset("Aequus/Items/Materials/PearlShards/PearlShardPink");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/PearlShards/PearlShardPink_Dropped
        /// </summary>
        public static readonly TextureAsset PearlShardPink_Dropped = new TextureAsset("Aequus/Items/Materials/PearlShards/PearlShardPink_Dropped");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/PearlShards/PearlShardWhite
        /// </summary>
        public static readonly TextureAsset PearlShardWhite = new TextureAsset("Aequus/Items/Materials/PearlShards/PearlShardWhite");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/PearlShards/PearlShardWhite_Dropped
        /// </summary>
        public static readonly TextureAsset PearlShardWhite_Dropped = new TextureAsset("Aequus/Items/Materials/PearlShards/PearlShardWhite_Dropped");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/PearlShards/PearlsTileBlack
        /// </summary>
        public static readonly TextureAsset PearlsTileBlack = new TextureAsset("Aequus/Items/Materials/PearlShards/PearlsTileBlack");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/PearlShards/PearlsTileHypnotic
        /// </summary>
        public static readonly TextureAsset PearlsTileHypnotic = new TextureAsset("Aequus/Items/Materials/PearlShards/PearlsTileHypnotic");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/PearlShards/PearlsTilePink
        /// </summary>
        public static readonly TextureAsset PearlsTilePink = new TextureAsset("Aequus/Items/Materials/PearlShards/PearlsTilePink");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/PearlShards/PearlsTileWhite
        /// </summary>
        public static readonly TextureAsset PearlsTileWhite = new TextureAsset("Aequus/Items/Materials/PearlShards/PearlsTileWhite");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Magic/PentalScythe
        /// </summary>
        public static readonly TextureAsset PentalScythe = new TextureAsset("Aequus/Items/Weapons/Magic/PentalScythe");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Magic/PentalScythe_Glow
        /// </summary>
        public static readonly TextureAsset PentalScythe_Glow = new TextureAsset("Aequus/Items/Weapons/Magic/PentalScythe_Glow");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Magic/PentalScytheProj
        /// </summary>
        public static readonly TextureAsset PentalScytheProj = new TextureAsset("Aequus/Projectiles/Magic/PentalScytheProj");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Photobook/PeonyPhotobook
        /// </summary>
        public static readonly TextureAsset PeonyPhotobook = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Photobook/PeonyPhotobook");
        /// <summary>
        /// Full Path: Aequus/Items/Vanity/Pets/Light/PersonalDronePack
        /// </summary>
        public static readonly TextureAsset PersonalDronePack = new TextureAsset("Aequus/Items/Vanity/Pets/Light/PersonalDronePack");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Necro/PharaohsCurse
        /// </summary>
        public static readonly TextureAsset PharaohsCurse = new TextureAsset("Aequus/Items/Accessories/Offense/Necro/PharaohsCurse");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/Thrown/PhaseDisc
        /// </summary>
        public static readonly TextureAsset PhaseDisc = new TextureAsset("Aequus/Items/Weapons/Melee/Thrown/PhaseDisc");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Melee/PhaseDiscProj
        /// </summary>
        public static readonly TextureAsset PhaseDiscProj = new TextureAsset("Aequus/Projectiles/Melee/PhaseDiscProj");
        /// <summary>
        /// Full Path: Aequus/Items/Tools/PhaseMirror
        /// </summary>
        public static readonly TextureAsset PhaseMirror = new TextureAsset("Aequus/Items/Tools/PhaseMirror");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Debuff/PhoenixRing
        /// </summary>
        public static readonly TextureAsset PhoenixRing = new TextureAsset("Aequus/Items/Accessories/Debuff/PhoenixRing");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Debuff/PhoenixRing_HandsOn
        /// </summary>
        public static readonly TextureAsset PhoenixRing_HandsOn = new TextureAsset("Aequus/Items/Accessories/Debuff/PhoenixRing_HandsOn");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Photobook/UI/Photobook
        /// </summary>
        public static readonly TextureAsset Photobook = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Photobook/UI/Photobook");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Photobook/UI/PhotobookButtonLeft
        /// </summary>
        public static readonly TextureAsset PhotobookButtonLeft = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Photobook/UI/PhotobookButtonLeft");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Photobook/UI/PhotobookButtonRight
        /// </summary>
        public static readonly TextureAsset PhotobookButtonRight = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Photobook/UI/PhotobookButtonRight");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Photobook/PhotobookItem
        /// </summary>
        public static readonly TextureAsset PhotobookItem = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Photobook/PhotobookItem");
        /// <summary>
        /// Full Path: Aequus/Content/Town/PhysicistNPC/Shimmer/Physicist_Glow
        /// </summary>
        public static readonly TextureAsset Physicist_Glow = new TextureAsset("Aequus/Content/Town/PhysicistNPC/Shimmer/Physicist_Glow");
        /// <summary>
        /// Full Path: Aequus/Content/Town/PhysicistNPC/Physicist_Head
        /// </summary>
        public static readonly TextureAsset Physicist_Head_PhysicistNPC = new TextureAsset("Aequus/Content/Town/PhysicistNPC/Physicist_Head");
        /// <summary>
        /// Full Path: Aequus/Content/Town/PhysicistNPC/Shimmer/Physicist_Head
        /// </summary>
        public static readonly TextureAsset Physicist_Head_Shimmer = new TextureAsset("Aequus/Content/Town/PhysicistNPC/Shimmer/Physicist_Head");
        /// <summary>
        /// Full Path: Aequus/Content/Town/PhysicistNPC/Physicist
        /// </summary>
        public static readonly TextureAsset Physicist_PhysicistNPC = new TextureAsset("Aequus/Content/Town/PhysicistNPC/Physicist");
        /// <summary>
        /// Full Path: Aequus/Content/Town/PhysicistNPC/Shimmer/Physicist
        /// </summary>
        public static readonly TextureAsset Physicist_Shimmer = new TextureAsset("Aequus/Content/Town/PhysicistNPC/Shimmer/Physicist");
        /// <summary>
        /// Full Path: Aequus/Content/Town/PhysicistNPC/PhysicistPet_Glow
        /// </summary>
        public static readonly TextureAsset PhysicistPet_Glow_PhysicistNPC = new TextureAsset("Aequus/Content/Town/PhysicistNPC/PhysicistPet_Glow");
        /// <summary>
        /// Full Path: Aequus/Content/Town/PhysicistNPC/Shimmer/PhysicistPet_Glow
        /// </summary>
        public static readonly TextureAsset PhysicistPet_Glow_Shimmer = new TextureAsset("Aequus/Content/Town/PhysicistNPC/Shimmer/PhysicistPet_Glow");
        /// <summary>
        /// Full Path: Aequus/Content/Town/PhysicistNPC/PhysicistPet
        /// </summary>
        public static readonly TextureAsset PhysicistPet_PhysicistNPC = new TextureAsset("Aequus/Content/Town/PhysicistNPC/PhysicistPet");
        /// <summary>
        /// Full Path: Aequus/Content/Town/PhysicistNPC/Shimmer/PhysicistPet
        /// </summary>
        public static readonly TextureAsset PhysicistPet_Shimmer = new TextureAsset("Aequus/Content/Town/PhysicistNPC/Shimmer/PhysicistPet");
        /// <summary>
        /// Full Path: Aequus/Tiles/Blocks/PhysicsBlock
        /// </summary>
        public static readonly TextureAsset PhysicsBlock = new TextureAsset("Aequus/Tiles/Blocks/PhysicsBlock");
        /// <summary>
        /// Full Path: Aequus/Tiles/Blocks/PhysicsBlockTile
        /// </summary>
        public static readonly TextureAsset PhysicsBlockTile = new TextureAsset("Aequus/Tiles/Blocks/PhysicsBlockTile");
        /// <summary>
        /// Full Path: Aequus/Items/Tools/PhysicsGun
        /// </summary>
        public static readonly TextureAsset PhysicsGun = new TextureAsset("Aequus/Items/Tools/PhysicsGun");
        /// <summary>
        /// Full Path: Aequus/Items/Tools/PhysicsGun_Glow
        /// </summary>
        public static readonly TextureAsset PhysicsGun_Glow = new TextureAsset("Aequus/Items/Tools/PhysicsGun_Glow");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/DebugItems/PhysicsGun2
        /// </summary>
        public static readonly TextureAsset PhysicsGun2 = new TextureAsset("Aequus/Unused/Items/DebugItems/PhysicsGun2");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/DebugItems/PhysicsGun2_Glow
        /// </summary>
        public static readonly TextureAsset PhysicsGun2_Glow = new TextureAsset("Aequus/Unused/Items/DebugItems/PhysicsGun2_Glow");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/PhysicsGunProj
        /// </summary>
        public static readonly TextureAsset PhysicsGunProj = new TextureAsset("Aequus/Projectiles/Misc/PhysicsGunProj");
        /// <summary>
        /// Full Path: Aequus/Buffs/Debuffs/PickBreak
        /// </summary>
        public static readonly TextureAsset PickBreak = new TextureAsset("Aequus/Buffs/Debuffs/PickBreak");
        /// <summary>
        /// Full Path: Aequus/Buffs/Minion/PiranhaPlantBuff
        /// </summary>
        public static readonly TextureAsset PiranhaPlantBuff = new TextureAsset("Aequus/Buffs/Minion/PiranhaPlantBuff");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/PiranhaPlantFireball
        /// </summary>
        public static readonly TextureAsset PiranhaPlantFireball = new TextureAsset("Aequus/Projectiles/Summon/PiranhaPlantFireball");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/PiranhaPlantMinion
        /// </summary>
        public static readonly TextureAsset PiranhaPlantMinion = new TextureAsset("Aequus/Projectiles/Summon/PiranhaPlantMinion");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Summon/Minion/PiranhaPot
        /// </summary>
        public static readonly TextureAsset PiranhaPot = new TextureAsset("Aequus/Items/Weapons/Summon/Minion/PiranhaPot");
        /// <summary>
        /// Full Path: Aequus/Buffs/Buildings/PirateBountyBuff
        /// </summary>
        public static readonly TextureAsset PirateBountyBuff = new TextureAsset("Aequus/Buffs/Buildings/PirateBountyBuff");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Quest/Bounties/Icons/PirateShipBounty
        /// </summary>
        public static readonly TextureAsset PirateShipBounty = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Quest/Bounties/Icons/PirateShipBounty");
        /// <summary>
        /// Full Path: Aequus/Assets/Pixel
        /// </summary>
        public static readonly TextureAsset Pixel = new TextureAsset("Aequus/Assets/Pixel");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Rewards/PixelCamera
        /// </summary>
        public static readonly TextureAsset PixelCamera = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Rewards/PixelCamera");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Rewards/PixelCameraClip
        /// </summary>
        public static readonly TextureAsset PixelCameraClip = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Rewards/PixelCameraClip");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/PixelCameraProj
        /// </summary>
        public static readonly TextureAsset PixelCameraProj = new TextureAsset("Aequus/Projectiles/Misc/PixelCameraProj");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/PixelPaintingTile
        /// </summary>
        public static readonly TextureAsset PixelPaintingTile = new TextureAsset("Aequus/Tiles/Misc/PixelPaintingTile");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Candles/PixieCandle
        /// </summary>
        public static readonly TextureAsset PixieCandle = new TextureAsset("Aequus/Items/Weapons/Necromancy/Candles/PixieCandle");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Candles/PixieCandle_Flame
        /// </summary>
        public static readonly TextureAsset PixieCandle_Flame = new TextureAsset("Aequus/Items/Weapons/Necromancy/Candles/PixieCandle_Flame");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Misc/PlasticBottle
        /// </summary>
        public static readonly TextureAsset PlasticBottle = new TextureAsset("Aequus/Content/Fishing/Misc/PlasticBottle");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Quest/Bounties/Icons/PondBridgeBounty
        /// </summary>
        public static readonly TextureAsset PondBridgeBounty = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Quest/Bounties/Icons/PondBridgeBounty");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/PossessedShard
        /// </summary>
        public static readonly TextureAsset PossessedShard = new TextureAsset("Aequus/Items/Materials/PossessedShard");
        /// <summary>
        /// Full Path: Aequus/Content/CrossMod/SplitSupport/Photography/PosterBloodMimic
        /// </summary>
        public static readonly TextureAsset PosterBloodMimic = new TextureAsset("Aequus/Content/CrossMod/SplitSupport/Photography/PosterBloodMimic");
        /// <summary>
        /// Full Path: Aequus/Content/CrossMod/SplitSupport/Photography/PosterBreadOfCthulhu
        /// </summary>
        public static readonly TextureAsset PosterBreadOfCthulhu = new TextureAsset("Aequus/Content/CrossMod/SplitSupport/Photography/PosterBreadOfCthulhu");
        /// <summary>
        /// Full Path: Aequus/Content/CrossMod/SplitSupport/Photography/PosterHeckto
        /// </summary>
        public static readonly TextureAsset PosterHeckto = new TextureAsset("Aequus/Content/CrossMod/SplitSupport/Photography/PosterHeckto");
        /// <summary>
        /// Full Path: Aequus/Content/CrossMod/SplitSupport/Photography/PosterOblivision
        /// </summary>
        public static readonly TextureAsset PosterOblivision = new TextureAsset("Aequus/Content/CrossMod/SplitSupport/Photography/PosterOblivision");
        /// <summary>
        /// Full Path: Aequus/Content/CrossMod/SplitSupport/Photography/PosterSkyMerchant
        /// </summary>
        public static readonly TextureAsset PosterSkyMerchant = new TextureAsset("Aequus/Content/CrossMod/SplitSupport/Photography/PosterSkyMerchant");
        /// <summary>
        /// Full Path: Aequus/Content/CrossMod/SplitSupport/Photography/PosterUltraStarite
        /// </summary>
        public static readonly TextureAsset PosterUltraStarite = new TextureAsset("Aequus/Content/CrossMod/SplitSupport/Photography/PosterUltraStarite");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Misc/PotionCanteen
        /// </summary>
        public static readonly TextureAsset PotionCanteen = new TextureAsset("Aequus/Items/Accessories/Misc/PotionCanteen");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Misc/PotionCanteen_Mask
        /// </summary>
        public static readonly TextureAsset PotionCanteen_Mask = new TextureAsset("Aequus/Items/Accessories/Misc/PotionCanteen_Mask");
        /// <summary>
        /// Full Path: Aequus/Items/Potions/Unique/PotionOfResurrection
        /// </summary>
        public static readonly TextureAsset PotionOfResurrection = new TextureAsset("Aequus/Items/Potions/Unique/PotionOfResurrection");
        /// <summary>
        /// Full Path: Aequus/Content/CrossMod/SplitSupport/Photography/PrintsTile
        /// </summary>
        public static readonly TextureAsset PrintsTile = new TextureAsset("Aequus/Content/CrossMod/SplitSupport/Photography/PrintsTile");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/ProtectiveProbe
        /// </summary>
        public static readonly TextureAsset ProtectiveProbe = new TextureAsset("Aequus/Projectiles/Misc/ProtectiveProbe");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/ProtectiveProbe_Glow
        /// </summary>
        public static readonly TextureAsset ProtectiveProbe_Glow = new TextureAsset("Aequus/Projectiles/Misc/ProtectiveProbe_Glow");
        /// <summary>
        /// Full Path: Aequus/Items/Tools/Pumpinator
        /// </summary>
        public static readonly TextureAsset Pumpinator = new TextureAsset("Aequus/Items/Tools/Pumpinator");
        /// <summary>
        /// Full Path: Aequus/Items/Tools/Pumpinator_Glow
        /// </summary>
        public static readonly TextureAsset Pumpinator_Glow = new TextureAsset("Aequus/Items/Tools/Pumpinator_Glow");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/PumpinatorProj
        /// </summary>
        public static readonly TextureAsset PumpinatorProj = new TextureAsset("Aequus/Projectiles/Misc/PumpinatorProj");
        /// <summary>
        /// Full Path: Aequus/Items/Vanity/PumpkingCloak
        /// </summary>
        public static readonly TextureAsset PumpkingCloak = new TextureAsset("Aequus/Items/Vanity/PumpkingCloak");
        /// <summary>
        /// Full Path: Aequus/Items/Vanity/PumpkingCloak_Back
        /// </summary>
        public static readonly TextureAsset PumpkingCloak_Back = new TextureAsset("Aequus/Items/Vanity/PumpkingCloak_Back");
        /// <summary>
        /// Full Path: Aequus/Items/Vanity/PumpkingCloak_Front
        /// </summary>
        public static readonly TextureAsset PumpkingCloak_Front = new TextureAsset("Aequus/Items/Vanity/PumpkingCloak_Front");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/PumpkingCursor
        /// </summary>
        public static readonly TextureAsset PumpkingCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/PumpkingCursor");
        /// <summary>
        /// Full Path: Aequus/Content/DronePylons/Items/PylonCleanserItem
        /// </summary>
        public static readonly TextureAsset PylonCleanserItem = new TextureAsset("Aequus/Content/DronePylons/Items/PylonCleanserItem");
        /// <summary>
        /// Full Path: Aequus/Content/DronePylons/Items/PylonGunnerItem
        /// </summary>
        public static readonly TextureAsset PylonGunnerItem = new TextureAsset("Aequus/Content/DronePylons/Items/PylonGunnerItem");
        /// <summary>
        /// Full Path: Aequus/Content/DronePylons/Items/PylonHealerItem
        /// </summary>
        public static readonly TextureAsset PylonHealerItem = new TextureAsset("Aequus/Content/DronePylons/Items/PylonHealerItem");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Misc/RabbitsFoot
        /// </summary>
        public static readonly TextureAsset RabbitsFoot = new TextureAsset("Aequus/Items/Accessories/Misc/RabbitsFoot");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/LegendaryFish/RadonFish
        /// </summary>
        public static readonly TextureAsset RadonFish = new TextureAsset("Aequus/Content/Fishing/LegendaryFish/RadonFish");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Equipment/RadonFishingBobber
        /// </summary>
        public static readonly TextureAsset RadonFishingBobber = new TextureAsset("Aequus/Content/Fishing/Equipment/RadonFishingBobber");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Equipment/RadonFishingBobberBuff
        /// </summary>
        public static readonly TextureAsset RadonFishingBobberBuff = new TextureAsset("Aequus/Content/Fishing/Equipment/RadonFishingBobberBuff");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Equipment/RadonFishingBobberProj
        /// </summary>
        public static readonly TextureAsset RadonFishingBobberProj = new TextureAsset("Aequus/Content/Fishing/Equipment/RadonFishingBobberProj");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/RadonBiome/Tiles/RadonMoss
        /// </summary>
        public static readonly TextureAsset RadonMoss = new TextureAsset("Aequus/Content/Biomes/RadonBiome/Tiles/RadonMoss");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/RadonBiome/Tiles/RadonMossBrickTile
        /// </summary>
        public static readonly TextureAsset RadonMossBrickTile = new TextureAsset("Aequus/Content/Biomes/RadonBiome/Tiles/RadonMossBrickTile");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/RadonBiome/Tiles/RadonMossGrass
        /// </summary>
        public static readonly TextureAsset RadonMossGrass = new TextureAsset("Aequus/Content/Biomes/RadonBiome/Tiles/RadonMossGrass");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/RadonBiome/Tiles/RadonMossTile
        /// </summary>
        public static readonly TextureAsset RadonMossTile = new TextureAsset("Aequus/Content/Biomes/RadonBiome/Tiles/RadonMossTile");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/RadonBiome/Tiles/RadonPlantTile
        /// </summary>
        public static readonly TextureAsset RadonPlantTile = new TextureAsset("Aequus/Content/Biomes/RadonBiome/Tiles/RadonPlantTile");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Dyes/RainbowOutlineDye
        /// </summary>
        public static readonly TextureAsset RainbowOutlineDye = new TextureAsset("Aequus/Items/Misc/Dyes/RainbowOutlineDye");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Equipment/Ramishroom
        /// </summary>
        public static readonly TextureAsset Ramishroom = new TextureAsset("Aequus/Content/Fishing/Equipment/Ramishroom");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/Bobbers/RamishroomBobber
        /// </summary>
        public static readonly TextureAsset RamishroomBobber = new TextureAsset("Aequus/Projectiles/Misc/Bobbers/RamishroomBobber");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Ranged/Raygun
        /// </summary>
        public static readonly TextureAsset Raygun = new TextureAsset("Aequus/Items/Weapons/Ranged/Raygun");
        /// <summary>
        /// Full Path: Aequus/Tiles/CraftingStations/RecyclingMachine
        /// </summary>
        public static readonly TextureAsset RecyclingMachine = new TextureAsset("Aequus/Tiles/CraftingStations/RecyclingMachine");
        /// <summary>
        /// Full Path: Aequus/Tiles/CraftingStations/RecyclingMachineTile
        /// </summary>
        public static readonly TextureAsset RecyclingMachineTile = new TextureAsset("Aequus/Tiles/CraftingStations/RecyclingMachineTile");
        /// <summary>
        /// Full Path: Aequus/Tiles/CraftingStations/RecyclingMachineTile_Highlight
        /// </summary>
        public static readonly TextureAsset RecyclingMachineTile_Highlight = new TextureAsset("Aequus/Tiles/CraftingStations/RecyclingMachineTile_Highlight");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/BossChecklist/RedSprite
        /// </summary>
        public static readonly TextureAsset RedSprite_BossChecklist = new TextureAsset("Aequus/Assets/UI/BossChecklist/RedSprite");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/RedSpriteMiniboss/RedSprite_Glow
        /// </summary>
        public static readonly TextureAsset RedSprite_Glow = new TextureAsset("Aequus/Content/Boss/RedSpriteMiniboss/RedSprite_Glow");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/RedSpriteMiniboss/RedSprite_Head_Boss
        /// </summary>
        public static readonly TextureAsset RedSprite_Head_Boss = new TextureAsset("Aequus/Content/Boss/RedSpriteMiniboss/RedSprite_Head_Boss");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/RedSpriteMiniboss/RedSprite
        /// </summary>
        public static readonly TextureAsset RedSprite_RedSpriteMiniboss = new TextureAsset("Aequus/Content/Boss/RedSpriteMiniboss/RedSprite");
        /// <summary>
        /// Full Path: Aequus/Buffs/Pets/RedSpriteBuff
        /// </summary>
        public static readonly TextureAsset RedSpriteBuff = new TextureAsset("Aequus/Buffs/Pets/RedSpriteBuff");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/RedSpriteMiniboss/Projectiles/RedSpriteCloud
        /// </summary>
        public static readonly TextureAsset RedSpriteCloud = new TextureAsset("Aequus/Content/Boss/RedSpriteMiniboss/Projectiles/RedSpriteCloud");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/RedSpriteMiniboss/Projectiles/RedSpriteCloudLightning
        /// </summary>
        public static readonly TextureAsset RedSpriteCloudLightning = new TextureAsset("Aequus/Content/Boss/RedSpriteMiniboss/Projectiles/RedSpriteCloudLightning");
        /// <summary>
        /// Full Path: Aequus/Particles/Dusts/RedSpriteDust
        /// </summary>
        public static readonly TextureAsset RedSpriteDust = new TextureAsset("Aequus/Particles/Dusts/RedSpriteDust");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/RedSpriteMiniboss/RedSpriteFriendly
        /// </summary>
        public static readonly TextureAsset RedSpriteFriendly = new TextureAsset("Aequus/Content/Boss/RedSpriteMiniboss/RedSpriteFriendly");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/RedSpriteMiniboss/Rewards/RedSpriteMask
        /// </summary>
        public static readonly TextureAsset RedSpriteMask = new TextureAsset("Aequus/Content/Boss/RedSpriteMiniboss/Rewards/RedSpriteMask");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/RedSpriteMiniboss/Rewards/RedSpriteMask_Head
        /// </summary>
        public static readonly TextureAsset RedSpriteMask_Head = new TextureAsset("Aequus/Content/Boss/RedSpriteMiniboss/Rewards/RedSpriteMask_Head");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/RedSpriteMinion
        /// </summary>
        public static readonly TextureAsset RedSpriteMinion = new TextureAsset("Aequus/Projectiles/Summon/RedSpriteMinion");
        /// <summary>
        /// Full Path: Aequus/Buffs/Minion/RedSpriteMinionBuff
        /// </summary>
        public static readonly TextureAsset RedSpriteMinionBuff = new TextureAsset("Aequus/Buffs/Minion/RedSpriteMinionBuff");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/Pets/RedSpritePet
        /// </summary>
        public static readonly TextureAsset RedSpritePet = new TextureAsset("Aequus/Projectiles/Misc/Pets/RedSpritePet");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/Pets/RedSpritePet_Glow
        /// </summary>
        public static readonly TextureAsset RedSpritePet_Glow = new TextureAsset("Aequus/Projectiles/Misc/Pets/RedSpritePet_Glow");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/RedSpriteMiniboss/Rewards/RedSpriteRelic
        /// </summary>
        public static readonly TextureAsset RedSpriteRelic = new TextureAsset("Aequus/Content/Boss/RedSpriteMiniboss/Rewards/RedSpriteRelic");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/RedSpriteMiniboss/Projectiles/RedSpriteThunderClap
        /// </summary>
        public static readonly TextureAsset RedSpriteThunderClap = new TextureAsset("Aequus/Content/Boss/RedSpriteMiniboss/Projectiles/RedSpriteThunderClap");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/RedSpriteMiniboss/Rewards/RedSpriteTrophy
        /// </summary>
        public static readonly TextureAsset RedSpriteTrophy = new TextureAsset("Aequus/Content/Boss/RedSpriteMiniboss/Rewards/RedSpriteTrophy");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/RedSpriteMiniboss/Projectiles/RedSpriteWindFire
        /// </summary>
        public static readonly TextureAsset RedSpriteWindFire = new TextureAsset("Aequus/Content/Boss/RedSpriteMiniboss/Projectiles/RedSpriteWindFire");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Equipment/RegrowingBait
        /// </summary>
        public static readonly TextureAsset RegrowingBait = new TextureAsset("Aequus/Content/Fishing/Equipment/RegrowingBait");
        /// <summary>
        /// Full Path: Aequus/Particles/Dusts/ReliefshroomDustSpore
        /// </summary>
        public static readonly TextureAsset ReliefshroomDustSpore = new TextureAsset("Aequus/Particles/Dusts/ReliefshroomDustSpore");
        /// <summary>
        /// Full Path: Aequus/Content/Town/SkyMerchantNPC/NameTags/RenameBackIcon
        /// </summary>
        public static readonly TextureAsset RenameBackIcon = new TextureAsset("Aequus/Content/Town/SkyMerchantNPC/NameTags/RenameBackIcon");
        /// <summary>
        /// Full Path: Aequus/Content/Town/ExporterNPC/RerollSystem/RerollButton
        /// </summary>
        public static readonly TextureAsset RerollButton = new TextureAsset("Aequus/Content/Town/ExporterNPC/RerollSystem/RerollButton");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/Thrown/Resonance
        /// </summary>
        public static readonly TextureAsset Resonance = new TextureAsset("Aequus/Items/Weapons/Melee/Thrown/Resonance");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Scepters/Revenant
        /// </summary>
        public static readonly TextureAsset Revenant = new TextureAsset("Aequus/Items/Weapons/Necromancy/Scepters/Revenant");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Scepters/Revenant_Glow
        /// </summary>
        public static readonly TextureAsset Revenant_Glow = new TextureAsset("Aequus/Items/Weapons/Necromancy/Scepters/Revenant_Glow");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Utility/RichMansMonocle
        /// </summary>
        public static readonly TextureAsset RichMansMonocle = new TextureAsset("Aequus/Items/Accessories/Utility/RichMansMonocle");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/RitualBuff
        /// </summary>
        public static readonly TextureAsset RitualBuff = new TextureAsset("Aequus/Buffs/Misc/RitualBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Necro/RitualisticSkull
        /// </summary>
        public static readonly TextureAsset RitualisticSkull = new TextureAsset("Aequus/Items/Accessories/Offense/Necro/RitualisticSkull");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/RockMan
        /// </summary>
        public static readonly TextureAsset RockMan = new TextureAsset("Aequus/Items/Weapons/Melee/RockMan");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/SlotMachines/Roulette
        /// </summary>
        public static readonly TextureAsset Roulette = new TextureAsset("Aequus/Unused/Items/SlotMachines/Roulette");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/BigGems/RubyDeposit
        /// </summary>
        public static readonly TextureAsset RubyDeposit = new TextureAsset("Aequus/Tiles/Misc/BigGems/RubyDeposit");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Gems/RubyGore
        /// </summary>
        public static readonly TextureAsset RubyGore = new TextureAsset("Aequus/Assets/Gores/Gems/RubyGore");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Magic/SandstormStaff
        /// </summary>
        public static readonly TextureAsset SandstormStaff = new TextureAsset("Aequus/Items/Weapons/Magic/SandstormStaff");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/BigGems/SapphireDeposit
        /// </summary>
        public static readonly TextureAsset SapphireDeposit = new TextureAsset("Aequus/Tiles/Misc/BigGems/SapphireDeposit");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Gems/SapphireGore
        /// </summary>
        public static readonly TextureAsset SapphireGore = new TextureAsset("Aequus/Assets/Gores/Gems/SapphireGore");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Magic/Healer/SavingGrace
        /// </summary>
        public static readonly TextureAsset SavingGrace = new TextureAsset("Aequus/Items/Weapons/Magic/Healer/SavingGrace");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Magic/SavingGraceProj
        /// </summary>
        public static readonly TextureAsset SavingGraceProj = new TextureAsset("Aequus/Projectiles/Magic/SavingGraceProj");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Magic/SavingGraceProj_Aura
        /// </summary>
        public static readonly TextureAsset SavingGraceProj_Aura = new TextureAsset("Aequus/Projectiles/Magic/SavingGraceProj_Aura");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Magic/SavingGraceProj_GlowIndicator
        /// </summary>
        public static readonly TextureAsset SavingGraceProj_GlowIndicator = new TextureAsset("Aequus/Projectiles/Magic/SavingGraceProj_GlowIndicator");
        /// <summary>
        /// Full Path: Aequus/Items/Consumables/ScarabDynamite
        /// </summary>
        public static readonly TextureAsset ScarabDynamite = new TextureAsset("Aequus/Items/Consumables/ScarabDynamite");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Dyes/ScorchingDye
        /// </summary>
        public static readonly TextureAsset ScorchingDye = new TextureAsset("Aequus/Items/Misc/Dyes/ScorchingDye");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Summon/Minion/ScribbleNotebook
        /// </summary>
        public static readonly TextureAsset ScribbleNotebook = new TextureAsset("Aequus/Items/Weapons/Summon/Minion/ScribbleNotebook");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Summon/Minion/ScribbleNotebook_Glow
        /// </summary>
        public static readonly TextureAsset ScribbleNotebook_Glow = new TextureAsset("Aequus/Items/Weapons/Summon/Minion/ScribbleNotebook_Glow");
        /// <summary>
        /// Full Path: Aequus/Buffs/Minion/ScribbleNotebookBuff
        /// </summary>
        public static readonly TextureAsset ScribbleNotebookBuff = new TextureAsset("Aequus/Buffs/Minion/ScribbleNotebookBuff");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/ScribbleNotebookMinion
        /// </summary>
        public static readonly TextureAsset ScribbleNotebookMinion = new TextureAsset("Aequus/Projectiles/Summon/ScribbleNotebookMinion");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Dyes/ScrollDye
        /// </summary>
        public static readonly TextureAsset ScrollDye = new TextureAsset("Aequus/Items/Misc/Dyes/ScrollDye");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/CrabCrevice/Tiles/SeaPickle
        /// </summary>
        public static readonly TextureAsset SeaPickle = new TextureAsset("Aequus/Content/Biomes/CrabCrevice/Tiles/SeaPickle");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/CrabCrevice/Tiles/SeaPickleTile
        /// </summary>
        public static readonly TextureAsset SeaPickleTile = new TextureAsset("Aequus/Content/Biomes/CrabCrevice/Tiles/SeaPickleTile");
        /// <summary>
        /// Full Path: Aequus/Buffs/Buildings/SecretEntranceBuff
        /// </summary>
        public static readonly TextureAsset SecretEntranceBuff = new TextureAsset("Aequus/Buffs/Buildings/SecretEntranceBuff");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/CrabCrevice/Tiles/SedimentaryRock
        /// </summary>
        public static readonly TextureAsset SedimentaryRock = new TextureAsset("Aequus/Content/Biomes/CrabCrevice/Tiles/SedimentaryRock");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/CrabCrevice/Tiles/SedimentaryRockTile
        /// </summary>
        public static readonly TextureAsset SedimentaryRockTile = new TextureAsset("Aequus/Content/Biomes/CrabCrevice/Tiles/SedimentaryRockTile");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/CrabCrevice/Tiles/SedimentaryRockWall
        /// </summary>
        public static readonly TextureAsset SedimentaryRockWall = new TextureAsset("Aequus/Content/Biomes/CrabCrevice/Tiles/SedimentaryRockWall");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/CrabCrevice/Tiles/SedimentaryRockWallWall
        /// </summary>
        public static readonly TextureAsset SedimentaryRockWallWall = new TextureAsset("Aequus/Content/Biomes/CrabCrevice/Tiles/SedimentaryRockWallWall");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/SentryInheriters/Sentinel6510
        /// </summary>
        public static readonly TextureAsset Sentinel6510 = new TextureAsset("Aequus/Items/Accessories/SentryInheriters/Sentinel6510");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/SentryInheriters/Sentinel6510_Eyes
        /// </summary>
        public static readonly TextureAsset Sentinel6510_Eyes = new TextureAsset("Aequus/Items/Accessories/SentryInheriters/Sentinel6510_Eyes");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/SentryInheriters/Sentry6502
        /// </summary>
        public static readonly TextureAsset Sentry6502 = new TextureAsset("Aequus/Items/Accessories/SentryInheriters/Sentry6502");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/SentryInheriters/Sentry6502_Eyes
        /// </summary>
        public static readonly TextureAsset Sentry6502_Eyes = new TextureAsset("Aequus/Items/Accessories/SentryInheriters/Sentry6502_Eyes");
        /// <summary>
        /// Full Path: Aequus/Buffs/SentryBuff
        /// </summary>
        public static readonly TextureAsset SentryBuff = new TextureAsset("Aequus/Buffs/SentryBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Potions/SentryPotion
        /// </summary>
        public static readonly TextureAsset SentryPotion = new TextureAsset("Aequus/Items/Potions/SentryPotion");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Sentry/SentrySquid
        /// </summary>
        public static readonly TextureAsset SentrySquid = new TextureAsset("Aequus/Items/Accessories/Offense/Sentry/SentrySquid");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Sentry/SentrySquid_Hat
        /// </summary>
        public static readonly TextureAsset SentrySquid_Hat = new TextureAsset("Aequus/Items/Accessories/Offense/Sentry/SentrySquid_Hat");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetSeraphim/SeraphimHood
        /// </summary>
        public static readonly TextureAsset SeraphimHood = new TextureAsset("Aequus/Items/Armor/SetSeraphim/SeraphimHood");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetSeraphim/SeraphimHood_Head
        /// </summary>
        public static readonly TextureAsset SeraphimHood_Head = new TextureAsset("Aequus/Items/Armor/SetSeraphim/SeraphimHood_Head");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetSeraphim/SeraphimRobes
        /// </summary>
        public static readonly TextureAsset SeraphimRobes = new TextureAsset("Aequus/Items/Armor/SetSeraphim/SeraphimRobes");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetSeraphim/SeraphimRobes_Body
        /// </summary>
        public static readonly TextureAsset SeraphimRobes_Body = new TextureAsset("Aequus/Items/Armor/SetSeraphim/SeraphimRobes_Body");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/SlotMachines/ShadowRoulette
        /// </summary>
        public static readonly TextureAsset ShadowRoulette = new TextureAsset("Aequus/Unused/Items/SlotMachines/ShadowRoulette");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Necro/ShadowVeer
        /// </summary>
        public static readonly TextureAsset ShadowVeer = new TextureAsset("Aequus/Items/Accessories/Offense/Necro/ShadowVeer");
        /// <summary>
        /// Full Path: Aequus/Assets/Shatter
        /// </summary>
        public static readonly TextureAsset Shatter = new TextureAsset("Aequus/Assets/Shatter");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/CrownOfBlood/Projectiles/ShieldOfCthulhuBoost
        /// </summary>
        public static readonly TextureAsset ShieldOfCthulhuBoost = new TextureAsset("Aequus/Items/Accessories/CrownOfBlood/Projectiles/ShieldOfCthulhuBoost");
        /// <summary>
        /// Full Path: Aequus/Items/Consumables/ShimmerFish
        /// </summary>
        public static readonly TextureAsset ShimmerFish = new TextureAsset("Aequus/Items/Consumables/ShimmerFish");
        /// <summary>
        /// Full Path: Aequus/Particles/ShinyFlashParticle
        /// </summary>
        public static readonly TextureAsset ShinyFlashParticle = new TextureAsset("Aequus/Particles/ShinyFlashParticle");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Misc/Shutterstocker
        /// </summary>
        public static readonly TextureAsset Shutterstocker = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Misc/Shutterstocker");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Misc/ShutterstockerClip
        /// </summary>
        public static readonly TextureAsset ShutterstockerClip = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Misc/ShutterstockerClip");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/ShutterstockerInterface
        /// </summary>
        public static readonly TextureAsset ShutterstockerInterface = new TextureAsset("Aequus/Assets/UI/ShutterstockerInterface");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Misc/ShutterstockerMarks
        /// </summary>
        public static readonly TextureAsset ShutterstockerMarks = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Misc/ShutterstockerMarks");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/ShutterstockerProj
        /// </summary>
        public static readonly TextureAsset ShutterstockerProj = new TextureAsset("Aequus/Projectiles/Misc/ShutterstockerProj");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/Thrown/SickBeat
        /// </summary>
        public static readonly TextureAsset SickBeat = new TextureAsset("Aequus/Items/Weapons/Melee/Thrown/SickBeat");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/SilkHammer
        /// </summary>
        public static readonly TextureAsset SilkHammer = new TextureAsset("Aequus/Unused/Items/SilkHammer");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/SilkPickaxe
        /// </summary>
        public static readonly TextureAsset SilkPickaxe = new TextureAsset("Aequus/Unused/Items/SilkPickaxe");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Dyes/SimplifiedDye
        /// </summary>
        public static readonly TextureAsset SimplifiedDye = new TextureAsset("Aequus/Items/Misc/Dyes/SimplifiedDye");
        /// <summary>
        /// Full Path: Aequus/Items/Tools/SkeletonKey
        /// </summary>
        public static readonly TextureAsset SkeletonKey = new TextureAsset("Aequus/Items/Tools/SkeletonKey");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/SkeletonMerchantHead
        /// </summary>
        public static readonly TextureAsset SkeletonMerchantHead = new TextureAsset("Aequus/Assets/UI/SkeletonMerchantHead");
        /// <summary>
        /// Full Path: Aequus/Content/Town/SkyMerchantNPC/SkyMerchant
        /// </summary>
        public static readonly TextureAsset SkyMerchant = new TextureAsset("Aequus/Content/Town/SkyMerchantNPC/SkyMerchant");
        /// <summary>
        /// Full Path: Aequus/Content/Town/SkyMerchantNPC/SkyMerchant_Head
        /// </summary>
        public static readonly TextureAsset SkyMerchant_Head = new TextureAsset("Aequus/Content/Town/SkyMerchantNPC/SkyMerchant_Head");
        /// <summary>
        /// Full Path: Aequus/Content/Town/SkyMerchantNPC/SkyMerchant_Resprite
        /// </summary>
        public static readonly TextureAsset SkyMerchant_Resprite = new TextureAsset("Aequus/Content/Town/SkyMerchantNPC/SkyMerchant_Resprite");
        /// <summary>
        /// Full Path: Aequus/Content/Town/SkyMerchantNPC/SkyMerchantBalloon
        /// </summary>
        public static readonly TextureAsset SkyMerchantBalloon = new TextureAsset("Aequus/Content/Town/SkyMerchantNPC/SkyMerchantBalloon");
        /// <summary>
        /// Full Path: Aequus/Content/Town/SkyMerchantNPC/SkyMerchantBasket
        /// </summary>
        public static readonly TextureAsset SkyMerchantBasket = new TextureAsset("Aequus/Content/Town/SkyMerchantNPC/SkyMerchantBasket");
        /// <summary>
        /// Full Path: Aequus/Content/Town/SkyMerchantNPC/SkyMerchantFlee
        /// </summary>
        public static readonly TextureAsset SkyMerchantFlee = new TextureAsset("Aequus/Content/Town/SkyMerchantNPC/SkyMerchantFlee");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Paintings/Items/SkyrimRock1
        /// </summary>
        public static readonly TextureAsset SkyrimRock1 = new TextureAsset("Aequus/Tiles/Furniture/Paintings/Items/SkyrimRock1");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Paintings/Items/SkyrimRock2
        /// </summary>
        public static readonly TextureAsset SkyrimRock2 = new TextureAsset("Aequus/Tiles/Furniture/Paintings/Items/SkyrimRock2");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Paintings/Items/SkyrimRock3
        /// </summary>
        public static readonly TextureAsset SkyrimRock3 = new TextureAsset("Aequus/Tiles/Furniture/Paintings/Items/SkyrimRock3");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/SlotMachines/SkyRoulette
        /// </summary>
        public static readonly TextureAsset SkyRoulette = new TextureAsset("Aequus/Unused/Items/SlotMachines/SkyRoulette");
        /// <summary>
        /// Full Path: Aequus/Assets/Textures/SlamEffect0
        /// </summary>
        public static readonly TextureAsset SlamEffect0 = new TextureAsset("Aequus/Assets/Textures/SlamEffect0");
        /// <summary>
        /// Full Path: Aequus/Assets/Textures/SlamEffect1
        /// </summary>
        public static readonly TextureAsset SlamEffect1 = new TextureAsset("Aequus/Assets/Textures/SlamEffect1");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/Slice
        /// </summary>
        public static readonly TextureAsset Slice = new TextureAsset("Aequus/Items/Weapons/Melee/Slice");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Melee/Swords/SliceBulletProj
        /// </summary>
        public static readonly TextureAsset SliceBulletProj = new TextureAsset("Aequus/Projectiles/Melee/Swords/SliceBulletProj");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Ranged/Misc/Slingsaber
        /// </summary>
        public static readonly TextureAsset Slingsaber = new TextureAsset("Aequus/Items/Weapons/Ranged/Misc/Slingsaber");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Ranged/Misc/Slingsaber_Glow
        /// </summary>
        public static readonly TextureAsset Slingsaber_Glow = new TextureAsset("Aequus/Items/Weapons/Ranged/Misc/Slingsaber_Glow");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Ranged/SlingsaberProj
        /// </summary>
        public static readonly TextureAsset SlingsaberProj = new TextureAsset("Aequus/Projectiles/Ranged/SlingsaberProj");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Ranged/SlingsaberProj_Glow
        /// </summary>
        public static readonly TextureAsset SlingsaberProj_Glow = new TextureAsset("Aequus/Projectiles/Ranged/SlingsaberProj_Glow");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Ranged/Misc/Slingshot
        /// </summary>
        public static readonly TextureAsset Slingshot = new TextureAsset("Aequus/Items/Weapons/Ranged/Misc/Slingshot");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Ranged/SlingshotProj
        /// </summary>
        public static readonly TextureAsset SlingshotProj = new TextureAsset("Aequus/Projectiles/Ranged/SlingshotProj");
        /// <summary>
        /// Full Path: Aequus/Content/Critters/Snobster
        /// </summary>
        public static readonly TextureAsset Snobster = new TextureAsset("Aequus/Content/Critters/Snobster");
        /// <summary>
        /// Full Path: Aequus/Content/Critters/SnobsterItem
        /// </summary>
        public static readonly TextureAsset SnobsterItem = new TextureAsset("Aequus/Content/Critters/SnobsterItem");
        /// <summary>
        /// Full Path: Aequus/Buffs/Minion/SnowflakeBuff
        /// </summary>
        public static readonly TextureAsset SnowflakeBuff = new TextureAsset("Aequus/Buffs/Minion/SnowflakeBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Ranged/SnowflakeCannon
        /// </summary>
        public static readonly TextureAsset SnowflakeCannon = new TextureAsset("Aequus/Items/Weapons/Ranged/SnowflakeCannon");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/SnowflakeMinion
        /// </summary>
        public static readonly TextureAsset SnowflakeMinion = new TextureAsset("Aequus/Projectiles/Summon/SnowflakeMinion");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Magic/Snowgrave
        /// </summary>
        public static readonly TextureAsset Snowgrave = new TextureAsset("Aequus/Items/Weapons/Magic/Snowgrave");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Magic/SnowgraveProj
        /// </summary>
        public static readonly TextureAsset SnowgraveProj = new TextureAsset("Aequus/Projectiles/Magic/SnowgraveProj");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/SlotMachines/SnowRoulette
        /// </summary>
        public static readonly TextureAsset SnowRoulette = new TextureAsset("Aequus/Unused/Items/SlotMachines/SnowRoulette");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/CrabCrevice/SoldierCrab
        /// </summary>
        public static readonly TextureAsset SoldierCrab = new TextureAsset("Aequus/NPCs/Monsters/CrabCrevice/SoldierCrab");
        /// <summary>
        /// Full Path: Aequus/Tiles/Banners/Items/SoldierCrabBanner
        /// </summary>
        public static readonly TextureAsset SoldierCrabBanner = new TextureAsset("Aequus/Tiles/Banners/Items/SoldierCrabBanner");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Monster/SoldierCrabProj
        /// </summary>
        public static readonly TextureAsset SoldierCrabProj = new TextureAsset("Aequus/Projectiles/Monster/SoldierCrabProj");
        /// <summary>
        /// Full Path: Aequus/Assets/SoulChains
        /// </summary>
        public static readonly TextureAsset SoulChains = new TextureAsset("Aequus/Assets/SoulChains");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/Gems/SoulGem
        /// </summary>
        public static readonly TextureAsset SoulGem = new TextureAsset("Aequus/Items/Materials/Gems/SoulGem");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/Gems/SoulGemTile
        /// </summary>
        public static readonly TextureAsset SoulGemTile = new TextureAsset("Aequus/Items/Materials/Gems/SoulGemTile");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Necro/SouljointCuffs
        /// </summary>
        public static readonly TextureAsset SouljointCuffs = new TextureAsset("Aequus/Items/Accessories/Offense/Necro/SouljointCuffs");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Debuff/SoulNeglectite
        /// </summary>
        public static readonly TextureAsset SoulNeglectite = new TextureAsset("Aequus/Items/Accessories/Debuff/SoulNeglectite");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/BossChecklist/SpaceSquid
        /// </summary>
        public static readonly TextureAsset SpaceSquid_BossChecklist = new TextureAsset("Aequus/Assets/UI/BossChecklist/SpaceSquid");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/SpaceSquidMiniboss/SpaceSquid_Glow
        /// </summary>
        public static readonly TextureAsset SpaceSquid_Glow = new TextureAsset("Aequus/Content/Boss/SpaceSquidMiniboss/SpaceSquid_Glow");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/SpaceSquidMiniboss/SpaceSquid_Head_Boss
        /// </summary>
        public static readonly TextureAsset SpaceSquid_Head_Boss = new TextureAsset("Aequus/Content/Boss/SpaceSquidMiniboss/SpaceSquid_Head_Boss");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/SpaceSquidMiniboss/SpaceSquid
        /// </summary>
        public static readonly TextureAsset SpaceSquid_SpaceSquidMiniboss = new TextureAsset("Aequus/Content/Boss/SpaceSquidMiniboss/SpaceSquid");
        /// <summary>
        /// Full Path: Aequus/Particles/Dusts/SpaceSquidBlood
        /// </summary>
        public static readonly TextureAsset SpaceSquidBlood = new TextureAsset("Aequus/Particles/Dusts/SpaceSquidBlood");
        /// <summary>
        /// Full Path: Aequus/Buffs/Pets/SpaceSquidBuff
        /// </summary>
        public static readonly TextureAsset SpaceSquidBuff = new TextureAsset("Aequus/Buffs/Pets/SpaceSquidBuff");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/SpaceSquidMiniboss/Projectiles/SpaceSquidDeathray
        /// </summary>
        public static readonly TextureAsset SpaceSquidDeathray = new TextureAsset("Aequus/Content/Boss/SpaceSquidMiniboss/Projectiles/SpaceSquidDeathray");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/SpaceSquidMiniboss/SpaceSquidDefeated
        /// </summary>
        public static readonly TextureAsset SpaceSquidDefeated = new TextureAsset("Aequus/Content/Boss/SpaceSquidMiniboss/SpaceSquidDefeated");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/SpaceSquidMiniboss/SpaceSquidDefeated_Glow
        /// </summary>
        public static readonly TextureAsset SpaceSquidDefeated_Glow = new TextureAsset("Aequus/Content/Boss/SpaceSquidMiniboss/SpaceSquidDefeated_Glow");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/SpaceSquidMiniboss/SpaceSquidFriendly
        /// </summary>
        public static readonly TextureAsset SpaceSquidFriendly = new TextureAsset("Aequus/Content/Boss/SpaceSquidMiniboss/SpaceSquidFriendly");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/SpaceSquidMiniboss/Projectiles/SpaceSquidLaser
        /// </summary>
        public static readonly TextureAsset SpaceSquidLaser = new TextureAsset("Aequus/Content/Boss/SpaceSquidMiniboss/Projectiles/SpaceSquidLaser");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/SpaceSquidMiniboss/Rewards/SpaceSquidMask
        /// </summary>
        public static readonly TextureAsset SpaceSquidMask = new TextureAsset("Aequus/Content/Boss/SpaceSquidMiniboss/Rewards/SpaceSquidMask");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/SpaceSquidMiniboss/Rewards/SpaceSquidMask_Head
        /// </summary>
        public static readonly TextureAsset SpaceSquidMask_Head = new TextureAsset("Aequus/Content/Boss/SpaceSquidMiniboss/Rewards/SpaceSquidMask_Head");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/Pets/SpaceSquidPet
        /// </summary>
        public static readonly TextureAsset SpaceSquidPet = new TextureAsset("Aequus/Projectiles/Misc/Pets/SpaceSquidPet");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/Pets/SpaceSquidPet_Glow
        /// </summary>
        public static readonly TextureAsset SpaceSquidPet_Glow = new TextureAsset("Aequus/Projectiles/Misc/Pets/SpaceSquidPet_Glow");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/SpaceSquidMiniboss/Rewards/SpaceSquidRelic
        /// </summary>
        public static readonly TextureAsset SpaceSquidRelic = new TextureAsset("Aequus/Content/Boss/SpaceSquidMiniboss/Rewards/SpaceSquidRelic");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/SpaceSquidMiniboss/Projectiles/SpaceSquidSnowflake
        /// </summary>
        public static readonly TextureAsset SpaceSquidSnowflake = new TextureAsset("Aequus/Content/Boss/SpaceSquidMiniboss/Projectiles/SpaceSquidSnowflake");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/SpaceSquidMiniboss/Rewards/SpaceSquidTrophy
        /// </summary>
        public static readonly TextureAsset SpaceSquidTrophy = new TextureAsset("Aequus/Content/Boss/SpaceSquidMiniboss/Rewards/SpaceSquidTrophy");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Magic/SpamMail
        /// </summary>
        public static readonly TextureAsset SpamMail = new TextureAsset("Aequus/Items/Weapons/Magic/SpamMail");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/CandleSpawners/SpawnEffect
        /// </summary>
        public static readonly TextureAsset SpawnEffect = new TextureAsset("Aequus/Projectiles/Summon/CandleSpawners/SpawnEffect");
        /// <summary>
        /// Full Path: Aequus/Items/Consumables/Foods/SpicyEel
        /// </summary>
        public static readonly TextureAsset SpicyEel = new TextureAsset("Aequus/Items/Consumables/Foods/SpicyEel");
        /// <summary>
        /// Full Path: Aequus/Buffs/SpicyEelBuff
        /// </summary>
        public static readonly TextureAsset SpicyEelBuff = new TextureAsset("Aequus/Buffs/SpicyEelBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Necro/SpiritBottle
        /// </summary>
        public static readonly TextureAsset SpiritBottle = new TextureAsset("Aequus/Items/Accessories/Offense/Necro/SpiritBottle");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Necro/SpiritKeg
        /// </summary>
        public static readonly TextureAsset SpiritKeg = new TextureAsset("Aequus/Items/Accessories/Offense/Necro/SpiritKeg");
        /// <summary>
        /// Full Path: Aequus/Content/ItemPrefixes/Potions/SplashGlint
        /// </summary>
        public static readonly TextureAsset SplashGlint = new TextureAsset("Aequus/Content/ItemPrefixes/Potions/SplashGlint");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Poles/Starcatcher
        /// </summary>
        public static readonly TextureAsset Starcatcher = new TextureAsset("Aequus/Content/Fishing/Poles/Starcatcher");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/Bobbers/StarcatcherBobber
        /// </summary>
        public static readonly TextureAsset StarcatcherBobber = new TextureAsset("Aequus/Projectiles/Misc/Bobbers/StarcatcherBobber");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Night/Glimmer/Starite
        /// </summary>
        public static readonly TextureAsset Starite = new TextureAsset("Aequus/NPCs/Monsters/Night/Glimmer/Starite");
        /// <summary>
        /// Full Path: Aequus/Tiles/Banners/Items/StariteBanner
        /// </summary>
        public static readonly TextureAsset StariteBanner = new TextureAsset("Aequus/Tiles/Banners/Items/StariteBanner");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/StariteBottle
        /// </summary>
        public static readonly TextureAsset StariteBottle = new TextureAsset("Aequus/Tiles/Misc/StariteBottle");
        /// <summary>
        /// Full Path: Aequus/Buffs/StariteBottleBuff
        /// </summary>
        public static readonly TextureAsset StariteBottleBuff = new TextureAsset("Aequus/Buffs/StariteBottleBuff");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/StariteBottleTile
        /// </summary>
        public static readonly TextureAsset StariteBottleTile = new TextureAsset("Aequus/Tiles/Misc/StariteBottleTile");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/StariteBottleTile_Glow
        /// </summary>
        public static readonly TextureAsset StariteBottleTile_Glow = new TextureAsset("Aequus/Tiles/Misc/StariteBottleTile_Glow");
        /// <summary>
        /// Full Path: Aequus/Buffs/Minion/StariteBuff
        /// </summary>
        public static readonly TextureAsset StariteBuff = new TextureAsset("Aequus/Buffs/Minion/StariteBuff");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/StariteMaterial
        /// </summary>
        public static readonly TextureAsset StariteMaterial = new TextureAsset("Aequus/Items/Materials/StariteMaterial");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/StariteMinion
        /// </summary>
        public static readonly TextureAsset StariteMinion = new TextureAsset("Aequus/Projectiles/Summon/StariteMinion");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Summon/Minion/StariteStaff
        /// </summary>
        public static readonly TextureAsset StariteStaff = new TextureAsset("Aequus/Items/Weapons/Summon/Minion/StariteStaff");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Summon/Minion/StariteStaff_Glow
        /// </summary>
        public static readonly TextureAsset StariteStaff_Glow = new TextureAsset("Aequus/Items/Weapons/Summon/Minion/StariteStaff_Glow");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Ranged/StarPhish
        /// </summary>
        public static readonly TextureAsset StarPhish = new TextureAsset("Aequus/Items/Weapons/Ranged/StarPhish");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/StatusBubble
        /// </summary>
        public static readonly TextureAsset StatusBubble = new TextureAsset("Aequus/Assets/UI/StatusBubble");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Crit/SteakEyes
        /// </summary>
        public static readonly TextureAsset SteakEyes = new TextureAsset("Aequus/Items/Accessories/Offense/Crit/SteakEyes");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Passive/Stormcloak
        /// </summary>
        public static readonly TextureAsset Stormcloak = new TextureAsset("Aequus/Items/Accessories/Passive/Stormcloak");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Passive/Stormcloak_Back
        /// </summary>
        public static readonly TextureAsset Stormcloak_Back = new TextureAsset("Aequus/Items/Accessories/Passive/Stormcloak_Back");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Passive/Stormcloak_Front
        /// </summary>
        public static readonly TextureAsset Stormcloak_Front = new TextureAsset("Aequus/Items/Accessories/Passive/Stormcloak_Front");
        /// <summary>
        /// Full Path: Aequus/Buffs/Cooldowns/StormcloakCooldown
        /// </summary>
        public static readonly TextureAsset StormcloakCooldown = new TextureAsset("Aequus/Buffs/Cooldowns/StormcloakCooldown");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Sky/GaleStreams/StreamingBalloon
        /// </summary>
        public static readonly TextureAsset StreamingBalloon = new TextureAsset("Aequus/NPCs/Monsters/Sky/GaleStreams/StreamingBalloon");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Magic/StudiesOfTheInkblot
        /// </summary>
        public static readonly TextureAsset StudiesOfTheInkblot = new TextureAsset("Aequus/Items/Weapons/Magic/StudiesOfTheInkblot");
        /// <summary>
        /// Full Path: Aequus/Content/ItemPrefixes/Potions/StuffedGlint
        /// </summary>
        public static readonly TextureAsset StuffedGlint = new TextureAsset("Aequus/Content/ItemPrefixes/Potions/StuffedGlint");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/CrabCrevice/SummonerCrab
        /// </summary>
        public static readonly TextureAsset SummonerCrab = new TextureAsset("Aequus/NPCs/Monsters/CrabCrevice/SummonerCrab");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/CrabCrevice/SummonerCrab_Glow
        /// </summary>
        public static readonly TextureAsset SummonerCrab_Glow = new TextureAsset("Aequus/NPCs/Monsters/CrabCrevice/SummonerCrab_Glow");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/CrabCrevice/SummonerCrabMinion
        /// </summary>
        public static readonly TextureAsset SummonerCrabMinion = new TextureAsset("Aequus/NPCs/Monsters/CrabCrevice/SummonerCrabMinion");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/CrabCrevice/SummonerCrabMinion_Glow
        /// </summary>
        public static readonly TextureAsset SummonerCrabMinion_Glow = new TextureAsset("Aequus/NPCs/Monsters/CrabCrevice/SummonerCrabMinion_Glow");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetTrap/SuperDartTrapHat
        /// </summary>
        public static readonly TextureAsset SuperDartTrapHat = new TextureAsset("Aequus/Items/Armor/SetTrap/SuperDartTrapHat");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetTrap/SuperDartTrapHat_Head
        /// </summary>
        public static readonly TextureAsset SuperDartTrapHat_Head = new TextureAsset("Aequus/Items/Armor/SetTrap/SuperDartTrapHat_Head");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/OmegaStarite/Misc/SupernovaFruit
        /// </summary>
        public static readonly TextureAsset SupernovaFruit = new TextureAsset("Aequus/Content/Boss/OmegaStarite/Misc/SupernovaFruit");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Night/Glimmer/SuperStarite
        /// </summary>
        public static readonly TextureAsset SuperStarite = new TextureAsset("Aequus/NPCs/Monsters/Night/Glimmer/SuperStarite");
        /// <summary>
        /// Full Path: Aequus/Tiles/Banners/Items/SuperStariteBanner
        /// </summary>
        public static readonly TextureAsset SuperStariteBanner = new TextureAsset("Aequus/Tiles/Banners/Items/SuperStariteBanner");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Monster/SuperStariteBullet
        /// </summary>
        public static readonly TextureAsset SuperStariteBullet = new TextureAsset("Aequus/Projectiles/Monster/SuperStariteBullet");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/Heavy/SuperStarSword
        /// </summary>
        public static readonly TextureAsset SuperStarSword = new TextureAsset("Aequus/Items/Weapons/Melee/Heavy/SuperStarSword");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Melee/Swords/SuperStarSwordProj
        /// </summary>
        public static readonly TextureAsset SuperStarSwordProj = new TextureAsset("Aequus/Projectiles/Melee/Swords/SuperStarSwordProj");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Melee/SuperStarSwordSlash
        /// </summary>
        public static readonly TextureAsset SuperStarSwordSlash = new TextureAsset("Aequus/Projectiles/Melee/SuperStarSwordSlash");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Magic/SurgeRod
        /// </summary>
        public static readonly TextureAsset SurgeRod = new TextureAsset("Aequus/Items/Weapons/Magic/SurgeRod");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Magic/SurgeRod_Glow
        /// </summary>
        public static readonly TextureAsset SurgeRod_Glow = new TextureAsset("Aequus/Items/Weapons/Magic/SurgeRod_Glow");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Magic/SurgeRodProj
        /// </summary>
        public static readonly TextureAsset SurgeRodProj = new TextureAsset("Aequus/Projectiles/Magic/SurgeRodProj");
        /// <summary>
        /// Full Path: Aequus/Items/Consumables/Foods/SuspiciousLookingSteak
        /// </summary>
        public static readonly TextureAsset SuspiciousLookingSteak = new TextureAsset("Aequus/Items/Consumables/Foods/SuspiciousLookingSteak");
        /// <summary>
        /// Full Path: Aequus/Items/Vanity/Pets/SwagLookingEye
        /// </summary>
        public static readonly TextureAsset SwagLookingEye = new TextureAsset("Aequus/Items/Vanity/Pets/SwagLookingEye");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Melee/Swords/Swish
        /// </summary>
        public static readonly TextureAsset Swish = new TextureAsset("Aequus/Projectiles/Melee/Swords/Swish");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Melee/Swords/Swish2
        /// </summary>
        public static readonly TextureAsset Swish2 = new TextureAsset("Aequus/Projectiles/Melee/Swords/Swish2");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Melee/Swords/SwishNoEdit
        /// </summary>
        public static readonly TextureAsset SwishNoEdit = new TextureAsset("Aequus/Projectiles/Melee/Swords/SwishNoEdit");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_10
        /// </summary>
        public static readonly TextureAsset SwordCursor_10 = new TextureAsset("Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_10");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_2
        /// </summary>
        public static readonly TextureAsset SwordCursor_2 = new TextureAsset("Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_2");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_3
        /// </summary>
        public static readonly TextureAsset SwordCursor_3 = new TextureAsset("Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_3");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_6
        /// </summary>
        public static readonly TextureAsset SwordCursor_6 = new TextureAsset("Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_6");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_7
        /// </summary>
        public static readonly TextureAsset SwordCursor_7 = new TextureAsset("Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_7");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_8
        /// </summary>
        public static readonly TextureAsset SwordCursor_8 = new TextureAsset("Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_8");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_9
        /// </summary>
        public static readonly TextureAsset SwordCursor_9 = new TextureAsset("Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_9");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/SwordCursor
        /// </summary>
        public static readonly TextureAsset SwordCursor_Items = new TextureAsset("Aequus/Content/CursorDyes/Items/SwordCursor");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_Smart
        /// </summary>
        public static readonly TextureAsset SwordCursor_Smart = new TextureAsset("Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor_Smart");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor
        /// </summary>
        public static readonly TextureAsset SwordCursor_SwordCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/SwordCursor/SwordCursor");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/CrownOfBlood/Symbol_Blood
        /// </summary>
        public static readonly TextureAsset Symbol_Blood = new TextureAsset("Aequus/Items/Accessories/CrownOfBlood/Symbol_Blood");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/CrownOfBlood/Symbol_Remains
        /// </summary>
        public static readonly TextureAsset Symbol_Remains = new TextureAsset("Aequus/Items/Accessories/CrownOfBlood/Symbol_Remains");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Misc/TatteredDemonHorn
        /// </summary>
        public static readonly TextureAsset TatteredDemonHorn = new TextureAsset("Aequus/Content/Fishing/Misc/TatteredDemonHorn");
        /// <summary>
        /// Full Path: Aequus/Assets/Textures/TerraSlash
        /// </summary>
        public static readonly TextureAsset TerraSlash = new TextureAsset("Aequus/Assets/Textures/TerraSlash");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/TheReconstruction
        /// </summary>
        public static readonly TextureAsset TheReconstruction = new TextureAsset("Aequus/Unused/Items/TheReconstruction");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/CrownOfBlood/Projectiles/ThermiteGel
        /// </summary>
        public static readonly TextureAsset ThermiteGel = new TextureAsset("Aequus/Items/Accessories/CrownOfBlood/Projectiles/ThermiteGel");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/Misc/ThunderClap
        /// </summary>
        public static readonly TextureAsset ThunderClap = new TextureAsset("Aequus/Items/Weapons/Melee/Misc/ThunderClap");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Melee/ThunderClapProj
        /// </summary>
        public static readonly TextureAsset ThunderClapProj = new TextureAsset("Aequus/Projectiles/Melee/ThunderClapProj");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Melee/ThunderClapProjChain
        /// </summary>
        public static readonly TextureAsset ThunderClapProjChain = new TextureAsset("Aequus/Projectiles/Melee/ThunderClapProjChain");
        /// <summary>
        /// Full Path: Aequus/Items/Misc/Dyes/TidalDye
        /// </summary>
        public static readonly TextureAsset TidalDye_Dyes = new TextureAsset("Aequus/Items/Misc/Dyes/TidalDye");
        /// <summary>
        /// Full Path: Aequus/Assets/Effects/Textures/TidalDye
        /// </summary>
        public static readonly TextureAsset TidalDye_Textures = new TextureAsset("Aequus/Assets/Effects/Textures/TidalDye");
        /// <summary>
        /// Full Path: Aequus/Items/Consumables/Permanent/TinkerersGuidebook
        /// </summary>
        public static readonly TextureAsset TinkerersGuidebook = new TextureAsset("Aequus/Items/Consumables/Permanent/TinkerersGuidebook");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/TonicSpawnratesBuff
        /// </summary>
        public static readonly TextureAsset TonicSpawnratesBuff = new TextureAsset("Aequus/Buffs/Misc/TonicSpawnratesBuff");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/TonicSpawnratesDebuff
        /// </summary>
        public static readonly TextureAsset TonicSpawnratesDebuff = new TextureAsset("Aequus/Buffs/Misc/TonicSpawnratesDebuff");
        /// <summary>
        /// Full Path: Aequus/Tiles/Misc/BigGems/TopazDeposit
        /// </summary>
        public static readonly TextureAsset TopazDeposit = new TextureAsset("Aequus/Tiles/Misc/BigGems/TopazDeposit");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Gems/TopazGore
        /// </summary>
        public static readonly TextureAsset TopazGore = new TextureAsset("Aequus/Assets/Gores/Gems/TopazGore");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/DustDevil/Misc/TornadoInABottle
        /// </summary>
        public static readonly TextureAsset TornadoInABottle = new TextureAsset("Aequus/Content/Boss/DustDevil/Misc/TornadoInABottle");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Summon/Whip/TornadoWhip
        /// </summary>
        public static readonly TextureAsset TornadoWhip = new TextureAsset("Aequus/Items/Weapons/Summon/Whip/TornadoWhip");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/Whip/TornadoWhipProj
        /// </summary>
        public static readonly TextureAsset TornadoWhipProj = new TextureAsset("Aequus/Projectiles/Summon/Whip/TornadoWhipProj");
        /// <summary>
        /// Full Path: Aequus/Buffs/Pets/TorraBuff
        /// </summary>
        public static readonly TextureAsset TorraBuff = new TextureAsset("Aequus/Buffs/Pets/TorraBuff");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Misc/Pets/TorraPet
        /// </summary>
        public static readonly TextureAsset TorraPet = new TextureAsset("Aequus/Projectiles/Misc/Pets/TorraPet");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Magic/TouhouBullet
        /// </summary>
        public static readonly TextureAsset TouhouBullet = new TextureAsset("Aequus/Projectiles/Magic/TouhouBullet");
        /// <summary>
        /// Full Path: Aequus/Assets/TownNPCExclamation
        /// </summary>
        public static readonly TextureAsset TownNPCExclamation = new TextureAsset("Aequus/Assets/TownNPCExclamation");
        /// <summary>
        /// Full Path: Aequus/Items/Vanity/Pets/ToySpaceGun
        /// </summary>
        public static readonly TextureAsset ToySpaceGun = new TextureAsset("Aequus/Items/Vanity/Pets/ToySpaceGun");
        /// <summary>
        /// Full Path: Aequus/Assets/Effects/Prims/Trail0
        /// </summary>
        public static readonly TextureAsset Trail0 = new TextureAsset("Aequus/Assets/Effects/Prims/Trail0");
        /// <summary>
        /// Full Path: Aequus/Assets/Effects/Prims/Trail1
        /// </summary>
        public static readonly TextureAsset Trail1 = new TextureAsset("Aequus/Assets/Effects/Prims/Trail1");
        /// <summary>
        /// Full Path: Aequus/Assets/Effects/Prims/Trail2
        /// </summary>
        public static readonly TextureAsset Trail2 = new TextureAsset("Aequus/Assets/Effects/Prims/Trail2");
        /// <summary>
        /// Full Path: Aequus/Assets/Effects/Prims/Trail3
        /// </summary>
        public static readonly TextureAsset Trail3 = new TextureAsset("Aequus/Assets/Effects/Prims/Trail3");
        /// <summary>
        /// Full Path: Aequus/Assets/Effects/Prims/Trail4
        /// </summary>
        public static readonly TextureAsset Trail4 = new TextureAsset("Aequus/Assets/Effects/Prims/Trail4");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Underworld/Trapper
        /// </summary>
        public static readonly TextureAsset Trapper = new TextureAsset("Aequus/NPCs/Monsters/Underworld/Trapper");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Trapper_0
        /// </summary>
        public static readonly TextureAsset Trapper_0 = new TextureAsset("Aequus/Assets/Gores/Trapper_0");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Trapper_1
        /// </summary>
        public static readonly TextureAsset Trapper_1 = new TextureAsset("Aequus/Assets/Gores/Trapper_1");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Trapper_2
        /// </summary>
        public static readonly TextureAsset Trapper_2 = new TextureAsset("Aequus/Assets/Gores/Trapper_2");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Trapper_3
        /// </summary>
        public static readonly TextureAsset Trapper_3 = new TextureAsset("Aequus/Assets/Gores/Trapper_3");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/Trapper_4
        /// </summary>
        public static readonly TextureAsset Trapper_4 = new TextureAsset("Aequus/Assets/Gores/Trapper_4");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Underworld/Trapper_Chain
        /// </summary>
        public static readonly TextureAsset Trapper_Chain = new TextureAsset("Aequus/NPCs/Monsters/Underworld/Trapper_Chain");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Monster/TrapperBullet
        /// </summary>
        public static readonly TextureAsset TrapperBullet = new TextureAsset("Aequus/Projectiles/Monster/TrapperBullet");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Underworld/TrapperImp
        /// </summary>
        public static readonly TextureAsset TrapperImp = new TextureAsset("Aequus/NPCs/Monsters/Underworld/TrapperImp");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/TrapperImp_0
        /// </summary>
        public static readonly TextureAsset TrapperImp_0 = new TextureAsset("Aequus/Assets/Gores/TrapperImp_0");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/TrapperImp_1
        /// </summary>
        public static readonly TextureAsset TrapperImp_1 = new TextureAsset("Aequus/Assets/Gores/TrapperImp_1");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/TrapperImp_2
        /// </summary>
        public static readonly TextureAsset TrapperImp_2 = new TextureAsset("Aequus/Assets/Gores/TrapperImp_2");
        /// <summary>
        /// Full Path: Aequus/Assets/Gores/TrapperImp_3
        /// </summary>
        public static readonly TextureAsset TrapperImp_3 = new TextureAsset("Aequus/Assets/Gores/TrapperImp_3");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Underworld/TrapperImp_Glow
        /// </summary>
        public static readonly TextureAsset TrapperImp_Glow = new TextureAsset("Aequus/NPCs/Monsters/Underworld/TrapperImp_Glow");
        /// <summary>
        /// Full Path: Aequus/Tiles/Banners/Items/TrapperImpBanner
        /// </summary>
        public static readonly TextureAsset TrapperImpBanner = new TextureAsset("Aequus/Tiles/Banners/Items/TrapperImpBanner");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Underworld/TrapperImpTail
        /// </summary>
        public static readonly TextureAsset TrapperImpTail = new TextureAsset("Aequus/NPCs/Monsters/Underworld/TrapperImpTail");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Underworld/TrapperImpWings
        /// </summary>
        public static readonly TextureAsset TrapperImpWings = new TextureAsset("Aequus/NPCs/Monsters/Underworld/TrapperImpWings");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Underground/TrapSkeleton
        /// </summary>
        public static readonly TextureAsset TrapSkeleton = new TextureAsset("Aequus/NPCs/Monsters/Underground/TrapSkeleton");
        /// <summary>
        /// Full Path: Aequus/Tiles/Banners/Items/TrapSkeletonBanner
        /// </summary>
        public static readonly TextureAsset TrapSkeletonBanner = new TextureAsset("Aequus/Tiles/Banners/Items/TrapSkeletonBanner");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Magic/Triacanthorn
        /// </summary>
        public static readonly TextureAsset Triacanthorn = new TextureAsset("Aequus/Items/Weapons/Magic/Triacanthorn");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Magic/Triacanthorn_Glow
        /// </summary>
        public static readonly TextureAsset Triacanthorn_Glow = new TextureAsset("Aequus/Items/Weapons/Magic/Triacanthorn_Glow");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Magic/TriacanthornBolt
        /// </summary>
        public static readonly TextureAsset TriacanthornBolt = new TextureAsset("Aequus/Projectiles/Magic/TriacanthornBolt");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Magic/TriacanthornProj
        /// </summary>
        public static readonly TextureAsset TriacanthornProj = new TextureAsset("Aequus/Projectiles/Magic/TriacanthornProj");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Trophies
        /// </summary>
        public static readonly TextureAsset Trophies = new TextureAsset("Aequus/Content/Boss/Trophies");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/Trophy
        /// </summary>
        public static readonly TextureAsset Trophy = new TextureAsset("Aequus/Content/Boss/Trophy");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/TubOfCookieDough
        /// </summary>
        public static readonly TextureAsset TubOfCookieDough = new TextureAsset("Aequus/Unused/Items/TubOfCookieDough");
        /// <summary>
        /// Full Path: Aequus/Items/Materials/Energies/UltimateEnergy
        /// </summary>
        public static readonly TextureAsset UltimateEnergy = new TextureAsset("Aequus/Items/Materials/Energies/UltimateEnergy");
        /// <summary>
        /// Full Path: Aequus/Assets/Effects/Textures/UltimateEnergyGradient
        /// </summary>
        public static readonly TextureAsset UltimateEnergyGradient = new TextureAsset("Aequus/Assets/Effects/Textures/UltimateEnergyGradient");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/Heavy/UltimateSword
        /// </summary>
        public static readonly TextureAsset UltimateSword = new TextureAsset("Aequus/Items/Weapons/Melee/Heavy/UltimateSword");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/Heavy/UltimateSword_Glow
        /// </summary>
        public static readonly TextureAsset UltimateSword_Glow = new TextureAsset("Aequus/Items/Weapons/Melee/Heavy/UltimateSword_Glow");
        /// <summary>
        /// Full Path: Aequus/Buffs/Misc/UltimateSwordBuff
        /// </summary>
        public static readonly TextureAsset UltimateSwordBuff = new TextureAsset("Aequus/Buffs/Misc/UltimateSwordBuff");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Melee/Swords/UltimateSwordProj
        /// </summary>
        public static readonly TextureAsset UltimateSwordProj = new TextureAsset("Aequus/Projectiles/Melee/Swords/UltimateSwordProj");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Melee/Swords/UltimateSwordProj_Glow
        /// </summary>
        public static readonly TextureAsset UltimateSwordProj_Glow = new TextureAsset("Aequus/Projectiles/Melee/Swords/UltimateSwordProj_Glow");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/BossChecklist/UltraStarite
        /// </summary>
        public static readonly TextureAsset UltraStarite_BossChecklist = new TextureAsset("Aequus/Assets/UI/BossChecklist/UltraStarite");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/UltraStariteMiniboss/UltraStarite_Head_Boss
        /// </summary>
        public static readonly TextureAsset UltraStarite_Head_Boss = new TextureAsset("Aequus/Content/Boss/UltraStariteMiniboss/UltraStarite_Head_Boss");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/UltraStariteMiniboss/UltraStarite
        /// </summary>
        public static readonly TextureAsset UltraStarite_UltraStariteMiniboss = new TextureAsset("Aequus/Content/Boss/UltraStariteMiniboss/UltraStarite");
        /// <summary>
        /// Full Path: Aequus/Tiles/Banners/Items/UltraStariteBanner
        /// </summary>
        public static readonly TextureAsset UltraStariteBanner = new TextureAsset("Aequus/Tiles/Banners/Items/UltraStariteBanner");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/UltraStariteMiniboss/Rewards/UltraStariteRelic
        /// </summary>
        public static readonly TextureAsset UltraStariteRelic = new TextureAsset("Aequus/Content/Boss/UltraStariteMiniboss/Rewards/UltraStariteRelic");
        /// <summary>
        /// Full Path: Aequus/Content/Boss/UltraStariteMiniboss/Rewards/UltraStariteTrophy
        /// </summary>
        public static readonly TextureAsset UltraStariteTrophy = new TextureAsset("Aequus/Content/Boss/UltraStariteMiniboss/Rewards/UltraStariteTrophy");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Magic/Umystick
        /// </summary>
        public static readonly TextureAsset Umystick = new TextureAsset("Aequus/Items/Weapons/Magic/Umystick");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Magic/UmystickBullet
        /// </summary>
        public static readonly TextureAsset UmystickBullet = new TextureAsset("Aequus/Projectiles/Magic/UmystickBullet");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Magic/UmystickProj
        /// </summary>
        public static readonly TextureAsset UmystickProj = new TextureAsset("Aequus/Projectiles/Magic/UmystickProj");
        /// <summary>
        /// Full Path: Aequus/Content/Events/DemonSiege/Misc/UnholyCore
        /// </summary>
        public static readonly TextureAsset UnholyCore = new TextureAsset("Aequus/Content/Events/DemonSiege/Misc/UnholyCore");
        /// <summary>
        /// Full Path: Aequus/Content/Events/DemonSiege/Misc/UnholyCoreSmall
        /// </summary>
        public static readonly TextureAsset UnholyCoreSmall = new TextureAsset("Aequus/Content/Events/DemonSiege/Misc/UnholyCoreSmall");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/Thrown/Valari
        /// </summary>
        public static readonly TextureAsset Valari = new TextureAsset("Aequus/Items/Weapons/Melee/Thrown/Valari");
        /// <summary>
        /// Full Path: Aequus/Content/Vampirism/Items/VampireSquid
        /// </summary>
        public static readonly TextureAsset VampireSquid = new TextureAsset("Aequus/Content/Vampirism/Items/VampireSquid");
        /// <summary>
        /// Full Path: Aequus/Content/Vampirism/Buffs/VampirismBuff
        /// </summary>
        public static readonly TextureAsset VampirismBuff = new TextureAsset("Aequus/Content/Vampirism/Buffs/VampirismBuff");
        /// <summary>
        /// Full Path: Aequus/Content/Vampirism/Buffs/VampirismDay
        /// </summary>
        public static readonly TextureAsset VampirismDay = new TextureAsset("Aequus/Content/Vampirism/Buffs/VampirismDay");
        /// <summary>
        /// Full Path: Aequus/Content/Vampirism/Buffs/VampirismDayRain
        /// </summary>
        public static readonly TextureAsset VampirismDayRain = new TextureAsset("Aequus/Content/Vampirism/Buffs/VampirismDayRain");
        /// <summary>
        /// Full Path: Aequus/Content/Vampirism/Buffs/VampirismNight
        /// </summary>
        public static readonly TextureAsset VampirismNight = new TextureAsset("Aequus/Content/Vampirism/Buffs/VampirismNight");
        /// <summary>
        /// Full Path: Aequus/Content/Vampirism/Buffs/VampirismNightEclipse
        /// </summary>
        public static readonly TextureAsset VampirismNightEclipse = new TextureAsset("Aequus/Content/Vampirism/Buffs/VampirismNightEclipse");
        /// <summary>
        /// Full Path: Aequus/Buffs/VeinminerBuff
        /// </summary>
        public static readonly TextureAsset VeinminerBuff = new TextureAsset("Aequus/Buffs/VeinminerBuff");
        /// <summary>
        /// Full Path: Aequus/Buffs/VeinminerBuffEmpowered
        /// </summary>
        public static readonly TextureAsset VeinminerBuffEmpowered = new TextureAsset("Aequus/Buffs/VeinminerBuffEmpowered");
        /// <summary>
        /// Full Path: Aequus/Items/Potions/VeinminerPotion
        /// </summary>
        public static readonly TextureAsset VeinminerPotion = new TextureAsset("Aequus/Items/Potions/VeinminerPotion");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetTrap/VenomDartTrapHat
        /// </summary>
        public static readonly TextureAsset VenomDartTrapHat = new TextureAsset("Aequus/Items/Armor/SetTrap/VenomDartTrapHat");
        /// <summary>
        /// Full Path: Aequus/Items/Armor/SetTrap/VenomDartTrapHat_Head
        /// </summary>
        public static readonly TextureAsset VenomDartTrapHat_Head = new TextureAsset("Aequus/Items/Armor/SetTrap/VenomDartTrapHat_Head");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/Misc/VenomDartTrapHatProj
        /// </summary>
        public static readonly TextureAsset VenomDartTrapHatProj = new TextureAsset("Aequus/Projectiles/Summon/Misc/VenomDartTrapHatProj");
        /// <summary>
        /// Full Path: Aequus/Items/Consumables/Permanent/VictorsReward
        /// </summary>
        public static readonly TextureAsset VictorsReward = new TextureAsset("Aequus/Items/Consumables/Permanent/VictorsReward");
        /// <summary>
        /// Full Path: Aequus/Assets/VignetteSmall
        /// </summary>
        public static readonly TextureAsset VignetteSmall = new TextureAsset("Aequus/Assets/VignetteSmall");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Jungle/Might/Vineroot
        /// </summary>
        public static readonly TextureAsset Vineroot = new TextureAsset("Aequus/NPCs/Monsters/Jungle/Might/Vineroot");
        /// <summary>
        /// Full Path: Aequus/Particles/Dusts/VoidDust
        /// </summary>
        public static readonly TextureAsset VoidDust = new TextureAsset("Aequus/Particles/Dusts/VoidDust");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Sky/GaleStreams/Vraine
        /// </summary>
        public static readonly TextureAsset Vraine = new TextureAsset("Aequus/NPCs/Monsters/Sky/GaleStreams/Vraine");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Sky/GaleStreams/Vraine_Cold
        /// </summary>
        public static readonly TextureAsset Vraine_Cold = new TextureAsset("Aequus/NPCs/Monsters/Sky/GaleStreams/Vraine_Cold");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Sky/GaleStreams/Vraine_Hot
        /// </summary>
        public static readonly TextureAsset Vraine_Hot = new TextureAsset("Aequus/NPCs/Monsters/Sky/GaleStreams/Vraine_Hot");
        /// <summary>
        /// Full Path: Aequus/Tiles/Banners/Items/VraineBanner
        /// </summary>
        public static readonly TextureAsset VraineBanner = new TextureAsset("Aequus/Tiles/Banners/Items/VraineBanner");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Melee/Thrown/Vrang
        /// </summary>
        public static readonly TextureAsset Vrang = new TextureAsset("Aequus/Items/Weapons/Melee/Thrown/Vrang");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Melee/VrangProj
        /// </summary>
        public static readonly TextureAsset VrangProj = new TextureAsset("Aequus/Projectiles/Melee/VrangProj");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Melee/VrangProj_Cold
        /// </summary>
        public static readonly TextureAsset VrangProj_Cold = new TextureAsset("Aequus/Projectiles/Melee/VrangProj_Cold");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Melee/VrangProj_Hot
        /// </summary>
        public static readonly TextureAsset VrangProj_Hot = new TextureAsset("Aequus/Projectiles/Melee/VrangProj_Hot");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Magic/Misc/Wabbajack
        /// </summary>
        public static readonly TextureAsset Wabbajack = new TextureAsset("Aequus/Items/Weapons/Magic/Misc/Wabbajack");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Magic/WabbajackProj
        /// </summary>
        public static readonly TextureAsset WabbajackProj = new TextureAsset("Aequus/Projectiles/Magic/WabbajackProj");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/WallClocks
        /// </summary>
        public static readonly TextureAsset WallClocks = new TextureAsset("Aequus/Tiles/Furniture/WallClocks");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/WallClocks_Highlight
        /// </summary>
        public static readonly TextureAsset WallClocks_Highlight = new TextureAsset("Aequus/Tiles/Furniture/WallClocks_Highlight");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Paintings/WallPaintings
        /// </summary>
        public static readonly TextureAsset WallPaintings = new TextureAsset("Aequus/Tiles/Furniture/Paintings/WallPaintings");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Paintings/WallPaintings2x2
        /// </summary>
        public static readonly TextureAsset WallPaintings2x2 = new TextureAsset("Aequus/Tiles/Furniture/Paintings/WallPaintings2x2");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Paintings/WallPaintings3x2
        /// </summary>
        public static readonly TextureAsset WallPaintings3x2 = new TextureAsset("Aequus/Tiles/Furniture/Paintings/WallPaintings3x2");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Paintings/WallPaintings6x4
        /// </summary>
        public static readonly TextureAsset WallPaintings6x4 = new TextureAsset("Aequus/Tiles/Furniture/Paintings/WallPaintings6x4");
        /// <summary>
        /// Full Path: Aequus/Items/Accessories/Offense/Summon/WarHorn
        /// </summary>
        public static readonly TextureAsset WarHorn = new TextureAsset("Aequus/Items/Accessories/Offense/Summon/WarHorn");
        /// <summary>
        /// Full Path: Aequus/Content/Town/CarpenterNPC/Rewards/WhiteFlag
        /// </summary>
        public static readonly TextureAsset WhiteFlag = new TextureAsset("Aequus/Content/Town/CarpenterNPC/Rewards/WhiteFlag");
        /// <summary>
        /// Full Path: Aequus/Items/Consumables/Permanent/WhitePhial
        /// </summary>
        public static readonly TextureAsset WhitePhial = new TextureAsset("Aequus/Items/Consumables/Permanent/WhitePhial");
        /// <summary>
        /// Full Path: Aequus/NPCs/Monsters/Sky/GaleStreams/WhiteSlime
        /// </summary>
        public static readonly TextureAsset WhiteSlime = new TextureAsset("Aequus/NPCs/Monsters/Sky/GaleStreams/WhiteSlime");
        /// <summary>
        /// Full Path: Aequus/Tiles/Banners/Items/WhiteSlimeBanner
        /// </summary>
        public static readonly TextureAsset WhiteSlimeBanner = new TextureAsset("Aequus/Tiles/Banners/Items/WhiteSlimeBanner");
        /// <summary>
        /// Full Path: Aequus/Assets/Wind
        /// </summary>
        public static readonly TextureAsset Wind = new TextureAsset("Aequus/Assets/Wind");
        /// <summary>
        /// Full Path: Aequus/Assets/Wind2
        /// </summary>
        public static readonly TextureAsset Wind2 = new TextureAsset("Aequus/Assets/Wind2");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Magic/Misc/WindFan
        /// </summary>
        public static readonly TextureAsset WindFan = new TextureAsset("Aequus/Items/Weapons/Magic/Misc/WindFan");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/Bait/XenonBait
        /// </summary>
        public static readonly TextureAsset XenonBait = new TextureAsset("Aequus/Content/Fishing/Bait/XenonBait");
        /// <summary>
        /// Full Path: Aequus/Content/Fishing/LegendaryFish/XenonFish
        /// </summary>
        public static readonly TextureAsset XenonFish = new TextureAsset("Aequus/Content/Fishing/LegendaryFish/XenonFish");
        /// <summary>
        /// Full Path: Aequus/Content/Elites/Misc/XenonSpore
        /// </summary>
        public static readonly TextureAsset XenonSpore = new TextureAsset("Aequus/Content/Elites/Misc/XenonSpore");
        /// <summary>
        /// Full Path: Aequus/Content/Biomes/MossBiomes/Tiles/XenonSummonPlant
        /// </summary>
        public static readonly TextureAsset XenonSummonPlant = new TextureAsset("Aequus/Content/Biomes/MossBiomes/Tiles/XenonSummonPlant");
        /// <summary>
        /// Full Path: Aequus/Content/CursorDyes/Items/XmasCursor
        /// </summary>
        public static readonly TextureAsset XmasCursor = new TextureAsset("Aequus/Content/CursorDyes/Items/XmasCursor");
        /// <summary>
        /// Full Path: Aequus/Unused/Items/XmasEnergy
        /// </summary>
        public static readonly TextureAsset XmasEnergy = new TextureAsset("Aequus/Unused/Items/XmasEnergy");
        /// <summary>
        /// Full Path: Aequus/Assets/Effects/Textures/XmasEnergyGradient
        /// </summary>
        public static readonly TextureAsset XmasEnergyGradient = new TextureAsset("Aequus/Assets/Effects/Textures/XmasEnergyGradient");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Paintings/Items/YangPainting
        /// </summary>
        public static readonly TextureAsset YangPainting = new TextureAsset("Aequus/Tiles/Furniture/Paintings/Items/YangPainting");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Paintings/Items/YinPainting
        /// </summary>
        public static readonly TextureAsset YinPainting = new TextureAsset("Aequus/Tiles/Furniture/Paintings/Items/YinPainting");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Paintings/Items/YinYangPainting
        /// </summary>
        public static readonly TextureAsset YinYangPainting = new TextureAsset("Aequus/Tiles/Furniture/Paintings/Items/YinYangPainting");
        /// <summary>
        /// Full Path: Aequus/Tiles/Furniture/Paintings/Items/YinYangXmasPainting
        /// </summary>
        public static readonly TextureAsset YinYangXmasPainting = new TextureAsset("Aequus/Tiles/Furniture/Paintings/Items/YinYangXmasPainting");
        /// <summary>
        /// Full Path: Aequus/Projectiles/Summon/Necro/ZombieBolt
        /// </summary>
        public static readonly TextureAsset ZombieBolt = new TextureAsset("Aequus/Projectiles/Summon/Necro/ZombieBolt");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Scepters/ZombieScepter
        /// </summary>
        public static readonly TextureAsset ZombieScepter = new TextureAsset("Aequus/Items/Weapons/Necromancy/Scepters/ZombieScepter");
        /// <summary>
        /// Full Path: Aequus/Items/Weapons/Necromancy/Scepters/ZombieScepter_Glow
        /// </summary>
        public static readonly TextureAsset ZombieScepter_Glow = new TextureAsset("Aequus/Items/Weapons/Necromancy/Scepters/ZombieScepter_Glow");
        /// <summary>
        /// Full Path: Aequus/Assets/UI/ZoologistAltHead
        /// </summary>
        public static readonly TextureAsset ZoologistAltHead = new TextureAsset("Aequus/Assets/UI/ZoologistAltHead");
    }
}
#endif