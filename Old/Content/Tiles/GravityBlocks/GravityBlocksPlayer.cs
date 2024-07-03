using System;
using Terraria.Audio;

namespace Aequu2.Old.Content.Tiles.GravityBlocks;
public class GravityBlocksPlayer : ModPlayer {
    public int gravityTileOld;
    public int gravityTile;

    public override void Load() {
        On_Player.JumpMovement += OverrideGravityDirection;
    }

    private static void OverrideGravityDirection(On_Player.orig_JumpMovement orig, Player player) {
        if (player.TryGetModPlayer(out GravityBlocksPlayer gravityPlayer) && gravityPlayer.gravityTile != 0) {
            player.gravDir = Math.Sign(gravityPlayer.gravityTile);
        }
        orig(player);
    }

    public override void ResetEffects() {
        if (gravityTile != 0) {
            Player.gravControl = false;
            Player.gravControl2 = false;
        }
    }

    public override void PostUpdateEquips() {
        gravityTileOld = gravityTile;
        gravityTile = GravityBlocks.GetGravity(Player.position, Player.width, Player.height);

        if (gravityTile != 0) {
            Player.gravity = 0.4f;
            Player.gravDir = gravityTile < 0 ? -1f : 1f;
        }

        if (gravityTile != gravityTileOld && (gravityTile != 1 || gravityTileOld != 0) && (gravityTile != 0 || gravityTileOld != 1)) {
            Player.controlJump = false;
            Player.velocity.Y = MathHelper.Clamp(Player.velocity.Y, -2f, 2f);
            SoundEngine.PlaySound(SoundID.Item8, Player.position);
        }
    }
}
