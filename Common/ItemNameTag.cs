using Aequus.Common.IO;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common
{
    public class ItemNameTag : GlobalItem
    {
        public static HashSet<int> CannotBeRenamed { get; private set; }

        public override void Load()
        {
            CannotBeRenamed = new HashSet<int>();
        }

        public override void Unload()
        {
            CannotBeRenamed?.Clear();
            CannotBeRenamed = null;
        }

        public override bool InstancePerEntity => true;

        [SaveData("NameTag")]
        public string NameTag;
        [SaveData("RenameCount")]
        public int RenameCount;

        public bool HasNameTag => NameTag != null;

        public ItemNameTag()
        {
            NameTag = null;
            RenameCount = 0;
        }

        public override void SaveData(Item item, TagCompound tag)
        {
            SaveDataAttribute.SaveData(tag, this);
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            SaveDataAttribute.LoadData(tag, this);
        }

        public override void NetSend(Item item, BinaryWriter writer)
        {
            if (NameTag != null)
            {
                writer.Write(true);
                writer.Write(NameTag);
            }
            else
            {
                writer.Write(false);
            }
            if (RenameCount > 0)
            {
                writer.Write(true);
                writer.Write((ushort)RenameCount);
            }
            else
            {
                writer.Write(false);
            }
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            NameTag = null;
            RenameCount = 0;
            if (reader.ReadBoolean())
            {
                NameTag = reader.ReadString();
            }
            if (reader.ReadBoolean())
            {
                RenameCount = reader.ReadUInt16();
            }
        }

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            updateNameTag(item);
        }

        public override void UpdateInventory(Item item, Player player)
        {
            updateNameTag(item);
        }

        public override void UpdateEquip(Item item, Player player)
        {
            updateNameTag(item);
        }

        public override void UpdateVanity(Item item, Player player)
        {
            updateNameTag(item);
        }

        public override bool CanStack(Item item1, Item item2)
        {
            return CanStackLogic(item1, item2);
        }

        public override bool CanStackInWorld(Item item1, Item item2)
        {
            return CanStackLogic(item1, item2);
        }

        public bool CanStackLogic(Item item1, Item item2)
        {
            return !item2.TryGetGlobalItem<ItemNameTag>(out var nameTag2) || !item1.TryGetGlobalItem<ItemNameTag>(out var nameTag1)
                || nameTag1.NameTag == nameTag2.NameTag;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            updateNameTag(item);
            if (HasNameTag && NameTag != "")
            {
                foreach (var t in tooltips)
                {
                    if (t.Mod == "Terraria" && t.Name == "ItemName")
                    {
                        t.Text = NameTag;
                    }
                }
            }
        }

        public void updateNameTag(Item item)
        {
            if (HasNameTag)
            {
                if (NameTag == "")
                {
                    item.ClearNameOverride();
                }
                else
                {
                    item.SetNameOverride(NameTag);
                }
            }
            else
            {
                item.ClearNameOverride();
            }
        }
        public static void UpdateNameTag(Item item)
        {
            item.GetGlobalItem<ItemNameTag>().updateNameTag(item);
        }

        public int renamePrice(Item item)
        {
            return Item.buyPrice(gold: 1) * Math.Max(RenameCount, 1);
        }
        public static int RenamePrice(Item item)
        {
            return item.GetGlobalItem<ItemNameTag>().renamePrice(item);
        }

        public static bool CanRename(Item item)
        {
            return !item.IsACoin && item.ammo <= ItemID.None && !CannotBeRenamed.Contains(item.type);
        }
    }
}