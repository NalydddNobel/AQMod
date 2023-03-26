
using Aequus.Common.Effects;
using Aequus.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Elites
{
    public abstract class ElitePrefix : ModType<NPC, ElitePrefix>
    {
        public abstract string EliteName { get; }

        public MiscShaderWrap Shader { get; private set; }
        public abstract Vector3 ShaderColor { get; }

        public int Type { get; private set; }

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
            Type = AequusNPC.Elites.Count;
            AequusNPC.Elites.Add(this);
            SetStaticDefaults();
        }

        protected virtual void SetupClone(ElitePrefix clone) {

        }

        public ElitePrefix ProperClone(NPC npc) {
            var clone = Clone(npc);
            SetupClone(clone);
            return clone;
        }

        protected override NPC CreateTemplateEntity() {
            return null;
        }

        public virtual void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {

        }
        public virtual void RecieveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {

        }
    }

    public class ArgonElite : ElitePrefix
    {
        public override string EliteName => "Gargantuan";
        public override Vector3 ShaderColor => new Vector3(1f, 0f, 0.5f);

        private bool _init;
        public float scaleIncrease;

        private void Init(NPC npc) {
            scaleIncrease = npc.scale + 0.5f;
        }

        public override void PostAI(NPC npc)
        {
            npc.StatSpeed() *= 0.8f;
            if (!_init) {
                Init(npc);
                int oldWidth = npc.width;
                int oldHeight = npc.height;
                int oldLifeMax = npc.lifeMax;
                npc.lifeMax = (int)(npc.lifeMax * 1.66f);
                npc.life += npc.lifeMax - oldLifeMax;
                npc.width = (int)(npc.width * scaleIncrease);
                npc.height = (int)(npc.height * scaleIncrease);
                npc.position.X -= (npc.width - oldWidth) / 2f;
                npc.position.Y -= (npc.height - oldHeight) / 2f;
                npc.knockBackResist = 0f;
                _init = true;
            }

            if (npc.scale < scaleIncrease) {
                npc.scale += 0.01f;
                if (npc.scale > scaleIncrease) {
                    npc.scale = scaleIncrease;
                }
            }
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
            binaryWriter.Write(scaleIncrease);
        }

        public override void RecieveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
            scaleIncrease = binaryReader.ReadSingle();
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
        public override string EliteName => "Mystic";
        public override Vector3 ShaderColor => new Vector3(0.5f, 0f, 1f);

        public ushort attackTimer;

        public override void PostAI(NPC npc)
        {
            if (!npc.HasValidTarget || !npc.HasPlayerTarget) {
                return;
            }

            var target = Main.player[npc.target];

            if (attackTimer > 360) {
                attackTimer = 0;
                npc.netUpdate = true;
                return;
            }

            if (attackTimer < 120) {
                if (!Collision.CanHitLine(npc.position, npc.width, npc.height, target.position, target.width, target.height)) {

                    if (attackTimer > 60) {
                        attackTimer--;
                    }
                    return;
                }
            }

            attackTimer++;

            if (attackTimer == 120 || attackTimer == 180) {
                npc.netUpdate = true;
            }

            if (attackTimer > 120 && attackTimer < 180) {
                npc.StatSpeed() *= Math.Max(1f - (attackTimer - 120) / 60f, 0.33f);
                var d = Dust.NewDustDirect(npc.Center - new Vector2(20f), 40, 40, DustID.CrystalPulse);
                d.velocity = (npc.Center - d.position) / 10f;
                d.noGravity = true;
            }
            if (attackTimer == 180) {
                for (int i = 0; i < 20; i++) {
                    var d = Dust.NewDustDirect(npc.Center - new Vector2(12f), 24, 24, DustID.CrystalPulse2);
                    d.velocity = npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(6f);
                    d.noGravity = true;
                    d.fadeIn = d.scale + Main.rand.NextFloat(1f);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient) {
                    var p = Projectile.NewProjectileDirect(
                        npc.GetSource_FromAI(),
                        npc.Center,
                        npc.DirectionTo(Main.player[npc.target].Center) * 9f,
                        ProjectileID.CrystalPulse,
                        14,
                        0f,
                        Main.myPlayer
                    );
                    p.friendly = false;
                    p.hostile = true;
                }
            }
            if (attackTimer > 180 && attackTimer < 190) {
                npc.velocity = npc.DirectionFrom(Main.player[npc.target].Center) * 4.5f;
            }
            if (attackTimer > 180 && attackTimer < 220 && Main.rand.NextBool(4)) {
                var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.CrystalPulse);
                d.velocity = -npc.velocity * 0.5f;
                d.noGravity = true;
            }
            base.PostAI(npc);
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
            binaryWriter.Write(attackTimer);
        }

        public override void RecieveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
            attackTimer = binaryReader.ReadUInt16();
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