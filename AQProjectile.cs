using AQMod.Common.Graphics;
using AQMod.Content.Players;
using AQMod.Items.Tools.Fishing.Bait;
using AQMod.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod
{
    public class AQProjectile : GlobalProjectile
    {
        public static class Sets
        {
            public static bool[] MinionHeadType { get; private set; }
            public static bool[] MinionChomperType { get; private set; }
            public static bool[] MinionRotationalType { get; private set; }
            public static bool[] UnaffectedByWind { get; private set; }
            public static bool[] IsGravestone { get; private set; }
            public static bool[] DamageReductionExtractor { get; private set; }
            public static List<int> HookBarbBlacklist { get; private set; }

            internal static void LoadSets()
            {
                HookBarbBlacklist = new List<int>() 
                {
                    ProjectileID.AntiGravityHook,
                    ProjectileID.StaticHook,
                };

                DamageReductionExtractor = new bool[ProjectileLoader.ProjectileCount];
                DamageReductionExtractor[ProjectileID.SiltBall] = true;
                DamageReductionExtractor[ProjectileID.SlushBall] = true;
                DamageReductionExtractor[ProjectileID.AshBallFalling] = true;
                DamageReductionExtractor[ProjectileID.SandBallFalling] = true;
                DamageReductionExtractor[ProjectileID.PearlSandBallFalling] = true;
                DamageReductionExtractor[ProjectileID.EbonsandBallFalling] = true;
                DamageReductionExtractor[ProjectileID.CrimsandBallFalling] = true;

                MinionHeadType = new bool[ProjectileLoader.ProjectileCount];
                MinionHeadType[ModContent.ProjectileType<Projectiles.Summon.MonoxiderMinion>()] = true;

                MinionChomperType = new bool[ProjectileLoader.ProjectileCount];
                MinionChomperType[ModContent.ProjectileType<Projectiles.Summon.Chomper>()] = true;

                MinionRotationalType = new bool[ProjectileLoader.ProjectileCount];
                MinionRotationalType[ModContent.ProjectileType<Projectiles.Summon.TrapperMinion>()] = true;

                IsGravestone = new bool[ProjectileLoader.ProjectileCount];
                IsGravestone[ProjectileID.Tombstone] = true;
                IsGravestone[ProjectileID.GraveMarker] = true;
                IsGravestone[ProjectileID.CrossGraveMarker] = true;
                IsGravestone[ProjectileID.Headstone] = true;
                IsGravestone[ProjectileID.Gravestone] = true;
                IsGravestone[ProjectileID.Obelisk] = true;
                IsGravestone[ProjectileID.RichGravestone1] = true;
                IsGravestone[ProjectileID.RichGravestone2] = true;
                IsGravestone[ProjectileID.RichGravestone3] = true;
                IsGravestone[ProjectileID.RichGravestone4] = true;
                IsGravestone[ProjectileID.RichGravestone5] = true;

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
                        var l = AQMod.GetInstance().Logger;
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
                MinionHeadType = null;
                MinionRotationalType = null;
                UnaffectedByWind = null;
            }
        }

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
        public bool IsBarb { get; private set; }

        public bool BarbCheck(Projectile projectile)
        {
            return IsBarb = projectile.aiStyle == 7 && projectile.friendly && !projectile.hostile && !Sets.HookBarbBlacklist.Contains(projectile.type);
        }

        public bool ShouldApplyWindMechanics(Projectile projectile)
        {
            return !windStruck && !Sets.UnaffectedByWind[projectile.type];
        }

        public override void SetDefaults(Projectile projectile)
        {
            if (temperature == 0)
            {
                temperature = 0;
                temperatureRate = 8;
            }
            temperatureUpdate = 0;
        }

        public void SetupTemperatureStats(sbyte temperature, byte temperatureRate = 8)
        {
            if (temperature < 0)
            {
                canHeat = false;
            }
            else
            {
                canFreeze = false;
            }
            this.temperature = temperature;
            this.temperatureRate = temperatureRate;
        }

        public void ApplyWindMechanics(Projectile projectile, Vector2 wind)
        {
            projectile.velocity += wind;
            windStruck = true;
        }

        private void FishingPopperCheck(Projectile projectile)
        {
            NPCLoader.blockLoot.Add(ItemID.Cascade);
            var fishing = Main.player[projectile.owner].GetModPlayer<PlayerFishing>();
            if (projectile.ai[1] > 0f && projectile.localAI[1] >= 0f && fishing.popperType > 0)
            {
                ((PopperBaitItem)ItemLoader.GetItem(fishing.popperType)).OnCatchEffect(Main.player[projectile.owner], fishing, projectile, Framing.GetTileSafely(projectile.Center.ToTileCoordinates()));
            }
            else if (projectile.ai[0] <= 0f && projectile.wet && projectile.rotation != 0f && fishing.popperType > 0)
            {
                ((PopperBaitItem)ItemLoader.GetItem(fishing.popperType)).OnEnterWater(Main.player[projectile.owner], fishing, projectile, Framing.GetTileSafely(projectile.Center.ToTileCoordinates()));
            }
        }
        private void YoyoStringGlow(Projectile projectile)
        {
            AQGraphics.SetCullPadding(padding: 200);
            if (AQGraphics.Cull_WorldPosition(projectile.getRect()))
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
        }
        private void BarbDamageCheck(Projectile projectile, AQPlayer aQPlayer)
        {
            var rect = projectile.getRect();
            var player = Main.player[projectile.owner];
            bool appliedMeathook = false;
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                if (Main.npc[k].active && !Main.npc[k].townNPC && rect.Intersects(Main.npc[k].getRect()) && AQUtils.CanNPCBeHitByProjectile(Main.npc[k], projectile))
                {
                    if (aQPlayer.meathook && !appliedMeathook && !AQNPC.Sets.CannotBeMeathooked.Contains(Main.npc[k].type))
                    {
                        aQPlayer.hasMeathookNPC = true;
                        projectile.Center = Main.npc[k].Center;
                        if (aQPlayer.meathookNPC == -1)
                        {
                            aQPlayer.meathookNPC = k;
                            projectile.damage = 0;
                        }
                    }
                    if (!aQPlayer.hasMeathookNPCOld && Main.npc[k].immune[projectile.owner] <= 0)
                    {
                        if (aQPlayer.hookDamage > 0)
                        {
                            Main.npc[k].immune[projectile.owner] = 12;
                            int damage = Main.DamageVar(aQPlayer.hookDamage);
                            float knockback = 0f;
                            bool crit = false;
                            int direction = projectile.velocity.X < 0f ? -1 : 1;
                            player.ApplyDamageToNPC(Main.npc[k], damage, knockback, direction, crit);
                        }
                        if (aQPlayer.hookDebuffs.Count > 0)
                        {
                            for (int l = 0; l < aQPlayer.hookDebuffs.Count; l++)
                            {
                                aQPlayer.hookDebuffs[l].ApplyDebuff(Main.npc[k]);
                            }
                        }
                    }
                }
            }
        }
        private void UpdateMeathookHook(Projectile projectile, AQPlayer aQPlayer)
        {
            for (int i = projectile.whoAmI + 1; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].aiStyle == AIStyles.GrapplingHookAI)
                {
                    projectile.Kill();
                }
            }
            if (Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.Kill();
                return;
            }
            float distance = projectile.Distance(Main.player[projectile.owner].Center);
            if (distance < Main.npc[aQPlayer.meathookNPC].Size.Length() * 2f)
            {
                Main.player[projectile.owner].immune = true;
                Main.player[projectile.owner].immuneTime = 12;
            }
            if (distance < Main.npc[aQPlayer.meathookNPC].Size.Length() * 1.5f)
            {
                projectile.Kill();
                return;
            }
            projectile.Center = Main.npc[aQPlayer.meathookNPC].Center;
            if (Main.player[projectile.owner].grapCount < 10)
            {
                Main.player[projectile.owner].grappling[Main.player[projectile.owner].grapCount] = projectile.whoAmI;
                Player player2 = Main.player[projectile.owner];
                Player player15 = player2;
                player15.grapCount++;
            }
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

            if (projectile.aiStyle == AIStyles.GrapplingHookAI)
            {
                if (BarbCheck(projectile))
                {
                    var aQPlayer = Main.player[projectile.owner].GetModPlayer<AQPlayer>();
                    if (aQPlayer.hasMeathookNPCOld && aQPlayer.meathookNPC != -1 && Main.npc[aQPlayer.meathookNPC].active)
                    {
                        aQPlayer.hasMeathookNPC = true;
                        projectile.velocity = Vector2.Zero;
                        projectile.ai[0] = 1f;
                        UpdateMeathookHook(projectile, aQPlayer);
                    }
                    else if ((aQPlayer.hookDamage > 0 || aQPlayer.hookDebuffs.Count > 0 || aQPlayer.meathook) && (int)projectile.ai[0] != 2)
                    {
                        BarbDamageCheck(projectile, aQPlayer);
                    }
                }
            }
            if (projectile.aiStyle == 61)
            {
                FishingPopperCheck(projectile);
            }
            if (projectile.aiStyle == 99 && Main.netMode != NetmodeID.Server &&
                Main.player[projectile.owner].GetModPlayer<AQPlayer>().glowString)
            {
                YoyoStringGlow(projectile);
            }
            return true;
        }

        private void OnCaptureFish(Projectile projectile)
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
            switch ((int)projectile.ai[1])
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
                    if ((int)projectile.ai[1] > Main.maxItemTypes)
                    {
                        if (ItemLoader.GetItem((int)projectile.ai[1]).CanRightClick())
                            goto case ItemID.WoodenCrate;
                    }
                    break;
            }
        }
        public override void Kill(Projectile projectile, int timeLeft)
        {
            if (projectile.aiStyle == 61 && projectile.ai[0] >= 1f && projectile.ai[1] > 0f && Main.myPlayer == projectile.owner)
            {
                OnCaptureFish(projectile);
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (temperature != 0)
            {
                var npcTemperature = target.GetGlobalNPC<NPCTemperatureManager>();
                npcTemperature.ChangeTemperature(target, temperature);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inGroup">Arg 1 is the projectile from the loop, Arg 2 is the projectile parameter in this method</param>
        /// <param name="projectile"></param>
        /// <param name="count"></param>
        /// <param name="whoAmI"></param>
        internal static void GetProjectileGroupStats(Func<Projectile, bool> inGroup, out int count, out int whoAmI, int projectile = -1)
        {
            count = 0;
            whoAmI = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && inGroup(Main.projectile[i]))
                {
                    if (i == projectile)
                    {
                        whoAmI = count;
                    }
                    count++;
                }
            }
        }

        internal static void GetProjectileGroupStats_RotationalType(out int count, out int whoAmI, Projectile projectile)
        {
            GetProjectileGroupStats((p) => p.owner == projectile.owner && Sets.MinionRotationalType[p.type], out count, out whoAmI, projectile.whoAmI);
        }

        /// <summary>
        /// Gets a radian which is a suggested rotation offset from the player. Rotate a Vector which goes upwards in order for this to apply correctly: new Vector2(0f, -1f)
        /// </summary>
        /// <param name="projectile"></param>
        /// <returns></returns>
        public static float GetSuggestedRotation(Projectile projectile)
        {
            GetProjectileGroupStats_RotationalType(out int count, out int whoAmI, projectile);
            if (count == 0)
            {
                return 0f; // ???
            }
            return GetSuggestedRotation(projectile, count, whoAmI);
        }

        public static float GetSuggestedRotation(Projectile projectile, int count, int whoAmI)
        {
            bool ownsChomper = Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Summon.Chomper>()] > 0;
            return InternalGetSuggestedRotation(ownsChomper, count, whoAmI);
        }

        internal static float InternalGetSuggestedRotation(bool ownsChomper, int rotationalCount, int rotationalIndex)
        {
            float startingRotation = 0f;
            float maxRotation = MathHelper.TwoPi;
            int useCount = rotationalCount;
            if (ownsChomper)
            {
                if (rotationalCount == 1)
                {
                    return MathHelper.Pi; // 180 degrees from the player, this makes it go downwards.
                }
                useCount--;
                startingRotation = MathHelper.PiOver2; // starts at a 90* angle
                maxRotation = MathHelper.Pi; // can only rotate 180 degrees instead of 360
            }
            return startingRotation + (maxRotation / useCount) * rotationalIndex;
        }

        public static float GetSuggestedRadius(Projectile projectile, float wantedRadius)
        {
            return wantedRadius;
        }
    }
}