using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Metadata;
using Terraria.Localization;
using Terraria.ObjectData;

namespace AequusRemake.Core.ContentGeneration;

internal interface IUnifiedTree : IModType {
    ModTile Sapling { get; }
    ITree Tree { get; }
    IEnumerable<int> ValidTiles { get; }

    int DustType { get; }
}

public abstract class UnifiedModTree : ModTexturedType, IUnifiedTree {
    protected Asset<Texture2D> TextureValue { get; private set; }
    protected Asset<Texture2D> BranchTextureValue { get; private set; }
    protected Asset<Texture2D> TopTextureValue { get; private set; }

    public int DustType { get; protected set; }
    protected int LeafGoreType;
    protected TreeTypes _treeTypeOverride = TreeTypes.None;
    protected TreePaintingSettings _shaderSettings;
    protected bool _disablesAcornDrop;

    public ModTile Sapling { get; private set; }
    public ModTree ModTree { get; private set; }
    public ITree Tree => ModTree;

    public abstract IEnumerable<int> ValidTiles { get; }

    public abstract int DropWood();

    public virtual bool Shake(int X, int Y, ref bool createLeaves) {
        return true;
    }

    public sealed override void Load() {
        string saplingTexturePath = AequusTextures.Tile(TileID.Saplings);
        LeafGoreType = GoreID.TreeLeaf_Normal;
        DustType = DustID.WoodFurniture;

        if (!Main.dedServ) {
            TextureValue = RequestOrGetDefault("", AequusTextures.Tile(TileID.Trees));
            BranchTextureValue = RequestOrGetDefault("_Branches", AequusTextures.Tree_Branches(0));
            TopTextureValue = RequestOrGetDefault("_Tops", AequusTextures.Tree_Tops(0));

            string saplingTexturePathWanted = $"{Texture}Sapling";
            if (ModContent.HasAsset(saplingTexturePathWanted)) {
                saplingTexturePath = saplingTexturePathWanted;
            }

            string goreTexturePathWanted = $"{Texture}Leaves";
            if (ModContent.HasAsset(goreTexturePathWanted)) {
                ModGore leaves = new InstancedTreeEffect(Name, goreTexturePathWanted);
                Mod.AddContent(leaves);
            }

            string dustTexturePathWanted = $"{Texture}Dust";
            if (ModContent.HasAsset(goreTexturePathWanted)) {
                ModDust leaves = new InstancedCloneDust(Name, dustTexturePathWanted, updateType: DustID.WoodFurniture);
                Mod.AddContent(leaves);
            }

            _shaderSettings = new TreePaintingSettings {
                UseSpecialGroups = true,
                SpecialGroupMinimalHueValue = 11f / 72f,
                SpecialGroupMaximumHueValue = 0.25f,
                SpecialGroupMinimumSaturationValue = 0.88f,
                SpecialGroupMaximumSaturationValue = 1f
            };
        }

        ModTree = new InstancedModTree(this);
        Sapling = new InstancedSaplingTile(this, saplingTexturePath);
        Mod.AddContent(Sapling);
        Mod.AddContent(ModTree);

        OnLoad();

        Asset<Texture2D> RequestOrGetDefault(string name, string defaultTexture) {
            return ModContent.RequestIfExists($"{Texture}{name}", out Asset<Texture2D> result)
                ? result : ModContent.Request<Texture2D>(defaultTexture);
        }
    }
    public virtual void OnLoad() { }

    protected sealed override void Register() {
    }

    public override void SetupContent() {
        ModTree.GrowsOnTileId = ValidTiles.ToArray();
        if (Mod.TryFind($"{Name}", out ModGore modGore)) {
            LeafGoreType = modGore.Type;
        }
        if (Mod.TryFind($"{Name}", out ModDust modDust)) {
            DustType = modDust.Type;
        }
        SetStaticDefaults();
    }

    [Autoload(value: false)]
    private class InstancedModTree : ModTree {
        [CloneByReference]
        private readonly UnifiedModTree _parent;

        public override TreeTypes CountsAsTreeType => _parent._treeTypeOverride;

        public override TreePaintingSettings TreeShaderSettings => _parent._shaderSettings;

        public InstancedModTree(UnifiedModTree parent) {
            _parent = parent;
        }

        public override Asset<Texture2D> GetTexture() {
            return _parent.TextureValue;
        }

        public override Asset<Texture2D> GetBranchTextures() {
            return _parent.BranchTextureValue;
        }

        public override Asset<Texture2D> GetTopTextures() {
            return _parent.TopTextureValue;
        }

        public override int SaplingGrowthType(ref int style) {
            style = 0;
            return _parent.Sapling.Type;
        }

        public override void SetStaticDefaults() { }

        public override int DropWood() {
            return _parent.DropWood();
        }

        public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight) {

        }

        public override bool Shake(int x, int y, ref bool createLeaves) {
            return _parent.Shake(x, y, ref createLeaves);
        }

        public override int TreeLeaf() {
            return _parent.LeafGoreType;
        }

        public override bool CanDropAcorn() {
            return !_parent._disablesAcornDrop;
        }

        public override int CreateDust() {
            return _parent.DustType;
        }
    }

    private class InstancedTreeEffect(string name, string texture) : InstancedModGore(name, texture, safe: true) {
        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            GoreSets.SpecialAI[Type] = 3;
            GoreSets.PaintedFallingLeaf[Type] = true;
        }
    }
}

internal class InstancedSaplingTile(IUnifiedTree Parent, string texture) : InstancedTile(Parent.Name, texture) {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;

        TileObjectData.newTile.Width = 1;
        TileObjectData.newTile.Height = 2;
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
        TileObjectData.newTile.UsesCustomCanPlace = true;
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        TileObjectData.newTile.CoordinateWidth = 16;
        TileObjectData.newTile.CoordinatePadding = 2;
        TileObjectData.newTile.AnchorValidTiles = Parent.ValidTiles.ToArray();
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.DrawFlipHorizontal = true;
        TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
        TileObjectData.newTile.LavaDeath = true;
        TileObjectData.newTile.RandomStyleRange = 3;
        TileObjectData.newTile.StyleMultiplier = 3;

        //TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
        //TileObjectData.newSubTile.AnchorValidTiles = new int[] { TileType<ExampleSand>() };
        //TileObjectData.addSubTile(1);

        TileObjectData.addTile(Type);

        AddMapEntry(CommonColor.MapWoodFurniture, Language.GetText("MapObject.Sapling"));

        TileID.Sets.TreeSapling[Type] = true;
        TileID.Sets.CommonSapling[Type] = true;
        TileID.Sets.SwaysInWindBasic[Type] = true;
        TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);

        DustType = Parent.DustType;

        AdjTiles = [TileID.Saplings];
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }

    public override void RandomUpdate(int i, int j) {
        //if (!WorldGen.genRand.NextBool(20)) {
        //    return;
        //}

        Tile tile = Framing.GetTileSafely(i, j);
        bool growSuccess;

        if (Parent is UnifiedModTree) {
            growSuccess = WorldGen.GrowTree(i, j);
        }
        else {
            growSuccess = WorldGen.GrowPalmTree(i, j);
        }

        // A flag to check if a player is near the sapling
        bool isPlayerNear = WorldGen.PlayerLOS(i, j);

        //If growing the tree was a success and the player is near, show growing effects
        if (growSuccess && isPlayerNear) {
            WorldGen.TreeGrowFXCheck(i, j);
        }
        Main.NewText(growSuccess);
    }

    public override void SetSpriteEffects(int i, int j, ref SpriteEffects effects) {
        if (i % 2 == 0) {
            effects = SpriteEffects.FlipHorizontally;
        }
    }
}
