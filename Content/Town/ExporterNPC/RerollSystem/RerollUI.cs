using Aequus.Items;
using Aequus.Items.Placeable.Furniture.Misc;
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

        private List<Item>[] CreateItemRollLists(int amt, int item, List<IItemDropRule> rules)
        {
            var lists = new List<Item>[amt];

            var tests = new List<AequusItem.NewItem>[50];
            DropAttemptInfo dropAttemptInfo = new()
            {
                item = item,
                player = Main.LocalPlayer,
                IsExpertMode = Main.expertMode,
                IsMasterMode = Main.masterMode,
                rng = Main.rand,
                IsInSimulation = true,
            };
            AequusItem.EnablePreventItemDrops = true;
            for (int i = 0; i < tests.Length; i++)
            {
                AequusItem.PreventedItemDrops.Clear();
                try
                {
                    Main.ItemDropSolver.TryDropping(dropAttemptInfo);
                    tests[i] = new(AequusItem.PreventedItemDrops);
                }
                catch (Exception ex)
                {
                    Aequus.Instance.Logger.Error($"{ex.Message}\n{ex.StackTrace}");
                }
            }
            AequusItem.EnablePreventItemDrops = false;

            Dictionary<int, int> itemCounts = new();
            for (int i = 0; i < tests.Length; i++)
            {
                foreach (var d in tests[i])
                {
                    if (ContentSamples.ItemsByType[d.Type].maxStack != 1)
                    {
                        continue;
                    }
                    if (itemCounts.ContainsKey(d.Type))
                    {
                        itemCounts[d.Type]++;
                    }
                    else
                    {
                        itemCounts[d.Type] = 1;
                    }
                }
            }

            if (itemCounts.Count == 0)
            {
                return lists;
            }

            List<Item> itemsToRoll = new();
            foreach (var data in itemCounts)
            {
                if (data.Value == tests.Length)
                {
                    continue;
                }
                itemsToRoll.Add(new(data.Key));
            }
            int amount = Math.Min(itemsToRoll.Count, amt);
            for (int i = 0; i < amount; i++)
            {
                lists[i] = new(itemsToRoll);
            }
            return lists;
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

            if (!itemSlot.HasItem)
            {
                return;
            }

            var lists = CreateItemRollLists(slots.Length, itemSlot.item.type, Main.ItemDropsDB.GetRulesForItemID(itemSlot.item.type, includeGlobalDrops: false));
            for (int i = 0; i < slots.Length; i++)
            {
                if (lists[i] != null)
                {
                    slots[i].BeginRoll(lists[i]);
                }
                else
                {
                    slots[i].disable = true;
                }
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
                BackgroundColor = new Color(68, 99, 164) * 0.825f
            };
            uiPanel.Width.Set(0, 1f);
            uiPanel.Height.Set(0, 1f);
            Append(uiPanel);

            SetupRollMachine();
            SetupItemSlot();
            var uiText = new UIText("* Rolled items are guaranteed to drop *");
            uiText.HAlign = 0.5f;
            uiText.Top.Set(80f, 0f);
            Append(uiText);
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
        public List<Item> selection;
        public Item[] workingItems = new Item[3];
        public float rollSpeed;
        public float rollAnimation;
        public float giveUp;
        public bool playRollAnimation;
        public bool disable;

        public void BeginRoll(List<Item> selection)
        {
            rollSpeed = 0.15f;
            disable = false;
            playRollAnimation = true;
            this.selection = selection;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!playRollAnimation)
            {
                if (rollSpeed > 0f)
                {
                    rollSpeed -= 0.0001f;
                    rollSpeed *= 0.99f;
                    if (rollSpeed < 0f)
                    {
                        rollSpeed = 0f;
                    }
                }
                giveUp = 0f;
                if (rollSpeed == 0f)
                {
                    rollAnimation %= 1f;
                    rollAnimation = MathHelper.Lerp(rollAnimation, 0.5f, 0.1f);
                }
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
            if (rollAnimation > 1f)
            {
                for (int i = 1; i < workingItems.Length; i++)
                {
                    workingItems[i] = workingItems[i - 1];
                }
                workingItems[0] = Main.rand.Next(selection);
                rollAnimation = 0f;
            }
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
            float animationOffset = 1f / workingItems.Length;
            Helper.DrawUIPanel(spriteBatch, AequusTextures.Panel, dimensions.ToRectangle());
            for (int i = 0; i < workingItems.Length; i++)
            {
                float opacity = MathF.Sin((i + rollAnimation) / workingItems.Length * MathHelper.TwoPi);
                Vector2 drawPosition = new(dimensions.X + dimensions.Width / 2f, dimensions.Y + heightAdd * (i + 0.75f) + heightAdd * rollAnimationWrapped);
                if (workingItems[i] == null || workingItems[i].IsAir)
                {
                    spriteBatch.Draw(
                        TextureAssets.Cd.Value,
                        drawPosition,
                        null,
                        Color.White * opacity,
                        0f,
                        TextureAssets.Cd.Value.Size() / 2f,
                        Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 0.9f, 1.1f),
                        SpriteEffects.None, 0f
                    );
                    continue;
                }

                Main.instance.LoadItem(workingItems[i].type);
                var texture = TextureAssets.Item[workingItems[i].type].Value;
                Helper.GetItemDrawData(workingItems[i], out var frame);
                float scale = 1f;
                int largestSide = Math.Max(frame.Width, frame.Height);
                if (largestSide > 40f)
                {
                    scale = 40f / largestSide;
                }
                spriteBatch.Draw(
                    texture,
                    drawPosition,
                    frame,
                    Color.White * opacity,
                    0f,
                    frame.Size() / 2f,
                    scale,
                    SpriteEffects.None, 0f
                );
            }
        }
    }
}