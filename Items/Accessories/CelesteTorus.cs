using Aequus.Common.Utilities;
using Aequus.Common.Utilities.Drawing;
using Aequus.Content;
using Aequus.Content.CrossMod;
using Aequus.Projectiles.Misc.Friendly;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class CelesteTorus : ModItem, ItemHooks.IUpdateItemDye
    {
        public struct RenderData
        {
            public Player playerRef;
            public Vector3 Rotation;
            public Vector3? Rotation2;
            public Vector2 Position;
            public float Radius;
            public float Radius2;
            public float Scale;
            public bool Eight;

            public Vector3[] TurnIntoRing()
            {
                var arr = new Vector3[Rotation2 != null ? 8 + 5 : 5];
                for (int i = 0; i < 5; i++)
                {
                    arr[i] = CelesteTorusProj.GetRot(i, Rotation, Radius);
                }
                if (Rotation2 != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        arr[i + 5] = CelesteTorusProj.GetRot(i, Rotation2.Value, Radius * 2f, 8);
                    }
                }
                return arr;
            }
        }

        public const float DimensionZMultiplier = 0.03f;

        public static List<RenderData> RenderPoints { get; private set; }

        #region Hooks
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                RenderPoints = new List<RenderData>();
                On.Terraria.Main.DrawProjectiles += DrawBackOrbs;
                On.Terraria.Main.DrawItems += Main_DrawItems;
            }
        }

        private static void DrawBackOrbs(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
        {
            DrawOrbs(BackOrbsCulling);
            orig(self);
        }
        private static void Main_DrawItems(On.Terraria.Main.orig_DrawItems orig, Main self)
        {
            Main.spriteBatch.End();
            DrawOrbs(FrontOrbsCulling);
            Main.spriteBatch.Begin_World(shader: false);;

            RenderPoints.Clear();
            orig(self);
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
                int dye = (render.playerRef?.Aequus()?.cCelesteTorus).GetValueOrDefault(0);
                if (dye != 0)
                {
                    Main.spriteBatch.Begin_World(shader: true);
                }
                else
                {
                    Main.spriteBatch.Begin_World(shader: false);;
                }

                var arr = render.TurnIntoRing();
                for (int i = 0; i < arr.Length; i++)
                {
                    var v = arr[i];
                    float layerValue = ViewHelper.GetViewScale(1f, v.Z * DimensionZMultiplier);
                    if (rule(layerValue))
                    {
                        var center = render.Position + new Vector2(v.X, v.Y);
                        var dd = new DrawData(texture, ViewHelper.GetViewPoint(center, v.Z * DimensionZMultiplier) - Main.screenPosition, frame, Lighting.GetColor((int)(center.X / 16f), (int)(center.Y / 16f)).UseA(200), 0f, orig,
                            ViewHelper.GetViewScale(render.Scale, v.Z * DimensionZMultiplier), SpriteEffects.None, 0);
                        if (dye != 0)
                        {
                            GameShaders.Armor.Apply(dye, render.playerRef, dd);
                        }
                        dd.Draw(Main.spriteBatch);
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
            SacrificeTotal = 1;

            SentryAccessoriesDatabase.OnAI.Add(Type, SentryAccessoriesDatabase.ApplyEquipFunctional_AI);
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.damage = 70;
            Item.knockBack = 2f;
            Item.accessory = true;
            Item.rare = ItemDefaults.RarityOmegaStarite;
            Item.value = ItemDefaults.ValueOmegaStarite;
            Item.expert = !ModSupportSystem.DoExpertDropsInClassicMode();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            if (aequus.accCelesteTorus != null)
                aequus.celesteTorusDamage++;
            aequus.accCelesteTorus = Item;
            if (aequus.ProjectilesOwned(ModContent.ProjectileType<CelesteTorusProj>()) <= 0)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<CelesteTorusProj>(),
                    0, 0f, player.whoAmI, player.Aequus().projectileIdentity + 1);
            }
        }

        void ItemHooks.IUpdateItemDye.UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            player.Aequus().cCelesteTorus = dyeItem.dye;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage += player.Aequus().celesteTorusDamage - 1f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveCritChance();
        }

        public override void AddRecipes()
        {
            ModContent.GetInstance<TheReconstructionGlobalItem>().addEntry(Type);
        }
    }
}