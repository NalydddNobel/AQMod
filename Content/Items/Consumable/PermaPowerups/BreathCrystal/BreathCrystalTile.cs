using Aequus.Common.ContentTemplates.Generic;
using Terraria.Audio;
using Terraria.ObjectData;

namespace Aequus.Content.Items.Consumable.PermaPowerups.BreathCrystal;

public class BreathCrystalTile : ModTile, IAddRecipes {
    public ModItem? PlaceItem { get; private set; }

#if !AQUA_CRYSTAL
    public override bool IsLoadingEnabled(Mod mod) {
        return false;
    }
#endif

    public override void Load() {
        PlaceItem = new InstancedTileItem(this, Settings: new() {
            Rare = ItemRarityID.Green,
            Value = Item.sellPrice(gold: 1, silver: 50),
            Research = 10,
            DisableAutomaticDropItem = true,
            DisplayName = new()
        });

        Mod.AddContent(PlaceItem);
    }

    public override void SetStaticDefaults() {
        this.CloneStaticDefaults(TileID.Heart);
        Main.tileOreFinderPriority[Type] -= 5;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.addTile(Type);

        DustType = DustID.Glass;
        AnimationFrameHeight = 36;

        RegisterItemDrop(ModContent.ItemType<BreathCrystal>());

        (PlaceItem as InstancedTileItem)!.Settings!.DisplayName!.Value = Instance<BreathCrystal>().GetLocalization("PlaceableDisplayName");
        AddMapEntry(Color.Cyan, Instance<BreathCrystal>().DisplayName);
    }

    void IAddRecipes.AddRecipes() {
        PlaceItem!.CreateRecipe()
            .AddIngredient<BreathCrystal>()
            .AddTile(TileID.HeavyWorkBench)
            .AddCondition(Condition.InGraveyard)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.RepairedManaCrystal);
    }

    public override bool KillSound(int i, int j, bool fail) {
        if (!fail) {
            SoundEngine.PlaySound(SoundID.Shatter, new Vector2(i, j).ToWorldCoordinates());
            return false;
        }

        return true;
    }

    public override void AnimateTile(ref int frame, ref int frameCounter) {
        frame = Main.tileFrame[TileID.Heart];
    }
}
