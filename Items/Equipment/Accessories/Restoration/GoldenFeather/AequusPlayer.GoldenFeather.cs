using Aequus.Common.Players.Attributes;
using System;
using Terraria;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public int teamRegeneration;
    public int teamRespawnTimeFlat;

    private void PostUpdateEquips_TeamEffects_GoldenFeather(Player teammate, AequusPlayer teammateAequusPlayer) {
        Player.lifeRegen += teammateAequusPlayer.teamRegeneration;
        respawnTimeModifier = Math.Min(teammateAequusPlayer.teamRespawnTimeFlat, respawnTimeModifier);
    }
}