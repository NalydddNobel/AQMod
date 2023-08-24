using Aequus;
using Aequus.Common.Buffs;
using Aequus.Common.Items;
using Aequus.Common.Items.EquipmentBooster;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Items.Equipment.Accessories.PotionCanteen {
    public class PotionCanteen : ModItem, ItemHooks.IHookPickupText {
        public int itemIDLookup;
        public int buffID;

        public bool HasBuff => buffID > 0;

        public static LocalizedText AltName { get; private set; }

        public override void SetStaticDefaults() {
            EquipBoostDatabase.Instance.SetNoEffect(Type);
            AltName = this.GetLocalization("DisplayNameAlt");
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<PotionCanteenEmpty>();
        }

        public void SetPotionDefaults() {
            Item.buffType = buffID;
            Item.rare = ItemRarityID.Orange;
            if (itemIDLookup > 0) {
                Item.rare += ContentSamples.ItemsByType[itemIDLookup].rare;
            }
            Item.rare = Math.Min(Item.rare, ItemRarityID.Purple);
            Item.Prefix(Item.prefix);
            Item.ClearNameOverride();
            if (!Main.dedServ && buffID > 0 && AltName != null) {
                Item.SetNameOverride(GetName(Item.AffixName()));
            }
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory(20, 20);
            Item.value = Item.buyPrice(gold: 10);
            Item.rare = ItemRarityID.Orange;
            SetPotionDefaults();
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            if (buffID > 0) {
                if (Main.myPlayer == player.whoAmI) {
                    AequusBuff.preventRightClick.Add(buffID);
                }
                player.AddBuff(buffID, 1, quiet: true);
            }
        }

        public string GetName(string originalName) {
            return originalName.Replace(Lang.GetItemNameValue(Type), AltName.Format(Lang.GetBuffName(buffID)));
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            if (buffID > 0) {
                foreach (var t in tooltips) {
                    if (t.Name == "ItemName" && buffID > 0) {
                        t.Text = GetName(t.Text);
                    }
                    if (t.Name == "Tooltip0") {
                        t.Text = Lang.GetBuffDescription(buffID);
                    }
                }
            }
        }

        private Rectangle GetLiquidFrame(Texture2D liquidTexture) {
            return liquidTexture.Frame(verticalFrames: 16, frameY: (int)Main.GameUpdateCount / 7 % 15);
        }

        private Color GetLiquidColor() {
            var clrs = ItemID.Sets.DrinkParticleColors[itemIDLookup];
            Color colorResult = Color.White;
            if (clrs != null && clrs.Length > 0) {
                //int minBrightness = 0;
                //for (int i = 0; i < clrs.Length; i++) {
                //    int colorBrightness = clrs[i].R + clrs[i].G + clrs[i].B;
                //    if (colorBrightness > minBrightness) {
                //        colorResult = clrs[i];
                //        minBrightness = colorBrightness;
                //    }
                //}
                colorResult = Helper.LerpBetween(clrs, Main.GlobalTimeWrappedHourly);
            }
            return colorResult * 1.1f;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            spriteBatch.Draw(AequusTextures.PotionCanteenEmpty, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            if (!HasBuff) {
                return true;
            }
            var liquidTexture = AequusTextures.PotionCanteen_Liquid.Value;
            var liquidFrame = GetLiquidFrame(liquidTexture);
            var liquidColor = GetLiquidColor();
            float a = drawColor.A > 0 ? drawColor.A / 255f : Main.inventoryBack.A / 255f;
            spriteBatch.Draw(liquidTexture, position, liquidFrame, liquidColor * a, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, liquidColor with { A = 255 }, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
            var texture = TextureAssets.Item[Type].Value;
            var position = Item.Center - Main.screenPosition;
            var origin = texture.Size() / 2f;
            Item.GetItemDrawData(out var frame);
            spriteBatch.Draw(AequusTextures.PotionCanteenEmpty, position, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
            if (HasBuff) {
                var liquidTexture = AequusTextures.PotionCanteen_Liquid.Value;
                var liquidFrame = GetLiquidFrame(liquidTexture);
                var liquidColor = GetLiquidColor();
                spriteBatch.Draw(liquidTexture, position, liquidFrame, liquidColor, rotation, origin, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture, position, frame, Helper.GetColor(Item.Center, liquidColor), rotation, origin, scale, SpriteEffects.None, 0f);
            }
            return false;
        }

        public override void SaveData(TagCompound tag) {
            if (!HasBuff) {
                return;
            }

            AequusBuff.SaveBuffID(tag, "Buff", buffID);
            AequusItem.SaveItemID(tag, "Item", itemIDLookup);
        }

        public override void LoadData(TagCompound tag) {
            buffID = AequusBuff.LoadBuffID(tag, "Buff");
            itemIDLookup = AequusItem.LoadItemID(tag, "Item");
            SetPotionDefaults();
        }

        public void OnPickupText(int index, PopupTextContext context, int stack, bool noStack, bool longText) {
            SetPotionDefaults();
            Main.popupText[index].name = GetName(Main.popupText[index].name);
        }
    }
}