﻿using Aequus.Common.Items.DropRules;
using Aequus.Common.NPCs.Bestiary;
using Aequus.Content.Events.GaleStreams;
using Aequus.Core.ContentGeneration;
using ReLogic.Content;
using System.IO;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Old.Content.Enemies.GaleStreams.Vraine;

[BestiaryBiome<GaleStreamsZone>()]
[AutoloadBanner]
public class Vraine : ModNPC {
    public const int Temperature = 40;
    public int transitionMax;
    public int temperature;

    private static Asset<Texture2D> HotTexture => AequusTextures.Vraine_Hot;
    private static Asset<Texture2D> ColdTexture => AequusTextures.Vraine_Cold;

    public override void SetStaticDefaults() {
        Main.npcFrameCount[NPC.type] = 16;

        NPCSets.TrailingMode[NPC.type] = 7;
        NPCSets.TrailCacheLength[NPC.type] = 20;
        ItemSets.KillsToBanner[BannerItem] = 100;
        NPCSets.NPCBestiaryDrawOffset.Add(Type, new() {
            Rotation = MathHelper.Pi,
        });
        NPCSets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
    }

    public override void SetDefaults() {
        NPC.width = 46;
        NPC.height = 36;
        NPC.lifeMax = 160;
        NPC.damage = 40;
        NPC.defense = 15;
        NPC.HitSound = SoundID.DD2_GoblinHurt;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.aiStyle = -1;
        NPC.noGravity = true;
        NPC.knockBackResist = 0.1f;
        NPC.value = Item.buyPrice(silver: 3);
        NPC.noTileCollide = true;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment) {
        if (AequusSystem.HardmodeTier) {
            NPC.lifeMax = (int)(NPC.lifeMax * 1.375f);
        }
        else {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f);
            NPC.damage = (int)(NPC.damage * 0.8f);
        }
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry);
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (NPC.life <= 0) {
            for (int i = 0; i < 20; i++) {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X * 0.25f, NPC.velocity.Y * 0.25f, 0, default(Color), Main.rand.NextFloat(1f, 1.45f));
            }
            //if ((int)NPC.ai[1] == 1)
            //{
            //    AQGore.NewGore(NPC.Center + Vector2.Normalize(NPC.oldVelocity) * NPC.width / 2f,
            //        NPC.velocity * 0.25f, "GaleStreams/brollow_1");
            //}
            //else
            //{
            //    AQGore.NewGore(NPC.Center + Vector2.Normalize(NPC.oldVelocity) * NPC.width / 2f,
            //        NPC.velocity * 0.25f, "GaleStreams/brollow_0");
            //}
            //AQGore.NewGore(NPC.getRect(), NPC.velocity.RotatedBy(-0.1f) * 0.25f, "GaleStreams/brollow_2");
            //AQGore.NewGore(NPC.getRect(), NPC.velocity.RotatedBy(0.1f) * 0.25f, "GaleStreams/brollow_3");
        }
        else {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X, NPC.velocity.Y);
        }
    }

    public override void AI() // ai[1] is temperature (1 = hot, 2 = cold)
    {
        if ((int)NPC.ai[1] == -1) {
            if (NPC.timeLeft > 100) {
                NPC.timeLeft = 100;
                transitionMax = 100;
                NPC.ai[3] = transitionMax;
                NPC.netUpdate = true;
            }
            NPC.velocity.Y -= 0.25f;
            temperature = 0;
            return;
        }
        if ((int)NPC.ai[1] == 0) {
            NPC.TargetClosest(faceTarget: false);
            if (!NPC.HasValidTarget) {
                NPC.ai[1] = -1f;
                return;
            }
            NPC.ai[1] = 2f;
            NPC.ai[2] = -1f;
            int count = Main.rand.Next(3) + 1;

            if (Main.remixWorld && Main.getGoodWorld) {
                count += Main.rand.Next(3);
            }

            int npcX = (int)NPC.position.X + NPC.width / 2;
            int npcY = (int)NPC.position.Y + NPC.height / 2;
            int lastNPC = NPC.whoAmI;
            int lastNPC2 = NPC.whoAmI;
            for (int i = 0; i < count; i++) {
                int n = NPC.NewNPC(NPC.GetSource_FromAI(), npcX + NPC.width * (i + 1), npcY, NPC.type, NPC.whoAmI);
                Main.npc[n].ai[1] = NPC.ai[1];
                Main.npc[n].ai[2] = lastNPC;
                Main.npc[n].localAI[0] = NPC.localAI[0];
                Main.npc[n].target = NPC.target;
                lastNPC = n;
                n = NPC.NewNPC(NPC.GetSource_FromAI(), npcX - NPC.width * (i + 1), npcY, NPC.type, NPC.whoAmI);
                Main.npc[n].ai[1] = NPC.ai[1];
                Main.npc[n].ai[2] = lastNPC2;
                Main.npc[n].localAI[0] = NPC.localAI[0];
                Main.npc[n].target = NPC.target;
                lastNPC2 = n;
            }
            NPC.netUpdate = true;
        }
        else if (!NPC.HasValidTarget) {
            NPC.ai[1] = -1f;
        }

        if (NPC.collideX && NPC.oldVelocity.X.Abs() > 2f)
            NPC.velocity.X = -NPC.oldVelocity.X * 0.8f;
        if (NPC.collideY && NPC.oldVelocity.Y.Abs() > 2f)
            NPC.velocity.Y = -NPC.oldVelocity.Y * 0.8f;

        bool hot = (int)NPC.ai[1] == 1;
        NPC.coldDamage = !hot;
        var center = NPC.Center;
        if (NPC.ai[3] > 0f) {
            NPC.ai[3]--;
            if (NPC.ai[3] <= 0) {
                NPC.ai[3] = 0f;
                temperature = (sbyte)(Temperature * (hot ? 1 : -1));
                NPC.netUpdate = true;
            }
            else if (transitionMax != 0) {
                temperature = (sbyte)(int)MathHelper.Lerp(temperature, Temperature * (hot ? 1 : -1), 1f - NPC.ai[3] / transitionMax);
            }
        }
        if ((int)NPC.ai[2] == -1) // leader
        {
            if (hot) {
                var plrCenter = Main.player[NPC.target].Center;
                Vector2 difference = plrCenter - center;
                float lerpAmount = 0.01f;
                if (NPC.ai[3] <= 0f && difference.Length() > 460f) {
                    NPC.ai[1] = 2f;
                    transitionMax = 100;
                    NPC.ai[3] = transitionMax;
                    NPC.netUpdate = true;
                }
                float length = NPC.velocity.Length();
                if (Main.expertMode) {
                    if (length < 4.85f) {
                        length = 4.85f;
                    }
                }
                else {
                    if (length < 2.65f) {
                        length = 2.65f;
                    }
                }
                NPC.velocity = Vector2.Normalize(Vector2.Lerp(NPC.velocity, difference, lerpAmount)) * length;
                NPC.direction = NPC.velocity.X > 0f ? 1 : -1;
                if (NPC.direction == 1) {
                    if (center.X > plrCenter.X) {
                        NPC.ai[1] = 2f;
                        transitionMax = 120;
                        NPC.ai[3] = transitionMax;
                        NPC.netUpdate = true;
                    }
                }
                else {
                    if (center.X < plrCenter.X) {
                        NPC.ai[1] = 2f;
                        transitionMax = 120;
                        NPC.ai[3] = transitionMax;
                        NPC.netUpdate = true;
                    }
                }
            }
            else {
                if (NPC.ai[3] <= 0f) {
                    Vector2 difference = Main.player[NPC.target].Center - center;
                    if (NPC.ai[3] < 0f) {
                        var gotoVeloc = Vector2.Normalize(difference) * (Main.expertMode ? 7.5f : 4.85f);
                        NPC.ai[3]++;
                        NPC.velocity = Vector2.Lerp(NPC.velocity, gotoVeloc, 0.1f);
                        if (NPC.ai[3] == -1) {
                            transitionMax = 100;
                            NPC.ai[1] = 1f;
                            NPC.ai[3] = transitionMax;
                            NPC.velocity = gotoVeloc;
                            SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                        }
                    }
                    else {
                        if (difference.Y < 500f && difference.Y > 375f) {
                            NPC.netUpdate = true;
                            NPC.ai[3] = -20f;
                        }
                        else if (difference.Y > 200f) {
                            NPC.velocity.Y *= 0.98f;
                        }
                        else if (difference.Length() < 360f) {
                            NPC.ai[1] = 1f;
                            transitionMax = 60;
                            NPC.ai[3] = transitionMax;
                            NPC.netUpdate = true;
                        }

                        float maxSpeedY = -8f;
                        if (Main.player[NPC.target].velocity.Y < -8f) {
                            maxSpeedY = Main.player[NPC.target].velocity.Y;
                        }
                        if (NPC.velocity.Y < -maxSpeedY) {
                            NPC.velocity.Y += Main.rand.NextFloat(-0.0025f, -0.1f);
                        }

                        float diffX = Main.player[NPC.target].position.X + Main.player[NPC.target].width / 2f - (NPC.position.X + NPC.width / 2f);
                        if (Main.player[NPC.target].position.X + Main.player[NPC.target].width / 2f < NPC.position.X) {
                            if (NPC.velocity.X > -2f) {
                                NPC.velocity.X -= 0.035f;
                            }
                        }
                        else {
                            if (NPC.velocity.X < 2f) {
                                NPC.velocity.X += 0.035f;
                            }
                        }
                    }
                }
                else {
                    if (NPC.velocity.Y > 0f) {
                        NPC.velocity.Y += Main.rand.NextFloat(-0.0025f, -0.1f);
                    }
                    if (NPC.velocity.X > -2f && NPC.velocity.X < 2f) {
                        NPC.velocity.X += 0.035f * NPC.direction;
                    }
                    else if (NPC.velocity.X < -3f && NPC.velocity.X > 3f) {
                        NPC.velocity.X *= 0.99f;
                    }
                }
            }
        }
        else {
            var leader = Main.npc[(int)NPC.ai[2]];
            if (!leader.active) {
                int closestNPC = -1;
                float closestDistance = NPC.width * 1.75f;
                for (int i = 0; i < Main.maxNPCs; i++) {
                    if (i != NPC.whoAmI && Main.npc[i].active && Main.npc[i].type == NPC.type && (int)Main.npc[i].ai[2] != NPC.whoAmI) {
                        var difference = Main.npc[i].Center - center;
                        float differenceLength = difference.Length();
                        if (differenceLength < closestDistance) {
                            closestNPC = i;
                            closestDistance = differenceLength;
                        }
                    }
                }
                if (closestNPC == -1) {
                    NPC.ai[2] = -1f;
                    return;
                }
                NPC.ai[2] = closestNPC;
            }

            var vraine = (Vraine)leader.ModNPC;
            if (vraine == null) {
                return;
            }
            NPC.ai[1] = leader.ai[1];
            NPC.ai[3] = leader.ai[3];
            transitionMax = vraine.transitionMax;

            if ((int)leader.ai[2] == -1) {
                NPC.direction = 1;
                for (int i = 0; i < Main.maxNPCs; i++) {
                    if (i == NPC.whoAmI) {
                        break;
                    }
                    if (Main.npc[i].active && Main.npc[i].type == NPC.type && i < NPC.whoAmI && (int)Main.npc[i].ai[2] == (int)NPC.ai[2]) {
                        NPC.direction = -1;
                        break;
                    }
                }
            }
            else {
                NPC.direction = leader.direction;
            }
            var gotoPosition = leader.Center + new Vector2(-NPC.width / 2f, NPC.height * NPC.direction * 1.5f).RotatedBy(leader.rotation);
            var difference2 = gotoPosition - center;
            float distance = difference2.Length();
            if (distance < 10f) {
                NPC.velocity = Vector2.Lerp(NPC.velocity, leader.velocity, 0.8f);
            }
            else {
                float speed = NPC.velocity.Length();
                float leaderSpeed = leader.velocity.Length();
                if (speed < leaderSpeed * 1.6f) {
                    speed = leaderSpeed * 1.6f;
                }
                else if (speed < 2.5f) {
                    speed = 2.5f;
                }
                NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(difference2) * speed, 0.025f);
            }
        }
        if (NPC.velocity.Length() < 2f) {
            NPC.velocity = NPC.velocity.SafeNormalize(-Vector2.UnitY) * 2f;
        }
        NPC.rotation = NPC.velocity.ToRotation();
    }

    public override bool? CanFallThroughPlatforms() {
        return true;
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(transitionMax);
        writer.Write(temperature);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        transitionMax = reader.ReadInt32();
        temperature = reader.ReadInt32();
    }

    public override void FindFrame(int frameHeight) {
        NPC.frameCounter += 1.0d;
        if (NPC.frameCounter > 4.0d) {
            NPC.frameCounter = 0.0d;
            NPC.frame.Y += frameHeight;
            if (NPC.frame.Y / frameHeight >= Main.npcFrameCount[NPC.type]) {
                NPC.frame.Y = 0;
            }
        }
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        // These only drop when you slay the last Vraine in a group.
        LeadingConditionRule lastAliveRule = new LeadingConditionRule(new ConditionLastAlive(Type));

        lastAliveRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.Melee.Vrang.Vrang>(), chanceDenominator: 3));

        npcLoot.Add(lastAliveRule);

        // These rules will drop on any Vraine killed.
        npcLoot.Add(ItemDropRule.Common(ItemID.FastClock, chanceDenominator: 100));
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) {
        if (Main.rand.NextBool(Main.expertMode ? 2 : 8)) {
            target.AddBuff(BuffID.Slow, 600);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        Texture2D texture = TextureAssets.Npc[Type].Value;
        var offset = new Vector2(NPC.width / 2f, NPC.height / 2f);
        Vector2 origin = NPC.frame.Size() / 2f;
        Vector2 drawPos = NPC.Center - screenPos;

        if ((int)NPC.localAI[0] == 0) {
            float mult = 1f / NPCSets.TrailCacheLength[NPC.type];
            var trailColor = drawColor * 0.1f;
            for (int i = 0; i < NPCSets.TrailCacheLength[NPC.type]; i++) {
                if ((int)NPC.oldPos[i].X == 0)
                    break;
                Main.spriteBatch.Draw(texture, NPC.oldPos[i] + offset - Main.screenPosition, NPC.frame, trailColor * (mult * (NPCSets.TrailCacheLength[NPC.type] - i)), NPC.oldRot[i], origin, NPC.scale, SpriteEffects.None, 0f);
            }
        }

        Main.spriteBatch.Draw(texture, drawPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);

        if (NPC.ai[3] > 0f && transitionMax > 0) {
            float progress = NPC.ai[3] / transitionMax;
            progress *= 2f;
            progress -= 1f;
            Texture2D overlayTexture;
            switch (NPC.ai[1]) {
                case -1:
                    progress = 1f - progress;
                    overlayTexture = HotTexture.Value;
                    break;
                case 1:
                    overlayTexture = HotTexture.Value;
                    break;
                default:
                    overlayTexture = ColdTexture.Value;
                    break;
            }
            Main.spriteBatch.Draw(overlayTexture, drawPos, NPC.frame, Color.Lerp(NPC.GetNPCColorTintedByBuffs(drawColor), new Color(0, 0, 0, 0), progress), NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
        }

        return false;
    }
}