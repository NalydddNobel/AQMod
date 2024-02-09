using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Aequus.Content.Tools.MagicMirrors.PhaseMirror;

public class PhaseMirror : ModItem, IPhaseMirror {
    public List<(int, int, Dust)> DustEffectCache { get; set; }
    public int UseAnimationMax => 64;

    public override void SetStaticDefaults() {
        ItemSets.WorksInVoidBag[Type] = true;
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

    public void GetPhaseMirrorDust(Player player, Item item, IPhaseMirror me, out int dustType, out Color dustColor) {
        dustType = DustID.MagicMirror;
        dustColor = Color.White;
    }
}