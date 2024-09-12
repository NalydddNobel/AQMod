using Aequus.Common.Preferences;
using Aequus.Common.Utilities;
using Aequus.Common.World;
using Terraria.Localization;

namespace Aequus.Content.World.Generation;

public class EOCOresGenerator : Generator {
    readonly LocalizedText Demonite = ALanguage.GetText("Announcement.EyeOfCthulhuDemonite");
    readonly LocalizedText Crimtane = ALanguage.GetText("Announcement.EyeOfCthulhuCrimtane");
    readonly LocalizedText Avalon_Baccilite = ALanguage.GetText("CrossMod.Avalon.Announcement.EoCBaccilite");
    readonly LocalizedText Drunk = ALanguage.GetText("Announcement.EyeOfCthulhuDrunk");

    static bool Generated { get => AequusWorld.eyeOfCthulhuOres; set => AequusWorld.eyeOfCthulhuOres = value; }

    public override bool IsLoadingEnabled(Mod mod) {
        return GameplayConfig.Instance.EyeOfCthulhuOres;
    }

    protected override void Generate() {
        int[] ore = DetermineOres();
        var rand = Rand;
        int oresWanted = Main.maxTilesX * Main.maxTilesY / (Generated ? 200000 : 30000);
        int oresGenerated = 0;

        for (int m = 0; m < oresWanted; m++) {
            for (int k = 0; k < 50; k++) {
                int x = rand.Next(80, Main.maxTilesX - 80);
                int y = rand.Next((int)Main.worldSurface + 50, Main.UnderworldLayer);

                if (TileHelper.ScanTilesSquare(x, y, 400, TileHelper.HasShimmer)) {
                    continue;
                }

                if (!WorldGen.InWorld(x, y, fluff: 40) || !TileID.Sets.Stone[Main.tile[x, y].TileType]) {
                    continue;
                }

                ushort chosenOre = (ushort)rand.Next(ore);
                for (int l = 0; l < 5; l++) {
                    WorldGen.OreRunner(x, y, rand.Next(3, 5), rand.Next(14, 26), chosenOre);
                }
                oresGenerated++;

                break;
            }
        }

        WorldGen.BroadcastText(GetMessage().ToNetworkText(), TextHelper.EventMessageColor);
        Generated = true;

        //Main.NewText(oresWanted + " | " + oresGenerated);
    }

    int[] DetermineOres() {
        return Main.drunkWorld ? ([TileID.Crimtane, TileID.Demonite]) : WorldGen.crimson ? [TileID.Crimtane] : [TileID.Demonite];
    }

    LocalizedText GetMessage() {
        return Main.drunkWorld ? Drunk : WorldGen.crimson ? Crimtane : Demonite;
    }

    [Gen.AequusSystem_PostUpdateWorld]
    internal static void CheckEoCOres() {
        if (!NPC.downedBoss1 || Generated) {
            return;
        }

        Instance<EOCOresGenerator>().GenerateOnThread();
    }
}