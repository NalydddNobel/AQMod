using Aequus.Common.Items;
using Aequus.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.DedicatedContent;

public class DedicatedFaelingItem : ModItem {
    public int dedicatedItemId;

    public override void SetStaticDefaults() {
        ContentSamples.CreativeResearchItemPersistentIdOverride[Type] = ItemID.Shimmerfly;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.Shimmerfly);
        Item.makeNPC = (short)ModContent.NPCType<DedicatedFaeling>();
    }

    public override void OnSpawn(IEntitySource source) {
        if (source is EntitySource_Caught caughtSource && caughtSource.Entity is NPC npc && npc.ModNPC is DedicatedFaeling dedicatedFaeling) {
            dedicatedItemId = dedicatedFaeling.dedicatedItemId;
        }
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        if (ItemLoader.GetItem(dedicatedItemId) is IDedicatedItem dedicatedItem) {
            tooltips.Insert(tooltips.GetIndex("OneDropLogo"), new(Mod, "DedicatedItem", Language.GetTextValue("Mods.Aequus.Items.CommonTooltips.DedicatedItem", dedicatedItem.DedicateeName)) { OverrideColor = dedicatedItem.TextColor, });
        }
    }

    public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset) {
        if (line.Name != "DedicatedItem") {
            return true;
        }

        DedicatedItemGlobalItem.DrawDedicatedTooltip(line);
        return false;
    }

    public override void PostDrawTooltipLine(DrawableTooltipLine line) {
        if (line.Mod == "Terraria" && line.Name == "ItemName") {
            DedicatedItemGlobalItem.DrawDedicatedItemName(line);
        }
    }

    private Color GetGlowColor(IDedicatedItem dedicatedItem) {
        return (dedicatedItem.FaelingColor * 2f) with { A = 220 };
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
        if (ItemLoader.GetItem(dedicatedItemId) is IDedicatedItem dedicatedItem) {
            var glowColor = GetGlowColor(dedicatedItem);
            spriteBatch.Draw(AequusTextures.DedicatedFaelingItem_Mask, position, frame, glowColor, 0f, origin, scale, SpriteEffects.None, 0f);

            ulong seed = (ulong)(Math.Abs(Main.LocalPlayer.name.GetHashCode()) + UISystem.CurrentItemSlot.Slot);
            var sparkleTexture = AequusTextures.Sparkles;
            var sparkleOrigin = new Vector2(6f, 5f);
            int size = (int)(TextureAssets.InventoryBack.Value.Width * 0.8f) / 4;
            for (int i = 0; i < 20; i++) {
                float uniqueTimer = (i * 0.1f + Main.GlobalTimeWrappedHourly * 0.3f) % 2f;
                var spriteEffects = Utils.RandomInt(ref seed, 2) == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                var sparkleFrame = sparkleTexture.Frame(verticalFrames: 5, frameY: Utils.RandomInt(ref seed, 5));
                var sparklePosition = position + new Vector2(Utils.RandomInt(ref seed, size), Utils.RandomInt(ref seed, size)).RotatedBy(Main.GlobalTimeWrappedHourly * 0.5f + Utils.RandomInt(ref seed, 100) / 100f * MathHelper.TwoPi);
                float sparkleScaleMultiplier = Utils.RandomInt(ref seed, 100) / 200f + 0.8f;
                if (uniqueTimer > 1f) {
                    continue;
                }

                sparkleFrame.Height -= 2;
                float sparkleScale = MathF.Sin(uniqueTimer * MathHelper.Pi);
                Main.spriteBatch.Draw(
                    sparkleTexture,
                    sparklePosition,
                    sparkleFrame,
                    Color.Lerp(Color.White, glowColor, 0.75f) with { A = 0 } * sparkleScale,
                    0f,
                    sparkleOrigin,
                    MathF.Pow(sparkleScale, 10f) * sparkleScaleMultiplier,
                    spriteEffects,
                    0f
                );
            }
        }
        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        var drawCoordinates = Item.Center - Main.screenPosition;
        Item.GetItemDrawData(out var frame);
        var origin = frame.Size() / 2f;
        spriteBatch.Draw(TextureAssets.Item[Type].Value, drawCoordinates, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
        if (ItemLoader.GetItem(dedicatedItemId) is IDedicatedItem dedicatedItem) {
            spriteBatch.Draw(AequusTextures.DedicatedFaelingItem_Mask, drawCoordinates, frame, GetGlowColor(dedicatedItem), rotation, origin, scale, SpriteEffects.None, 0f);
        }
        return false;
    }

    // TODO - Make the item sparkle when dropped
    public override void Update(ref float gravity, ref float maxFallSpeed) {
    }

    public override bool CanStack(Item source) {
        return dedicatedItemId == (source.ModItem as DedicatedFaelingItem).dedicatedItemId;
    }

    public override void SaveData(TagCompound tag) {
        if (ItemLoader.GetItem(dedicatedItemId) is not IDedicatedItem dedicatedItem) {
            return;
        }
        tag["DedicatedName"] = dedicatedItem.DedicateeName;
    }

    public override void LoadData(TagCompound tag) {
        if (tag.TryGet("DedicatedName", out string name)) {
            dedicatedItemId = IDedicatedItem.GetItemIdFromName(name);
        }
    }

    public override void NetSend(BinaryWriter writer) {
        writer.Write(dedicatedItemId);
    }

    public override void NetReceive(BinaryReader reader) {
        dedicatedItemId = reader.ReadInt32();
    }
}