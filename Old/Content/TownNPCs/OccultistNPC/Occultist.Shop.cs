using Aequus.Old.Content.Equipment.GrapplingHooks.EnemyGrappleHook;
using Aequus.Old.Content.Events.DemonSiege.Spawners;
using Aequus.Old.Content.Events.DemonSiege.Tiles;
using Aequus.Old.Content.Necromancy.Equipment.Accessories.SpiritKeg;

namespace Aequus.Old.Content.TownNPCs.OccultistNPC;

public partial class Occultist {
    public override void AddShops() {
        NPCShop shop = new NPCShop(Type);
        shop.Add(ModContent.ItemType<Meathook>());
        shop.Add(ModContent.ItemType<UnholyCore>());
        shop.Add(ModContent.ItemType<BottleOSpirits>());
        shop.Add(ItemID.WhoopieCushion, Condition.BloodMoon);
        shop.Add(ItemID.ShadowChest, Condition.DownedSkeletron);
        shop.Add(OblivionAltar.Item.Type, Condition.Hardmode);
        shop.Register();
    }
}
