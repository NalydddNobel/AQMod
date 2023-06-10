using Aequus.NPCs.Town.PhysicistNPC.Analysis;
using Aequus.Tiles.Base;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Blocks.GravityBlocks {
    [LegacyName("ForceAntiGravityBlock")]
    public class AntiGravityBlock : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 100;
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<AntiGravityBlockTile>());
            Item.value = Item.buyPrice(silver: 2, copper: 50);
        }
    }

    [LegacyName("ForceAntiGravityBlockTile")]
    public class AntiGravityBlockTile : GravityBlockBase {
        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            Main.tileLighted[Type] = true;
            DustType = DustID.OrangeStainedGlass;
            GravityType = -1;
            Auras = new[] { AequusTextures.AntiGravityAura_0, AequusTextures.AntiGravityAura_1 };
            DustTexture = AequusTextures.AntiGravityDust;
            AddMapEntry(Color.Orange, TextHelper.GetItemName<AntiGravityBlock>());
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
            r = 1f;
            g = 0.5f;
            b = 0.1f;
        }
    }

    public class AncientAntiGravityBlock : ModItem {
        public override string Texture => AequusTextures.AncientAntiGravityBlock.Path;

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 100;
            AnalysisSystem.IgnoreItem.Add(Type);
            ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<AntiGravityBlock>()] = Type;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<AncientAntiGravityBlockTile>());
            Item.value = Item.buyPrice(silver: 2, copper: 50);
        }
    }
    public class AncientAntiGravityBlockTile : AntiGravityBlockTile {
        public override string Texture => AequusTextures.AncientAntiGravityBlockTile.Path;
    }
}