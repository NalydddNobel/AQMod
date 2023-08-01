using Aequus.Common.Carpentry;
using Aequus.Common.UI;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Content.UI.BountyBoard;

public class BountyPostsUIElement : UIElement {
    public class PostInfo {
        public BuildChallenge challenge;
        public string DisplayName;
        public string Description;
        public int NPCId;
        public int NPCHeadId;
        public bool Unlocked;
        public float HoverAnimation;
        public bool Hovering;

        public PostInfo(BuildChallenge challenge) {
            this.challenge = challenge;
            DisplayName = challenge.GetDisplayName().Value;
            Description = challenge.GetDescription().Value;
            Unlocked = challenge.IsAvailable();
            NPCId = challenge.BountyNPCType;
            NPCHeadId = NPC.TypeToDefaultHeadIndex(NPCId);
        }
    }

    public List<PostInfo> posts;
    public BountyDetailsUIElement detailsUIElement;

    public BountyPostsUIElement(IEnumerable<BuildChallenge> challenges, BountyDetailsUIElement detailsUIElement) {
        this.detailsUIElement = detailsUIElement;
        posts = new();
        foreach (var challenge in challenges) {
            if (challenge != null) {
                posts.Add(new(challenge));
            }
        }
    }

    public override void OnInitialize() {
        Width.Set(0f, 0.35f);
        Height.Set(0f, 0.95f);
        Left.Set(0f, 0f);
        VAlign = 0.5f;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch) {
        if (!Main.LocalPlayer.TryGetModPlayer<CarpentryPlayer>(out var carpentryPlayer)) {
            return;
        }
        var dimensions = GetDimensions();
        Helper.DrawRectangle(dimensions.ToRectangle(), Color.Black * 0.3f);
        var font = FontAssets.MouseText.Value;
        int boxHeight = 60;
        for (int i = 0; i < posts.Count; i++) {
            Color textColor = new(222, 222, 255, 233);
            var post = posts[i];
            var boxRectangle = new Rectangle((int)dimensions.X, (int)dimensions.Y + i * boxHeight, (int)dimensions.Width, boxHeight);
            bool selected = post == detailsUIElement.PostInfo;
            bool currentQuest = post.challenge.Type == carpentryPlayer.SelectedBounty;
            bool completed = CarpentrySystem.CompletedBounties.ContainsChallenge(post.challenge);
            bool available = post.challenge.IsAvailable();
            bool reallyHovering = boxRectangle.Contains(Main.mouseX, Main.mouseY);
            if (reallyHovering && available) {
                if (!post.Hovering) {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                }
                post.Hovering = true;
            }
            else if (post.HoverAnimation >= 0.75f) {
                post.Hovering = false;
            }
            if (post.Hovering) {
                post.HoverAnimation += 0.15f;
                if (post.HoverAnimation > 1f) {
                    post.HoverAnimation = 1f;
                }
            }
            else {
                post.HoverAnimation -= 0.2f;
                if (post.HoverAnimation < 0f) {
                    post.HoverAnimation = 0f;
                }
            }
            if (reallyHovering && available && !completed) {
                if (Main.mouseLeft && Main.mouseLeftRelease) {
                    if (!selected) {
                        SoundEngine.PlaySound(SoundID.MenuOpen);
                        detailsUIElement.PostInfo = post;
                        carpentryPlayer.SelectedBounty = post.challenge.Type;
                    }
                    else {
                        SoundEngine.PlaySound(SoundID.MenuClose);
                        detailsUIElement.PostInfo = null;
                    }
                }
            }
            string name = post.DisplayName;
            float colorAnimation = selected ? post.HoverAnimation + 0.8f : post.HoverAnimation;
            var boxColor = AequusUI.InventoryBackColor * (colorAnimation * 0.5f + 1f);
            if (completed) {
                boxColor *= 0.5f;
                textColor *= 0.5f;
            }
            else if (!available) {
                boxColor *= 0.4f;
                name = "???";
            }
            boxColor.A = 255;
            var boxOutlineColor = (Color.White * colorAnimation) with { A = 255 };
            int boxOffsetX = (int)MathHelper.Lerp(0f, 50f, post.HoverAnimation) + 8;
            boxRectangle.X -= boxOffsetX;
            boxRectangle.Width += boxOffsetX;

            var textPosition = new Vector2(dimensions.X, dimensions.Y + boxHeight / 2f + i * boxHeight);
            textPosition.X -= boxOffsetX;
            for (int k = 0; k < 4; k++) {
                var offset = Vector2.One.RotatedBy(k * MathHelper.PiOver2) * 2f;
                Helper.DrawUIPanel(spriteBatch, TextureAssets.InventoryBack18.Value, boxRectangle with { X = boxRectangle.X+ (int)offset.X, Y = boxRectangle.Y + (int)offset.Y }, boxOutlineColor);
            }
            Helper.DrawUIPanel(spriteBatch, TextureAssets.InventoryBack18.Value, boxRectangle, boxColor);
            //Utils.DrawInvBG(spriteBatch, boxRectangle, boxColor);

            int npcHead = post.NPCHeadId;
            if (currentQuest) {
                textPosition.X += 16f;
                var bountyArrowTexture = AequusTextures.BountyUIArrow;
                spriteBatch.Draw(bountyArrowTexture, textPosition, null, (available || completed) ? Color.White : Color.Black, 0f, bountyArrowTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
                textPosition.X += 4f;
            }
            if (TextureAssets.NpcHead.IndexInRange(npcHead)) {
                textPosition.X += 20f;
                var npcHeadTexture = TextureAssets.NpcHead[npcHead].Value;
                spriteBatch.Draw(npcHeadTexture, textPosition, null, (available || completed) ? Color.White : Color.Black, 0f, npcHeadTexture.Size()/ 2f, 1f, SpriteEffects.None, 0f);
            }
            textPosition.X += 20f;
            if (available && !completed) {
                textPosition.Y -= post.HoverAnimation * 6f;
            }
            var textSize = Vector2.One;
            var textMeasurement = font.MeasureString(name);
            if (textMeasurement.X > boxRectangle.Width / 1.5f) {
                textSize *= boxRectangle.Width / 1.5f / textMeasurement.X;
            }
            ChatManager.DrawColorCodedString(spriteBatch, font, name, textPosition, textColor, 0f, new Vector2(0f, textMeasurement.Y / 2f), textSize);

            if (post.HoverAnimation > 0f && available && !completed) {
                string descriptionText = post.Description;
                var descriptionMeasurement = font.MeasureString(descriptionText) * 0.7f;
                var descriptionTextSize = Vector2.One * 0.7f;
                if (descriptionMeasurement.X > boxRectangle.Width / 1.3f) {
                    descriptionTextSize *= boxRectangle.Width / 1.3f / descriptionMeasurement.X;
                }
                ChatManager.DrawColorCodedString(spriteBatch, font, descriptionText, new Vector2(textPosition.X, boxRectangle.Y + boxHeight / 2f + 4f), textColor * post.HoverAnimation, 0f, Vector2.Zero, descriptionTextSize);
            }

            if (available) {
                var starTexture = AequusTextures.BountySelection;
                var starFrame = starTexture.Frame(verticalFrames: 2, frameY: completed ? 1 : 0);
                starFrame.Y -= 2;
                var starOrigin = starFrame.Size() / 2f;
                spriteBatch.Draw(starTexture, new Vector2(dimensions.X + dimensions.Width - starFrame.Width, boxRectangle.Y + starFrame.Height * 0.75f), starFrame, Color.White, 0f, starOrigin, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}