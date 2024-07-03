using Aequu2.Old.Content.DronePylons.NPCs;

namespace Aequu2.Old.Content.DronePylons;

public class GunnerDroneSlot : DroneSlot {
    public override int ItemValue => Item.buyPrice(gold: 10);
    public override int NPCType => ModContent.NPCType<GunnerDrone>();
}