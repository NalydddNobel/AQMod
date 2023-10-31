using Aequus.Common.Items.Components;
using Aequus.Content.DataSets;
using Aequus.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus;

public partial class AequusItem : GlobalItem {
    public const char LanguageKeyChar = '$';

    public static int RenamePrice = Item.buyPrice(silver: 25);

    public bool HasNameTag => NameTag != null;

    [SaveData("NameTag")]
    public string NameTag;

    public string GetDecodedName() {
        return !HasNameTag ? null : NameTag.Length == 0 ? NameTag : DecodeName(NameTag);
    }
    public static string DecodeName(string nameTag) {
        string newName = "";
        for (int i = 0; i < nameTag.Length; i++) {
            if (nameTag[i] == LanguageKeyChar) {
                string keyText = "";
                int j = i + 1;
                for (; j < nameTag.Length; j++) {
                    if (nameTag[j] == '|') {
                        j++;
                        break;
                    }
                    if (nameTag[j] == ' ') {
                        break;
                    }
                    keyText += nameTag[j];
                }
                i = j - 1;
                newName += Language.GetText(keyText).FormatWith(Lang.CreateDialogSubstitutionObject());
            }
            else {
                newName += nameTag[i];
            }
        }
        return newName;
    }

    public static bool NametagStackCheck(Item item1, Item item2) {
        return !item2.TryGetGlobalItem<AequusItem>(out var nameTag2) || !item1.TryGetGlobalItem<AequusItem>(out var nameTag1) || nameTag1.NameTag == nameTag2.NameTag;
    }

    public void TooltipNametag(Item item, List<TooltipLine> tooltips) {
        if (HasNameTag && NameTag != "") {
            foreach (var t in tooltips) {
                if (t.Mod == "Terraria" && t.Name == "ItemName") {
                    t.Text = GetDecodedName();
                }
            }
            if (Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift)) {
                item.ClearNameOverride();
                tooltips.Insert(tooltips.GetIndex("ItemName", 1), new(Mod, "OriginalName", item.AffixName()) { OverrideColor = Color.Gray * 1.4f });
            }
        }
        CheckNameTag(item);
    }

    internal void CheckNameTag(Item item) {
        // Hope that name overrides aren't important to some other mod lul
        if (HasNameTag) {
            if (NameTag == "") {
                item.ClearNameOverride();
            }
            else {
                item.SetNameOverride(GetDecodedName());
            }
        }
        else {
            item.ClearNameOverride();
        }
    }

    public static int GetRenamePrice(Item item) {
        if (item.TryGetGlobalItem<AequusItem>(out var aequusItem) && item.ModItem is ICustomNameTagPrice customNameTagPrice) {
            return customNameTagPrice.GetNameTagPrice(aequusItem);
        }
        return RenamePrice;
    }

    public static bool CanRename(Item item) {
        return !item.IsACoin && !ItemSets.CannotRename.Contains(item.type);
    }
}