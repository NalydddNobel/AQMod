using Terraria.DataStructures;
using Terraria.Utilities;

namespace AequusRemake.Systems.Fishing;

public struct FishDropInfo(Player player, FishingAttempt Attempt, int Item, int NPC, AdvancedPopupRequest Sonar, Vector2 SonarPosition, UnifiedRandom RNG = null) {
    public readonly FishingAttempt Attempt = Attempt;
    public int Item = Item;
    public int NPC = NPC;
    public AdvancedPopupRequest Sonar = Sonar;
    public Vector2 SonarPosition = SonarPosition;
    public Player Player = player;
    public UnifiedRandom RNG = RNG ?? Main.rand;

    public readonly void ApplyTo(ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition) {
        itemDrop = Item;
        npcSpawn = NPC;
        sonar = Sonar;
        sonarPosition = SonarPosition;
    }
}
