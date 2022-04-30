using Aequus.Common.Catalogues;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.ItemModules
{
    public class GrapplingHookModules
    {
        public interface IHookBarbData : IItemModule
        {
            /// <summary>
            /// Ran in <see cref="GlobalProjectile.PreAI(Projectile)"/>
            /// </summary>
            /// <param name="projectile"></param>
            /// <param name="barbEquipType"></param>
            /// <param name="player"></param>
            /// <param name="aequus"></param>
            /// <returns></returns>
            bool ProjPreAI(Projectile projectile, int barbEquipType, Player player, AequusPlayer aequus)
            {
                return true;
            }
            /// <summary>
            /// Ran in <see cref="GlobalProjectile.AI(Projectile)"/>
            /// </summary>
            /// <param name="projectile"></param>
            /// <param name="barbEquipType"></param>
            /// <param name="player"></param>
            /// <param name="aequus"></param>
            /// <returns></returns>
            void ProjAI(Projectile projectile, int barbEquipType, Player player, AequusPlayer aequus)
            {
            }
            /// <summary>
            /// Ran in <see cref="GlobalProjectile.PostAI(Projectile)"/>
            /// </summary>
            /// <param name="projectile"></param>
            /// <param name="barbEquipType"></param>
            /// <param name="player"></param>
            /// <param name="aequus"></param>
            /// <returns></returns>
            void ProjPostAI(Projectile projectile, int barbEquipType, Player player, AequusPlayer aequus)
            {
            }
        }
        public interface IDamageBarbData : IHookBarbData
        {
            int GetBarbDamage();

            public static void FlagUseProjDamage(bool value)
            {
                ModuleLookupsProjectile.moduleAddProjDamage = value;
            }
            public static void AddToDamageBusVariableDamage(int npc, int damage, float luck = 0f)
            {
                AddToDamageBus(npc, Main.DamageVar(damage, luck));
            }
            public static void AddToDamageBus(int npc, int damage)
            {
                if (ModuleLookupsProjectile.modulesCombinedDamage.ContainsKey(npc))
                {
                    ModuleLookupsProjectile.modulesCombinedDamage[npc] += damage;
                }
                else
                {
                    ModuleLookupsProjectile.modulesCombinedDamage.Add(npc, damage);
                }
            }
        }
        public interface IDebuffBarbData : IHookBarbData
        {
            public List<(int, int, int)> DebuffsToApply { get; set; }
        }

        public abstract class NPCCollisionHookBarbData : IHookBarbData
        {
            public virtual List<int> ModuleTypes { get; set; }

            void IHookBarbData.ProjPostAI(Projectile projectile, int barbEquipType, Player player, AequusPlayer aequus)
            {
                ProjPostAI(projectile, barbEquipType, player, aequus);
                var myRect = projectile.getRect();
                ProjectileLoader.ModifyDamageHitbox(projectile, ref myRect);
                if (projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (!(Main.npc[i].active && !Main.npc[i].dontTakeDamage) || Main.npc[i].aiStyle == 112 && Main.npc[i].ai[2] > 1f)
                        {
                            continue;
                        }
                        bool? modCanHit = ProjectileLoader.CanHitNPC(projectile, Main.npc[i]);
                        if (modCanHit == false)
                        {
                            continue;
                        }
                        bool? modCanBeHit = NPCLoader.CanBeHitByProjectile(Main.npc[i], projectile);
                        if (modCanBeHit == false)
                        {
                            continue;
                        }
                        bool? modCanHit2 = PlayerLoader.CanHitNPCWithProj(projectile, Main.npc[i]);
                        if (modCanHit2 == false)
                        {
                            continue;
                        }
                        bool canHitFlag = modCanBeHit == true || modCanHit == true || modCanHit2 == true;
                        Main.npc[i].position += Main.npc[i].netOffset;

                        bool flag12 = !Main.npc[i].friendly;
                        flag12 |= projectile.type == 318;
                        flag12 |= Main.npc[i].type == NPCID.Guide && projectile.owner < 255 && Main.player[projectile.owner].killGuide;
                        flag12 |= Main.npc[i].type == NPCID.Clothier && projectile.owner < 255 && Main.player[projectile.owner].killClothier;
                        if (projectile.owner < 255 && !Main.player[projectile.owner].CanNPCBeHitByPlayerOrPlayerProjectile(Main.npc[i], projectile))
                        {
                            flag12 = false;
                        }
                        bool flag13 = Main.npc[i].friendly && !Main.npc[i].dontTakeDamageFromHostiles;
                        if (canHitFlag || projectile.friendly && flag12 || projectile.hostile && flag13)
                        {
                            if (projectile.owner < 0 || Main.npc[i].immune[projectile.owner] == 0)
                            {
                                if (!projectile.Colliding(myRect, Main.npc[i].getRect()))
                                {
                                    continue;
                                }
                                ProjOnCollideNPC(projectile, Main.npc[i]);
                            }
                        }
                        Main.npc[i].position -= Main.npc[i].netOffset;
                    }
                }
            }

            public virtual void ProjPostAI(Projectile projectile, int barbEquipType, Player player, AequusPlayer aequus)
            {
            }

            /// <summary>
            /// Checked in <see cref="GlobalProjectile.PostAI(Projectile)"/>
            /// </summary>
            /// <param name="projectile"></param>
            /// <param name="npc">The NPC that is colliding with this hook</param>
            public abstract void ProjOnCollideNPC(Projectile projectile, NPC npc);
        }
        public abstract class NPCPlayerPVPCollisionHookBarbData : NPCCollisionHookBarbData
        {
            /// <summary>
            /// Checked in <see cref="GlobalProjectile.PostAI(Projectile)"/>
            /// </summary>
            /// <param name="projectile"></param>
            /// <param name="player">The player that is colliding with this hook. Does not check if they are hostile and a part of an opposing team.</param>
            public abstract void ProjOnCollidePlayer(Projectile projectile, Player player);
        }
        public class DamageBarbData : NPCPlayerPVPCollisionHookBarbData, IDamageBarbData
        {
            public int damage;
            private int crit;
            private float kb;

            public DamageBarbData(int damage)
            {
                this.damage = damage;
                kb = 0.1f;
                crit = 0;
                ModuleTypes = new List<int>()
                {
                    ItemModuleTypeCatalogue.BarbHook,
                };
            }

            public int GetBarbDamage()
            {
                return damage;
            }

            public override void ProjOnCollideNPC(Projectile projectile, NPC npc)
            {
                IDamageBarbData.FlagUseProjDamage(value: true);
                IDamageBarbData.AddToDamageBusVariableDamage(npc.whoAmI, GetBarbDamage(), Main.player[projectile.owner].luck);
            }

            public override void ProjOnCollidePlayer(Projectile projectile, Player player)
            {
                if (Main.player[projectile.owner].hostile && player.hostile && player.team == Main.player[projectile.owner].team)
                {
                    int damage = Main.DamageVar(projectile.damage + this.damage, Main.player[projectile.owner].luck);
                    player.Hurt(PlayerDeathReason.ByOther(0), damage, Math.Sign(projectile.velocity.X), pvp: true);
                }
            }

            private static int CalcDamage(int damage, Projectile projectile, NPC npc)
            {
                if (Main.myPlayer == projectile.owner)
                {
                    int num19 = Item.NPCtoBanner(npc.BannerID());
                    if (num19 >= 0)
                    {
                        Main.player[Main.myPlayer].lastCreatureHit = num19;
                    }
                    if (Main.netMode != NetmodeID.Server)
                    {
                        int num20 = Item.NPCtoBanner(npc.BannerID());
                        if (num20 > 0 && Main.player[projectile.owner].HasNPCBannerBuff(num20))
                        {
                            damage = !Main.expertMode ? (int)(damage * ItemID.Sets.BannerStrength[Item.BannerToItem(num20)].NormalDamageDealt) : (int)(damage * ItemID.Sets.BannerStrength[Item.BannerToItem(num20)].ExpertDamageDealt);
                        }
                    }
                }
                return damage;
            }
        }
        public class DebuffBarbData : NPCPlayerPVPCollisionHookBarbData, IDebuffBarbData
        {
            public List<(int, int, int)> DebuffsToApply { get; set; }

            public DebuffBarbData(List<(int, int, int)> debuffs)
            {
                ModuleTypes = new List<int>()
                {
                    ItemModuleTypeCatalogue.BarbHook,
                };
                DebuffsToApply = debuffs;
            }
            public DebuffBarbData(int debuffToApply, int duration, int chance = 1) : this(new List<(int, int, int)>() { (debuffToApply, duration, chance) })
            {
            }

            public override void ProjOnCollideNPC(Projectile projectile, NPC npc)
            {
                foreach (var b in DebuffsToApply)
                {
                    if (b.Item3 <= 1 || Main.rand.NextBool(b.Item3))
                    {
                        npc.AddBuff(b.Item1, b.Item2);
                    }
                }
            }

            public override void ProjOnCollidePlayer(Projectile projectile, Player player)
            {
                if (Main.player[projectile.owner].hostile && player.hostile && player.team == Main.player[projectile.owner].team)
                {
                    foreach (var b in DebuffsToApply)
                    {
                        if (b.Item3 <= 1 || Main.rand.NextBool(b.Item3))
                        {
                            player.AddBuff(b.Item1, b.Item2);
                        }
                    }
                }
            }
        }
        public class DebuffDamageBarbData : NPCPlayerPVPCollisionHookBarbData, IDebuffBarbData, IDamageBarbData
        {
            public int damage;
            private int crit;
            private float kb;
            public List<(int, int, int)> DebuffsToApply { get; set; }

            public DebuffDamageBarbData(int damage, List<(int, int, int)> debuffs)
            {
                this.damage = damage;
                kb = 0.1f;
                crit = 0;
                ModuleTypes = new List<int>()
                {
                    ItemModuleTypeCatalogue.BarbHook,
                };
                DebuffsToApply = debuffs;
            }
            public DebuffDamageBarbData(int damage, int debuffToApply, int duration, int chance = 1) : this(damage, new List<(int, int, int)>() { (debuffToApply, duration, chance) })
            {
            }


            public int GetBarbDamage()
            {
                return damage;
            }

            public override void ProjOnCollideNPC(Projectile projectile, NPC npc)
            {
                foreach (var b in DebuffsToApply)
                {
                    if (b.Item3 <= 1 || Main.rand.NextBool(b.Item3))
                    {
                        npc.AddBuff(b.Item1, b.Item2);
                    }
                }
                IDamageBarbData.FlagUseProjDamage(value: true);
                IDamageBarbData.AddToDamageBusVariableDamage(npc.whoAmI, GetBarbDamage(), Main.player[projectile.owner].luck);
            }

            public override void ProjOnCollidePlayer(Projectile projectile, Player player)
            {
                if (Main.player[projectile.owner].hostile && player.hostile && player.team == Main.player[projectile.owner].team)
                {
                    foreach (var b in DebuffsToApply)
                    {
                        if (b.Item3 <= 1 || Main.rand.NextBool(b.Item3))
                        {
                            player.AddBuff(b.Item1, b.Item2);
                        }
                    }
                    int damage = Main.DamageVar(projectile.damage + GetBarbDamage(), Main.player[projectile.owner].luck);
                    player.Hurt(PlayerDeathReason.ByOther(0), damage, Math.Sign(projectile.velocity.X), pvp: true);
                }
            }
        }

        /// <summary>
        /// Registers a hook barb for a given item.
        /// </summary>
        /// <param name="item">Item ID</param>
        /// <param name="data">Data statically stored for this item type</param>
        /// <returns></returns>
        public static void RegisterHookBarb(int item, IHookBarbData data)
        {
            ModularItems.Catalogue.RegisterModule(item, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item">Item ID</param>
        /// <returns>The generic hook barb data for this item. If there is none, throws an error</returns>
        public static IHookBarbData GetHookBarb(int item)
        {
            return (IHookBarbData)ModularItems.Catalogue.GetModuleData(item);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item">Item ID</param>
        /// <returns>The specific hook barb data for this item. If there is none or it's an incompatible type, throws an error</returns>
        public static T GetHookBarb<T>(int item) where T : IHookBarbData
        {
            return (T)GetHookBarb(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item">Item ID</param>
        /// <param name="value">An output value. When this method returns true, this is the data for the hook barb, otherwise it is useless data</param>
        /// <returns>The generic hook barb data for this item</returns>
        public static bool TryGetHookBarb(int item, out IHookBarbData value)
        {
            if (ModularItems.Catalogue.TryGetModuleData(item, out var value2))
            {
                value = (IHookBarbData)value2;
                return true;
            }
            value = default(IHookBarbData);
            return false;
        }
    }
}