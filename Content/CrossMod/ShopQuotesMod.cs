using Aequus.Content.CursorDyes.Items;
using Aequus.Content.Town.CarpenterNPC;
using Aequus.Content.Town.ExporterNPC;
using Aequus.Content.Town.OccultistNPC;
using Aequus.Content.Town.PhysicistNPC;
using Aequus.Content.Town.SkyMerchantNPC;
using Aequus.Items.Accessories.Combat.Sentry.EquipmentChips;
using Aequus.Items.Vanity.Pets;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod {
    internal class ShopQuotesMod : ModSupport<ShopQuotesMod> {
        public const string Key = "Mods.Aequus.ShopQuote.";

        public override void SetStaticDefaults() {
            Instance.Call("SetDefaultKey", Mod, Key);

            ModContent.GetInstance<Exporter>().SetupShopQuotes(Instance);
            ModContent.GetInstance<Carpenter>().SetupShopQuotes(Instance);
            ModContent.GetInstance<Occultist>().SetupShopQuotes(Instance);
            ModContent.GetInstance<Physicist>().SetupShopQuotes(Instance);
            ModContent.GetInstance<SkyMerchant>().SetupShopQuotes(Instance);
            Instance.Call("SetQuote", NPCID.Mechanic, ModContent.ItemType<Sentry6502>(), GetKey("Mechanic.Sentry6502"));
            Instance.Call("SetQuote", NPCID.DyeTrader, ModContent.ItemType<DyableCursor>(), GetKey("DyeTrader.DyableCursor"));
            Instance.Call("SetQuote", NPCID.Clothier, ModContent.ItemType<FamiliarPickaxe>(), GetKey("Clothier.FamiliarPickaxe"));
        }

        public static string GetKey(string key) {
            return Key + key;
        }

        public static string GetTextValue(string key) {
            return Language.GetTextValue(GetKey(key));
        }

        public static LocalizedText GetText(string key) {
            return Language.GetText(GetKey(key));
        }
    }
}