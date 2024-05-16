using Aequus.Common.NPCs;
using Aequus.Content.Dusts;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.Audio;
using Terraria.Graphics.Shaders;

namespace Aequus.Old.Content.DronePylons.NPCs;

public class HealerDrone : TownDroneBase {
    public override DroneSlot Slot => ModContent.GetInstance<HealerDroneSlot>();

    public float healingAuraOpacity;
    public int healingTarget;

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 7;
        NPCSets.NoTownNPCHappiness[Type] = true;
        NPCSets.ActsLikeTownNPC[Type] = true;
    }

    public override void SetDefaults() {
        NPC.width = 20;
        NPC.height = 20;
        NPC.friendly = true;
        NPC.aiStyle = -1;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.knockBackResist = 0f;
        NPC.lifeMax = 250;
        NPC.dontTakeDamage = true;
        NPC.damage = 15;
    }

    public override void AI() {
        base.AI();

        healingAuraOpacity = Math.Clamp(healingAuraOpacity, 0f, 1f);

        var target = FindTarget();

        float targetDistance = 3200f;
        float minDistance = 128f;
        bool noAura = true;
        if (target != null) {
            targetDistance = NPC.Distance(target.Center);
            int healingDistance = Math.Max(NPC.damage, (int)minDistance + 50);
            if (Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height)) {
                healingDistance += 50;
            }
            if (targetDistance < healingDistance) {
                NPC.ai[0]++;
                healingAuraOpacity += 0.015f;
                int healingDelay = 15;
                if (NPCSets.CountsAsCritter[target.type]) {
                    healingDelay = 61;
                }
                if ((int)NPC.ai[0] % healingDelay == 0) {
                    if (healingTarget != target.whoAmI + 1) {
                        healingTarget = target.whoAmI + 1;
                        healingAuraOpacity = 0f;
                        NPC.netUpdate = true;
                    }
                    bool end = (int)NPC.ai[0] >= 30;
                    var shootPosition = NPC.Center + new Vector2(0f, 12f);
                    int healAmt = (int)Math.Max(target.lifeMax * 0.064f, 1);
                    if (target.life + healAmt > target.lifeMax) {
                        healAmt = target.lifeMax - target.life;
                        target.GetGlobalNPC<AequusNPC>().immuneToDamageTime = Math.Max((byte)120, target.GetGlobalNPC<AequusNPC>().immuneToDamageTime);
                    }
                    target.life += healAmt;

                    for (int i = 0; i < NPC.maxBuffs; i++) {
                        if (Main.debuff[target.buffType[i]] || target.buffType[i] == BuffID.ShadowFlame || target.buffType[i] == BuffID.OnFire3 || target.buffType[i] == BuffID.Frostburn2) {
                            target.DelBuff(i);
                            i--;
                        }
                    }

                    if (Main.netMode != NetmodeID.Server) {
                        int c = CombatText.NewText(new Rectangle((int)target.position.X, (int)target.position.Y, target.width, target.height), CombatText.HealLife, healAmt, dot: true);
                        Main.combatText[c].scale *= 0.5f;
                        Main.combatText[c].velocity.X += Main.rand.NextFloat(2f, 4f) * ((int)NPC.ai[0] % 2 == 0 ? -1 : 1);
                    }
                    NPC.netUpdate = true;
                    target.netUpdate = true;
                    SoundEngine.PlaySound(SoundID.Item4 with { Volume = 0.5f, Pitch = 0.75f }, NPC.Center);
                    if (end) {
                        NPC.ai[0] = 0f;
                        NPC.netUpdate = true;
                    }
                }

                if ((int)NPC.ai[0] % 10 == 0) {
                    var d = Dust.NewDustDirect(target.position + new Vector2(0f, target.height - 10), target.width, 10, ModContent.DustType<MonoDust>(), newColor: CombatText.HealLife with { A = 0 });
                    d.velocity.X *= 0.4f;
                    d.velocity.X = Math.Abs(d.velocity.X) * ((int)NPC.ai[0] % 2 == 0 ? -1 : 1);
                    d.velocity.Y -= target.height / 10 * Main.rand.NextFloat(0.5f, 1f);
                }
                noAura = false;
            }
        }

        if (noAura) {
            healingAuraOpacity -= 0.01f + (1f - healingAuraOpacity) * 0.025f;
            minDistance = 20f;
        }

        int tileX = ((int)NPC.position.X + NPC.width / 2) / 16;
        int tileY = ((int)NPC.position.Y + NPC.height / 2) / 16;

        if (target != null) {
            NPC.frameCounter++;
            if (NPC.frameCounter > 7.0 && healingAuraOpacity > 0f) {
                NPC.frame.Y += NPC.frame.Height;
                NPC.frameCounter = 0.0;
                if (NPC.frame.Y >= NPC.frame.Height * Main.npcFrameCount[Type]) {
                    NPC.frame.Y = NPC.frame.Height * 4;
                }
            }
        }
        else if (NPC.frame.Y > 0) {
            NPC.frameCounter++;
            NPC.frame.Y = Math.Min(NPC.frame.Y, NPC.frame.Height * 4);
            if (NPC.frameCounter > 8.0 && healingAuraOpacity < 0.1f) {
                NPC.frameCounter = 0.0;
                NPC.frame.Y -= NPC.frame.Height;
            }
        }
        if (target != null && targetDistance >= minDistance) {
            var normal = Vector2.Normalize(target.Center - NPC.Center + new Vector2(0f, -20f));
            NPC.velocity = Vector2.Lerp(NPC.velocity, normal * (8f + NPC.damage / 90f), 0.025f);
        }
        else if (target != null) {
            if (NPC.velocity.Length() < target.velocity.Length() * 0.1f) {
                if (NPC.Bottom.Y > target.Top.Y) {
                    NPC.velocity.Y -= 0.1f;
                }
                NPC.velocity += target.velocity * 0.05f;
            }
            else {
                NPC.velocity += Main.rand.NextVector2Unit() * 0.1f;
            }
            NPC.velocity *= 0.95f;
            NPC.direction = Math.Sign(target.Center.X - NPC.Center.X);
            if (Math.Sign(target.Center.X - (NPC.Center.X - target.direction * (24f + target.width))) == target.direction) {
                NPC.direction = target.direction;
                NPC.position += target.velocity * 0.8f;
            }
        }
        else if (healingAuraOpacity < 0.1f) {
            DefaultMovement();
        }
        if (target != null) {
            NPC.ai[1] = target.whoAmI + 1;
        }
        else {
            NPC.ai[1] = 0f;
        }
        if (healingAuraOpacity <= 0f) {
            if (target != null) {
                healingTarget = target.whoAmI + 1;
            }
            else {
                healingTarget = 0;
            }
        }
        else {
            Lighting.AddLight(NPC.Center, new Vector3(0.15f, 1f, 0.1f) * healingAuraOpacity);
            if (target != null) {
                Lighting.AddLight(target.Center, new Vector3(0.15f, 1f, 0.1f) * healingAuraOpacity);
            }
        }

        NPC.rotation = NPC.velocity.X * 0.1f;
        NPC.spriteDirection = NPC.direction;
        NPC.CollideWithOthers(0.1f);
    }

    private bool CanBeTargetted(NPC npc, bool countCritter = false) {
        if (!npc.active || npc.HasBuff(BuffID.Stinky) || npc.life >= npc.lifeMax || npc.immortal || npc.dontTakeDamage) {
            return false;
        }

        return npc.townNPC || npc.isLikeATownNPC ? true : countCritter && (npc.friendly || NPCSets.CountsAsCritter[npc.type]);
    }

    public NPC FindTarget() {
        int target = -1;
        if (NPC.ai[1] > 0f) {
            int alreadyChosenTarget = (int)NPC.ai[1] - 1;
            if (CanBeTargetted(Main.npc[alreadyChosenTarget], countCritter: true)) {
                return Main.npc[alreadyChosenTarget];
            }
        }
        float distance = 2400f;
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (CanBeTargetted(Main.npc[i], countCritter: false)) {
                float d = NPC.Distance(Main.npc[i].Center) + Main.npc[i].life * 2;
                if (d < distance) {
                    target = i;
                    distance = d;
                }
            }
        }
        if (target != -1) {
            return Main.npc[target];
        }
        distance = 3200f;
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (CanBeTargetted(Main.npc[i], countCritter: false)) {
                float d = NPC.Distance(Main.npc[i].Center) + Main.npc[i].life * 2;
                if (d < distance && Collision.CanHitLine(Main.npc[i].position, Main.npc[i].width, Main.npc[i].height, NPC.position, NPC.width, NPC.height)) {
                    target = i;
                    distance = d;
                }
            }
        }
        if (target != -1) {
            return Main.npc[target];
        }
        distance = 2400f;
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (CanBeTargetted(Main.npc[i], countCritter: true)) {
                float d = NPC.Distance(Main.npc[i].Center) + Main.npc[i].life * 2;
                if (d < distance && Collision.CanHitLine(Main.npc[i].position, Main.npc[i].width, Main.npc[i].height, NPC.position, NPC.width, NPC.height)) {
                    target = i;
                    distance = d;
                }
            }
        }
        if (target != -1) {
            return Main.npc[target];
        }
        return null;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        NPC.GetDrawInfo(out var texture, out var off, out var frame, out var origin, out int _);

        var drawCoords = NPC.position + off - screenPos;
        var color = GetPylonColor();
        if (HealerDroneRenderer.RenderingNow) {
            spriteBatch.End();
            Main.spriteBatch.BeginWorld(shader: true);
            var s = GameShaders.Armor.GetSecondaryShader(ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex, Main.LocalPlayer);
            s.Apply(null);
            spriteBatch.Draw(texture, drawCoords, frame, NPC.GetNPCColorTintedByBuffs(drawColor),
            NPC.rotation, origin, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }

        spriteBatch.Draw(texture, drawCoords, frame, NPC.GetNPCColorTintedByBuffs(drawColor),
            NPC.rotation, origin, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        spriteBatch.Draw(AequusTextures.HealerDrone_Glow, drawCoords, frame, color * SpawnInOpacity,
            NPC.rotation, origin, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);


        if (healingTarget > 0 && Main.npc[healingTarget - 1].active) {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginWorld(shader: false); ;
            HealerDroneRenderer.Instance.AddHealingAura(healingTarget - 1, NPC.whoAmI, (float)Math.Pow(healingAuraOpacity, 2f));
            Main.npc[healingTarget - 1].behindTiles = false;
            if (healingAuraOpacity > 0f) {
                DrawHealingPrim();
            }
            Main.spriteBatch.End();
            Main.spriteBatch.BeginWorld(shader: false); ;
        }
        return false;
    }

    public void DrawHealingPrim() {
        var startPos = NPC.Center + new Vector2(5f * NPC.spriteDirection, 0f);
        var endPos = Main.npc[healingTarget - 1].Center;
        var difference = endPos - startPos;
        var dir = Vector2.Normalize(difference);
        var list = new List<Vector2>
            {
                startPos,
            };
        int amt = ExtendedMod.HighQualityEffects ? 20 : 7;
        if (difference.Length() < 300f) {
            amt = 0;
        }
        var pos = list[0];
        for (int i = 0; i < 1000; i++) {
            float length = (pos - endPos).Length();
            if (length <= 10f)
                break;

            pos.X = MathHelper.Lerp(pos.X, endPos.X, 0.005f);
            pos.Y = MathHelper.Lerp(pos.Y, endPos.Y, 0.01f);
            //AequusHelpers.dustDebug(pos);
            list.Add(pos);
        }
        //list.Add(new Vector2(NPC.Center.X, list[0].Y));
        list.Add(endPos);
        Vector2[] array = list.ToArray();

        float opacity = (float)Math.Pow(healingAuraOpacity, 2f);
        DrawHelper.DrawBasicVertexLine(AequusTextures.Trail, array, Helper.GenerateRotationArr(array),
            p => Color.Lerp(GetPylonColor(), CombatText.HealLife, p) with { A = 60 } * opacity,
            p => 6f,
            -Main.screenPosition);
    }

    public override void SendExtraAI(BinaryWriter writer) {
        base.SendExtraAI(writer);
        writer.Write(healingTarget);
        writer.Write(healingAuraOpacity);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        base.ReceiveExtraAI(reader);
        healingTarget = reader.ReadInt32();
        healingAuraOpacity = reader.ReadSingle();
    }
}