using AQMod.Content.World.Events.GlimmerEvent;
using AQMod.Projectiles.FallingStars;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content.World.FallingStars
{
    public sealed class ImitatedFallingStars : ModWorld
    {
        public static int SpawnManaCrystal;
        public static int SpawnLifeCrystal;
        public static bool CanCosmicBurpHappen => !GlimmerEvent.IsActive && !Main.bloodMoon;

        public override void Initialize()
        {
            SpawnManaCrystal = -1;
            SpawnLifeCrystal = -1;
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["Version"] = AQMod.Instance.Version.ToString(),
                ["SpawnManaCrystal"] = SpawnManaCrystal,
                ["SpawnLifeCrystal"] = SpawnLifeCrystal,
            };
        }

        public override void Load(TagCompound tag)
        {
            if (!tag.ContainsKey("Version"))
            {
                return;
            }
            SpawnManaCrystal = tag.GetInt("SpawnManaCrystal");
            SpawnLifeCrystal = tag.GetInt("SpawnLifeCrystal");
        }

        public override void PostUpdate()
        {
            if (PassingDays.OnTurnNight)
            {
                if (CanCosmicBurpHappen && (PassingDays.daysPassedSinceLastGlimmerEvent <= 1 || Main.rand.NextBool(3)))
                {
                    UpdateCosmicBurp();
                }
            }
            else if (Main.dayTime)
            {
                SpawnManaCrystal = -1;
                SpawnLifeCrystal = -1;
            }
            else
            {
                if (SpawnManaCrystal != -1 && Main.time > SpawnManaCrystal)
                {
                    SpawnManaCrystal = -1;
                    CastFallingStar(ModContent.ProjectileType<ManaCrystalFallingStar>());
                }
                if (SpawnLifeCrystal != -1 && Main.time > SpawnLifeCrystal)
                {
                    SpawnLifeCrystal = -1;
                    CastFallingStar(ModContent.ProjectileType<LifeCrystalFallingStar>());
                }
            }
        }

        private static void UpdateCosmicBurp() // Some weird logic here, since this used to use a different system that supported more than one cosmic burp event thingy using flags
        {
            if (Main.rand.NextBool())
            {
                SpawnLifeCrystal = Main.rand.Next(1000, (int)Main.nightLength - 1000);
            }

            if (SpawnLifeCrystal < 999 || Main.rand.NextBool())
            {
                SpawnManaCrystal = Main.rand.Next(1000, (int)Main.nightLength - 1000);
            }
        }

        public static void CastFallingStar(int projectileID = ProjectileID.FallingStar, int damage = 1000, float knockback = 10f)
        {
            int num44 = Main.rand.Next(Main.maxTilesX - 50) + 100;
            num44 *= 16;
            int num45 = Main.rand.Next((int)((double)Main.maxTilesY * 0.05));
            num45 *= 16;
            var vector = new Vector2(num44, num45);
            float num46 = Main.rand.Next(-100, 101);
            float num47 = Main.rand.Next(200) + 100;
            float num48 = (float)Math.Sqrt(num46 * num46 + num47 * num47);
            num48 = 12f / num48;
            num46 *= num48;
            num47 *= num48;
            Projectile.NewProjectile(vector.X, vector.Y, num46, num47, projectileID, damage, knockback, Main.myPlayer);
        }
    }
}