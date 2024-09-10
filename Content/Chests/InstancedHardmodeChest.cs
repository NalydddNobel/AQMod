using Aequus.Common.ContentTemplates;
using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.Entities.Items;
using Terraria.GameContent;

namespace Aequus.Content.Chests;

[Autoload(false)]
public class InstancedHardmodeChest : UnifiedModChest {
    private readonly string _name;

    public override string Name => _name;

    public override string Texture => $"Aequus/Content/Chests/HardmodeChests/{Name}";

    public override string LocalizationCategory => "Tiles.Chests";

    public InstancedHardmodeChest(string Name, Color MapColor, int DustType) {
        _name = $"{Name}Chest";
        // Create display name reference.
        Settings.MapColor = MapColor;
        Settings.DisplayName = ItemSettings.DisplayName = new();
        ItemSettings.Texture = Texture;
        this.DustType = DustType;
        Item = new InstancedTileItem(this, Settings: ItemSettings);
    }

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