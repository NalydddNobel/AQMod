using Terraria;

namespace AQMod.Common.UI
{
    public class StatLifeUI
    {
        private static bool shouldDrawAtAll()
        {
            if (!Main.gameMenu || Main.myPlayer == -1 || Main.player[Main.myPlayer].active || Main.player[Main.myPlayer].ghost)
            {
                return false;
            }
            return true;
        }
    }
}