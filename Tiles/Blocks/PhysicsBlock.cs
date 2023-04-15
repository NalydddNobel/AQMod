using Aequus.Content.Town.PhysicistNPC.Analysis;
using Aequus.Projectiles.Misc;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Blocks {
    public class PhysicsBlock : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 100;
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<PhysicsBlockTile>());
            Item.value = Item.buyPrice(silver: 2, copper: 50);
            Item.maxStack = 9999;
        }
    }

    public class PhysicsBlockTile : ModTile {
        public override void SetStaticDefaults() {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            AddMapEntry((Color.Blue * 0.1f).UseA(255));
            DustType = DustID.Ambient_DarkBrown;
            HitSound = SoundID.Tink;
            PhysicsGunProj.TilePickupBlacklist.Add(Type);
        }

        public override bool Slope(int i, int j) {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}