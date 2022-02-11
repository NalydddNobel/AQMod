using AQMod.Content.Players;
using AQMod.Items.DrawOverlays;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class OmegaStariteMask : ModItem, IItemOverlaysWorldDraw
    {
        private static readonly GlowmaskOverlay _overlay = new GlowmaskOverlay(AQUtils.GetPath<OmegaStariteMask>("_Glow"));
        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => _overlay;

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
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