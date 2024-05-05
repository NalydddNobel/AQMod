using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria.Enums;
using Terraria.GameContent;

namespace Aequus.Core.ContentGeneration;

public abstract class UnifiedModTree : ModTexturedType {
    protected Asset<Texture2D> TextureValue { get; private set; }
    protected Asset<Texture2D> BranchTextureValue { get; private set; }
    protected Asset<Texture2D> TopTextureValue { get; private set; }

    protected int TreeLeafGoreId;
    protected int TreeLeafDustId;
    protected TreeTypes _treeTypeOverride = TreeTypes.None;
    protected bool _disablesAcornDrop;

    public ModTile Sapling { get; private set; }
    public ModTree Tree { get; private set; }

    public abstract IEnumerable<int> ValidTiles();
    public abstract int DropWood();

    public virtual bool Shake(int X, int Y, ref bool createLeaves) {
        return true;
    }

    public sealed override void Load() {
        Tree = new InternalModTree(this);

        TreeLeafGoreId = GoreID.TreeLeaf_Normal;
        TreeLeafDustId = DustID.WoodFurniture;
        TextureValue = RequestOrGetDefault("", "Terraria/Images/Tiles_5");
        BranchTextureValue = RequestOrGetDefault("_Branches", "Terraria/Images/Tree_Branches_0");
        TopTextureValue = RequestOrGetDefault("_Tops", "Terraria/Images/Tree_Tops_0");

        //Mod.AddContent(Tree);

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
        Tree.GrowsOnTileId = ValidTiles().ToArray();
        SetStaticDefaults();
    }

    [Autoload(value: false)]
    private class InternalModTree : ModTree {
        [CloneByReference]
        private readonly UnifiedModTree _parent;

        public override TreePaintingSettings TreeShaderSettings => new TreePaintingSettings {
            UseSpecialGroups = true,
            SpecialGroupMinimalHueValue = 11f / 72f,
            SpecialGroupMaximumHueValue = 0.25f,
            SpecialGroupMinimumSaturationValue = 0.88f,
            SpecialGroupMaximumSaturationValue = 1f
        };

        public InternalModTree(UnifiedModTree parent) {
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
            return _parent.TreeLeafGoreId;
        }

        public override TreeTypes CountsAsTreeType => _parent._treeTypeOverride;

        public override bool CanDropAcorn() {
            return !_parent._disablesAcornDrop;
        }

        public override int CreateDust() {
            return _parent.TreeLeafDustId;
        }
    }
}
