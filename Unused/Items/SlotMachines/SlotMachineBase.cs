using Aequus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items.SlotMachines {
    [Obsolete("Slot Machines were removed.")]
    public abstract class SlotMachineBase : ModItem {
        protected virtual List<int> GetLootTable() {
            var dropAttemptInfo = new DropAttemptInfo() {
                IsExpertMode = Main.expertMode,
                IsMasterMode = Main.masterMode,
                IsInSimulation = false,
                item = -1,
                npc = null,
                player = Main.LocalPlayer,
            };
            var l = new List<int>();
            foreach (var r in Main.ItemDropsDB.GetRulesForItemID(Type)
                .Where((r) => r is SlotMachineDropRule && r.CanDrop(dropAttemptInfo))) {
                var rule = r as SlotMachineDropRule;
                if (rule.itemId == rule.rouletteChoice) {
                    l.Add((r as SlotMachineDropRule).itemId);
                }
            }
            return l;
        }

        public virtual int PickTableIndex(float time, int total) {
            return (int)(time % total);
        }

        public virtual float DefaultTime() {
            return SlotMachineSystem.Time * 2f;
        }

        public int GetItem() {
            return GetLootTable()[PickTableIndex(DefaultTime(), GetLootTable().Count)];
        }
        public virtual int GetItem(float time) {
            return GetLootTable()[PickTableIndex(time, GetLootTable().Count)];
        }

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 10;
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.WoodenCrate);
            Item.rare = ItemRarityID.Gray;
            Item.createTile = -1;
            Item.placeStyle = 0;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(gold: 5);
        }

        public override bool CanRightClick() {
            return true;
        }

        public override void RightClick(Player player) {
            SoundEngine.PlaySound(Aequus.GetSound("Item/slotMachine", 0.3f, 0.05f, 0.075f));
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            if (Item.buy)
                return;
            for (int i = 0; i < 3; i++) {
                tooltips.Add(new TooltipLine(Mod, "None" + i, Helper.AirString));
            }
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset) {
            if (line.Mod == "Aequus" && line.Name.StartsWith("None")) {
                return false;
            }
            return true;
        }

        public override void PostDrawTooltip(ReadOnlyCollection<DrawableTooltipLine> lines) {
            if (Item.buy)
                return;
            float longest = 30f;
            foreach (var l in lines) {
                float length = FontAssets.MouseText.Value.MeasureString(l.Text).X;
                if (length > longest) {
                    longest = length;
                }
            }

            float x = lines[0].X + longest / 2f;
            float y = 20f;

            foreach (var l in lines) {
                if (l.Mod == "Aequus" && l.Name == "None0") {
                    y += l.Y;
                }
            }

            //BatcherMethods.UI.Begin(Main.spriteBatch, BatcherMethods.Regular);

            float oldScale = Main.inventoryScale;
            Main.inventoryScale = 1f;

            var back = TextureAssets.InventoryBack.Value;
            x -= back.Width * Main.inventoryScale / 2f;
            //Main.spriteBatch.Draw(back, new Vector2(x, y), null, new Color(255, 255, 255, 255), 0f, new Vector2(0f, 0f), Main.inventoryScale, SpriteEffects.None, 0f);
            int backHeight = back.Height + 40;
            foreach (var c in Helper.CircularVector(4)) {
                Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)(x + c.X * 2f), (int)(y - 20f + c.Y * 2f), back.Width, backHeight), Color.Black);
            }
            Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)x, (int)y, back.Width, back.Height), new Color(100, 100, 151, 255) * 2f * 0.35f);
            Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)x, (int)y - 20, back.Width, backHeight), new Color(100, 100, 151, 255) * 2f * 0.485f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin_UI(immediate: false, useScissorRectangle: false);
            Main.graphics.GraphicsDevice.ScissorRectangle = new Rectangle((int)(x * Main.UIScale), (int)((y - 16) * Main.UIScale), (int)(back.Width * Main.UIScale), (int)((back.Height + 32) * Main.UIScale));
            Main.graphics.GraphicsDevice.RasterizerState.ScissorTestEnable = true;

            float rollY = DefaultTime() % 1f;
            float power = 2f;
            if (rollY < 0.5f) {
                rollY = 1f - (float)Math.Pow(1f - rollY / 0.5f, power) * 0.5f - 0.5f;
            }
            else {
                rollY = (float)Math.Pow(1f - rollY / 0.5f, power) * 0.5f + 0.5f;
            }


            for (int i = -1; i <= 1; i++) {
                float opacity = 1f;
                if (i == 1) {
                    opacity *= (rollY - 0.5f) * 2f;
                }
                if (i == -1) {
                    opacity *= 1f - rollY * 2f;
                }
                int item = GetItem(DefaultTime() + i);
                Main.instance.LoadItem(item);
                var itemTexture = TextureAssets.Item[item].Value;
                var itemScale = 1f;
                int max = itemTexture.Width > itemTexture.Height ? itemTexture.Width : itemTexture.Height;
                if (max > 32) {
                    itemScale = 32f / max;
                }
                var drawCoords = new Vector2(x, y + (rollY - i - 0.5f) * back.Height) + back.Size() / 2f;
                var itemOrigin = itemTexture.Size() / 2f;
                Main.spriteBatch.Draw(AequusTextures.Bloom0, drawCoords, null, Color.Black * opacity * 0.33f, 0f, AequusTextures.Bloom0.Size() / 2f, Main.inventoryScale * 0.45f, SpriteEffects.None, 0f);

                Main.spriteBatch.Draw(itemTexture, drawCoords + new Vector2(2f), null, Color.Black * opacity * 0.33f, 0f, itemOrigin, Main.inventoryScale * itemScale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(itemTexture, drawCoords, null, Color.White, 0f, itemOrigin, Main.inventoryScale * itemScale, SpriteEffects.None, 0f);
                if (i == 0) {
                    Main.spriteBatch.Draw(itemTexture, drawCoords, null, Color.White.UseA(0) * (float)Math.Pow(0.5f * (float)Math.Sin(rollY * MathHelper.Pi), 2), 0f, itemTexture.Size() / 2f, Main.inventoryScale * itemScale, SpriteEffects.None, 0f);
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin_UI(immediate: false);
            Main.graphics.GraphicsDevice.RasterizerState.ScissorTestEnable = false;

            var arrowTexture = TextureAssets.LockOnCursor.Value;
            var arrowDrawY = y + back.Height / 2f;
            var arrowFrame = arrowTexture.Frame(verticalFrames: 2, frameY: 0);
            var arrowOrigin = arrowFrame.Size() / 2f;
            x += back.Width / 2;
            float arrowDrawXWave = (float)Math.Sin(rollY * MathHelper.Pi) * 6f;
            if (SlotMachineSystem.TimeSpeed > 0f) {
                arrowDrawXWave = MathHelper.Lerp(arrowDrawXWave, -3f, Math.Min(SlotMachineSystem.TimeSpeed, 1f));
            }
            arrowDrawXWave = 6 - arrowDrawXWave;
            for (int i = -1; i <= 1; i++) {
                if (i == 0)
                    continue;
                var drawCoords = new Vector2(x + (arrowDrawXWave + back.Width / 2 + arrowFrame.Height / 2f + 4) * i, arrowDrawY);
                foreach (var c in Helper.CircularVector(4)) {
                    Main.spriteBatch.Draw(arrowTexture, drawCoords + c * 2f, arrowFrame, Color.Black, MathHelper.PiOver2 * i, arrowOrigin, Main.inventoryScale, SpriteEffects.None, 0f);
                }
                Main.spriteBatch.Draw(arrowTexture, drawCoords, arrowFrame, Color.White, MathHelper.PiOver2 * i, arrowOrigin, Main.inventoryScale, SpriteEffects.None, 0f);
            }

            Main.inventoryScale = oldScale;
        }

        protected void ModifyItemLoot_AddCommonDrops(ItemLoot loot, List<int> potionSelection = null) {
            loot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(2, (potionSelection ?? SlotMachineSystem.DefaultPotions).ToArray()));
        }

        protected void Split_PoolArmorPolish(Player player, int dropChance = 2) {
        }
    }
}