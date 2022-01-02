using AQMod.Common.DeveloperTools;
using AQMod.Common.NetCode;
using AQMod.Content.LegacyWorldEvents.CrabSeason;
using AQMod.Content.LegacyWorldEvents.DemonSiege;
using AQMod.Content.World;
using AQMod.Content.World.Events.GaleStreams;
using AQMod.Content.World.Events.GlimmerEvent;
using AQMod.Content.World.FallingStars;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Common
{
    internal static class ModCallDictionary
    {
        public static class Auto
        {
            public static void CreateCallsForType<T>() where T : class
            {
                object instance = null;
                try
                {
                    instance = ModContent.GetInstance<T>();
                }
                catch
                {
                }
                if (instance == null)
                    CreateCallsForType((T)Activator.CreateInstance(typeof(T)));
                else
                    CreateCallsForType((T)instance);
            }
            public static void CreateCallsForType<T>(T Instance) where T : class
            {
                string typeName = typeof(T).Name.ToLower();
                var fields = typeof(T).GetFields();
                foreach (var f in fields)
                {
                    AQMod.Instance.Logger.Debug(typeName + "." + f.Name.ToLower());
                    _calls.Add(typeName + "." + f.Name.ToLower(), (o) => f.GetValue(Instance));
                    if (!f.IsInitOnly)
                    {
                        _calls.Add(typeName + "." + f.Name.ToLower() + "_set", (o) =>
                        {
                            f.SetValue(Instance, o[1]);
                            return o[1];
                        });
                    }
                }
            }
        }

        private static Dictionary<string, Func<object[], object>> _calls;


        public static void Load()
        {
            _calls = new Dictionary<string, Func<object[], object>>
            {
                { "addloadtask", (o) =>
                    {
                        AQMod.addLoadTask(new CachedTask((object)o[1], (Func<object, object>)o[2]));
                        return null;
                    }
                },

                //{ "glimmerevent.tilex", (o) => GlimmerEvent.tileX },
                //{ "glimmerevent.tiley", (o) => GlimmerEvent.tileY },
                //{ "glimmerevent.deactivationtimer", (o) => GlimmerEvent.deactivationTimer },
                { "glimmerevent.staritedisco", (o) => GlimmerEvent.StariteDisco },

                //{ "glimmerevent.tilex_set", (o) => GlimmerEvent.tileX = (ushort)o[1] },
                //{ "glimmerevent.tiley_set", (o) => GlimmerEvent.tileY = (ushort)o[1] },
                //{ "glimmerevent.deactivationtimer_set", (o) => GlimmerEvent.deactivationTimer = (int)o[1] },
                { "glimmerevent.staritedisco_set", (o) => GlimmerEvent.StariteDisco = (bool)o[1] },

                { "glimmerevent_isactive", (o) => GlimmerEvent.IsActive },
                { "glimmerevent_stariteprojectilecolor", (o) => GlimmerEvent.stariteProjectileColor },
                { "glimmerevent_activate", (o) =>
                    {
                        bool value = GlimmerEvent.Activate();
                        NetHelper.GlimmerEventNetUpdate();
                        return value;
                    }
                },
                { "glimmerevent_spawnsactive", (o) => GlimmerEvent.SpawnsActive((Player)o[1]) },
                { "glimmerevent_canshowinvasionprogress", (o) => GlimmerEvent.CanShowInvasionProgress() },
                { "glimmerevent_deactivate", (o) =>
                    {
                        GlimmerEvent.Deactivate();
                        return null;
                    }
                },
                { "glimmerevent_gettiledistance", (o) => GlimmerEvent.GetTileDistance((Player)o[1]) },

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
                { "worlddefeats.downedcrabson_set", (o) => WorldDefeats.DownedCrabson = (bool)o[1]},
                { "worlddefeats.downedstarite", (o) => WorldDefeats.DownedStarite },
                { "worlddefeats.downedstarite_set", (o) => WorldDefeats.DownedStarite = (bool)o[1]},
                { "worlddefeats.nohitomegastarite", (o) => WorldDefeats.NoHitOmegaStarite },
                { "worlddefeats.nohitomegastarite_set", (o) => WorldDefeats.NoHitOmegaStarite = (bool)o[1] },

                { "worlddefeats.downedredsprite", (o) => WorldDefeats.DownedRedSprite },
                { "worlddefeats.downedredsprite_set", (o) => WorldDefeats.DownedRedSprite = (bool)o[1]},

                { "worlddefeats.downedcrabseason", (o) => WorldDefeats.DownedCrabSeason },
                { "worlddefeats.downedcrabseason_set", (o) => WorldDefeats.DownedCrabSeason = (bool)o[1]},
                { "worlddefeats.downedglimmer", (o) => WorldDefeats.DownedGlimmer },
                { "worlddefeats.downedglimmer_set", (o) => WorldDefeats.DownedGlimmer = (bool)o[1]},
                { "worlddefeats.downeddemonsiege", (o) => WorldDefeats.DownedDemonSiege },
                { "worlddefeats.downeddemonsiege_set", (o) => WorldDefeats.DownedDemonSiege = (bool)o[1]},
                { "worlddefeats.downedgalestreams", (o) => WorldDefeats.DownedGaleStreams },
                { "worlddefeats.downedgalestreams_set", (o) => WorldDefeats.DownedGaleStreams = (bool)o[1]},

                { "worlddefeats.obtainedmothmanmask", (o) => WorldDefeats.ObtainedMothmanMask },
                { "worlddefeats.obtainedmothmanmask_set", (o) => WorldDefeats.ObtainedMothmanMask = (bool)o[1]},
                { "worlddefeats.obtainedcatalystpainting", (o) => WorldDefeats.ObtainedCatalystPainting },
                { "worlddefeats.obtainedcatalystpainting_set", (o) => WorldDefeats.ObtainedCatalystPainting = (bool)o[1]},
            };

            Auto.CreateCallsForType(ModContent.GetInstance<GlimmerEvent>());
            Auto.CreateCallsForType(ModContent.GetInstance<GaleStreams>());
            Auto.CreateCallsForType(ModContent.GetInstance<PassingDays>());
            Auto.CreateCallsForType(ModContent.GetInstance<ImitatedFallingStars>());

            checkifnalydisstupid();
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