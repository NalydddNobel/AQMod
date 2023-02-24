using Aequus.Content.Carpentery;
using Aequus.Content.Carpentery.Photobook;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Items.Misc.Carpentry
{
    public class ShutterstockerClip : PhotoClipBase<RenderTarget2D>
    {
        public float worldXPercent;
        public float worldYPercent;
        public int time;
        public bool daytime;
        public TileMapCache tileMap;
        public bool reviewed;
        public List<Point> reviewNotesPoints;
        public string reviewNotesLanguageKey;

        public override bool HasTooltipTexture => base.HasTooltipTexture && !TooltipTexture.Value.IsContentLost;
        public override int OldItemID => ModContent.ItemType<ShutterstockerClipAmmo>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            tileMap = null;
            reviewNotesPoints = null;
            reviewNotesLanguageKey = null;
        }

        public override void SetClip(Rectangle area)
        {
            base.SetClip(area);
            tileMap = new TileMapCache(area);
            worldXPercent = area.X / (float)Main.maxTilesX;
            worldYPercent = area.Y / (float)Main.maxTilesY;
            daytime = Main.dayTime;
            time = (int)Main.time;
            reviewed = false;
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);

            tag["Reviewed"] = reviewed;

            if (tileMap == null)
                return;

            tag["Map"] = tileMap.SerializeData();
            tag["WorldXPercent"] = worldXPercent;
            tag["WorldYPercent"] = worldYPercent;
            tag["Daytime"] = daytime;
            tag["Time"] = time;
            if (reviewNotesLanguageKey != null)
                tag["ReviewNotesLanguageKey"] = reviewNotesLanguageKey;
            if (reviewNotesPoints != null && reviewNotesPoints.Count > 0)
            {
                tag["ReviewNotesPointsX"] = reviewNotesPoints.ConvertAll((p) => p.X);
                tag["ReviewNotesPointsY"] = reviewNotesPoints.ConvertAll((p) => p.Y);
            }
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            reviewed = tag.Get<bool>("Reviewed");
            if (tag.TryGet<TagCompound>("Map", out var val))
            {
                tileMap = TileMapCache.DeserializeData(val);
                worldXPercent = tag.Get<float>("WorldXPercent");
                worldYPercent = tag.Get<float>("WorldYPercent");
                daytime = tag.Get<bool>("Daytime");
                time = tag.Get<int>("Time");
                reviewNotesLanguageKey = tag.Get<string>("ReviewNotesLanguageKey");
                var x = tag.Get<List<int>>("ReviewNotesPointsX");
                var y = tag.Get<List<int>>("ReviewNotesPointsY");
                if (x != null && y != null && x.Count == y.Count)
                {
                    reviewNotesPoints = new List<Point>();
                    for (int i = 0; i < x.Count; i++)
                    {
                        reviewNotesPoints.Add(new Point(x[i], y[i]));
                    }
                }
            }
        }

        public override ModItem Clone(Item newEntity)
        {
            var clone = (ShutterstockerClip)base.Clone(newEntity);

            if (tileMap == null)
                return clone;

            if (TooltipTexture == null)
                TooltipTexture = new Ref<RenderTarget2D>();

            clone.tileMap = tileMap;
            clone.daytime = daytime;
            clone.time = time;
            clone.reviewed = reviewed;
            clone.timeCreatedSerialized = timeCreatedSerialized;
            clone.reviewNotesLanguageKey = reviewNotesLanguageKey;
            clone.reviewNotesPoints = reviewNotesPoints;
            return clone;
        }

        public override void OnMissingTooltipTexture()
        {
            if (TooltipTexture == null)
                TooltipTexture = new Ref<RenderTarget2D>();

            PhotoRenderer.RenderRequests.Add(PhotoData.FromLegacyClip(this));
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (tileMap == null)
                return;

            base.ModifyTooltips(tooltips);
            if (reviewed)
            {
                int index = 0;
                for (int i = 0; i < tooltips.Count; i++)
                {
                    if (tooltips[i].Name.StartsWith("Fake"))
                        index = i;
                }
                index = Math.Min(index + 1, tooltips.Count);
                tooltips.Insert(index, new TooltipLine(Mod, "Reviewed", TextHelper.GetTextValue($"ItemTooltip.{Name}.Reviewed")) { OverrideColor = Color.Lerp(Color.BlueViolet, Color.White, 0.5f), });
                if (reviewNotesLanguageKey != null)
                {
                    string text = Language.GetTextValue(reviewNotesLanguageKey);
                    if (text != reviewNotesLanguageKey)
                    {
                        tooltips.Insert(index, new TooltipLine(Mod, "ReviewNotes", $"* '{text}'") { OverrideColor = Color.Lerp(Color.OrangeRed, Color.White, 0.5f), });
                    }
                }
            }
            return;
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            writer.Write(tileMap == null);
            if (tileMap == null)
            {
                return;
            }

            var bb = new BitsByte(daytime, reviewed, !string.IsNullOrEmpty(reviewNotesLanguageKey), reviewNotesPoints != null && reviewNotesPoints.Count > 0);
            writer.Write(bb);
            writer.Write(time);
            writer.Write(worldXPercent);
            writer.Write(worldYPercent);
            writer.Write(tileMap.WorldID);
            writer.Write(tileMap.Area.X);
            writer.Write(tileMap.Area.Y);
            writer.Write(tileMap.Area.Width);
            writer.Write(tileMap.Area.Height);
            if (bb[2])
            {
                writer.Write(reviewNotesLanguageKey);
            }
            if (bb[3])
            {
                writer.Write(reviewNotesPoints.Count);
                for (int i = 0; i < reviewNotesPoints.Count; i++)
                {
                    writer.Write((ushort)reviewNotesPoints[i].X);
                    writer.Write((ushort)reviewNotesPoints[i].Y);
                }
            }
            var buffer = tileMap.CompressTileArray();
            writer.Write(buffer.Length);
            //Aequus.Instance.Logger.Debug(buffer.Length);
            writer.Write(buffer);
        }

        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
            if (reader.ReadBoolean())
                return;

            var bb = (BitsByte)reader.ReadByte();
            daytime = bb[0];
            reviewed = bb[1];
            time = reader.ReadInt32();
            worldXPercent = reader.ReadSingle();
            worldYPercent = reader.ReadSingle();
            int worldID = reader.ReadInt32();
            var rect = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            if (bb[2])
            {
                reviewNotesLanguageKey = reader.ReadString();
            }
            if (bb[3])
            {
                reviewNotesPoints = new List<Point>();
                int amt = reader.ReadInt32();
                for (int i = 0; i < amt; i++)
                {
                    reviewNotesPoints.Add(new Point(reader.ReadUInt16(), reader.ReadUInt16()));
                }
            }
            var bufferLength = reader.ReadInt32();
            tileMap = new TileMapCache(rect, TileMapCache.DecompressInfo(rect.Width, rect.Height, reader.ReadBytes(bufferLength)), worldID);
        }
    }

    public class ShutterstockerClipAmmo : ModItem
    {
        public static int AmmoID => ModContent.ItemType<ShutterstockerClipAmmo>();

        public override string Texture => ModContent.GetInstance<ShutterstockerClip>().Texture;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
            DisplayName.SetDefault("{$Mods.Aequus.ItemName.ShutterstockerClip}");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
            Item.maxStack = 9999;
            Item.ammo = AmmoID;
            Item.consumable = true;
        }
    }
}