using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.StatSheet
{
    public class StatTrackerPlayer : ModPlayer
    {
        public struct PlayerUpdateInfo : IStatUpdateInfo
        {
            public string Context { get; set; }
            public Player player;
        }
        public struct OnHitInfo : IStatUpdateInfo
        {
            public string Context { get; set; }
            public Player player;
            public Entity PlayerOrNPCVictim;
            public Entity ProjectileOrItem;
            public int damage;
            public float knockback;
            public bool crit;
        }

        public OnHitInfo onHitInfo;
        public PlayerUpdateInfo playerUpdateInfo;

        public static List<IStatTracker> SavesWithPlayer { get; private set; }

        public override void Load()
        {
            SavesWithPlayer = new List<IStatTracker>();
        }

        public override void SetStaticDefaults()
        {
            RegisterSavableHook<int>(StatSheetManager.StatHooks.Player_PostUpdate,
                (i, s) => (((PlayerUpdateInfo)i).player.Aequus().idleTime > 0 ? 1 : 0) + s.stat, "IdleTime");
            RegisterSavableHook<float>(StatSheetManager.StatHooks.Player_PostUpdate,
                (i, s) =>
                {
                    float movement = ((PlayerUpdateInfo)i).player.velocity.X.Abs() / 16f;
                    return movement + s.stat;
                }, "HorizontalDistanceTraveled");
            RegisterSavableHook<float>(StatSheetManager.StatHooks.Player_PostUpdate,
                (i, s) =>
                {
                    float movement = ((PlayerUpdateInfo)i).player.velocity.Y.Abs() / 16f;
                    return movement + s.stat;
                }, "VerticalDistanceTraveled");
            RegisterSavableHook<float>(StatSheetManager.StatHooks.Player_PostUpdate,
                (i, s) =>
                {
                    float movement = ((PlayerUpdateInfo)i).player.velocity.Length();
                    return movement > s.stat ? movement : s.stat;
                }, "FastestSpeed");
            RegisterSavableHook<int>(StatSheetManager.StatHooks.Player_OnHitVictim,
                (i, s) => ((OnHitInfo)i).damage + s.stat, "TotalDamageDealt");
            RegisterSavableHook<int>(StatSheetManager.StatHooks.Player_OnHitVictim,
                (i, s) => {
                    var onHit = (OnHitInfo)i;
                    if (onHit.ProjectileOrItem is Projectile proj && (proj.minion || proj.sentry || ProjectileID.Sets.MinionShot[proj.type] || ProjectileID.Sets.SentryShot[proj.type]))
                    {
                        return onHit.damage + s.stat;
                    }
                    return s.stat;
                }, "TotalMinionDamageDealt");
            RegisterSavableHook<int>(StatSheetManager.StatHooks.Player_OnHitVictim,
                (i, s) => {
                    int damage = ((OnHitInfo)i).damage;
                    return damage > s.stat ? damage : s.stat;
                }, "MostDamageDealt");
            RegisterSavableHook<int>(StatSheetManager.StatHooks.Player_OnHitVictim,
                (i, s) => {
                    return (((OnHitInfo)i).crit ? 1 : 0) + s.stat;
                }, "TotalCritsDealt");
        }

        public static void RegisterSavableHook<T>(List<IStatTracker> hooklist, Func<IStatUpdateInfo, HookedStatTracker<T>, T> func, string name, string key = null)
        {
            SavesWithPlayer.Add(HookedStatTracker<T>.Register(hooklist, func, name, key));
        }

        public override void SaveData(TagCompound tag)
        {
            foreach (var stat in SavesWithPlayer)
            {
                tag[stat.Name] = stat.ProvideSaveData();
            }
        }

        public override void LoadData(TagCompound tag)
        {
            foreach (var stat in SavesWithPlayer)
            {
                stat.LoadSaveData(tag);
            }
        }

        public override void OnHitPvpWithProj(Projectile proj, Player target, int damage, bool crit)
        {
            if (!CheckPlayer())
                return;

            onHitInfo.Context = "OnHitPvp_Proj";
            onHitInfo.player = Player;
            onHitInfo.ProjectileOrItem = proj;
            onHitInfo.PlayerOrNPCVictim = target;
            onHitInfo.damage = damage;
            onHitInfo.knockback = 1f;
            onHitInfo.crit = crit;
            StatSheetManager.StatHooks.UpdateHooklist(onHitInfo, StatSheetManager.StatHooks.Player_OnHitVictim);
        }
        public override void OnHitPvp(Item item, Player target, int damage, bool crit)
        {
            if (!CheckPlayer())
                return;

            onHitInfo.Context = "OnHitPvp_Item";
            onHitInfo.player = Player;
            onHitInfo.ProjectileOrItem = item;
            onHitInfo.PlayerOrNPCVictim = target;
            onHitInfo.damage = damage;
            onHitInfo.knockback = 1f;
            onHitInfo.crit = crit;
            StatSheetManager.StatHooks.UpdateHooklist(onHitInfo, StatSheetManager.StatHooks.Player_OnHitVictim);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (!CheckPlayer())
                return;

            onHitInfo.Context = "OnHitNPC_Proj";
            onHitInfo.player = Player;
            onHitInfo.ProjectileOrItem = proj;
            onHitInfo.PlayerOrNPCVictim = target;
            onHitInfo.damage = damage;
            onHitInfo.knockback = knockback;
            onHitInfo.crit = crit;
            StatSheetManager.StatHooks.UpdateHooklist(onHitInfo, StatSheetManager.StatHooks.Player_OnHitVictim);
        }
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (!CheckPlayer())
                return;

            onHitInfo.Context = "OnHitNPC_Item";
            onHitInfo.player = Player;
            onHitInfo.ProjectileOrItem = item;
            onHitInfo.PlayerOrNPCVictim = target;
            onHitInfo.damage = damage;
            onHitInfo.knockback = knockback;
            onHitInfo.crit = crit;
            StatSheetManager.StatHooks.UpdateHooklist(onHitInfo, StatSheetManager.StatHooks.Player_OnHitVictim);
        }

        public override void PostUpdate()
        {
            if (!CheckPlayer())
                return;

            playerUpdateInfo.Context = "PostUpdate";
            playerUpdateInfo.player = Player;
            StatSheetManager.StatHooks.UpdateHooklist(playerUpdateInfo, StatSheetManager.StatHooks.Player_PostUpdate);
        }

        private bool CheckPlayer()
        {
            if (Main.myPlayer == Player.whoAmI)
            {
                return true;
            }
            return false;
        }
    }
}