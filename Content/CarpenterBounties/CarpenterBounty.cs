using Aequus.Items.Tools.Misc;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.CarpenterBounties
{
    public abstract class CarpenterBounty : ModType
    {
        public int Type { get; internal set; }

        protected sealed override void Register()
        {
            CarpenterSystem.RegisterBounty(this);
        }

        public sealed override void SetupContent()
        {
            SetStaticDefaults();
        }

        public abstract Item ProvideBountyRewardItem();

        public virtual bool CheckConditions(Rectangle rect, out string message)
        {
            message = "";
            return true;
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