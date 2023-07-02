using Aequus.Common.Recipes;
using Aequus.Items.Pets.Drone;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools {
    public class PhaseMirror : ModItem
    {
        public List<(int, int, Dust)> dustEffectCache;

        public override void Load()
        {
            Terraria.On_Player.HasUnityPotion += Player_HasUnityPotion;
            Terraria.On_Player.TakeUnityPotion += Player_TakeUnityPotion;
        }

        private static void Player_TakeUnityPotion(Terraria.On_Player.orig_TakeUnityPotion orig, Player self)
        {
            if (self.HasItemInInvOrVoidBag(ModContent.ItemType<PhaseMirror>()))
                return;
            orig(self);
        }

        private static bool Player_HasUnityPotion(Terraria.On_Player.orig_HasUnityPotion orig, Player self)
        {
            if (self.HasItemInInvOrVoidBag(ModContent.ItemType<PhaseMirror>()))
                return true;
            return orig(self);
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.IceMirror);
            Item.rare = ItemRarityID.Green;
            Item.useTime = 64;
            Item.useAnimation = 64;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            int amt = player.itemAnimationMax % 8;
            if (player.itemAnimationMax != 64)
            {
                player.itemAnimation = 64;
                player.itemAnimationMax = 64;
                player.itemTime = player.itemAnimation;
                player.itemTimeMax = player.itemAnimationMax;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                UpdateMagicMirrorEffect(player);
            }

            if (player.itemTime == 0)
            {
                player.ApplyItemTime(Item);
            }
            else if (player.itemTime == player.itemTimeMax / 2)
            {
                player.grappling[0] = -1;
                player.grapCount = 0;
                dustEffectCache?.Clear();

                for (int p = 0; p < 1000; p++)
                {
                    if (Main.projectile[p].active && Main.projectile[p].owner == player.whoAmI && Main.projectile[p].aiStyle == 7)
                    {
                        Main.projectile[p].Kill();
                    }
                }

                player.Spawn(PlayerSpawnContext.RecallFromItem);
            }
        }

        public void UpdateMagicMirrorEffect(Player player)
        {
            var hitbox = player.getRect();
            hitbox.Inflate(12, 12);
            hitbox.Width += 1;
            hitbox.Height -= 6;

            //Main.NewText($"{player.itemAnimation} | {player.itemAnimationMax}");
            if (player.itemAnimation == player.itemAnimationMax)
            {
                dustEffectCache?.Clear();
            }

            int pieces = player.itemAnimationMax / 8;
            int curPiece = player.itemAnimation / 8;
            int y = 0;
            //Main.NewText($"{pieces}: {curPiece}");
            if (curPiece < 7 && curPiece >= 5)
            {
                float progression = (player.itemAnimation - 40) / 16f;
                y = (int)(hitbox.Height * (1f - progression)) - 6;
                player.Aequus().CustomDrawShadow = (float)Math.Pow(1f - progression, 2f);
            }
            else if (curPiece < 2)
            {
                float progression = 1f - player.itemAnimation / 16f;
                player.Aequus().CustomDrawShadow = progression > 0.9f ? null : (1f - progression);
                y = (int)(hitbox.Height * (1f - progression)) - 4;
            }
            if (curPiece > 4 || curPiece < 2)
            {
                if (dustEffectCache == null)
                {
                    dustEffectCache = new List<(int, int, Dust)>();
                }
                for (int i = 0; i < hitbox.Width; i += 3)
                {
                    var d = Dust.NewDustPerfect(hitbox.TopLeft() + new Vector2(i, y), DustID.MagicMirror, Vector2.Zero, 150, Color.White, 0.8f);
                    d.noGravity = true;
                    d.fadeIn = d.scale + 0.2f;
                    dustEffectCache.Add((i, y, d));
                }
            }
            for (int i = 0; i < dustEffectCache.Count; i++)
            {
                if (!dustEffectCache[i].Item3.active || dustEffectCache[i].Item3.type != DustID.MagicMirror)
                {
                    dustEffectCache.RemoveAt(i);
                    i--;
                }
                else
                {
                    dustEffectCache[i].Item3.position = hitbox.TopLeft() + new Vector2(dustEffectCache[i].Item1, dustEffectCache[i].Item2);
                }
            }
        }

        public override void AddRecipes()
        {
            AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<PersonalDronePack>());
        }
    }
}