using Aequu2.Core.ContentGeneration;

namespace Aequu2.Old.Content.DronePylons;

internal class InstancedDroneItem : InstancedModItem {
    private readonly DroneSlot _parent;
    private readonly int _value;

    public InstancedDroneItem(DroneSlot slot, int value) : base(slot.Name.Replace("Slot", ""), $"{slot.NamespaceFilePath()}/{slot.Name.Replace("Slot", "")}Item") {
        _parent = slot;
        _value = value;
    }

    public override void Load() {
        ModTypeLookup<ModItem>.RegisterLegacyNames(this, $"Inactive{Name}", $"{Name}Item");
    }

    public override void SetDefaults() {
        Item.width = 14;
        Item.height = 14;
        Item.consumable = true;
        Item.maxStack = Item.CommonMaxStack;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.noUseGraphic = true;
        Item.useAnimation = 50;
        Item.useTime = 50;
        Item.UseSound = SoundID.Item4;
        Item.rare = ItemRarityID.Green;
        Item.shootSpeed = 4f;
        Item.value = _value;
    }

    public override bool? UseItem(Player player) {
        int tileX = (int)(player.position.X + player.width / 2f) / 16;
        int tileY = (int)(player.position.Y + player.height / 2f) / 16;
        for (int i = -25; i < 25; i++) {
            for (int j = -25; j < 25; j++) {
                int x = tileX + i;
                int y = tileY + j;
                if (!WorldGen.InWorld(x, y, 10) && TileHelper.IsSectionLoaded(x, y)) {
                    continue;
                }

                if (DroneSystem.ValidSpot(x, y)) {
                    return DroneSystem.FindOrAddDrone(x, y)?.ConsumeSlot(_parent, player) == true;
                }
            }
        }
        return false;
    }
}