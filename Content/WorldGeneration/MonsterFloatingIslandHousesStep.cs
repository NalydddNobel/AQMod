using Aequus.Common.WorldGeneration;
using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Aequus.Content.WorldGeneration;
public class MonsterFloatingIslandHousesStep : AequusGenStep {
    public override string InsertAfter => "Floating Island Houses";

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        for (int i = 0; i < MonsterFloatingIslandsStep.IslandHouses.Count; i++) {
            WorldGen.IslandHouse(MonsterFloatingIslandsStep.IslandHouses[i].X, MonsterFloatingIslandsStep.IslandHouses[i].Y, MonsterFloatingIslandsStep.IslandHouses[i].Style);
        }
    }
}