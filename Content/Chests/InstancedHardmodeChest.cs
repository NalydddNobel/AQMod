using Aequus.Common.ContentTemplates;
using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.Entities.Items;
using Terraria.GameContent;

namespace Aequus.Content.Chests;

[Autoload(false)]
public class InstancedHardmodeChest : UnifiedModChest {
    private readonly string _name;
    private readonly string _texture;

    public override string Name => _name;

    public override string Texture => _texture;

    public override string LocalizationCategory => "Tiles.Chests";

    public InstancedHardmodeChest() { }

    public InstancedHardmodeChest(string Name, string Texture, Color MapColor, int DustType) : this() {
        _name = $"{Name}Chest";
        _texture = $"{Texture}Chest";
        Settings.MapColor = MapColor;
        // Create display name reference.
        Settings.DisplayName = ItemSettings.DisplayName = new();
        ItemSettings.Texture = _texture;
        this.DustType = DustType;
        Item = new InstancedTileItem(this, Settings: ItemSettings);
    }

    internal InstancedHardmodeChest(string Name, Color MapColor, int DustType) : this(Name, $"Aequus/Content/Chests/HardmodeChests/{Name}", MapColor, DustType) { }

    public override void Load() {
        Settings.DisplayName!.Value = this.GetLocalization("DisplayName", PrettyPrintName);
        ModTypeLookup<ModTile>.RegisterLegacyNames(this, $"{Name[..^5]}", $"{Name}Tile");
        ModTypeLookup<ModItem>.RegisterLegacyNames(Item, $"{Item.Name[..^5]}", $"{Item.Name}Item");

        base.Load();
    }

    public override void SafeSetStaticDefaults() {
        Main.RegisterItemAnimation(Item.Type, new CustomItemDrawFrame(new Rectangle(38, 2, 32, 32)));
        Main.RegisterItemAnimation(TrappedChest!.Item.Type, new CustomItemDrawFrame(new Rectangle(38, 2, 32, 32)));
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
        Texture2D texture = TextureAssets.Tile[Type].Value;
        if (texture.Height <= 114) {
            return;
        }

        DrawChestTile(i, j, spriteBatch, texture, Color.White, frameYOffset: 114);
    }
}