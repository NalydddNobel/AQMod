using Aequu2.Core.UI;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.UI;

namespace Aequu2.Content.Enemies.PollutedOcean.Scavenger.UI;

public class ScavengerLootBagUI : UIState, ILoad {
    public override void OnActivate() {
        Main.playerInventory = true;
        Main.npcChatText = "";
    }

    public override void OnDeactivate() {
        Main.trashSlotOffset = new Point16(0, 0);
    }

    public override void Update(GameTime gameTime) {
        if (Main.LocalPlayer.TalkNPC?.ModNPC is not ScavengerLootBag) {
            ModContent.GetInstance<NPCChat>().Interface.SetState(null);
        }
        base.Update(gameTime);
    }

    private static Vector2 GetSlotPosition(int i) {
        return new Vector2(74f + 42f * (i % 10), 260f + 42f * (i / 10));
    }

    protected override void DrawSelf(SpriteBatch spriteBatch) {
        var player = Main.LocalPlayer;
        var talkNPC = player.TalkNPC;
        if (talkNPC?.ModNPC is not ScavengerLootBag lootBag) {
            return;
        }

        Main.inventoryScale = 0.75f;
        Main.trashSlotOffset = new Point16(4, 170);
        var slotTexture = TextureAssets.InventoryBack5.Value;
        var slotOrigin = slotTexture.Size() / 2f;
        var slotColor = Main.inventoryBack;
        int context = ItemSlot.Context.ChestItem;
        for (int i = 0; i < Chest.maxItems; i++) {
            var slotPosition = GetSlotPosition(i);

            bool invalidSlot = !lootBag.drops.IndexInRange(i) || lootBag.drops[i].IsAir;
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

                ExtendUI.HoverItem(lootBag.drops[i], context);
            }

            ItemSlotDrawHelper.DrawFullItem(lootBag.drops[i], context, i, spriteBatch, slotPosition, slotPosition + slotOrigin * Main.inventoryScale, Main.inventoryScale, 32f, Color.White, Color.White);
        }
    }

    public void Load(Mod mod) { }

    public void Unload() { }
}