using Aequus.Items.Misc;
using Aequus.Items.Tools.Camera;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.CarpenterBounties
{
    public abstract class CarpenterBounty : ModType
    {
        public class ConditionInfo
        {
            public TileMapCache Map { get; private set; }
            /// <summary>
            /// Carpenter town NPC that the player is talking to. If the player is not talking to a carpenter, this is set to null.
            /// </summary>
            public readonly NPC Carpenter;
            public readonly Rectangle SamplingArea;

            public int Width => Map.Width;
            public int Height => Map.Height;
            public TileDataCache this[int i, int j] { get => Map[i, j]; set => Map[i, j] = value; }
            public TileDataCache this[Point p] { get => Map[p]; set => Map[p] = value; }

            public ConditionInfo(TileMapCache map, NPC carpenter = null, Rectangle? worldArea = null)
            {
                Map = map;
                Carpenter = carpenter;
                SamplingArea = worldArea ?? new Rectangle(Main.maxTilesX / 2 - map.Width / 2, 200 + map.Height / 2, map.Width, map.Height);
            }

            public ConditionInfo(ShutterstockerClip clip, NPC carpenter = null) : this(clip.tileMap, carpenter,
                new Rectangle((int)(clip.worldXPercent * Main.maxTilesX), (int)(clip.worldYPercent * Main.maxTilesY), clip.tileMap.Width, clip.tileMap.Height))
            {
            }

            public void SwapWorldSample()
            {
                var map = new TileMapCache(SamplingArea);
                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        var p = new Point(SamplingArea.X + i, SamplingArea.Y + j);
                        Main.tile[p].Get<TileTypeData>() = this[i, j].Type;
                        Main.tile[p].Get<LiquidData>() = this[i, j].Liquid;
                        Main.tile[p].Get<TileWallWireStateData>() = this[i, j].Misc;
                        Main.tile[p].Get<WallTypeData>() = this[i, j].Wall;
                        Main.tile[p].Get<AequusTileData>() = this[i, j].Aequus;
                    }
                }
                Map = map;
            }
        }

        public int Type { get; internal set; }
        public string ModKey => $"Mods.{Mod.Name}.Bounty";
        public string LanguageKey => $"{ModKey}.{Name}";
        public string ReplyKey => $"{LanguageKey}.Reply";

        public string CommonLanguageKey => $"Mods.Aequus.Bounty.Common";
        public string CommonReplyKey => $"{CommonLanguageKey}.Reply";

        public string NotEnoughFurniture()
        {
            return Language.GetTextValue(CommonLanguageKey + ".NotEnoughFurniture");
        }

        protected sealed override void Register()
        {
            CarpenterSystem.RegisterBounty(this);
        }

        public override void SetupContent()
        {
            SetStaticDefaults();
        }

        public abstract Item ProvideBountyRewardItem();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="message">Translated message that will either pop up in chat or be put into the carpenter's dialogue</param>
        /// <returns></returns>
        public abstract bool CheckConditions(ConditionInfo info, out string message);

        public virtual void OnCompleteBounty(Player player, NPC npc)
        {
            var reward = ProvideBountyRewardItem();
            player.QuickSpawnClonedItem(npc.GetSource_GiftOrReward(), reward, reward.stack);
        }

        public virtual bool IsBountyAvailable()
        {
            return true;
        }

        public virtual CarpenterBountyItem ProvideBountyItem()
        {
            var i = new Item();
            i.SetDefaults(ModContent.ItemType<CarpenterBountyItem>());
            var bounty = i.ModItem<CarpenterBountyItem>();
            bounty.BountyFullName = FullName;
            return bounty;
        }
    }
}