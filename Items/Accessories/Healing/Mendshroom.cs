using Aequus.Buffs;
using Aequus.Items.Accessories.Summon.Sentry;
using Aequus.Particles.Dusts;
using Aequus.Projectiles.Misc;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Healing
{
    public sealed class Mendshroom : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);

            SantankInteractions.OnAI.Add(Type, SantankInteractions.ApplyEquipFunctional_AI);
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.rare = ItemDefaults.RarityCrabCrevice;
            Item.value = ItemDefaults.CrabCreviceValue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var stat = player.GetModPlayer<MendshroomPlayer>();
            stat.Add(circumference: 240f, regen: 60);
            if (stat.EffectActive)
            {
                if (player.Aequus()
                    .ProjectilesOwned_ConsiderProjectileIdentity(ModContent.ProjectileType<MendshroomAuraProj>()) <= 0)
                {
                    Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<MendshroomAuraProj>(),
                        0, 0f, player.whoAmI, player.Aequus().projectileIdentity + 1);
                }
                var v = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                var d = Dust.NewDustPerfect(player.Center + v * Main.rand.NextFloat(stat.diameter / 2f), ModContent.DustType<MonoDust>(), -v, 0, new Color(10, 100, 20, 25));
            }
        }
    }

    /// <summary>
    /// Used by <see cref="Mendshroom"/>
    /// </summary>
    public sealed class MendshroomPlayer : ModPlayer
    {
        public float diameter;
        public int regenerationToGive;
        public int idleTime;

        public int increasedRegen;

        public bool EffectActive => idleTime >= 30;

        public override void ResetEffects()
        {
            float lerpTo = 0f;
            if (diameter > 0f)
            {
                if (Player.velocity.Length() < 1f)
                {
                    idleTime++;
                }
                else
                {
                    idleTime = 0;
                }
                if (EffectActive)
                {
                    lerpTo = diameter;
                }
            }
            diameter = 0f;
            regenerationToGive = 0;
        }

        public override void clientClone(ModPlayer clientClone)
        {
            var clone = (MendshroomPlayer)clientClone;
            clone.diameter = diameter;
            clone.regenerationToGive = regenerationToGive;
            clone.idleTime = idleTime;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
        }

        public override void UpdateDead()
        {
            diameter = 0f;
            regenerationToGive = 0;
        }

        public override void UpdateLifeRegen()
        {
            Player.AddLifeRegen(increasedRegen);
            increasedRegen = 0;
        }

        public void Add(float circumference, int regen)
        {
            diameter = Math.Max(diameter, circumference);
            regenerationToGive += regen;
        }

        public void HealPlayers()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead && Main.player[i].Distance(Player.Center) < diameter / 2f)
                {
                    HealPlayer(i);
                }
            }
        }
        public void HealPlayer(int i)
        {
            var bungus = Main.player[i].GetModPlayer<MendshroomPlayer>();
            if (bungus.increasedRegen < regenerationToGive)
            {
                bungus.increasedRegen = regenerationToGive;
                Main.player[i].AddBuff(ModContent.BuffType<MendshroomBuff>(), 2);
            }
        }
    }
}