using Aequus.Graphics;
using Aequus.Graphics.RenderTargets;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Items.Tools.Camera
{
    public class ShutterstockerClip : PhotoClipBase<RenderTarget2D>
    {
        public float worldXPercent;
        public float worldYPercent;
        public int time;
        public bool daytime;
        public TileMapCache tileMap;
        public bool reviewed;

        public override bool HasTooltipTexture => base.HasTooltipTexture && !TooltipTexture.Value.IsContentLost;

        public override void SetDefaults()
        {
            base.SetDefaults();
            tileMap = null;
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
            return clone;
        }

        public override void OnMissingTooltipTexture()
        {
            if (TooltipTexture == null)
                TooltipTexture = new Ref<RenderTarget2D>();

            ShutterstockerSceneRenderer.RenderRequests.Add(this);
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
                tooltips.Insert(index + 1, new TooltipLine(Mod, "Reviewed", AequusText.GetText($"ItemTooltip.{Name}.Reviewed")) { OverrideColor = Color.Lerp(Color.BlueViolet, Color.White, 0.5f), });
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

            var bb = new BitsByte(daytime, reviewed);
            writer.Write(bb);
            writer.Write(time);
            writer.Write(worldXPercent);
            writer.Write(worldYPercent);
            writer.Write(tileMap.WorldID);
            writer.Write(tileMap.Area.X);
            writer.Write(tileMap.Area.Y);
            writer.Write(tileMap.Area.Width);
            writer.Write(tileMap.Area.Height);
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