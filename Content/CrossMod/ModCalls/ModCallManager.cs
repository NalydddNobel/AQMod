using Aequus.Common.GlobalNPCs;
using Aequus.Common.Utilities.TypeUnboxing;
using Aequus.Content.Events.DemonSiege;
using Aequus.Content.Necromancy;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod.ModCalls {
    /// <summary>
    /// Check https://terrariamods.wiki.gg/wiki/Aequus/Mod_Calls
    /// </summary>
    public class ModCallManager : ILoadable
    {
        public const string Success = "Success";
        public const string Failure = "Failure";

        private static object[] argument;
        private static int nextArg;
        private static string type;
        private static Mod mod;

        public static object HandleModCall(object[] args)
        {
            argument = args;
            nextArg = 0;
            type = "Unknown";
            mod = null;
            try
            {
                type = Get<string>();
                mod = Get<Mod>();
                switch (type)
                {
                    case "downedCrabson":
                        return GetterSetter(ref AequusWorld.downedCrabson);
                    case "downedOmegaStarite":
                        return GetterSetter(ref AequusWorld.downedOmegaStarite);
                    case "downedDustDevil":
                        return GetterSetter(ref AequusWorld.downedDustDevil);
                    case "downedHyperStarite":
                        return GetterSetter(ref AequusWorld.downedHyperStarite);
                    case "downedUltraStarite":
                        return GetterSetter(ref AequusWorld.downedUltraStarite);
                    case "downedRedSprite":
                        return GetterSetter(ref AequusWorld.downedRedSprite);
                    case "downedSpaceSquid":
                        return GetterSetter(ref AequusWorld.downedSpaceSquid);
                    case "downedEventDemon":
                        return GetterSetter(ref AequusWorld.downedEventDemon);
                    case "downedEventCosmic":
                        return GetterSetter(ref AequusWorld.downedEventCosmic);
                    case "downedEventAtmosphere":
                        return GetterSetter(ref AequusWorld.downedEventAtmosphere);
                    case "chestAdamantiteTier":
                        return GetterSetter(ref AequusWorld.chestAdamantiteTier);
                    case "chestMythrilTier":
                        return GetterSetter(ref AequusWorld.chestMythrilTier);
                    case "chestCobaltTier":
                        return GetterSetter(ref AequusWorld.chestCobaltTier);
                    case "hardmodeChests":
                        return GetterSetter(ref AequusWorld.hardmodeChests);
                    case "downedUpriser":
                        return GetterSetter(ref AequusWorld.downedUpriser);
                    case "shadowOrbsBrokenTotal":
                        return GetterSetter(ref AequusWorld.shadowOrbsBrokenTotal);
                    case "tinkererRerolls":
                        return GetterSetter(ref AequusWorld.tinkererRerolls);
                    case "usedWhiteFlag":
                        return GetterSetter(ref AequusWorld.usedWhiteFlag);
                    case "xmasHats":
                        return GetterSetter(ref AequusWorld.xmasHats);
                    case "xmasWorld":
                        return GetterSetter(ref AequusWorld.xmasWorld);

                    // Args: int/NPC - NPC, int/Player - Player | Returns: whether or not the player has been hit
                    case "Flawless":
                        {
                            var npc = GetNPC();
                            var player = GetPlayer();
                            return npc.GetGlobalNPC<FlawlessGlobalNPC>().damagedPlayers[player.whoAmI];
                        }

                    // Args: int/NPC - NPC | Returns: value of FlawlessGlobalNPC.preventNoHitCheck on the provided NPC
                    case "FlawlessCheck":
                        {
                            var npc = GetNPC();
                            return GetterSetter(ref npc.GetGlobalNPC<FlawlessGlobalNPC>().preventNoHitCheck);
                        }

                    // Args: int/NPC - NPC | Returns: bool[] of size Main.maxPlayers
                    case "FlawlessStat":
                        return GetNPC().GetGlobalNPC<FlawlessGlobalNPC>().damagedPlayers;

                    // Args: int - Buff ID (Optional: Color - Color) | Returns: Color of the provided buff ID
                    case "PotionBuffColor":
                        {
                            int buff = Get<int>();
                            if (TryGet<Color>(out var color))
                            {
                                if (Aequus.LogMore)
                                    Aequus.Instance.Logger.Info($"{mod.Name}: Setting Potion Color for Buff {Lang.GetBuffName(buff)} ({buff}) to {color}");
                                PotionColorsDatabase.BuffToColor[buff] = color;
                            }
                            if (!PotionColorsDatabase.BuffToColor.TryGetValue(buff, out var buffColor))
                            {
                                return Color.Transparent;
                            }
                            return buffColor;
                        }

                    // Args: int - Item (Optional: Color - Color) | Returns: Color of the provided item ID
                    case "PotionItemColor":
                        {
                            int item = Get<int>();
                            if (TryGet<Color>(out var color))
                            {
                                if (Aequus.LogMore)
                                    Aequus.Instance.Logger.Info($"{mod.Name}: Setting Potion Color for Item {Lang.GetItemNameValue(item)} ({item}) to {color}");
                                PotionColorsDatabase.ItemToBuffColor[item] = color;
                            }
                            if (!PotionColorsDatabase.ItemToBuffColor.TryGetValue(item, out var buffColor))
                            {
                                return Color.Transparent;
                            }
                            return buffColor;
                        }

                    // Args: int - Projectile (Optional: bool - shouldBePushable) | Returns: If the provided Projectile ID can be pushed
                    case "PushableProjectile":
                        {
                            int proj = Get<int>();
                            if (TryGet<bool>(out var shouldBePushable))
                            {
                                if (Aequus.LogMore)
                                    Aequus.Instance.Logger.Info($"{mod.Name}: Setting Pushable state for for Projectile {Lang.GetProjectileName(proj)} ({proj}) to {shouldBePushable}");
                                if (shouldBePushable)
                                {
                                    PushableEntities.ProjectileIDs.Add(proj);
                                }
                                else
                                {
                                    PushableEntities.ProjectileIDs.Remove(proj);
                                }
                            }
                            return PushableEntities.ProjectileIDs.Contains(proj);
                        }

                    // Args: int - NPC (Optional: bool - shouldBePushable) | Returns: If the provided NPC ID can be pushed
                    case "PushableNPC":
                        {
                            int npc = Get<int>();
                            if (TryGet<bool>(out var shouldBePushable))
                            {
                                if (Aequus.LogMore)
                                    Aequus.Instance.Logger.Info($"{mod.Name}: Setting Pushable state for for NPC {Lang.GetNPCNameValue(npc)} ({npc}) to {shouldBePushable}");
                                if (shouldBePushable)
                                {
                                    PushableEntities.NPCIDs.Add(npc);
                                }
                                else
                                {
                                    PushableEntities.NPCIDs.Remove(npc);
                                }
                            }
                            return PushableEntities.NPCIDs.Contains(npc);
                        }

                    case "SentryAccessory":
                        {
                            int item = Get<int>();
                            if (Aequus.LogMore)
                                Aequus.Instance.Logger.Info($"{mod.Name}: Setting Sentry On-AI function for {Lang.GetItemNameValue(item)}");
                            if (TryGet<Action<Projectile, Item, Player>>(out var simpleFunc))
                            {
                                SentryAccessoriesDatabase.OnAI.Add(item, (info) =>
                                    {
                                        simpleFunc(info.Projectile, info.Accessory, info.Player);
                                    });
                                return Success;
                            }
                            if (TryGet<Action<Projectile, GlobalProjectile, Item, Player>>(out var simpleFunc2))
                            {
                                SentryAccessoriesDatabase.OnAI.Add(item, (info) =>
                                    {
                                        simpleFunc2(info.Projectile, info.SentryAccessories, info.Accessory, info.Player);
                                    });
                                return Success;
                            }
                        }
                        return Failure;

                    case "SentryAccessoryOnShoot":
                        {
                            int item = Get<int>();
                            if (Aequus.LogMore)
                                Aequus.Instance.Logger.Info($"{mod.Name}: Setting Sentry On-Shoot function for {Lang.GetItemNameValue(item)}");
                            if (TryGet<Action<Projectile, Item, Player>>(out var simpleFunc))
                            {
                                SentryAccessoriesDatabase.OnShoot.Add(item, (info) =>
                                {
                                    simpleFunc(info.Projectile, info.Accessory, info.Player);
                                });
                                return Success;
                            }
                            if (TryGet<Action<IEntitySource, Projectile, Item, Player>>(out var simpleFunc2))
                            {
                                SentryAccessoriesDatabase.OnShoot.Add(item, (info) =>
                                {
                                    simpleFunc2(info.Source, info.Projectile, info.Accessory, info.Player);
                                });
                                return Success;
                            }
                            if (TryGet<Action<Projectile, Projectile, Item, Player>>(out var simpleFunc3))
                            {
                                SentryAccessoriesDatabase.OnShoot.Add(item, (info) =>
                                {
                                    simpleFunc3(info.Projectile, info.ParentProjectile, info.Accessory, info.Player);
                                });
                                return Success;
                            }
                            if (TryGet<Action<IEntitySource, Projectile, Projectile, Item, Player>>(out var simpleFunc4))
                            {
                                SentryAccessoriesDatabase.OnShoot.Add(item, (info) =>
                                {
                                    simpleFunc4(info.Source, info.Projectile, info.ParentProjectile, info.Accessory, info.Player);
                                });
                                return Success;
                            }
                        }
                        return Failure;

                    case "PylonColor":
                        {
                            var key = new Point(Helper.UnboxInt.Unbox(args[2]), args.Length > 4 ? Helper.UnboxInt.Unbox(args[3]) : 0);
                            if (args.Length >= 3)
                            {
                                int index = args.Length > 4 ? 4 : 3;
                                if (Aequus.LogMore)
                                    Aequus.Instance.Logger.Info($"{mod.Name}: Setting pylon color data for {TextHelper.GetInternalNameOrUnknown(key.X, TileID.Search)} ({key.X}): Style={key.Y}");
                                if (args[index] is Color color)
                                {
                                    AequusTile.PylonColors[key] = () => color;
                                }
                                else if (args[index] is Func<Color> getColor)
                                {
                                    AequusTile.PylonColors[key] = getColor;
                                }
                            }
                            return AequusTile.PylonColors[key];
                        }

                    case "NecromancyDatabase":
                        return NecromancyDatabase.CallAddNecromancyData(mod, args);
                    case "NecromancyNoAutogeneration":
                        return NecromancyDatabase.CallAddNecromancyModBlacklist(mod, args);

                    case "DemonSiegeSacrifice":
                        return DemonSiegeSystem.CallAddDemonSiegeData(mod, args);
                    case "DemonSiegeSacrificeHide":
                        return DemonSiegeSystem.CallHideDemonSiegeData(mod, args);
                }
            }
            catch
            {
            }
            return null;
        }

        internal static void TestModCalls()
        {
            var mod = Aequus.Instance;
            //mod.Call("SentryAccessory", mod, ItemID.CobaltShield, (Projectile proj, GlobalProjectile sentryAcc, Item item, Player player) =>
            //{
            //    Main.NewText("tester!?");
            //});
            //mod.Call("SentryAccessory", mod, ItemID.ObsidianShield, (Projectile proj, Item item, Player player) =>
            //{
            //    Main.NewText("tester 22!?");
            //});
            //mod.Call("SentryAccessoryOnShoot", mod, ItemID.AnkhShield, (Projectile proj, Item item, Player player) =>
            //{
            //    Main.NewText("tester 333!?");
            //});
            //mod.Call("SentryAccessoryOnShoot", mod, ItemID.AnkhCharm, (IEntitySource source, Projectile proj, Projectile parentProj, Item item, Player player) =>
            //{
            //    Main.NewText("tester 4444!?");
            //});
            //mod.Call("SentryAccessoryOnShoot", mod, ItemID.LavaCharm, (IEntitySource source, Projectile proj, Item item, Player player) =>
            //{
            //    Main.NewText($"{source.GetType().FullName}");
            //});
        }

        private static T GetterSetter<T>(ref T value)
        {
            if (TryGet<T>(out var value2))
            {
                value = value2;
            }
            return value;
        }

        private static NPC GetNPC()
        {
            if (TryGet(out int whoAmI))
            {
                return Main.npc[whoAmI];
            }
            return Get<NPC>();
        }

        private static Player GetPlayer()
        {
            if (TryGet(out int whoAmI))
            {
                return Main.player[whoAmI];
            }
            return Get<Player>();
        }

        private static bool TryGet<T>(out T value)
        {
            value = default(T);
            if (nextArg >= argument.Length)
            {
                return false;
            }
            if (TypeUnboxer<T>.Instance != null && TypeUnboxer<T>.Instance.TryUnbox(argument[nextArg], out var result))
            {
                nextArg++;
                value = result;
                return true;
            }
            if (argument[nextArg] is T result2)
            {
                nextArg++;
                value = result2;
                return true;
            }
            return false;
        }

        private static T Get<T>()
        {
            try
            {
                if (TypeUnboxer<T>.Instance != null && TypeUnboxer<T>.Instance.TryUnbox(argument[nextArg], out var result))
                {
                    nextArg++;
                    return result;
                }
                var result2 = (T)argument[nextArg];
                nextArg++;
                return result2;
            }
            catch (Exception ex)
            {
                string nextArgTextValue = nextArg >= argument.Length ? "Outside of Index, Cannot retrieve value." : argument[nextArg].ToString();
                string callerName = mod != null ? mod.Name : "Unknown";
                Aequus.Instance.Logger.Error($"Could not get 'arg{nextArg}' ({nextArgTextValue}) as '{typeof(T).FullName}'.\nCaller: {callerName}, Type: {type}\n{ex.Message}\n{ex.StackTrace}");
            }
            return default(T);
        }

        public void Load(Mod mod)
        {
        }

        public void Unload()
        {
            nextArg = 0;
            argument = null;
            mod = null;
            type = null;
        }
    }
}