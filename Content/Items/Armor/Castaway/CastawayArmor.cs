using AequusRemake.Core;
using AequusRemake.Content.Items.Materials;
using AequusRemake.Content.Tiles.CraftingStations.TrashCompactor;
using AequusRemake.Core.ContentGeneration;
using AequusRemake.DataSets;
using System.Linq;

namespace AequusRemake.Content.Items.Armor.Castaway;

public class CastawayArmor : UnifiedArmorSet {
    public static readonly int DefenseRegenerationCooldown = 420;
    public static readonly int DefenseRegenerationRate = 24;

    /// <summary>Determines how many spiky balls can be in the world at once before they're forced to despawn.</summary>
    public static readonly int MaxBallsOut = 10;
    /// <summary>Determines how many spiky balls can be released at once when damaged.</summary>
    public static readonly int MaxBallsOnHit = 3;

    public static readonly float KnockbackResistanceBody = 0.3f;
    public static readonly float KnockbackResistanceLegs = 0.15f;

    private static InstancedArmor _head;
    private static InstancedArmor _body;
    private static InstancedArmor _legs;

    public static ModItem Head => _head;
    public static ModItem Body => _body;
    public static ModItem Legs => _legs;

    public static int HeadTextureId { get; private set; }
    public static int Head2TextureId { get; private set; }
    public static int BodyTextureId { get; private set; }
    public static int Body2TextureId { get; private set; }

    public override void Load() {
        var keyword = new Keyword(this.GetLocalization("DefenseDamageKeyword"), this.GetLocalization("DefenseDamageKeywordTip"), ItemID.AdhesiveBandage, new Color(187, 255, 170));
        AddArmor(new InstancedHelmet(
                this,
                new ArmorStats(Defense: 9, Rarity: Commons.Rare.BiomeOcean, Value: Item.sellPrice(silver: 22, copper: 50)),
                keyword
            ), out _head);
        AddArmor(new InstancedBody(
                this,
                new ArmorStats(Defense: 10, Rarity: Commons.Rare.BiomeOcean, Value: Item.sellPrice(silver: 37, copper: 50)),
                keyword,
                tooltipArguments: ALanguage.Percent(KnockbackResistanceBody)
            ), out _body);
        AddArmor(new InstancedLegs(
                this,
                new ArmorStats(Defense: 9, Rarity: Commons.Rare.BiomeOcean, Value: Item.sellPrice(silver: 30)),
                keyword,
                tooltipArguments: ALanguage.Percent(KnockbackResistanceLegs)
            ), out _legs);

        _head.HookUpdateEquip(UpdateBrokenDefense)
            .HookIsArmorSet(IsSet)
            .HookUpdateArmorSet(UpdateSetbonus);

        _body.HookUpdateEquip(UpdateBrokenDefense).HookUpdateEquip(UpdateBody);

        _legs.HookUpdateEquip(UpdateBrokenDefense).HookUpdateEquip(UpdateLegs);

        HeadTextureId = EquipLoader.GetEquipSlot(Mod, _head.Name, EquipType.Head);
        BodyTextureId = EquipLoader.GetEquipSlot(Mod, _body.Name, EquipType.Body);
        Head2TextureId = EquipLoader.AddEquipTexture(Mod, $"{_head.Texture}_Head_2", EquipType.Head, null, $"{_head.Name}2");
        Body2TextureId = EquipLoader.AddEquipTexture(Mod, $"{_body.Texture}_Body_2", EquipType.Body, null, $"{_body.Name}2");

        AequusRemake.OnAddRecipes += AddRecipes;
    }

    bool IsSet(Item head, Item body, Item legs) {
        return body.type == _body.Type && legs.type == _legs.Type;
    }

    void UpdateSetbonus(Item item, Player player) {
        player.setBonus = this.GetLocalizedValue("Setbonus");
        player.GetModPlayer<CastawayPlayer>().setbonus = true;
    }

    void UpdateBrokenDefense(Item item, Player player) {
        player.GetModPlayer<CastawayPlayer>().brokenDefenseMax += item.defense;
    }

    void UpdateBody(Item item, Player player) {
        player.GetModPlayer<CastawayPlayer>().kbResist -= KnockbackResistanceBody;
    }

    void UpdateLegs(Item item, Player player) {
        player.GetModPlayer<CastawayPlayer>().kbResist -= KnockbackResistanceLegs;
    }

    void AddRecipes() {
        foreach (int helm in ItemTypeVariantDataSet.CopperHelmet.Where(i => i.ValidEntry)) {
            _head.CreateRecipe()
                .AddIngredient(ModContent.ItemType<CompressedTrash>(), 20)
                .AddIngredient(helm)
                .AddTile(ModContent.TileType<TrashCompactor>())
                .Register();
        }
        foreach (int chainmail in ItemTypeVariantDataSet.CopperChainmail.Where(i => i.ValidEntry)) {
            _body.CreateRecipe()
                .AddIngredient(ModContent.ItemType<CompressedTrash>(), 20)
                .AddIngredient(chainmail)
                .AddTile(ModContent.TileType<TrashCompactor>())
                .Register();
        }
        foreach (int greaves in ItemTypeVariantDataSet.CopperGreaves.Where(i => i.ValidEntry)) {
            _legs.CreateRecipe()
                .AddIngredient(ModContent.ItemType<CompressedTrash>(), 20)
                .AddIngredient(greaves)
                .AddTile(ModContent.TileType<TrashCompactor>())
                .Register();
        }
    }
}