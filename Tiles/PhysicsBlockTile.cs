using Aequus.Items.Placeable;
using Aequus.Projectiles.Misc;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles
{
    public class PhysicsBlockTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            AddMapEntry((Color.Blue * 0.1f).UseA(255));
            DustType = DustID.Ambient_DarkBrown;
            ItemDrop = ModContent.ItemType<PhysicsBlock>();
            PhysicsGunProj.TilePickupBlacklist.Add(Type);
        }

        public override bool Slope(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}