using Aequus.Common.ModPlayers;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs
{
    public class BrokenMapBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            var aequus = player.Aequus();
            aequus.invisibleOnMap = true;
            aequus.invisibleOnTeamUI = true;
            aequus.noTeamUI = true;
            aequus.noInfoAccessories = true;
            aequus.noMapPings = true;
            if (Main.myPlayer == player.whoAmI)
            {
                AequusPlayer.DisableMapLighting = true;
            }
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }

    public class BrokenMapBuffGlobalInfoDisplay : GlobalInfoDisplay
    {
        public override bool? Active(InfoDisplay currentDisplay)
        {
            if (Main.LocalPlayer.Aequus().noInfoAccessories)
                return false;
            return null;
        }
    }
}

namespace Aequus.Common.ModPlayers
{
    partial class AequusPlayer : ModPlayer
    {
        public static bool DisableMapLighting;

        public bool invisibleOnMap;
        public bool invisibleOnTeamUI;
        public bool noTeamUI;
        public bool noInfoAccessories;
        public bool noMapPings;

        public void Load_BrokenMap()
        {
            On.Terraria.Map.WorldMap.UpdateLighting += WorldMap_UpdateLighting;
            On.Terraria.Map.PingMapLayer.Draw += PingMapLayer_Draw;
            On.Terraria.GameContent.PlayerHeadDrawRenderTargetContent.DrawTheContent += PlayerHeadDrawRenderTargetContent_DrawTheContent;
            On.Terraria.GameContent.UI.LegacyMultiplayerClosePlayersOverlay.Draw += LegacyMultiplayerClosePlayersOverlay_Draw;
        }

        private static bool WorldMap_UpdateLighting(On.Terraria.Map.WorldMap.orig_UpdateLighting orig, Terraria.Map.WorldMap self, int x, int y, byte light)
        {
            if (DisableMapLighting)
            {
                light = 0;
            }
            return orig(self, x, y, light);
        }

        private static void LegacyMultiplayerClosePlayersOverlay_Draw(On.Terraria.GameContent.UI.LegacyMultiplayerClosePlayersOverlay.orig_Draw orig, Terraria.GameContent.UI.LegacyMultiplayerClosePlayersOverlay self)
        {
            if (Main.LocalPlayer.Aequus().noTeamUI)
            {
                return;
            }
            _playerQuickList.Clear();
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead && !Main.player[i].Aequus().invisibleOnTeamUI)
                {
                    Main.player[i].dead = true;
                    _playerQuickList.Add(Main.player[i]);
                }
            }
            orig(self);
            foreach (var player in _playerQuickList)
            {
                player.dead = false;
            }
            _playerQuickList.Clear();
        }

        private static void PlayerHeadDrawRenderTargetContent_DrawTheContent(On.Terraria.GameContent.PlayerHeadDrawRenderTargetContent.orig_DrawTheContent orig, Terraria.GameContent.PlayerHeadDrawRenderTargetContent self, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.Aequus().invisibleOnMap)
            {
                return;
            }
            orig(self, spriteBatch);
        }

        private static void PingMapLayer_Draw(On.Terraria.Map.PingMapLayer.orig_Draw orig, Terraria.Map.PingMapLayer self, ref Terraria.Map.MapOverlayDrawContext context, ref string text)
        {
            if (Main.LocalPlayer.Aequus().noMapPings)
            {
                return;
            }
            orig(self, ref context, ref text);
        }

        public void ResetEffects_BrokenMap()
        {
            invisibleOnMap = false;
            invisibleOnTeamUI = false;
            noTeamUI = false;
            noInfoAccessories = false;
            noMapPings = false;
            if (Main.myPlayer == Player.whoAmI)
            {
                DisableMapLighting = false;
            }
        }
    }
}