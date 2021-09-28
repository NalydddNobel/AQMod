using AQMod.Assets.ItemOverlays;
using AQMod.Assets.PlayerLayers;
using AQMod.Assets.Textures;
using AQMod.Common;
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
                ItemOverlayLoader.Register(new Glowmask(GlowID.OmegaStariteMaskItem), item.type);
        }

        public override void SetDefaults()
        {
            int oldHead = item.headSlot;
            item.CloneDefaults(ItemID.SkeletronMask);
            item.headSlot = oldHead;
        }

        public override void UpdateVanity(Player player, EquipType type)
        {
            player.GetModPlayer<GraphicsPlayer>().specialHead = SpecialHeadID.OmegaStariteMask;
        }
    }
}