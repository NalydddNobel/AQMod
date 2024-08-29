using System;

namespace Aequus.Content.Items.Tools.PocketWormhole;

internal class PhaseMirrorGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.ModItem is IPhaseMirror;
    }

    public override void UseStyle(Item item, Player player, Rectangle heldItemFrame) {
        if (player.JustDroppedAnItem || player.altFunctionUse == 2 || item.ModItem is not IPhaseMirror mirror) {
            return;
        }

        //if (player.itemAnimationMax != 64) {
        //    player.itemAnimation = 64;
        //    player.itemAnimationMax = 64;
        //    player.itemTime = player.itemAnimation;
        //    player.itemTimeMax = player.itemAnimationMax;
        //}

        if (Main.netMode != NetmodeID.Server && player.TryGetModPlayer(out PhaseMirrorPlayer phaseMirrorPlayer)) {
            Rectangle hitbox = phaseMirrorPlayer.GetAnimationBox();

            //Main.NewText($"{player.itemAnimation} | {player.itemAnimationMax}");
            if (player.itemAnimation == player.itemAnimationMax) {
                phaseMirrorPlayer.DustCache?.Clear();
            }

            int pieces = player.itemAnimationMax / 8;
            int curPiece = player.itemAnimation / 8;
            int y = 0;
            //Main.NewText($"{pieces}: {curPiece}");
            if (curPiece < 7 && curPiece >= 5) {
                float progression = (player.itemAnimation - 40) / 16f;
                y = (int)(hitbox.Height * (1f - progression)) - 6;
                player.Aequus().CustomDrawShadow = (float)Math.Pow(1f - progression, 2f);
            }
            else if (curPiece < 2) {
                float progression = 1f - player.itemAnimation / 16f;
                player.Aequus().CustomDrawShadow = progression > 0.9f ? null : 1f - progression;
                y = (int)(hitbox.Height * (1f - progression)) - 4;
            }
            if (curPiece > 4 || curPiece < 2) {
                for (int i = 0; i < hitbox.Width; i += 3) {
                    mirror.GetPhaseMirrorDust(player, out int dustType, out var dustColor);
                    var d = Dust.NewDustPerfect(hitbox.TopLeft() + new Vector2(i, y), dustType, Vector2.Zero, 150, dustColor, 0.8f);
                    d.noGravity = true;
                    d.fadeIn = d.scale + 0.2f;
                    d.customData = new PhaseMirrorDustInstanceInfo(i, y);
                    phaseMirrorPlayer.DustCache!.Add(d);
                }
            }
        }

        if (player.itemTime == 0) {
            player.ApplyItemTime(item);
        }
        else if (player.itemTime == player.itemTimeMax / 2) {
            player.grappling[0] = -1;
            player.grapCount = 0;

            player.RemoveAllGrapplingHooks();

            mirror.Teleport(player);
        }
    }
}
