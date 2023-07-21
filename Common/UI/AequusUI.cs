using Aequus.Common.Items.SlotDecals;
using Aequus.Common.UI.EventBars;
using Aequus.Content.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Common.UI {
    public class AequusUI : ModSystem {
        public class InterfaceLayers {
            public const string InterfaceLogic1_0 = "Vanilla: Interface Logic 1";
            public const string MultiplayerPlayerNames_1 = "Vanilla: MP Player Names";
            public const string EmoteBubbles_2 = "Vanilla: Emote Bubbles";
            public const string EntityMarkers_3 = "Vanilla: Entity Markers";
            public const string SmartCursorTargets_4 = "Vanilla: Smart Cursor Targets";
            public const string LaserRuler_5 = "Vanilla: Laser Ruler";
            public const string Ruler_6 = "Vanilla: Ruler";
            public const string GamepadLockOn_7 = "Vanilla: Gamepad Lock On";
            public const string TileGridOption_8 = "Vanilla: Tile Grid Option";
            public const string TownNPCHouseBanners_9 = "Vanilla: Town NPC House Banners";
            public const string HideUIToggle_10 = "Vanilla: Hide UI Toggle";
            public const string WireSelection_11 = "Vanilla: Wire Selection";
            public const string CaptureManagerCheck_12 = "Vanilla: Capture Manager Check";
            public const string IngameOptions_13 = "Vanilla: Ingame Options";
            public const string FancyUI_14 = "Vanilla: Fancy UI";
            public const string AchievementCompletePopups_15 = "Vanilla: Achievement Complete Popups";
            public const string EntityHealthBars_16 = "Vanilla: Entity Health Bars";
            public const string InvasionProgressBars_17 = "Vanilla: Invasion Progress Bars";
            public const string MapMinimap_18 = "Vanilla: Map / Minimap";
            public const string DiagnoseNet_19 = "Vanilla: Diagnose Net";
            public const string DiagnoseVideo_20 = "Vanilla: Diagnose Video";
            public const string SignTileBubble_21 = "Vanilla: Sign Tile Bubble";
            public const string HairWindow_22 = "Vanilla: Hair Window";
            public const string DresserWindow_23 = "Vanilla: Dresser Window";
            public const string NPCSignDialog_24 = "Vanilla: NPC / Sign Dialog";
            public const string InterfaceLogic2_25 = "Vanilla: Interface Logic 2";
            public const string ResourceBars_26 = "Vanilla: Resource Bars";
            public const string InterfaceLogic3_27 = "Vanilla: Interface Logic 3";
            public const string Inventory_28 = "Vanilla: Inventory";
            public const string InfoAccessoriesBar_29 = "Vanilla: Info Accessories Bar";
            public const string SettingsButton_30 = "Vanilla: Settings Button";
            public const string Hotbar_31 = "Vanilla: Hotbar";
            public const string BuildersAccessoriesBar_32 = "Vanilla: Builder Accessories Bar";
            public const string RadialHotbars_33 = "Vanilla: Radial Hotbars";
            public const string MouseText_34 = "Vanilla: Mouse Text";
            public const string PlayerChat_35 = "Vanilla: Player Chat";
            public const string Cursor_36 = "Vanilla: Cursor";
            public const string MouseItemOrNPCHead_37 = "Vanilla: Mouse Item / NPC Head";
            public const string MouseOver_38 = "Vanilla: Mouse Over";
            public const string InteractItemIcon_39 = "Vanilla: Interact Item Icon";
            public const string InterfaceLogic4_40 = "Vanilla: Interface Logic 4";
        }
        public record struct ItemSlotContext(int Context, int Slot, Item[] Inventory, Vector2 Position, Color LightColor);

        public const int LeftInv = 20;

        public static int leftInvOffset;
        public static byte linkClickDelay;
        public static byte specialLeftClickDelay;
        public static byte disableItemLeftClick;

        public static ItemSlotContext CurrentItemSlot;

        public static HashSet<int> ValidOnlineLinkedSlotContext { get; private set; }
        public static List<UILayer> UserInterfaces { get; private set; }

        public static int BottomInventory => 260;

        public static byte DisableItemLeftClick { get => disableItemLeftClick; set => disableItemLeftClick = Math.Max(disableItemLeftClick, value); }

        public static bool CanDoLeftClickItemActions => specialLeftClickDelay == 0;

        public const float invBackColorMultipler = 0.785f;
        public static readonly Color invBackColor = new Color(63, 65, 151, 255);
        public static Color InventoryBackColor => invBackColor * invBackColorMultipler;

        public static void RegisterUserInterface(UILayer face) {
            (UserInterfaces ??= new()).Add(face);
        }

        public override void Load() {
            LoadHooks();
            UserInterfaces ??= new();

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
                SlotDecals.DrawEmptySlotDecals(spriteBatch, inv, context, slot, position, lightColor);
            }
        }

        public override void Unload() {
            ValidOnlineLinkedSlotContext?.Clear();
            ValidOnlineLinkedSlotContext = null;
        }

        public override void ClearWorld() {
            Aequus.UserInterface?.SetState(null);
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
            leftInvOffset = 0;
            foreach (var i in UserInterfaces) {
                i.OnUIUpdate(gameTime);
            }
            Aequus.UserInterface.Update(gameTime);
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
            leftInvOffset = 0;
            ManageUserInterfaceLayer(layers, Aequus.UserInterface, InterfaceLayers.Inventory_28, "Aequus: NPC Talk Interface", InterfaceScaleType.UI);

            foreach (var i in UserInterfaces) {
                InsertInterfaceDrawMethod(layers, i.Layer, $"{i.Mod}: {i.Name}", () => {
                    return i.Draw(Main.spriteBatch);
                }, i.ScaleType);
            }

            InsertInterfaceDrawMethod(layers, InterfaceLayers.Ruler_6, "Aequus: Misc World Interface", () => {
                if (AdvancedRulerInterface.Instance.Enabled) {
                    AdvancedRulerInterface.Instance.Render(Main.spriteBatch);
                }
                return true;
            }, InterfaceScaleType.Game);

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

        public static int LeftInventory(bool ignoreCreative = false) {
            int left = LeftInv;
            if (!ignoreCreative && Main.LocalPlayer.difficulty == 3 && !Main.CreativeMenu.Blocked) {
                left += 48;
            }
            return leftInvOffset + left;
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
                Aequus.UserInterface.SetState(null);
            }
        }

        public static void HoverItem(Item item, int context = -1) {
            Main.hoverItemName = item.Name;
            Main.HoverItem = item.Clone();
            Main.HoverItem.tooltipContext = context;
        }
    }
}