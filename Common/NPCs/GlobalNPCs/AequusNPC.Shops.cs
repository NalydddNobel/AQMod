using Aequus.Content.CrossMod.SplitSupport;
using Aequus.Content.CursorDyes.Items;
using Aequus.CrossMod.SplitSupport.ItemContent.Prints;
using Aequus.Items.Equipment.Accessories.Combat.OnHitAbility.BoneRing;
using Aequus.Items.Equipment.Accessories.Sentry.SentryChip;
using Aequus.Items.Equipment.PetsVanity.Familiar;
using Aequus.Items.Tools.FishingPoles;
using Aequus.Items.Weapons.Sentry.PhysicistSentry;
using Terraria.GameContent.Bestiary;

namespace Aequus;

public partial class AequusNPC {
    public override void ModifyShop(NPCShop shop) {
        ModifyShopInner(shop);
        switch (shop.NpcType) {
            case NPCID.Steampunker: {
                    shop.Add(ModContent.ItemType<SteampunkerFishingPole>(), Condition.MoonPhasesEven, Condition.NpcIsPresent(NPCID.Angler));
                    break;
                }

            case NPCID.Clothier: {
                    shop.InsertAfter(ItemID.FamiliarPants, ModContent.ItemType<FamiliarPickaxe>(), Condition.Hardmode);
                    break;
                }

            case NPCID.Mechanic: {
                    shop.Add<Sentry6502>(Condition.NotRemixWorld);
                    shop.Add<PhysicistSentry>(Condition.RemixWorld);
                    break;
                }

            case NPCID.DyeTrader: {
                    shop.Add<DyableCursor>();
                    break;
                }

            case NPCID.SkeletonMerchant: {
                    shop.Add<BoneRing>(Condition.DownedSkeletron);
                    break;
                }
        }
    }

    public override void ModifyActiveShop(NPC npc, string shopName, Item[] items) {
        for (int i = 0; i < items.Length; i++) {
            items[i]?.Refresh(onlyIfVariantChanged: true);
        }
    }

    #region Travelling Merchant
    private static void AddSplitPoster(int[] shop, ref int nextSlot) {
        var prints = ModContent.GetInstance<PrintsTile>().printInfo;

        var print = prints[Main.rand.Next(prints.Length)];
        var bestiaryEntry = Main.BestiaryDB.FindEntryByNPCID(print.npcID);
        if (bestiaryEntry == null) {
            return;
        }

        var unlockState = bestiaryEntry.UIInfoProvider.GetEntryUICollectionInfo().UnlockState;
        if (unlockState != BestiaryEntryUnlockState.NotKnownAtAll_0) {
            shop[nextSlot++] = print.posterItemID;
        }
    }

    public override void SetupTravelShop(int[] shop, ref int nextSlot) {
        if (Split.Instance == null) {
            AddSplitPoster(shop, ref nextSlot);
        }
    }
    #endregion
}