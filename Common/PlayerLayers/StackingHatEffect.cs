using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.PlayerLayers {
    public class StackingHatEffect : PlayerDrawLayer
    {
        public static HashSet<int> Blacklist { get; private set; }

        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.Head);
        }

        public override void Load()
        {
            Blacklist = new HashSet<int>()
            {
                ArmorIDs.Head.Merman,
                ArmorIDs.Head.Werewolf,
                ArmorIDs.Head.FishBowl,
                ArmorIDs.Head.GoldGoldfishBowl,
                ArmorIDs.Head.BadgersHat,
                ArmorIDs.Head.CapricornMask,
                ArmorIDs.Head.SpectreHood,
                ArmorIDs.Head.SpectreMask,
            };
        }

        public override void Unload()
        {
            Blacklist?.Clear();
            Blacklist = null;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.Aequus().stackingHat > 0)
            {
                int head = drawInfo.drawPlayer.head;
                drawInfo.drawPlayer.head = drawInfo.drawPlayer.Aequus().stackingHat;
                drawInfo.drawPlayer.Aequus().stackingHat = head;
                var drawCoords = (drawInfo.Position - Main.screenPosition + new Vector2(-drawInfo.drawPlayer.bodyFrame.Width / 2 + drawInfo.drawPlayer.width / 2, drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4 - 14 * drawInfo.drawPlayer.gravDir)).Floor() + drawInfo.drawPlayer.headPosition + drawInfo.headVect;
                drawInfo.DrawDataCache.Add(new DrawData(TextureAssets.ArmorHead[drawInfo.drawPlayer.head].Value, drawCoords, drawInfo.drawPlayer.bodyFrame, drawInfo.colorArmorHead,
                    drawInfo.drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0));
            }
        }
    }
    public class StackingHatEffectBackLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new BeforeParent(PlayerDrawLayers.Head);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.Aequus().stackingHat > 0)
            {
                int head = drawInfo.drawPlayer.head;
                drawInfo.drawPlayer.head = drawInfo.drawPlayer.Aequus().stackingHat;
                drawInfo.drawPlayer.Aequus().stackingHat = head;
            }
        }
    }
}