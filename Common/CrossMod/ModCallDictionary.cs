using AQMod.Common.Utilities;
using AQMod.Common.Utilities.Debugging;
using AQMod.Content.World;
using AQMod.Content.World.Events;
using AQMod.Content.World.FallingStars;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common.CrossMod
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
                var attributes = typeof(T).GetCustomAttributes(inherit: true);
                foreach (var attr in attributes)
                {
                    if (attr is CallSpecialAttribute)
                    {
                        InternalCreateCalls(Instance, (CallSpecialAttribute)attr);
                        return;
                    }
                }
                InternalCreateCalls(Instance);
            }

            private static void InternalCreateCalls<T>(T Instance, CallSpecialAttribute attr) where T : class
            {
                attr.type = Instance.GetType();

                if (attr.AddCallsForFields)
                    InternalCreateCalls(attr.Name, Instance, attr.Fields, null);
                attr.CustomCreateCalls(Instance, _calls);
                attr.type = null;
            }

            private static void InternalCreateCalls<T>(T Instance) where T : class
            {
                string typeName = typeof(T).TypeName();
                
                var fields = typeof(T).GetFields();
                var properties = typeof(T).GetProperties();
                InternalCreateCalls(typeName, Instance, fields, properties);
            }

            private static void InternalCreateCalls<T>(string typeName, T Instance, FieldInfo[] fields, PropertyInfo[] properties)
            {
                DebugUtilities.Logger? l = null;
                if (DebugUtilities.LogModCallObjectInitialization)
                    l = DebugUtilities.GetDebugLogger();
                if (fields != null)
                {
                    foreach (var f in fields)
                    {
                        if (f.GetCustomAttribute<ModCallLeaveOutAttribute>() != null)
                        {
                            continue;
                        }
                        l?.Log(typeName + "." + f.Name);
                        _calls.Add(typeName + "." + f.Name, (o) => f.GetValue(Instance));
                        if (!f.IsInitOnly)
                        {
                            _calls.Add(typeName + "." + f.Name + "_set", (o) =>
                            {
                                f.SetValue(Instance, o[1]);
                                return o[1];
                            });
                        }
                    }
                }
                if (properties != null)
                {
                    foreach (var p in properties)
                    {
                        if (p.GetCustomAttribute<ModCallLeaveOutAttribute>() != null)
                        {
                            continue;
                        }
                        l?.Log(typeName + "." + p.Name);
                        _calls.Add(typeName + "." + p.Name, (o) => p.GetValue(Instance));
                        if (InternalAddList<T, int>(p, typeName, Instance))
                        {
                            continue;
                        }
                        if (InternalAddList<T, ushort>(p, typeName, Instance))
                        {
                            continue;
                        }
                        if (InternalAddList<T, short>(p, typeName, Instance))
                        {
                            continue;
                        }
                        if (InternalAddHashSet<T, int>(p, typeName, Instance))
                        {
                            continue;
                        }
                        if (InternalAddHashSet<T, ushort>(p, typeName, Instance))
                        {
                            continue;
                        }
                        if (InternalAddHashSet<T, short>(p, typeName, Instance))
                        {
                            continue;
                        }
                    }
                }
            }
        }

        private static bool InternalAddList<T, T2>(PropertyInfo p, string typeName, T Instance)
        {
            if (p.PropertyType == typeof(List<T2>))
            {
                _calls.Add(typeName + "." + p.Name + ".add", (o) =>
                {
                    ((List<T2>)p.GetValue(Instance)).Add((T2)o[1]);
                    return null;
                });
                return true;
            }
            return false;
        }

        private static bool InternalAddHashSet<T, T2>(PropertyInfo p, string typeName, T Instance)
        {
            if (p.PropertyType == typeof(HashSet<T2>))
            {
                _calls.Add(typeName + "." + p.Name + ".add", (o) => ((HashSet<T2>)p.GetValue(Instance)).Add((T2)o[1]));
                return true;
            }
            return false;
        }

        private static Dictionary<string, Func<object[], object>> _calls;

        public static void Load()
        {
            _calls = new Dictionary<string, Func<object[], object>>
            {
                { "addloadtask", (o) =>
                    {
                        AQMod.GetInstance().cachedLoadTasks.Add(new CachedTask(o[1], (Func<object, object>)o[2]));
                        return null;
                    }
                },

                { "glimmerevent_isactive", (o) => Glimmer.IsGlimmerEventCurrentlyActive() },
                { "glimmerevent_activate", (o) =>
                    {
                        bool value = Glimmer.Activate();
                        NetMessage.SendData(MessageID.WorldData);
                        return value;
                    }
                },
                { "glimmerevent_spawnsactive", (o) => Glimmer.SpawnsCheck((Player)o[1]) },
                { "glimmerevent_canshowinvasionprogress", (o) => Glimmer.IsAbleToShowInvasionProgressBar() },
                { "glimmerevent_deactivate", (o) =>
                    {
                        Glimmer.Deactivate();
                        return null;
                    }
                },
                { "glimmerevent_gettiledistance", (o) => Glimmer.Distance((Player)o[1]) },

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
                             DemonSiege.AddDemonSeigeEnemy(new DemonSiege.SiegeEnemy((int)o[1], (DemonSiege.UpgradeProgression)o[2], (int)o[3], (int)o[4]));
                         }
                         else if (o.Length > 4)
                         {
                             DemonSiege.AddDemonSeigeEnemy(new DemonSiege.SiegeEnemy((int)o[1], (DemonSiege.UpgradeProgression)o[2], (int)o[3]));
                         }
                         else
                         {
                             DemonSiege.AddDemonSeigeEnemy(new DemonSiege.SiegeEnemy((int)o[1], (DemonSiege.UpgradeProgression)o[2]));
                         }
                        return null;
                    }
                },
                { "demonsiege_adddemonsiegeupgrade", (o) =>
                {
                    if (o.Length > 5)
                    {
                        DemonSiege.AddDemonSeigeUpgrade(new DemonSiege.SiegeUpgrade((int)o[1], (int)o[2], (DemonSiege.UpgradeProgression)o[3], (ushort)o[4]));
                    }
                    else
                    {
                        DemonSiege.AddDemonSeigeUpgrade(new DemonSiege.SiegeUpgrade((int)o[1], (int)o[2], (DemonSiege.UpgradeProgression)o[3]));
                    }
                    return null;
                }
                },
                { "demonsiege_closeenoughtodemonsiege", (o) => ((Player)o[1]).Biomes().zoneDemonSiege },
                {
                    "demonsiege_deactivate", (o) =>
                    {
                    DemonSiege.Deactivate();
                    return null;
                    }
                },
                { "biomes.zonedemonsiege", (o) => ((Player)o[1]).Biomes().zoneDemonSiege },

                { "worlddefeats.downedcrabson", (o) => WorldDefeats.DownedCrabson },
                { "worlddefeats.downedcrabson_set", (o) => WorldDefeats.DownedCrabson = (bool)o[1]},
                { "worlddefeats.downedstarite", (o) => WorldDefeats.DownedStarite },
                { "worlddefeats.downedstarite_set", (o) => WorldDefeats.DownedStarite = (bool)o[1]},
                { "worlddefeats.nohitomegastarite", (o) => WorldDefeats.NoHitOmegaStarite },
                { "worlddefeats.nohitomegastarite_set", (o) => WorldDefeats.NoHitOmegaStarite = (bool)o[1] },

                { "worlddefeats.downedredsprite", (o) => WorldDefeats.DownedRedSprite },
                { "worlddefeats.downedredsprite_set", (o) => WorldDefeats.DownedRedSprite = (bool)o[1]},
                { "worlddefeats.downedspacesquid", (o) => WorldDefeats.DownedSpaceSquid },
                { "worlddefeats.downedspacesquid_set", (o) => WorldDefeats.DownedSpaceSquid = (bool)o[1]},

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

            Auto.CreateCallsForType(ModContent.GetInstance<Glimmer>());
            Auto.CreateCallsForType(ModContent.GetInstance<GaleStreams>());
            Auto.CreateCallsForType(ModContent.GetInstance<PassingDays>());
            Auto.CreateCallsForType(ModContent.GetInstance<ImitatedFallingStars>());
            Auto.CreateCallsForType<AQItem.Sets>();
            Auto.CreateCallsForType<AQBuff.Sets>();
            Auto.CreateCallsForType<AQProjectile.Sets>();
            Auto.CreateCallsForType<AQTile.Sets>();

            Lowercase();
        }

        private static void Lowercase()
        {
            var renames = new List<KeyValuePair<string, Func<object[], object>>>();
            foreach (var keyValuePair in _calls)
            {
                //AQMod.GetInstance().Logger.Debug(keyValuePair.Key);
                string lower = keyValuePair.Key.ToLower();
                if (lower != keyValuePair.Key)
                {
                    renames.Add(keyValuePair);
                }
            }
            foreach (var keyValuePair in renames)
            {
                _calls.Remove(keyValuePair.Key);
                _calls.Add(keyValuePair.Key.ToLower(), keyValuePair.Value);
            }
        }

        public static void Unload()
        {
            _calls?.Clear();
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
                if (DebugUtilities.LogModCalls)
                {
                    DebugUtilities.GetDebugLogger().Log("calling type: " + callType);
                }
                var value = method.Invoke(args);
                if (DebugUtilities.LogModCalls)
                {
                    if (value == null)
                    {
                        DebugUtilities.GetDebugLogger().Log("call has returned a null value");
                    }
                    else
                    {
                        DebugUtilities.GetDebugLogger().Log("call has returned a value of " + value.GetType().FullName + " (" + value.ToString() + ")");
                    }
                }
                return value;
            }
            if (DebugUtilities.LogModCalls)
            {
                DebugUtilities.GetDebugLogger().Log("Invalid call type! (" + callType + ")");
            }
            return null;
        }
    }
}