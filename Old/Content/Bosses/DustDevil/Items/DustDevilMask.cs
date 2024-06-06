using Aequus.Core.ContentGeneration;

namespace Aequus.Old.Content.Bosses.DustDevil.Items;

internal class DustDevilMask(string name) : InstancedModItem($"{name}Mask", typeof(DustDevilMask).GetFilePath()) {
    private int _fireSide;
    private int _iceSide;

    public override void Load() {
        if (Main.netMode != NetmodeID.MultiplayerClient) {
            _fireSide = EquipLoader.AddEquipTexture(Mod, AequusTextures.DustDevilMaskFire_Head.Path, EquipType.Head, this);
            _iceSide = EquipLoader.AddEquipTexture(Mod, AequusTextures.DustDevilMaskIce_Head.Path, EquipType.Head, this);
        }
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.headSlot = _fireSide;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 75);
        Item.vanity = true;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.Lerp(Color.White, lightColor, 0.5f);
    }

    public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor) {
        color = Color.Lerp(Color.White, color, 0.5f);
    }

    public override void EquipFrameEffects(Player player, EquipType type) {
        player.head = player.direction == -1 ? _fireSide : _iceSide;
    }
}
