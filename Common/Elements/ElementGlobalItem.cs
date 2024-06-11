using System.Collections.Generic;

namespace Aequus.Common.Elements;

public class ElementGlobalItem : GlobalItem {
    private IEnumerable<Element> GetVisibleElements(Item item) {
#if DEBUG
        if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left)) {
            return ElementLoader.Elements;
        }
#endif

        if (item.damage > 0) {
            return Main.LocalPlayer.GetModPlayer<ElementalPlayer>().visibleElements;
        }

        return null;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
#if DEBUG
        if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down)) {
            string fullName = item.ModItem?.FullName ?? ItemID.Search.GetName(item.netID);
            tooltips.Add(new TooltipLine(Mod, "fullname", fullName));

            // Copy to clipboard
            if (!Main.oldKeyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down)) {
                dynamic clipboard = ReLogic.OS.Platform.Get<ReLogic.OS.IClipboard>();

                clipboard.Value = fullName;
            }
        }
#endif

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
                text += ", ";
            }

            text += ChatTagWriter.Color(Colors.AlphaDarken(e.Color), e.DisplayName.Value);
        }

        if (!string.IsNullOrEmpty(text)) {
            tooltips.Add(new TooltipLine(Mod, "Elements", $"Elements: {text}"));
        }
    }

    public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        if (!Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right)) {
            return;
        }

        IEnumerable<Element> elements = GetVisibleElements(item);
        if (elements == null) {
            return;
        }

        position += new Vector2(-24f, 16f) * Main.inventoryScale;
        foreach (Element e in elements) {
            if (!e.ContainsItem(item.type)) {
                continue;
            }

            spriteBatch.Draw(e.texture.Value, position, null, Color.White, 0f, Vector2.Zero, Main.inventoryScale, SpriteEffects.None, 0f);
            position.X += (e.texture.Value.Width + 2f) * Main.inventoryScale;
        }
    }
}
