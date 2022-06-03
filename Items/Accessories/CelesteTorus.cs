using Aequus.Common.Utilities;
using Aequus.Graphics;
using Aequus.Items.Accessories.Summon;
using Aequus.Projectiles.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class CelesteTorus : ModItem, Hooks.IUpdateItemDye
    {
        public struct RenderData
        {
            public Vector3 Rotation;
            public Vector2 Position;
            public float Radius;
            public float Scale;
            public int Dye;

            public Vector3[] TurnIntoRing()
            {
                var arr = new Vector3[5];
                for (int i = 0; i < 5; i++)
                {
                    arr[i] = CelesteTorusProj.GetRot(i, Rotation, Radius);
                }
                return arr;
            }
        }

        public const float DimensionZMultiplier = 0.03f;

        public static List<RenderData> RenderPoints { get; private set; }

        #region Hooks
        public override void Load()
        {
            RenderPoints = new List<RenderData>();
            On.Terraria.Main.DrawProjectiles += DrawBackOrbs;
            On.Terraria.Main.DrawPlayers_AfterProjectiles += DrawFrontOrbs;
        }

        private static void DrawBackOrbs(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
        {
            DrawOrbs(BackOrbsCulling);
            orig(self);
        }
        private static void DrawFrontOrbs(On.Terraria.Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            orig(self);
            DrawOrbs(FrontOrbsCulling);

            RenderPoints.Clear();
        }
        public static void DrawOrbs(Func<float, bool> rule)
        {
            if (RenderPoints.Count == 0)
            {
                return;
            }

            Main.instance.LoadProjectile(ModContent.ProjectileType<CelesteTorusProj>());
            var texture = TextureAssets.Projectile[ModContent.ProjectileType<CelesteTorusProj>()].Value;
            var frame = new Rectangle(0, 0, texture.Width, texture.Height);
            var orig = frame.Size() / 2f;
            foreach (var render in RenderPoints)
            {
                if (render.Dye != 0)
                {
                    CommonSpriteBatchBegins.GeneralEntities.BeginShader(Main.spriteBatch);
                }
                else
                {
                    CommonSpriteBatchBegins.GeneralEntities.Begin(Main.spriteBatch);
                }

                var arr = render.TurnIntoRing();
                for (int i = 0; i < arr.Length; i++)
                {
                    var v = arr[i];
                    float layerValue = OrthographicView.GetViewScale(1f, v.Z * DimensionZMultiplier);
                    if (rule(layerValue))
                    {
                        var center = render.Position + new Vector2(v.X, v.Y);
                        Main.spriteBatch.Draw(texture, OrthographicView.GetViewPoint(center, v.Z * DimensionZMultiplier) - Main.screenPosition, frame, Lighting.GetColor((int)(center.X / 16f), (int)(center.Y / 16f)), 0f, orig,
                            OrthographicView.GetViewScale(render.Scale, v.Z * DimensionZMultiplier), SpriteEffects.None, 0);
                    }
                }
                Main.spriteBatch.End();
            }
        }
        public static bool BackOrbsCulling(float scale)
        {
            return scale < 1f;
        }
        public static bool FrontOrbsCulling(float scale)
        {
            return scale >= 1f;
        }
        #endregion

        public override void SetStaticDefaults()
        {
            this.SetResearch(1);

            SantankInteractions.OnAI.Add(Type, SantankInteractions.ApplyEquipFunctional_AI);
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.damage = 70;
            Item.knockBack = 2f;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 4);
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CelesteTorusPlayer>().celesteTorus = Item;
            if (player.Aequus().ProjectilesOwned_ConsiderProjectileIdentity(ModContent.ProjectileType<CelesteTorusProj>()) <= 0)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<CelesteTorusProj>(),
                    0, 0f, player.whoAmI, player.Aequus().projectileIdentity + 1);
            }
        }

        void Hooks.IUpdateItemDye.UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            player.GetModPlayer<CelesteTorusPlayer>().cCelesteTorus = dyeItem.dye;
        }
    }

    public class CelesteTorusPlayer : ModPlayer
    {
        public Item celesteTorus;
        public int cCelesteTorus;

        public override void ResetEffects()
        {
            celesteTorus = null;
            cCelesteTorus = 0;
        }
    }
}