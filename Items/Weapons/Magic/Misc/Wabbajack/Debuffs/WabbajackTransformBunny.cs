﻿using Aequus.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic.Misc.Wabbajack.Debuffs {
    public class WabbajackTransformBunny : ModBuff {
        public override string Texture => Aequus.PlaceholderDebuff;

        public override void Update(NPC npc, ref int buffIndex) {
            int i = npc.whoAmI;
            npc.Transform(NPCID.Bunny);
            npc.AddBuff(ModContent.BuffType<WabbajackEffectParticles>(), 180);
            npc.Aequus().noTakingDamage = 60;
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                Projectile.NewProjectile(npc.GetSource_Buff(buffIndex), npc.Bottom, Microsoft.Xna.Framework.Vector2.Zero,
                    ModContent.ProjectileType<WabbajackEffect>(), 0, 0f, Main.myPlayer);
            }
            npc.DelBuff(buffIndex);
            buffIndex--;
            if (Main.netMode == NetmodeID.Server) {
                NetMessage.SendData(MessageID.SyncNPC, number: i);
            }
        }
    }
}