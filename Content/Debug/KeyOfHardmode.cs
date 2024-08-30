using Aequus.Common.Items;
using Aequus.Content.Chests;

namespace Aequus.Content.Debug;
public class KeyOfHardmode : ModItem, ItemHooks.ICheckBigMimicSummon {
    public override string Texture => AequusTextures.SkeletonKey.FullPath;

    public override bool IsLoadingEnabled(Mod mod) {
        return Aequus.DevelopmentFeatures;
    }

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 0;
    }

    public override void SetDefaults() {
        Item.width = 20;
        Item.height = 20;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.Red;
        Item.color = Main.OurFavoriteColor;
    }

    bool ItemHooks.ICheckBigMimicSummon.Choose(int x, int y, int chest, int currentItemCount, Player user) {
        return true;
    }

    bool ItemHooks.ICheckBigMimicSummon.DestroyChest(int x, int y, int chest, ushort tileID, int tileStyle, int itemCount, Player user) {
        return false;
    }

    void ItemHooks.ICheckBigMimicSummon.OnActivate(int x, int y, ushort tileID, int tileStyle, int itemCount, Player user) {
        Item.stack--;
        if (Item.stack <= 0) {
            Item.TurnToAir();
        }
        if (Main.netMode != NetmodeID.Server) {
            Vector2 center = new(x * 16f + 16f, y * 16f + 16f);
            for (int i = 0; i < 24; i++) {
                var d = Dust.NewDustDirect(new(x * 16f, y * 16f), 32, 32, DustID.TreasureSparkle, newColor: Color.Yellow with { A = 0 });
                d.velocity = (d.position - center) / 16f;
                d.fadeIn = d.scale + 0.4f;
            }
        }

        if (Main.netMode == NetmodeID.MultiplayerClient) {
            return;
        }

        int chestID = Chest.FindChestByGuessing(x, y);
        if (chestID < 0) {
            return;
        }

        ChestUpgradeSystem.Instance.Upgrade(Main.chest[chestID], chestID);
        if (Main.netMode != NetmodeID.SinglePlayer) {
            Helper.ChestConversionNetUpdate(chestID, tileID, x, y);
        }
    }
}