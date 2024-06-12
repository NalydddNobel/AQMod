using Aequus.Core.Debugging.CheatCodes;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.Localization;

namespace Aequus.Common.Elements;

public class ElementGlobalItem : GlobalItem {
    private static IEnumerable<Element> GetVisibleElements(Item item) {
        if (CheatCode.IsActive<CheatCode.RevealAllElements>()) {
            return ElementLoader.Elements;
        }

        if (item.damage > 0) {
            return Main.LocalPlayer.GetModPlayer<ElementalPlayer>().visibleElements;
        }

        return null;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        IEnumerable<Element> elements = GetVisibleElements(item);
        if (elements == null) {
            return;
        }

        string text = "";
        foreach (Element e in elements) {
            if (!e.ContainsItem(item.type)) {
                continue;
            }

            if (!string.IsNullOrEmpty(text)) {
                text += "\n";
            }

            text += ChatTagWriter.Color(Colors.AlphaDarken(e.Color), Language.GetOrRegister("Mods.Aequus.Elements.ElementTip").Format(e.DisplayName.Value));
        }

        if (!string.IsNullOrEmpty(text)) {
            TooltipLine line = new TooltipLine(Mod, "Elements", text);
            tooltips.Insert(1, line);
            //tooltips.Add(line);
        }
    }

    public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        if (!CheatCode.IsActive<CheatCode.VisibleElements>()) {
            return;
        }

        IEnumerable<Element> elements = GetVisibleElements(item);
        if (elements == null) {
            return;
        }

        int index = 0;
        int iconsPerRow = 4;
        position += new Vector2(-24f, 16f) * Main.inventoryScale;
        foreach (Element e in elements) {
            if (!e.ContainsItem(item.type)) {
                continue;
            }

            e.Sprite.GetSpriteParams(out Texture2D elementTexture, out Rectangle elementFrame);
            int collum = index % iconsPerRow;
            Vector2 elementPosition = position + new Vector2(TextureAssets.InventoryBack.Value.Width / (float)iconsPerRow * collum, index / iconsPerRow * -10);
            Vector2 elementOrigin = new Vector2(elementFrame.Width / (float)iconsPerRow * collum, 0f);

            spriteBatch.Draw(elementTexture, elementPosition, elementFrame, Color.White, 0f, elementOrigin, Main.inventoryScale, SpriteEffects.None, 0f);

            index++;
        }
    }
}
