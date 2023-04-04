using Aequus.Content.Biomes.CrabCrevice;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Boss.Crabson.Rewards
{
    public class CrabsonTreasureChest : ModProjectile
    {
        public const int State_Falling = 0;
        public const int State_Opening = 1;

        private bool _droppedLoot;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            DrawOriginOffsetY = -16;
        }

        private ItemDropAttemptResult RandomRoll(IItemDropRule rule, DropAttemptInfo info)
        {
            ItemDropAttemptResult result = new() { State = ItemDropAttemptResultState.DidNotRunCode, };
            if (Main.rand.NextBool(10))
            {
                //Main.NewText(rule.GetType().FullName);
                if (rule is INestedItemDropRule nestedRule)
                {
                    return nestedRule.TryDroppingItem(info, RandomRoll);
                }
                
                for (int i = 0; i < 100; i++)
                {
                    result = rule.TryDroppingItem(info);
                    if (result.State == ItemDropAttemptResultState.Success)
                        return result;
                }
            }

            return result;
        }

        protected virtual void DropLoot()
        {
            var player = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];
            var oldPosition = player.position;
            int oldWidth = player.width;
            int oldHeight = player.height;
            Projectile.position.Y -= 10f;
            player.position = Projectile.position;
            player.width = Projectile.width;
            player.height = Projectile.height;

            var dropAttemptInfo = new DropAttemptInfo()
            {
                npc = null,
                player = player,
                IsExpertMode = Main.expertMode,
                IsMasterMode = Main.masterMode,
                rng = Main.rand,
            };

            try
            {
                var drop = Main.rand.Next(CrabCreviceBiome.ChestPrimaryLoot);
                Item.NewItem(Projectile.GetSource_Loot(), player.getRect(), drop.item, drop.RollStack(Main.rand));

                int drops = 0;
                for (int i = 0; i < 150 && drops < 2; i++)
                {
                    if (Main.rand.NextBool())
                    {
                        Main.ItemDropSolver.TryDropping(dropAttemptInfo with { item = ItemID.WoodenCrate, });
                        drops++;
                    }

                    if (Main.rand.NextBool())
                    {
                        Main.ItemDropSolver.TryDropping(dropAttemptInfo with { item = ItemID.IronCrate, });
                        drops++;
                    }

                    if (Main.rand.NextBool())
                    {
                        Main.ItemDropSolver.TryDropping(dropAttemptInfo with { item = ItemID.GoldenCrate, });
                        drops++;
                    }
                }
            }
            catch
            {
            }

            Projectile.position.Y += 10f;

            player.position = oldPosition;
            player.width = oldWidth;
            player.height = oldHeight;
        }

        private void Open()
        {
            Projectile.ai[1]++;

            if (Projectile.ai[1] > 90f)
            {
                Projectile.Kill();
                return;
            }

            Projectile.frame = (int)Math.Min(Projectile.ai[1] / 12, 2);
            Projectile.velocity.Y += 0.2f;
            Projectile.velocity.X *= 0.8f;

            if (Projectile.ai[1] > 60f)
            {
                if (!_droppedLoot)
                {
                    SoundEngine.PlaySound(SoundID.Mech, Projectile.Center);
                    if (Main.myPlayer == Projectile.owner)
                    {
                        DropLoot();
                        Projectile.netUpdate = true;
                    }
                    _droppedLoot = true;
                }
                Projectile.alpha += 10;
            }
        }

        private void SetOpenState()
        {
            Projectile.ai[0] = State_Opening;
            Projectile.netUpdate = true;
        }

        protected virtual void CheckOpenState()
        {
            if (Projectile.velocity.Length() <= 0.1f)
            {
                var plr = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];

                if (Projectile.Distance(plr.Center) < 200f)
                {
                    SoundEngine.PlaySound(SoundID.Unlock, Projectile.Center);
                    SetOpenState();
                    return;
                }
            }
        }

        private void Fall()
        {
            CheckOpenState();

            if (Projectile.velocity.Y == 0)
            {
                Projectile.velocity.X *= 0.8f;
            }
            Projectile.velocity.X *= 0.95f;
            Projectile.CollideWithOthers();
            Projectile.velocity.Y += 0.4f;
        }

        public override void AI()
        {
            if (Projectile.alpha < 80)
            {
                if (Main.GameUpdateCount % 8 == 0)
                {
                    var d = Dust.NewDustDirect(Projectile.position with { Y = Projectile.position.Y - 16f },
                        Projectile.width, Projectile.height + 16, DustID.TreasureSparkle, Scale: 0.5f);
                    d.velocity *= 0.1f;
                    d.fadeIn = d.scale + 0.75f;
                }
                var myHitbox = Projectile.Hitbox;
                for (int i = 0; i < Main.maxItems; i++)
                {
                    if (Main.item[i].active && Projectile.Colliding(myHitbox, Main.item[i].Hitbox))
                    {
                        Main.item[i].velocity.X += Math.Sign(Main.item[i].Center.X - Projectile.Center.X) * Main.rand.NextFloat(0.25f, 0.5f) + 0.1f;
                        Main.item[i].velocity.Y -= 0.2f;
                    }
                }
            }
            
            if (Projectile.timeLeft < 30)
            {
                Projectile.alpha += 10;
                return;
            }
            
            if ((int)Projectile.ai[0] == State_Opening)
            {
                Open();
                return;
            }
            Fall();
        }

        protected virtual void CollideEffect()
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 8; i++)
            {
                var d = Dust.NewDustDirect(
                    Projectile.position, Projectile.width, Projectile.height,
                    DustID.GemSapphire,
                    Projectile.velocity.X, Projectile.velocity.Y,
                    200,
                    Scale: Main.rand.NextFloat(0.8f, 1.6f
                ));

                d.velocity *= 0.8f;
                d.noGravity = true;
                d.fadeIn = d.scale + 0.1f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 2f)
            {
                Projectile.velocity.X = oldVelocity.X * -0.4f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 2f)
            {
                Projectile.velocity.X *= 0.75f;
                Projectile.velocity.Y = oldVelocity.Y * -0.4f;
                CollideEffect();
            }
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }
}