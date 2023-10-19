using Aequus.Common.Buffs;
using Aequus.Content.DedicatedContent.TorraTh;
using Terraria.ModLoader;

namespace Aequus.Content.DedicatedContent.TorraTh;

/// <summary>
/// Applied by <see cref="SwagLookingEye"/>
/// </summary>
public class TorraBuff : BasePetBuff {
    protected override int PetProj => ModContent.ProjectileType<TorraPet>();
}