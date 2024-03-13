using Aequus.Common.NPCs;
using Aequus.Common.NPCs.Bestiary;
using Aequus.Content.Biomes.PollutedOcean;
using Terraria.GameContent.Bestiary;

namespace Aequus.Content.Critters.Chromite;

[CritterCommons.AutoloadCatchItem(value: Item.silver, rarity: ItemRarityID.Blue, baitPower: 10)]
[ModBiomes(typeof(PollutedOceanBiomeUnderground))]
public class Chromite : ModNPC, CritterCommons.ICritter {
    public bool IsGolden => false;

    public override void SetDefaults() {
        NPC.width = 8;
        NPC.height = 8;
        NPC.lifeMax = 5;
        NPC.damage = 0;
        NPC.defense = 0;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.aiStyle = -1;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry);
    }

    public override void AI() {
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        return true;
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        if (NPC.life <= 0) {
            for (int i = 0; i < 10; i++) {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.AncientLight, hit.HitDirection * 2f, -2f);
            }
            return;
        }
        for (int i = 0; i < hit.Damage / NPC.lifeMax * 20; i++) {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.AncientLight, hit.HitDirection, -1f);
        }
    }
}
