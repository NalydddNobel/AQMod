using Aequus.Projectiles;
using Aequus.Projectiles.Misc;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public class SentryAccessoriesDatabase : ILoadable
    {
        public struct OnAIInfo
        {
            public Projectile Projectile;
            public SentryAccessoriesProj SentryAccessories;
            public Item Accessory;
            public Player Player;
        }
        public struct OnShootInfo
        {
            public IEntitySource Source;
            public Projectile Projectile;
            public Projectile ParentProjectile;
            public Item Accessory;
            public Player Player;
        }

        public static Dictionary<int, Action<OnAIInfo>> OnAI { get; private set; }
        public static Dictionary<int, Action<OnShootInfo>> OnShoot { get; private set; }

        public static MethodInfo Player_SpawnHallucination;

        void ILoadable.Load(Mod mod)
        {
            Player_SpawnHallucination = typeof(Player).GetMethod("SpawnHallucination", BindingFlags.NonPublic | BindingFlags.Instance);

            // Players get info effects from nearby players, maybe inherited info items should do the same?
            OnAI = new Dictionary<int, Action<OnAIInfo>>()
            {
                [ItemID.BrainOfConfusion] = ApplyEquipFunctional_AI,
                [ItemID.SporeSac] = SporeSac_AI,
                [ItemID.TerrasparkBoots] = WaterWalkingBoots_AI,
                [ItemID.LavaWaders] = WaterWalkingBoots_AI,
                [ItemID.ObsidianWaterWalkingBoots] = WaterWalkingBoots_AI,
                [ItemID.WaterWalkingBoots] = WaterWalkingBoots_AI,
                [ItemID.FireGauntlet] = ApplyEquipFunctional_AI,
                [ItemID.ArcticDivingGear] = ApplyEquipFunctional_AI,
                [ItemID.JellyfishDivingGear] = ApplyEquipFunctional_AI,
                [ItemID.JellyfishNecklace] = ApplyEquipFunctional_AI,
                [ItemID.Magiluminescence] = ApplyEquipFunctional_AI,
                [ItemID.FloatingTube] = InnerTube_AI,
                [ItemID.BoneHelm] = BoneHelm_AI,
                [ItemID.VolatileGelatin] = VolatileGelatin_AI,
                [ItemID.BoneGlove] = BoneGlove_AI,
            };

            OnShoot = new Dictionary<int, Action<OnShootInfo>>()
            {
                [ItemID.SharkToothNecklace] = SharkToothNecklace_OnShoot,
                [ItemID.BoneGlove] = BoneGlove_OnShoot,
            };
        }

        void ILoadable.Unload()
        {
            Player_SpawnHallucination = null;

            OnAI?.Clear();
            OnAI = null;
            OnShoot?.Clear();
            OnShoot = null;
        }

        public static void SporeSac_AI(OnAIInfo info)
        {
            var sporeSacProjs = new List<Projectile>();
            int myCount = 0;
            for (int i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == info.Projectile.owner && (Main.projectile[i].type == ProjectileID.SporeTrap || Main.projectile[i].type == ProjectileID.SporeTrap2
                    || Main.projectile[i].type == ModContent.ProjectileType<NaniteSpore>()))
                {
                    int identity = Main.projectile[i].Aequus().sourceProjIdentity;
                    if (identity >= 0)
                    {
                        sporeSacProjs.Add(Main.projectile[i]);
                        Main.projectile[i].owner = -1;
                        if (AequusHelpers.FindProjectileIdentity(info.Projectile.owner, identity) == info.Projectile.whoAmI)
                        {
                            myCount++;
                            if (myCount > 10)
                            {
                                goto Reset;
                            }
                        }
                    }
                }
            }
            info.SentryAccessories.dummyPlayer.SporeSac(info.Accessory);
        Reset:
            foreach (var p in sporeSacProjs)
            {
                p.owner = info.Projectile.owner;
            }
        }
        public static void WaterWalkingBoots_AI(OnAIInfo info)
        {
        }
        public static void InnerTube_AI(OnAIInfo info)
        {
        }
        public static void BoneHelm_AI(OnAIInfo info)
        {
            Player_SpawnHallucination.Invoke(info.SentryAccessories.dummyPlayer, new object[] { info.Accessory });
        }
        public static void VolatileGelatin_AI(OnAIInfo info)
        {
            info.SentryAccessories.dummyPlayer.VolatileGelatin(info.Accessory);
        }
        public static void BoneGlove_AI(OnAIInfo info)
        {
            info.SentryAccessories.dummyPlayer.boneGloveTimer--;
        }
        public static void ApplyEquipFunctional_AI(OnAIInfo info)
        {
            info.SentryAccessories.dummyPlayer.ApplyEquipFunctional(info.Accessory, false);
        }

        public static void SharkToothNecklace_OnShoot(OnShootInfo info)
        {
            info.Projectile.ArmorPenetration += 5;
        }
        public static void BoneGlove_OnShoot(OnShootInfo info)
        {
            info.ParentProjectile.TryGetGlobalProjectile<SentryAccessoriesProj>(out var parentSentry);
            if (Main.myPlayer != info.Player.whoAmI || parentSentry == null || parentSentry.dummyPlayer == null)
            {
                return;
            }

            parentSentry.dummyPlayer.boneGloveTimer = 60;
            var center = info.Projectile.Center;
            var vector = info.Projectile.DirectionTo(info.Player.ApplyRangeCompensation(0.2f, center, center + Vector2.Normalize(info.Projectile.velocity) * 100f)) * 10f;
            Projectile.NewProjectile(info.Player.GetSource_Accessory(info.Accessory), center.X, center.Y, vector.X, vector.Y, ProjectileID.BoneGloveProj, 25, 5f, info.Player.whoAmI);
        }
    }
}