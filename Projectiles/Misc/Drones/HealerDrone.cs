using Aequus.Content.Necromancy;
using Aequus.Graphics;
using Aequus.Graphics.Primitives;
using Aequus.Items.Consumables.Drones;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.Drones
{
    public class HealerDrone : TownDroneBase
    {
        public override int ItemDrop => ModContent.ItemType<InactivePylonHealer>();

        public float healingAuraOpacity;
        public int healingTarget;

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.npcProj = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {

            base.AI();

            healingAuraOpacity = Math.Clamp(healingAuraOpacity, 0f, 1f);

            var target = FindTarget();

            float targetDistance = 3200f;
            float minDistance = 100f;
            bool noAura = true;
            if (target != null)
            {
                targetDistance = Projectile.Distance(target.Center);
                int healingDistance = Math.Max(Projectile.damage, (int)minDistance + 50);
                if (Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, target.position, target.width, target.height))
                {
                    healingDistance += 50;
                }
                if (targetDistance < healingDistance)
                {
                    Projectile.ai[0]++;
                    healingAuraOpacity += 0.015f;
                    int healingDelay = 15;
                    if (NPCID.Sets.CountsAsCritter[target.type])
                    {
                        healingDelay = 61;
                    }
                    if ((int)Projectile.ai[0] % healingDelay == 0)
                    {
                        if (healingTarget != target.whoAmI + 1)
                        {
                            healingTarget = target.whoAmI + 1;
                            healingAuraOpacity = 0f;
                            Projectile.netUpdate = true;
                        }
                        bool end = (int)Projectile.ai[0] >= 30;
                        var shootPosition = Projectile.Center + new Vector2(0f, 12f);
                        int healAmt = (int)Math.Max(target.lifeMax * 0.064f, 1);
                        if (target.life + healAmt > target.lifeMax)
                        {
                            healAmt = target.lifeMax - target.life;
                        }
                        target.life += healAmt;
                        if (Main.netMode != NetmodeID.Server)
                        {
                            int c = CombatText.NewText(new Rectangle((int)target.position.X, (int)target.position.Y, target.width, target.height), CombatText.HealLife, healAmt, dot: true);
                            Main.combatText[c].scale *= 0.5f;
                            Main.combatText[c].velocity.X += Main.rand.NextFloat(2f, 4f) * ((int)Projectile.ai[0] % 2 == 0 ? -1 : 1);
                        }
                        Projectile.netUpdate = true;
                        target.netUpdate = true;
                        SoundEngine.PlaySound(SoundID.Item4.WithPitch(0.75f).WithVolume(0.5f), Projectile.Center);
                        if (end)
                        {
                            Projectile.ai[0] = 0f;
                            Projectile.netUpdate = true;
                        }
                    }

                    if ((int)Projectile.ai[0] % 10 == 0)
                    {
                        var d = Dust.NewDustDirect(target.position + new Vector2(0f, target.height - 10), target.width, 10, ModContent.DustType<MonoDust>(), newColor: CombatText.HealLife.UseA(0));
                        d.velocity.X *= 0.4f;
                        d.velocity.X = d.velocity.X.Abs() * ((int)Projectile.ai[0] % 2 == 0 ? -1 : 1);
                        d.velocity.Y -= target.height / 10 * Main.rand.NextFloat(0.5f, 1f);
                    }
                    noAura = false;
                }
            }

            if (noAura)
            {
                healingAuraOpacity -= 0.01f + (1f - healingAuraOpacity) * 0.025f;
                minDistance = 20f;
            }

            int tileX = ((int)Projectile.position.X + Projectile.width / 2) / 16;
            int tileY = ((int)Projectile.position.Y + Projectile.height / 2) / 16;

            if (target == null)
            {
                int tileHeight = 30;
                for (int i = 0; i < 30; i++)
                {
                    if (WorldGen.InWorld(tileX, tileY + i, 10) && AequusHelpers.IsSolid(Main.tile[tileX, tileY + i]))
                    {
                        tileHeight = i + 1;
                        break;
                    }
                }

                if (tileHeight >= 30)
                {
                    gotoVelocityY = 3f;
                }
                else if (tileHeight >= 20)
                {
                    gotoVelocityY = 1f;
                }
                else if (tileHeight >= 10)
                {
                    gotoVelocityY = 0.5f;
                }
                else if (tileHeight < 2)
                {
                    gotoVelocityY = -0.5f;
                }
                else if (targetDistance < minDistance)
                {
                    if (gotoVelocityYResetTimer <= 0)
                    {
                        gotoVelocityY = Main.rand.NextFloat(-1.5f, 0.1f);
                        gotoVelocityYResetTimer = Main.rand.Next(20, 300);
                        Projectile.netUpdate = true;
                    }
                    else
                    {
                        gotoVelocityYResetTimer--;
                    }
                }
            }

            var pylonWorld = pylonSpot.ToWorldCoordinates();
            float yLerp = 0.02f;
            if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                if (pylonWorld.Y > Projectile.position.Y || Main.rand.NextBool(30))
                {
                    for (int i = 0; i > -4; i--)
                    {
                        if (WorldGen.InWorld(tileX, tileY + i, 10) && AequusHelpers.IsSolid(Main.tile[tileX, tileY + i]))
                        {
                            gotoVelocityY = Math.Abs(gotoVelocityY);
                            yLerp = 0.125f;
                            Projectile.netUpdate = true;
                            break;
                        }
                    }
                }
            }

            if (target == null || targetDistance < minDistance)
            {
                if (gotoVelocityXResetTimer <= 0)
                {
                    gotoVelocityX = Main.rand.NextFloat(-2f, 2f);
                    gotoVelocityXResetTimer = Main.rand.Next(60, 500);
                    Projectile.netUpdate = true;
                }
                else
                {
                    gotoVelocityXResetTimer--;
                }

                if (Projectile.wet)
                {
                    gotoVelocityY = -gotoVelocityY.Abs();
                }
                var diffFromPylon = pylonSpot.ToWorldCoordinates() - Projectile.Center;
                if (diffFromPylon.Length() > 1000f)
                {
                    var gotoVector = Vector2.Normalize(diffFromPylon) * (float)Math.Sqrt(gotoVelocityX * gotoVelocityX + gotoVelocityY * gotoVelocityY);
                    gotoVelocityX = gotoVector.X;
                    gotoVelocityY = gotoVector.Y;
                }
                Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, gotoVelocityX, 0.02f);
                Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, gotoVelocityY, yLerp);
                if (target != null && Projectile.velocity.Length() > 2f)
                {
                    Projectile.velocity *= 0.95f;
                }
            }
            else
            {
                var normal = Vector2.Normalize(target.Center - Projectile.Center + new Vector2(0f, -20f));
                gotoVelocityX = MathHelper.Lerp(gotoVelocityX, normal.X, 0.01f);
                gotoVelocityY = MathHelper.Lerp(gotoVelocityY, normal.Y, 0.015f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, normal * 6f, 0.01f);
            }
            if (target != null)
            {
                Projectile.ai[1] = target.whoAmI + 1;
            }
            else
            {
                Projectile.ai[1] = 0f;
            }
            if (healingAuraOpacity <= 0f)
            {
                if (target != null)
                {
                    healingTarget = target.whoAmI + 1;
                }
                else
                {
                    healingTarget = 0;
                }
            }

            Projectile.rotation = Projectile.velocity.X * 0.1f;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.CollideWithOthers(0.1f);
        }

        public NPC FindTarget()
        {
            int target = -1;
            if (Projectile.ai[1] > 0f)
            {
                int alreadyChosenTarget = (int)Projectile.ai[1] - 1;
                if (Main.npc[alreadyChosenTarget].active && (Main.npc[alreadyChosenTarget].townNPC || Main.npc[alreadyChosenTarget].friendly || NPCID.Sets.CountsAsCritter[Main.npc[alreadyChosenTarget].type]) && Main.npc[alreadyChosenTarget].life < Main.npc[alreadyChosenTarget].lifeMax)
                {
                    return Main.npc[alreadyChosenTarget];
                }
            }
            float distance = 2400f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && (NPCID.Sets.CountsAsCritter[Main.npc[i].type] || Main.npc[i].friendly || Main.npc[i].townNPC) && Main.npc[i].life < Main.npc[i].lifeMax)
                {
                    float d = Projectile.Distance(Main.npc[i].Center) + Main.npc[i].life * 2;
                    if (d < distance)
                    {
                        target = i;
                        distance = d;
                    }
                }
            }
            if (target != -1)
            {
                return Main.npc[target];
            }
            distance = 3200f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && (NPCID.Sets.CountsAsCritter[Main.npc[i].type] || Main.npc[i].friendly || Main.npc[i].townNPC) && Main.npc[i].life < Main.npc[i].lifeMax)
                {
                    float d = Projectile.Distance(Main.npc[i].Center) + Main.npc[i].life * 2;
                    if (d < distance && Collision.CanHitLine(Main.npc[i].position, Main.npc[i].width, Main.npc[i].height, Projectile.position, Projectile.width, Projectile.height))
                    {
                        target = i;
                        distance = d;
                    }
                }
            }
            if (target != -1)
            {
                return Main.npc[target];
            }
            return null;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                gotoVelocityX = -gotoVelocityX;
                Projectile.velocity.X = Projectile.oldVelocity.X * 0.8f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                gotoVelocityY = -gotoVelocityY;
                Projectile.velocity.Y = Projectile.oldVelocity.Y * 0.8f;
            }
            Projectile.netUpdate = true;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var off, out var frame, out var origin, out int _);

            var drawCoords = Projectile.position + off - Main.screenPosition;
            var color = GetPylonColor();
            if (HealerDroneRenderer.RenderingNow)
            {
                if (healingAuraOpacity > 0f)
                {
                    if (healingTarget > 0)
                        DrawHealingPrim();
                }
                Main.spriteBatch.End();
                Begin.GeneralEntities.BeginShader(Main.spriteBatch);
                var s = GameShaders.Armor.GetSecondaryShader(ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex, Main.LocalPlayer);
                s.Apply(null);
                Main.spriteBatch.Draw(texture, drawCoords, frame, lightColor,
                Projectile.rotation, origin, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                return false;
            }

            if (healingTarget > 0 && Main.npc[healingTarget - 1].active)
            {
                HealerDroneRenderer.Instance.AddHealingAura(healingTarget - 1, Projectile.whoAmI, healingAuraOpacity);
                Main.npc[healingTarget - 1].behindTiles = false;
            }

            Main.EntitySpriteDraw(texture, drawCoords, frame, lightColor,
                Projectile.rotation, origin, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, drawCoords, frame, color * SpawnInOpacity,
                Projectile.rotation, origin, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }
        public void DrawHealingPrim()
        {
            var prim = new TrailRenderer(TextureCache.Trail[0].Value, TrailRenderer.DefaultPass, (p) => new Vector2(4f), (p) => CombatText.HealLife.UseA(60) * healingAuraOpacity,
                drawOffset: Vector2.Zero);

            var startPos = Projectile.Center + new Vector2(0f, Projectile.height * 0.4f);
            var endPos = Main.npc[healingTarget - 1].Center;
            var difference = endPos - startPos;
            var dir = Vector2.Normalize(difference);
            var list = new List<Vector2>
                {
                    startPos,
                };
            int amt = Aequus.HQ ? 20 : 7;
            if (difference.Length() < 300f)
            {
                amt = 0;
            }
            var pos = list[0];
            for (int i = 0; i < 1000; i++)
            {
                float length = (pos - endPos).Length();
                if (length <= 10f)
                    break;

                pos.X = MathHelper.Lerp(pos.X, endPos.X, 0.005f);
                pos.Y = MathHelper.Lerp(pos.Y, endPos.Y, 0.01f);
                //AequusHelpers.dustDebug(pos);
                list.Add(pos);
            }
            //list.Add(new Vector2(Projectile.Center.X, list[0].Y));
            list.Add(endPos);
            prim.Draw(list.ToArray());
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(healingTarget);
            writer.Write(healingAuraOpacity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            healingTarget = reader.ReadInt32();
            healingAuraOpacity = reader.ReadSingle();
        }
    }
}