using Aequus.Common.Items.Components;
using Aequus.Content.DataSets;
using Aequus.Content.Items.Tools.NameTag;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Renaming;

public class RenameItem : GlobalItem {
    public static int RenamePrice { get; set; } = Item.buyPrice(silver: 25);

    public bool HasCustomName => !string.IsNullOrEmpty(CustomName);

    public string CustomName { get; set; } = string.Empty;

    private bool _resetNameTag;

    public override bool InstancePerEntity => true;

    protected override bool CloneNewInstances => true;

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return CanRename(entity);
    }

    public string GetDecodedName() {
        return CustomName.Length == 0 ? CustomName : RenamingSystem.GetPlainDecodedText(CustomName);
    }

    public void UpdateCustomName(Item item) {
        if (HasCustomName) {
            if (_resetNameTag) {
                return;
            }

            _resetNameTag = true;

            if (CustomName == string.Empty) {
                item.ClearNameOverride();
            }
            else {
                item.SetNameOverride(GetDecodedName());
            }

            return;
        }

        if (_resetNameTag) {
            _resetNameTag = false;
            item.ClearNameOverride();
        }
    }

    public override GlobalItem Clone(Item from, Item to) {
        var renameItem = (RenameItem)base.Clone(from, to);
        renameItem.UpdateCustomName(to);
        return renameItem;
    }

    public override bool CanStack(Item destination, Item source) {
        return !destination.TryGetGlobalItem<RenameItem>(out var nameTag1) || !source.TryGetGlobalItem<RenameItem>(out var nameTag2) || nameTag1.CustomName == nameTag2.CustomName;
    }

    public override void PostUpdate(Item item) {
        UpdateCustomName(item);
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (HasCustomName && CustomName != string.Empty) {
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
        UpdateCustomName(item);
    }

    public override void SaveData(Item item, TagCompound tag) {
        if (!string.IsNullOrEmpty(CustomName)) {
            tag["CustomName"] = CustomName;
        }
    }

    public override void LoadData(Item item, TagCompound tag) {
        if (tag.TryGet("CustomName", out string customName)) {
            CustomName = customName;
        }
    }

    public override void NetSend(Item item, BinaryWriter writer) {
        writer.Write(CustomName);
    }

    public override void NetReceive(Item item, BinaryReader reader) {
        CustomName = reader.ReadString();
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