using Aequus.UI.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.NPCs.Friendly
{
    public class PhysicistPet : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.HatOffsetY[Type] = 2;
            NPCID.Sets.SpawnsWithCustomName[Type] = true;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true,
            });
        }

        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.width = 16;
            NPC.height = 20;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 1f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int dustAmount = NPC.life > 0 ? 1 : 5;
            for (int k = 0; k < dustAmount; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric);
            }
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>()
            {
                "Quimble",
                "Pimble",
                "Nimble",
                "Zimble",

                "Spinzie",
                "Pinzie",
                "Zinzie",
                "Xinzie",

                "Squondle",
                "Mondle",
                "Chondle",
                "Wandle",

                "Squizzer",
                "Chizzer",
                "Whizzer",
                "Fizzer",
                "Zizzer",
                "Tizzer",

                "Skeebler",
                "Beebler",
                "Zeebler",
                "Xeebler",
                "Teebler",
                "Weebler",
                "Meebler",

                "Whibbler",
                "Blipper",
                "Bleeper",
                "Blooper",
                "Zipper",
                "Zooper",

                "Pooper",
            };
        }

        public override bool CanChat()
        {
            return true;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.92");
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                SoundEngine.PlaySound(RenameItemState.SqueakSound);
            }
        }

        public override string GetChat()
        {
            return Language.GetTextValue("Mods.Aequus.Chat.PhysicistPet." + Main.rand.Next(2));
        }

        public override void AI()
        {
            int npcOwnerIndex = NPC.FindFirstNPC(ModContent.NPCType<Physicist>());
            if (npcOwnerIndex == -1)
            {
                NPC.life = -1;
                NPC.HitEffect();
                NPC.active = false;
                return;
            }
            var npcOwner = Main.npc[npcOwnerIndex];
            var gotoLocation = npcOwner.Center + new Vector2(npcOwner.width * 1.5f * npcOwner.direction, 0f);

            NPC.direction = npcOwner.direction;
            NPC.spriteDirection = npcOwner.spriteDirection;
            var difference = gotoLocation - NPC.Center;
            if (difference.Length() < 30f)
            {
                if (NPC.velocity.Length() < 0.2f)
                {
                    NPC.velocity += Main.rand.NextVector2Unit() * 0.1f;
                }
                if (NPC.velocity.Length() > 4f)
                {
                    NPC.velocity *= 0.85f;
                }
                NPC.velocity *= 0.99f;
            }
            else
            {
                NPC.velocity += difference / 600f;
                if (difference.Length()  > 2000f)
                {
                    NPC.Center = npcOwner.Center;
                }
            }
            if (NPC.position.Y < npcOwner.position.Y)
            {
                NPC.velocity.X *= 0.98f;
                NPC.velocity.Y -= 0.02f;
                if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                {
                    NPC.velocity.X *= 0.95f;
                    NPC.velocity.Y -= 0.3f;
                }
            }
            var rect = NPC.getRect();
            if (npcOwner.getRect().Intersects(rect))
            {
                NPC.velocity += npcOwner.DirectionTo(NPC.Center).UnNaN() * 0.075f;
            }
            NPC.rotation = MathHelper.Clamp(NPC.velocity.X * 0.1f, -1f, 1f);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 6.0)
            {
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[Type])
                    NPC.frame.Y = 0;
                NPC.frameCounter = 0.0;
            }
        }

        public override bool NeedSaving()
        {
            return true;
        }

        public override void SaveData(TagCompound tag)
        {
            if (!string.IsNullOrEmpty(NPC.GivenName))
                tag["Name"] = NPC.GivenName;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet<string>("Name", out var value))
                NPC.GivenName = value;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var texture = TextureAssets.Npc[Type].Value;
            var drawCoords = NPC.Center - screenPos;
            var origin = NPC.frame.Size() / 2f;
            var spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, drawCoords, NPC.frame, drawColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, drawCoords, NPC.frame, Color.White, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
            return false;
        }
    }
}