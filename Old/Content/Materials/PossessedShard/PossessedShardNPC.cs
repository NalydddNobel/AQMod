using Aequus.Common.Renaming;
using Aequus.Core.Graphics.Primitives;
using System;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Localization;

namespace Aequus.Old.Content.Materials.PossessedShard;

public class PossessedShardNPC : ModNPC {
    private Item _giveItem;

    public override string Texture => AequusTextures.PossessedShard.Path;

    public override void SetStaticDefaults() {
        NPCSets.TrailingMode[Type] = 7;
        NPCSets.TrailCacheLength[Type] = 35;
        NPCSets.NPCBestiaryDrawOffset[Type] = new() {
            Hide = true,
        };
        NPCSets.ImmuneToRegularBuffs[Type] = true;
    }

    public override LocalizedText DisplayName => ModContent.GetInstance<PossessedShard>().DisplayName;

    public override void SetDefaults() {
        NPC.width = 20;
        NPC.height = 20;
        NPC.lifeMax = 150;
        NPC.damage = 10;
        NPC.defense = 20;
        NPC.noGravity = true;
        NPC.aiStyle = -1;
        NPC.HitSound = AequusSounds.ShardHit;
    }

    public override void HitEffect(NPC.HitInfo hit) {
        int dustAmount = Math.Min(3 + hit.Damage / 16, 16);
        for (int i = 0; i < dustAmount; i++) {
            var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.PurpleCrystalShard, Alpha: 100, Scale: Main.rand.NextFloat(0.6f, 1.2f));
            d.velocity = Vector2.Normalize(d.position - NPC.Center) * (Main.rand.NextFloat(4f) + 1f);
            d.noGravity = true;
            d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.6f);
        }
        if (NPC.life <= 0) {
            for (int i = 0; i < 50; i++) {
                var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.PurpleCrystalShard, Alpha: 100, Scale: Main.rand.NextFloat(0.6f, 1.5f));
                d.velocity = Vector2.Normalize(d.position - NPC.Center) * (Main.rand.NextFloat(8f) + 1f);
                d.noGravity = true;
                d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.3f);
            }
        }

        NPC.localAI[2] += 8 + hit.Damage / 20;
        NPC.ai[2] = -16f;
        NPC.ai[3] = 0f;
        if (NPC.localAI[2] > 16f) {
            NPC.localAI[2] = 16f;
        }
    }

    private void Collide() {
        bool collisionEffects = false;
        if (NPC.collideX && Math.Abs(NPC.oldVelocity.X) > 3f) {
            NPC.velocity.X = -NPC.oldVelocity.X * 0.8f;
            collisionEffects = true;
        }
        if (NPC.collideY && Math.Abs(NPC.oldVelocity.Y) > 4f) {
            NPC.velocity.Y = -NPC.oldVelocity.Y * 0.8f;
            collisionEffects = true;
        }

        if (!collisionEffects) {
            return;
        }

        NPC.velocity = NPC.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f));
        //if (NPC.velocity.Length() < 7f) {
        //    NPC.velocity.Normalize();
        //    NPC.velocity *= 7f;
        //}

        int dustSizeAdd = 10;
        var dustPosition = NPC.position - Vector2.Normalize(NPC.velocity) * (NPC.width + 4) - new Vector2(dustSizeAdd / 2f);
        for (int i = 0; i < 8; i++) {
            var d = Dust.NewDustDirect(dustPosition, NPC.width + dustSizeAdd, NPC.height + dustSizeAdd, DustID.PurpleTorch, Scale: Main.rand.NextFloat(2f));
            d.velocity = Vector2.Normalize(NPC.velocity) * Main.rand.NextFloat(3f);
            d.noGravity = true;
            d.fadeIn = d.scale + 0.5f;
        }

        if (NPC.soundDelay <= 0) {
            SoundEngine.PlaySound(AequusSounds.ShardHit, NPC.Center);
        }

        NPC.localAI[2] = 8f;
        NPC.ai[2] = 8f;
        NPC.soundDelay = 8;
        NPC.netUpdate = true;
    }

    private void UpdateVisuals() {
        if (Main.GameUpdateCount % ((int)NPC.ai[0] == 1 ? 16 : 8) == 0) {
            var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.PurpleTorch, Alpha: 100);
            d.velocity = Vector2.Normalize(d.position - NPC.Center) * (Main.rand.NextFloat(2f) + 1f);
            d.noGravity = true;
            d.fadeIn = d.scale + 0.5f;
        }
        if (NPC.localAI[1] < 1f) {
            NPC.localAI[1] += 0.05f;
            if (NPC.localAI[1] > 1f) {
                NPC.localAI[1] = 1f;
            }
        }
        if (NPC.localAI[2] > 0f) {
            NPC.localAI[2]--;
            if (NPC.localAI[1] < 0f) {
                NPC.localAI[1] = 0f;
            }
        }
        if (NPC.HasValidTarget) {
            NPC.localAI[1] = Math.Min(NPC.localAI[1], NPC.Distance(Main.player[NPC.target].Center) / 120f);
        }
    }

    public override void AI() {
        NPC.TargetClosest(faceTarget: false);
        NPC.noTileCollide = false;

        UpdateVisuals();

        var tileCoords = NPC.Center.ToTileCoordinates();
        NPC.noGravity = true;
        switch ((int)NPC.ai[0]) {
            case 0: {
                    NPC.dontTakeDamage = false;
                    NPC.rotation = MathHelper.WrapAngle(NPC.rotation) * 0.9f;

                    if (Collision.IsWorldPointSolid(NPC.Center, treatPlatformsAsNonSolid: true)) {
                        NPC.localAI[0]++;
                        if (NPC.localAI[0] > 32f) {
                            NPC.noTileCollide = true;
                        }
                    }
                    else {
                        NPC.localAI[0] = 0f;
                        if (!TileHelper.ScanDown(tileCoords, 12, out var _, TileHelper.IsFullySolid, TileHelper.HasAnyLiquid)) {
                            NPC.velocity.Y += 0.6f;
                        }
                        if (!TileHelper.ScanUp(tileCoords, 3, out var _)) {
                            NPC.velocity.Y -= 0.14f;
                        }
                    }

                    if (!NPC.HasValidTarget || NPC.ai[2] > 0f) {
                        break;
                    }

                    float moveSpeed = Main.expertMode ? 0.5f : 0.3f;
                    if ((int)NPC.ai[0] == 1) {
                        moveSpeed /= 2f;
                    }
                    float viewDistance = Player.defaultItemGrabRange * (NPC.ai[1] != 0f ? 10f : 5f);
                    var fleeTarget = Main.player[NPC.target].Center;
                    var dir = NPC.DirectionFrom(fleeTarget);
                    if (NPC.noTileCollide) {
                        NPC.velocity -= dir * 0.3f;
                    }
                    if (NPC.ai[2] < 0f || NPC.Distance(fleeTarget) < viewDistance) {
                        if ((int)NPC.ai[1] == 0) {
                            NPC.ai[1] = 1f;
                            NPC.velocity = dir * 10f;
                            NPC.netUpdate = true;
                        }
                        else if ((int)NPC.ai[1] == 1) {
                            SoundEngine.PlaySound(AequusSounds.Dash with { Volume = 0.6f }, NPC.Center);
                            for (int j = 0; j < 8; j++) {
                                var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.PurpleTorch, Scale: Main.rand.NextFloat(2f));
                                d.velocity = Vector2.Normalize(NPC.velocity) * Main.rand.NextFloat(3f);
                                d.noGravity = true;
                                d.fadeIn = d.scale + 0.5f;
                            }
                            NPC.ai[1] = 2f;
                        }
                        NPC.velocity += dir * moveSpeed;
                        NPC.velocity *= 0.96f;
                    }
                    else {
                        NPC.ai[1] = 0f;
                        NPC.velocity *= 0.86f;
                    }
                }
                break;

            case 1: {
                    NPC.dontTakeDamage = true;
                    NPC.noGravity = false;
                    NPC.rotation += NPC.velocity.X * 0.1f;
                    NPC.velocity.X *= 0.985f;
                    NPC.noTileCollide = false;
                    NPC.localAI[0] = 0f;
                    NPC.ai[3]++;
                    if (NPC.ai[3] > 240f) {
                        NPC.ai[3] = 0f;
                        NPC.ai[0] = 0f;
                        NPC.netUpdate = true;
                    }
                }
                break;
        }

        float grabRange = Player.defaultItemGrabRange * (NPC.ai[0] == 1 ? 3f : 1f);
        _giveItem ??= new Item(ModContent.ItemType<PossessedShard>());
        if (_giveItem.TryGetGlobalItem<RenameItem>(out var renameItem) && NPC.TryGetGlobalNPC<RenameNPC>(out var renameNPC)) {
            renameItem.CustomName = renameNPC.CustomName;
        }
        for (int i = 0; i < Main.maxPlayers; i++) {
            if (!Main.player[i].active || Main.player[i].dead || !Main.player[i].ItemSpace(_giveItem).CanTakeItem) {
                continue;
            }

            float distance = NPC.Distance(Main.player[i].Center);
            if (distance < 32f) {
                if (Main.netMode != NetmodeID.MultiplayerClient) {
                    NPC.active = false;
                    Item.NewItem(NPC.GetSource_FromThis(), Main.player[i].Center, _giveItem);
                }
                return;
            }
            if (distance > grabRange) {
                continue;
            }

            NPC.noGravity = true;
            NPC.netUpdate = true;
            NPC.velocity += NPC.DirectionTo(Main.player[i].Center) * 2f;
            break;
        }

        if (NPC.ai[2] > 0f) {
            NPC.ai[2]--;
            if (NPC.ai[2] < 0f) {
                NPC.ai[2] = 0f;
            }
        }
        else if (NPC.ai[2] < 0f) {
            NPC.ai[2]++;
            if (NPC.ai[2] > 0f) {
                NPC.ai[2] = 0f;
            }
        }
        if (NPC.collideX || NPC.collideY) {
            Collide();
        }
        if (Math.Abs(NPC.velocity.X) < 0.1f && Math.Abs(NPC.velocity.Y) < 0.1f) {
            NPC.velocity = Vector2.Zero;
        }
    }

    public override bool CheckDead() {
        if (NPC.ai[0] == 1) {
            return true;
        }
        NPC.ai[0] = 1f;
        NPC.dontTakeDamage = true;
        NPC.life = NPC.lifeMax / 2;
        NPC.netUpdate = true;
        return false;
    }

    public override bool CanHitNPC(NPC target) {
        return false;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
        return false;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.Lerp(Color.White, lightColor, Helper.Oscillate(Main.GlobalTimeWrappedHourly * 2.5f, 0.66f, 1f));
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        float rotation = NPC.rotation;
        float scale = NPC.scale;
        var texture = TextureAssets.Npc[Type].Value;
        var drawCoordinates = NPC.Center - Main.screenPosition;
        var frame = NPC.frame;
        var origin = frame.Size() / 2f;

        if ((int)NPC.ai[0] != 1) {
            DrawHelper.DrawBasicVertexLine(AequusTextures.Trail2, NPC.oldPos, NPC.oldRot, (p) => Color.BlueViolet with { A = 0 } * MathF.Pow(1f - p, 2f) * NPC.localAI[1] * 0.7f, (p) => 6f - 4f * p, NPC.Size / 2f - screenPos);
        }

        //DrawHelper.ApplyBasicEffect(AequusTextures.Trail3);
        UVVertexStrip.Instance.PrepareUV(1f, 0f);
        UVVertexStrip.Instance.PrepareStripWithProceduralPadding(NPC.oldPos, NPC.oldRot, (p) => Color.BlueViolet with { A = 0 } * MathF.Pow(1f - p, 4f) * NPC.localAI[1] * 1.2f, (p) => 14f + 20f * MathF.Pow(p, 2f), NPC.Size / 2f - screenPos, includeBacksides: true);
        DrawHelper.ApplyUVEffect(AequusTextures.Trail3, new Vector2(1f, 1f), new Vector2((1f - (Main.GlobalTimeWrappedHourly % 1f)) * 1.5f, 0f));
        UVVertexStrip.Instance.DrawTrail();

        //flameTrail.coord1 = 0f;
        //flameTrail.coord2 = 1f;
        //flameTrail.Draw(NPC.oldPos, uvAdd: Main.GlobalTimeWrappedHourly * -1.5f, uvMultiplier: 1.5f);

        float progress = MathF.Pow(1f - Main.GlobalTimeWrappedHourly % 1f, 2f);
        var glowColor = Color.BlueViolet with { A = 0 };
        if (NPC.localAI[2] > 0) {
            drawCoordinates += new Vector2(Main.rand.NextFloat(-NPC.localAI[2], NPC.localAI[2]), Main.rand.NextFloat(-NPC.localAI[2], NPC.localAI[2])) / 1.5f;
        }

        if ((int)NPC.ai[0] != 1) {
            if (progress > 0.5f) {
                float glowProgress = (progress - 0.5f) / 0.5f;
                for (int i = 0; i < 4; i++) {
                    spriteBatch.Draw(
                        texture,
                        drawCoordinates + (i * MathHelper.PiOver2).ToRotationVector2() * 2f * scale,
                        frame,
                        glowColor * glowProgress,
                        rotation,
                        origin,
                        scale, SpriteEffects.None, 0f
                    );
                }
            }

            for (int i = 0; i < 4; i++) {
                spriteBatch.Draw(
                    texture,
                    drawCoordinates + (i * MathHelper.PiOver2 + progress * MathHelper.PiOver2).ToRotationVector2() * (progress * 24f + 2f) * scale,
                    frame,
                    glowColor * (1f - progress),
                    rotation,
                    origin,
                    scale, SpriteEffects.None, 0f
                );
            }
        }
        spriteBatch.Draw(
            texture,
            drawCoordinates,
            frame,
            NPC.GetAlpha(drawColor),
            rotation,
            origin,
            scale, SpriteEffects.None, 0f
        );
        return false;
    }
}