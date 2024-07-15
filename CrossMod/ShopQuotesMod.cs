﻿using Aequus.Common.CrossMod;
using Aequus.Content.CursorDyes.Items;
using Aequus.Items.Equipment.Accessories.Sentry.SentryChip;
using Aequus.Items.Equipment.PetsVanity.Familiar;
using Aequus.NPCs.Town.CarpenterNPC;
using Aequus.NPCs.Town.ExporterNPC;
using Aequus.NPCs.Town.OccultistNPC;
using Aequus.NPCs.Town.PhysicistNPC;
using Terraria.Localization;

namespace Aequus.CrossMod;
internal class ShopQuotesMod : ModSupport<ShopQuotesMod> {
    public const string Key = "Mods.Aequus.ShopQuote.";

    public override void SetStaticDefaults() {
        Instance.Call("SetDefaultKey", Mod, Key);

        ModContent.GetInstance<Exporter>().SetupShopQuotes(Instance);
        ModContent.GetInstance<Carpenter>().SetupShopQuotes(Instance);
        ModContent.GetInstance<Occultist>().SetupShopQuotes(Instance);
        ModContent.GetInstance<Physicist>().SetupShopQuotes(Instance);
#if RELEASE
        ModContent.GetInstance<SkyMerchant>().SetupShopQuotes(Instance);
#endif
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

    public static void TryAddQuote(int npcId, int itemId, string key) {
        Instance?.Call("SetQuote", npcId, itemId, GetKey(key));
    }

    public static void TryAddQuote(int npcId, int itemId) {
        string npc = NPCID.Search.GetName(npcId).Replace("Aequus/", "");
        string item = NPCID.Search.GetName(itemId).Replace("Aequus/", "");
        TryAddQuote(npcId, itemId, npc + "." + item);
    }
}