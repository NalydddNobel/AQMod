using Aequus.Common.CrossMod;
using Aequus.Common.Items;
using Aequus.Common.Items.SentryChip;
using Aequus.Common.Utilities;
using Aequus.Content.Items.SentryChip;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace Aequus.Items.Equipment.Accessories.Combat.Passive.CelesteTorus;

public class CelesteTorus : ModItem, ItemHooks.IUpdateItemDye {
    #region Drawing
    public const float DimensionZMultiplier = 0.03f;

    private static readonly List<CelesteTorusDrawData> DrawData = new();
    private static readonly List<CelesteTorusDrawData> DrawDataReversed = new();

    public static void AddDrawData(CelesteTorusDrawData drawData) {
        DrawData.Add(drawData);
        DrawDataReversed.Insert(0, drawData);
    }
    public static void ClearDrawData() {
        DrawData.Clear();
        DrawDataReversed.Clear();
    }

    public delegate bool CullingRule(float scale);
    public delegate Color ColorRule(CelesteTorusDrawData drawData, Vector2 worldPosition, Vector3 threeDimensionalOffset, Vector2 drawPosition, float scale);

    /// <summary>
    /// Do not run this method if "<see cref="DrawData"/>.Count == 0".
    /// </summary>
    /// <param name="players"></param>
    /// <param name="cullingRule"></param>
    /// <param name="colorRule"></param>
    /// <param name="texture"></param>
    /// <param name="frame"></param>
    /// <param name="origin"></param>
    /// <param name="DefaultDye"></param>
    /// <param name="GlobalScale"></param>
    /// <param name="End"></param>
    /// <param name="Reversed"></param>
    public static void DrawDrawData(IEnumerable<Player> players, CullingRule cullingRule, ColorRule colorRule, Texture2D texture, Rectangle frame, Vector2 origin, int DefaultDye = 0, float GlobalScale = 1f, bool End = false, bool Reversed = false) {
        float multiplierScaleZ = 0.07f;
        float multiplierPositionZ = 0.03f;
        int oldDye = -1;
        var drawDataList = !Reversed ? DrawData : DrawDataReversed;
        foreach (var drawData in drawDataList) {
            if (!players.Contains(drawData.Player)) {
                continue;
            }

            int dye = DefaultDye;
            if (drawData.Player.TryGetModPlayer<AequusPlayer>(out var aequus)) {
                dye = aequus.cCelesteTorus == 0 ? DefaultDye : aequus.cCelesteTorus;
            }
            if (oldDye != dye) {
                if (End) {
                    Main.spriteBatch.End();
                }

                if (dye != 0) {
                    Main.spriteBatch.Begin_World(shader: true);
                }
                else {
                    Main.spriteBatch.Begin_World(shader: false);
                }
                End = true;
            }
            float orbScale = drawData.Player.ZoneSnow ? 0.98f : 1f;
            for (int i = 0; i < drawData.OrbsCount; i++) {
                var v = drawData.GetVector3(i);
                float layerValue = ViewHelper.GetViewScale(orbScale, v.Z * multiplierScaleZ);
                if (cullingRule(layerValue)) {
                    var center = drawData.WorldPosition + new Vector2(v.X, v.Y);

                    var drawPosition = ViewHelper.GetViewPoint(center, v.Z * multiplierPositionZ) - Main.screenPosition;
                    DrawData dd = new(
                        texture,
                        drawPosition.Floor(),
                        frame,
                        colorRule(drawData, center, v, drawPosition, layerValue),
                        0f,
                        origin,
                        Math.Min(layerValue, 1.5f) * GlobalScale * drawData.Scale,
                        SpriteEffects.None,
                        0
                    );

                    if (dye != 0) {
                        GameShaders.Armor.Apply(dye, drawData.Player, dd);
                    }

                    dd.Draw(Main.spriteBatch);
                }
            }
        }
        if (End) {
            Main.spriteBatch.End();
        }
    }
    internal static void DrawGlows(IEnumerable<Player> players) {
        if (DrawData.Count == 0) {
            return;
        }

        var texture = AequusTextures.Bloom0;
        DrawDrawData(
            players,
            AlwaysTrueCullingRule,
            GlowColorRule,
            texture,
            texture.Bounds(),
            texture.Size() / 2f,
            DefaultDye: GameShaders.Armor.GetShaderIdFromItemId(ItemID.StardustDye),
            GlobalScale: 0.7f,
            End: false
        );
    }
    internal static void DrawOrbs(CullingRule cullingRule, bool Reversed, IEnumerable<Player> players) {
        if (DrawData.Count == 0) {
            return;
        }

        Main.instance.LoadProjectile(ModContent.ProjectileType<CelesteTorusProj>());
        var texture = TextureAssets.Projectile[ModContent.ProjectileType<CelesteTorusProj>()].Value;
        DrawDrawData(
            players,
            cullingRule,
            OrbsColorRule,
            texture,
            texture.Bounds,
            texture.Size() / 2f,
            DefaultDye: 0,
            End: false,
            Reversed: Reversed
        );
    }
    public static bool BackOrbsCullingRule(float scale) {
        return scale < 1f;
    }
    public static bool FrontOrbsCullingRule(float scale) {
        return scale >= 1f;
    }
    public static bool AlwaysTrueCullingRule(float scale) {
        return true;
    }
    public static Color OrbsColorRule(CelesteTorusDrawData drawData, Vector2 worldPosition, Vector3 threeDimensionalOffset, Vector2 drawPosition, float scale) {
        return Color.White;
    }
    public static Color GlowColorRule(CelesteTorusDrawData drawData, Vector2 worldPosition, Vector3 threeDimensionalOffset, Vector2 drawPosition, float scale) {
        return Main.tenthAnniversaryWorld ? Color.HotPink with { A = 0 } : Color.White with { A = 0 } * 0.5f;
    }
    #endregion

    public override void SetStaticDefaults() {
        SentryAccessoriesDatabase.Register<ApplyEquipFunctionalInteraction>(Type);
    }

    public override void Unload() {
        DrawData.Clear();
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
        var aequus = player.Aequus();
        if (aequus.accCelesteTorus != null)
            aequus.celesteTorusDamage++;
        aequus.accCelesteTorus = Item;
        if (aequus.ProjectilesOwned(ModContent.ProjectileType<CelesteTorusProj>()) <= 0) {
            Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<CelesteTorusProj>(),
                0, 0f, player.whoAmI, player.Aequus().projectileIdentity + 1);
        }
    }

    void ItemHooks.IUpdateItemDye.UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
        player.Aequus().cCelesteTorus = dyeItem.dye;
    }

    public override void ModifyWeaponDamage(Player player, ref StatModifier damage) {
        damage += player.Aequus().celesteTorusDamage - 1f;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        tooltips.RemoveCritChance();
    }
}