using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common
{
    public sealed class FlawlessManager : ModSystem
    {
        public readonly List<byte> DamagedPlayers;

        public FlawlessManager()
        {
            DamagedPlayers = new List<byte>();
        }

        public override void PostUpdatePlayers()
        {
            DamagedPlayers.Clear();
            for (byte i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && Main.player[i].statLife < Main.player[i].statLifeMax2)
                {
                    DamagedPlayers.Add(i);
                }
            }
        }
    }
}