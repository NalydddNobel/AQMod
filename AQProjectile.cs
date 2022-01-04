using AQMod.Common.Graphics;
using AQMod.Items.Tools.Fishing.Bait;
using AQMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod
{
    public class AQProjectile : GlobalProjectile
    {
        public static class AIStyles
        {
            public const int BulletAI = 0;
            public const int ArrowAI = 1;
            public const int ThrownAI = 2;
            public const int BoomerangAI = 3;
            public const int VilethornAI = 4;
            public const int FallingStarAI = 5;
            public const int PowderAI = 6;
            public const int GrapplingHookAI = 7;
            public const int BounceAI = 8;
            public const int MagicMissileAI = 9;
            public const int FallingBlockAI = 10;
            public const int ShadowOrbPetAI = 11;
            public const int AquaScepterAI = 12;
            public const int HarpoonAI = 13;
            public const int GlowstickAI = 14;
            public const int FlailAI = 15;
            public const int ExplosiveAI = 16;
            public const int TombstoneAI = 17;
            public const int DemonSickleAI = 18;
            public const int SpearAI = 19;
            public const int DrillAI = 20;
            public const int HarpNotesAI = 21;
            public const int IceRodAI = 22;
            public const int FlamesAI = 23;
            public const int CrystalStormAI = 24;
            public const int BoulderAI = 25;
            public const int PetAI = 26;
        }

        public static class Sets
        {
            public static bool[] UntaggableProjectile { get; private set; }
            public static bool[] HeadMinion { get; private set; }
            public static bool[] UnaffectedByWind { get; private set; }

            internal static void LoadSets()
            {
                UntaggableProjectile = new bool[ProjectileLoader.ProjectileCount];
                UntaggableProjectile[ModContent.ProjectileType<CelesteTorusCollider>()] = true;

                HeadMinion = new bool[ProjectileLoader.ProjectileCount];
                HeadMinion[ModContent.ProjectileType<Projectiles.Summon.Monoxider>()] = true;

                UnaffectedByWind = new bool[ProjectileLoader.ProjectileCount];

                for (int i = 0; i < ProjectileLoader.ProjectileCount; i++)
                {
                    try
                    {
                        var projectile = new Projectile();
                        projectile.SetDefaults(i);
                        if (projectile.aiStyle != AIStyles.BulletAI && projectile.aiStyle != AIStyles.ArrowAI && projectile.aiStyle != AIStyles.ThrownAI &&
                            projectile.aiStyle != AIStyles.GrapplingHookAI && projectile.aiStyle != AIStyles.DemonSickleAI && projectile.aiStyle != AIStyles.BoulderAI &&
                            projectile.aiStyle != AIStyles.BoomerangAI && projectile.aiStyle != AIStyles.ExplosiveAI && projectile.aiStyle != AIStyles.HarpNotesAI &&
                            projectile.aiStyle != AIStyles.BounceAI && projectile.aiStyle != AIStyles.CrystalStormAI)
                        {
                            UnaffectedByWind[i] = true;
                        }
                    }
                    catch (Exception e)
                    {
                        UnaffectedByWind[i] = true;
                        var l = AQMod.Instance.Logger;
                        string projectileName;
                        if (i > Main.maxProjectileTypes)
                        {
                            string tryName = Lang.GetNPCName(i).Value;
                            if (string.IsNullOrWhiteSpace(tryName) || tryName.StartsWith("Mods"))
                            {
                                projectileName = ProjectileLoader.GetProjectile(i).Name;
                            }
                            else
                            {
                                projectileName = tryName + "/" + ProjectileLoader.GetProjectile(i).Name;
                            }
                        }
                        else
                        {
                            projectileName = Lang.GetNPCName(i).Value;
                        }
                        l.Error("An error occured when doing algorithmic checks for sets for {" + projectileName + ", ID: " + i + "}");
                        l.Error(e.Message);
                        l.Error(e.StackTrace);
                    }
                }
            }

            internal static void UnloadSets()
            {
                UntaggableProjectile = null;
                HeadMinion = null;
                UnaffectedByWind = null;
            }
        }

        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        /// <summary>
        /// When this flag is raised, no wind events should be applied to this Projectile
        /// </summary>
        public bool windStruck;
        public bool windStruckOld;

        /// <summary>
        /// Values over 10 mean the projectile is hot, values below -10 means the projectile is cold
        /// </summary>
        public sbyte temperature;
        public byte temperatureRate;
        private byte temperatureUpdate;
        public bool canHeat;
        public bool canFreeze;

        public bool ShouldApplyWindMechanics(Projectile projectile)
        {
            return !windStruck && !Sets.UnaffectedByWind[projectile.type];
        }

        public override void SetDefaults(Projectile projectile)
        {
            temperature = 0;
            temperatureRate = 8;
            temperatureUpdate = 0;
        }

        public void ApplyWindMechanics(Projectile projectile, Vector2 wind)
        {
            projectile.velocity += wind;
            windStruck = true;
        }

        public override bool PreAI(Projectile projectile)
        {
            windStruckOld = windStruck;
            windStruck = false;

            temperatureUpdate--;
            if (temperatureUpdate == 0)
            {
                temperatureUpdate = temperatureRate;
                if (canHeat && temperature < 0)
                {
                    temperature++;
                }
                else if (canFreeze && temperature > 0)
                {
                    temperature--;
                }
            }

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
            if (projectile.aiStyle == 99 && Main.netMode != NetmodeID.Server && 
                Main.player[projectile.owner].GetModPlayer<AQPlayer>().glowString && 
                AQGraphics.Rendering.Culling.InScreenWorld(Utils.CenteredRectangle(projectile.Center, new Vector2(200f, 200f))))
            {
                var center = projectile.Center;
                var playerCenter = Main.player[projectile.owner].MountedCenter;
                playerCenter.Y += Main.player[projectile.owner].gfxOffY;
                float differenceX = center.X - playerCenter.X;
                float differenceY = center.Y - playerCenter.Y;

                float length = (float)Math.Sqrt(differenceX * differenceX + differenceY * differenceY);
                float add = Math.Min(length / 32f, 4f);
                var toYoyo = Vector2.Normalize(center - playerCenter);
                for (float l = 0f; l < length; l += add)
                {
                    int stringColor = Main.player[projectile.owner].stringColor;
                    Color lightColor = WorldGen.paintColor(stringColor);
                    if (lightColor.R < 75)
                    {
                        lightColor.R = 75;
                    }
                    if (lightColor.G < 75)
                    {
                        lightColor.G = 75;
                    }
                    if (lightColor.B < 75)
                    {
                        lightColor.B = 75;
                    }
                    switch (stringColor)
                    {
                        case 13:
                            lightColor = new Color(20, 20, 20);
                            break;
                        case 0:
                        case 14:
                            lightColor = new Color(200, 200, 200);
                            break;
                        case 28:
                            lightColor = new Color(163, 116, 91);
                            break;
                        case 27:
                            lightColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
                            break;
                    }
                    Lighting.AddLight(playerCenter + toYoyo * l, lightColor.ToVector3() * 0.6f);
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

        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (temperature != 0)
            {
                var aQNPC = target.GetGlobalNPC<AQNPC>();
                aQNPC.ChangeTemperature(target, temperature);
            }
        }

        public void ChangeTemperature(sbyte newTemperature)
        {
            if (temperature < 0)
            {
                if (newTemperature < 0)
                {
                    if (temperature > newTemperature)
                        temperature = newTemperature;
                }
                else
                {
                    temperature = 0;
                }
            }
            else if (temperature > 0)
            {
                if (newTemperature > 0)
                {
                    if (temperature < newTemperature)
                        temperature = newTemperature;
                }
                else
                {
                    temperature = 0;
                }
            }
            else
            {
                temperature = newTemperature;
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
                    count++;
            }
            return count;
        }

        public static void UpdateHeldProjDoVelocity(Player player, Vector2 rotatedRelativePoint, Projectile projectile)
        {
            float speed = 1f;
            var item = player.ItemInHand();
            if (item.shoot == projectile.type)
                speed = item.shootSpeed * projectile.scale;
            Vector2 newVelocity = (Main.MouseWorld - rotatedRelativePoint).SafeNormalize(Vector2.UnitX * player.direction) * speed;
            if (projectile.velocity.X != newVelocity.X || projectile.velocity.Y != newVelocity.Y)
                projectile.netUpdate = true;
            projectile.velocity = newVelocity;
        }

        public static void UpdateHeldProj(Player player, Vector2 rotatedRelativePoint, float offsetAmount, Projectile projectile)
        {
            float velocityAngle = projectile.velocity.ToRotation();
            projectile.direction = (Math.Cos(velocityAngle) > 0.0).ToDirectionInt();
            float offset = offsetAmount * projectile.scale;
            projectile.position = rotatedRelativePoint - projectile.Size * 0.5f + velocityAngle.ToRotationVector2() * offset;
            projectile.spriteDirection = projectile.direction;
            projectile.rotation = velocityAngle + (projectile.spriteDirection == -1).ToInt() * (float)Math.PI;
            player.ChangeDir(projectile.direction);
            player.itemRotation = (projectile.velocity * projectile.direction).ToRotation();
            player.heldProj = projectile.whoAmI;
        }

        /// <summary>
        /// Tries to find an active projectile with that identity. Returns -1 if none is found.
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static int FindIdentity(int identity)
        {
            int owner = -1;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].identity == identity)
                {
                    if (Main.projectile[i].active)
                    {
                        owner = i;
                        break;
                    }
                    owner = -1;
                    break;
                }
            }
            return owner;
        }

        /// <summary>
        /// Tries to find an active projectile with that identity. Returns -1 if none is found.
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static int FindIdentityAndType(int identity, int type)
        {
            int owner = -1;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type == type && Main.projectile[i].identity == identity)
                {
                    if (Main.projectile[i].active)
                    {
                        owner = i;
                        break;
                    }
                    owner = -1;
                    break;
                }
            }
            return owner;
        }
    }
}