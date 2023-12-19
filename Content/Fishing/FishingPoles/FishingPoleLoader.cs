using Aequus.Common.Projectiles;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace Aequus.Content.Fishing.FishingPoles;

public class FishingPoleLoader : ModSystem {
    private static InstancedFishingPole _steampunkersRod;
    public static ModItem SteampunkersRod => _steampunkersRod;

    public override void Load() {
        _steampunkersRod = AddFishingPole("Steampunker", new(20, 10f, ItemRarityID.LightPurple, Item.buyPrice(gold: 20)), new(new Vector2(50f, -30f), (p) => Color.LightGray))
            .PreAI.Add((projectile) => {
                //Main.NewText($"{projectile.ai[0]}, {projectile.ai[1]}, {projectile.localAI[0]}, {projectile.localAI[1]}");
                if ((int)projectile.ai[0] == 0 && projectile.ai[1] < 0f && projectile.ai[1] > -30f && projectile.TryGetGlobalProjectile<AequusProjectile>(out var aequusProjectile)) {
                    aequusProjectile.itemData = 1;
                    Main.player[projectile.owner].GetModPlayer<AequusPlayer>().forceUseItem = true;
                }

                return true;
            })
            .OnKill.Add((projectile, timeLeft) => {
                if (projectile.TryGetGlobalProjectile<AequusProjectile>(out var aequusProjectile) && aequusProjectile.itemData == 1) {
                    Main.player[projectile.owner].GetModPlayer<AequusPlayer>().forceUseItem = true;
                }
            });

        InstancedFishingPole AddFishingPole(string name, InstancedFishingPole.FishingRodStats stats, InstancedFishingPole.FishingRodDrawInfo drawInfo) {
            var pole = new InstancedFishingPole(name, stats, drawInfo);
            Mod.AddContent(pole);
            var bobber = new InstancedFishingBobber(name);
            Mod.AddContent(bobber);

            pole._bobber = bobber;
            return pole;
        }
    }

    public override void SetStaticDefaults() {
        Main.RegisterItemAnimation(SteampunkersRod.Type, new DrawAnimationVertical(5, 4));
    }
}