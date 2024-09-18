#if !CRAB_CREVICE_DISABLE
using Aequus.Buffs.Debuffs;
using Aequus.Common.NPCs;
using Aequus.Common.Utilities;
using Aequus.Content.Biomes.CrabCrevice;
using Aequus.Items.Equipment.Accessories.Money.BusinessCard;
using Aequus.Items.Equipment.Accessories.Money.FaultyCoin;
using Aequus.Items.Equipment.Accessories.Money.FoolsGoldRing;
using Aequus.Items.Materials.PearlShards;
using Aequus.Projectiles.Monster;
using Aequus.Tiles.Banners.Items;
using System;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;

namespace Aequus.NPCs.Monsters.CrabCrevice;
public class SoldierCrab : ModNPC {
    public const int FramesX = 2;

    private bool _setupFrame;
    public int frameIndex;

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 9;
    }

    public override void SetDefaults() {
        NPC.width = 40;
        NPC.height = 30;
        NPC.lifeMax = 75;
        NPC.damage = 50;
        NPC.knockBackResist = 0.1f;
        NPC.aiStyle = -1;
        NPC.defense = 8;
        NPC.value = Item.buyPrice(silver: 3);
        NPC.HitSound = SoundID.NPCHit2;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.waterMovementSpeed = 0.9f;

        Banner = NPC.type;
        BannerItem = ModContent.ItemType<SoldierCrabBanner>();

        this.SetBiome<CrabCreviceBiome>();
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        this.CreateLoot(npcLoot)
            .AddOptions(chance: 15, ModContent.ItemType<BusinessCard>(), ModContent.ItemType<FaultyCoin>(), ModContent.ItemType<FoolsGoldRing>())
            .Add<PearlShardWhite>(chance: 5, stack: 1)
            .Add<PearlShardBlack>(chance: 10, stack: 1)
            .Add<PearlShardPink>(chance: 15, stack: 1)
            .Add(ItemID.ArmorPolish, chance: 25, stack: 1);
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (NPC.life <= 0) {
            for (int i = 0; i < 10; i++) {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X, NPC.velocity.X, 0, default(Color), 1.25f);
            }
            for (int i = 0; i < 10; i++) {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Cobalt, NPC.velocity.X, NPC.velocity.X, 0, default(Color), 1.25f);
            }
        }
        Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X, NPC.velocity.X, 0, default(Color), 0.9f);
        Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Cobalt, NPC.velocity.X, NPC.velocity.X, 0, default(Color), 0.9f);
    }

    public override void AI() {
        switch ((int)NPC.ai[0]) {
            case 0:
            case 2: {
                    if (NPC.velocity.Y.Abs() <= 0.1f) {
                        NPC.ai[1]++;
                        if ((int)NPC.ai[0] != 2 && Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[1] > 10f
                            && Collision.CanHitLine(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height) && Main.rand.NextBool(60)) {
                            NPC.netUpdate = true;
                            NPC.ai[1] = 0f;
                            NPC.ai[0] = 1f;
                        }
                        if (NPC.velocity.Y == 0f && (NPC.targetRect.X - (NPC.position.X + NPC.width / 2f)).Abs() < 100f) {
                            NPC.ai[1] += 1.5f;
                        }
                        if (NPC.ai[1] > 110f) {
                            frameIndex = 1;
                        }
                        if (NPC.ai[1] > 120f) {
                            NPC.netUpdate = true;
                            NPC.ai[0] = 0f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                            NPC.TargetClosest(faceTarget: true);
                            NPC.velocity.Y = -8f;
                            NPC.velocity.X = NPC.direction * 6f;
                            if ((NPC.targetRect.X - (NPC.position.X + NPC.width / 2f)).Abs() < 240f) {
                                NPC.velocity.X = (NPC.targetRect.X - (NPC.position.X + NPC.width / 2f)) / 40f;
                                if (NPC.position.Y - 120f > NPC.targetRect.Y) {
                                    NPC.velocity.Y = Math.Max((NPC.targetRect.Y - NPC.position.Y) / 18f, -15f);
                                }
                            }
                        }
                    }
                    else {
                        if (NPC.ai[1] > 0f) {
                            NPC.ai[1]--;
                            if (NPC.ai[1] < 0f) {
                                NPC.ai[1] = 0f;
                            }
                        }
                        if (NPC.velocity.X.Abs() < 6f) {
                            NPC.velocity.X += NPC.direction * 0.1f;
                        }
                    }
                    NPC.rotation = NPC.velocity.X * 0.05f * NPC.velocity.Y * 0.2f;
                    if (NPC.velocity.Y.Abs() < 0.1f) {
                        NPC.velocity.X *= 0.8f;
                    }
                }
                break;

            case 1: {
                    if (NPC.velocity.Y == 0f) {
                        NPC.TargetClosest(faceTarget: true);
                        NPC.velocity.X *= 0.9f;
                        if (NPC.ai[2] > 0f) {
                            NPC.ai[2] -= 0.5f;
                            if (NPC.ai[2] < 0f) {
                                NPC.ai[2] = 0f;
                            }
                        }
                        NPC.ai[1]++;
                        frameIndex = 9 + (int)(NPC.ai[1] / 35f * 7f);
                        if ((int)NPC.ai[1] == 25) {
                            SoundEngine.PlaySound(SoundID.Item71.WithPitch(0.7f).WithVolume(0.7f), NPC.Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient) {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(NPC.targetRect.Center.ToVector2() - NPC.Center) * 6f,
                                    ModContent.ProjectileType<SoldierCrabProj>(), NPC.FixedDamage() / 3, 1f, Main.myPlayer);
                            }
                        }
                        if (NPC.ai[1] > 35f) {
                            NPC.ai[0] = 2f;
                            NPC.ai[1] = 100f;
                            NPC.ai[2] = 0f;
                            NPC.netUpdate = true;
                        }
                    }
                    else {
                        NPC.ai[2]++;
                        if (NPC.ai[2] > 60f) {
                            NPC.ai[0] = 2f;
                            NPC.ai[1] = 100f;
                            NPC.ai[2] = 0f;
                            NPC.netUpdate = true;
                        }
                    }
                }
                break;
        }
        NPC.spriteDirection = NPC.direction;
    }

    public override void FindFrame(int frameHeight) {
        if (Main.netMode == NetmodeID.Server)
            return;
        if (!_setupFrame) {
            _setupFrame = true;
            NPC.frame.Width = NPC.frame.Width / FramesX;
        }

        if (NPC.velocity.Y > 0f) {
            NPC.frameCounter = 0.0;
            frameIndex = NPC.velocity.Y > 4f ? 5 : 4;
        }
        else if (NPC.velocity.Y < 0f) {
            NPC.frameCounter = 0.0;
            frameIndex = NPC.velocity.Y < -5f ? 2 : 3;
        }
        if (NPC.velocity.Y == 0f && frameIndex >= 4 && frameIndex < 9) {
            frameIndex = Math.Max(frameIndex, 6);
            NPC.frameCounter++;
            if (NPC.frameCounter > 4) {
                NPC.frameCounter = 0.0;
                frameIndex++;
            }
        }

        NPC.frame.Y = frameIndex * frameHeight;

        if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type]) {
            NPC.frame.X = NPC.frame.Width * (NPC.frame.Y / (frameHeight * Main.npcFrameCount[NPC.type]));
            NPC.frame.Y = NPC.frame.Y % (frameHeight * Main.npcFrameCount[NPC.type]);
        }
        else {
            NPC.frame.X = 0;
        }
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) {
        if (Main.rand.NextBool(Main.expertMode ? 1 : 2)) {
            target.AddBuff(ModContent.BuffType<PickBreak>(), 480);
        }
        if (Main.rand.NextBool(Main.expertMode ? 2 : 8)) {
            target.AddBuff(BuffID.BrokenArmor, 600);
        }
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo) {
        return spawnInfo.Player.InModBiome<CrabCreviceBiome>() ? 0.2f : 0f;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        spriteBatch.Draw(TextureAssets.Npc[Type].Value, NPC.Center + new Vector2(0f, NPC.gfxOffY) - screenPos,
            NPC.frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        return false;
    }

    public override bool? CanFallThroughPlatforms() {
        return NPC.HasValidTarget && Main.player[NPC.target].position.Y > NPC.position.Y + NPC.height;
    }
}
#endif