using Aequus.Buffs.Buildings;
using Aequus.Content.Town.CarpenterNPC.Quest.Bounties.Steps;
using Aequus.Items.Misc.Carpentry.Rewards;
using Aequus.Items.Placeable.Furniture.Interactable;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria.ID;

namespace Aequus.Content.Town.CarpenterNPC.Quest.Bounties
{
    public class BountiesLoader
    {
        public static int ActuatorDoorBountyID { get; private set; }
        public static int PondBridgeBountyID { get; private set; }
        public static int BiomePaletteBountyID { get; private set; }
        public static int PirateShipBountyID { get; private set; }
        public static int FountainBountyID { get; private set; }

        private static void ActuatorBounty()
        {
            ActuatorDoorBountyID = new CarpenterBounty("ActuatorDoorBounty")
                .SetReward<PixelCamera>()
                .AddMiscUnlock<PixelCameraClipAmmo>()
                .SetBuff<SecretEntranceBuff>()
                .SetNPC(NPCID.Mechanic)
                .SetProgression(BountyProgression.Mechanic)
                .AddStep(new FindHousesStep(minHouses: 1)
                    .AfterSuccess((i, s) =>
                    {
                        var houses = i.GetInterest<FindHousesStep.Interest>();
                        i.GetInterest<ActuatorDoorStep.Interest>().givenHouses = houses.housingWalls;
                        var mess = houses.housingWalls.Count > 0 ? houses.housingWalls.Values.First() : new List<Point>() { new Point(0, 0), };
                        var r = CarpenterSystem.TurnPointMessIntoRectangleBounds(mess);
                        r.Inflate(1, 1);
                        i.GetInterest<CraftableTilesStep.Interest>().givenRectangle = r;
                    }))
                .AddStep(new ActuatorDoorStep())
                .AddStep(new CraftableTilesStep(minTiles: 12, ratioTiles: 0.5f))
                .Register();
        }
        private static void PondBridgeBounty()
        {
            PondBridgeBountyID = new CarpenterBounty("PondBridgeBounty")
                .SetReward<FishSign>()
                .SetBuff<BridgeBountyBuff>()
                .SetNPC(NPCID.Merchant)
                .SetProgression(BountyProgression.Merchant)
                .AddStep(new FindBridgeStep(waterTilesNeeded: 50, waterHeightNeeded: 4, liquidIDWanted: LiquidID.Water, bridgeLengthWanted: 12)
                    .AfterSuccess((i, s) =>
                    {
                        var bridge = i.GetInterest<FindBridgeStep.Interest>();
                        var dict = new Dictionary<Point, List<Point>>() { [new Point(bridge.bridgeLocation.X, bridge.bridgeLocation.Y)] = CarpenterSystem.TurnRectangleIntoUnoptimizedPointMess(bridge.bridgeLocation) };
                        i.GetInterest<FurnitureCountStep.Interest>().givenHouses = dict;
                        i.GetInterest<CraftableTilesStep.Interest>().givenRectangle = bridge.bridgeLocation;
                    }))
                .AddStep(new FurnitureCountStep(minFurniture: 8))
                .AddStep(new CraftableTilesStep(minTiles: 12, ratioTiles: 0f))
                .Register();
        }
        private static void PaletteBounty()
        {
            BiomePaletteBountyID = new CarpenterBounty("BiomePaletteBounty")
                .SetReward<OmniPaint>()
                .SetBuff<PaletteBountyBuff>()
                .SetNPC(NPCID.DyeTrader)
                .SetProgression(BountyProgression.DyeTrader)
                .AddStep(new FindHousesStep(minHouses: 1)
                    .AfterSuccess((i, s) =>
                    {
                        var houses = i.GetInterest<FindHousesStep.Interest>();
                        i.GetInterest<BiomePaletteStep.Interest>().givenHouses = houses.housingWalls;
                        i.GetInterest<FurnitureCountStep.Interest>().givenHouses = houses.housingWalls;
                    }))
                .AddStep(new FurnitureCountStep(minFurniture: 5))
                .AddStep(new BiomePaletteStep(minCredit: 0.5f))
                .Register();
        }
        private static void PirateShipBounty()
        {
            PirateShipBountyID = new CarpenterBounty("PirateShipBounty")
                .SetReward<WhiteFlag>()
                .SetBuff<PirateBountyBuff>()
                .SetNPC(NPCID.Pirate)
                .SetProgression(BountyProgression.Pirate)
                .AddStep(new FindHousesStep(minHouses: 2)
                    .AfterSuccess((i, s) =>
                    {
                        var houses = i.GetInterest<FindHousesStep.Interest>();
                        i.GetInterest<WaterLineStep.Interest>().givenHouses = houses.housingWalls;
                        i.GetInterest<FurnitureCountStep.Interest>().givenHouses = houses.housingWalls;
                    }))
                .AddStep(new WaterLineStep(minWaterLine: 5))
                .AddStep(new FurnitureCountStep(minFurniture: 15))
                .Register();
        }
        private static void FountainBounty()
        {
            FountainBountyID = new CarpenterBounty("FountainBounty")
                .SetReward<AdvancedRuler>()
                .SetBuff<FountainBountyBuff>()
                .SetNPC(NPCID.Guide)
                .SetProgression(BountyProgression.Guide)
                .AddStep(new WaterfallSearchStep(liquidWanted: LiquidID.Water)
                    .AfterSuccess((i, s) =>
                        i.GetInterest<CraftableTilesStep.Interest>().givenRectangle = i.GetInterest<WaterfallSearchStep.WaterfallInterest>().resultRectangle))
                .AddStep(new WaterfallHeightStep(minHeight: 7))
                .AddStep(new CraftableTilesStep(minTiles: 12, ratioTiles: 0f)
                    .AfterSuccess((i, s) =>
                    {
                        var crafted = i.GetInterest<CraftableTilesStep.Interest>();
                        var symmetric = i.GetInterest<SymmetricHorizontalStep.Interest>();
                        symmetric.givenRectangle = crafted.resultRectangle;
                        symmetric.givenPoints = crafted.craftableTiles;
                    }))
                .AddStep(new SymmetricHorizontalStep())
                .Register();
        }

        internal static void SetupBounties()
        {
            FountainBounty();
            PirateShipBounty();
            PaletteBounty();
            PondBridgeBounty();
            ActuatorBounty();
        }
    }
}
