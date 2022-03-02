using Terraria;

namespace AQMod.Items
{
    public interface ICooldown
    {
        ushort Cooldown(Player player, AQPlayer aQPlayer);
    }
}