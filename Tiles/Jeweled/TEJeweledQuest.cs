using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Tiles.Jeweled
{
    public class TEJeweledQuest : ModTileEntity
    {
        public static HashSet<int> QuestTiles { get; private set; }

        public override void Load(Mod mod)
        {
            QuestTiles = new HashSet<int>();
        }

        public override void Unload()
        {
            QuestTiles?.Clear();
            QuestTiles = null;
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            return Main.tile[x, y].HasTile && QuestTiles.Contains(Main.tile[x, y].TileType);
        }
    }
}