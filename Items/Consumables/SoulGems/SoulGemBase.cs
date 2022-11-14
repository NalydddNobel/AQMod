using Aequus.Common;
using Aequus.Items.Accessories;
using Aequus.Items.GlobalItems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Items.Consumables.SoulGems
{
    public abstract class SoulGemBase : ModItem
    {
        public int filled;

        public abstract int Tier { get; }

        protected override bool CloneNewInstances => true;

        public static List<SoulGemBase> RegisteredSoulGems { get; private set; }

        public override void Load()
        {
            if (RegisteredSoulGems == null)
                RegisteredSoulGems = new List<SoulGemBase>();
            RegisteredSoulGems.Add(this);
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            AequusItem.PrioritizeVoidBagPickup.Add(Type);
            AmmoBackpack.AmmoBlacklist.Add(Type);
        }

        public override void Unload()
        {
            RegisteredSoulGems?.Clear();
            RegisteredSoulGems = null;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.consumable = true;
            Item.maxStack = 9999;
            setFilledDefaults();
        }

        public void setFilledDefaults()
        {
            if (filled <= 0)
                return;

            if (Item.TryGetGlobalItem<ItemNameTag>(out var nameTag))
            {
                nameTag.nameTag2 = $"$Mods.Aequus.ItemName.{Name} ($Mods.Aequus.SoulGemTier.{filled}|)";
                nameTag.updateNameTag(Item);
            }
            switch (filled)
            {
                case 1:
                    Item.damage = 10;
                    break;
                case 2:
                    Item.damage = 25;
                    break;
                case 3:
                    Item.damage = 40;
                    break;
                case 4:
                    Item.damage = 60;
                    break;
            }
            SetFilledDefaults();
        }

        protected abstract void SetFilledDefaults();

        public virtual void OnFillSoulGem(Player player, EnemyKillInfo npc)
        {
            setFilledDefaults();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Lerp(lightColor, Color.White, 0.75f) * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 0.1f, 0.9f, 1f);
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (filled > 0)
            {
                for (int i = 0; i < filled; i++)
                {
                    float wave2 = ((float)Math.Sin(AequusHelpers.CalcProgress(filled, i) * MathHelper.TwoPi + Main.GlobalTimeWrappedHourly * 2.5f) + 1f) / 2f;

                    var itemFrame = frame;
                    itemFrame.Height /= filled;
                    itemFrame.Y = itemFrame.Height * i;
                    var itemPosition = position;
                    itemPosition.Y += itemFrame.Height * i * scale;
                    foreach (var v in AequusHelpers.CircularVector(4))
                    {
                        spriteBatch.Draw(TextureAssets.Item[Type].Value, itemPosition + v * 2f * scale, itemFrame, drawColor.UseA(0).UseA(0) * wave2 * (0.1f + filled * 0.05f), 0f, origin, scale, SpriteEffects.None, 0f);
                    }
                }
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveCritChance();
            tooltips.RemoveKnockback();
        }

        public override bool CanStack(Item item2)
        {
            return item2.ModItem<SoulGemBase>().filled == filled;
        }
        public override bool CanStackInWorld(Item item2)
        {
            return item2.ModItem<SoulGemBase>().filled == filled;
        }

        public override void SaveData(TagCompound tag)
        {
            if (filled > 0)
            {
                tag["SoulGemTier"] = filled;
            }
        }

        public override void LoadData(TagCompound tag)
        {
            filled = tag.Get<int>("SoulGemTier");
            setFilledDefaults();
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write((byte)filled);
        }

        public override void NetReceive(BinaryReader reader)
        {
            filled = reader.ReadByte();
            setFilledDefaults();
        }
    }
}