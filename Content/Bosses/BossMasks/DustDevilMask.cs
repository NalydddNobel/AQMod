using Aequus.Common.Items;

namespace Aequus.Content.Bosses.BossMasks;

internal class DustDevilMask : InstancedModItem {
    public DustDevilMask(System.String name) : base($"{name}Mask", typeof(DustDevilMask).GetFilePath()) {
    }

    private System.Int32 _fireSide;
    private System.Int32 _iceSide;

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

    public override void DrawArmorColor(Player drawPlayer, System.Single shadow, ref Color color, ref System.Int32 glowMask, ref Color glowMaskColor) {
        color = Color.Lerp(Color.White, color, 0.5f);
    }

    public override void EquipFrameEffects(Player player, EquipType type) {
        player.head = player.direction == -1 ? _fireSide : _iceSide;
    }
}
