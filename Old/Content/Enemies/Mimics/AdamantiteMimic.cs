using Aequu2.Content.Configuration;
using Aequu2.Core.Entities.Bestiary;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using tModLoaderExtended.Terraria.ModLoader;

namespace Aequu2.Old.Content.Enemies.Mimics;

public class AdamantiteMimic : ModNPC, IPostPopulateItemDropDatabase, IAddRecipes {
    protected virtual int CloneNPC => NPCID.Mimic;
    protected virtual int DustType => DustID.Adamantite;
    protected virtual SpawnConditionBestiaryInfoElement Biome => BestiaryBiomeTag.Caverns;

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 6;
        NPCSets.TrailingMode[Type] = 7;
        NPCSets.DontDoHardmodeScaling[Type] = true;
        NPCSets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCSets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
        NPCSets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
        NPCSets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;

        if (!GameplayConfig.Instance.HardmodeMimics) {
            NPCSets.NPCBestiaryDrawOffset[Type] = new() {
                Hide = true,
            };
        }
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        bestiaryEntry.AddTags(
            new FlavorTextBestiaryInfoElement("CommonBestiaryFlavor.Mimic"),
            Biome
        );
    }

    public override void SetDefaults() {
        int cloneNPC = CloneNPC;
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
        AIType = cloneNPC;
        AnimationType = cloneNPC;
        Banner = Item.NPCtoBanner(cloneNPC);
        BannerItem = Item.BannerToItem(Banner);
    }

    public virtual void AddRecipes(Mod mod) {
        BestiaryBuilder.ReSort(this, CloneNPC);
    }

    public override void HitEffect(NPC.HitInfo hit) {
        int dustId = DustType;
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

        var source = NPC.GetSource_FromThis();
        var gore = Gore.NewGoreDirect(source, new Vector2(NPC.position.X, NPC.position.Y - 10f), new Vector2(hit.HitDirection, 0f), Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1), NPC.scale);
        gore.velocity *= 0.3f;
        gore = Gore.NewGoreDirect(source, new Vector2(NPC.position.X, NPC.position.Y + NPC.height / 2 - 15f), new Vector2(hit.HitDirection, 0f), Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1), NPC.scale);
        gore.velocity *= 0.3f;
        gore = Gore.NewGoreDirect(source, new Vector2(NPC.position.X, NPC.position.Y + NPC.height - 20f), new Vector2(hit.HitDirection, 0f), Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1), NPC.scale);
        gore.velocity *= 0.3f;
    }

    public override void AI() {
        NPC.ai[1] = 1f;
    }

    public override void FindFrame(int frameHeight) {
        NPC.frame.Y %= frameHeight * Main.npcFrameCount[Type];
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        var texture = TextureAssets.Npc[Type].Value;
        var frame = texture.Frame(verticalFrames: 6, frameY: NPC.frame.Y / NPC.frame.Height % 6);
        int trailLength = NPCSets.TrailCacheLength[NPC.type];
        var offset = NPC.Size / 2f + new Vector2(0f, -7f);
        var origin = frame.Size() / 2f;
        var spriteDirection = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        for (int i = 0; i < trailLength; i++) {
            if (i < trailLength - 1 && (NPC.oldPos[i] - NPC.oldPos[i + 1]).Length() < 1f) {
                continue;
            }

            float progress = 1f - i / (float)trailLength;

            spriteBatch.Draw(texture, (NPC.oldPos[i] - screenPos + offset).Floor(), frame,
                ExtendLight.Get(NPC.oldPos[i] + offset) * progress * 0.4f, NPC.rotation, origin, NPC.scale, spriteDirection, 0f);
        }
        spriteBatch.Draw(texture, (NPC.position - screenPos + offset).Floor(), frame,
            drawColor, NPC.rotation, origin, NPC.scale, spriteDirection, 0f);
        return false;
    }

    public virtual void PostPopulateItemDropDatabase(Mod mod, ItemDropDatabase database) {
        ExtendLoot.InheritDropRules(CloneNPC, Type, database);
        //if (ThoriumMod.Instance != null && ThoriumMod.TryGetItem("VanquisherMedal", out ModItem vanquisherMedal)) {
        //    database.RegisterToNPC(Type, ItemDropRule.Common(vanquisherMedal.Type));
        //}
    }
}