using Terraria.ModLoader.IO;

namespace Aequu2.Content.Fishing.CrabPots;

public struct CrabPotBiomeData {
    public int LiquidStyle { get; private set; }

    public CrabPotBiomeData(int liquidStyle) {
        LiquidStyle = liquidStyle;
    }

    public TagCompound Save() {
        var tag = new TagCompound();
        if (LiquidStyle < Main.maxLiquidTypes) {
            tag["liquid"] = LiquidStyle;
        }
        else {
            var modLiquid = LoaderManager.Get<WaterStylesLoader>().Get(LiquidStyle);
            if (modLiquid == null) {
                return tag;
            }
            tag["modLiquid"] = modLiquid.FullName;
        }
        return tag;
    }

    public static CrabPotBiomeData Load(TagCompound tag) {
        var crabPot = new CrabPotBiomeData();
        if (tag == null) {
            return crabPot;
        }

        if (tag.TryGet("liquid", out int liquidStyleId)) {
            crabPot.LiquidStyle = liquidStyleId;
        }
        else if (tag.TryGet("modLiquid", out string name)) {
            crabPot.LiquidStyle = GetWaterStyle(name);
        }
        return crabPot;
    }

    internal static int GetWaterStyle(string name) {
        var split = name.Split('/');
        if (ModLoader.TryGetMod(split[0], out var mod) && mod.TryFind<ModWaterStyle>(name, out var waterStyle)) {
            return waterStyle.Slot;
        }
        return 0;
    }
}