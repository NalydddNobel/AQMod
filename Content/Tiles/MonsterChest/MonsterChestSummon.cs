using Aequus.Common.NPCs;
using Aequus.Common.UI;
using Aequus.Core;
using Aequus.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.MonsterChest;

public class MonsterChestSummon : ModNPC {
    private int npcLock;
    public int NPCLock { get => npcLock - 1; set => npcLock = value + 1; }

    public float originX;
    public float originY;

    protected virtual Texture2D ChainTexture => TextureAssets.Chain22.Value;
    protected virtual Texture2D LockTexture {
        get {
            Main.GetItemDrawFrame(ItemID.ChestLock, out var itemTexture, out _);
            return itemTexture;
        }
    }
    public virtual Color ChainColor => new Color(255, 100, 40, 0);
    public virtual Color LockColor => new Color(255, 160, 40, 200);

    public override string Texture => AequusTextures.TownNPCExclamation.Path;

    public const int AnimationTimeChestLockBreak = 60;
    public const int AnimationTimeChainBreakStart = AnimationTimeChestLockBreak - 4;
    public const int AnimationTimeChainBreakLength = 60;

    #region Initialization
    public override void SetStaticDefaults() {
        NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
        NPCID.Sets.NPCBestiaryDrawOffset[Type] = new() {
            PortraitPositionYOverride = 28f,
            Position = new(0f, 14f),
            Scale = 1f,
        };
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry)
            .AddMainSpawn(BestiaryBuilder.CavernsBiome);
    }

    public override void SetDefaults() {
        NPC.lifeMax = 1000;
        NPC.defense = 10;
        NPC.damage = 10;
        NPC.aiStyle = -1;
        NPC.noTileCollide = true;
        NPC.width = 32;
        NPC.height = 32;
        NPC.noGravity = true;
        NPC.dontTakeDamage = true;
        NPC.rarity = 2;
    }
    #endregion

    public override void OnSpawn(IEntitySource source) {
        var tileCoordinates = NPC.Center / 16f;
        originX = tileCoordinates.X;
        originY = tileCoordinates.Y;
    }

    #region AI
    protected virtual int GetSpawnedNPCId() {
        return Main.rand.NextFromList(NPCID.SkeletonAstonaut, NPCID.BoneThrowingSkeleton, NPCID.CaveBat);
    }

    private void SpawnAI() {
        if (NPC.ai[2] == 0f) {
            for (int i = 0; i < 1000; i++) {
                int randomX = Main.rand.Next(10, 20) * (Main.rand.NextBool() ? -1 : 1) + (int)originX;
                int randomY = Main.rand.Next(-20, 20) + (int)originY;
                if (i >= 999) {
                    NPC.ai[2] = randomX;
                    NPC.ai[3] = randomY;
                    return;
                }

                if (!WorldGen.InWorld(randomX, randomY, fluff: 10)) {
                    continue;
                }

                if (Main.tile[randomX, randomY].IsFullySolid() && !Main.tile[randomX, randomY - 1].IsFullySolid()) {
                    NPC.ai[2] = randomX;
                    NPC.ai[3] = randomY;
                    break;
                }
            }
        }

        float animation = MathF.Pow(Math.Min(NPC.ai[1], 1f), 1f);
        NPC.ai[1] += 0.025f;
        float topY = Math.Min(NPC.ai[3], originY - 4f);
        float y;
        if (animation < 0.5f) {
            y = MathHelper.Lerp(originY, topY, MathF.Sin(animation / 0.5f * MathHelper.PiOver2));
        }
        else {
            y = MathHelper.Lerp(topY, NPC.ai[3] - 1.5f, 1f - MathF.Sin((1f - (animation - 0.5f) / 0.5f) * MathHelper.PiOver2));
        }
        NPC.Center = new Vector2(MathHelper.Lerp(originX, NPC.ai[2] + 0.5f, animation) * 16f, y * 16f);
        if (NPC.ai[1] > 1f) {
            var d = Dust.NewDustPerfect(NPC.Center + (NPC.ai[1] * MathHelper.TwoPi).ToRotationVector2() * NPC.ai[1] * 10f, DustID.Torch, Scale: Main.rand.NextFloat(0.4f, 0.6f));
            d.noGravity = true;
            d.velocity *= Main.rand.NextFloat(0.2f);
            d.fadeIn = d.scale + 0.7f;
            d = Dust.NewDustPerfect(NPC.Center + (NPC.ai[1] * MathHelper.TwoPi + MathHelper.Pi).ToRotationVector2() * NPC.ai[1] * 10f, DustID.Torch, Scale: Main.rand.NextFloat(0.4f, 0.6f));
            d.noGravity = true;
            d.velocity *= Main.rand.NextFloat(0.2f);
            d.fadeIn = d.scale + 0.7f;
        }
        else {
            var d = Dust.NewDustPerfect(NPC.Center, DustID.Torch, Scale: Main.rand.NextFloat(0.4f, 0.6f));
            d.noGravity = true;
            d.velocity *= Main.rand.NextFloat(0.2f);
            d.fadeIn = d.scale + 0.7f;
        }
        if (NPC.ai[1] > 2f) {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                NPC.ai[1] = 0f;
                int npcType = GetSpawnedNPCId();
                NPCLock = NPC.NewNPC(new EntitySource_TileUpdate((int)originX, (int)originY), (int)NPC.Center.X, (int)NPC.Center.Y, npcType, NPC.whoAmI);
                Main.npc[NPCLock].Center = NPC.Center;
                Main.npc[NPCLock].alpha = 255;
            }
            NPC.netUpdate = true;
        }
    }

    private void AttachedAI(int npcLock) {
        for (int i = 0; i < NPC.playerInteraction.Length; i++) {
            NPC.playerInteraction[i] = Main.npc[npcLock].playerInteraction[i];
        }

        NPC.ai[1] += 0.025f;

        var lockNPC = Main.npc[npcLock];
        NPC.Center = lockNPC.Center;
        if (lockNPC.alpha > 0) {
            lockNPC.alpha -= 15;
            if (lockNPC.alpha < 0) {
                lockNPC.alpha = 0;
            }
        }
        var difference = new Vector2(originX * 16f, originY * 16f) - lockNPC.Center;
        int wantedDirection = Math.Sign(difference.X);
        if (difference.Length() > 200f && lockNPC.direction != wantedDirection) {
            int velocityDirection = Math.Sign(lockNPC.velocity.X);
            if (velocityDirection != wantedDirection) {
                lockNPC.velocity.X *= 0.975f;
            }
            else {
                lockNPC.direction = velocityDirection;
            }

            lockNPC.velocity += difference * Helper.Oscillate(NPC.ai[1], 0.0005f, 0.001f) * Math.Clamp(NPC.knockBackResist, 0.5f, 2f);
            var stepUpPosition = new Vector2(lockNPC.position.X + lockNPC.width / 2f + (lockNPC.width / 2f + 16f) * velocityDirection, lockNPC.position.Y + lockNPC.height - 4f);
            if (velocityDirection != 0 && Framing.GetTileSafely(stepUpPosition.ToTileCoordinates()).IsFullySolid()) {
                lockNPC.velocity.X -= velocityDirection * 0.2f;
                lockNPC.velocity.Y -= 2f;
            }
        }
    }

    private void BrokenAI(int npcLock) {
        if (npcLock != NPC.whoAmI) {
            NPCLock = npcLock = NPC.whoAmI;
        }
        if (!Main.npc[npcLock].active) {
            return;
        }

        NPC.localAI[2]++;
        int count = 0;
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active && Main.npc[i].ModNPC is MonsterChestSummon monsterChest && monsterChest.NPCLock != i) {
                count++;
            }
        }
        var origin = new Vector2(originX, originY) * 16f;
        NPC.Center = Vector2.Lerp(NPC.Center, origin + NPC.DirectionFrom(origin) * 64f, 0.05f);

        if (count > 0) {
            NPC.velocity += Main.rand.NextVector2Square(-0.05f, 0.05f);
            return;
        }

        int animationTimer = (int)NPC.localAI[1];
        var tile = Main.tile[(int)originX, (int)originY];
        int left = (int)originX - tile.TileFrameX % 36 / 18;
        int top = (int)originY - tile.TileFrameY % 36 / 18;
        if (animationTimer == AnimationTimeChestLockBreak) {
            SoundEngine.PlaySound(SoundID.Unlock, NPC.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                if (Chest.IsLocked(left, top) && Chest.Unlock(left, top)) {
                    if (Main.netMode == NetmodeID.Server) {
                        NetMessage.SendData(MessageID.LockAndUnlock, number: Player.FindClosest(NPC.position, NPC.width, NPC.height), number2: 1f, number3: left, number4: top);
                        NetMessage.SendTileSquare(-1, left, top, 2);
                    }
                }
                else {
                    for (int i = 0; i < NPC.playerInteraction.Length; i++) {
                        NPC.playerInteraction[i] = false;
                    }
                }
            }
        }

        var d = Dust.NewDustPerfect(new Vector2(left, top) * 16f, DustID.Torch, Scale: Main.rand.NextFloat(0.8f, 1f));
        d.noGravity = true;
        d.velocity *= Main.rand.NextFloat(0.2f);

        if (animationTimer < AnimationTimeChestLockBreak) {
            float shake = animationTimer / (AnimationTimeChestLockBreak * 2f);
            NPC.velocity += Main.rand.NextVector2Square(-shake, shake);
        }
        else {
            NPC.rarity = 0;
        }
        if (animationTimer > AnimationTimeChainBreakStart + AnimationTimeChainBreakLength) {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                NPC.StrikeInstantKill();
            }
        }

        NPC.localAI[1]++;
    }

    public override void AI() {
        if (!WorldGen.InWorld((int)originX, (int)originY)) {
            return;
        }

        int npcLock = NPCLock;
        if (npcLock == -1) {
            SpawnAI();
        }
        else if (npcLock != NPC.whoAmI && Main.npc[npcLock].active) {
            AttachedAI(npcLock);
        }
        else {
            BrokenAI(npcLock);
        }
    }
    #endregion

    public override void FindFrame(int frameHeight) {
        NPC.localAI[0] += 0.025f;
    }

    private void DrawBestiary(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        int tileId = ModContent.TileType<MonsterChest>();
        Main.instance.LoadTiles(tileId);

        var tileTexture = TextureAssets.Tile[tileId].Value;
        var chestPosition = NPC.Center;
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < 2; j++) {
                int tileFrameX = 18 * i;
                int tileFrameY = 18 * j + 76;

                spriteBatch.Draw(tileTexture, chestPosition + new Vector2(i * 16f - 16f, j * 16f - 16f), new(tileFrameX, tileFrameY, 16, j > 0 ? 18 : 16), drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }

        var chainTexture = ChainTexture;
        var chainsPosition = NPC.Center + new Vector2(0f, -24f);
        float animationTime = NPC.localAI[0];
        for (int k = -1; k < 2; k++) {
            NPC.localAI[0] += 8f;
            var lockPosition = chainsPosition + NPC.localAI[0].ToRotationVector2() * Helper.Oscillate(NPC.localAI[0] * 2.15f + k * 9f, 2f, 6f) + new Vector2(0f, -30f).RotatedBy(k * 1.25f);
            var velocity = Vector2.Normalize(lockPosition - chestPosition);
            var chainWobble = velocity.RotatedBy(MathHelper.PiOver2);
            velocity *= chainTexture.Height;

            DrawChain(spriteBatch, chainTexture, chestPosition, lockPosition, velocity, chainWobble, 0f, 20f, ChainColor);

            DrawLock(lockPosition, 0f, NPC.Opacity, LockColor with { A = 250 }, spriteBatch.Draw);
        }
        NPC.localAI[0] = animationTime;
    }

    private void DrawChain(SpriteBatch spriteBatch, Texture2D chainTexture, Vector2 startLocation, Vector2 endLocation, Vector2 velocity, Vector2 chainWobble, float chainBreak, float opacityDistance, Color chainColor) {
        float traveled = 0f;
        for (int i = 0; i < 100; i++) {
            var distance = Vector2.Distance(startLocation, endLocation);
            float opacity = Math.Clamp(traveled / opacityDistance, 0f, 1f);
            spriteBatch.Draw(chainTexture, startLocation + chainWobble * MathF.Sin(NPC.localAI[0] * 5f + i * 0.3f) * MathF.Pow(opacity, 2f) * 6f + chainWobble.RotatedBy(i * 10f) * chainBreak, null, chainColor * opacity, velocity.ToRotation() - MathHelper.PiOver2, chainTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            traveled += chainTexture.Height;
            startLocation += velocity;
            if (distance < 20f) {
                break;
            }
        }
    }

    private void DrawLock(Vector2 drawCoordinates, float shake, float opacity, Color color, DrawDelegate drawMethod) {
        var lockTexture = LockTexture;
        var lockFrame = lockTexture.Frame();
        float pulse = Helper.Oscillate(NPC.localAI[0] * 4f, 0.8f, 1f);
        pulse = Math.Max(pulse, Math.Min(NPC.localAI[1] / 20f, 1f)) * opacity;
        var lockLocation = drawCoordinates + Main.rand.NextVector2Square(-shake, shake) * 4f;
        var lockOrigin = lockFrame.Size() / 2f;
        drawMethod(lockTexture, lockLocation, lockFrame, color * pulse, 0f, lockOrigin, new(pulse), SpriteEffects.None, 0f);
        if (shake > 0f) {
            drawMethod(lockTexture, lockLocation, lockFrame, color with { A = 0 } * shake, 0f, lockOrigin, new(pulse + shake * 0.5f), SpriteEffects.None, 0f);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        if (NPC.IsABestiaryIconDummy) {
            DrawBestiary(spriteBatch, screenPos, drawColor);
            return false;
        }

        if (NPCLock == -1) {
            return false;
        }

        var lockNPC = Main.npc[NPCLock];
        var startLocation = new Vector2(originX, originY) * 16f - screenPos;
        var endLocation = NPC.Center + new Vector2(0f, lockNPC.gfxOffY) - screenPos;
        var chainTexture = ChainTexture;
        var velocity = Vector2.Normalize(endLocation - startLocation);
        var chainWobble = velocity.RotatedBy(MathHelper.PiOver2);
        velocity *= chainTexture.Height;
        float chainBreak = 0f;
        float opacityDistance = 200f - 180f * MathF.Min(NPC.localAI[2] / 40f, 1f);
        var chainColor = ChainColor * lockNPC.Opacity;

        if (NPC.localAI[1] > AnimationTimeChainBreakStart) {
            chainBreak = (NPC.localAI[1] - AnimationTimeChainBreakStart) * 2f;
            chainColor *= 1f - chainBreak / AnimationTimeChainBreakLength;
        }

        DrawChain(spriteBatch, chainTexture, startLocation, endLocation, velocity, chainWobble, chainBreak, opacityDistance, chainColor);

        if (NPC.localAI[1] < AnimationTimeChestLockBreak) {
            DrawLock(endLocation + lockNPC.velocity, MathF.Pow(NPC.localAI[1] / 60f, 4f), lockNPC.Opacity, LockColor, MiscWorldInterfaceElements.Draw);
        }
        return false;
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(originX);
        writer.Write(originY);
        writer.Write(NPCLock);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        originX = reader.ReadSingle();
        originY = reader.ReadSingle();
        NPCLock = reader.ReadInt32();
    }
}