using AQMod.Common.DeveloperTools;
using AQMod.Content.WorldEvents.CrabSeason;
using AQMod.Content.WorldEvents.DemonSiege;
using AQMod.Content.WorldEvents.GlimmerEvent;
using System;
using System.Collections.Generic;
using Terraria;

namespace AQMod.Common
{
    internal static class ModCallHelper
    {
        private static Dictionary<string, Func<object[], object>> _calls;

        public static void SetupCalls()
        {
            _calls = new Dictionary<string, Func<object[], object>>
            {
                { "addloadtask", (o) =>
                    {
                        AQMod.addLoadTask(new CachedTask((object)o[1], (Func<object, object>)o[2]));
                        return null;
                    }
                },

                { "glimmerevent.tilex", (o) => AQMod.CosmicEvent.tileX },
                { "glimmerevent.tiley", (o) => AQMod.CosmicEvent.tileY },
                { "glimmerevent.spawnchance", (o) => AQMod.CosmicEvent.spawnChance },
                { "glimmerevent.deactivationtimer", (o) => AQMod.CosmicEvent.deactivationTimer },
                { "glimmerevent.staritedisco", (o) => AQMod.CosmicEvent.StariteDisco },

                { "glimmerevent.tilex_set", (o) => AQMod.CosmicEvent.tileX = (ushort)o[1] },
                { "glimmerevent.tiley_set", (o) => AQMod.CosmicEvent.tileY = (ushort)o[1] },
                { "glimmerevent.spawnchance_set", (o) => AQMod.CosmicEvent.spawnChance = (int)o[1] },
                { "glimmerevent.deactivationtimer_set", (o) => AQMod.CosmicEvent.deactivationTimer = (int)o[1] },
                { "glimmerevent.staritedisco_set", (o) => AQMod.CosmicEvent.StariteDisco = (bool)o[1] },

                { "glimmerevent_isactive", (o) => AQMod.CosmicEvent.IsActive },
                { "glimmerevent_stariteprojectilecolor", (o) => AQMod.CosmicEvent.stariteProjectileColor },
                { "glimmerevent_activate", (o) =>
                    {
                        if (o.Length > 1 && o[1] is bool flag)
                        {
                            return AQMod.CosmicEvent.Activate(flag);
                        }
                        else
                        {
                            return AQMod.CosmicEvent.Activate();
                        }
                    }
                },
                { "glimmerevent_spawnsactive", (o) => AQMod.CosmicEvent.SpawnsActive((Player)o[1]) },
                { "glimmerevent_canshowinvasionprogress", (o) => GlimmerEvent.CanShowInvasionProgress() },
                { "glimmerevent_deactivate", (o) =>
                    {
                        AQMod.CosmicEvent.Deactivate();
                        return null;
                    }
                },
                { "glimmerevent_gettiledistance", (o) => AQMod.CosmicEvent.GetTileDistance((Player)o[1]) },

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
                { "crabseasontimer_set", (o) => CrabSeason.crabSeasonTimer = (int)o[1] },

                { "worlddefeats.downedcrabson", (o) => WorldDefeats.DownedCrabson },
                { "worlddefeats.downeddemonsiege", (o) => WorldDefeats.DownedDemonSiege },
                { "worlddefeats.downedglimmer", (o) => WorldDefeats.DownedGlimmer },
                { "worlddefeats.downedstarite", (o) => WorldDefeats.DownedStarite },
                { "worlddefeats.downedyinyang", (o) => WorldDefeats.DownedYinYang },

                { "worlddefeats.downedcrabson_set", (o) => WorldDefeats.DownedCrabson = (bool)o[1]},
                { "worlddefeats.downeddemonsiege_set", (o) => WorldDefeats.DownedDemonSiege = (bool)o[1]},
                { "worlddefeats.downedglimmer_set", (o) => WorldDefeats.DownedGlimmer = (bool)o[1]},
                { "worlddefeats.downedstarite_set", (o) => WorldDefeats.DownedStarite = (bool)o[1]},
                { "worlddefeats.downedyinyang_set", (o) => WorldDefeats.DownedYinYang = (bool)o[1]},
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