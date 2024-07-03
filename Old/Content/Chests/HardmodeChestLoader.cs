using Aequu2.Core.ContentGeneration;
using Aequu2.DataSets.Structures.Enums;
using ReLogic.Content;
using System.Collections.Generic;

namespace Aequu2.Old.Content.Chests;

public sealed class HardmodeChestLoader : ILoad {
    public const int RECIPE_CREATE_AMOUNT = 5;

    public static ModTile AdamantiteChest { get; private set; }
    public static ModTile FrostChest { get; private set; }
    public static ModTile GraniteChest { get; private set; }
    public static ModTile TortoiseCase { get; private set; }
    public static ModTile MarbleChest { get; private set; }
    public static ModTile ShroomiteChest { get; private set; }
    public static ModTile ScarabChest { get; private set; }

    public static readonly Dictionary<ChestStyle, ModTile> StyleToChest = new();

    public void Load(Mod mod) {
        AdamantiteChest = new InstancedHardmodeChest("AdamantiteChest", new() {
            DustType = DustID.Adamantite,
            MapColor = new Color(160, 25, 50),
            PreHardmodeChestItemId = ItemID.GoldChest,
            SecondaryRecipeMaterial = new(ItemID.SoulofLight, 2)
        });
        GraniteChest = new InstancedHardmodeChest("HardGraniteChest", new() {
            DustType = DustID.Granite,
            MapColor = new Color(100, 255, 255),
            PreHardmodeChestItemId = ItemID.GraniteChest,
            SecondaryRecipeMaterial = new(ItemID.SoulofNight, 2)
        });
        MarbleChest = new InstancedHardmodeChest("HardMarbleChest", new() {
            DustType = DustID.Marble,
            MapColor = new Color(200, 185, 100),
            PreHardmodeChestItemId = ItemID.MarbleChest,
            SecondaryRecipeMaterial = new(ItemID.SoulofLight, 2)
        });
        ShroomiteChest = new InstancedHardmodeChest("HardMushroomChest", new() {
            DustType = DustID.GlowingMushroom,
            MapColor = new Color(0, 50, 215),
            PreHardmodeChestItemId = ItemID.MushroomChest,
            SecondaryRecipeMaterial = new(ItemID.SoulofNight, 2)
        });
        FrostChest = new InstancedHardmodeChest("HardFrozenChest", new() {
            DustType = DustID.t_Frozen,
            MapColor = new Color(105, 115, 255),
            PreHardmodeChestItemId = ItemID.IceChest,
            SecondaryRecipeMaterial = new(ItemID.FrostCore, 1)
        });
        ScarabChest = new InstancedHardmodeChest("HardSandstoneChest", new() {
            DustType = DustID.Sand,
            MapColor = new Color(180, 130, 20),
            PreHardmodeChestItemId = ItemID.DesertChest,
            SecondaryRecipeMaterial = new(ItemID.AncientBattleArmorMaterial, 1)
        });
        TortoiseCase = new InstancedHardmodeChest("HardJungleChest", new() {
            DustType = DustID.WoodFurniture,
            MapColor = new Color(170, 105, 70),
            PreHardmodeChestItemId = ItemID.IvyChest,
            SecondaryRecipeMaterial = new(ItemID.TurtleShell, 1)
        });

        StyleToChest[ChestStyle.Gold] = AdamantiteChest;
        StyleToChest[ChestStyle.Granite] = GraniteChest;
        StyleToChest[ChestStyle.Marble] = MarbleChest;
        StyleToChest[ChestStyle.Mushroom] = ShroomiteChest;
        StyleToChest[ChestStyle.Frozen] = FrostChest;
        StyleToChest[ChestStyle.Sandstone] = ScarabChest;
        StyleToChest[ChestStyle.Ivy] = TortoiseCase;
        StyleToChest[ChestStyle.RichMahogany] = TortoiseCase;

        mod.AddContent(AdamantiteChest);
        mod.AddContent(GraniteChest);
        mod.AddContent(MarbleChest);
        mod.AddContent(ShroomiteChest);
        mod.AddContent(FrostChest);
        mod.AddContent(ScarabChest);
        mod.AddContent(TortoiseCase);
    }

    public void Unload() {
        AdamantiteChest = null;
    }

    private class InstancedHardmodeChest : UnifiedModChest {
        private readonly string _name;
        private ChestInfo _info;
        private Asset<Texture2D> _glowTexture;

        public override string Name => _name;

        public InstancedHardmodeChest(string name, ChestInfo info) {
            _name = name;
            DustType = info.DustType;
            _info = info;
        }

        public override void Load() {
            base.Load();
            Aequu2.OnAddRecipes += () => {
                Recipe recipe = DropItem.CreateRecipe(RECIPE_CREATE_AMOUNT)
                    .AddIngredient(_info.PreHardmodeChestItemId, RECIPE_CREATE_AMOUNT)
                    .AddTile(TileID.MythrilAnvil);

                if (_info.SecondaryRecipeMaterial != null) {
                    recipe.AddIngredient(_info.SecondaryRecipeMaterial.Value.X, _info.SecondaryRecipeMaterial.Value.Y);
                }

                recipe.Register()
                    .SortBeforeFirstRecipesOf(ItemID.Chest);
            };
            if (!Main.dedServ && ModContent.RequestIfExists($"{Texture}_Glow", out Asset<Texture2D> glowTexture)) {
                _glowTexture = glowTexture;
            }
            ModTypeLookup<ModTile>.RegisterLegacyNames(this, $"{Name}Tile");
        }

        public override void SafeSetStaticDefaults() {
            Main.tileShine[Type] = Main.tileShine[Type] / 3;
            Main.tileOreFinderPriority[Type] = (short)(Main.tileOreFinderPriority[TileID.Chlorophyte] + 10);
            ItemSets.ShimmerTransformToItem[DropItem.Type] = _info.PreHardmodeChestItemId;
            AddMapEntry(_info.MapColor, CreateMapEntryName(), MapChestName);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
            if (_glowTexture != null) {
                DrawBasicGlowmask(i, j, spriteBatch, _glowTexture.Value, Color.White);
            }
        }

        public record struct ChestInfo(Color MapColor, int DustType, int PreHardmodeChestItemId, Point? SecondaryRecipeMaterial = null);
    }
}
