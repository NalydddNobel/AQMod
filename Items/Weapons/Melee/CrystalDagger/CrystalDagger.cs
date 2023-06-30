using Aequus;
using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.CrystalDagger;

public class CrystalDagger : ModItem, ItemHooks.IDrawSpecialItemDrop {
    public override void SetDefaults() {
        Item.width = 30;
        Item.height = 30;
        Item.DamageType = DamageClass.Melee;
        Item.damage = 12;
        Item.knockBack = 2f;
        Item.useAnimation = 10;
        Item.useTime = 10;
        Item.useStyle = ItemUseStyleID.Rapier;
        Item.UseSound = SoundID.Item1;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 75);
        Item.shootSpeed = 8f;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.shoot = ModContent.ProjectileType<CrystalDaggerProj>();
        Item.autoReuse = true;
    }

    public override Color? GetAlpha(Color lightColor) {
        return lightColor.MaxRGBA(120);
    }

    public override bool? UseItem(Player player) {
        Item.FixSwing(player);
        return null;
    }

    public override bool MeleePrefix() {
        return true;
    }

    public override bool CanUseItem(Player player) {
        return player.ownedProjectileCounts[Item.shoot] < 1;
    }

    public override void HoldItem(Player player) {
        player.AddBuff(ModContent.BuffType<CrystalDaggerBuff>(), 1, quiet: true);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
        velocity = velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f));
    }

    public override void Update(ref float gravity, ref float maxFallSpeed) {
        if (Item.timeSinceItemSpawned % 9 == 0) {
            var d = Dust.NewDustDirect(Item.position, Item.width, Item.height, DustID.FrostStaff);
            d.noGravity = true;
            d.fadeIn = d.scale + 0.1f;
            d.noLight = true;
        }

        Lighting.AddLight(Item.Center, (Color.Cyan * 0.2f).ToVector3());
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        Item.GetItemDrawData(out var frame);
        var itemDrawPosition = Item.Center - Main.screenPosition;
        Main.spriteBatch.Draw(
            AequusTextures.Bloom0,
            itemDrawPosition,
            null,
            Color.Blue with { A = 0 } * 0.25f,
            0f,
            AequusTextures.Bloom0.Size() / 2f,
            1f,
            SpriteEffects.None,
            0f
        );
        DrawCrystalDagger(spriteBatch, TextureAssets.Item[Type].Value, itemDrawPosition, frame, lightColor, rotation, frame.Size() / 2f, SpriteEffects.None, scale);
        return false;
    }

    public void OnPreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        Main.instance.LoadItem(Type);
        int npcFrame = npc.frame.Y / npc.frame.Height;
        float gfxOffY = npc.gfxOffY;
        gfxOffY += npcFrame switch {
            2 => -2,
            3 => -2,
            4 => -2,
            9 => -2,
            10 => -2,
            11 => -2,
            _ => 0,
        };

        var itemDrawPosition = npc.Center - screenPos + new Vector2(-12f * npc.spriteDirection, -npc.height / 2f - 8f).RotatedBy(npc.rotation) + new Vector2(0f, gfxOffY);
        var effect = npc.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;

        Main.spriteBatch.Draw(
            AequusTextures.Bloom0,
            itemDrawPosition,
            null,
            Color.Blue with { A = 0 } * 0.25f,
            0f,
            AequusTextures.Bloom0.Size() / 2f,
            1f,
            SpriteEffects.None,
            0f
        );
        var texture = TextureAssets.Item[Type].Value;
        Helper.GetItemDrawData(Type, out var frame);
        DrawCrystalDagger(spriteBatch, texture, itemDrawPosition, frame, drawColor, npc.rotation + MathHelper.PiOver2, frame.Size() / 2f, effect, 1f);
    }

    public static void DrawCrystalDagger(SpriteBatch spriteBatch, Texture2D texture, Vector2 where, Rectangle frame, Color drawColor, float rotation, Vector2 origin, SpriteEffects effects, float scale) {
        for (int i = 0; i < 4; i++) {
            spriteBatch.Draw(texture, where + (i * MathHelper.PiOver2).ToRotationVector2() * 2f, frame, new Color(10, 60, 100, 0), rotation, frame.Size() / 2f, scale, effects, 0f);
        }
        spriteBatch.Draw(texture, where, frame, drawColor.MaxRGBA(120), rotation, frame.Size() / 2f, scale, effects, 0f);
    }
}