using Aequus.Common.Effects;
using Aequus.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Elites
{
    public abstract class ElitePrefix : ModType
    {
        public abstract string EliteName { get; }

        public MiscShaderWrap Shader { get; private set; }
        public abstract Vector3 ShaderColor { get; }

        public virtual bool PreAI(NPC npc)
        {
            return true;
        }
        public virtual void AI(NPC npc)
        {
        }
        public virtual void PostAI(NPC npc)
        {
        }

        public virtual bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin_World(shader: true);
            Shader.Apply(null);
            return true;
        }

        public virtual void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin_World(shader: false);
        }

        protected sealed override void Register()
        {
        }

        public sealed override void SetupContent()
        {
            if (!Main.dedServ)
            {
                Shader = new MiscShaderWrap($"{this.NamespacePath()}/MossEnemyShader", $"{Name}", "MossEnemyShaderPass", loadStatics: true);
                Shader.UseColor(ShaderColor);
                Shader.UseImage1(ModContent.Request<Texture2D>($"{Aequus.VanillaTexture}Misc/noise"));
            }
            AequusNPC.registeredPrefixes.Add(this);
            SetStaticDefaults();
        }
    }

    public class ArgonElite : ElitePrefix
    {
        public override string EliteName => "Mushwalking";
        public override Vector3 ShaderColor => new Vector3(1f, 0f, 0.5f);

        public override void PostAI(NPC npc)
        {
            //if (Main.netMode != NetmodeID.MultiplayerClient && Main.GameUpdateCount % 20 == 0)
            //{
            //    Projectile.NewProjectile(
            //        npc.GetSource_FromThis(),
            //        npc.Bottom,
            //        Utils.SafeNormalize(npc.velocity, Vector2.Zero),
            //        ProjectileID.Mushroom,
            //        0,
            //        0f,
            //        Main.myPlayer);
            //}
        }
    }

    public class KryptonElite : ElitePrefix
    {
        public override string EliteName => "Mending";
        public override Vector3 ShaderColor => new Vector3(0.5f, 1f, 0f);

        public override void PostAI(NPC npc)
        {
            base.PostAI(npc);
        }
    }

    public class NeonElite : ElitePrefix
    {
        public override string EliteName => "Silencing";
        public override Vector3 ShaderColor => new Vector3(0.5f, 0f, 1f);

        public override void PostAI(NPC npc)
        {
            base.PostAI(npc);
        }
    }

    public class XenonElite : ElitePrefix
    {
        public override string EliteName => "Collaborating";
        public override Vector3 ShaderColor => new Vector3(0f, 0.5f, 1f);

        public override void PostAI(NPC npc)
        {
            base.PostAI(npc);
        }
    }
}