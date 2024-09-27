using Aequus.Common.Entities.Banners;
using Aequus.Common.Entities.Bestiary;
using Aequus.Content.Biomes.PollutedOcean;
using System;
using Terraria.Audio;

namespace Aequus.Content.Monsters.PollutedOcean.Conductor;

[AutoloadBanner]
[BestiaryBiome<PollutedOceanUnderground>()]
public partial class Conductor : ModNPC {
    #region States
    /// <summary>Targetting state. The conductor walks towards the player and activates his attack here.</summary>
    public const int A_TARGETING = 0;

    /// <summary>Upon ai[1] reaching this value, set <see cref="State"/> to <see cref="A_TELEPORT"/>.</summary>
    public const int TARGETTING_TELEPORT_TIME_THRESHOLD = -60;
    /// <summary>This tick value is subtracted to ai[1] every AI tick when the Conductor cannot see the player. Upon reaching <see cref="TARGETTING_TELEPORT_TIME_THRESHOLD"/>, set <see cref="State"/> to <see cref="A_TELEPORT"/>.</summary>
    public const float TARGETTING_TELEPORT_TICK = 0.2f;
    /// <summary>Upon ai[1] reaching this value, set <see cref="State"/> to <see cref="A_ATTACKING"/>.</summary>
    public const int TARGETTING_ATTACK_TIME_THRESHOLD = 120;

    public const int A_ATTACKING = 1;

    public const int ATTACK_TIME = 360;
    public const int ATTACK_SHOOT_TIME = 280;
    public const float ATTACK_SHOOT_VELOCITY_CLASSIC = 6f;
    public const float ATTACK_SHOOT_VELOCITY_EXPERT = 10f;
    public const int ATTACK_RATE = 32;
    public const int ATTACK_RANGE_X = 20;
    public const int ATTACK_RANGE_Y = 10;

    /// <summary>Dodge state. The Conductor slides backwards to avoid melee confrontation with the player. 
    /// Upon the state ending if the distance is greater than <see cref="MELEE_DISTANCE"/>, set <see cref="State"/> to <see cref="A_TARGETING"/>, otherwise <see cref="A_TELEPORT"/>.</summary>
    public const int A_SLIDE_BACK = 2;

    /// <summary>How long the Conductor must be in the <see cref="A_SLIDE_BACK"/> state even if he has no horizontal velocity.</summary>
    public const int SLIDE_BACK_REQUIRED_TIME = 50;
    /// <summary>How many times the Conductor can enter the <see cref="A_SLIDE_BACK"/> state. Tracked using ai[2]. ai[2] is reset in <see cref="A_ATTACKING"/>.</summary>
    public const int SLIDE_BACK_COUNT = 1;
    /// <summary>Sets ai[1] (attack delay timer) to this value upon entering <see cref="A_TARGETING"/>. This makes the Conductor enter the attack state faster.</summary>
    public const int SLIDE_ATTACK_AGGRESSION = TARGETTING_ATTACK_TIME_THRESHOLD / 2 + 10;
    public const float SLIDE_BACK_VELOCITY_MULTIPLIER = 0.93f;
    public const float SLIDE_BACK_SPEED = 12f;

    public const int A_TELEPORT = 3;
    /// <summary>How many update ticks to waste teleporting when <see cref="Main.zenithWorld"/> equals <see langword="true"/>.</summary>
    public const int FTW_TELEPORT_SPAM = 40;

    /// <summary>The distance in pixels which is considered "Melee distance"</summary>
    public const int MELEE_DISTANCE = 96;
    #endregion

    private int State {
        get => (int)NPC.ai[0];
        set {
            NPC.ai[0] = value;
            NPC.ai[1] = 0f;
            NPC.netUpdate = true;
        }
    }

    public override void AI() {
        switch (State) {
            case A_TARGETING:
                Action_Targeting();
                break;

            case A_ATTACKING:
                Action_Attacking();
                break;

            case A_SLIDE_BACK:
                Action_SlideBack();
                break;

            case A_TELEPORT:
                Action_Teleport();
                break;
        }

        NPC.spriteDirection = NPC.direction;
    }

    private void Action_Targeting() {
        if (CheckForceTP()) {
            return;
        }

        bool walk = false;
        NPC.TargetClosest(faceTarget: true);
        if (NPC.ai[1] < TARGETTING_TELEPORT_TIME_THRESHOLD) {
            State = A_TELEPORT;
        }
        else if (NPC.HasValidTarget && Collision.CanHitLine(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height)) {
            NPC.ai[1]++;
            float distance = NPC.Distance(Main.player[NPC.target].Center);
            if (distance < MELEE_DISTANCE && NPC.ai[2] < SLIDE_BACK_COUNT) {
                State = A_SLIDE_BACK;
            }
            else if (NPC.ai[3] < 2f && distance > MELEE_DISTANCE * 3f) {
                walk = true;
            }

            if (NPC.ai[1] > TARGETTING_ATTACK_TIME_THRESHOLD && NPC.velocity.Y == 0f && NPC.velocity.X == 0f) {
                State = A_ATTACKING;
                NPC.ai[2] = 0f;
            }
        }
        else {
            walk = true;
            NPC.ai[3]--;
            NPC.ai[1] -= TARGETTING_TELEPORT_TICK;
        }

        if (walk) {
            NPC.localAI[0] = 1f;
            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X + NPC.direction * 0.066f, -2f, 2f);
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
        }
        else {
            NPC.localAI[0] = 0f;
            if (NPC.velocity.Y == 0f) {
                NPC.velocity.X *= 0.9f;
                if (Math.Abs(NPC.velocity.X) < 0.1f) {
                    NPC.velocity.X = 0f;
                }
            }
        }
    }

    private void Action_Attacking() {
        GetAttackTimings(out int rate, out int duration, out _);

        if (CheckForceTP()) {
            return;
        }

        if (NPC.velocity.Y == 0f) {
            NPC.velocity.X *= 0.66f;
        }

        if ((int)NPC.ai[1] == 0) {
            SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
        }

        int timer = (int)NPC.ai[1];
        bool hasNotReachedHalfWay = timer * 2 <= duration;
        if (timer < rate || !hasNotReachedHalfWay) {
            NPC.TargetClosest(faceTarget: true);
        }
        if (timer > duration) {
            State = A_TELEPORT;
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;
        }
        else if (timer >= rate && timer % rate == 0 && hasNotReachedHalfWay) {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                CastNewWaterSphere((timer / 14 + 3) * NPC.direction);
            }
        }

        NPC.ai[1]++;
    }

    private void CastNewWaterSphere(int forward) {
        Vector2 spawnPosition = NPC.Center;
        Point npcTileCoordinates = NPC.Center.ToTileCoordinates();
        int minX = Math.Max(npcTileCoordinates.X - 1 + forward, 40);
        int maxX = Math.Min(npcTileCoordinates.X + 1 + forward, Main.maxTilesX - 40);
        int minY = Math.Max(npcTileCoordinates.Y - ATTACK_RANGE_Y, 40);
        int maxY = Math.Min(npcTileCoordinates.Y + ATTACK_RANGE_Y, Main.maxTilesY - 40);
        for (int i = 0; i < 600; i++) {
            int x = Main.rand.Next(minX, maxX);
            int y = Main.rand.Next(minY, maxY);
            Tile tile = Main.tile[x, y];
            if (tile.IsSolid() || tile.LiquidAmount > 0) {
                continue;
            }

            Tile belowTile = Main.tile[x, y + 1];

            if (belowTile.IsSolid()) {
                spawnPosition = new Vector2(x, y).ToWorldCoordinates();
                break;
            }

            if (belowTile.LiquidAmount > 0 && belowTile.LiquidType == LiquidID.Water) {
                spawnPosition = new Vector2(x * 16f + 8f, y * 16f + 28f);
                break;
            }
        }

        NPC.NewNPCDirect(NPC.GetSource_FromAI(), spawnPosition, ModContent.NPCType<ConductorProj>(), NPC.whoAmI, ai2: NPC.whoAmI, target: NPC.target);
    }

    private void CastWaterSphere() {
        Vector2 spawnPosition = NPC.Center;
        Point npcTileCoordinates = NPC.Center.ToTileCoordinates();
        int minX = Math.Max(npcTileCoordinates.X - ATTACK_RANGE_X, 40);
        int maxX = Math.Min(npcTileCoordinates.X + ATTACK_RANGE_X, Main.maxTilesX - 40);
        int minY = Math.Max(npcTileCoordinates.Y - ATTACK_RANGE_Y, 40);
        int maxY = Math.Min(npcTileCoordinates.Y + ATTACK_RANGE_Y, Main.maxTilesY - 40);
        for (int i = 0; i < 600; i++) {
            int x = Main.rand.Next(minX, maxX);
            int y = Main.rand.Next(minY, maxY);
            Tile tile = Main.tile[x, y];
            if (tile.IsSolid() || tile.LiquidAmount > 0) {
                continue;
            }

            Tile belowTile = Main.tile[x, y + 1];

            if (belowTile.IsSolid()) {
                spawnPosition = new Vector2(x, y).ToWorldCoordinates();
                break;
            }

            if (belowTile.LiquidAmount > 0 && belowTile.LiquidType == LiquidID.Water) {
                spawnPosition = new Vector2(x * 16f + 8f, y * 16f + 28f);
                break;
            }
        }

        NPC.NewNPCDirect(NPC.GetSource_FromAI(), spawnPosition, ModContent.NPCType<ConductorProj>(), NPC.whoAmI, ai2: NPC.whoAmI, target: NPC.target);
    }

    private void Action_SlideBack() {
        if (CheckForceTP()) {
            return;
        }

        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

        NPC.velocity.X *= SLIDE_BACK_VELOCITY_MULTIPLIER;

        if (NPC.ai[1] > SLIDE_BACK_REQUIRED_TIME && Math.Abs(NPC.velocity.X) < 1f) {
            NPC.velocity.X = 0f;
            NPC.TargetClosest(faceTarget: true);
            if (NPC.HasValidTarget && NPC.Distance(Main.player[NPC.target].Center) < MELEE_DISTANCE) {
                State = A_TELEPORT;
            }
            else {
                State = A_TARGETING;
                NPC.ai[1] = 40f;
            }
        }

        if ((int)NPC.ai[1] == 0) {
            NPC.TargetClosest(faceTarget: true);
            NPC.ai[2]++;
            NPC.velocity.X = SLIDE_BACK_SPEED * -NPC.direction;
        }

        NPC.ai[1]++;
    }

    private void Action_Teleport() {
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            return;
        }

        NPC.TargetClosest();
        if (!NPC.HasValidTarget) {
            NPC.KillEffects();
            return;
        }

        if (NPC.ai[1] > 5f) {
            State = A_TARGETING;
            return;
        }

        Player target = Main.player[NPC.target];
        Point teleportLocation = target.Center.ToTileCoordinates();
        Vector2 teleportResult = Vector2.Zero;
        for (int i = 0; i < 4000; i++) {
            int x = Math.Clamp(teleportLocation.X + Main.rand.Next(-40, 41), 40, Main.maxTilesX - 40);
            int y = Math.Clamp(teleportLocation.Y + Main.rand.Next(-25, 26), 40, Main.maxTilesX - 40);

            if (!Main.tile[x, y].IsFullySolid()) {
                continue;
            }

            i += 20;
            Vector2 position = new Vector2(x * 16f + 8f - NPC.width / 2f, y * 16f - NPC.height);
            if (Collision.SolidCollision(position, NPC.width, NPC.height - 2)) {
                continue;
            }

            i += 40;
            int closestPlayer = Player.FindClosest(position, NPC.width, NPC.height);
            Player closest = Main.player[closestPlayer];
            if (!closest.active || closest.DeadOrGhost || Vector2.Distance(position, closest.Center) < 120f) {
                continue;
            }

            //Dust.NewDustPerfect(position, DustID.Torch);
            if (Collision.CanHitLine(position, NPC.width, NPC.height, closest.position, closest.width, closest.height)) {
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                for (int k = 0; k < 40; k++) {
                    Terraria.Dust d = Terraria.Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Iron);
                    d.noGravity = true;
                    d.velocity *= 0.5f;
                }
                NPC.position = position;
                NPC.velocity = Vector2.Zero;
                NPC.ai[3]++;
                NPC.TargetClosest(faceTarget: true);
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                for (int k = 0; k < 40; k++) {
                    Terraria.Dust d = Terraria.Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Iron);
                    d.noGravity = true;
                    d.velocity *= 0.5f;
                }

                if (Main.getGoodWorld && NPC.ai[1] < FTW_TELEPORT_SPAM) {
                    NPC.ai[1]++;
                    break;
                }

                State = A_TARGETING;
                break;
            }
        }

        NPC.ai[1]++;
    }

    private bool CheckForceTP() {
        Point bottom = NPC.Bottom.ToTileCoordinates();
        int tileWidth = (int)Math.Ceiling(NPC.width / 16f);
        if (!TileHelper.ScanTiles(new Rectangle(bottom.X, bottom.Y, tileWidth, 3), TileHelper.IsSolid)) {
            State = A_TELEPORT;
            return true;
        }

        return false;
    }

    public static void GetAttackTimings(out int rate, out int attackDuration, out int attackTime) {
        rate = ATTACK_RATE;
        attackDuration = ATTACK_TIME;
        attackTime = ATTACK_SHOOT_TIME;

        if (Main.getGoodWorld) {
            attackTime *= 2;
            attackDuration *= 2;
            rate /= 2;
        }
    }

    public override bool? CanFallThroughPlatforms() {
        return NPC.HasValidTarget && Main.player[NPC.target].position.Y > NPC.Bottom.Y;
    }
}
