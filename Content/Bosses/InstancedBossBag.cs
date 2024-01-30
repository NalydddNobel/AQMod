using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Localization;

namespace Aequus.Content.Bosses;

internal class InstancedBossBag : InstancedModItem {
    private readonly int InternalRarity;
    private readonly bool PreHardmode;
    private string _bossName;

    public InstancedBossBag(string name, int internalRarity, bool preHardmode = false) : base($"{name}Bag", $"{typeof(InstancedBossBag).NamespaceFilePath()}/TreasureBags/{name}Bag") {
        _bossName = name;
        InternalRarity = internalRarity;
        PreHardmode = preHardmode;
    }

    public override LocalizedText DisplayName => Language.GetText("Mods.Aequus.Items.TreasureBag.DisplayName").WithFormatArgs(Language.GetOrRegister($"Mods.Aequus.NPCs.{_bossName}.DisplayName"));
    public override LocalizedText Tooltip => Language.GetText("CommonItemTooltip.RightClickToOpen");

    public override void SetStaticDefaults() {
        ItemID.Sets.BossBag[Type] = true;
        ItemID.Sets.PreHardmodeLikeBossBag[Type] = PreHardmode;

        Item.ResearchUnlockCount = 3;
    }

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.consumable = true;
        Item.rare = InternalRarity;
        Item.maxStack = Item.CommonMaxStack;
        Item.expert = true;
    }

    public override bool CanRightClick() => true;

    public override Color? GetAlpha(Color lightColor) {
        return Color.Lerp(lightColor, Color.White, 0.4f);
    }

    public override void PostUpdate() {
        Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.4f);

        if (Item.timeSinceItemSpawned % 12 == 0) {
            var center = Item.Center + new Vector2(0f, Item.height * -0.1f);

            var direction = Main.rand.NextVector2CircularEdge(Item.width * 0.6f, Item.height * 0.6f);
            float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
            var velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);

            var d = Dust.NewDustPerfect(center + direction * distance, DustID.SilverFlame, velocity);
            d.scale = 0.5f;
            d.fadeIn = 1.1f;
            d.noGravity = true;
            d.noLight = true;
            d.alpha = 0;
        }
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        var texture = TextureAssets.Item[Item.type].Value;

        Rectangle frame;

        if (Main.itemAnimations[Item.type] != null) {
            frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
        }
        else {
            frame = texture.Frame();
        }

        var origin = frame.Size() / 2f;
        var offset = new Vector2(Item.width / 2 - origin.X, Item.height - frame.Height);
        var drawPos = Item.position - Main.screenPosition + origin + offset;

        float time = Main.GlobalTimeWrappedHourly;
        float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

        time %= 4f;
        time /= 2f;

        if (time >= 1f) {
            time = 2f - time;
        }

        time = time * 0.5f + 0.5f;

        for (float i = 0f; i < 1f; i += 0.25f) {
            float radians = (i + timer) * MathHelper.TwoPi;

            spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(90, 70, 255, 50), rotation, origin, scale, SpriteEffects.None, 0);
        }

        for (float i = 0f; i < 1f; i += 0.34f) {
            float radians = (i + timer) * MathHelper.TwoPi;

            spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(140, 120, 255, 77), rotation, origin, scale, SpriteEffects.None, 0);
        }

        return true;
    }

    //public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
    //    Texture2D texture = TextureAssets.Item[Item.type].Value;

    //    float time = Main.GlobalTimeWrappedHourly;
    //    float timer = Main.GameUpdateCount / 240f + time * 0.04f;

    //    time %= 4f;
    //    time /= 2f;

    //    if (time >= 1f) {
    //        time = 2f - time;
    //    }

    //    time = time * 0.5f + 0.5f;

    //    for (float i = 0f; i < 1f; i += 0.25f) {
    //        float radians = (i + timer) * MathHelper.TwoPi;

    //        spriteBatch.Draw(texture, position + new Vector2(0f, 8f).RotatedBy(radians) * time * Main.inventoryScale, frame, new Color(90, 70, 255, 50), 0f, origin, scale, SpriteEffects.None, 0);
    //    }

    //    for (float i = 0f; i < 1f; i += 0.34f) {
    //        float radians = (i + timer) * MathHelper.TwoPi;

    //        spriteBatch.Draw(texture, position + new Vector2(0f, 4f).RotatedBy(radians) * time * Main.inventoryScale, frame, new Color(140, 120, 255, 77), 0f, origin, scale, SpriteEffects.None, 0);
    //    }
    //    return true;
    //}
}