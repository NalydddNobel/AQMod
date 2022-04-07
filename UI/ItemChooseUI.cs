using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace Aequus.UI
{
    public sealed class ItemChooseUI : UIState
    {
        private List<Vector3> _drops;
        private UIPanel _panel;
        private Item[] _dropsInstance;
        private UIItemSlot[] _slots;

        public ItemChooseUI(List<Vector3> drops)
        {
            _drops = drops;
        }

        public override void OnInitialize()
        {
            _panel = new UIPanel();
            _panel.Left.Set(67, 0f);
            _panel.Top.Set(260, 0f);
            _panel.Width.Set(377f, 0f);
            int value = (int)(52f * 0.8f);
            _panel.Height.Set(value * (_drops.Count / 8 + 1) + 10, 0f);
            _panel.BackgroundColor = new Color(20, 20, 80, 200);

            _dropsInstance = new Item[_drops.Count];
            _slots = new UIItemSlot[_drops.Count];
            int yOffsetHack = 11 * (_drops.Count / 8);
            for (int i = 0; i < _drops.Count; i++)
            {
                _dropsInstance[i] = new Item();
                _dropsInstance[i].SetDefaults((int)_drops[i].X);
                _dropsInstance[i].stack = (int)(_drops[i].Y > _drops[i].Z ? _drops[i].Y : _drops[i].Z);
                _slots[i] = new UIItemSlot(_dropsInstance, i, ItemSlot.Context.ChatItem);
                int horizontal = i % 8;
                int vertical = i / 8;
                _slots[i].Left.Set(value * horizontal + 10, 0f);
                _slots[i].Top.Set(value * vertical - yOffsetHack, 0f);
                _panel.Append(_slots[i]);
            }

            Append(_panel);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Main.LocalPlayer.GetModPlayer<AequusPlayer>().itemDropChooser.IsTrackingSomething || !Main.playerInventory || Main.LocalPlayer.talkNPC >= 0)
            {
                UISystem.InventoryInterface.SetState(null);
            }
            // Prevents the ui slots from updating
            // base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (_panel.GetDimensions().ToRectangle().Contains(Main.mouseX, Main.mouseY))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].GetDimensions().ToRectangle().Contains(Main.mouseX, Main.mouseY))
                {
                    Main.HoverItem = _dropsInstance[i];
                    Main.instance.MouseText(_dropsInstance[i].Name, _dropsInstance[i].rare);
                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        SoundEngine.PlaySound(SoundID.Grab);
                        Main.LocalPlayer.QuickSpawnClonedItem(null, _dropsInstance[i], _dropsInstance[i].stack);
                        Main.projectile[Main.LocalPlayer.GetModPlayer<AequusPlayer>().itemDropChooser.ProjectileLocalIndex].Kill();
                        Main.LocalPlayer.GetModPlayer<AequusPlayer>().itemDropChooser.Clear();
                        UISystem.InventoryInterface.SetState(null);
                    }
                }
            }
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            float scale = Main.inventoryScale;
            Main.inventoryScale = 0.8f;
            base.DrawChildren(spriteBatch);
            Main.inventoryScale = scale;
        }
    }
}