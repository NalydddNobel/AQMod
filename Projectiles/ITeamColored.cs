using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Projectiles
{
    public interface ITeamColored
    {
        Color TeamColor { get; protected set; }

        public static void UpdateTeamColor(Player player, ITeamColored colored)
        {
            colored.TeamColor = Main.teamColor[player.team];
        }
    }
}