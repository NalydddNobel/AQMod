using System.Collections.Generic;

namespace Aequus.Content.Tools.MagicMirrors.PhaseMirror;

public class PhaseMirror : ModItem, IPhaseMirror {
    public List<(System.Int32, System.Int32, Dust)> DustEffectCache { get; set; }
    public System.Int32 UseAnimationMax => 64;

    public override void SetStaticDefaults() {
        ItemID.Sets.WorksInVoidBag[Type] = true;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.IceMirror);
        Item.rare = ItemRarityID.Green;
        Item.useTime = UseAnimationMax;
        Item.useAnimation = UseAnimationMax;
        DustEffectCache = new();
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame) {
        if (!player.JustDroppedAnItem) {
            IPhaseMirror.UsePhaseMirror(player, Item, this);
        }
    }

    public void Teleport(Player player, Item item, IPhaseMirror me) {
        player.Spawn(PlayerSpawnContext.RecallFromItem);
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<AequusPlayer>().infiniteWormhole = true;
    }

    public void GetPhaseMirrorDust(Player player, Item item, IPhaseMirror me, out System.Int32 dustType, out Color dustColor) {
        dustType = DustID.MagicMirror;
        dustColor = Color.White;
    }
}