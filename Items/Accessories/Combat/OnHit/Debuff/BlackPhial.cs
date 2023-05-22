using Aequus;
using Aequus.Common;
using Aequus.Common.Net.Sounds;
using Aequus.Content.Biomes.RadonBiome.Tiles;
using Aequus.Items.Materials;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Combat.OnHit.Debuff {
    [AutoloadEquip(EquipType.Waist)]
    public class BlackPhial : ModItem {
        /// <summary>
        /// Default Value(s):
        /// <list type="bullet">
        /// <item><see cref="BuffID.Poisoned"/></item>
        /// <item><see cref="BuffID.OnFire3"/> (Hellfire)</item>
        /// <item><see cref="BuffID.Frostburn2"/> (Frostbite)</item>
        /// <item><see cref="BuffID.CursedInferno"/></item>
        /// <item><see cref="BuffID.Ichor"/></item>
        /// <item><see cref="BuffID.ShadowFlame"/></item>
        /// </list>
        /// </summary>
        public static readonly List<int> DebuffsAfflicted = new();

        public override void Load() {
            DebuffsAfflicted.AddRange(new[] {
                BuffID.Poisoned,
                BuffID.OnFire3,
                BuffID.Frostburn2,
                BuffID.CursedInferno,
                BuffID.Ichor,
                BuffID.ShadowFlame,
            });
        }

        public override void Unload() {
            DebuffsAfflicted.Clear();
        }

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory(14, 20);
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            var aequus = player.Aequus();
            aequus.accBlackPhial++;
            aequus.DebuffsInfliction.OverallTimeMultiplier += 0.5f;
        }

        public static void OnHitEffects(AequusPlayer aequus, Entity target, int damage, float knockback, bool crit) {
            int buffCount = 0;
            var entity = new EntityCommons(target);
            for (int i = 0; i < entity.maxBuffs; i++) {
                if (entity.buffType[i] > 0 && Main.debuff[entity.buffType[i]]) {
                    buffCount++;
                }
            }
            if (Main.rand.NextBool(Math.Max(4 / aequus.accBlackPhial + aequus.cdBlackPhial / 5 + buffCount * 2, 1))) {
                var buffsToInflict = new List<int>(DebuffsAfflicted);
                for (int i = 0; i < NPC.maxBuffs; i++) {
                    if (entity.buffTime[i] > 0 && entity.buffType[i] > 0 && buffsToInflict.Contains(entity.buffType[i])) {
                        buffsToInflict.Remove(entity.buffType[i]);
                    }
                }
                if (buffsToInflict.Count <= 0)
                    return;

                int buff = Main.rand.Next(buffsToInflict);
                if (!entity.ImmuneToBuff(buff)) {
                    aequus.cdBlackPhial += 30 / aequus.accBlackPhial;
                    entity.AddBuff(buff, 150);
                    ModContent.GetInstance<BlackPhialSound>().Play(target.Center);
                    var size = target.Size;
                    int amt = (int)(size.Length() / 4f);
                    for (int i = 0; i < amt; i++) {
                        var v = Main.rand.NextVector2CircularEdge(size.X, size.Y) * 0.5f;
                        var d = Dust.NewDustPerfect(target.Center + v, DustID.BatScepter, Vector2.Normalize(-v) * Main.rand.NextFloat(4f) + target.velocity, Scale: Main.rand.NextFloat(1f, 1.6f));
                        d.fadeIn = d.scale + 0.5f;
                        d.noGravity = true;
                    }
                }
            }
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<PossessedShard>(3)
                .AddIngredient<RadonMoss>(16)
                .AddIngredient(ItemID.SoulofNight, 8)
                .Register();
        }
    }
}