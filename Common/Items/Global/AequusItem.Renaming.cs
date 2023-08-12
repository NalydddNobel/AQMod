using Aequus.Common.Items;
using Aequus.Common.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items {
    public partial class AequusItem : GlobalItem {
        public const char LanguageKeyChar = '$';

        public static HashSet<int> CannotBeRenamed { get; private set; }

        public void Load_Renaming() {
            CannotBeRenamed = new HashSet<int>();
        }
        public void Unload_Renaming() {
            CannotBeRenamed?.Clear();
            CannotBeRenamed = null;
        }

        public bool HasNameTag => NameTag != null;

        [SaveData("NameTag")]
        public string NameTag;
        [SaveData("RenameCount")]
        public int RenameCount;

        public string GetDecodedName() {
            if (!HasNameTag)
                return null;
            if (NameTag.Length == 0)
                return NameTag;
            return decodeName(NameTag);
        }
        public static string decodeName(string nameTag) {
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

        public bool NametagStackCheck(Item item1, Item item2) {
            return !item2.TryGetGlobalItem<AequusItem>(out var nameTag2) || !item1.TryGetGlobalItem<AequusItem>(out var nameTag1)
                || nameTag1.NameTag == nameTag2.NameTag;
        }

        public void Tooltip_NameTag(Item item, List<TooltipLine> tooltips) {
            CheckNameTag(item);
            if (HasNameTag && NameTag != "") {
                foreach (var t in tooltips) {
                    if (t.Mod == "Terraria" && t.Name == "ItemName") {
                        t.Text = GetDecodedName();
                    }
                }
            }
        }

        internal void CheckNameTag(Item item) {
            if (HasNameTag) {
                if (NameTag == "") {
                    item.ClearNameOverride();
                }
                else {
                    item.SetNameOverride(GetDecodedName()); // Hope that name overrides aren't important to some other mod lul
                }
            }
            else {
                item.ClearNameOverride();
            }
        }
        public static void UpdateNameTag(Item item) {
            item.GetGlobalItem<AequusItem>().CheckNameTag(item);
        }

        internal int CalculateDefaultRenamePrice(Item item) {
            return Item.buyPrice(gold: 1) * Math.Max(RenameCount, 1);
        }

        public static int GetRenamePrice(Item item) {
            if (item.TryGetGlobalItem<AequusItem>(out var aequusItem)) {
                if (item.ModItem is ItemHooks.ICustomNameTagPrice customNameTagPrice) {
                    return customNameTagPrice.GetNameTagPrice(aequusItem);
                }
                return aequusItem.CalculateDefaultRenamePrice(item);
            }
            return 0;
        }

        public static bool CanRename(Item item) {
            return !item.IsACoin && item.ammo <= ItemID.None && !CannotBeRenamed.Contains(item.type);
        }
    }
}