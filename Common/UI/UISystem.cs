using Aequus.Common.UI.EventBars;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.GameInput;
using Terraria.UI;

namespace Aequus.Common.UI;

public partial class UISystem : ModSystem {
    public static int bottomLeftInventoryOffsetX;
    public static byte linkClickDelay;
    public static byte specialLeftClickDelay;
    public static byte disableItemLeftClick;

    public static ItemSlotContext CurrentItemSlot;

    public static HashSet<int> ValidOnlineLinkedSlotContext { get; private set; }
    public static readonly List<UILayer> UserInterfaces = new();

    public static int BottomInventoryY => 260;

    public static byte DisableItemLeftClick { get => disableItemLeftClick; set => disableItemLeftClick = Math.Max(disableItemLeftClick, value); }

    public static bool CanDoLeftClickItemActions => specialLeftClickDelay == 0;

    public const float invBackColorMultipler = 0.785f;
    public static readonly Color invBackColor = new Color(63, 65, 151, 255);
    public static Color InventoryBackColor => invBackColor * invBackColorMultipler;

    public static UserInterface TalkInterface { get; private set; }

    public static void RegisterUserInterface(UILayer face) {
        UserInterfaces.Add(face);
    }

    public override void Load() {
        LoadHooks();
        TalkInterface = new();
        ValidOnlineLinkedSlotContext = new() {
            ItemSlot.Context.EquipAccessory,
            ItemSlot.Context.ModdedAccessorySlot,
            ItemSlot.Context.EquipAccessoryVanity,
            ItemSlot.Context.ModdedVanityAccessorySlot,
            ItemSlot.Context.InventoryItem,
            ItemSlot.Context.BankItem,
            ItemSlot.Context.ChestItem,
            ItemSlot.Context.VoidItem,
        };
    }
    private void LoadHooks() {
        On_ItemSlot.LeftClick_ItemArray_int_int += Hook_DisableLeftClick;
        On_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += ItemSlot_Draw;
    }

    private static void Hook_DisableLeftClick(On_ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot) {
        if (disableItemLeftClick == 0) {
            orig(inv, context, slot);
        }
    }

    private static void ItemSlot_Draw(On_ItemSlot.orig_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color orig, SpriteBatch spriteBatch, Item[] inv, int context, int slot, Vector2 position, Color lightColor) {
        CurrentItemSlot = new(context, slot, inv, position, lightColor);
        orig(spriteBatch, inv, context, slot, position, lightColor);

        if (inv[slot].IsAir) {
            //SlotDecals.DrawEmptySlotDecals(spriteBatch, inv, context, slot, position, lightColor);
        }
    }

    public override void Unload() {
        ValidOnlineLinkedSlotContext?.Clear();
        ValidOnlineLinkedSlotContext = null;
    }

    public override void ClearWorld() {
        TalkInterface?.SetState(null);
        if (Main.netMode != NetmodeID.Server) {
            foreach (var i in UserInterfaces) {
                i.OnClearWorld();
            }
        }
    }

    public override void PreUpdatePlayers() {
        if (Main.netMode != NetmodeID.Server) {
            foreach (var i in UserInterfaces) {
                i.OnPreUpdatePlayers();
            }
        }
    }

    public override void UpdateUI(GameTime gameTime) {
        bottomLeftInventoryOffsetX = 0;
        foreach (var i in UserInterfaces) {
            i.OnUIUpdate(gameTime);
        }
        TalkInterface.Update(gameTime);
        if (Main.mouseItem != null && !Main.mouseItem.IsAir) {
            specialLeftClickDelay = Math.Max(specialLeftClickDelay, (byte)20);
        }
        else if (specialLeftClickDelay > 0) {
            specialLeftClickDelay--;
        }
        if (linkClickDelay > 0) {
            linkClickDelay--;
        }
        if (disableItemLeftClick > 0) {
            disableItemLeftClick--;
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
        bottomLeftInventoryOffsetX = 0;
        ManageUserInterfaceLayer(layers, TalkInterface, InterfaceLayers.Inventory_28, "Aequus: NPC Talk Interface", InterfaceScaleType.UI);

        foreach (var i in UserInterfaces) {
            InsertInterfaceDrawMethod(layers, i.Layer, $"{i.Mod}: {i.Name}", () => {
                return i.Draw(Main.spriteBatch);
            }, i.ScaleType);
        }

        //InsertInterfaceDrawMethod(layers, InterfaceLayers.Ruler_6, "Aequus: Misc World Interface", () => {
        //    return true;
        //}, InterfaceScaleType.Game);

        InsertInterfaceDrawMethod(layers, InterfaceLayers.Inventory_28, "Aequus: Inventory", () => {
            AequusEventBarLoader.Draw();
            return true;
        });
    }
    private void ManageUserInterfaceLayer(List<GameInterfaceLayer> layers, UserInterface userInterface, string defaultLayer, string layerName, InterfaceScaleType scaleType = InterfaceScaleType.UI) {
        int layer = -1;
        if (userInterface.CurrentState is AequusUIState aequusUIState) {
            if (!aequusUIState.ModifyInterfaceLayers(layers, ref scaleType)) {
                return;
            }
            layer = aequusUIState.GetLayerIndex(layers);
        }
        if (layer == -1) {
            layer = layers.FindIndex((g) => g.Name.Equals(defaultLayer));
        }
        if (layer != -1) {
            layers.Insert(layer, new LegacyGameInterfaceLayer(layerName, () => {
                userInterface.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
                return true;
            }, scaleType));
        }
    }
    private void InsertInterfaceDrawMethod(List<GameInterfaceLayer> layers, string name, string yourName, GameInterfaceDrawMethod method, InterfaceScaleType scaleType = InterfaceScaleType.UI) {
        int index = layers.FindIndex((l) => l.Name.Equals(name));
        if (index != -1) {
            layers.Insert(index + 1, new LegacyGameInterfaceLayer(yourName, method, scaleType));
        }
    }

    public static void CloseAllInventoryRelatedUI() {
        CloseAllInventoryRelatedUI(Main.LocalPlayer);
    }
    public static void CloseAllInventoryRelatedUI(Player player) {
        player.chest = -1;
        player.sign = -1;
        player.SetTalkNPC(-1);
        if (player.whoAmI == Main.myPlayer) {
            Main.playerInventory = false;
            Main.recBigList = false;
            Main.CreativeMenu.CloseMenu();
            if (PlayerInput.GrappleAndInteractAreShared) {
                PlayerInput.Triggers.JustPressed.Grapple = false;
            }
            TalkInterface.SetState(null);
        }
    }

    public static void HoverItem(Item item, int context = -1) {
        Main.hoverItemName = item.Name;
        Main.HoverItem = item.Clone();
        Main.HoverItem.tooltipContext = context;
    }
}