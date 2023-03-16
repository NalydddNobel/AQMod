using Aequus.Content.Town.CarpenterNPC;
using Aequus.UI;
using Aequus.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Content.Town.ExporterNPC.RerollSystem
{
    public class RerollUI : AequusUIState
    {
        public PlaceableItemSlotElement itemSlot;
        public RerollSlot[] slots;

        public static HashSet<int> RerollBlacklist = new();
        public static Dictionary<int, Func<Player, bool>> RerollCondition = new();

        public override void Load(Mod mod)
        {
        }

        public override void Unload()
        {
            RerollCondition.Clear();
            RerollBlacklist.Clear();
        }

        public int GetSlotAmount(Player player)
        {
            return 3;
        }

        private void SetupRollMachine()
        {
            int amount = GetSlotAmount(Main.LocalPlayer);
            slots = new RerollSlot[amount];
            float padding = 1f / (amount * amount);
            for (int i = 0; i < amount; i++)
            {
                slots[i] = new RerollSlot();
                slots[i].Width.Set(54, 0f);
                slots[i].Height.Set(120, 0f);
                slots[i].Left.Set(-slots[i].Width.Pixels / (amount * amount), 1 / (float)amount * i + padding);
                slots[i].Top.Set(-140f, 1f);
                Append(slots[i]);
            }
        }
        private void SetupItemSlot()
        {
            itemSlot = new(TextureAssets.InventoryBack.Value);
            itemSlot.Width.Set(54f, 0f);
            itemSlot.Height.Set(54f, 0f);
            itemSlot.Top.Set(10f, 0f);
            itemSlot.HAlign = 0.5f;
            itemSlot.CanPlaceInSlot = ItemSlot_CanPlaceInSlot;
            Append(itemSlot);

            var button = new UIImageButton(AequusTextures.RerollButton.Asset);
            button.Left.Set(70f, 0f);
            button.Top.Set(10f, 0f);
            button.HAlign = itemSlot.HAlign;
            button.OnClick += RerollButton_OnClick;
            Append(button);
        }

        private bool ItemSlot_CanPlaceInSlot(Item incomingItem)
        {
            return true;
        }

        private void RerollButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].playRollAnimation)
                {
                    slots[i].EndRoll();
                    return;
                }
            }

            if (itemSlot.HasItem)
            {
                return;
            }

            var list = Main.ItemDropsDB.GetRulesForItemID(itemSlot.item.type, includeGlobalDrops: false);
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].BeginRoll(list);
            }
        }

        public override void OnInitialize()
        {
            OverrideSamplerState = SamplerState.LinearClamp;

            Width.Set(300, 0.05f);
            Height.Set(200, 0.05f);
            Top.Set(100, 0f);
            HAlign = 0.5f;

            var uiPanel = new UIPanel
            {
                BackgroundColor = new Color(68, 99, 164) * 0.5f
            };
            uiPanel.Width.Set(0, 1f);
            uiPanel.Height.Set(0, 1f);
            Append(uiPanel);

            SetupRollMachine();
            SetupItemSlot();
        }

        public override void Update(GameTime gameTime)
        {
            if (NotTalkingTo<Exporter>())
            {
                Aequus.UserInterface.SetState(null);
                return;
            }
            if (GetDimensions().ToRectangle().Contains(Main.mouseX, Main.mouseY))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            base.Update(gameTime);
        }

        public override void ConsumePlayerControls(Player player)
        {
            if (player.controlInv)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                player.releaseInventory = false;
                player.SetTalkNPC(-1);
                Aequus.UserInterface.SetState(null);
            }
        }

        public override int GetLayerIndex(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex((l) => l.Name.Equals(AequusUI.InterfaceLayers.Inventory_28));
            if (index == -1)
                return -1;
            return index + 1;
        }

        public override bool ModifyInterfaceLayers(List<GameInterfaceLayer> layers, ref InterfaceScaleType scaleType)
        {
            DisableAnnoyingInventoryLayeringStuff(layers);
            return true;
        }
    }

    public class RerollSlot : UIElement
    {
        public Item[] workingItems = new Item[3];
        public List<IItemDropRule> workingRules;
        public float rollSpeed;
        public float rollAnimation;
        public float giveUp;
        public bool playRollAnimation;

        public void BeginRoll(List<IItemDropRule> rules)
        {
            Main.NewText("began roll!?");
            rollSpeed = 0.15f;
            playRollAnimation = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!playRollAnimation)
            {
                if (rollSpeed > 0f)
                {
                    rollSpeed -= 0.004f;
                    if (rollSpeed < 0f)
                    {
                        rollAnimation = 0f;
                        rollSpeed = 0f;
                    }
                }
                giveUp = 0f;
            }
            else
            {
                giveUp++;
                if (giveUp > 600)
                {
                    giveUp = 0f;
                    playRollAnimation = false;
                }
            }
            rollAnimation += rollSpeed;
        }

        public void EndRoll()
        {
            playRollAnimation = false;
            rollAnimation *= 0.98f;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var dimensions = GetDimensions();
            float heightAdd = dimensions.Height / workingItems.Length;
            float rollAnimationWrapped = rollAnimation % 1f;
            for (int i = 0; i < workingItems.Length; i++)
            {
                Vector2 drawPosition = new(dimensions.X + dimensions.Width / 2f, dimensions.Y + heightAdd * i + heightAdd * rollAnimationWrapped);
                if (workingItems[i] == null || workingItems[i].IsAir)
                {
                    spriteBatch.Draw(
                        TextureAssets.Cd.Value,
                        drawPosition,
                        null,
                        Color.White,
                        0f,
                        TextureAssets.Cd.Value.Size() / 2f,
                        Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 0.9f, 1.1f),
                        SpriteEffects.None, 0f
                    );
                    continue;
                }
            }
        }
    }
}