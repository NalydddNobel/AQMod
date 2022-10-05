using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.AIs
{
    public abstract class LegacyAIManEater : ModNPC
    {
        /// <summary>
        /// The range in units the NPC can travel. Defaults to 150
        /// </summary>
        public float range = 150f;

        /// <summary>
        /// Used to control the movement speed of the NPC. Defaults to 0.035.
        /// </summary>
        public float movementSpeed = 0.035f;

        /// <summary>
        /// The maximum speed the NPC can travel. Defaults to 3
        /// </summary>
        public float speedCap = 2f;

        /// <summary>
        /// In order to make the NPC rotate their sprite like the Fungi Bulbs.
        /// </summary>
        public bool faceAwayFromConnection = false;

        /// <summary>
        /// Whether the NPC shoots at all. Defaults to false
        /// </summary>
        public bool shoots;

        public void FindVaildSpot(int amt = 5)
        {
            Point pos = NPC.Center.ToTileCoordinates();
            if (pos.Y < 10 || pos.Y > Main.maxTilesX - 10)
            {
                return;
            }
            Tile t = Framing.GetTileSafely(pos.X, pos.Y);
            if (t.HasTile && Main.tileSolid[t.TileType] && !Main.tileSolidTop[t.TileType])
            {
                NPC.ai[0] = NPC.Center.ToTileCoordinates().X;
                NPC.ai[1] = NPC.Center.ToTileCoordinates().Y;
                return;
            }
            for (int j = pos.Y - amt; j <= pos.Y + amt; j++)
            {
                if (!WorldGen.InWorld(pos.X, j))
                {
                    continue;
                }
                var t2 = Main.tile[pos.X, j];
                if (t2.HasTile && Main.tileSolid[t2.TileType] && !Main.tileSolidTop[t2.TileType])
                {
                    NPC.ai[0] = pos.X;
                    NPC.ai[1] = j;
                    return;
                }
            }
            for (int i = pos.X - amt; i <= pos.X + amt; i++)
            {
                if (!WorldGen.InWorld(i, pos.Y))
                {
                    continue;
                }
                var t2 = Main.tile[i, pos.Y];
                if (t2.HasTile && Main.tileSolid[t2.TileType] && !Main.tileSolidTop[t2.TileType])
                {
                    NPC.ai[0] = i;
                    NPC.ai[1] = pos.Y;
                    return;
                }
            }
        }

        public void SetPosition()
        {
            NPC.ai[0] = NPC.Center.ToTileCoordinates().X;
            NPC.ai[1] = NPC.Center.ToTileCoordinates().Y;
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Rotation = -MathHelper.PiOver4,
            });
        }

        public override void AI()
        {
            if (NPC.ai[0] < 0f || NPC.ai[0] >= Main.maxTilesX || NPC.ai[1] < 0f || NPC.ai[1] >= (float)Main.maxTilesX)
            {
                return;
            }
            if (!Main.tile[(int)NPC.ai[0], (int)NPC.ai[1]].HasTile)
            {
                NPC.life = -1;
                NPC.HitEffect();
                NPC.active = false;
                return;
            }
            FixExploitManEaters.ProtectSpot((int)NPC.ai[0], (int)NPC.ai[1]);
            NPC.TargetClosest();
            float range = this.range;
            float movementSpeed = this.movementSpeed;
            NPC.ai[2] += 1f;
            if (NPC.ai[2] > 300f)
            {
                range = (int)(range * 1.3);
                if (NPC.ai[2] > 450f)
                {
                    NPC.ai[2] = 0f;
                }
            }
            Vector2 tileWorld = new Vector2(NPC.ai[0] * 16f + 8f, NPC.ai[1] * 16f + 8f);
            float num680 = Main.player[NPC.target].position.X + Main.player[NPC.target].width / 2 - NPC.width / 2 - tileWorld.X;
            float num681 = Main.player[NPC.target].position.Y + Main.player[NPC.target].height / 2 - NPC.height / 2 - tileWorld.Y;
            float num682 = (float)Math.Sqrt(num680 * num680 + num681 * num681);
            if (num682 > range)
            {
                num682 = range / num682;
                num680 *= num682;
                num681 *= num682;
            }
            if (NPC.position.X < NPC.ai[0] * 16f + 8f + num680)
            {
                NPC.velocity.X += movementSpeed;
                if (NPC.velocity.X < 0f && num680 > 0f)
                {
                    NPC.velocity.X += movementSpeed * 1.5f;
                }
            }
            else if (NPC.position.X > NPC.ai[0] * 16f + 8f + num680)
            {
                NPC.velocity.X -= movementSpeed;
                if (NPC.velocity.X > 0f && num680 < 0f)
                {
                    NPC.velocity.X -= movementSpeed * 1.5f;
                }
            }
            if (NPC.position.Y < NPC.ai[1] * 16f + 8f + num681)
            {
                NPC.velocity.Y += movementSpeed;
                if (NPC.velocity.Y < 0f && num681 > 0f)
                {
                    NPC.velocity.Y += movementSpeed * 1.5f;
                }
            }
            else if (NPC.position.Y > NPC.ai[1] * 16f + 8f + num681)
            {
                NPC.velocity.Y -= movementSpeed;
                if (NPC.velocity.Y > 0f && num681 < 0f)
                {
                    NPC.velocity.Y -= movementSpeed * 1.5f;
                }
            }
            if (faceAwayFromConnection)
            {
                NPC.rotation = (float)Math.Atan2(num681, num680) + 1.57f;
            }
            else
            {
                if (num680 > 0f)
                {
                    NPC.spriteDirection = 1;
                    NPC.rotation = (float)Math.Atan2(num681, num680);
                }
                if (num680 < 0f)
                {
                    NPC.spriteDirection = -1;
                    NPC.rotation = (float)Math.Atan2(num681, num680) + 3.14f;
                }
            }
            if (NPC.collideX)
            {
                NPC.netUpdate = true;
                NPC.velocity.X = NPC.oldVelocity.X * -0.7f;
                if (NPC.velocity.X > 0f && NPC.velocity.X < 2f)
                {
                    NPC.velocity.X = 2f;
                }
                if (NPC.velocity.X < 0f && NPC.velocity.X > -2f)
                {
                    NPC.velocity.X = -2f;
                }
            }
            if (NPC.collideY)
            {
                NPC.netUpdate = true;
                NPC.velocity.Y = NPC.oldVelocity.Y * -0.7f;
                if (NPC.velocity.Y > 0f && NPC.velocity.Y < 2f)
                {
                    NPC.velocity.Y = 2f;
                }
                if (NPC.velocity.Y < 0f && NPC.velocity.Y > -2f)
                {
                    NPC.velocity.Y = -2f;
                }
            }
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            if (shoots && !Main.player[NPC.target].dead)
            {
                if (NPC.justHit)
                {
                    NPC.localAI[0] = ResetShootTimer(NPC.justHit, (int)NPC.localAI[0]);
                }
                NPC.localAI[0] += 1f;
                if (NPC.localAI[0] >= GetShootTimer())
                {
                    if (CanShoot())
                    {
                        NPC.localAI[0] = 0f;
                        Shoot();
                    }
                    else
                    {
                        NPC.localAI[0] = ResetShootTimer(NPC.justHit, (int)NPC.localAI[0]);
                    }
                }
            }
        }

        /// <summary>
        /// Whether the NPC can shoot when it is ready and shoots is set to true. Defaults to if it is not in a tile, and has a line to shoot the player 
        /// </summary>
        /// <returns></returns>
        public virtual bool CanShoot() => !Collision.SolidCollision(NPC.position, NPC.width, NPC.height) && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height);

        /// <summary>
        /// The maximum time before the NPC shoots. Defaults to 120.
        /// </summary>
        /// <returns></returns>
        public virtual int GetShootTimer() => 120;

        /// <summary>
        /// Called when the NPC shoots.
        /// </summary>
        public virtual void Shoot()
        {
        }

        /// <summary>
        /// Defaults to reseting the timer to 0 when hit. And reseting the timer to [Timer max] - 20 when not hit
        /// </summary>
        /// <param name="justHit">Whether the NPC was hit. This is the basically exact same as <see cref="NPC.justHit"/> just passed as a parameter</param>
        /// <param name="current"></param>
        /// <returns>The new timer</returns>
        public virtual int ResetShootTimer(bool justHit, int current) => justHit ? 0 : GetShootTimer() - 20;

        /// <summary>
        /// Allows you to modify how the chain is drawn
        /// </summary>
        /// <param name="spriteBatch">The sprite batch</param>
        /// <param name="screenPos"></param>
        /// <param name="drawColor"></param>
        /// <param name="chainTexture">The texture</param>
        /// <param name="frame">The texture</param>
        /// <param name="_drawColor">The color the entire chain is drawn in</param>
        /// <returns>Whether to draw the chain</returns>
        public virtual bool PreDrawChain(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, ref Texture2D chainTexture, out Rectangle frame, ref Color _drawColor)
        {
            chainTexture = TextureAssets.Chain.Value;
            frame = chainTexture.Bounds;
            drawColor = Color.White;
            return true;
        }

        public virtual bool SafePreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => true;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Chain.Value;
            Color _drawColor = new Color(255, 255, 255, 255);
            if (!PreDrawChain(spriteBatch, screenPos, drawColor, ref texture, out var frame, ref _drawColor))
            {
                return SafePreDraw(spriteBatch, screenPos, drawColor);
            }
            Point offset = new Point(frame.Width - 2, frame.Height);

            Vector2 center = new Vector2(NPC.position.X + (float)(NPC.width / 2), NPC.position.Y + (float)(NPC.height / 2));
            Vector2 origin = new Vector2((float)frame.Width * 0.5f, (float)frame.Height * 0.5f);
            float num193 = NPC.ai[0] * 16f + 8f - center.X;
            float num204 = NPC.ai[1] * 16f + 8f - center.Y;
            float rotation2 = (float)Math.Atan2(num204, num193) - 1.57f;
            bool flag7 = true;
            var toTile = new Vector2(NPC.ai[0], NPC.ai[1]);
            while (flag7)
            {
                float length = (float)Math.Sqrt(num193 * num193 + num204 * num204);
                if (length < (float)offset.Y)
                {
                    offset.X = (int)length - (int)offset.Y + offset.X;
                    flag7 = false;
                }
                length = (float)offset.X / length;
                num193 *= length;
                num204 *= length;
                center.X += num193;
                center.Y += num204;
                num193 = toTile.X * 16f + 8f - center.X;
                num204 = toTile.Y * 16f + 8f - center.Y;
                Color color12 = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16f), _drawColor);
                spriteBatch.Draw(texture, new Vector2(center.X - screenPos.X, center.Y - screenPos.Y), frame, color12, rotation2, origin, 1f, SpriteEffects.None, 0f);
            }
            return SafePreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}
