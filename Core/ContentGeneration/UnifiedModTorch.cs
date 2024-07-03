using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequu2.Core.ContentGeneration;

// TODO -- Fix Torch God's Favor disliking underwater torches.
public abstract class UnifiedModTorch : ModTile {
    public ModItem Item { get; protected set; }

    public abstract int TorchIngredient { get; }
    public abstract Vector3 LightColor { get; }

    public virtual bool AllowWaterPlacement => false;
    public virtual int TorchCraftAmount => 3;

    public override void Load() {
        Item = new InstancedTorchItem(this);

        Mod.AddContent(Item);
    }

    public override void Unload() {
    }

    public sealed override void SetStaticDefaults() {
        Main.tileLighted[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileNoAttach[Type] = true;
        Main.tileNoFail[Type] = true;
        TileID.Sets.FramesOnKillWall[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.DisableSmartInteract[Type] = true;
        TileID.Sets.Torch[Type] = true;

        AdjTiles = new int[] { TileID.Torches };
        VanillaFallbackOnModDeletion = TileID.Torches;

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

        TileObjectData.newTile.CopyFrom(TileObjectData.StyleTorch);
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 0);

        if (AllowWaterPlacement) {
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
        }
        else {
            Main.tileWaterDeath[Type] = true;
        }

        SafeSetStaticDefaults();
        TileObjectData.addTile(Type);

        AddMapEntry(CommonColor.MapTorch, Language.GetText("ItemName.Torch"));
    }
    protected virtual void SafeSetStaticDefaults() { }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        Vector3 lightColor = LightColor;
        r = lightColor.X;
        g = lightColor.Y;
        b = lightColor.Z;
    }

    public override void MouseOver(int i, int j) {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;

        int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
        player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, style);
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = Main.rand.Next(1, 3);
    }

    public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) {
        offsetY = 0;

        if (WorldGen.SolidTile(i, j - 1)) {
            offsetY = 4;
        }
    }

    protected ulong GetFlameSeed(int i, int j) {
        return Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)i);
    }

    private class InstancedTorchItem : InstancedTileItem {
        private readonly UnifiedModTorch _modTorch;

        public InstancedTorchItem(UnifiedModTorch modTile) : base(modTile, 0, "", true, 0, 50, researchSacrificeCount: 100) {
            _modTorch = modTile;
        }

        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            ItemSets.ShimmerTransformToItem[Type] = ItemID.ShimmerTorch;
            ItemSets.SingleUseInGamepad[Type] = true;
            ItemSets.Torches[Type] = true;
            ItemSets.WaterTorches[Type] = _modTorch.AllowWaterPlacement;
        }

        public override void SetDefaults() {
            Item.DefaultToTorch(_modTile.Type, 0, allowWaterPlacement: _modTorch.AllowWaterPlacement);
            Item.value = 50;
        }

        // Note: This method is also called when Torch God's Favor selects this torch.
        public override void HoldItem(Player player) {
            if (player.wet && Item.noWet) {
                return;
            }

            if (Main.rand.NextBool(player.itemAnimation > 0 ? 7 : 30)) {
                Dust dust = Dust.NewDustDirect(new Vector2(player.itemLocation.X + (player.direction == -1 ? -16f : 6f), player.itemLocation.Y - 14f * player.gravDir), 4, 4, _modTile.DustType, 0f, 0f, 100);
                if (!Main.rand.NextBool(3)) {
                    dust.noGravity = true;
                }

                dust.velocity *= 0.3f;
                dust.velocity.Y -= 1.5f;
                dust.position = player.RotatedRelativePoint(dust.position);
            }

            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
            Lighting.AddLight(position, _modTorch.LightColor);
        }

        public override void PostUpdate() {
            if (Item.wet && Item.noWet) {
                Lighting.AddLight(Item.Center, _modTorch.LightColor);
            }
        }

        public override void AddRecipes() {
            CreateRecipe(_modTorch.TorchCraftAmount)
                .AddIngredient(ItemID.Torch, _modTorch.TorchCraftAmount)
                .AddIngredient(_modTorch.TorchCraftAmount)
                .SortAfterFirstRecipesOf(ItemID.RainbowTorch)
                .Register();
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Torches;
        }
    }
}
