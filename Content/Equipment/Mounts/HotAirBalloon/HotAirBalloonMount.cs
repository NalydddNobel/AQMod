using Aequus.Core.ContentGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Utilities;

namespace Aequus.Content.Equipment.Mounts.HotAirBalloon;

public class HotAirBalloonMount : UnifiedModMount {
    public const int BalloonFrames = 2;

    #region Balloon Data and Easter Eggs
    public static readonly Dictionary<string, IBalloonProvider> EasterEggs = new();

    public interface IBalloonProvider {
        IBalloonData GetBalloonData(Player player);
    }

    public interface IBalloonData {
        int Frame { get; }
        Color DrawColor();
        Color? FlameColor();
    }

    public struct BalloonData : IBalloonData, IBalloonProvider {
        public int frame;
        public Color color;
        public Color? flameColor;

        public BalloonData(int frame, Color color, Color? flameColor = null) {
            this.frame = frame;
            this.color = color;
            this.flameColor = flameColor;
        }

        public int Frame => frame;

        public Color DrawColor() {
            return color;
        }

        public Color? FlameColor() {
            return flameColor;
        }

        public IBalloonData GetBalloonData(Player player) {
            return this;
        }
    }

    public struct DynamicColorBalloonData : IBalloonData, IBalloonProvider {
        public int frame;
        public Func<Color> color;
        public Func<Color> flameColor;

        public DynamicColorBalloonData(int frame, Func<Color> color, Func<Color> flameColor = null) {
            this.frame = frame;
            this.color = color;
            this.flameColor = flameColor;
        }

        public int Frame => frame;

        public Color DrawColor() {
            return color();
        }

        public Color? FlameColor() {
            return flameColor?.Invoke();
        }

        public IBalloonData GetBalloonData(Player player) {
            return this;
        }
    }

    public struct EasterEggBalloonProvider : IBalloonProvider {
        public int frame;
        public Color? color;
        public Color? flameColor;

        public EasterEggBalloonProvider(int frame, Color? color, Color? flameColor = null) {
            this.frame = frame;
            this.color = color;
            this.flameColor = flameColor;
        }

        public IBalloonData GetBalloonData(Player player) {
            return new BalloonData(frame, color ?? GetRandomBalloon(player).color, flameColor);
        }
    }

    private void LoadEasterEggs() {
        EasterEggs.Clear();
        EasterEggs["modzilla"] = new BalloonData(1, Color.White);
        EasterEggs["nalyddd"] = new DynamicColorBalloonData(0, () => Color.Lerp(Color.Violet, Color.BlueViolet, Helper.Oscillate(Main.GlobalTimeWrappedHourly, 1f)), () => Color.BlueViolet);
    }
    #endregion

    protected override void OnSetStaticDefaults() {
        LoadEasterEggs();
        MountData.jumpHeight = 1;
        MountData.jumpSpeed = 1f;
        MountData.acceleration = 0.02f;
        MountData.blockExtraJumps = true;
        MountData.heightBoost = 16;
        MountData.runSpeed = 3f;
        MountData.dashSpeed = 3f;
        MountData.flightTimeMax = 100000;
        MountData.fatigueMax = 0;
        MountData.fallDamage = 0f;
        MountData.usesHover = true;

        MountData.spawnDust = DustID.Torch;
        MountData.spawnDustNoGravity = true;

        MountData.totalFrames = 1;
        MountData.playerYOffsets = Enumerable.Repeat(4, MountData.totalFrames).ToArray();
        MountData.xOffset = 0;
        MountData.yOffset = 2;
        MountData.playerHeadOffset = 4;
        MountData.bodyFrame = 3;

        MountData.standingFrameCount = 1;
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
            MountData.textureWidth = MountData.backTexture.Width();
            MountData.textureHeight = MountData.backTexture.Height();
        }
    }

    public override void Unload() {
        EasterEggs.Clear();
    }

    public override void UpdateEffects(Player player) {
        if (player.wet) {
            player.mount.Dismount(player);
        }
        //player.fullRotation += player.velocity.X * 0.01f;
        player.fullRotation = Math.Clamp(player.velocity.X * 0.03f, -0.1f, 0.1f);
        Lighting.AddLight(player.Top, new Vector3(1f, 0.76f, 0.1f) * 0.1f);
    }

    private static BalloonData GetRandomBalloon(Player player) {
        float hue = Main.rand.Next(20) / 20f;
        float luminosity = 0.4f + Main.rand.Next(7) / 10f;
        float saturation = 0.75f + Main.rand.Next(3) * 0.125f;
        return new(0, Main.hslToRgb(hue, saturation, luminosity, a: 255));
    }

    private static IBalloonData GetBalloonData(Player player) {
        foreach (var easterEgg in EasterEggs) {
            if (player.name.ToLower().Equals(easterEgg.Key)) {
                return easterEgg.Value.GetBalloonData(player);
            }
        }
        if (Main.rand.NextBool(25)) {
            return EasterEggs["modzilla"].GetBalloonData(player);
        }
        return GetRandomBalloon(player);
    }

    private void SpawnMountDust(Player player) {
        var dustSpawn = player.position;
        int boxHeight = 12;
        dustSpawn.Y -= boxHeight;
        for (int i = 0; i < 10; i++) {
            var d = Terraria.Dust.NewDustDirect(dustSpawn, player.width, boxHeight, MountData.spawnDust);
            d.velocity *= 0.1f;
            d.velocity.Y += Main.rand.NextFloat(-4f, -0.25f);
            d.fadeIn = d.scale * 2f;
            d.noGravity = MountData.spawnDustNoGravity;
            d.noLight = true;
        }
    }

    private void SpawnDismountDust(Player player) {
        for (int i = 0; i < 40; i++) {
            var d = Terraria.Dust.NewDustDirect(player.position, player.width, player.height, MountData.spawnDust);
            d.scale *= 1.2f;
            d.velocity *= 0.5f;
            d.noGravity = MountData.spawnDustNoGravity;
            d.noLight = true;
        }
    }

    public override void SetMount(Player player, ref bool skipDust) {
        player.mount._mountSpecificData = GetBalloonData(player);
        if (!Main.dedServ) {
            SpawnMountDust(player);
            SpawnDismountDust(player);
            skipDust = true;
        }
    }

    public override void Dismount(Player player, ref bool skipDust) {
        if (!Main.dedServ) {
            SpawnDismountDust(player);
            skipDust = true;
        }
    }

    public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow) {
        if (drawType == 0) {
            var balloonTexture = AequusTextures.HotAirBalloonMount;
            int balloonFrameY = 0;
            var color = Color.White;

            var flameColor = Color.Orange;
            if (drawPlayer.team != (int)Team.None) {
                flameColor = Main.teamColor[drawPlayer.team];
            }
            if (drawPlayer.mount._mountSpecificData is IBalloonData val) {
                balloonFrameY = val.Frame;
                color = val.DrawColor();
                var flameColorOverride = val.FlameColor();
                if (flameColorOverride != null) {
                    flameColor = flameColorOverride.Value;
                }
            }

            var balloonFrame = balloonTexture.Frame(verticalFrames: BalloonFrames, frameY: balloonFrameY);
            var balloonDrawPos = drawPosition + new Vector2(drawPlayer.width / 2f - 10f, -balloonFrame.Height / 2f - frame.Height + 33f);
            balloonDrawPos.X -= MountData.xOffset * drawPlayer.direction;
            var lightColor = ExtendLight.GetBrightestLight((balloonDrawPos + Main.screenPosition).ToTileCoordinates(), 8);
            float lightIntensity = (lightColor.R + lightColor.G + lightColor.B) / 3f / 255f * 0.9f;
            var balloonColor = lightColor.MultiplyRGB(color);
            if (Aequus.HighQualityEffects) {
                playerDrawData.Add(new(balloonTexture, balloonDrawPos, balloonFrame, balloonColor, rotation, balloonFrame.Size() / 2f, 1f, spriteEffects, 0) { shader = drawPlayer.cMount });
                if (lightIntensity < 0.99f) {
                    var balloonFlameColor = flameColor with { A = 0 } * Helper.Oscillate(Main.GlobalTimeWrappedHourly * 2f, 0.75f, 1f);
                    playerDrawData.Add(new(AequusTextures.Bloom, balloonDrawPos, null, balloonFlameColor, 0f, AequusTextures.Bloom.Size() / 2f, new Vector2(0.8f, 1f), spriteEffects, 0) { shader = drawPlayer.cMount });
                    playerDrawData.Add(new(AequusTextures.HotAirBalloonMount_Glow, balloonDrawPos, null, balloonFlameColor, rotation, balloonFrame.Size() / 2f, 1f, spriteEffects, 0) { shader = drawPlayer.cMount });
                    var random = new FastRandom(drawPlayer.name.GetHashCode());
                    var dustTexture = AequusTextures.BaseParticleTexture.Value;
                    for (int i = 0; i < 10; i++) {
                        var dustFrame = dustTexture.Frame(verticalFrames: 3, frameY: random.Next(3));
                        float time = Main.GlobalTimeWrappedHourly * random.NextFloat(0.9f, 1.1f);
                        float wrappedTime = time % 1f;
                        float wrappedTimeReverse = 1f - wrappedTime;
                        var dustPosition = balloonDrawPos + new Vector2(random.NextFloat(-6f, 6f), balloonFrame.Height / 2f - 20f * wrappedTime - random.NextFloat(10f, 18f)).RotatedBy(random.NextFloat(-0.25f, 0.25f));
                        playerDrawData.Add(new(dustTexture, dustPosition, dustFrame, balloonFlameColor * wrappedTimeReverse * 0.5f, time, dustFrame.Size() / 2f, MathF.Sin(wrappedTime * MathHelper.PiOver2) * random.NextFloat(2f, 3f), spriteEffects, 0) { shader = drawPlayer.cMount });
                    }
                }
            }
            playerDrawData.Add(new(balloonTexture, balloonDrawPos, balloonFrame, balloonColor * Math.Max(MathF.Pow(lightIntensity, 6f), 0.4f), rotation, balloonFrame.Size() / 2f, 1f, spriteEffects, 0) { shader = drawPlayer.cMount });
        }
        return true;
    }

    internal override ModItem CreateMountItem() {
        return new InstancedMountItem(this, value: Item.buyPrice(gold: 10), SoundOverride: SoundID.Item34);
    }

    protected override void OnLoad() {
        ModTypeLookup<ModItem>.RegisterLegacyNames(MountItem, "BalloonKit");
    }
}