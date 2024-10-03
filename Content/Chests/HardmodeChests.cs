using Aequus.Common;
using Aequus.Common.Items;
using Aequus.Systems.Chests;

namespace Aequus.Content.Chests;

public class HardmodeChests : LoadedType {
    public readonly InstancedHardmodeChest Adamantite;
    public readonly InstancedHardmodeChest Frost;
    public readonly InstancedHardmodeChest Tortoise;
    public readonly InstancedHardmodeChest Granite;
    public readonly InstancedHardmodeChest Marble;
    public readonly InstancedHardmodeChest Shroomite;
    public readonly InstancedHardmodeChest Forbidden;

    public HardmodeChests() {
        Adamantite = new InstancedHardmodeChest("Adamantite", new Color(160, 25, 50), DustID.Adamantite);
        Granite = new InstancedHardmodeChest("Granite", new Color(100, 255, 255), DustID.Granite);
        Marble = new InstancedHardmodeChest("Marble", new Color(200, 185, 100), DustID.Marble);
        Frost = new InstancedHardmodeChest("Frost", new Color(105, 115, 255), DustID.t_Frozen);
        Tortoise = new InstancedHardmodeChest("Tortoise", new Color(170, 105, 70), DustID.WoodFurniture);
        Shroomite = new InstancedHardmodeChest("Shroomite", new Color(0, 50, 215), DustID.GlowingMushroom);
        Forbidden = new InstancedHardmodeChest("Forbidden", new Color(180, 130, 20), DustID.Sand);
    }

    protected override void Load() {
        this.RegisterMembers();

        // Legacy names for loading, honestly who cares about the trapped chests though.
        ModTypeLookup<ModItem>.RegisterLegacyNames(Frost.Item, "HardFrozenChest");
        ModTypeLookup<ModTile>.RegisterLegacyNames(Frost, "HardFrozenChestTile");
        ModTypeLookup<ModItem>.RegisterLegacyNames(Granite.Item, "HardGraniteChest");
        ModTypeLookup<ModTile>.RegisterLegacyNames(Granite, "HardGraniteChestTile");
        ModTypeLookup<ModItem>.RegisterLegacyNames(Tortoise.Item, "HardJungleChest");
        ModTypeLookup<ModTile>.RegisterLegacyNames(Tortoise, "HardJungleChestTile");
        ModTypeLookup<ModItem>.RegisterLegacyNames(Marble.Item, "HardMarbleChest");
        ModTypeLookup<ModTile>.RegisterLegacyNames(Marble, "HardMarbleChestTile");
        ModTypeLookup<ModItem>.RegisterLegacyNames(Shroomite.Item, "HardMushroomChest");
        ModTypeLookup<ModTile>.RegisterLegacyNames(Shroomite, "HardMushroomChestTile");
        ModTypeLookup<ModItem>.RegisterLegacyNames(Forbidden.Item, "HardSandstoneChest");
        ModTypeLookup<ModTile>.RegisterLegacyNames(Forbidden, "HardSandstoneChestTile");
    }

    public override void PostSetupContent() {
        ItemID.Sets.ShimmerTransformToItem[Adamantite.Item.Type] = ItemID.GoldChest;
        ItemID.Sets.ShimmerTransformToItem[Frost.Item.Type] = ItemID.IceChest;
        ItemID.Sets.ShimmerTransformToItem[Granite.Item.Type] = ItemID.GraniteChest;
        ItemID.Sets.ShimmerTransformToItem[Marble.Item.Type] = ItemID.MarbleChest;
        ItemID.Sets.ShimmerTransformToItem[Tortoise.Item.Type] = ItemID.IvyChest;
        ItemID.Sets.ShimmerTransformToItem[Shroomite.Item.Type] = ItemID.MushroomChest;
        ItemID.Sets.ShimmerTransformToItem[Forbidden.Item.Type] = ItemID.DesertChest;

        var convert = ChestSets.Instance.HardmodeConvert;
        convert[ChestID.Gold] = new((TileKey)Adamantite.Type, ChestPool.UndergroundHard);
        convert[ChestID.Frozen] = new((TileKey)Frost.Type, ChestPool.SnowHard);
        convert[ChestID.Granite] = new((TileKey)Granite.Type, ChestPool.UndergroundHard);
        convert[ChestID.RichMahogany] = new((TileKey)Tortoise.Type, ChestPool.JungleHard);
        convert[ChestID.Ivy] = new((TileKey)Tortoise.Type, ChestPool.JungleHard);
        convert[ChestID.Marble] = new((TileKey)Marble.Type, ChestPool.UndergroundHard);
        convert[ChestID.Mushroom] = new((TileKey)Shroomite.Type, ChestPool.UndergroundHard);
        convert[ChestID.Webbed] = new(ChestID.Spider, ChestPool.UndergroundHard);
        convert[ChestID.Sandstone] = new((TileKey)Forbidden.Type, ChestPool.DesertHard);
    }

    public override void AddRecipes() {
        Adamantite.Item.CreateRecipe(5)
            .AddIngredient(ItemID.GoldChest, 5)
            .AddIngredient(ItemID.AdamantiteBar, 2)
            .AddTile(TileID.MythrilAnvil)
            .Register()
            .DisableDecraft()
            .Clone()
            .ReplaceItem(ItemID.AdamantiteBar, ItemID.TitaniumBar)
            .Register();

        Granite.Item.CreateRecipe(5)
            .AddIngredient(ItemID.GraniteChest, 5)
            .AddIngredient(ItemID.SoulofLight, 8)
            .AddTile(TileID.MythrilAnvil)
            .Register()
            .DisableDecraft();

        Marble.Item.CreateRecipe(5)
            .AddIngredient(ItemID.MarbleChest, 5)
            .AddIngredient(ItemID.SoulofNight, 8)
            .AddTile(TileID.MythrilAnvil)
            .Register()
            .DisableDecraft();

        Frost.Item.CreateRecipe(5)
            .AddIngredient(ItemID.IceChest, 5)
            .AddIngredient(ItemID.FrostCore)
            .AddTile(TileID.MythrilAnvil)
            .Register()
            .DisableDecraft();

        Forbidden.Item.CreateRecipe(5)
            .AddIngredient(ItemID.DesertChest, 5)
            .AddIngredient(ItemID.AncientBattleArmorMaterial)
            .AddTile(TileID.MythrilAnvil)
            .Register()
            .DisableDecraft();

        Tortoise.Item.CreateRecipe(5)
            .AddIngredient(ItemID.IvyChest, 5)
            .AddIngredient(ItemID.TurtleShell)
            .AddTile(TileID.MythrilAnvil)
            .Register()
            .DisableDecraft();

        Shroomite.Item.CreateRecipe(5)
            .AddIngredient(ItemID.MushroomChest, 5)
            .AddIngredient(ItemID.ShroomiteBar, 2)
            .AddTile(TileID.MythrilAnvil)
            .Register()
            .DisableDecraft();
    }
}
