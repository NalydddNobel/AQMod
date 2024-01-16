using Aequus.Common.NPCs;
using Aequus.Common.NPCs.Bestiary;
using Aequus.Content.Biomes.PollutedOcean;
using Aequus.Core.Initialization;
using System;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;

namespace Aequus.Content.Critters.HorseshoeCrab;

[ModBiomes(typeof(PollutedOceanBiome))]
internal class HorseshoeCrab : InstancedModNPC, CritterCommons.ICritter, IAddRecipes {
    public const int GetUpTime = -30;

    public float tailRotation;
    public int closestPlayerOld;
    public int wallTime;

    private readonly int[] _dustType;
    private readonly int _size;
    private readonly bool _golden;

    public HorseshoeCrab(string name, int size, int[] dustType, bool golden = false) : base(name + "HorseshoeCrab", $"{typeof(HorseshoeCrab).NamespaceFilePath()}/{name}HorseshoeCrab") {
        _size = size;
        _dustType = dustType;
        _golden = golden;
    }

    public override LocalizedText DisplayName => this.GetCategoryText($"HorseshoeCrab.DisplayName{(IsGolden ? ".Golden" : "")}");

    public bool IsGolden => _golden;

    public override void SetDefaults() {
        NPC.width = _size;
        NPC.height = _size;
        NPC.lifeMax = 5;
        NPC.damage = 0;
        NPC.defense = 0;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.npcSlots = 0.5f;

        // Why re-invent the wheel? (or in this case, AI which walks along the edges of tiles)
        NPC.aiStyle = NPCAIStyleID.Snail;
        AIType = NPCID.Snail;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        if (IsGolden) {
            this.CreateEntry(BestiaryBuilder.GoldenCritterKey, database, bestiaryEntry);
        }
        else {
            this.CreateEntry(this.GetCategoryKey("HorseshoeCrab.Bestiary"), database, bestiaryEntry);
        }
    }

    public override void FindFrame(int frameHeight) {
    }

    public override void AI() {
        if (wallTime < 0) {
            NPC.aiStyle = -1;
            NPC.directionY = 1;
            NPC.noGravity = false;
            if (NPC.collideY || wallTime > -20) {
                NPC.velocity.X *= 0.9f;
                wallTime++;
            }

            if (wallTime == GetUpTime) {
                NPC.velocity.Y = -3f;
                NPC.direction = -NPC.direction;
                NPC.spriteDirection = NPC.direction;
                wallTime++;
            }
            if (wallTime > GetUpTime) {
                NPC.rotation = Utils.AngleTowards(NPC.rotation, 0f, 0.1f);
                tailRotation = Utils.AngleLerp(tailRotation, NPC.rotation, 0.1f);
                DrawOffsetY = 0f;
            }
            else {
                NPC.rotation = Utils.AngleTowards(NPC.rotation, MathHelper.Pi + MathF.Sin(wallTime / 5f) * 0.2f, 0.1f);
                tailRotation = Utils.AngleTowards(tailRotation, MathHelper.PiOver2 * 1.5f * -NPC.spriteDirection, 0.05f);
                DrawOffsetY = 4f;
            }

            return;
        }

        NPC.aiStyle = NPCAIStyleID.Snail;
        tailRotation = Utils.AngleLerp(tailRotation, NPC.rotation, 0.1f);
        DrawOffsetY = 0f;

        if (!NPC.noGravity || !NPC.TryGetGlobalNPC<AequusNPC>(out var aequusNPC)) {
            return;
        }

        float detectionRange = closestPlayerOld != -1 ? 250f : 100f;
        int closestPlayer = -1;
        NPC.TargetClosest(faceTarget: NPC.direction == 0);
        if (NPC.HasValidTarget && NPC.Distance(Main.player[NPC.target].Center) < detectionRange) {
            closestPlayer = NPC.target;
        }

        if (closestPlayerOld != closestPlayer) {
            closestPlayerOld = closestPlayer;

            // Swap Directions
            if (closestPlayer != -1) {
                float directionX = NPC.Center.X + NPC.width / 2 * NPC.direction;
                float directionY = NPC.Center.Y + NPC.height / 2 * NPC.directionY;
                if (NPC.direction != 0 && !Framing.GetTileSafely((int)(directionX / 16f), (int)(NPC.Center.Y / 16f)).IsFullySolid()) {
                    NPC.direction = Math.Sign(NPC.Center.X - Main.player[closestPlayer].Center.X);
                }
                if (!NPC.collideY && NPC.directionY != 0 && !Framing.GetTileSafely((int)(NPC.Center.Y / 16f), (int)(directionY / 16f)).IsFullySolid()) {
                    NPC.directionY = Math.Sign(NPC.Center.Y - Main.player[closestPlayer].Center.Y);
                }
            }
        }

        aequusNPC.statSpeedX += 1f;
        aequusNPC.statSpeedY += 1f;
        if (closestPlayer != -1) {
            aequusNPC.statSpeedX += 4f;
            aequusNPC.statSpeedY += 4f;
        }

        // Snail is on wall or ceiling.
        if ((int)NPC.ai[1] == 1 || ((int)NPC.ai[1] == 0 && NPC.directionY == -1)) {
            wallTime += (int)aequusNPC.statSpeedY;
            float speedReduction = Math.Clamp(1f - wallTime / 350f, -0.2f, 1f);
            if (NPC.directionY == -1) {
                aequusNPC.statSpeedY *= speedReduction;
            }
            aequusNPC.statSpeedX *= Math.Max(speedReduction, 0f);

            if (wallTime > 400) {
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
                NPC.netUpdate = true;

                NPC.position.X -= NPC.direction;
                NPC.position.Y -= NPC.directionY;
                //NPC.velocity.X -= NPC.direction * Main.rand.NextFloat(4f, 8f);
                NPC.velocity = Vector2.Zero;

                wallTime = GetUpTime - 30;
                if (Main.rand.NextBool() && Main.netMode != NetmodeID.MultiplayerClient) {
                    wallTime = -480;
                }
            }
        }
        else {
            wallTime = 0;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        bool onWall = false;
        var texture = TextureAssets.Npc[Type].Value;
        var frame = texture.Frame(verticalFrames: Main.npcFrameCount[Type], frameY: onWall ? 1 : 0);
        drawColor = NPC.GetNPCColorTintedByBuffs(drawColor);
        var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        Vector2 down = (NPC.rotation + MathHelper.PiOver2).ToRotationVector2();
        Vector2 offset = new Vector2(0f, NPC.gfxOffY + DrawOffsetY);
        Vector2 bodyDrawCoordinates = NPC.Center + down * (NPC.height - frame.Height * NPC.scale + 8f) / 2f + offset - screenPos;
        Vector2 tailCoordinates = NPC.Center + down * (NPC.height - 2f) / 2f + offset - screenPos + down.RotatedBy(MathHelper.PiOver2 * NPC.spriteDirection) * frame.Size() / 2f * NPC.scale;

        spriteBatch.Draw(texture, tailCoordinates, frame with { Y = frame.Y + frame.Height * 2 }, drawColor, tailRotation, new Vector2(NPC.spriteDirection == 1 ? (frame.Width - 4) : 4, frame.Height - 5f), NPC.scale, effects, 0f);
        spriteBatch.Draw(texture, bodyDrawCoordinates, frame, drawColor, NPC.rotation, frame.Size() / 2f, NPC.scale, effects, 0f);

        //spriteBatch.Draw(AequusTextures.BaseParticleTexture, tailCoordinates, new(0, 0, 10, 10), Color.Red, NPC.rotation, new(5f, 5f), NPC.scale, effects, 0f);
        return false;
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        if (NPC.life <= 0) {
            for (int i = 0; i < 10; i++) {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, Main.rand.Next(_dustType), hit.HitDirection * 2f, -2f);
            }

            var source = NPC.GetSource_Death();
            if (Mod.TryFind<ModGore>(Name + "GoreHead", out var headGore)) {
                Gore.NewGore(source, NPC.position, NPC.velocity, headGore.Type);
            }
            if (Mod.TryFind<ModGore>(Name + "GoreTail", out var tailGore)) {
                Gore.NewGore(source, NPC.position, NPC.velocity, tailGore.Type);
            }
            return;
        }
        for (int i = 0; i < hit.Damage / NPC.lifeMax * 20; i++) {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, Main.rand.Next(_dustType), hit.HitDirection, -1f);
        }
    }

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 4;
    }

    public void AddRecipes(Aequus aequus) {
        (ModNPC, ModNPC) pair = HorseshoeCrabInitializer.HorseshoeCrabs.Find(p => p.Item1 == this);

        if (pair == default((ModNPC, ModNPC))) {
            return;
        }

        BestiaryBuilder.InsertEntry(pair.Item2, ContentSamples.NpcBestiarySortingId[HorseshoeCrabInitializer.BestiaryHorseshoeCrabAnchor.Type]);
        HorseshoeCrabInitializer.BestiaryHorseshoeCrabAnchor = pair.Item2;
    }
}
