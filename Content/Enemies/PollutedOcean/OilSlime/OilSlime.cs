using Aequus.Common.NPCs.Bestiary;
using Aequus.Content.Biomes.PollutedOcean;
using Aequus.Core.ContentGeneration;
using Aequus.DataSets;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Content.Enemies.PollutedOcean.OilSlime;

[AutoloadBanner]
[BestiaryBiome<PollutedOceanBiomeSurface>()]
[BestiaryBiome<PollutedOceanBiomeUnderground>()]
public class OilSlime : ModNPC {
    public static int ItemSpawnChance { get; set; } = 7;

    public int ItemDrop { get => (int)NPC.ai[1]; set => NPC.ai[1] = value; }

    #region Initialization
    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 2;
        NPCSets.ShimmerTransformToNPC[Type] = NPCID.ShimmerSlime;

        NPCSets.ImmuneToRegularBuffs[Type] = true;
        NPCSets.SpecificDebuffImmunity[Type][BuffID.OnFire] = false;
        NPCSets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = false;

        NPCDataSet.PushableByTypeId.Add(Type);
    }

    public override void SetDefaults() {
        NPC.width = 24;
        NPC.height = 18;
        NPC.aiStyle = NPCAIStyleID.Slime;
        NPC.damage = 15;
        NPC.defense = 15;
        NPC.lifeMax = 40;
        NPC.knockBackResist = 0.25f;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.alpha = 40;
        NPC.value = 25f;
        AnimationType = NPCID.BlueSlime;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry)
            .AddSpawn(BestiaryTimeTag.DayTime);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.FastClock, 100, 50));
        npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.SlimeStaff, 8000, 5600));
    }
    #endregion

    public override void AI() {
        if (ItemDrop == 0 && Main.netMode != NetmodeID.MultiplayerClient && NPC.value > 0f) {
            if (Main.rand.NextBool(ItemSpawnChance)) {
                ItemDrop = GetBodyItem();
                NPC.alpha += 40;
            }
            else {
                ItemDrop = -1;
            }
            NPC.netUpdate = true;
        }
    }

    public override bool? CanFallThroughPlatforms() {
        return NPC.HasValidTarget && Main.player[NPC.target].position.Y > NPC.Bottom.Y;
    }

    private static int GetBodyItem() {
        return Main.rand.Next(FishDataSet.Junk);
    }

    public override void HitEffect(NPC.HitInfo hit) {
        // Interpreted vanilla code for slime on hit effects.
        Color slimeColor = ContentSamples.NpcsByNetId[NPCID.BlackSlime].color;
        if (NPC.life > 0) {
            double dustCount = hit.Damage / (double)NPC.lifeMax * 100.0;
            for (int i = 0; i < dustCount; i++) {
                Terraria.Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TintableDust, hit.HitDirection, -1f, NPC.alpha, slimeColor);
            }
        }
        else {
            for (int i = 0; i < 50; i++) {
                Terraria.Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TintableDust, 2 * hit.HitDirection, -2f, NPC.alpha, slimeColor);
            }
        }
    }

    public override void OnKill() {
        if (ItemDrop > ItemID.None) {
            Item.NewItem(NPC.GetSource_Death(), NPC.Hitbox, ItemDrop);
        }
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) {
        if (Main.rand.NextBool(8)) {
            target.AddBuff(BuffID.Slow, 300);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        // Interpreted vanilla code for rendering items inside of slimes.
        if (!NPC.IsABestiaryIconDummy && ItemDrop > ItemID.None) {
            Color color = drawColor;
            int itemDrop = ItemDrop;
            float scale = 1f;
            float maxWidth = 22f * NPC.scale;
            float maxHeight = 18f * NPC.scale;

            Main.GetItemDrawFrame(itemDrop, out var texture, out var frame);
            float width = frame.Width;
            float height = frame.Height;

            if (width > maxWidth) {
                scale *= maxWidth / width;
                height *= scale;
            }
            if (height > maxHeight) {
                scale *= maxHeight / height;
            }

            float offsetX = -1f;
            float offsetY = 1f;
            int frameNumber = NPC.frame.Y / (TextureAssets.Npc[Type].Height() / Main.npcFrameCount[Type]);
            offsetY -= frameNumber;
            offsetX += frameNumber * 2;

            float rotation = 0.2f;
            rotation -= 0.3f * frameNumber;

            color = NPC.GetShimmerColor(color);
            spriteBatch.Draw(texture, new Vector2(NPC.Center.X - screenPos.X + offsetX, NPC.Center.Y - screenPos.Y + NPC.gfxOffY + offsetY), frame, color, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
        }

        return true;
    }
}
