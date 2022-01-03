using AQMod.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content
{
    public class NameTagItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        public string nameTag = null;
        public int timesRenamed;

        public override void UpdateInventory(Item item, Player player)
        {
            UpdateName(item);
        }

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            UpdateName(item);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (HasNameTag())
            {
                if (nameTag == "")
                {
                    for (int i = 0; i < tooltips.Count; i++)
                    {
                        if (tooltips[i].mod == "Terraria" && tooltips[i].Name == "ItemName")
                        {
                            if (i < tooltips.Count - 1)
                            {
                                if (tooltips[i + 1].overrideColor == null)
                                {
                                    tooltips[i + 1].overrideColor = new Color(255, 255, 255, 255);
                                }
                            }
                            tooltips.RemoveAt(i);
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < tooltips.Count; i++)
                    {
                        if (tooltips[i].mod == "Terraria" && tooltips[i].Name == "ItemName")
                        {
                            tooltips[i].text = nameTag;
                            if (nameTag == "_jeb")
                            {
                                tooltips[i].overrideColor = Main.DiscoColor;
                            }
                            break;
                        }
                    }
                }
            }
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            return base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
        }

        public override bool NeedsSaving(Item item)
        {
            return HasNameTag();
        }

        public override TagCompound Save(Item item)
        {
            return new TagCompound()
            {
                ["nameTag"] = nameTag,
                ["timesRenamed"] = timesRenamed,
            };
        }

        public override void Load(Item item, TagCompound tag)
        {
            if (tag.ContainsKey("nameTag"))
            {
                nameTag = tag.GetString("nameTag");
                timesRenamed = tag.GetInt("timesRenamed");
                UpdateName(item);
            }
        }

        public override void NetSend(Item item, BinaryWriter writer)
        {
            if (nameTag != null)
            {
                writer.Write(true);
                writer.Write(nameTag);
            }
            else
            {
                writer.Write(false);
            }
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            if (reader.ReadBoolean())
                nameTag = reader.ReadString();
        }

        public bool HasNameTag()
        {
            return nameTag != null;
        }

        public void UpdateName(Item item)
        {
            if (HasNameTag())
            {
                if (nameTag == "")
                {
                    item.ClearNameOverride();
                }
                else
                {
                    item.SetNameOverride(nameTag);
                }
            }
            else
            {
                item.ClearNameOverride();
            }
        }

        public static string GetItemName(Item item)
        {
            if (AQGraphics.Data.CanUseAssets)
            {
                try
                {
                    var nameTagItem = item.GetGlobalItem<NameTagItem>();
                    if (nameTagItem.HasNameTag())
                    {
                        return nameTagItem.nameTag;
                    }
                }
                catch
                {

                }
            }
            return item.Name;
        }

        public static bool CanRenameItem(Item item)
        {
            if (item.stack == 0)
            {
                return true;
            }
            return false;
        }

        public static int RenamePrice(Item item)
        {
            var nameTagItem = item.GetGlobalItem<NameTagItem>();
            int basePrice = Item.buyPrice(gold: 1);
            if (nameTagItem.HasNameTag())
            {
                return basePrice * nameTagItem.timesRenamed;
            }
            return basePrice;
        }

        public static bool IsNameTooLong(string pendingName)
        {
            if (pendingName.Length > 64)
            {
                return true;
            }
            return false;
        }
    }
}