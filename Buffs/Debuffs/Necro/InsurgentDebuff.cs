using Aequus.Content.Necromancy;
using Aequus.Projectiles.Summon.Necro;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs.Necro
{
    public class InsurgentDebuff : NecromancyDebuff
    {
        public override string Texture => Aequus.Debuff;
        public override float Tier => 4f;

        public override void Update(NPC npc, ref int buffIndex)
        {
            int damageOverTime = 15;
            if (npc.life < 200)
            {
                damageOverTime = 200;
            }
            else if (npc.life < 500)
            {
                damageOverTime = 25;
            }
            var zombie = npc.GetGlobalNPC<NecromancyNPC>();
            zombie.zombieDrain = damageOverTime * AequusHelpers.NPCREGEN;
            zombie.DebuffTier(Tier);
            zombie.RenderLayer(GhostOutlineRenderer.IDs.Insurgent);

            if (Main.myPlayer == zombie.zombieOwner && Main.rand.NextBool(60))
            {
                var v = Main.rand.NextVector2Unit();
                var p = Projectile.NewProjectileDirect(npc.GetSource_Buff(buffIndex), npc.Center + v * (npc.Size.Length() / 2f), v * 10f,
                    ModContent.ProjectileType<InsurgentBolt>(), 1, 0f, zombie.zombieOwner, ai1: npc.whoAmI);
            }
        }
    }
}