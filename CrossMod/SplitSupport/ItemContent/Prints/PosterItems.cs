using Aequus.CrossMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.CrossMod.SplitSupport.ItemContent.Prints {
    public abstract class BasePosterItem : ModItem {
        public abstract int PrintTileStyle { get; }

        public override void SetStaticDefaults() {
            ShopQuotesMod.TryAddQuote(NPCID.TravellingMerchant, Type, "TravellingMerchant.SplitModPrint");
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<PrintsTile>(), PrintTileStyle);
            Item.value = Terraria.Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.LightRed;
        }
    }

    public class PosterBreadOfCthulhu : BasePosterItem {
        public override int PrintTileStyle => PrintsTile.BreadOfCthulhu;
    }

    public class PosterBloodMimic : BasePosterItem {
        public override int PrintTileStyle => PrintsTile.BloodMimic;
    }

    public class PosterUltraStarite : BasePosterItem {
        public override int PrintTileStyle => PrintsTile.UltraStarite;
    }

    public class PosterHeckto : BasePosterItem {
        public override int PrintTileStyle => PrintsTile.Heckto;
    }

    public class PosterOblivision : BasePosterItem {
        public override int PrintTileStyle => PrintsTile.Oblivision;
    }

    public class PosterSkyMerchant : BasePosterItem {
        public override int PrintTileStyle => PrintsTile.SkyMerchant;
    }
}
