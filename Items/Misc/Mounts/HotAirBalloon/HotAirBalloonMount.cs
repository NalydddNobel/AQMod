using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Mounts.HotAirBalloon {
    public class HotAirBalloonMount : ModMount {
        public const int BalloonFrames = 6;

        public override void SetStaticDefaults() {
            MountData.jumpHeight = 1;
            MountData.jumpSpeed = 1f;
            MountData.acceleration = 0.006f;
            MountData.blockExtraJumps = true;
            MountData.constantJump = true;
            MountData.heightBoost = 0;
            MountData.fallDamage = 0f;
            MountData.runSpeed = 2f;
            MountData.dashSpeed = 2f;
            MountData.flightTimeMax = 14400;
            MountData.fatigueMax = 14400;
            MountData.fallDamage = 0f;
            MountData.usesHover = true;

            MountData.buff = ModContent.BuffType<HotAirBalloonBuff>();

            MountData.spawnDust = DustID.Torch;
            MountData.spawnDustNoGravity = true;

            MountData.totalFrames = 4;
            MountData.playerYOffsets = Enumerable.Repeat(4, MountData.totalFrames).ToArray();
            MountData.xOffset = 10;
            MountData.yOffset = -2;
            MountData.playerHeadOffset = 22;
            MountData.bodyFrame = 3;

            MountData.standingFrameCount = 4;
            MountData.standingFrameDelay = 32;
            MountData.standingFrameStart = 0;

            MountData.runningFrameCount = MountData.standingFrameCount;
            MountData.runningFrameDelay = MountData.standingFrameDelay;
            MountData.runningFrameStart = MountData.standingFrameStart;

            MountData.flyingFrameCount = MountData.standingFrameCount;
            MountData.flyingFrameDelay = MountData.standingFrameDelay;
            MountData.flyingFrameStart = MountData.standingFrameStart;

            MountData.inAirFrameCount = MountData.standingFrameCount;
            MountData.inAirFrameDelay = MountData.standingFrameDelay;
            MountData.inAirFrameStart = MountData.standingFrameStart;

            MountData.idleFrameCount = MountData.standingFrameCount;
            MountData.idleFrameDelay = MountData.standingFrameDelay;
            MountData.idleFrameStart = MountData.standingFrameStart;
            MountData.idleFrameLoop = true;

            MountData.swimFrameCount = MountData.standingFrameCount;
            MountData.swimFrameDelay = MountData.standingFrameDelay;
            MountData.swimFrameStart = MountData.standingFrameStart;

            if (!Main.dedServ) {
                MountData.textureWidth = MountData.backTexture.Width() + 20;
                MountData.textureHeight = MountData.backTexture.Height();
            }
        }

        public override void UpdateEffects(Player player) {
            if (player.wet) {
                player.mount.Dismount(player);
            }
        }

        public override void SetMount(Player player, ref bool skipDust) {
            player.mount._mountSpecificData = Main.rand.Next(BalloonFrames);
            if (!Main.dedServ) {
                var dustSpawn = player.position;
                int boxHeight = 12;
                dustSpawn.Y -= boxHeight;
                for (int i = 0; i < 10; i++) {
                    var d = Dust.NewDustDirect(dustSpawn, player.width, boxHeight, MountData.spawnDust);
                    d.velocity *= 0.1f;
                    d.velocity.Y += Main.rand.NextFloat(-4f, -0.25f);
                    d.fadeIn = d.scale * 2f;
                    d.noGravity = MountData.spawnDustNoGravity;
                }

                skipDust = true;
            }
        }

        public override void Dismount(Player player, ref bool skipDust) {
            if (!Main.dedServ) {
                for (int i = 0; i < 40; i++) {
                    var d = Dust.NewDustDirect(player.position, player.width, player.height, MountData.spawnDust);
                    d.scale *= 1.2f;
                    d.velocity *= 0.5f;
                    d.noGravity = MountData.spawnDustNoGravity;
                }

                skipDust = true;
            }
        }

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow) {
            if (drawType == 0) {
                var balloonTexture = AequusTextures.HotAirBalloonMount;
                int balloonFrameY = 0;
                if (drawPlayer.mount._mountSpecificData is int val) {
                    balloonFrameY = val;
                }
                var balloonFrame = balloonTexture.Frame(verticalFrames: BalloonFrames, frameY: balloonFrameY);
                var balloonDrawPos = drawPosition + new Vector2(drawPlayer.width / 2f - 10f, -balloonFrame.Height / 2f - frame.Height + 27f);
                balloonDrawPos.X -= MountData.xOffset * drawPlayer.direction;
                playerDrawData.Add(new DrawData(balloonTexture, balloonDrawPos, balloonFrame,
                    Helper.GetLightingSection(balloonDrawPos + Main.screenPosition, 4), rotation, balloonFrame.Size() / 2f, 1f, spriteEffects, 0) { shader = drawPlayer.cMount });
            }
            return true;
        }
    }
}