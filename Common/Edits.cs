using AQMod.Common.Utilities;
using AQMod.Content.CursorDyes;
using AQMod.Content.Players;
using AQMod.Effects;
using System.Reflection;
using Terraria;

namespace AQMod.Common
{
    public static class Edits
    {
        internal static void LoadHooks()
        {
            TimeActions.Hooks.Main_UpdateTime_SpawnTownNPCs = typeof(Main).GetMethod("UpdateTime_SpawnTownNPCs", BindingFlags.NonPublic | BindingFlags.Static);
            On.Terraria.Main.UpdateTime += TimeActions.Hooks.Main_UpdateTime;

            On.Terraria.Main.DrawNPCs += GameWorldRenders.Hooks.Main_DrawNPCs;
            On.Terraria.Main.DrawTiles += GameWorldRenders.Hooks.Main_DrawTiles;
            On.Terraria.Main.UpdateDisplaySettings += GameWorldRenders.Hooks.Main_UpdateDisplaySettings;

            On.Terraria.Main.CursorColor += CursorDyeManager.Hooks.Main_CursorColor;
            On.Terraria.Main.DrawCursor += CursorDyeManager.Hooks.Main_DrawCursor;
            On.Terraria.Main.DrawThickCursor += CursorDyeManager.Hooks.Main_DrawThickCursor;
            On.Terraria.Main.DrawInterface_36_Cursor += CursorDyeManager.Hooks.Main_DrawInterface_36_Cursor;

            On.Terraria.Player.DropTombstone += TombstonesPlayer.Hooks.Player_DropTombstone;

            On.Terraria.NetMessage.BroadcastChatMessage += MessageBroadcast.Hooks.NetMessage_BroadcastChatMessage;
            On.Terraria.Main.NewText_string_byte_byte_byte_bool += MessageBroadcast.Hooks.Main_NewText_string_byte_byte_byte_bool;

            On.Terraria.Projectile.NewProjectile_float_float_float_float_int_int_float_int_float_float += Projectile_NewProjectile_float_float_float_float_int_int_float_int_float_float;

            On.Terraria.UI.ItemSlot.MouseHover_ItemArray_int_int += PlayerStorage.Hooks.ItemSlot_MouseHover_ItemArray_int_int;
        }

        internal static void UnloadHooks() // I am pretty sure TModLoader automatically unloads hooks, so this will just be used in some other cases
        {
            On.Terraria.Main.UpdateTime -= TimeActions.Hooks.Main_UpdateTime;

            On.Terraria.Main.DrawNPCs -= GameWorldRenders.Hooks.Main_DrawNPCs;
            On.Terraria.Main.DrawTiles -= GameWorldRenders.Hooks.Main_DrawTiles;
            On.Terraria.Main.UpdateDisplaySettings -= GameWorldRenders.Hooks.Main_UpdateDisplaySettings;

            On.Terraria.Main.CursorColor -= CursorDyeManager.Hooks.Main_CursorColor;
            On.Terraria.Main.DrawCursor -= CursorDyeManager.Hooks.Main_DrawCursor;
            On.Terraria.Main.DrawThickCursor -= CursorDyeManager.Hooks.Main_DrawThickCursor;
            On.Terraria.Main.DrawInterface_36_Cursor -= CursorDyeManager.Hooks.Main_DrawInterface_36_Cursor;

            On.Terraria.Player.DropTombstone += TombstonesPlayer.Hooks.Player_DropTombstone;

            On.Terraria.Projectile.NewProjectile_float_float_float_float_int_int_float_int_float_float -= Projectile_NewProjectile_float_float_float_float_int_int_float_int_float_float;

            On.Terraria.NetMessage.BroadcastChatMessage -= MessageBroadcast.Hooks.NetMessage_BroadcastChatMessage;
            On.Terraria.Main.NewText_string_byte_byte_byte_bool -= MessageBroadcast.Hooks.Main_NewText_string_byte_byte_byte_bool;

            TimeActions.Hooks.Main_UpdateTime_SpawnTownNPCs = null;
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
    }
}