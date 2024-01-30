using Aequus.Common.Items.Components;
using Aequus.Content.DataSets;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Renaming;

public sealed class RenameItem : GlobalItem {
    public static System.Int32 RenamePrice { get; set; } = Item.buyPrice(silver: 25);

    public System.Boolean HasCustomName => !System.String.IsNullOrEmpty(CustomName);

    public System.String CustomName { get; set; } = System.String.Empty;

    private System.Boolean _resetNameTag;

    public override System.Boolean InstancePerEntity => true;

    protected override System.Boolean CloneNewInstances => true;

    public override System.Boolean AppliesToEntity(Item entity, System.Boolean lateInstantiation) {
        return CanRename(entity);
    }

    public System.String GetDecodedName() {
        return CustomName.Length == 0 ? CustomName : RenamingSystem.GetPlainDecodedText(CustomName);
    }

    public void UpdateCustomName(Item item) {
        if (HasCustomName) {
            if (_resetNameTag) {
                return;
            }

            _resetNameTag = true;

            if (CustomName == System.String.Empty) {
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
        renameItem._resetNameTag = false;
        renameItem.UpdateCustomName(to);
        return renameItem;
    }

    public override System.Boolean CanStack(Item destination, Item source) {
        return !destination.TryGetGlobalItem<RenameItem>(out var nameTag1) || !source.TryGetGlobalItem<RenameItem>(out var nameTag2) || nameTag1.CustomName == nameTag2.CustomName;
    }

    public override void PostUpdate(Item item) {
        if (item.timeSinceItemSpawned % 60 == 0) {
            _resetNameTag = false;
        }
        UpdateCustomName(item);
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (HasCustomName && CustomName != System.String.Empty) {
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
        if (!System.String.IsNullOrEmpty(CustomName)) {
            tag["CustomName"] = CustomName;
        }
    }

    public override void LoadData(Item item, TagCompound tag) {
        if (tag.TryGet("CustomName", out System.String customName)) {
            CustomName = customName;
        }
    }

    public override void NetSend(Item item, BinaryWriter writer) {
        writer.Write(CustomName);
    }

    public override void NetReceive(Item item, BinaryReader reader) {
        CustomName = reader.ReadString();
    }

    public static System.Int32 GetRenamePrice(Item item) {
        if (item.ModItem is ICustomNameTagPrice customNameTagPrice) {
            return customNameTagPrice.GetNameTagPrice();
        }
        return RenamePrice;
    }

    public static System.Boolean CanRename(Item item) {
        return !item.IsACoin && !ItemSets.CannotRename.Contains(item.type);
    }
}