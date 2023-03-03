using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Content.Town.CarpenterNPC.Quest.Bounties
{
    public class TestBountyUIState : AequusUIState
    {
        public string Texture => this.NamespacePath();
        public int HoverButtonAnimation => 5;
        public int IconPanelAnimation => 100;

        public int selectedBounty;
        public int iconPanel;
        public int swap;
        public int[] hoverButton;
        public List<CarpenterBounty> bounties;

        public override void OnInitialize()
        {
            OverrideSamplerState = SamplerState.LinearClamp;

            Width.Set(400, 0.1f);
            Height.Set(400, 0.1f);
            Top.Set(100, 0f);
            HAlign = 0.5f;
            VAlign = 0.15f;
            hoverButton = new int[2];
            bounties = new List<CarpenterBounty>();
            for (int i = 0; i < CarpenterSystem.BountyCount; i++)
            {
                if (CarpenterSystem.BountiesByID[i].IsBountyAvailable())
                {
                    bounties.Add(CarpenterSystem.BountiesByID[i]);
                }
            }

            var separator = new UIHorizontalSeparator();
            separator.Left.Set(20f, 0f);
            separator.Top.Set(160f, 0f);
            separator.Width.Set(-40f, 1f);
            separator.Color = new Color(116, 125, 202);
            Append(separator);
        }

        public override void Update(GameTime gameTime)
        {
            if (NotTalkingTo<Carpenter>())
            {
                Aequus.UserInterface.SetState(null);
                return;
            }
            var r = GetDimensions().ToRectangle();
            if (r.Contains(Main.mouseX, Main.mouseY))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            int wantedPanel = selectedBounty * IconPanelAnimation;
            if (iconPanel != wantedPanel)
            {
                iconPanel = (int)MathHelper.Lerp(iconPanel, wantedPanel, 0.1f);
                iconPanel += Math.Sign(wantedPanel - iconPanel);
            }
            base.Update(gameTime);
        }

        public void DrawButtons(SpriteBatch sb, Rectangle r)
        {
            var button = ModContent.Request<Texture2D>($"{Texture}/BountyUIArrow");
            var buttonOrigin = !button.IsLoaded ? Vector2.Zero : button.Value.Size() / 2f;
            float buttonScale = 0.66f;
            int width = (int)(button.Value.Width * buttonScale);
            int height = (int)(button.Value.Height * buttonScale);
            if (selectedBounty > 0)
            {
                sb.Draw(button.Value, new Vector2(r.X, r.Y + r.Height / 2), null, Color.White, 0f, buttonOrigin, buttonScale, SpriteEffects.None, 0f);
                if (new Rectangle(r.X - width / 2, r.Y + r.Height / 2 - height / 2, width, height).Contains(Main.mouseX, Main.mouseY))
                {
                    hoverButton[0] += 2;
                    Main.LocalPlayer.mouseInterface = true;
                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        selectedBounty--;
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    }
                }
                else if (hoverButton[0] > 0)
                {
                    hoverButton[0] = Math.Min(hoverButton[0] - 1, HoverButtonAnimation);
                }
                if (hoverButton[0] > 0)
                {
                    sb.Draw(button.Value, new Vector2(r.X, r.Y + r.Height / 2), null, Color.White.UseA(0) * 0.3f * Math.Min(hoverButton[0] / (float)HoverButtonAnimation, 1f), 0f, buttonOrigin, buttonScale, SpriteEffects.None, 0f);
                }
            }
            if (selectedBounty < bounties.Count - 1)
            {
                sb.Draw(button.Value, new Vector2(r.X + r.Width, r.Y + r.Height / 2), null, Color.White, 0f, buttonOrigin, buttonScale, SpriteEffects.FlipHorizontally, 0f);
                if (new Rectangle(r.X + r.Width - width / 2, r.Y + r.Height / 2 - height / 2, width, height).Contains(Main.mouseX, Main.mouseY))
                {
                    hoverButton[1] += 2;
                    Main.LocalPlayer.mouseInterface = true;
                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        selectedBounty++;
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    }
                }
                else if (hoverButton[1] > 0)
                {
                    hoverButton[1] = Math.Min(hoverButton[1] - 1, HoverButtonAnimation);
                }
                if (hoverButton[1] > 0)
                {
                    sb.Draw(button.Value, new Vector2(r.X + r.Width, r.Y + r.Height / 2), null, Color.White.UseA(0) * 0.3f * Math.Min(hoverButton[1] / (float)HoverButtonAnimation, 1f), 0f, buttonOrigin, buttonScale, SpriteEffects.FlipHorizontally, 0f);
                }
            }
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            var dim = GetDimensions();
            var r = dim.ToRectangle();
            //Utils.DrawInvBG(spriteBatch, r);
            var panelTexture = ModContent.Request<Texture2D>($"{Texture}/Panel").Value;
            Helper.DrawUIPanel(spriteBatch, panelTexture, r, Color.White);

            int wantedPanel = selectedBounty * IconPanelAnimation;
            var iconSidebars = ModContent.Request<Texture2D>($"{Aequus.VanillaTexture}UI/Achievement_Borders");
            var scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(r.X + 20, r.Y, 120, r.Height);
            spriteBatch.Begin_UI(useScissorRectangle: true);
            for (int i = iconPanel / IconPanelAnimation - 1; i <= iconPanel / IconPanelAnimation + 1; i++)
            {
                if (i < 0 || i >= bounties.Count)
                    continue;

                var icon = ModContent.Request<Texture2D>(bounties[i].Icon);
                spriteBatch.Draw(icon.Value, new Vector2(r.X + 20f - 120 * (iconPanel / (float)IconPanelAnimation - i), r.Y + 20f), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            spriteBatch.End();
            spriteBatch.Begin_UI(useScissorRectangle: false);
            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
            spriteBatch.Draw(iconSidebars.Value, new Vector2(r.X + 18f, r.Y + 18f), new Rectangle(0, 0, 20, iconSidebars.Value.Height - 10), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(iconSidebars.Value, new Vector2(r.X + 18f, r.Y + 78f), new Rectangle(0, 8, 20, iconSidebars.Value.Height - 8), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            spriteBatch.Draw(iconSidebars.Value, new Vector2(r.X + 120, r.Y + 18f), new Rectangle(iconSidebars.Value.Width - 20, 0, 20, iconSidebars.Value.Height - 10), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(iconSidebars.Value, new Vector2(r.X + 120, r.Y + 78f), new Rectangle(iconSidebars.Value.Width - 20, 8, 20, iconSidebars.Value.Height - 8), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            var font = FontAssets.DeathText.Value;
            var text = bounties[selectedBounty].DisplayName;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, text, new Vector2(r.X + r.Width / 2f, r.Y + 60),
                Color.White, 0f, font.MeasureString(text) / 2f, Vector2.One * 0.8f);

            float invScale = Main.inventoryScale;
            Main.inventoryScale = 1.33f;
            var item = ContentSamples.ItemsByType[bounties[selectedBounty].ItemReward];
            float itemSlotSize = 50f;
            ItemSlotRenderer.Draw(spriteBatch, item, new Vector2(r.X + r.Width / 2f - itemSlotSize / 2f - 10, r.Y + 100 - itemSlotSize / 2f), maxSize: (int)(itemSlotSize - 16));
            Main.inventoryScale = invScale;

            DrawButtons(spriteBatch, r);

            spriteBatch.Draw(panelTexture, new Vector2(r.X + r.Width + 5, r.Y), null, Color.White);
            spriteBatch.Draw(panelTexture, new Vector2(r.X + r.Width + 5, r.Y + 50), null, Color.White);
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
}