using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.HookEquips
{
    public sealed class HookEquipsProjectile : GlobalProjectile
    {
        internal static bool moduleAddProjDamage;
        internal static Dictionary<int, int> modulesCombinedDamage;
        public int[] moduleLookups;

        public override bool InstancePerEntity => true;

        public override void Load()
        {
            modulesCombinedDamage = new Dictionary<int, int>();
        }

        public override void Unload()
        {
            modulesCombinedDamage?.Clear();
            modulesCombinedDamage = null;
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source != null)
            {
                if (projectile.aiStyle == ProjAIStyleID.Hook)
                {
                    if (source is EntitySource_ItemUse_WithAmmo itemWithAmmoSource)
                    {
                        UpdateProjModuleLookups(projectile, itemWithAmmoSource.Item);
                    }
                    else if (source is EntitySource_ItemUse itemUseSource)
                    {
                        UpdateProjModuleLookups(projectile, itemUseSource.Item);
                    }
                }
            }
        }
        private void UpdateProjModuleLookups(Projectile projectile, Item item)
        {
            var grapplingHook = item;
            if (grapplingHook != null && !grapplingHook.IsAir)
            {
                var modules = grapplingHook.GetGlobalItem<ModularItems>().modules;
                if (modules != null)
                {
                    int[] barbs = new int[modules.dict.Count];
                    int i = 0;
                    foreach (var pair in modules.dict)
                    {
                        barbs[i] = pair.Value.type;
                        i++;
                    }
                    projectile.GetGlobalProjectile<HookEquipsProjectile>().moduleLookups = barbs;
                }
            }
        }

        public override bool PreAI(Projectile projectile)
        {
            if (moduleLookups != null)
            {
                var player = Main.player[projectile.owner];
                var aequus = player.GetModPlayer<AequusPlayer>();
                bool result = true;
                foreach (var i in moduleLookups)
                {
                    result &= GrapplingHookModules.GetHookBarb(i).ProjPreAI(projectile, i, player, aequus);
                }
                return result;
            }
            return true;
        }
        public override void AI(Projectile projectile)
        {
            if (moduleLookups != null)
            {
                var player = Main.player[projectile.owner];
                var aequus = player.GetModPlayer<AequusPlayer>();
                foreach (var i in moduleLookups)
                {
                    GrapplingHookModules.GetHookBarb(i).ProjAI(projectile, i, player, aequus);
                }
            }
        }
        public override void PostAI(Projectile projectile)
        {
            if (moduleLookups != null)
            {
                var player = Main.player[projectile.owner];
                var aequus = player.GetModPlayer<AequusPlayer>();
                foreach (var i in moduleLookups)
                {
                    GrapplingHookModules.GetHookBarb(i).ProjPostAI(projectile, i, player, aequus);
                }
                if (modulesCombinedDamage.Count > 0)
                {
                    if (Main.myPlayer == projectile.owner)
                    {
                        foreach (var pair in modulesCombinedDamage)
                        {
                            var npc = Main.npc[pair.Key];
                            Main.player[projectile.owner].ApplyDamageToNPC(npc, (moduleAddProjDamage ? projectile.damage : 0) + pair.Value, 0f, Math.Sign(projectile.velocity.X), crit: false);
                            npc.immune[projectile.owner] = 10;
                        }
                    }
                    modulesCombinedDamage.Clear();
                }
            }
        }
    }
}