using Aequus.Common.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic.StunGun;

public class StunGunDebuff : ModBuff {
    public override string Texture => AequusTextures.TemporaryDebuffIcon;

    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
    }

    public override void Update(NPC npc, ref int buffIndex) {
        npc.GetGlobalNPC<AequusNPC>().stunGun = true;
        npc.velocity *= 0.1f;
        npc.netOffset.X = Main.rand.NextFloat(-2f, 2f);

        var dustSpotFront = npc.Center + StunGun.GetVisualOffset(npc, StunGun.GetVisualTime(StunGun.VisualTimer, front: true));
        var dustSpotBack = npc.Center + StunGun.GetVisualOffset(npc, StunGun.GetVisualTime(StunGun.VisualTimer, front: false));
        float dustScale = StunGun.GetVisualScale(npc);
        int dustSize = (int)(5 * dustScale);

        if (npc.buffTime[buffIndex] <= 1) {
            for (int i = 0; i < 2; i++) {
                var d = Dust.NewDustPerfect(dustSpotFront + Main.rand.NextVector2Square(-dustSize, dustSize), DustID.Electric);
                d.velocity *= 2f;
                d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.2f);
                d.noGravity = true;
                
                d = Dust.NewDustPerfect(dustSpotBack + Main.rand.NextVector2Square(-dustSize, dustSize), DustID.Electric);
                d.velocity *= 0.5f;
                d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.2f);
                d.noGravity = true;
            }
        }
        if (Main.GameUpdateCount % 15 == 0) {
            var d = Dust.NewDustPerfect(dustSpotFront + Main.rand.NextVector2Square(-dustSize, dustSize), DustID.Electric, Scale: 0.6f * dustScale);
            d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.2f);
            d.noGravity = true;

            d = Dust.NewDustPerfect(dustSpotBack + Main.rand.NextVector2Square(-dustSize, dustSize), DustID.Electric, Scale: 0.6f * dustScale);
            d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.2f);
            d.noGravity = true;
        }
    }
}