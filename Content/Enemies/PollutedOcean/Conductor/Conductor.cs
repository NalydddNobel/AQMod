using Aequus.Core.ContentGeneration;
using System;
using Terraria.Audio;
using Terraria.GameContent;

namespace Aequus.Content.Enemies.PollutedOcean.Conductor;

[AutoloadBanner]
public partial class Conductor : ModNPC {
    public const int A_TARGETING = 0;
    public const int A_ATTACKING = 1;
    public const int A_SLIDE_BACK = 2;
    public const int A_TELEPORT = 3;

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
            case A_TARGETING: {
                    if (NPC.velocity.Y == 0f) {
                        NPC.velocity.X *= 0.9f;
                    }

                    Point bottom = NPC.Bottom.ToTileCoordinates();
                    int tileWidth = (int)Math.Ceiling(NPC.width / 16f);
                    if (!TileHelper.ScanTiles(new Rectangle(bottom.X, bottom.Y, tileWidth, 3), TileHelper.IsSolid)) {
                        State = A_TELEPORT;
                        return;
                    }

                    NPC.TargetClosest(faceTarget: true);
                    if (NPC.ai[1] < -60f) {
                        State = A_TELEPORT;
                    }
                    else if (NPC.HasValidTarget && Collision.CanHitLine(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height)) {
                        NPC.ai[1]++;
                        if (NPC.ai[1] > 60f) {
                            State = A_ATTACKING;
                        }
                    }
                    else {
                        NPC.ai[1] -= 0.2f;
                    }
                }
                break;

            case A_ATTACKING: {
                    State = A_TARGETING;
                }
                break;

            case A_SLIDE_BACK: {

                }
                break;

            case A_TELEPORT: {
                    if (Main.netMode == NetmodeID.MultiplayerClient) {
                        return;
                    }

                    NPC.TargetClosest();
                    if (!NPC.HasValidTarget) {
                        NPC.KillEffects();
                        return;
                    }

                    Player target = Main.player[NPC.target];
                    Point teleportLocation = target.Center.ToTileCoordinates();
                    Vector2 teleportResult = Vector2.Zero;
                    for (int i = 0; i < 10000; i++) {
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
                            SoundEngine.PlaySound(SoundID.Item8);
                            for (int k = 0; k < 40; k++) {
                                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Iron);
                                d.noGravity = true;
                                d.velocity *= 0.5f;
                            }
                            NPC.position = position;
                            NPC.TargetClosest(faceTarget: true);
                            SoundEngine.PlaySound(SoundID.Item8);
                            for (int k = 0; k < 40; k++) {
                                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Iron);
                                d.noGravity = true;
                                d.velocity *= 0.5f;
                            }

                            if (Main.zenithWorld && NPC.ai[1] < 40f) {
                                NPC.ai[1]++;
                                break;
                            }

                            State = A_TARGETING;
                            break;
                        }
                    }
                }
                break;
        }

        NPC.spriteDirection = NPC.direction;
    }

    public override void FindFrame(int frameHeight) {

    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Rectangle frame = NPC.frame;
        Vector2 origin = frame.Size() / 2f;
        origin.X -= 14f * NPC.spriteDirection;
        Vector2 drawCoordinates = NPC.Bottom + new Vector2(0f, origin.Y - frame.Height + 4f);
        drawColor = NPC.GetAlpha(NPC.GetNPCColorTintedByBuffs(drawColor));
        SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        spriteBatch.Draw(texture, drawCoordinates - screenPos, frame, drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);
        return false;
    }
}
