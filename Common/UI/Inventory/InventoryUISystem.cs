using Aequus.Common.Backpacks;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Gamepad;

namespace Aequus.Common.UI.Inventory;

public class InventoryUISystem : ModSystem {
    public static int CoinsAmmoOffsetX;
    public static int RightsideButtonsOffsetY;

    public override void Load() {
        IL_Main.DrawInventory += IL_Main_DrawInventory;
        IL_Main.DrawBestiaryIcon += IL_Main_DrawEmoteAndBestiaryButton;
        IL_Main.DrawEmoteBubblesButton += IL_Main_DrawEmoteAndBestiaryButton;
    }

    private static void IL_Main_DrawEmoteAndBestiaryButton(ILContext il) {
        MonoModHooks.DumpIL(ModContent.GetInstance<Aequus>(), il);
        var cursor = new ILCursor(il);
        if (!cursor.TryGotoNext((i) => i.MatchLdsfld(typeof(Main), nameof(Main.player)))) {
            throw new Exception($"Could not find {nameof(Main)}.{nameof(Main.player)} loading code.");
        }
        cursor.Index--;
        cursor.EmitDelegate<Func<int, int>>((y) => y + ((Main.LocalPlayer.chest == -1 && Main.npcShop <= 0) || Main.recBigList ? RightsideButtonsOffsetY : 0));
    }

    private static void MoveAchievementAdvisor(ILCursor cursor) {
        if (!cursor.TryGotoNext((i) => i.MatchCallvirt(typeof(AchievementAdvisor), nameof(AchievementAdvisor.DrawOneAchievement)))) {
            throw new Exception($"Could not find {nameof(AchievementAdvisor)}.{nameof(AchievementAdvisor.DrawOneAchievement)} virtual method.");
        }
        if (!cursor.TryGotoPrev((i) => i.MatchLdsfld(typeof(Main), nameof(Main.spriteBatch)))) {
            throw new Exception($"Could not find {nameof(Main)}.{nameof(Main.spriteBatch)} loading code.");
        }
        cursor.Index += 2;
        cursor.EmitDelegate<Func<int, int>>((x) => x + CoinsAmmoOffsetX);
    }

    private static void MoveSlots(ILCursor cursor, string weirdHardcodedNameLeftover, float hardcodedTextXValue, int hardcodedSlotXValue) {
        // Find the "Coins" text
        if (!cursor.TryGotoNext((i) => i.MatchLdstr(weirdHardcodedNameLeftover))) {
            throw new Exception($"Could not find '{weirdHardcodedNameLeftover}' loaded string.");
        }

        // Find the number 496, a hardcoded value for rendering the Coins text
        if (!cursor.TryGotoNext((i) => i.MatchLdcR4(hardcodedTextXValue))) {
            throw new Exception($"Could not find '{weirdHardcodedNameLeftover}' text X value.");
        }
        cursor.Index++;
        cursor.EmitDelegate<Func<float, float>>((i) => i + CoinsAmmoOffsetX);

        // Find the number 497, another hardcoded value but this time for rendering the Coins slot
        if (!cursor.TryGotoNext((i) => i.MatchLdcI4(hardcodedSlotXValue))) {
            throw new Exception($"Could not find '{weirdHardcodedNameLeftover}' slot X value.");
        }
        cursor.Index++;
        cursor.EmitDelegate<Func<int, int>>((x) => x + CoinsAmmoOffsetX);
    }

    private static void MoveChestIcons(ILCursor cursor) {
        if (!cursor.TryGotoNext((i) => i.MatchCall(typeof(ChestUI), nameof(ChestUI.Draw)))) {
            throw new Exception($"Could not find {nameof(Main)}.{nameof(ChestUI.Draw)} method.");
        }
        if (!cursor.TryGotoNext((i) => i.MatchCall(typeof(UILinkPointNavigator), nameof(UILinkPointNavigator.SetPosition)))) {
            throw new Exception($"Could not find {nameof(UILinkPointNavigator)}.{nameof(UILinkPointNavigator.SetPosition)} method.");
        }
        if (!cursor.TryGotoNext((i) => i.MatchLdsfld(typeof(TextureAssets), nameof(TextureAssets.ChestStack)))) {
            throw new Exception($"Could not find {nameof(TextureAssets)}.{nameof(TextureAssets.ChestStack)} loading code.");
        }
        if (!cursor.TryGotoNext((i) => i.MatchLdelemRef())) {
            throw new Exception($"Could not find ldelem.ref.");
        }
        // X
        if (!cursor.TryGotoNext((i) => i.MatchLdloc(out _))) {
            throw new Exception($"Could not find x value for chest stack drawing.");
        }
        // Y
        if (!cursor.TryGotoNext((i) => i.MatchLdloc(out _))) {
            throw new Exception($"Could not find y value for chest stack drawing.");
        }
        cursor.Index++;
        cursor.EmitDelegate<Func<int, int>>((y) => y + RightsideButtonsOffsetY);

        if (!cursor.TryGotoNext((i) => i.MatchCall(typeof(UILinkPointNavigator), nameof(UILinkPointNavigator.SetPosition)))) {
            throw new Exception($"Could not find {nameof(UILinkPointNavigator)}.{nameof(UILinkPointNavigator.SetPosition)} method.");
        }
        if (!cursor.TryGotoNext((i) => i.MatchLdsfld(typeof(TextureAssets), nameof(TextureAssets.InventorySort)))) {
            throw new Exception($"Could not find {nameof(TextureAssets)}.{nameof(TextureAssets.InventorySort)} loading code.");
        }
        if (!cursor.TryGotoNext((i) => i.MatchLdelemRef())) {
            throw new Exception($"Could not find ldelem.ref.");
        }
        if (!cursor.TryGotoNext((i) => i.MatchLdsfld(typeof(Main), nameof(Main.spriteBatch)))) {
            throw new Exception($"Could not find {nameof(Main)}.{nameof(Main.spriteBatch)} loading code.");
        }
        // Texture2D
        if (!cursor.TryGotoNext((i) => i.MatchLdloc(out _))) {
            throw new Exception($"Could not find x value for chest sort drawing.");
        }
        // X
        if (!cursor.TryGotoNext((i) => i.MatchLdloc(out _))) {
            throw new Exception($"Could not find y value for chest sort drawing.");
        }
        // Y
        if (!cursor.TryGotoNext((i) => i.MatchLdloc(out _))) {
            throw new Exception($"Could not find y value for chest sort drawing.");
        }
        cursor.Index++;
        cursor.EmitDelegate<Func<int, int>>((y) => y + RightsideButtonsOffsetY);
    }

    private static void IL_Main_DrawInventory(ILContext il) {
        var cursor = new ILCursor(il);
        try {
            MoveAchievementAdvisor(cursor);
            MoveSlots(cursor, "Coins", 496f, 497);
            MoveSlots(cursor, "Ammo", 532f, 534);
            MoveChestIcons(cursor);
        }
        catch {
            MonoModHooks.DumpIL(ModContent.GetInstance<Aequus>(), il);
        }
    }

    public override void UpdateUI(GameTime gameTime) {
        if (!Main.LocalPlayer.TryGetModPlayer(out BackpackPlayer aequusPlayer)) {
            return;
        }
        BackpackLoader.AnimateBackpacks(aequusPlayer.backpacks, out int totalInventorySlots, out int activeBackpacks);

        int coinsAmmoOffsetWantedX = 0;
        if (totalInventorySlots > 0) {
            coinsAmmoOffsetWantedX = (int)(((totalInventorySlots - 1) / 5 + 1) * BackpackSlotsUI.SlotWidth * BackpackSlotsUI.InventoryScale) + BackpackSlotsUI.BackpackPadding * activeBackpacks;
        }

        if (CoinsAmmoOffsetX < coinsAmmoOffsetWantedX) {
            CoinsAmmoOffsetX = (int)MathHelper.Lerp(CoinsAmmoOffsetX, coinsAmmoOffsetWantedX, 0.33f);
            CoinsAmmoOffsetX++;
            if (CoinsAmmoOffsetX > coinsAmmoOffsetWantedX) {
                CoinsAmmoOffsetX = coinsAmmoOffsetWantedX;
            }
        }
        else if (CoinsAmmoOffsetX > coinsAmmoOffsetWantedX) {
            CoinsAmmoOffsetX = (int)MathHelper.Lerp(CoinsAmmoOffsetX, coinsAmmoOffsetWantedX, 0.1f);
            CoinsAmmoOffsetX--;
            if (CoinsAmmoOffsetX < coinsAmmoOffsetWantedX) {
                CoinsAmmoOffsetX = coinsAmmoOffsetWantedX;
            }
        }

        if (CoinsAmmoOffsetX > 0) {
            if (RightsideButtonsOffsetY < 16) {
                RightsideButtonsOffsetY++;
            }
        }
        else if (RightsideButtonsOffsetY > 0) {
            RightsideButtonsOffsetY--;
        }
    }
}