using Aequus.Common.Items;
using Aequus.Common.Items.SentryChip;
using Aequus.Common.Utilities;
using Aequus.Content.Items.SentryChip;
using Aequus.CrossMod;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;

namespace Aequus.Content.Items.Accessories.CelesteTorus;

[Gen.AequusPlayer_ResetField<Item>("accStariteExpert")]
[Gen.AequusPlayer_ResetField<global::Aequus.Content.Items.Accessories.CelesteTorus.CelesteTorusDrawData>("stariteExpertDrawData")]
[Gen.AequusPlayer_ResetField<int>("stariteExpertStacks")]
[Gen.AequusPlayer_ResetField<int>("stariteExpertDye")]
public class CelesteTorus : ModItem, ItemHooks.IUpdateItemDye {
    #region Drawing
    const float Orb_Draw_Scale_Z = 0.07f;
    const float Orb_Draw_Position_Z = 0.03f;
    static readonly Color OrbColor = Color.White with { A = 230 } * 0.92f;

    private static void LegacyPlayerRenderer_DrawPlayers(On_LegacyPlayerRenderer.orig_DrawPlayers orig, LegacyPlayerRenderer self, Camera camera, IEnumerable<Player> players) {
        Main.spriteBatch.Begin_World(shader: true);
        DrawAction(players, Main.spriteBatch, DrawGlows);
        Main.spriteBatch.End();

        Main.spriteBatch.Begin_World(shader: true);
        DrawAction(players, Main.spriteBatch, DrawBackOrbs);
        Main.spriteBatch.End();

        orig(self, camera, players);

        Main.spriteBatch.Begin_World(shader: true);

        DrawAction(players, Main.spriteBatch, DrawFrontOrbs);

        Main.spriteBatch.End();
    }

    public delegate bool CullingRule(float scale);
    public delegate Color ColorRule(CelesteTorusDrawData drawData, Vector2 worldPosition, Vector3 threeDimensionalOffset, Vector2 drawPosition, float scale);

    delegate DrawData? GetOrbDrawData(int Dye, Vector2 position, float scale);

    static void DrawAction(IEnumerable<Player> players, SpriteBatch sb, GetOrbDrawData getDrawData) {
        foreach (Player player in players) {
            if (!player.active || !player.TryGetModPlayer(out AequusPlayer aequus) || aequus.stariteExpertDrawData.Offsets == null) {
                continue;
            }

            int dye = aequus.stariteExpertDye;
            float orbScale = 1f;

            if (player.ZoneSnow) {
                orbScale *= 0.98f;
            }

            CelesteTorusDrawData drawData = aequus.stariteExpertDrawData;

            foreach (Vector3 offset in drawData.Offsets) {
                float layerValue = ViewHelper.GetViewScale(orbScale, offset.Z * Orb_Draw_Scale_Z);
                Vector2 center = player.Center + new Vector2(offset.X, offset.Y);
                Vector2 drawPosition = ViewHelper.GetViewPoint(center, offset.Z * Orb_Draw_Position_Z) - Main.screenPosition;

                DrawData? drawResult = getDrawData(dye, drawPosition, layerValue);

                if (drawResult == null) {
                    continue;
                }

                if (drawResult.Value.shader != 0) {
                    GameShaders.Armor.Apply(drawResult.Value.shader, player, drawResult);
                }

                drawResult.Value.Draw(sb);
            }
        }
    }

    static DrawData? DrawGlows(int Dye, Vector2 position, float scale) {
        Color color = GetGlowColor();
        return new DrawData(AequusTextures.Bloom, position, null, color * 0.3f, 0f, AequusTextures.Bloom.Size() / 2f, scale, SpriteEffects.None) with { shader = Dye == 0 ? GameShaders.Armor.GetShaderIdFromItemId(ItemID.StardustDye) : Dye };
    }
    static Color GetGlowColor() {
        return Main.tenthAnniversaryWorld ? Color.HotPink with { A = 0 } : Color.White with { A = 0 } * 0.5f;
    }

    static DrawData? DrawBackOrbs(int Dye, Vector2 position, float scale) {
        if (scale >= 1f) {
            return default;
        }
        return new DrawData(AequusTextures.CelesteTorusProj, position, null, OrbColor * scale, 0f, AequusTextures.CelesteTorusProj.Size() / 2f, scale, SpriteEffects.None) with { shader = Dye };
    }

    static DrawData? DrawFrontOrbs(int Dye, Vector2 position, float scale) {
        if (scale < 1f) {
            return default;
        }

        float distance = Helper.DistanceRectangleVCircle(position + Main.screenPosition, AequusTextures.CelesteTorusProj.Width() / 2f * scale, Main.LocalPlayer.Hitbox);
        const float FadeDistance = 32f;
        float opacity = Math.Clamp(distance, 0f, 32f) / FadeDistance;
        return new DrawData(AequusTextures.CelesteTorusProj, position, null, OrbColor * (0.3f + opacity * 0.7f), 0f, AequusTextures.CelesteTorusProj.Size() / 2f, scale, SpriteEffects.None) with { shader = Dye };
    }
    #endregion

    public override void Load() {
        On_LegacyPlayerRenderer.DrawPlayers += LegacyPlayerRenderer_DrawPlayers;
    }

    public override void SetStaticDefaults() {
        SentryAccessoriesDatabase.Register<ApplyEquipFunctionalInteraction>(Type);
    }

    public override void SetDefaults() {
        Item.width = 20;
        Item.height = 20;
        Item.damage = 70;
        Item.knockBack = 2f;
        Item.accessory = true;
        Item.rare = ItemDefaults.RarityOmegaStarite;
        Item.value = ItemDefaults.ValueOmegaStarite;
        Item.expert = !ModSupportCommons.DoExpertDropsInClassicMode();
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        AequusPlayer aequus = player.GetModPlayer<AequusPlayer>();
        aequus.accStariteExpert = Item;
        aequus.stariteExpertStacks++;
        if (aequus.ProjectilesOwned(ModContent.ProjectileType<CelesteTorusProj>()) <= 0) {
            Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<CelesteTorusProj>(),
                0, 0f, player.whoAmI, player.Aequus().projectileIdentity + 1);
        }
    }

    void ItemHooks.IUpdateItemDye.UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
        player.Aequus().stariteExpertDye = dyeItem.dye;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        tooltips.RemoveCritChance();
    }
}