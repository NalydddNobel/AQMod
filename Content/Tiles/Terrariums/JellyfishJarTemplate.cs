using Aequu2.Core.ContentGeneration;
using Aequu2.Core.Graphics.Tiles;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.ObjectData;
using Terraria.Utilities;
using tModLoaderExtended.Terraria.GameContent.Creative;

namespace Aequu2.Content.Tiles.Terrariums;

public abstract class JellyfishJarTemplate : ModTile {
    protected readonly byte[] _mode = new byte[Main.cageFrames];
    protected readonly byte[] _frameCounter = new byte[Main.cageFrames];
    protected readonly byte[] _frame = new byte[Main.cageFrames];
    protected bool _onScreen;

    private Asset<Texture2D> _glowTexture;

    protected int AnimationShockChance { get; set; }
    protected int JellyfishItem { get; set; }

    public override void Load() {
        ModItem jarItem = new InstancedTileItem(this, journeyOverride: new JourneySortByTileId(TileID.GreenJellyfishBowl));
        Mod.AddContent(jarItem);

        SpecialTileRenderer.PreDrawNonSolidTiles += () => _onScreen = false;
        Aequu2.OnAddRecipes += () => {
            jarItem.CreateRecipe()
                .AddIngredient(JellyfishItem)
                .AddIngredient(ItemID.BottledWater)
                .Register()
                .SortBeforeFirstRecipesOf(ItemID.GreenJellyfishJar);
        };

        if (!Main.dedServ) {
            _glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow");
        }
    }

    public override void SetStaticDefaults() {
        AnimationShockChance = 1800;
        Main.tileLavaDeath[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileFrameImportant[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.addTile(Type);

        DustType = DustID.Glass;
    }

    public override bool CreateDust(int i, int j, ref int type) {
        if (WorldGen.genRand.NextBool(3)) {
            type = -1;
        }
        return true;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        Tile tile = Main.tile[i, j];
        int style = GetFramingStyle(i, j);
        int jellyfishBowlFraming = _frame[style];

        Vector2 drawCoordinates = new Vector2(i * 16f, j * 16f) - Main.screenPosition + TileHelper.DrawOffset;
        Rectangle frame = new Rectangle(tile.TileFrameX, tile.TileFrameY + jellyfishBowlFraming * 36, 16, 16);
        Color lightColor = Lighting.GetColor(i, j);
        Color jellyfishGlowColor = new Color(200, 200, 200, 0);

        Main.spriteBatch.Draw(TextureAssets.Tile[Type].Value, drawCoordinates, frame, lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(_glowTexture.Value, drawCoordinates, frame, jellyfishGlowColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        _onScreen = true;
        return false;
    }

    public override void AnimateTile(ref int frame, ref int frameCounter) {
        if (!_onScreen) {
            return;
        }

        UnifiedRandom rand = Main.rand;
        for (int i = 0; i < Main.cageFrames; i++) {
            _frameCounter[i]++;
            if (_mode[i] == 0 && rand.NextBool(AnimationShockChance)) {
                _mode[i] = 1;
            }
            if (_mode[i] == 2 && rand.NextBool(60)) {
                _mode[i] = 3;
            }

            int frameCounterMax = 1;
            if (_mode[i] == 0) {
                frameCounterMax = rand.Next(10, 20);
            }
            if (_mode[i] == 1) {
                frameCounterMax = rand.Next(15, 25);
            }
            if (_mode[i] == 2) {
                frameCounterMax = rand.Next(4, 9);
            }
            if (_mode[i] == 3) {
                frameCounterMax = rand.Next(15, 25);
            }

            if (_mode[i] == 0 && _frame[i] <= 3 && _frameCounter[i] >= frameCounterMax) {
                _frameCounter[i] = 0;
                _frame[i]++;
                if (_frame[i] >= 4) {
                    _frame[i] = 0;
                }
            }
            if (_mode[i] == 1 && _frame[i] <= 7 && _frameCounter[i] >= frameCounterMax) {
                _frameCounter[i] = 0;
                _frame[i]++;
                if (_frame[i] >= 7) {
                    _mode[i] = 2;
                }
            }
            if (_mode[i] == 2 && _frame[i] <= 9 && _frameCounter[i] >= frameCounterMax) {
                _frameCounter[i] = 0;
                _frame[i]++;
                if (_frame[i] >= 9) {
                    _frame[i] = 7;
                }
            }
            if (_mode[i] == 3 && _frame[i] <= 10 && _frameCounter[i] >= frameCounterMax) {
                _frameCounter[i] = 0;
                _frame[i]++;
                if (_frame[i] >= 10) {
                    _frame[i] = 3;
                    _mode[i] = 0;
                }
            }
        }
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        int style = GetFramingStyle(i, j);
        // Shocking frames I think
        if (_mode[style] == 2) {
            r = 0.7f;
            g = 0.6f;
            b = 0.1f;
        }
        //else {
        //    r = 0.4f;
        //    g = 0.3f;
        //    b = 0.05f;
        //}
    }

    protected static int GetFramingStyle(int i, int j) {
        Tile tile = Main.tile[i, j];
        int left = i - tile.TileFrameX / 18;
        int top = j - tile.TileFrameY / 18;
        return left / 2 * (top / 3) % Main.cageFrames;
    }
}
