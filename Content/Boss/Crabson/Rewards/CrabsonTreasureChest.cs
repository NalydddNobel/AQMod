using Aequus.Content.Biomes.CrabCrevice;
using Microsoft.Xna.Framework;
using System;
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
        }

        protected virtual void DropLoot()
        {
            var player = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];
            var oldPosition = player.position;
            int oldWidth = player.width;
            int oldHeight = player.height;
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

                if (Main.rand.NextBool())
                {
                    drop = Main.rand.Next(CrabCreviceBiome.ChestSecondaryLoot);
                    Item.NewItem(Projectile.GetSource_Loot(), player.getRect(), drop.item, drop.RollStack(Main.rand));
                }

                if (Main.rand.NextBool())
                {
                    var dropRules = Main.ItemDropsDB.GetRulesForItemID(ItemID.WoodenCrate);
                    dropAttemptInfo.item = ItemID.WoodenCrate;
                    dropRules.Find((idr) => idr is AlwaysAtleastOneSuccessDropRule)?.TryDroppingItem(dropAttemptInfo);
                }

                if (Main.rand.NextBool())
                {
                    var dropRules = Main.ItemDropsDB.GetRulesForItemID(ItemID.IronCrate);
                    dropAttemptInfo.item = ItemID.IronCrate;
                    dropRules.Find((idr) => idr is AlwaysAtleastOneSuccessDropRule)?.TryDroppingItem(dropAttemptInfo);
                }

                if (Main.rand.NextBool())
                {
                    var dropRules = Main.ItemDropsDB.GetRulesForItemID(ItemID.GoldenCrate);
                    dropAttemptInfo.item = ItemID.GoldenCrate;
                    dropRules.Find((idr) => idr is AlwaysAtleastOneSuccessDropRule)?.TryDroppingItem(dropAttemptInfo);
                }
            }
            catch
            {
            }

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
            Projectile.CollideWithOthers();
            Projectile.velocity.Y += 0.4f;
        }

        public override void AI()
        {
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
    }
}