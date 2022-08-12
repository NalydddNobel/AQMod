using Aequus.Common;
using Aequus.Items.Tools.CarpenterTools;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.CarpenterBounties
{
    public abstract class CarpenterBounty : ModType
    {
        public int Type { get; internal set; }
        public string LanguageKey => $"Mods.{Mod.Name}.Bounty.{Name}";

        protected sealed override void Register()
        {
            CarpenterSystem.RegisterBounty(this);
        }

        public sealed override void SetupContent()
        {
            SetStaticDefaults();
        }

        public abstract Item ProvideBountyRewardItem();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <param name="message">Translated message that will either pop up in chat or be put into the carpenter's dialogue</param>
        /// <param name="carpenter">Carpenter town NPC that the player is talking to. If the player is not talking to a carpenter, this is set to null.</param>
        /// <returns></returns>
        public abstract bool CheckConditions(TileMapCache map, out string message, NPC carpenter = null);

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