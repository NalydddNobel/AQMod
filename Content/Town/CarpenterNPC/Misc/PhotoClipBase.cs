using Aequus.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;

namespace Aequus.Content.Town.CarpenterNPC.Misc {
    public abstract class PhotoClipBase<T> : ModItem where T : Texture2D
    {
        public Ref<T> TooltipTexture;
        public long timeCreatedSerialized;

        public abstract int OldItemID { get; }
        public virtual float BaseTooltipTextureScale => 1f;
        public float TooltipTextureScale
        {
            get
            {
                float scale = BaseTooltipTextureScale;
                int maxSize = (Main.screenWidth < Main.screenHeight ? Main.screenWidth : Main.screenHeight) / 2;
                int largestSide = (int)((TooltipTexture.Value.Width > TooltipTexture.Value.Height ? TooltipTexture.Value.Width : TooltipTexture.Value.Height) * scale);

                if (largestSide > maxSize)
                {
                    return maxSize / (float)largestSide * scale;
                }
                return scale;
            }
        }
        public DateTime TimeCreated { get => DateTime.FromBinary(timeCreatedSerialized); set => timeCreatedSerialized = value.ToBinary(); }
        public string TimeCreatedString => TimeCreated.ToString("MM/dd/yy h:mm tt", Language.ActiveCulture.CultureInfo);

        public bool AppendTimeCreatedTextToImage(float scale)
        {
            var measurement = FontAssets.MouseText.Value.MeasureString(TimeCreatedString);
            return TooltipTexture != null && TooltipTexture.Value != null && TooltipTexture.Value.Height * scale > measurement.Y * 2f && TooltipTexture.Value.Width * scale > measurement.X * 1.1f;
        }
        public virtual bool HasTooltipTexture => TooltipTexture != null && TooltipTexture.Value != null && !TooltipTexture.Value.IsDisposed;

        public virtual void SetClip(Rectangle area)
        {
            TimeCreated = DateTime.Now;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
        }

        public override ModItem Clone(Item newEntity)
        {
            var clone = (PhotoClipBase<T>)base.Clone(newEntity);
            if (TooltipTexture == null)
            {
                TooltipTexture = new Ref<T>();
            }
            clone.TooltipTexture = TooltipTexture;
            return clone;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Item.wet && Main.netMode != NetmodeID.MultiplayerClient && OldItemID > 0)
            {
                Item.active = false;
                var item2 = Item.NewItem(Item.GetSource_FromThis(), Item.Center, OldItemID, noGrabDelay: true);
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncItem, number: Item.whoAmI);
                }
            }
        }

        public abstract void OnMissingTooltipTexture();
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!HasTooltipTexture || Main.mouseRight && Main.mouseRightRelease)
            {
                OnMissingTooltipTexture();
                return;
            }

            int index = tooltips.GetIndex("Tooltip#");
            float scale = TooltipTextureScale;
            AequusItem.FitTooltipBackground(tooltips, (int)(TooltipTexture.Value.Width * scale), (int)(TooltipTexture.Value.Height * scale), index, "Image");
            if (!AppendTimeCreatedTextToImage(scale))
            {
                for (int i = 0; i < tooltips.Count; i++)
                {
                    if (tooltips[i].Name.StartsWith("Fake"))
                        index = i;
                }
                tooltips.Insert(index + 1, new TooltipLine(Mod, "Date", TimeCreatedString) { OverrideColor = Color.Lerp(Color.Yellow, Color.White, 0.5f), });
            }
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Aequus" && line.Name == "Image")
            {
                if (HasTooltipTexture)
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

                    var scale = line.BaseScale * TooltipTextureScale;
                    Main.spriteBatch.Draw(TooltipTexture.Value, new Vector2(line.X, line.Y) + new Vector2(8f), null, Color.Black, line.Rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    if (Aequus.HQ)
                    {
                        foreach (var c in Helper.CircularVector(8))
                        {
                            Main.spriteBatch.Draw(TooltipTexture.Value, new Vector2(line.X, line.Y) + new Vector2(8f) + c * 4f, null, Color.Black * 0.2f, line.Rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
                        }
                        foreach (var c in Helper.CircularVector(32))
                        {
                            Main.spriteBatch.Draw(TooltipTexture.Value, new Vector2(line.X, line.Y) + c * 2f, null, Color.Black, line.Rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
                        }
                    }
                    Main.spriteBatch.Draw(TooltipTexture.Value, new Vector2(line.X, line.Y), null, Color.White, line.Rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin_UI(immediate: false, useScissorRectangle: true);
                    if (AppendTimeCreatedTextToImage(Math.Min(scale.X, scale.Y)))
                    {
                        var text = TimeCreatedString;
                        var measurement = FontAssets.MouseText.Value.MeasureString(text);
                        var drawCoords = new Vector2(line.X + 4, line.Y + TooltipTexture.Value.Height * scale.Y - measurement.Y / 2f - 8f);

                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, text, drawCoords, Color.Lerp(Color.Yellow, Color.White, 0.5f), line.Rotation, Vector2.Zero, line.BaseScale);
                    }
                }
                return false;
            }
            return true;
        }

        public override void SaveData(TagCompound tag)
        {
            if (timeCreatedSerialized != 0)
                tag["TimeCreated"] = timeCreatedSerialized;
        }

        public override void LoadData(TagCompound tag)
        {
            timeCreatedSerialized = tag.Get<long>("TimeCreated");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(timeCreatedSerialized);
        }

        public override void NetReceive(BinaryReader reader)
        {
            timeCreatedSerialized = reader.ReadInt64();
        }

        public override bool CanBeConsumedAsAmmo(Item weapon, Player player)
        {
            return false;
        }
    }
}