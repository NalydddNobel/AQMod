using AQMod.Items.Foods;
using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.CrabSeason
{
    public class ArrowCrab : ModNPC
    {
        public override void SetDefaults()
        {
            npc.width = 58;
            npc.height = 18;
            npc.lifeMax = 50;
            npc.damage = 30;
            npc.knockBackResist = 0.4f;
            npc.noGravity = true;
            npc.aiStyle = -1;
            npc.value = Item.buyPrice(silver: 8);
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath8;
            banner = npc.type;
            bannerItem = ModContent.ItemType<Items.Placeable.Banners.ArrowCrabBanner>();
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, npc.velocity.X, npc.velocity.X, 0, default(Color), 1.25f);
                }
                for (int i = 0; i < 3; i++)
                {
                    Gore.NewGore(npc.Center, npc.velocity, mod.GetGoreSlot("Gores/ArrowCrab_" + i));
                }
            }
            else
            {
                for (int i = 0; i < damage / 5; i++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, npc.velocity.X, npc.velocity.X, 0, default(Color), 0.9f);
                }
            }
        }

        public override void AI()
        {
            var center = npc.Center;
            npc.TargetClosest();
            var plrCenter = Main.player[npc.target].Center;
            var tilePosition = center.ToTileCoordinates();
            int x = tilePosition.X + (int)(npc.velocity.X / 16f);
            int y = tilePosition.Y;
            Tile tile = null;
            for (int i = 0; i < 3; i++)
            {
                if (Framing.GetTileSafely(x, y).active() && Main.tileSolid[Main.tile[x, y].type])
                {
                    tile = Main.tile[x, y];
                    break;
                }
                y++;
            }
            if ((plrCenter.X - center.X).Abs() < npc.width * 2f)
            {
                npc.direction = 0;
            }
            else if (plrCenter.X < center.X)
            {
                npc.direction = -1;
                npc.spriteDirection = 1;
            }
            else
            {
                npc.direction = 1;
                npc.spriteDirection = -1;
            }
            npc.rotation = npc.velocity.X * 0.0415f;
            if (tile != null || Collision.WetCollision(npc.position, npc.width, npc.height) && Main.player[npc.target].position.Y < npc.position.Y)
            {
                if (npc.velocity.Y > -1f)
                {
                    npc.velocity.Y -= 0.3f;
                }
                else
                {
                    npc.velocity.Y *= 0.96f;
                }
                npc.velocity.X += npc.direction * 0.1f;
                if (npc.velocity.X < -8f)
                {
                    npc.velocity.X = -8f;
                }
                else if (npc.velocity.X > 8f)
                {
                    npc.velocity.X = 8f;
                }
            }
            else
            {
                npc.velocity.Y += 0.25f;
                if (npc.velocity.Y > 7f)
                    npc.velocity.Y -= 0.25f;
                npc.velocity.X -= npc.direction * 0.02f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(8))
            {
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PickBreak>(), 480);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Biomes().zoneCrabCrevice)
                return 0.2f;
            return 0f;
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool())
                Item.NewItem(npc.getRect(), ModContent.ItemType<CrabShell>());
            if (Main.rand.NextBool())
                Item.NewItem(npc.getRect(), ModContent.ItemType<AquaticEnergy>());
            if (Main.rand.NextBool(10))
                Item.NewItem(npc.getRect(), ModContent.ItemType<CheesePuff>());
        }

        private Vector2[] _feetPositions;
        private Vector2[] _feetPositionsGoto;

        private void DrawLeg(int i, Vector2 center, int legFrameHeight, Texture2D legTexture, Rectangle footFrame, Rectangle legFrame, Color drawColor, Vector2 legOrigin)
        {
            for (int j = 0; j < 3; j++)
            {
                if (i != j && (_feetPositions[i].X - _feetPositions[j].X).Abs() < 20)
                {
                    if (_feetPositions[i].X < _feetPositions[j].X)
                    {
                        _feetPositions[i].X--;
                    }
                    else
                    {
                        _feetPositions[i].X++;
                    }
                }
            }
            if (_feetPositions[i].Y < center.Y)
                _feetPositions[i].Y = center.Y;
            if (_feetPositionsGoto[i].Y < center.Y)
                _feetPositionsGoto[i].Y = center.Y;
            _feetPositions[i] = Vector2.Lerp(_feetPositions[i], _feetPositionsGoto[i], npc.velocity.Length() / 12f);
            if ((_feetPositions[i] - center).Length() > legFrameHeight * 3.33f)
            {
                var offset = new Vector2(npc.velocity.X * npc.velocity.X * 0.157f, npc.height * 1.75f + 4f);
                if (center.X > _feetPositions[i].X)
                {
                    offset.X += npc.width * 0.6f;
                }
                else
                {
                    offset.X += -npc.width * 0.6f;
                }
                _feetPositionsGoto[i] = center + offset;
            }
            float rotation = (center.X - _feetPositions[i].X) * 0.0157f;
            Main.spriteBatch.Draw(legTexture, _feetPositions[i] - Main.screenPosition, footFrame, drawColor, rotation, legOrigin, npc.scale, SpriteEffects.None, 0f);
            var pos = _feetPositions[i] + new Vector2(0f, -legFrameHeight + 2f).RotatedBy(rotation);
            rotation = ((center - pos).ToRotation() + MathHelper.PiOver2) / 2f;
            Main.spriteBatch.Draw(legTexture, pos - Main.screenPosition, legFrame, drawColor, rotation, legOrigin, npc.scale, SpriteEffects.None, 0f);
            pos += new Vector2(0f, -legFrameHeight + 2f).RotatedBy(rotation);
            rotation = (center - pos).ToRotation() + MathHelper.PiOver2;
            Main.spriteBatch.Draw(legTexture, pos - Main.screenPosition, legFrame, drawColor, rotation, legOrigin, npc.scale, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            var center = npc.Center;
            var legTexture = this.GetTextureobj("_Legs");
            int legFrameHeight = legTexture.Height / 2;
            var legFrame = new Rectangle(0, 0, legTexture.Width, legFrameHeight - 2);
            var footFrame = new Rectangle(0, legFrameHeight, legTexture.Width, legFrameHeight - 2);
            var legOrigin = new Vector2(legFrame.Width / 2f, legFrame.Height);
            if (_feetPositions == null)
            {
                _feetPositions = new Vector2[3];
                _feetPositionsGoto = new Vector2[3];
                for (int i = 0; i < 3; i++)
                {
                    _feetPositionsGoto[i] = _feetPositions[i] = center + new Vector2(-20f + 20f * i, 20f);
                }
            }
            for (int i = 0; i < 3; i++)
            {
                DrawLeg(i, center, legFrameHeight, legTexture, footFrame, legFrame, drawColor, legOrigin);
            }
            var drawPosition = npc.Center - Main.screenPosition;
            Main.spriteBatch.Draw(Main.npcTexture[npc.type], drawPosition, npc.frame, drawColor, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            return false;
        }
    }
}