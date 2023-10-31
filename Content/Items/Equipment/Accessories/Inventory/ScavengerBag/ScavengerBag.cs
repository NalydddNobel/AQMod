using Aequus.Common.Items;
using Aequus.Common.Items.Components;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Items.Equipment.Accessories.Inventory.ScavengerBag;

[AutoloadEquip(EquipType.Back)]
public class ScavengerBag : ModItem, IStorageItem {
    public static int SlotAmount = 10;

    public Item[] Inventory { get; set; }
    public LocalizedText StorageDisplayName => ModContent.GetInstance<ScavengerBagBackpackData>().displayName;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SlotAmount);

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemCommons.Rarity.PollutedOceanLoot;
        Item.value = ItemCommons.Price.PollutedOceanLoot;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().accScavengerBag = Item;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
    }

    public override ModItem Clone(Item newEntity) {
        var clone = base.Clone(newEntity) as ScavengerBag;
        if (Inventory != null) {
            clone.Inventory = new Item[Inventory.Length];
            (this as IStorageItem).Deposit(clone.Inventory);
        }
        return clone;
    }

    public override void NetSend(BinaryWriter writer) {
        if (Inventory == null) {
            writer.Write(false);
            return;
        }
        writer.Write(Inventory.Length);
        for (int i = 0; i < Inventory.Length; i++) {
            if (Inventory[i] != null && !Inventory[i].IsAir) {
                writer.Write(true);
                ItemIO.Send(Inventory[i], writer, writeStack: true, writeFavorite: true);
            }
            else {
                writer.Write(false);
            }
        }
    }

    public override void NetReceive(BinaryReader reader) {
        if (!reader.ReadBoolean()) {
            return;
        }
        int length = reader.ReadInt32();
        Inventory = new Item[length];
        for (int i = 0; i < length; i++) {
            if (!reader.ReadBoolean()) {
                continue;
            }
            Inventory[i] = ItemIO.Receive(reader, readStack: true, readFavorite: true);
        }
    }

    public override void SaveData(TagCompound tag) {
        if (Inventory != null) {
            for (int i = 0; i < Inventory.Length; i++) {
                if (Inventory[i] == null) {
                    Inventory[i] = new();
                }
            }
            tag["Inventory"] = Inventory;
        }
    }

    public override void LoadData(TagCompound tag) {
        if (tag.TryGet<Item[]>("Inventory", out var inventory)) {
            Inventory = inventory;
        }
    }
}