using Terraria;
using Terraria.ID;

namespace AQMod.Content.Players
{
    internal static class PlayerUtils
    {
        /// <summary>
        /// Method taken from 1.4, remove when porting.
        /// </summary>
        /// <param name="plr"></param>
        /// <param name="targetX"></param>
        /// <param name="targetY"></param>
        /// <returns></returns>
        public static bool IsInTileInteractionRange(this Player plr, int targetX, int targetY)
        {
            if (plr.position.X / 16f - Player.tileRangeX <= targetX && (plr.position.X + plr.width) / 16f + Player.tileRangeX - 1f >= targetX && plr.position.Y / 16f - Player.tileRangeY <= targetY)
                return (plr.position.Y + plr.height) / 16f + Player.tileRangeY - 2f >= targetY;
            return false;
        }

        public static void UpdatePlayerVisualAccessories(Item item, Player player)
        {
            if (item.handOnSlot > 0)
                player.handon = item.handOnSlot;
            if (item.handOffSlot > 0)
                player.handoff = item.handOffSlot;
            if (item.backSlot > 0)
                player.back = item.backSlot;
            if (item.frontSlot > 0)
                player.front = item.frontSlot;
            if (item.shoeSlot > 0)
                player.shoe = item.shoeSlot;
            if (item.waistSlot > 0)
                player.waist = item.waistSlot;
            if (item.shieldSlot > 0)
                player.shield = item.shieldSlot;
            if (item.neckSlot > 0)
                player.neck = item.neckSlot;
            if (item.faceSlot > 0)
                player.face = item.faceSlot;
            if (item.balloonSlot > 0)
                player.balloon = item.balloonSlot;
            if (item.wingSlot > 0)
                player.wings = item.wingSlot;
        }

        public static void UpdatePlayerVisualAccessoriesDyes(Item item, Item dye, Player player)
        {
            if (item.handOnSlot > 0)
                player.cHandOn = dye.dye;
            if (item.handOffSlot > 0)
                player.cHandOff = dye.dye;
            if (item.backSlot > 0)
                player.cBack = dye.dye;
            if (item.frontSlot > 0)
                player.cFront = dye.dye;
            if (item.shoeSlot > 0)
                player.cShoe = dye.dye;
            if (item.waistSlot > 0)
                player.cWaist = dye.dye;
            if (item.shieldSlot > 0)
                player.cShield = dye.dye;
            if (item.neckSlot > 0)
                player.cNeck = dye.dye;
            if (item.faceSlot > 0)
                player.cFace = dye.dye;
            if (item.balloonSlot > 0)
                player.cBalloon = dye.dye;
            if (item.wingSlot > 0)
                player.cWings = dye.dye;
            if (item.type == ItemID.FlyingCarpet)
                player.cCarpet = dye.dye;
        }
    }
}
