using Aequus.Tiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Aequus.Content.CarpenterBounties
{
    public class PhotoInfo : TagSerializable
    {
        public TileMapCache tileMap;
        public ushort time;
        public Vector2 world;
        public long timeCreated;

        public int ID { get; internal set; }

        public bool DayTime => time < Main.dayLength;
        public bool Nighttime => time >= Main.dayLength;
        public int Time => time % (int)Main.dayLength;
        public DateTime TimeCreated { get => DateTime.FromBinary(timeCreated); set => timeCreated = value.ToBinary(); }
        public string TimeCreatedString => TimeCreated.ToString("MM/dd/yy h:mm tt", Language.ActiveCulture.CultureInfo);

        public TagCompound SerializeData()
        {
            return new TagCompound
            {
                ["Map"] = tileMap.SerializeData(),
                ["WorldX"] = world.X,
                ["WorldY"] = world.Y,
                ["Time"] = time,
                ["Date"] = timeCreated,
                ["ID"] = ID,
            };
        }

        public static PhotoInfo DeserializeData(TagCompound tag)
        {
            if (tag.TryGet<TagCompound>("Map", out var mapTag))
            {
                return new PhotoInfo
                {
                    tileMap = TileMapCache.DeserializeData(mapTag),
                    time = tag.Get<ushort>("Time"),
                    world = new Vector2(tag.Get<float>("WorldX"), tag.Get<float>("WorldY")),
                    timeCreated = tag.Get<long>("Date"),
                    ID = tag.Get<int>("ID"),
                };
            }
            return null;
        }
    }
}