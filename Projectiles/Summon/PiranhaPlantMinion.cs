using Aequus.Buffs.Minion;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon
{
    public class PiranhaPlantMinion : ModProjectile
    {
        public const int MaxOutset = 20;

        public Vector2 anchorLocation;
        public int targetRetract;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.manualDirectionChange = true;
            Projectile.hide = true;
        }

        public override bool? CanCutTiles() => false;
        public override bool MinionContactDamage() => Projectile.ai[1] > -1;

        private void GotoLocation(Vector2 location, int target, int offsetTileSize = 10)
        {
            if (anchorLocation != default(Vector2) && target == -1 && Vector2.Distance(anchorLocation, location) < 128f)
            {
                return;
            }
            int x = (int)location.X / 16;
            int y = (int)location.Y / 16;
            var validPositions = new List<Vector2>();
            var rect = new Rectangle(x - offsetTileSize, y - offsetTileSize, offsetTileSize * 2, offsetTileSize * 2).Fluffize(11);
            for (int tileX = rect.X; tileX < rect.X + rect.Width; tileX++)
            {
                for (int tileY = rect.Y; tileY < rect.Y + rect.Height; tileY++)
                {
                    var tile = Main.tile[tileX, tileY];
                    if (tile.HasTile)
                    {
                        if (tile.SolidTopType())
                        {
                            if (!Main.tile[tileX, tileY - 1].HasTile || !Main.tile[tileX, tileY - 1].IsSolid() || Main.tile[tileX, tileY - 1].SolidTopType())
                            {
                                validPositions.Add(new Vector2(tileX * 16f + 8f, tileY * 16f - 2f));
                            }
                        }
                        else if (tile.IsSolid())
                        {
                            if (!Main.tile[tileX, tileY - 1].HasTile || !Main.tile[tileX, tileY - 1].IsSolid() || Main.tile[tileX, tileY - 1].SolidTopType())
                            {
                                validPositions.Add(new Vector2(tileX * 16f + 8f, tileY * 16f - 2f));
                            }
                            if (!Main.tile[tileX, tileY + 1].HasTile || !Main.tile[tileX, tileY + 1].IsSolid() || Main.tile[tileX, tileY + 1].SolidTopType())
                            {
                                validPositions.Add(new Vector2(tileX * 16f + 8f, tileY * 16f + 18f));
                            }
                            if (!Main.tile[tileX + 1, tileY].HasTile || !Main.tile[tileX + 1, tileY].IsSolid() || Main.tile[tileX + 1, tileY].SolidTopType())
                            {
                                validPositions.Add(new Vector2(tileX * 16f + 18f, tileY * 16f + 8f));
                            }
                            if (!Main.tile[tileX - 1, tileY].HasTile || !Main.tile[tileX - 1, tileY].IsSolid() || Main.tile[tileX - 1, tileY].SolidTopType())
                            {
                                validPositions.Add(new Vector2(tileX * 16f - 2f, tileY * 16f + 8f));
                            }
                        }
                    }
                }
            }
            if (validPositions.Count == 0)
            {
                if (target == -1)
                {
                    Projectile.ai[1] = -2f;
                    anchorLocation = Main.player[Projectile.owner].Center;
                }
                else
                {
                    var oldTargetPosition = Main.npc[target].position;
                    Main.npc[target].Center = Main.player[Projectile.owner].Center;
                    GotoLocation(Main.npc[target].Center, target, offsetTileSize); // Go to the player instead!
                    Main.npc[target].position = oldTargetPosition;
                    if ((int)Projectile.ai[0] > -1)
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
            if ((int)Projectile.ai[1] == -2)
            {
                Projectile.ai[1] = -3f;
            }
            else if ((int)Projectile.ai[0] <= 0)
            {
                Projectile.Center = anchorLocation;
                DetermineVelocity(target);
                Projectile.ai[0] = 1f;
                Projectile.ai[1] = 1f;
            }
            else if (Projectile.ai[1] >= 0f)
            {
                Projectile.ai[1] = -1f;
            }
        }

        private void DetermineVelocity(int target)
        {
            if (target != -1 && Vector2.Distance(Main.npc[target].Center, Projectile.Center) > 100f)
            {
                Projectile.velocity = Vector2.Normalize(Main.npc[target].Center - Projectile.Center);
                if (!Collision.CanHitLine(Projectile.Center, 2, 2, Projectile.Center + Projectile.velocity * 12f, 2, 2))
                {
                    DetermineVelocity(-1);
                }
            }
            else
            {
                var c = Projectile.Center;
                for (int i = 0; i < 1000; i++)
                {
                    float r = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
                    var v = r.ToRotationVector2();
                    if (Collision.CanHitLine(c, 2, 2, c + v * 12f, 2, 2))
                    {
                        Projectile.velocity = v;
                        break;
                    }
                }
            }
        }

        private Vector2 MinionPosition()
        {
            return new Vector2(Main.player[Projectile.owner].position.X + Main.player[Projectile.owner].width / 2f + (Projectile.width * (Projectile.minionPos + 1) + 8) * -Main.player[Projectile.owner].direction,
                    Main.player[Projectile.owner].position.Y + Main.player[Projectile.owner].height);
        }

        private void GoToASpot(int target, int offsetTileSize = 10)
        {
            if (Main.myPlayer != Projectile.owner)
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
            Projectile.netUpdate = true;
        }

        private void ResetTargetRetractTimer()
        {
            targetRetract = 60;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            var aQPlayer = player.Aequus();
            var center = Projectile.Center;
            if (!AequusHelpers.UpdateProjActive<PiranhaPlantBuff>(Projectile))
            {
                return;
            }
            int target = -1;
            float distance = 1500f;

            if ((int)Projectile.ai[1] == -2 || (int)Projectile.ai[1] == -3)
            {
                if (Vector2.Distance(Projectile.Center, Main.player[Projectile.owner].Center) > 2000f)
                {
                    Projectile.Center = Main.player[Projectile.owner].Center;
                    Projectile.velocity *= 0.1f;
                }
                if ((int)Projectile.ai[1] == -2)
                {
                    GoToASpot(-1, 5);
                    if (Vector2.Distance(Projectile.Center, Main.player[Projectile.owner].Center) > 100f)
                    {
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, (Main.player[Projectile.owner].Center - Projectile.Center) / 16f, 0.025f);
                    }
                }
                else
                {
                    if (Vector2.Distance(Projectile.Center, anchorLocation) > 40f)
                    {
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, (anchorLocation - Projectile.Center) / 6f, 0.1f);
                    }
                    else
                    {
                        Projectile.ai[1] = 1f;
                        Projectile.Center = anchorLocation;
                        DetermineVelocity(-1);
                        return;
                    }
                }
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (i != Projectile.whoAmI && Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].type == Projectile.type)
                    {
                        if (Main.projectile[i].getRect().Intersects(Projectile.getRect()))
                        {
                            Projectile.velocity += Vector2.Normalize(Projectile.Center - Main.projectile[i].Center) * 0.2f;
                        }
                    }
                }

                if (Projectile.frame < 2)
                {
                    Projectile.frame = 2;
                }

                Projectile.frameCounter++;
                if (Projectile.frameCounter > 4)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame > 3)
                    {
                        Projectile.frame = 2;
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
                    if (npc.CanBeChasedBy(attacker: Projectile, ignoreDontTakeDamage: false))
                    {
                        var difference = npc.Center - center;
                        float c = (float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y);
                        if (!Collision.CanHitLine(npc.position, npc.width, npc.height, Main.player[Projectile.owner].position, Main.player[Projectile.owner].width, Main.player[Projectile.owner].height))
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

            Projectile.Center = anchorLocation + Projectile.velocity * (Projectile.ai[0] - 8f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if ((int)Projectile.ai[1] == -1)
            {
                ResetTargetRetractTimer();
                if (Projectile.ai[0] <= 0f)
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
                    Projectile.ai[0] -= 1f + (MaxOutset - Projectile.ai[0]) * 0.1f;
                }
            }
            else
            {
                if (Projectile.ai[0] < MaxOutset)
                {
                    Projectile.ai[0] += 1f + (MaxOutset - Projectile.ai[0]) * 0.1f;
                    if (Projectile.ai[0] > MaxOutset)
                    {
                        Projectile.ai[0] = MaxOutset;
                    }
                }
                if (target == -1)
                {
                    ResetTargetRetractTimer();
                    if (Vector2.Distance(Projectile.Center, MinionPosition()) > 320f)
                    {
                        Projectile.ai[1] = -1f;
                    }
                }
                else
                {
                    if (Vector2.Distance(Projectile.Center, Main.player[Projectile.owner].Center) > 2250f)
                    {
                        Projectile.ai[1] = -1f;
                        ResetTargetRetractTimer();
                    }
                    else
                    {
                        targetRetract--;
                        if (!Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, Main.npc[target].position, Main.npc[target].width, Main.npc[target].height))
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
                                SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
                            if (Main.myPlayer == Projectile.owner)
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Normalize(Main.npc[target].Center - Projectile.Center) * 10f,
                                    ModContent.ProjectileType<PiranhaPlantFireball>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                        else if (targetRetract <= 0)
                        {
                            targetRetract = 120;
                            Projectile.ai[1] = -1f;
                        }
                    }
                }
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 1)
                {
                    Projectile.frame = 0;
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

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            Main.instance.DrawCacheProjsBehindNPCsAndTiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            var frame = new Rectangle(0, Projectile.frame * frameHeight, texture.Width, frameHeight);
            var offset = new Vector2(Projectile.width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var drawPos = Projectile.position + offset - Main.screenPosition;
            drawPos = new Vector2((int)drawPos.X, (int)drawPos.Y);
            Main.spriteBatch.Draw(texture, drawPos, frame, lightColor, Projectile.rotation, frame.Size() / 2f, 1f, effects, 0f);
            return false;
        }
    }
}