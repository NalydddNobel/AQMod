using Aequus.Common;
using Aequus.Items.Misc.Energies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.UI.Elements
{
    public class BestiaryNotebookElement : UIElement
    {
        public static bool InNotebookPage;

        public int npc;
        public BestiarySpawnInfo spawnInfo;
        public bool hovering;
        public bool setupPage;
        public bool installHooks;

        public UIList UIListParent => (UIList)Parent.Parent.Parent;

        public BestiaryNotebookElement(int npc, BestiarySpawnInfo spawnInfo)
        {
            this.npc = npc;
            this.spawnInfo = spawnInfo;
            hovering = false;
            setupPage = false;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            hovering = true;
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            hovering = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (!installHooks)
            {
                UIListParent.OnUpdate += UIListParent_OnUpdate;
                installHooks = false;
            }
            if (hovering)
            {
                string t = "Bestiary spawn options";
                Main.instance.MouseText(t);
            }
        }

        private void UIListParent_OnUpdate(UIElement affectedElement)
        {
            if (Parent == null)
            {
                return;
            }

            if (setupPage)
            {
                setupPage = false;

                if (InNotebookPage)
                {
                    var parent = UIListParent;
                    for (int i = 4; i < parent._items.Count; i++)
                    {
                        int old = parent._items.Count;
                        parent.Remove(parent._items[i]);
                        if (parent._items.Count != old)
                            i--;
                    }
                    var t = new UIText("Consumes " + spawnInfo.CrystalValue + " " + AequusText.ItemText<UltimateEnergy>() + " to summon", 0.8f);
                    t.HAlign = 0.5f;
                    t.VAlign = 0.5f;
                    t.IgnoresMouseInteraction = true;
                    var textHolder = new UIElement
                    {
                        Width = new StyleDimension(0f, 1f),
                        Height = new StyleDimension(24f, 0f)
                    };
                    textHolder.Append(t);
                    parent.Add(textHolder);
                }
                else
                {
                    var infoSpace = AequusHelpers.TryFindChildElement<UIBestiaryEntryInfoPage>(Main.BestiaryUI);
                    if (infoSpace == null)
                    {
                        Main.NewText("What.");
                        return;
                    }
                    infoSpace.FillInfoForEntry(null, default(ExtraBestiaryInfoPageInformation));
                    infoSpace.FillInfoForEntry(((UIBestiaryEntryButton)typeof(UIBestiaryTest).GetField("_selectedEntryButton", AequusHelpers.LetMeIn).GetValue(Main.BestiaryUI)).Entry, new ExtraBestiaryInfoPageInformation
                    {
                        BestiaryProgressReport = Main.BestiaryUI.GetUnlockProgress(),
                    });
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var c = GetDimensions();
            base.Draw(spriteBatch);
            var book = ModContent.Request<Texture2D>(Aequus.AssetsPath + "UI/BestiaryNotebook");
            var frame = book.Value.Frame(verticalFrames: 2, frameY: hovering ? 1 : 0);
            frame.Height -= 2;
            if (hovering)
            {
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    SoundEngine.PlaySound(SoundID.MenuOpen);
                    setupPage = true;
                    InNotebookPage = !InNotebookPage;
                }
                //for (int i = 0; i < UIListParent._items.Count; i++)
                //{
                //    var r = UIListParent._items[i].GetDimensions().ToRectangle();
                //    r.Inflate(4, 4);
                //    AequusHelpers.DrawRectangle(r, Color.White.UseA(0) * 0.2f);
                //}
            }
            spriteBatch.Draw(book.Value, c.Center(), frame, Color.White, 0f, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);
        }
    }
}