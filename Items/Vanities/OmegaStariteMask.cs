using AQMod.Common.ItemOverlays;
using AQMod.Common.PlayerLayers.ArmorOverlays;
using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Vanities
{
    [AutoloadEquip(EquipType.Head)]
    public class OmegaStariteMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                AQMod.ItemOverlays.Register(new LegacyGlowmaskOverlayData(AQUtils.GetPath(this) + "_Glow"), item.type);
                AQMod.ArmorOverlays.AddHeadOverlay<OmegaStariteMask>(new ArmorHeadOverlay(AQUtils.GetPath(this) + "_HeadGlow", new Color(200, 200, 200, 0)));
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