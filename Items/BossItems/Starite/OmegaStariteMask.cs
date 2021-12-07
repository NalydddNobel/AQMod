using AQMod.Assets.LegacyItemOverlays;
using AQMod.Common.Graphics.PlayerEquips;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.BossItems.Starite
{
    [AutoloadEquip(EquipType.Head)]
    public class OmegaStariteMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                AQMod.ItemOverlays.Register(new GlowmaskOverlay(this.GetPath() + "_Glow"), item.type);
                AQMod.ArmorOverlays.AddHeadOverlay<OmegaStariteMask>(new EquipHeadOverlay(this.GetPath() + "_HeadGlow", new Color(200, 200, 200, 0)));
            }
        }

        public override void SetDefaults()
        {
            int oldHead = item.headSlot;
            item.CloneDefaults(ItemID.SkeletronMask);
            item.headSlot = oldHead;
        }
    }
}