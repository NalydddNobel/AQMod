using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Aequus.Common.Utilities;
using Terraria.GameContent.Bestiary;
using Aequus.Common.NPCs;
using System;

namespace Aequus.NPCs.Monsters.ChestMimics;

public class ShadowMimic : ModNPC, IAddRecipes {
    public override string Texture => Aequus.NPCTexture(NPCID.Mimic);

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Mimic];
        NPCID.Sets.TrailingMode[Type] = 7;
        NPCID.Sets.DontDoHardmodeScaling[Type] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.ShadowFlame] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.ShadowCandle] = true;
    }

    public virtual void AddRecipes(Aequus aequus) {
        BestiaryBuilder.MoveBestiaryEntry(this, NPCID.Mimic);
    }

    public override void SetDefaults() {
        NPC.width = 24;
        NPC.height = 24;
        NPC.aiStyle = 25;
        NPC.damage = 80;
        NPC.defense = 30;
        NPC.lifeMax = 500;
        NPC.HitSound = SoundID.NPCHit4;
        NPC.DeathSound = SoundID.NPCDeath6;
        NPC.value = 100000f;
        NPC.knockBackResist = 0.3f;
        NPC.rarity = 4;
        AIType = NPCID.Mimic;
        AnimationType = NPCID.Mimic;
        Banner = Item.NPCtoBanner(NPCID.Mimic);
        BannerItem = Item.BannerToItem(Banner);
    }

    public override void HitEffect(NPC.HitInfo hit) {
        int dustId = DustID.Demonite;
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

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        bestiaryEntry.AddTags(
            new FlavorTextBestiaryInfoElement("CommonBestiaryFlavor.Mimic"),
            BestiaryBuilder.Underworld
        );
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(new Conditions.NotRemixSeed(), ItemDropRule.OneFromOptions(1, ItemID.DarkLance, ItemID.Sunfury, ItemID.FlowerofFire, ItemID.Flamelash, ItemID.HellwingBow));
    }

    public override void AI() {
        NPC.ai[3] = 3f;
        base.AI();
    }

    public override void FindFrame(int frameHeight) {
        NPC.ai[3] = 3f;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        var texture = TextureAssets.Npc[Type].Value;
        var frame = NPC.frame;
        frame.Y = Math.Max(frame.Y, NPC.frame.Height * Main.npcFrameCount[Type] / 3 * 2);

        int trailLength = NPCID.Sets.TrailCacheLength[NPC.type];
        var offset = NPC.Size / 2f + new Vector2(0f, -7f);
        var origin = frame.Size() / 2f;
        var spriteDirection = (-NPC.spriteDirection).ToSpriteEffect();
        for (int i = 0; i < trailLength; i++) {
            if (i < trailLength - 1 && (NPC.oldPos[i] - NPC.oldPos[i + 1]).Length() < 1f) {
                continue;
            }
            spriteBatch.Draw(texture, (NPC.oldPos[i] - screenPos + offset).Floor(), frame,
                (Color.BlueViolet with { A = 0 }) * Helper.CalcProgress(trailLength, i) * 0.2f, NPC.rotation, origin, NPC.scale, spriteDirection, 0f);
        }
        spriteBatch.Draw(texture, (NPC.position - screenPos + offset).Floor(), frame,
            drawColor, NPC.rotation, origin, NPC.scale, spriteDirection, 0f);
        return false;
    }
}