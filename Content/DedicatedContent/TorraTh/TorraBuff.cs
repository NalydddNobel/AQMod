using Aequus.Common.Buffs;
using Aequus.Content.DedicatedContent.torrath;
using Terraria.ModLoader;

namespace Aequus.Content.DedicatedContent.torrath;

/// <summary>
/// Applied by <see cref="SwagLookingEye"/>
/// </summary>
public class TorraBuff : BasePetBuff {
    protected override int PetProj => ModContent.ProjectileType<TorraPet>();
}