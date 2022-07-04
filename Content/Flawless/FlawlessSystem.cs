using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.NPCs
{
    public class FlawlessSystem : ModSystem
    {
        public readonly List<byte> DamagedPlayers;

        public FlawlessSystem()
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