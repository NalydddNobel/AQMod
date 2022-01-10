using AQMod.Common.Graphics.SceneLayers;
using AQMod.Content.Players;
using AQMod.Content.World.Events.GlimmerEvent;
using AQMod.Effects;
using AQMod.Effects.GoreNest;
using System.Reflection;
using Terraria;
using Terraria.ID;

namespace AQMod.Common
{
    public static class Edits
    {
        internal static MethodInfo Main_UpdateTime_SpawnTownNPCs;

        internal static void LoadHooks()
        {
            Main_UpdateTime_SpawnTownNPCs = typeof(Main).GetMethod("UpdateTime_SpawnTownNPCs", BindingFlags.NonPublic | BindingFlags.Static);

            On.Terraria.Main.DrawNPCs += Main_DrawNPCs;
            On.Terraria.Main.DrawTiles += Main_DrawTiles;
            On.Terraria.Main.UpdateTime += Main_UpdateTime;

            On.Terraria.Player.DropTombstone += Player_DropTombstone;

            On.Terraria.NetMessage.BroadcastChatMessage += MessageBroadcast.NetMessage_BroadcastChatMessage;
            On.Terraria.Main.NewText_string_byte_byte_byte_bool += MessageBroadcast.Main_NewText_string_byte_byte_byte_bool;

            On.Terraria.Projectile.NewProjectile_float_float_float_float_int_int_float_int_float_float += Projectile_NewProjectile_float_float_float_float_int_int_float_int_float_float;
        }

        internal static void UnloadHooks() // I am pretty sure TModLoader automatically unloads hooks, so this will just be used in some other cases
        {
            Main_UpdateTime_SpawnTownNPCs = null;
            CustomRenderBehindTiles.DrawProjsCache = null;
        }

        private static void Main_UpdateTime(On.Terraria.Main.orig_UpdateTime orig)
        {
            AQSystem.UpdatingWorld = true;
            Main.dayRate += AQSystem.DayrateIncrease;
            if (Main.dayTime)
            {
                if (Main.time + Main.dayRate > Main.dayLength)
                {
                    AQSystem.CosmicanonActive = AQPlayer.IgnoreMoons();
                    AprilFoolsJoke.UpdateActive();
                    if (Main.netMode != NetmodeID.Server)
                    {
                        GlimmerEventSky.InitNight();
                    }
                }
                orig();
                AQSystem.CosmicanonActive = false;
            }
            else
            {
                if (Main.time + Main.dayRate > Main.nightLength)
                {
                    AQSystem.CosmicanonActive = AQPlayer.IgnoreMoons();
                }
                orig();
                if (WorldDefeats.TownNPCMoveAtNight && !Main.dayTime)
                {
                    Main_UpdateTime_SpawnTownNPCs.Invoke(null, null);
                }
                AQSystem.CosmicanonActive = false;
            }
            AQSystem.DayrateIncrease = 0;
            MessageBroadcast.PreventChat = false;
            MessageBroadcast.PreventChatOnce = false;
            AQSystem.UpdatingWorld = false;
        }

        private static void Player_DropTombstone(On.Terraria.Player.orig_DropTombstone orig, Player self, int coinsOwned, Terraria.Localization.NetworkText deathText, int hitDirection)
        {
            var tombstonesPlayer = self.GetModPlayer<TombstonesPlayer>();
            if (tombstonesPlayer.disableTombstones)
            {
                return;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && tombstonesPlayer.CreateTombstone(coinsOwned, deathText, hitDirection))
            {
                orig(self, coinsOwned, deathText, hitDirection);
            }
        }

        private static int Projectile_NewProjectile_float_float_float_float_int_int_float_int_float_float(On.Terraria.Projectile.orig_NewProjectile_float_float_float_float_int_int_float_int_float_float orig, float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner, float ai0, float ai1)
        {
            int originalValue = orig(X, Y, SpeedX, SpeedY, Type, Damage, KnockBack, Owner, ai0, ai1);
            var projectile = Main.projectile[originalValue];
            if (projectile.coldDamage || (projectile.friendly && projectile.owner != 255 && Main.player[projectile.owner].frostArmor && (projectile.melee || projectile.ranged)))
            {
                var aQProj = projectile.GetGlobalProjectile<AQProjectile>();
                aQProj.canHeat = false;
                aQProj.temperature = -15;
            }
            return originalValue;
        }

        private static void Main_DrawNPCs(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            if (behindTiles)
            {
                SceneLayersManager.DrawLayer(SceneLayering.BehindTiles_BehindNPCs);
            }
            else
            {
                GoreNestRenderer.RenderGoreNests();
                CustomRenderUltimateSword.RenderUltimateSword();
                CustomRenderTrapperChains.RenderTrapperChains();
                SceneLayersManager.DrawLayer(SceneLayering.BehindNPCs);
            }
            orig(self, behindTiles);
            if (behindTiles)
            {
                CustomRenderBehindTiles.Render();
                SceneLayersManager.DrawLayer(SceneLayering.BehindTiles_InfrontNPCs);
            }
            else
            {
                SceneLayersManager.DrawLayer(SceneLayering.InfrontNPCs);
            }
        }

        private static void Main_DrawTiles(On.Terraria.Main.orig_DrawTiles orig, Main self, bool solidOnly, int waterStyleOverride)
        {
            if (!solidOnly)
            {
                GoreNestRenderer.RefreshCoordinates();
            }
            orig(self, solidOnly, waterStyleOverride);
        }
    }
}