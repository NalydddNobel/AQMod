﻿using Aequus.Buffs.Debuffs;
using Aequus.Content.Necromancy;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon.Necro
{
    public class GhostHitbox : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public int NPC => (int)Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = NecromancyDamageClass.Instance;
            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = 120;
            Projectile.ignoreWater = true;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            //AequusHelpers.dustDebug(Projectile.getRect());
            int npc = (int)Projectile.ai[0];
            if (Main.npc[npc].active)
            {
                Projectile.timeLeft = 2;
            }
            int size = (int)Projectile.ai[1];
            Projectile.position = Main.npc[npc].position - new Vector2(size / 2f);
            Projectile.width = Main.npc[npc].width + size;
            Projectile.height = Main.npc[npc].height + size;
            Projectile.wet = Main.npc[npc].wet;
            Projectile.lavaWet = Main.npc[npc].lavaWet;
            Projectile.honeyWet = Main.npc[npc].honeyWet;
            Projectile.velocity = Vector2.Normalize(Main.npc[npc].velocity) * 0.1f;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return NPCLoader.CanHitNPC(Main.npc[NPC], target);
        }

        public override bool CanHitPlayer(Player target)
        {
            int c = CooldownSlot;
            var value = NPCLoader.CanHitPlayer(Main.npc[NPC], target, ref c);
            CooldownSlot = c;
            return value;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.player[Projectile.owner].Aequus().accPandorasBox > 0)
            {
                var zombie = target.GetGlobalNPC<NecromancyNPC>();
                zombie.pandoraBox = Math.Max(zombie.pandoraBox, Main.player[Projectile.owner].Aequus().accPandorasBox + 1);
                target.AddBuff(ModContent.BuffType<PandorasCurse>(), 300);
            }
            target.AddBuff(ModContent.BuffType<SoulStolen>(), 1200);
        }
    }
}