using AequusRemake.Core.GUI;
using AequusRemake.Core.GUI.Elements;
using AequusRemake.Systems.Renaming;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;

namespace AequusRemake.Content.TownNPCs.SkyMerchant.UI;

public class SkyMerchantRenameUIState : UIState, ILoad {
    public ImprovedItemSlot SendItem { get; private set; }
    public ImprovedItemSlot ReceiveItem { get; private set; }
    public RenameTextBox TextBox { get; private set; }

    public override void OnInitialize() {
        Left.Set(InventoryUI.LeftInventoryPosition, 0f);
        Top.Set(InventoryUI.BottomInventoryY + 12, 0f);
        Width.Set(474, 0f);
        Height.Set(120, 0f);

        Color backgroundColor = new Color(48, 110, 66) * 0.9f;
        Color iconColor = Color.White * 0.35f;
        float itemSlotScale = 0.9f;

        TextBox = new RenameTextBox("", 0.9f) {
            DrawPanel = true
        };
        TextBox.OnTextChanged += (oldText, newText) => {
            if (SendItem.Item == null || SendItem.Item.IsAir) {
                return;
            }

            ReceiveItem.Item = SendItem.Item.Clone();
            RenameItem renameItem = ReceiveItem.Item.GetGlobalItem<RenameItem>();
            if (ReceiveItem.Item.Name == newText) {
                ReceiveItem.Item = null;
            }

            if (ReceiveItem.Item != null && !ReceiveItem.Item.IsAir) {
                renameItem.CustomName = newText.Trim();
                renameItem.UpdateCustomName(ReceiveItem.Item);
            }

            ReceiveItem.UpdateIcon();
        };
        TextBox.SetTextMaxLength(60);
        TextBox.Width.Set(Width.Pixels, Width.Percent);
        TextBox.Height.Set(24, 0f);
        TextBox.Top.Set(56, 0f);
        TextBox.TextHAlign = 0f;
        TextBox.BackgroundColor = backgroundColor;
        Append(TextBox);

        UIImage sendItemPanel = new UIImage(TextureAssets.InventoryBack13) {
            ImageScale = itemSlotScale,
            Color = backgroundColor
        };

        Append(sendItemPanel);

        UIImage receiveItemPanel = new UIImage(TextureAssets.InventoryBack13) {
            ImageScale = itemSlotScale,
            Color = backgroundColor
        };
        receiveItemPanel.Left.Set(sendItemPanel.Width.Pixels + 36f, sendItemPanel.Width.Percent);

        Append(receiveItemPanel);

        SendItem = new ImprovedItemSlot(ItemSlot.Context.GuideItem, itemSlotScale);
        SendItem.CanPutItemIntoSlot += RenameItem.CanRename;
        SendItem.CanTakeItemFromSlot += (i) => true;
        SendItem.OnItemSwap += (from, to) => {
            SoundEngine.PlaySound(SoundID.Grab);

            ReceiveItem.Item?.TurnToAir();
            ReceiveItem.UpdateIcon();
            if (to == null || to.IsAir) {
                TextBox.SetText("");
                if (TextBox.IsWritingText) {
                    TextBox.ToggleText();
                }
                return;
            }

            TextBox.SetText(to.Name);
            TextBox.Unhighlight();
            TextBox.BringCursorToEnd();

            if (!TextBox.IsWritingText) {
                TextBox.ToggleText();
            }
        };
        SendItem.WhileHoveringItem += (item) => {
            if (item != null && !item.IsAir) {
                ExtendUI.HoverItem(item, SendItem.Context);
            }
            else {
                Main.hoverItemName = "";
                Main.HoverItem.TurnToAir();
                Main.instance.MouseText(ModContent.GetInstance<SkyMerchant>().GetLocalizedValue("Interface.PlaceHere"));
            }
        };
        SendItem.CanHover = true;
        SendItem.ShowItemTooltip = true;
        SendItem.Width.Set(48f, 0f);
        SendItem.Height.Set(48f, 0f);
        SendItem.SetIcon(new UIImage(AequusTextures.NameTagBlank.Asset) { HAlign = 0.5f, VAlign = 0.5f, Color = iconColor });
        SendItem.HAlign = 0.5f;
        SendItem.VAlign = 0.5f;

        sendItemPanel.Append(SendItem);

        ReceiveItem = new ImprovedItemSlot(ItemSlot.Context.GuideItem, itemSlotScale);
        ReceiveItem.CanTakeItemFromSlot += (i) => {
            ReceiveItem.GetDimensions();
            int price = RenameItem.GetRenamePrice(i);

            return price > 0 ? Main.LocalPlayer.CanAfford(price) : true;
        };
        ReceiveItem.OnItemSwap += (from, to) => {
            Main.LocalPlayer.BuyItem(RenameItem.GetRenamePrice(ReceiveItem.Item));
            SendItem.Item.TurnToAir();
            SendItem.UpdateIcon();
            SoundEngine.PlaySound(SoundID.Coins);

            TextBox.SetText("");
            TextBox.Unhighlight();
            TextBox.BringCursorToEnd();
            if (TextBox.IsWritingText) {
                TextBox.ToggleText();
            }
        };
        ReceiveItem.CanHover = true;
        ReceiveItem.ShowItemTooltip = true;
        ReceiveItem.Width.Set(SendItem.Width.Pixels, SendItem.Width.Percent);
        ReceiveItem.Height.Set(SendItem.Height.Pixels, SendItem.Height.Percent);
        ReceiveItem.SetIcon(new UIImage(AequusTextures.NameTag.Asset) { HAlign = 0.5f, VAlign = 0.5f, Color = iconColor });
        ReceiveItem.HAlign = 0.5f;
        ReceiveItem.VAlign = 0.5f;

        receiveItemPanel.Append(ReceiveItem);

        Asset<Texture2D> texturePackButtons = ModContent.Request<Texture2D>("Terraria/Images/UI/TexturePackButtons", AssetRequestMode.ImmediateLoad);
        UIImageFramed image = new UIImageFramed(texturePackButtons, texturePackButtons.Frame(horizontalFrames: 2, verticalFrames: 2, frameX: 1, frameY: 1));
        image.Left.Set(SendItem.Width.Pixels + 4f, SendItem.Width.Percent);
        image.Top.Set(SendItem.Top.Pixels + 10f, SendItem.Width.Percent);
        Append(image);
    }

    public override void OnActivate() {
        Main.playerInventory = true;
        Main.npcChatText = "";
    }

    public override void OnDeactivate() {
        if (!SendItem?.Item?.IsAir == true) {
            Main.LocalPlayer.QuickSpawnItem(new EntitySource_WorldEvent(), SendItem.Item, SendItem.Item.stack);
            SendItem.Item.TurnToAir();
        }
    }

    public override void Update(GameTime gameTime) {
        if (Main.LocalPlayer.TalkNPC?.ModNPC is not SkyMerchant) {
            ModContent.GetInstance<NPCChat>().Interface.SetState(null);
        }
        base.Update(gameTime);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch) {
        Main.hidePlayerCraftingMenu = true;

        CalculatedStyle style = GetDimensions();

        float savingsX = style.X + 140f;
        float savingsY = style.Y;
        ItemSlot.DrawSavings(Main.spriteBatch, savingsX, savingsY - 40f, true);

        if (!ReceiveItem.HasItem) {
            return;
        }
        int price = RenameItem.GetRenamePrice(SendItem.Item);

        string cost = Language.GetTextValue("LegacyInterface.46") + ": ";
        string priceText = ALanguage.PriceTextColored(price, NoValueText: Language.GetTextValue("Mods.AequusRemake.Misc.PriceFree"), pulse: true);

        var font = FontAssets.MouseText.Value;
        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, cost, new Vector2(savingsX, savingsY + 24f), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), 0f, Vector2.Zero, Vector2.One, -1f, 2f);
        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, priceText, new Vector2(savingsX + font.MeasureString(cost).X, savingsY + 24f), Color.White, 0f, Vector2.Zero, Vector2.One, -1f, 2f);
    }

    public void Load(Mod mod) { }

    public void Unload() { }
}