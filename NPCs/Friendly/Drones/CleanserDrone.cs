using Aequus.Content.DronePylons;
using Aequus.Items.Consumables.Drones;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Friendly.Drones
{
    public class CleanserDrone : TownDroneBase
    {
        public override int ItemDrop => ModContent.ItemType<InactivePylonCleanser>();

        public override void SetDefaults()
        {
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

        public override void AI()
        {
            base.AI();

            var diffFromPylon = pylonSpot.ToWorldCoordinates() - NPC.Center;
            if (diffFromPylon.Length() > 1000f || NPC.ai[1] >= 1000f)
            {
                NPC.ai[1] = Math.Max(NPC.ai[1] + 1f, 1000f);
                if (NPC.ai[1] > 1120f)
                    NPC.ai[1] = 0f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(diffFromPylon) * 8f, 0.01f);
            }
            else if (NPC.ai[1] >= 0f)
            {
                DefaultMovement();
                if (NPC.wet && NPC.velocity.Y < 8f)
                {
                    NPC.velocity.Y -= 0.1f;
                }
            }

            NPC.rotation = NPC.velocity.X * 0.1f;
            NPC.spriteDirection = NPC.direction;
            NPC.CollideWithOthers(0.1f);

            if (Main.netMode == NetmodeID.MultiplayerClient || NPC.ai[1] >= 1000f)
                return;

            if (NPC.ai[1] < 0f)
            {
                NPC.velocity *= 0.95f;
                if (NPC.localAI[2] % 5 == 0)
                {
                    var p = new Point(-(int)NPC.ai[1], -(int)NPC.ai[2]);
                    int pylonStyle = (int)NPC.localAI[1] - 1;
                    int tileType = Main.tile[p].TileType;
                    int solution = 0;
                    foreach (var drones in PylonManager.ActiveDrones)
                    {
                        if (drones is CleanserDroneSlot cleanserSlot)
                        {
                            solution = cleanserSlot.GetSolutionProjectileID(tileType);
                            if (solution > 0)
                                break;
                        }
                    }

                    if (solution > 0)
                    {
                        var spawnPosition = NPC.Center;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition,
                            Vector2.Normalize(p.ToWorldCoordinates() + new Vector2(8f) - spawnPosition).RotatedBy(NPC.localAI[2] / 30f * NPC.direction) * 7.5f, solution, 0, 0, Main.myPlayer);
                    }
                }
                if (NPC.localAI[2] > 40f)
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.localAI[2] = 0f;
                }
                NPC.localAI[2]++;
                return;
            }

            NPC.ai[1]++;
            if (NPC.ai[1] < 90f)
                return;

            var convertibleTile = FindConvertibleTile();
            if (convertibleTile == Point.Zero)
                return;

            NPC.ai[1] = -convertibleTile.X;
            NPC.ai[2] = -convertibleTile.Y;
            NPC.direction = Main.rand.NextBool() ? -1 : 1;
            NPC.netUpdate = true;
        }

        public Point FindConvertibleTile()
        {
            var tilePos = NPC.Center.ToTileCoordinates();
            return CleanserDroneSlot.FindConvertibleTile(tilePos);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC.GetDrawInfo(out var texture, out var off, out var frame, out var origin, out int _);

            var color = GetPylonColor();
            spriteBatch.Draw(texture, NPC.position + off - screenPos, frame, NPC.GetNPCColorTintedByBuffs(drawColor),
                NPC.rotation, origin, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Glow", AssetRequestMode.ImmediateLoad).Value, NPC.position + off - screenPos, frame, color * SpawnInOpacity,
                NPC.rotation, origin, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
    }
}