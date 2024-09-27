using Aequus.Common.NPCs;
using Aequus.Common.Preferences;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Content.Monsters.Mimics;

public class DungeonChestMimic : ModNPC {
    public static readonly float Spawnrate = 0.01f;

    public override bool IsLoadingEnabled(Mod mod) {
        return GameplayConfig.Instance.DungeonMimics;
    }

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 6;
        NPCID.Sets.TrailingMode[Type] = 7;
        NPCID.Sets.DontDoHardmodeScaling[Type] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        bestiaryEntry.AddTags(
            new FlavorTextBestiaryInfoElement("CommonBestiaryFlavor.Mimic"),
            BestiaryBiomeTag.TheDungeon
        );
    }

    public override void SetDefaults() {
        NPC.width = 24;
        NPC.height = 24;
        NPC.aiStyle = NPCAIStyleID.Mimic;
        NPC.damage = 50;
        NPC.defense = 15;
        NPC.lifeMax = 250;
        NPC.HitSound = SoundID.NPCHit4;
        NPC.DeathSound = SoundID.NPCDeath6;
        NPC.value = 10000f;
        NPC.knockBackResist = 0.3f;
        NPC.rarity = 4;
        AnimationType = NPCID.IceMimic;
        Banner = Item.NPCtoBanner(NPCID.Mimic);
        BannerItem = Item.BannerToItem(Banner);
    }

    public virtual void AddRecipes() {
        BestiaryBuilder.MoveBestiaryEntry(this, NPCID.Mimic);
    }

    public override void HitEffect(NPC.HitInfo hit) {
        int dustId = DustID.Gold;
        if (NPC.life > 0) {
            for (int i = 0; i < hit.Damage / (double)NPC.lifeMax * 50.0; i++) {
                var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustId, 0f, 0f, 50, default(Color), 1.5f);
                d.velocity *= 2f;
                d.noGravity = true;
            }
            return;
        }

        for (int i = 0; i < 20; i++) {
            var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustId, 0f, 0f, 50, default(Color), 1.5f);
            d.velocity *= 2f;
            d.noGravity = true;
        }

        var source = NPC.GetSource_HitEffect();
        var gore = Gore.NewGoreDirect(source, new Vector2(NPC.position.X, NPC.position.Y - 10f), new Vector2(hit.HitDirection, 0f), Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1), NPC.scale);
        gore.velocity *= 0.3f;
        gore = Gore.NewGoreDirect(source, new Vector2(NPC.position.X, NPC.position.Y + NPC.height / 2 - 15f), new Vector2(hit.HitDirection, 0f), Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1), NPC.scale);
        gore.velocity *= 0.3f;
        gore = Gore.NewGoreDirect(source, new Vector2(NPC.position.X, NPC.position.Y + NPC.height - 20f), new Vector2(hit.HitDirection, 0f), Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1), NPC.scale);
        gore.velocity *= 0.3f;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.Common(ItemID.LockBox));
    }
}
