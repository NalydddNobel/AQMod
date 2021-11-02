using AQMod.Common.Utilities;
using AQMod.Items.Bait;
using AQMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common
{
    public class AQProjectile : GlobalProjectile
    {
        public static class Sets
        {
            public static bool[] UntaggableProjectile { get; private set; }
            public static bool[] HeadMinion { get; private set; }

            internal static void Setup()
            {
                UntaggableProjectile = new bool[ProjectileLoader.ProjectileCount];
                UntaggableProjectile[ModContent.ProjectileType<CelesteTorusCollider>()] = true;

                HeadMinion = new bool[ProjectileLoader.ProjectileCount];
                HeadMinion[ModContent.ProjectileType<Projectiles.Summon.Monoxider>()] = true;
            }
        }

        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        public override bool PreAI(Projectile projectile)
        {
            if (projectile.aiStyle == 61)
            {
                if (projectile.ai[1] > 0f && projectile.localAI[1] >= 0f) // on catch effects
                {
                    var aQPlayer = Main.player[projectile.owner].GetModPlayer<AQPlayer>();
                    if (aQPlayer.PopperType > 0)
                    {
                        var item = new Item();
                        item.SetDefaults(aQPlayer.PopperType);
                        ((PopperBaitItem)item.modItem).OnCatchEffect(Main.player[projectile.owner], aQPlayer, projectile, Framing.GetTileSafely(projectile.Center.ToTileCoordinates()));
                    }
                }
                else if (projectile.ai[0] <= 0f && projectile.wet && projectile.rotation != 0f) // When it enters the water
                {
                    var aQPlayer = Main.player[projectile.owner].GetModPlayer<AQPlayer>();
                    if (aQPlayer.PopperType > 0)
                    {
                        var item = new Item();
                        item.SetDefaults(aQPlayer.PopperType);
                        ((PopperBaitItem)item.modItem).PopperEffects(Main.player[projectile.owner], aQPlayer, projectile, Framing.GetTileSafely(projectile.Center.ToTileCoordinates()));
                    }
                }
            }
            return true;
        }

        public override void Kill(Projectile projectile, int timeLeft)
        {
            if (projectile.aiStyle == 61)
            {
                if (projectile.ai[0] >= 1f && projectile.ai[1] > 0f && Main.myPlayer == projectile.owner)
                {
                    var plr = Main.player[projectile.owner];
                    var aQPlr = plr.GetModPlayer<AQPlayer>();
                    var item = new Item();
                    item.SetDefaults((int)projectile.ai[1]);
                    if (aQPlr.goldSeal && item.value > Item.sellPrice(gold: 1))
                    {
                        int sonarPotionIndex = plr.FindBuffIndex(BuffID.Sonar);
                        if (sonarPotionIndex != -1)
                        {
                            plr.buffTime[sonarPotionIndex] += 6000;
                        }
                        else
                        {
                            plr.AddBuff(BuffID.Sonar, 6000);
                        }
                    }
                    if (aQPlr.silverSeal)
                    {
                        int fishingPotionIndex = plr.FindBuffIndex(BuffID.Fishing);
                        if (fishingPotionIndex != -1)
                        {
                            plr.buffTime[fishingPotionIndex] += 600;
                        }
                        else
                        {
                            plr.AddBuff(BuffID.Fishing, 600);
                        }
                    }
                    switch (projectile.ai[1])
                    {
                        case ItemID.CorruptFishingCrate:
                        case ItemID.CrimsonFishingCrate:
                        case ItemID.DungeonFishingCrate:
                        case ItemID.FloatingIslandFishingCrate:
                        case ItemID.HallowedFishingCrate:
                        case ItemID.JungleFishingCrate:
                        case ItemID.GoldenCrate:
                        case ItemID.IronCrate:
                        case ItemID.WoodenCrate:
                        if (aQPlr.copperSeal)
                        {
                            int cratePotionIndex = plr.FindBuffIndex(BuffID.Crate);
                            if (cratePotionIndex != -1)
                            {
                                plr.buffTime[cratePotionIndex] += 1800;
                            }
                            else
                            {
                                plr.AddBuff(BuffID.Crate, 1800);
                            }
                        }
                        break;

                        default:
                        if (projectile.ai[1] > Main.maxItemTypes)
                        {
                            var modItem = ItemLoader.GetItem((int)projectile.ai[1]);
                            if (modItem.CanRightClick())
                                goto case ItemID.WoodenCrate;
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Counts how many projectiles are active of a given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int CountProjectiles(int type)
        {
            int count = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == type)
                {
                    count++;
                }
            }
            return count;
        }

        public static void UpdateHeldProjDoVelocity(Player player, Vector2 rotatedRelativePoint, Projectile projectile)
        {
            float speed = 1f;
            var item = player.ItemInHand();
            if (item.shoot == projectile.type)
            {
                speed = item.shootSpeed * projectile.scale;
            }
            Vector2 newVelocity = (Main.MouseWorld - rotatedRelativePoint).SafeNormalize(Vector2.UnitX * player.direction) * speed;
            if (projectile.velocity.X != newVelocity.X || projectile.velocity.Y != newVelocity.Y)
            {
                projectile.netUpdate = true;
            }
            projectile.velocity = newVelocity;
        }

        public static void UpdateHeldProj(Player player, Vector2 rotatedRelativePoint, float offsetAmount, Projectile projectile)
        {
            float velocityAngle = projectile.velocity.ToRotation();
            projectile.rotation = velocityAngle + ((projectile.spriteDirection == -1).ToInt() * (float)Math.PI);
            projectile.direction = (Math.Cos(velocityAngle) > 0.0).ToDirectionInt();
            float offset = offsetAmount * projectile.scale;
            projectile.position = rotatedRelativePoint - (projectile.Size * 0.5f) + (velocityAngle.ToRotationVector2() * offset);
            projectile.spriteDirection = projectile.direction;
            player.ChangeDir(projectile.direction);
            projectile.timeLeft = 2;
            player.itemRotation = (projectile.velocity * projectile.direction).ToRotation();
            player.heldProj = projectile.whoAmI;
        }
    }
}