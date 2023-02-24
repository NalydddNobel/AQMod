using Aequus;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Misc.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Debuff
{
    public class LittleInferno : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = -1;
            Item.value = ItemDefaults.ValueDustDevil;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accLittleInferno++;
        }

        public override void AddRecipes()
        {
            return;
            CreateRecipe()
                .AddIngredient<Fluorescence>(10)
                .AddIngredient<AtmosphericEnergy>()
                .AddIngredient(ItemID.InfernoPotion)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public static void InfernoPotionEffect(Player player, Vector2 where, int npcWhoAmIBlacklist = -1)
        {
            Lighting.AddLight(where, 0.65f, 0.4f, 0.1f);
            if (player.whoAmI != Main.myPlayer)
            {
                return;
            }

            int stack = player.Aequus().accLittleInferno;
            int fireDebuff = BuffID.OnFire3;
            float minDistance = 200f;
            bool dealDamage = player.infernoCounter % Math.Max(60 / stack, 1) == 0;
            int damageDealt = 25 * stack;
            for (int i = 0; i < 200; i++)
            {
                NPC target = Main.npc[i];
                if (target.active && !target.friendly && target.damage > 0 && !target.justHit && !target.dontTakeDamage && !target.buffImmune[fireDebuff] && player.CanNPCBeHitByPlayerOrPlayerProjectile(target) && Vector2.Distance(where, target.Center) <= minDistance)
                {
                    if (dealDamage || !target.HasBuff(fireDebuff))
                    {
                        int oldDef = target.defense;
                        target.defense -= 20 + 10 * (stack - 1);
                        player.ApplyDamageToNPC(target, damageDealt, 0f, 0, crit: false);
                        target.justHit = true;
                        target.defense = oldDef;
                    }

                    if (i != npcWhoAmIBlacklist)
                    {
                        target.AddBuff(fireDebuff, 120 * stack);
                    }
                }
            }
            if (!player.hostile)
            {
                return;
            }
            for (int i = 0; i < 255; i++)
            {
                Player plr = Main.player[i];
                if (plr == player || !plr.active || plr.dead || !plr.hostile || plr.buffImmune[fireDebuff] || plr.team == player.team && player.team != 0 || !(Vector2.Distance(where, plr.Center) <= minDistance))
                {
                    continue;
                }
                if (plr.FindBuffIndex(fireDebuff) == -1)
                {
                    plr.AddBuff(fireDebuff, 120 * stack);
                }
                if (dealDamage)
                {
                    plr.Hurt(PlayerDeathReason.LegacyEmpty(), damageDealt, 0, pvp: true);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        NetMessage.SendPlayerHurt(i, PlayerDeathReason.ByOther(16), damageDealt, 0, critical: false, pvp: true, -1);
                    }
                }
            }
        }

        public static void DrawInfernoRings(Vector2 screenPosition, float opacity = 1f)
        {
            Main.instance.LoadFlameRing();

            var texture = TextureAssets.FlameRing.Value;
            var frame = texture.Frame(verticalFrames: 3, frameY: 0);
            var origin = frame.Size() / 2f;
            for (int i = 0; i < 3; i++)
            {
                float wave = ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 10f + i * (MathHelper.Pi / 1.5f)) + 1f) / 2f;
                Main.spriteBatch.Draw(texture, screenPosition, frame, new Color(255, 255, 255, 128) * opacity * wave, Main.GlobalTimeWrappedHourly * 1.5f + i, origin, 1f + wave * 0.1f - 0.05f, SpriteEffects.None, 0f);
            }
        }
    }
}