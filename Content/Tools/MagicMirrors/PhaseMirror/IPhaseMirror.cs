using System;
using System.Collections.Generic;

namespace Aequus.Content.Tools.MagicMirrors.PhaseMirror; 

public interface IPhaseMirror {
    List<(Int32, Int32, Dust)> DustEffectCache { get; set; }
    Int32 UseAnimationMax { get; }
    void GetPhaseMirrorDust(Player player, Item item, IPhaseMirror me, out Int32 dustType, out Color dustColor);

    void Teleport(Player player, Item item, IPhaseMirror me);

    public static void UsePhaseMirror(Player player, Item item, IPhaseMirror me) {
        Int32 useAnimMax = me.UseAnimationMax;
        Int32 amt = player.itemAnimationMax % 8;
        if (player.itemAnimationMax != useAnimMax) {
            player.itemAnimation = useAnimMax;
            player.itemAnimationMax = useAnimMax;
            player.itemTime = player.itemAnimation;
            player.itemTimeMax = player.itemAnimationMax;
        }

        if (Main.netMode != NetmodeID.Server) {
            var hitbox = player.getRect();
            hitbox.Inflate(12, 12);
            hitbox.Width += 1;
            hitbox.Height -= 6;

            me.DustEffectCache ??= new();
            //Main.NewText($"{player.itemAnimation} | {player.itemAnimationMax}");
            if (player.itemAnimation == player.itemAnimationMax) {
                me.DustEffectCache?.Clear();
            }

            Int32 pieces = player.itemAnimationMax / 8;
            Int32 curPiece = player.itemAnimation / 8;
            Int32 y = 0;
            //Main.NewText($"{pieces}: {curPiece}");
            if (curPiece < pieces - 1 && curPiece >= pieces - 3) {
                Single progression = (player.itemAnimation - 40) / 16f;
                y = (Int32)(hitbox.Height * (1f - progression)) - 6;
                player.GetModPlayer<AequusPlayer>().CustomDrawShadow = (Single)Math.Pow(1f - progression, 2f);
            }
            else if (curPiece < 2) {
                Single progression = 1f - player.itemAnimation / 16f;
                player.GetModPlayer<AequusPlayer>().CustomDrawShadow = progression > 0.9f ? null : 1f - progression;
                y = (Int32)(hitbox.Height * (1f - progression)) - 4;
            }
            else if (curPiece < pieces - 1) {
                player.GetModPlayer<AequusPlayer>().CustomDrawShadow = 1f;
            }
            if (curPiece > 4 || curPiece < 2) {
                for (Int32 i = 0; i < hitbox.Width; i += 3) {
                    me.GetPhaseMirrorDust(player, item, me, out Int32 dustType, out var dustColor);
                    var d = Dust.NewDustPerfect(hitbox.TopLeft() + new Vector2(i, y), dustType, Vector2.Zero, 150, dustColor, 0.8f);
                    d.noGravity = true;
                    d.fadeIn = d.scale + 0.2f;
                    me.DustEffectCache.Add((i, y, d));
                }
            }
            for (Int32 i = 0; i < me.DustEffectCache.Count; i++) {
                if (!me.DustEffectCache[i].Item3.active) {
                    me.DustEffectCache.RemoveAt(i);
                    i--;
                }
                else {
                    me.DustEffectCache[i].Item3.position = hitbox.TopLeft() + new Vector2(me.DustEffectCache[i].Item1, me.DustEffectCache[i].Item2);
                }
            }
        }

        if (player.itemTime == 0) {
            player.ApplyItemTime(item);
        }
        else if (player.itemTime == player.itemTimeMax / 2) {
            player.grappling[0] = -1;
            player.grapCount = 0;
            me.DustEffectCache?.Clear();

            for (Int32 p = 0; p < 1000; p++) {
                if (Main.projectile[p].active && Main.projectile[p].owner == player.whoAmI && Main.projectile[p].aiStyle == 7) {
                    Main.projectile[p].Kill();
                }
            }

            me.Teleport(player, item, me);
        }
    }
}