using Aequus.Core.DataSets;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aequus.Content.DataSets;

public class TileMetadata : MetadataSet {
    [JsonProperty]
    public static HashSet<Entry<TileID>> PhysicsGunCannotPickUp { get; private set; } = new();
    [JsonProperty]
    public static HashSet<Entry<TileID>> PhysicsGunBlocksLaser { get; private set; } = new();
    [JsonProperty]
    public static HashSet<Entry<TileID>> IsSmashablePot { get; private set; } = new();
    [JsonProperty]
    public static Dictionary<Entry<TileID>, Dictionary<int, Color>> PylonColors { get; private set; } = new();
    [JsonIgnore]
    public static HashSet<ushort> Convertible { get; private set; } = new();
    /// <summary>Prevents tiles below this tile from being sloped.</summary>
    [JsonIgnore]
    public static HashSet<ushort> PreventsSlopesBelow { get; private set; } = new();

    public override void AddRecipes() {
        for (ushort i = 0; i < (ushort)TileLoader.TileCount; i++) {
            if (TileID.Sets.Conversion.Dirt[i] 
                || TileID.Sets.Conversion.GolfGrass[i]
                || TileID.Sets.Conversion.Grass[i]
                || TileID.Sets.Conversion.HardenedSand[i]
                || TileID.Sets.Conversion.Ice[i]
                || TileID.Sets.Conversion.JungleGrass[i]
                || TileID.Sets.Conversion.Moss[i]
                || TileID.Sets.Conversion.MushroomGrass[i]
                || TileID.Sets.Conversion.Sand[i]
                || TileID.Sets.Conversion.Sandstone[i]
                || TileID.Sets.Conversion.Snow[i]
                || TileID.Sets.Conversion.Stone[i]
                || TileID.Sets.Conversion.Thorn[i]) {
                Convertible.Add(i);
            }
        }
    }
    /*
    "CalamityMod/AstralPylonTile": { "0": "143, 0, 255, 255" },
    "CalamityMod/CragsPylonTile": { "0": "255, 200, 0, 255" },
    "CalamityMod/SunkenPylonTile": { "0": "0, 100, 255, 255" },
    "CalamityMod/SulphurPylonTile": { "0": "120, 190, 33, 255" },
    "ExtraPylons/ChlorophytePylon": { "0": "53, 224, 31, 255" },
    "ExtraPylons/CloudPylon": { "0": "233, 255, 255, 255" },
    "ExtraPylons/CoralPylon": { "0": "215, 40, 95, 255" },
    "ExtraPylons/CorruptionPylon": { "0": "138, 75, 180, 255" },
    "ExtraPylons/CrimsonPylon": { "0": "209, 46, 46, 255" },
    "ExtraPylons/DungeonPylon": { "0": "172, 83, 162, 255" },
    "ExtraPylons/GemPylon": { "0": "106, 205, 236, 255" },
    "ExtraPylons/GlowingMossPylon": { "0": "133, 151, 239, 255" },
    "ExtraPylons/GranitePylon": { "0": "79, 98, 191, 255" },
    "ExtraPylons/MarblePylon": { "0": "233, 255, 233, 255" },
    "ExtraPylons/GravePylon": { "0": "255, 233, 233, 255" },
    "ExtraPylons/HoneyPylon": { "0": "255, 255, 0, 255" },
    "ExtraPylons/IcePylon": { "0": "90, 132, 196, 255" },
    "ExtraPylons/LivingPylon": { "0": "95, 56, 17, 255" },
    "ExtraPylons/LivingMahoganyPylon": { "0": "95, 74, 17, 255" },
    "ExtraPylons/OasisPylon": { "0": "143, 234, 255, 255" },
    "ExtraPylons/SpacePylon": { "0": "156, 143, 255, 255" },
    "ExtraPylons/SpiderPylon": { "0": "233, 143, 255, 255" },
    "ExtraPylons/TemplePylon": { "0": "219, 77, 0, 255" },
    "ExtraPylons/UnderworldPylon": { "0": "255, 65, 36, 255" },
    "Verdant/VerdantPylonTile": { "0": "189, 66, 162, 255" },
    "Fargowiltas/SiblingPylonTile": { "0": "0, 82, 44, 255" },
    "Spooky/SpookyBiomePylon": { "0": "255, 150, 30, 255" },
    "Spooky/SpookyHellPylon": { "0": "145, 30, 255, 255" }
     */
    /*
        "(TeleportationPylon, 0)": "100, 255, 128",
        "(TeleportationPylon, 1)": "200, 255, 65",
        "(TeleportationPylon, 2)": "248, 106, 255",
        "(TeleportationPylon, 3)": "148, 118, 213",
        "(TeleportationPylon, 4)": "0, 181, 226",
        "(TeleportationPylon, 5)": "255, 222, 120",
        "(TeleportationPylon, 6)": "120, 222, 255",
        "(TeleportationPylon, 7)": "100, 128, 255",
        "(TeleportationPylon, 8)": "255, 255, 233",
        "(CalamityMod/AstralPylonTile, 0)": "143, 0, 255",
        "(CalamityMod/CragsPylonTile, 0)": "255, 200, 0",
        "(CalamityMod/SunkenPylonTile, 0)": "0, 100, 255",
        "(CalamityMod/SulphurPylonTile, 0)": "120, 190, 33",
        "(ExtraPylons/ChlorophytePylon, 0)": "53, 224, 31",
        "(ExtraPylons/CloudPylon, 0)": "233, 255, 255",
        "(ExtraPylons/CoralPylon, 0)": "215, 40, 95",
        "(ExtraPylons/CorruptionPylon, 0)": "138, 75, 180",
        "(ExtraPylons/CrimsonPylon, 0)": "209, 46, 46",
        "(ExtraPylons/DungeonPylon, 0)": "172, 83, 162",
        "(ExtraPylons/GemPylon, 0)": "106, 205, 236",
        "(ExtraPylons/GlowingMossPylon, 0)": "133, 151, 239",
        "(ExtraPylons/GranitePylon, 0)": "79, 98, 191",
        "(ExtraPylons/MarblePylon, 0)": "233, 255, 233",
        "(ExtraPylons/GravePylon, 0)": "255, 233, 233",
        "(ExtraPylons/HoneyPylon, 0)": "255, 255, 0",
        "(ExtraPylons/IcePylon, 0)": "90, 132, 196",
        "(ExtraPylons/LivingPylon, 0)": "95, 56, 17",
        "(ExtraPylons/LivingMahoganyPylon, 0)": "95, 74, 17",
        "(ExtraPylons/OasisPylon, 0)": "143, 234, 255",
        "(ExtraPylons/SpacePylon, 0)": "156, 143, 255",
        "(ExtraPylons/SpiderPylon, 0)": "233, 143, 255",
        "(ExtraPylons/TemplePylon, 0)": "219, 77, 0",
        "(ExtraPylons/UnderworldPylon, 0)": "255, 65, 36",
        "(Verdant/VerdantPylonTile, 0)": "189, 66, 162",
        "(Fargowiltas/SiblingPylonTile, 0)": "0, 82, 44",
        "(Spooky/SpookyBiomePylon, 0)": "255, 150, 30",
        "(Spooky/SpookyHellPylon, 0)": "145, 30, 255"
    */
    /*
        "(TeleportationPylon, 0)": "0.4, 1, 0.5",
        "(TeleportationPylon, 1)": "0.78, 1, 0.25",
        "(TeleportationPylon, 2)": "1, 0.4, 1",
        "(TeleportationPylon, 3)": "0.6, 0.5, 0.8",
        "(TeleportationPylon, 4)": "0, 0.7, 0.88",
        "(TeleportationPylon, 5)": "1, 0.85, 0.5",
        "(TeleportationPylon, 6)": "0.5, 0.85, 1",
        "(TeleportationPylon, 7)": "0.4, 0.5, 1",
        "(TeleportationPylon, 8)": "1, 1, 0.95",
        "(CalamityMod/AstralPylonTile, 0)": "0.55, 0, 1",
        "(CalamityMod/CragsPylonTile, 0)": "1, 0.75, 0",
        "(CalamityMod/SunkenPylonTile, 0)": "0, 0.4, 1",
        "(CalamityMod/SulphurPylonTile, 0)": "0.5, 0.85, 0.1",
        "(ExtraPylons/ChlorophytePylon, 0)": "0.25, 0.9, 0.1",
        "(ExtraPylons/CloudPylon, 0)": "0.9, 1, 1",
        "(ExtraPylons/CoralPylon, 0)": "0.8, 0.15, 0.4",
        "(ExtraPylons/CorruptionPylon, 0)": "0.66, 0.4, 0.66",
        "(ExtraPylons/CrimsonPylon, 0)": "0.8, 0.15, 0.2",
        "(ExtraPylons/DungeonPylon, 0)": "0.66, 0.4, 0.5",
        "(ExtraPylons/GemPylon, 0)": "0.45, 0.75, 0.88",
        "(ExtraPylons/GlowingMossPylon, 0)": "0.5, 0.6, 0.88",
        "(ExtraPylons/GranitePylon, 0)": "0.33, 0.45, 0.75",
        "(ExtraPylons/MarblePylon, 0)": "1, 1, 0.88",
        "(ExtraPylons/GravePylon, 0)": "0.75, 1, 1",
        "(ExtraPylons/HoneyPylon, 0)": "1, 1, 0",
        "(ExtraPylons/IcePylon, 0)": "0.4, 0.5, 0.8",
        "(ExtraPylons/LivingPylon, 0)": "0.4, 0.2, 0.1",
        "(ExtraPylons/LivingMahoganyPylon, 0)": "0.4, 0.3, 0.1",
        "(ExtraPylons/OasisPylon, 0)": "0.55, 0.88, 1",
        "(ExtraPylons/SpacePylon, 0)": "0.6, 0.55, 1",
        "(ExtraPylons/SpiderPylon, 0)": "0.95, 0.55, 1",
        "(ExtraPylons/TemplePylon, 0)": "0.88, 0.3, 0",
        "(ExtraPylons/UnderworldPylon, 0)": "1, 0.25, 0.1",
        "(Verdant/VerdantPylonTile, 0)": "0.75, 0.25, 0.65",
        "(Fargowiltas/SiblingPylonTile, 0)": "0, 0.3, 0.2",
        "(Spooky/SpookyBiomePylon, 0)": "1, 0.65, 0.1",
        "(Spooky/SpookyHellPylon, 0)": "0.55, 0.1, 1"
    */
}