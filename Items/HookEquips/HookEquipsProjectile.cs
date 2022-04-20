using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.HookEquips
{
    public sealed class HookEquipsProjectile : GlobalProjectile
    {
        public int[] barbs;

        public override bool InstancePerEntity => true;

        public override void Load()
        {
            On.Terraria.Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float += OnSpawnCheckForBarb;
        }

        private int OnSpawnCheckForBarb(On.Terraria.Projectile.orig_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float orig, Terraria.DataStructures.IEntitySource spawnSource, float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner, float ai0, float ai1)
        {
            int p = orig(spawnSource, X, Y, SpeedX, SpeedY, Type, Damage, KnockBack, Owner, ai0, ai1);
            if (Main.projectile[p].aiStyle == ProjAIStyleID.Hook)
            {
                var grapplingHook = Main.player[Owner].GetModPlayer<AequusPlayer>().GrapplingHookForBarbs;
                if (grapplingHook != null && !grapplingHook.IsAir)
                {
                    var modules = grapplingHook.GetGlobalItem<ModularItemsManager>().moduleItems;
                    if (modules != null)
                    {
                        Main.projectile[p].GetGlobalProjectile<HookEquipsProjectile>().barbs =
                            Array.ConvertAll(modules, (i) => i.type);
                    }
                }
            }
            return p;
        }

        public override bool PreAI(Projectile projectile)
        {
            if (barbs != null)
            {
                var player = Main.player[projectile.owner];
                var aequus = player.GetModPlayer<AequusPlayer>();
                bool result = true;
                foreach (var i in barbs)
                {
                    result &= GrapplingHookModules.GetHookBarb(i).ProjPreAI(projectile, i, player, aequus);
                }
                return result;
            }
            return true;
        }
        public override void AI(Projectile projectile)
        {
            if (barbs != null)
            {
                var player = Main.player[projectile.owner];
                var aequus = player.GetModPlayer<AequusPlayer>();
                foreach (var i in barbs)
                {
                    GrapplingHookModules.GetHookBarb(i).ProjAI(projectile, i, player, aequus);
                }
            }
        }
        public override void PostAI(Projectile projectile)
        {
            if (barbs != null)
            {
                var player = Main.player[projectile.owner];
                var aequus = player.GetModPlayer<AequusPlayer>();
                foreach (var i in barbs)
                {
                    GrapplingHookModules.GetHookBarb(i).ProjPostAI(projectile, i, player, aequus);
                }
            }
        }
    }
}