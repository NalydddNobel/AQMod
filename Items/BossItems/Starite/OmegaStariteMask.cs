using AQMod.Items.DrawOverlays;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.BossItems.Starite
{
    [AutoloadEquip(EquipType.Head)]
    public class OmegaStariteMask : ModItem, IItemOverlaysWorldDraw
    {
        private static readonly BasicOverlay _overlay = new BasicOverlay(AQUtils.GetPath<OmegaStariteMask>("_Glow"));
        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => _overlay;

        //public override void SetStaticDefaults()
        //{
        //    if (!Main.dedServ)
        //    {
        //        AQMod.ArmorOverlays.AddHeadOverlay<OmegaStariteMask>(new EquipHeadOverlay(this.GetPath() + "_HeadGlow", new Color(200, 200, 200, 0)));
        //    }
        //}

        public override void SetDefaults()
        {
            int oldHead = Item.headSlot;
            Item.CloneDefaults(ItemID.SkeletronMask);
            Item.headSlot = oldHead;
        }
    }
}