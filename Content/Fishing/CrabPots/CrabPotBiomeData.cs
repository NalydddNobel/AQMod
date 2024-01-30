using Terraria.ModLoader.IO;

namespace Aequus.Content.Fishing.CrabPots;

public struct CrabPotBiomeData {
    public System.Int32 LiquidStyle { get; private set; }

    public CrabPotBiomeData(System.Int32 liquidStyle) {
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

        if (tag.TryGet("liquid", out System.Int32 liquidStyleId)) {
            crabPot.LiquidStyle = liquidStyleId;
        }
        else if (tag.TryGet("modLiquid", out System.String name)) {
            crabPot.LiquidStyle = GetWaterStyle(name);
        }
        return crabPot;
    }

    internal static System.Int32 GetWaterStyle(System.String name) {
        var split = name.Split('/');
        if (ModLoader.TryGetMod(split[0], out var mod) && mod.TryFind<ModWaterStyle>(name, out var waterStyle)) {
            return waterStyle.Slot;
        }
        return 0;
    }
}