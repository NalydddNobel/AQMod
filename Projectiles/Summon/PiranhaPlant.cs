using AQMod.Effects;
using AQMod.Sounds;
using AQMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Summon
{
    public class PiranhaPlant : ModProjectile
    {
        public const int MaxOutset = 20;

        // I'm so evil I use custom variables now!
        public Vector2 anchorLocation;
        public int targetRetract;

        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 4;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 40;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 1;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.manualDirectionChange = true;
        }

        public override bool? CanCutTiles() => false;
        public override bool MinionContactDamage() => projectile.ai[1] > -1;

        private void GotoLocation(Vector2 location, int target, int offsetTileSize = 10)
        {
            if (anchorLocation != default(Vector2) && target == -1 && Vector2.Distance(anchorLocation, location) < 128f)
            {
                return;
            }
            int x = (int)location.X / 16;
            int y = (int)location.Y / 16;
            List<Vector2> validPositions = new List<Vector2>();
            new Rectangle(x - offsetTileSize, y - offsetTileSize, offsetTileSize * 2, offsetTileSize * 2).KeepInWorld(11)
                .RectangleMethod((tileX, tileY) =>
                {
                    var tile = Main.tile[tileX, tileY];
                    if (tile == null)
                    {
                        Main.tile[tileX, tileY] = new Tile();
                        return true;
                    }
                    if (tile.active())
                    {
                        if (tile.SolidTop())
                        {
                            if (!Main.tile[tileX, tileY - 1].active() || !Main.tile[tileX, tileY - 1].Solid() || Main.tile[tileX, tileY - 1].IsASolidTop())
                            {
                                validPositions.Add(new Vector2(tileX * 16f + 8f, tileY * 16f - 2f));
                            }
                        }
                        else if (tile.Solid())
                        {
                            if (!Main.tile[tileX, tileY - 1].active() || !Main.tile[tileX, tileY - 1].Solid() || Main.tile[tileX, tileY - 1].IsASolidTop())
                            {
                                validPositions.Add(new Vector2(tileX * 16f + 8f, tileY * 16f - 2f));
                            }
                            if (!Main.tile[tileX, tileY + 1].active() || !Main.tile[tileX, tileY + 1].Solid() || Main.tile[tileX, tileY + 1].IsASolidTop())
                            {
                                validPositions.Add(new Vector2(tileX * 16f + 8f, tileY * 16f + 18f));
                            }
                            if (!Main.tile[tileX + 1, tileY].active() || !Main.tile[tileX + 1, tileY].Solid() || Main.tile[tileX + 1, tileY].IsASolidTop())
                            {
                                validPositions.Add(new Vector2(tileX * 16f + 18f, tileY * 16f + 8f));
                            }
                            if (!Main.tile[tileX - 1, tileY].active() || !Main.tile[tileX - 1, tileY].Solid() || Main.tile[tileX - 1, tileY].IsASolidTop())
                            {
                                validPositions.Add(new Vector2(tileX * 16f - 2f, tileY * 16f + 8f));
                            }
                        }
                    }
                    return true;
                });
            if (validPositions.Count == 0)
            {
                if (target == -1)
                {
                    projectile.ai[1] = -2f;
                    anchorLocation = Main.player[projectile.owner].Center;
                }
                else
                {
                    var oldTargetPosition = Main.npc[target].position;
                    Main.npc[target].Center = Main.player[projectile.owner].Center;
                    GotoLocation(Main.npc[target].Center, target, offsetTileSize); // Go to the player instead!
                    Main.npc[target].position = oldTargetPosition;
                    if ((int)projectile.ai[0] > -1)
                    {
                        DetermineVelocity(target);
                    }
                }
                return;
            }
            else
            {
                int choice = 0;
                if (target != -1)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        choice = Main.rand.Next(validPositions.Count);
                        if (Collision.CanHitLine(validPositions[choice], 2, 2, Main.npc[target].position, Main.npc[target].width, Main.npc[target].height))
                        {
                            break;
                        }
                    }
                }
                else
                {
                    choice = Main.rand.Next(validPositions.Count);
                }
                anchorLocation = validPositions[choice];
            }
            if ((int)projectile.ai[1] == -2)
            {
                projectile.ai[1] = -3f;
            }
            else if ((int)projectile.ai[0] <= 0)
            {
                projectile.Center = anchorLocation;
                DetermineVelocity(target);
                projectile.ai[0] = 1f;
                projectile.ai[1] = 1f;
            }
            else if (projectile.ai[1] >= 0f)
            {
                projectile.ai[1] = -1f;
            }
        }

        private void DetermineVelocity(int target)
        {
            if (target != -1 && Vector2.Distance(Main.npc[target].Center, projectile.Center) > 100f)
            {
                projectile.velocity = Vector2.Normalize(Main.npc[target].Center - projectile.Center);
                if (!Collision.CanHitLine(projectile.Center, 2, 2, projectile.Center + projectile.velocity * 12f, 2, 2))
                {
                    DetermineVelocity(-1);
                }
            }
            else
            {
                var c = projectile.Center;
                for (int i = 0; i < 1000; i++)
                {
                    float r = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
                    var v = r.ToRotationVector2();
                    if (Collision.CanHitLine(c, 2, 2, c + v * 12f, 2, 2))
                    {
                        projectile.velocity = v;
                        break;
                    }
                }
            }
        }

        private Vector2 MinionPosition()
        {
            return new Vector2(Main.player[projectile.owner].position.X + Main.player[projectile.owner].width / 2f + (projectile.width * (projectile.minionPos + 1) + 8) * -Main.player[projectile.owner].direction,
                    Main.player[projectile.owner].position.Y + Main.player[projectile.owner].height);
        }

        private void GoToASpot(int target, int offsetTileSize = 10)
        {
            if (Main.myPlayer != projectile.owner)
            {
                return;
            }
            if (target == -1)
            {
                GotoLocation(MinionPosition(), target, offsetTileSize);
            }
            else
            {
                GotoLocation(Main.npc[target].Center, target, offsetTileSize);
            }
            projectile.netUpdate = true;
        }

        private void ResetTargetRetractTimer()
        {
            targetRetract = 60;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            var center = projectile.Center;
            if (player.dead)
                aQPlayer.piranhaPlant = false;
            if (aQPlayer.piranhaPlant)
                projectile.timeLeft = 2;
            int target = -1;
            float distance = 1500f;

            if ((int)projectile.ai[1] == -2 || (int)projectile.ai[1] == -3)
            {
                if (Vector2.Distance(projectile.Center, Main.player[projectile.owner].Center) > 2000f)
                {
                    projectile.Center = Main.player[projectile.owner].Center;
                    projectile.velocity *= 0.1f;
                }
                if ((int)projectile.ai[1] == -2)
                {
                    GoToASpot(-1, 5);
                    if (Vector2.Distance(projectile.Center, Main.player[projectile.owner].Center) > 100f)
                    {
                        projectile.velocity = Vector2.Lerp(projectile.velocity, (Main.player[projectile.owner].Center - projectile.Center) / 16f, 0.025f);
                    }
                }
                else
                {
                    if (Vector2.Distance(projectile.Center, anchorLocation) > 40f)
                    {
                        projectile.velocity = Vector2.Lerp(projectile.velocity, (anchorLocation - projectile.Center) / 6f, 0.1f);
                    }
                    else
                    {
                        projectile.ai[1] = 1f;
                        projectile.Center = anchorLocation;
                        DetermineVelocity(-1);
                        return;
                    }
                }
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (i != projectile.whoAmI && Main.projectile[i].active && Main.projectile[i].owner == projectile.owner && Main.projectile[i].type == projectile.type)
                    {
                        if (Main.projectile[i].getRect().Intersects(projectile.getRect()))
                        {
                            projectile.velocity += Vector2.Normalize(projectile.Center - Main.projectile[i].Center) * 0.2f;
                        }
                    }
                }

                if (projectile.frame < 2)
                {
                    projectile.frame = 2;
                }

                projectile.frameCounter++;
                if (projectile.frameCounter > 4)
                {
                    projectile.frameCounter = 0;
                    projectile.frame++;
                    if (projectile.frame > 3)
                    {
                        projectile.frame = 2;
                    }
                }
                return;
            }

            if (player.HasMinionAttackTargetNPC)
            {
                int t = player.MinionAttackTargetNPC;
                float d = (Main.npc[t].Center - center).Length();
                if (d < distance)
                {
                    target = t;
                    distance = d;
                }
            }

            if (target == -1)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(attacker: projectile, ignoreDontTakeDamage: false))
                    {
                        var difference = npc.Center - center;
                        float c = (float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y);
                        if (!Collision.CanHitLine(npc.position, npc.width, npc.height, Main.player[projectile.owner].position, Main.player[projectile.owner].width, Main.player[projectile.owner].height))
                            c *= 2;
                        if (c < distance)
                        {
                            target = i;
                            distance = c;
                        }
                    }
                }
            }

            if (anchorLocation == default(Vector2))
            {
                GoToASpot(target);
            }

            projectile.Center = anchorLocation + projectile.velocity * (projectile.ai[0] - 8f);
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if ((int)projectile.ai[1] == -1)
            {
                ResetTargetRetractTimer();
                if (projectile.ai[0] <= 0f)
                {
                    int size = 16;
                    if (target == -1)
                    {
                        size = 4;
                    }
                    else if (Main.npc[target].noGravity)
                    {
                        size += 8;
                    }
                    GoToASpot(target, size);
                }
                else
                {
                    projectile.ai[0] -= 1f + (MaxOutset - projectile.ai[0]) * 0.1f;
                }
            }
            else
            {
                if (projectile.ai[0] < MaxOutset)
                {
                    projectile.ai[0] += 1f + (MaxOutset - projectile.ai[0]) * 0.1f;
                    if (projectile.ai[0] > MaxOutset)
                    {
                        projectile.ai[0] = MaxOutset;
                    }
                }
                if (target == -1)
                {
                    ResetTargetRetractTimer();
                    if (Vector2.Distance(projectile.Center, MinionPosition()) > 320f)
                    {
                        projectile.ai[1] = -1f;
                    }
                }
                else
                {
                    if (Vector2.Distance(projectile.Center, Main.player[projectile.owner].Center) > 2250f)
                    {
                        projectile.ai[1] = -1f;
                        ResetTargetRetractTimer();
                    }
                    else
                    {
                        targetRetract--;
                        if (!Collision.CanHitLine(projectile.position, projectile.width, projectile.height, Main.npc[target].position, Main.npc[target].width, Main.npc[target].height))
                        {
                            if (targetRetract > 50)
                            {
                                targetRetract -= 3;
                                if (targetRetract <= 51)
                                {
                                    targetRetract = 49;
                                }
                            }
                            else
                            {
                                targetRetract -= 3;
                            }
                        }
                        if (targetRetract == 50)
                        {
                            if (Main.netMode != NetmodeID.Server)
                                SoundID.Item8.Play(projectile.Center);
                            if (Main.myPlayer == projectile.owner)
                                Projectile.NewProjectile(projectile.Center, Vector2.Normalize(Main.npc[target].Center - projectile.Center) * 10f,
                                    ModContent.ProjectileType<PiranhaPlantFireball>(), projectile.damage, projectile.knockBack, projectile.owner);
                        }
                        else if (targetRetract <= 0)
                        {
                            targetRetract = 120;
                            projectile.ai[1] = -1f;
                        }
                    }
                }
            }

            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 1)
                {
                    projectile.frame = 0;
                }
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(anchorLocation.X);
            writer.Write(anchorLocation.Y);
            writer.Write(targetRetract);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            anchorLocation.X = reader.ReadSingle();
            anchorLocation.Y = reader.ReadSingle();
            targetRetract = reader.ReadInt32();
        }

        //public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        //{
        //    Main.instance.DrawCacheProjsBehindNPCsAndTiles.Add(index);
        //}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (GameWorldRenders.ProjsBehindTiles.drawingNow)
            {
                Texture2D texture = Main.projectileTexture[projectile.type];
                int frameHeight = texture.Height / Main.projFrames[projectile.type];
                Rectangle frame = new Rectangle(0, projectile.frame * frameHeight, texture.Width, frameHeight);
                Vector2 offset = new Vector2(projectile.width / 2, projectile.height / 2);
                var effects = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                var drawPos = projectile.position + offset - Main.screenPosition;
                drawPos = new Vector2((int)drawPos.X, (int)drawPos.Y);
                Main.spriteBatch.Draw(texture, drawPos, frame, lightColor, projectile.rotation, frame.Size() / 2f, 1f, effects, 0f);
            }
            else
            {
                GameWorldRenders.ProjsBehindTiles.Add(projectile.whoAmI);
            }
            return false;
        }
    }
}