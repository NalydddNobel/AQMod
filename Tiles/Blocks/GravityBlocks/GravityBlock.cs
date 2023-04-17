using Aequus.Content.Town.PhysicistNPC.Analysis;
using Aequus.Tiles.Base;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Blocks.GravityBlocks {
    [LegacyName("ForceGravityBlock")]
    public class GravityBlock : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 100;
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<GravityBlockTile>());
            Item.value = Item.buyPrice(silver: 2, copper: 50);
        }
    }

    [LegacyName("ForceGravityBlockTile")]
    public class GravityBlockTile : GravityBlockBase {
        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            Main.tileLighted[Type] = true;
            DustType = DustID.BlueCrystalShard;
            GravityType = 1;
            Auras = new[] { AequusTextures.GravityAura_0, AequusTextures.GravityAura_1 };
            DustTexture = AequusTextures.GravityDust;
            AddMapEntry(Color.Blue, TextHelper.GetItemName<GravityBlock>());
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
            r = 0.1f;
            g = 0.5f;
            b = 1f;
        }
    }

    public class AncientGravityBlock : ModItem {
        public override string Texture => AequusTextures.AncientGravityBlock.Path;

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 100;
            AnalysisSystem.IgnoreItem.Add(Type);
            ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<GravityBlock>()] = Type;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<AncientGravityBlockTile>());
            Item.value = Item.buyPrice(silver: 2, copper: 50);
        }
    }
    public class AncientGravityBlockTile : GravityBlockTile {
        public override string Texture => AequusTextures.AncientGravityBlock.Path;
    }
}