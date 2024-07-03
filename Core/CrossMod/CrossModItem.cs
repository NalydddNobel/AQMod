using System.Collections.Generic;
using Terraria.Localization;

namespace Aequu2.Core.CrossMod;

public abstract class CrossModItem : ModItem {
    public virtual void OnSetStaticDefaults() { }
    protected virtual void SafeAddRecipes() { }

    public string CrossModName { get; init; }

    public CrossModItem() {
        CrossModName = ExtendCrossMod.GetModFromNamespace(GetType());
    }

    public override string LocalizationCategory => $"CrossMod.{CrossModName}.Items";

    public sealed override void SetStaticDefaults() {
        OnSetStaticDefaults();

        if (!ModLoader.HasMod(CrossModName)) {
            Item.ResearchUnlockCount = 0;
        }
    }

    public sealed override void AddRecipes() {
        if (ModLoader.HasMod(CrossModName)) {
            SafeAddRecipes();
        }
    }
}

internal sealed class CrossModGlobalItem : GlobalItem {
    public static readonly Color TooltipColor = Color.Lerp(Color.White, Color.Turquoise * 1.5f, 0.5f);

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.ModItem is CrossModItem;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (item.ModItem is CrossModItem crossModItem && !ModLoader.HasMod(crossModItem.CrossModName)) {
            tooltips.Add(new TooltipLine(Mod, "NeedsMod", Language.GetTextValue("Mods.Aequu2.Misc.NeedsMod", crossModItem.CrossModName)) {
                OverrideColor = TooltipColor
            });
        }
    }
}
