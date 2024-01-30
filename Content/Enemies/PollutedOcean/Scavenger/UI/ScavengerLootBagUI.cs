using Aequus.Common.UI;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.UI;

namespace Aequus.Content.Enemies.PollutedOcean.Scavenger.UI;

public class ScavengerLootBagUI : AequusUIState {
    public override void OnInitialize() {
        OverrideSamplerState = SamplerState.LinearClamp;

        Width.Set(750, 0f);
        Height.Set(100, 0f);
    }

    public override void OnDeactivate() {
        Main.trashSlotOffset = new(0, 0);
    }

    public override void Update(GameTime gameTime) {
        base.Update(gameTime);
        if (NotTalkingTo<ScavengerLootBag>()) {
            CloseThisInterface();
        }
    }

    private Vector2 GetSlotPosition(System.Int32 i) {
        return new Vector2(74f + 42f * (i % 10), 260f + 42f * (i / 10));
    }

    protected override void DrawSelf(SpriteBatch spriteBatch) {
        base.DrawSelf(spriteBatch);
        var player = Main.LocalPlayer;
        var talkNPC = player.TalkNPC;
        if (talkNPC?.ModNPC is not ScavengerLootBag lootBag) {
            return;
        }

        Main.inventoryScale = 0.75f;
        Main.trashSlotOffset = new(4, 170);
        var slotTexture = TextureAssets.InventoryBack5.Value;
        var slotOrigin = slotTexture.Size() / 2f;
        var slotColor = Main.inventoryBack;
        System.Int32 context = ItemSlot.Context.ChestItem;
        for (System.Int32 i = 0; i < Chest.maxItems; i++) {
            var slotPosition = GetSlotPosition(i);

            System.Boolean invalidSlot = !lootBag.drops.IndexInRange(i) || lootBag.drops[i].IsAir;
            Main.spriteBatch.Draw(slotTexture, slotPosition + slotOrigin * Main.inventoryScale, null, slotColor * (invalidSlot ? 0.5f : 1f), 0f, slotOrigin, Main.inventoryScale, SpriteEffects.None, 0f);

            if (invalidSlot) {
                continue;
            }

            if (Main.mouseX >= slotPosition.X && Main.mouseX <= slotPosition.X + slotTexture.Width * Main.inventoryScale && Main.mouseY >= slotPosition.Y && Main.mouseY <= slotPosition.Y + slotTexture.Height * Main.inventoryScale && !PlayerInput.IgnoreMouseInterface) {
                player.mouseInterface = true;
                Main.cursorOverride = CursorOverrideID.ChestToInventory;

                if (Main.mouseLeftRelease && Main.mouseLeft && player.ItemSpace(lootBag.drops[i]).CanTakeItem) {
                    if (Main.netMode == NetmodeID.MultiplayerClient) {
                        ModContent.GetInstance<ScavengerLootBagGrabPacket>().Send(talkNPC.whoAmI, Main.myPlayer, i);
                    }
                    else {
                        player.GetItem(player.whoAmI, lootBag.drops[i].Clone(), GetItemSettings.NPCEntityToPlayerInventorySettings);
                        lootBag.drops[i].TurnToAir();
                    }
                    Recipe.FindRecipes();
                    continue;
                }
                ItemSlot.MouseHover(lootBag.drops, context, i);
            }

            ItemSlotRenderer.DrawFullItem(lootBag.drops[i], context, i, Main.spriteBatch, slotPosition, slotPosition + slotOrigin * Main.inventoryScale, Main.inventoryScale, 32f, Color.White, Color.White);
        }
    }
}