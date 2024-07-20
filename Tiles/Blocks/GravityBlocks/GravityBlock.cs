using Aequus.NPCs.Town.PhysicistNPC.Analysis;
using Aequus.Tiles.Base;
using Aequus.Tiles.Blocks.GravityBlocks.Ancient;

namespace Aequus.Tiles.Blocks.GravityBlocks;
[LegacyName("ForceGravityBlock")]
public class GravityBlock : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 100;
        AnalysisSystem.IgnoreItem.Add(Type);
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<GravityBlockTile>());
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(silver: 1);
    }
}

[LegacyName("ForceGravityBlockTile")]
public class GravityBlockTile : GravityBlockBase {
    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        Main.tileLighted[Type] = true;
        DustType = DustID.BlueCrystalShard;
        GravityType = 1;
        Auras = [AequusTextures.GravityAura_0, AequusTextures.GravityAura_1];
        DustTexture = AequusTextures.GravityDust;
        this.SetMerge<GravityBlockTile>();
        this.SetMerge<AntiGravityBlockTile>();
        this.SetMerge<AncientGravityBlockTile>();
        this.SetMerge<AncientAntiGravityBlockTile>();
        AddMapEntry(Color.Blue, TextHelper.GetItemName<GravityBlock>());
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        r = 0.1f;
        g = 0.5f;
        b = 1f;
    }
}