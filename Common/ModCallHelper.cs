using AQMod.Content.WorldEvents.CrabSeason;
using AQMod.Content.WorldEvents.DemonSiege;
using System;
using System.Collections.Generic;
using Terraria;

namespace AQMod.Common
{
    internal static class ModCallHelper
    {
        private static Dictionary<string, Func<object[], object>> _calls;

        public static void Setup()
        {
            _calls = new Dictionary<string, Func<object[], object>>
            {
                { "glimmerevent.tilex", (o) => AQMod.glimmerEvent.tileX },
                { "glimmerevent.tiley", (o) => AQMod.glimmerEvent.tileY },
                { "glimmerevent.spawnchance", (o) => AQMod.glimmerEvent.spawnChance },
                { "glimmerevent.deactivationtimer", (o) => AQMod.glimmerEvent.deactivationTimer },
                { "glimmerevent.staritedisco", (o) => AQMod.glimmerEvent.StariteDisco },

                { "glimmerevent.tilex_set", (o) => AQMod.glimmerEvent.tileX = (ushort)o[1] },
                { "glimmerevent.tiley_set", (o) => AQMod.glimmerEvent.tileY = (ushort)o[1] },
                { "glimmerevent.spawnchance_set", (o) => AQMod.glimmerEvent.spawnChance = (int)o[1] },
                { "glimmerevent.deactivationtimer_set", (o) => AQMod.glimmerEvent.deactivationTimer = (int)o[1] },
                { "glimmerevent.staritedisco_set", (o) => AQMod.glimmerEvent.StariteDisco = (bool)o[1] },

                { "glimmerevent_isactive", (o) => AQMod.glimmerEvent.IsActive },
                { "glimmerevent_stariteprojectilecolor", (o) => AQMod.glimmerEvent.stariteProjectileColor },
                { "glimmerevent_activate", (o) =>
                    {
                        if (o.Length > 1 && o[1] is bool flag)
                        {
                            return AQMod.glimmerEvent.Activate(flag);
                        }
                        else
                        {
                            return AQMod.glimmerEvent.Activate();
                        }
                    }
                },
                { "glimmerevent_spawnsactive", (o) => AQMod.glimmerEvent.SpawnsActive((Player)o[1]) },
                { "glimmerevent_canshowinvasionprogress", (o) => AQMod.glimmerEvent.CanShowInvasionProgress() },
                { "glimmerevent_deactivate", (o) =>
                    {
                        AQMod.glimmerEvent.Deactivate();
                        return null;
                    }
                },
                { "glimmerevent_gettiledistance", (o) => AQMod.glimmerEvent.GetTileDistance((Player)o[1]) },

                { "demonsiege.x", (o) => DemonSiege.X },
                { "demonsiege.y", (o) => DemonSiege.Y },

                { "demonsiege.x_set", (o) => DemonSiege.X = (ushort)o[1] },
                { "demonsiege.y_set", (o) => DemonSiege.Y = (ushort)o[1] },

                { "demonsiege_activate", (o) => DemonSiege.Activate((int)o[1], (int)o[2], (int)o[3], (Item)o[4]) },
                {
                    "demonsiege_adddemonseigeenemy", (o) =>
                    {
                         if (o.Length > 5)
                         {
                             DemonSiege.AddDemonSeigeEnemy(new DemonSiegeEnemy((int)o[1], (DemonSiegeUpgradeProgression)o[2], (int)o[3], (int)o[4]));
                         }
                         else if (o.Length > 4)
                         {
                             DemonSiege.AddDemonSeigeEnemy(new DemonSiegeEnemy((int)o[1], (DemonSiegeUpgradeProgression)o[2], (int)o[3]));
                         }
                         else
                         {
                             DemonSiege.AddDemonSeigeEnemy(new DemonSiegeEnemy((int)o[1], (DemonSiegeUpgradeProgression)o[2]));
                         }
                        return null;
                    }
                },
                { "demonsiege_adddemonsiegeupgrade", (o) =>
                {
                    if (o.Length > 5)
                    {
                        DemonSiege.AddDemonSeigeUpgrade(new DemonSiegeUpgrade((int)o[1], (int)o[2], (DemonSiegeUpgradeProgression)o[3], (ushort)o[4]));
                    }
                    else
                    {
                        DemonSiege.AddDemonSeigeUpgrade(new DemonSiegeUpgrade((int)o[1], (int)o[2], (DemonSiegeUpgradeProgression)o[3]));
                    }
                    return null;
                }
                },
                { "demonsiege_closeenoughtodemonsiege", (o) => DemonSiege.CloseEnoughToDemonSiege((Player)o[1]) },
                {
                    "demonsiege_deactivate", (o) =>
                    {
                    DemonSiege.Deactivate();
                    return null;
                    }
                },

                { "crabseasontimer", (o) => CrabSeason.crabSeasonTimer },
                { "crabseasontimer_set", (o) => CrabSeason.crabSeasonTimer = (int)o[0] }
            };
        }

        private static void checkifnalydisstupid()
        {
            foreach (var keyValuePair in _calls)
            {
                string lower = keyValuePair.Key.ToLower();
                if (lower != keyValuePair.Key)
                {
                    throw new Exception(keyValuePair.Key + " is not lowercase, it is actually: " + lower);
                }
            }
        }

        public static void Unload()
        {
            _calls = null;
        }

        public static bool VerifyCall(object[] args)
        {
            if (args == null || args.Length == 0)
            {
                return false;
            }
            return true;
        }

        public static object InvokeCall(object[] args)
        {
            string callType = ((string)args[0]).ToLower();
            if (_calls.TryGetValue(callType, out var method))
            {
                return method.Invoke(args);
            }
            return null;
        }
    }
}