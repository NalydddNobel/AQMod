using AQMod.Common;
using AQMod.Common.Graphics;
using AQMod.Items.Armor;
using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Tools.GrapplingHooks;
using AQMod.Items.Vanities;
using AQMod.Projectiles.Monster;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.CrabSeason
{
    public sealed class StriderCrab : ModNPC, IDecideFallThroughPlatforms
    {
        public const float LEG_1_LENGTH = 60f;
        public const float LEG_2_KNEECAP_Y_OFFSET = 90f;
        public const int tileHeight = 9;
        public static readonly Rectangle[] _frames = new Rectangle[]
        {
            new Rectangle(0, 0, 50, 34),
            new Rectangle(0, 36, 14, 44),
        };
        public static readonly Vector2[] _legPositions = new Vector2[]
        {
            new Vector2(38f, 26f),
            new Vector2(24f, 26f),
            new Vector2(40, 24f),
        };

        private Projectiles.Monster.StriderCrab[] _legs;

        public void Flip(int newDir)
        {
            float x = npc.position.X + npc.width / 2f;
            foreach (var l in _legs)
            {
                if (l.projectile.direction != newDir)
                {
                    float xOff = l.projectile.position.X + l.projectile.width / 2f - x;
                    l.projectile.position.X = x - xOff - l.projectile.width / 2f;
                    l.projectile.direction = newDir;
                }
            }
        }

        public Vector2 GetLegPosition(int i, int direction)
        {
            return npc.position + (direction == 1 ? new Vector2(_frames[0].Width - (int)_legPositions[i].X, _legPositions[i].Y) : _legPositions[i]);
        }

        private void Init()
        {
            var center = npc.Center;
            int legType = ModContent.ProjectileType<Projectiles.Monster.StriderCrab>();
            npc.TargetClosest(faceTarget: true);
            int legsCount = _legPositions.Length;
            _legs = new Projectiles.Monster.StriderCrab[legsCount];
            for (int i = 0; i < legsCount; i++)
            {
                var p = Projectile.NewProjectileDirect(GetLegPosition(i, npc.direction), Vector2.Zero, legType, 20, 0f, Main.myPlayer, i, npc.whoAmI + 1);
                _legs[i] = p.modProjectile as Projectiles.Monster.StriderCrab;
            }
        }

        public override void SetDefaults()
        {
            npc.width = 50;
            npc.height = 30;
            npc.lifeMax = 190;
            npc.damage = 30;
            npc.knockBackResist = 0.08f;
            npc.noGravity = true;
            npc.aiStyle = -1;
            npc.value = Item.buyPrice(silver: 8);
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath8;
            banner = npc.type;
            bannerItem = ModContent.ItemType<Items.Placeable.Banners.StriderCrabBanner>();
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, npc.velocity.X, npc.velocity.X, 0, default(Color), 1.25f);
                }
                var center = npc.Center;
                for (int i = 0; i < _legs.Length; i++)
                {
                    var position = GetLegPosition(i, npc.direction);
                    var kneePos = _legs[i].projectile.position + _legs[i].getKneecapPositionOffset();
                    float rotation = (kneePos - position).ToRotation();
                    rotation += MathHelper.PiOver2 * 3;
                    Main.gore[Gore.NewGore(position, npc.velocity, mod.GetGoreSlot("Gores/StriderCrab_4"))].rotation = rotation;
                    for (int j = 0; j < 8; j++)
                        Dust.NewDust(position + new Vector2(-10f, -10f), 20, 20, DustID.Blood, npc.velocity.X, npc.velocity.X, 0);
                    Main.gore[Gore.NewGore(position + new Vector2(15f, 0f).RotatedBy(rotation), npc.velocity, mod.GetGoreSlot("Gores/StriderCrab_5"))].rotation = rotation;
                    Main.gore[Gore.NewGore(kneePos, npc.velocity, mod.GetGoreSlot("Gores/StriderCrab_6"))].rotation = 0f;
                    for (int j = 0; j < 6; j++)
                        Dust.NewDust(kneePos + new Vector2(-10f, -10f), 20, 20, DustID.Blood, npc.velocity.X, npc.velocity.X, 0, default(Color), 0.8f);
                    Main.gore[Gore.NewGore(kneePos + new Vector2(0f, 15f), npc.velocity, mod.GetGoreSlot("Gores/StriderCrab_5"))].rotation = 0f;
                    Main.gore[Gore.NewGore(kneePos + new Vector2(0f, 65f), npc.velocity, mod.GetGoreSlot("Gores/StriderCrab_8"))].rotation = 0f;
                    Main.gore[Gore.NewGore(kneePos + new Vector2(0f, 80f), npc.velocity, mod.GetGoreSlot("Gores/StriderCrab_5"))].rotation = 0f;
                }

                Gore.NewGore(center + new Vector2(4f * npc.direction, 10f), npc.velocity, mod.GetGoreSlot("Gores/StriderCrab_2"));
                Gore.NewGore(center + new Vector2(-2f * npc.direction, 10f), npc.velocity, mod.GetGoreSlot("Gores/StriderCrab_3"));
                Gore.NewGore(center + new Vector2(20 * npc.direction), npc.velocity, mod.GetGoreSlot("Gores/StriderCrab_0"));
                Gore.NewGore(center, npc.velocity, mod.GetGoreSlot("Gores/StriderCrab_1"));
            }
            else
            {
                for (int i = 0; i < damage / 10; i++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, npc.velocity.X, npc.velocity.X, 0, default(Color), 0.9f);
                }
            }
        }

        public override void AI()
        {
            if (_legs == null)
            {
                Init();
            }
            if ((int)npc.ai[0] == 0f)
            {
                npc.ai[0] += 1f;
            }
            npc.noGravity = true;
            int gotoTileX = -1;
            int gotoTileY = -1;
            var center = npc.Center;
            bool validTarget = npc.HasValidTarget;
            bool idle = false;
            bool shooting = false;
            if (validTarget)
            {
                var player = Main.player[npc.target];
                var difference = player.Center - center;

                if (difference.X.Abs() < 32f ||
                    difference.Length() < 300f && Collision.CanHitLine(npc.position, npc.width, npc.height, player.position, player.width, player.height)
                    && (difference.Length() < 100f || Collision.CanHitLine(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height)))
                {
                    shooting = true;
                    idle = true;
                }
                else
                {
                    int playerX = (int)(player.position.X + player.width / 2f) / 16;
                    int playerY = (int)(player.position.Y + player.height / 2f) / 16;
                    for (int i = 0; i < 20; i++)
                    {
                        if (Framing.GetTileSafely(playerX, playerY + i).active() && Main.tileSolid[Main.tile[playerX, playerY + i].type])
                        {
                            gotoTileX = playerX;
                            gotoTileY = playerY + i;
                            break;
                        }
                    }
                }
                if (!shooting)
                    shooting = difference.Length() < 750f && Collision.CanHitLine(npc.position, npc.width, npc.height, player.position, player.width, player.height);
            }
            else
            {
                npc.TargetClosest(faceTarget: true);
            }

            npc.ai[1]++;
            if (npc.ai[1] > 600f)
                npc.ai[1] = 0f;

            if (gotoTileX != -1)
            {
                //if (gotoTileY != -1)
                //{
                //    Dust.NewDust(new Vector2(gotoTileX * 16f + 8f, gotoTileY * 16f), 2, 2, DustID.Fire);
                //}
                npc.direction = gotoTileX * 16f + 8f < center.X ? -1 : 1;
                if (!idle || npc.ai[1] > 450f)
                {
                    npc.velocity.X += 0.35f * npc.direction;
                    npc.velocity.X = MathHelper.Clamp(npc.velocity.X, -3f, 3f);
                }
            }

            npc.spriteDirection = npc.direction;

            int tileX = (int)(npc.position.X + npc.width / 2f) / 16;
            int tileY = (int)(npc.position.Y + npc.height / 2f) / 16;
            var tile = Framing.GetTileSafely(tileX, tileY);

            int hoverTileY = -1;
            if (gotoTileY == -1)
            {
                for (int i = 1; i < tileHeight; i++)
                {
                    if (Framing.GetTileSafely(tileX, tileY + i).active() && Main.tileSolid[Main.tile[tileX, tileY + i].type])
                    {
                        hoverTileY = tileY + i;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 1; i < tileHeight; i++)
                {
                    if (Framing.GetTileSafely(tileX, tileY + i).active() && Main.tileSolid[Main.tile[tileX, tileY + i].type])
                    {
                        hoverTileY = tileY + i;
                        if (hoverTileY == gotoTileY || !Main.tileSolidTop[Main.tile[tileX, hoverTileY].type])
                            break;
                    }
                }
            }
            bool abovePlayer = gotoTileY != -1 && validTarget && Main.player[npc.target].Center.Y - 150f > npc.position.Y;
            if (hoverTileY != -1)
            {
                //Dust.NewDust(new Vector2(tileX * 16f + 8f, hoverTileY * 16f), 2, 2, DustID.Fire);
                float gotoY = hoverTileY * 16f - 120f;
                if (abovePlayer)
                    gotoY = gotoTileY * 16f - 120f;
                float y = npc.position.Y + npc.height / 2f;
                if ((y - gotoY).Abs() < 0.1f)
                {
                    npc.velocity.Y *= 0.8f;
                }
                else
                {
                    if (y < gotoY)
                    {
                        npc.velocity.Y += 0.2f;
                        npc.velocity.Y = Math.Min(npc.velocity.Y, 1f);
                    }
                    else
                    {
                        npc.velocity.Y -= 0.2f;
                        npc.velocity.Y = Math.Max(npc.velocity.Y, -1f);
                    }
                }
                if (shooting)
                {
                    npc.ai[2]++;
                    if (npc.ai[2] <= 90f)
                    {
                        if (npc.ai[2] % 30 == 0f)
                        {
                            Main.PlaySound(SoundID.Item87, npc.Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int damage = 10;
                                if (Main.expertMode)
                                    damage = 15;
                                float speed = 6f;
                                if (Main.expertMode)
                                    speed = 10f;
                                int type = ModContent.ProjectileType<StriderCrabLaser>();
                                Projectile.NewProjectile(center, Vector2.Normalize(Main.player[npc.target].Center - center) * speed, type, damage, 1f, Main.myPlayer);
                            }
                        }
                    }
                    if (npc.ai[2] > 312f)
                        npc.ai[2] = 0f;
                }
                else
                {
                    if (npc.ai[2] > 0f)
                        npc.ai[2] = 0f;
                }
                if (idle)
                {
                    npc.TargetClosest(faceTarget: true);
                    npc.velocity.X *= 0.75f;
                    if (!abovePlayer)
                        npc.velocity.Y *= 0.9f;
                }
            }
            else
            {
                npc.noGravity = false;
            }
            if (npc.direction != npc.oldDirection)
            {
                npc.oldDirection = npc.direction;
                Flip(npc.direction);
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
            if (Content.WorldEvents.CrabSeason.CrabSeason.Active && spawnInfo.spawnTileY < Main.worldSurface && SpawnCondition.OceanMonster.Active)
                return SpawnCondition.OceanMonster.Chance * 0.2f;
            return 0f;
        }

        public override void NPCLoot()
        {
            if (!WorldDefeats.DownedCrabSeason)
            {
                WorldDefeats.DownedCrabSeason = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (Main.rand.NextBool())
                Item.NewItem(npc.getRect(), ModContent.ItemType<StriderCarapace>());
            if (Main.rand.NextBool())
                Item.NewItem(npc.getRect(), ModContent.ItemType<StriderPalms>());
            if (Main.rand.NextBool())
                Item.NewItem(npc.getRect(), ModContent.ItemType<StriderHook>());
            if (Main.rand.NextBool(10))
                Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Foods.CrabSeason.CheesePuff>());
            if (Main.rand.NextBool(20))
                Item.NewItem(npc.getRect(), ModContent.ItemType<FishyFins>());
            Item.NewItem(npc.getRect(), ModContent.ItemType<CrabShell>(), Main.rand.Next(3) + 2);
            Item.NewItem(npc.getRect(), ModContent.ItemType<AquaticEnergy>(), Main.rand.Next(3) + 1);
        }

        private void DrawLeg(int i, Texture2D texture, Vector2 screenPos, Vector2 orig)
        {
            var effects = npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            var drawPosition = npc.position + (npc.spriteDirection == 1 ? new Vector2(_frames[0].Width - (int)_legPositions[i].X, _legPositions[i].Y) : _legPositions[i]);
            var kneePos = _legs[i].projectile.position + _legs[i].getKneecapPositionOffset();
            float rotation = (kneePos - drawPosition).ToRotation();
            rotation += MathHelper.PiOver2 * 3;
            var legTip = drawPosition + new Vector2(_frames[1].Height - 24f, 0f).RotatedBy(rotation + MathHelper.PiOver2);
            if ((legTip - kneePos).Length() > 8f)
            {
                var chainTexture = ModContent.GetTexture(AQUtils.GetPath<StriderHookHook>("_Chain"));
                Drawing.DrawChain_UseLighting(chainTexture, kneePos, legTip, screenPos);
            }
            Main.spriteBatch.Draw(texture, drawPosition - screenPos, _frames[1], Lighting.GetColor((int)drawPosition.X / 16, (int)drawPosition.Y / 16), npc.rotation + rotation, new Vector2(orig.X, 6f), npc.scale, effects, 0f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            var texture = Main.npcTexture[npc.type];
            SpriteEffects effects = npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            var orig = _frames[1].Size() / 2f;
            DrawLeg(_legPositions.Length - 1, texture, Main.screenPosition, orig);
            Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition, _frames[0], drawColor, npc.rotation, _frames[0].Size() / 2f, npc.scale, effects, 0f);
            for (int i = 0; i < _legPositions.Length - 1; i++)
            {
                DrawLeg(i, texture, Main.screenPosition, orig);
            }
            return false;
        }

        bool IDecideFallThroughPlatforms.Decide() => true;
    }
}