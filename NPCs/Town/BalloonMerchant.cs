using AQMod.Common;
using AQMod.Content;
using AQMod.Content.WorldEvents.GaleStreams;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.NPCs.Town
{
    [AutoloadHead()]
    public class BalloonMerchant : ModNPC
    {
        private int _oldSpriteDirection;
        private int _balloonFrameCounter;
        private int _balloonFrame;
        private int _balloonColor;
        private bool _init;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 25;
            NPCID.Sets.ExtraFramesCount[npc.type] = 9;
            NPCID.Sets.AttackFrameCount[npc.type] = 4;
            NPCID.Sets.DangerDetectRange[npc.type] = 700;
            NPCID.Sets.AttackType[npc.type] = 0;
            NPCID.Sets.AttackTime[npc.type] = 90;
            NPCID.Sets.AttackAverageChance[npc.type] = 50;
            NPCID.Sets.HatOffsetY[npc.type] = 0;
        }

        public override void SetDefaults()
        {
            npc.townNPC = true;
            npc.friendly = true;
            npc.width = 18;
            npc.height = 40;
            npc.aiStyle = 7;
            npc.aiAction = 1;
            npc.damage = 10;
            npc.defense = 15;
            npc.lifeMax = 250;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0.5f;
            animationType = NPCID.Guide;
        }

        private bool Offscreen()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && (player.Center - npc.Center).Length() < 1000f)
                    return false;
            }
            return true;
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Tools.EquivalenceMachine>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Tools.Cosmicanon>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Accessories.FidgetSpinner.FidgetSpinner>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.BossItems.Starite.MythicStarfruit>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Materials.Energies.CosmicEnergy>());
            shop.item[nextSlot].shopCustomPrice = AQItem.Prices.EnergyBuyValue;
            nextSlot++;
            if (!Main.dayTime && WorldDefeats.ObtainedUltimateSword)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Weapons.Melee.UltimateSword>());
                nextSlot++;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int dustAmount = npc.life > 0 ? 1 : 5;
            for (int k = 0; k < dustAmount; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood);
            }
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return false;
        }

        public override string TownNPCName()
        {
            switch (WorldGen.genRand.Next(25))
            {
                default:
                return "Link";
                case 0:
                return "Buddy";
                case 1:
                return "Dobby";
                case 2:
                return "Winky";
                case 3:
                return "Hermey";
                case 4:
                return "Altmer";
                case 5:
                return "Summerset";
                case 6:
                return "Calcelmo";
                case 7:
                return "Ancano";
                case 8:
                return "Nurelion";
                case 9:
                return "Vingalmo";
                case 10:
                return "Bosmer";
                case 11:
                return "Faendal";
                case 12:
                return "Malborn";
                case 13:
                return "Niruin";
                case 14:
                return "Enthir";
                case 15:
                return "Dunmer";
                case 16:
                return "Aranea";
                case 17:
                return "Ienith";
                case 18:
                return "Brand-Shei";
                case 19:
                return "Telvanni";
                case 20:
                return "Jenassa";
                case 21:
                return "Erandur";
                case 22:
                return "Neloth";
                case 23:
                return "Gelebor";
                case 24:
                return "Vyrthur";
            }
        }

        public override string GetChat()
        {
            if (!GaleStreams.IsActive)
            {
                return Language.GetTextValue("Mods.AQMod.BalloonMerchant.Chat.Leaving." + Main.rand.Next(3));
            }
            if (!WorldDefeats.HunterIntroduction)
            {
                WorldDefeats.HunterIntroduction = true;
                return Language.GetTextValue("Mods.AQMod.BalloonMerchant.Chat.Introduction", npc.GivenName);
            }
            var potentialText = new List<string>();
            var player = Main.LocalPlayer;
            if (player.ZoneHoly)
            {
                potentialText.Add("BalloonMerchant.Chat.Hallow");
            }
            else if (player.ZoneCorrupt)
            {
                return Language.GetTextValue("Mods.AQMod.BalloonMerchant.Chat.Corruption");
            }
            else if (player.ZoneCrimson)
            {
                return Language.GetTextValue("Mods.AQMod.BalloonMerchant.Chat.Crimson");
            }

            potentialText.Add("BalloonMerchant.Chat.0");
            potentialText.Add("BalloonMerchant.Chat.1");
            potentialText.Add("BalloonMerchant.Chat.2");
            potentialText.Add("BalloonMerchant.Chat.3");
            potentialText.Add("BalloonMerchant.Chat.Vraine");
            potentialText.Add("BalloonMerchant.Chat.StreamingBalloon");
            potentialText.Add("BalloonMerchant.Chat.WhiteSlime");

            if (AQMod.SudoHardmode)
            {
                potentialText.Add("BalloonMerchant.Chat.RedSprite");
                potentialText.Add("BalloonMerchant.Chat.SpaceSquid");
            }

            if (GaleStreams.MeteorTime())
            {
                potentialText.Add("BalloonMerchant.Chat.MeteorTime");
            }

            string chosenText = potentialText[Main.rand.Next(potentialText.Count)];
            string text = Language.GetTextValue("Mods.AQMod." + chosenText);
            if (text == "Mods.AQMod." + chosenText)
                return chosenText;
            return text;
        }

        public override void AI()
        {
            npc.homeless = true;
            bool offscreen = Offscreen();
            if (!_init)
            {
                _init = true;
                if (GaleStreams.IsActive)
                {
                    bool notInTown = true;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].townNPC && (npc.Center - Main.npc[i].Center).Length() < 800f)
                        {
                            SetToTownNPC();
                            notInTown = false;
                            break;
                        }
                    }
                    if (notInTown)
                        SetToBalloon();
                }
            }
            if (!GaleStreams.IsActive && offscreen)
            {
                npc.active = false;
                npc.netSkip = -1;
                npc.life = 0;
                return;
            }
            if (npc.position.X <= 240f || npc.position.X + npc.width > Main.maxTilesX * 16f - 240f
                || (npc.aiStyle == 7 && offscreen && Main.rand.NextBool(1500)))
            {
                BalloonMerchantManager.SpawnMerchant(npc.whoAmI);
                return;
            }

            if (npc.aiStyle == -3)
            {
                npc.velocity.Y -= 1f;
                npc.noGravity = true;
                npc.noTileCollide = true;
                if (offscreen)
                {
                    npc.active = false;
                    npc.netSkip = -1;
                    npc.life = 0;
                }
                return;
            }
            if (npc.aiStyle == -1)
            {
                SetToBalloon();
            }
            if (npc.aiStyle == -2)
            {
                npc.noGravity = true;
                if (offscreen)
                {
                    npc.noTileCollide = true;
                }
                else if (npc.noTileCollide && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                {
                    npc.noTileCollide = false;
                }
                bool canSwitchDirection = true;
                if (npc.position.Y > 3600f)
                {
                    npc.aiStyle = -3;
                    npc.netUpdate = true;
                }
                else if (npc.position.Y > 3000f)
                {
                    npc.velocity.Y -= 0.0125f;
                }
                else if (npc.position.Y < 1600)
                {
                    npc.velocity.Y += 0.0125f;
                }
                else
                {
                    if (npc.velocity.Y.Abs() > 3f)
                    {
                        npc.velocity.Y *= 0.99f;
                    }
                    else
                    {
                        npc.velocity.Y += Main.rand.NextFloat(-0.005f, 0.005f) + npc.velocity.Y * 0.0025f;
                    }
                    bool foundStoppingSpot = false;
                    if (GaleStreams.IsActive)
                    {
                        if (!npc.noTileCollide)
                        {
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                if (Main.player[i].active && !Main.player[i].dead && (npc.Center - Main.player[i].Center).Length() < 150f)
                                {
                                    npc.velocity.Y *= 0.94f;
                                    npc.velocity.X *= 0.96f;
                                    foundStoppingSpot = true;
                                    break;
                                }
                            }
                        }
                        if (npc.ai[0] <= 0f)
                        {
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].townNPC && (npc.Center - Main.npc[i].Center).Length() < 800f)
                                {
                                    if (offscreen)
                                    {
                                        npc.position.X = Main.npc[i].position.X + (Main.npc[i].width - npc.width);
                                        npc.position.Y = Main.npc[i].position.Y + (Main.npc[i].height - npc.height);
                                        SetToTownNPC();
                                    }
                                    else if (!npc.noTileCollide)
                                    {
                                        foundStoppingSpot = true;
                                        npc.velocity.Y *= 0.92f;
                                        npc.velocity.X *= 0.92f;
                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            npc.ai[0]--;
                        }
                    }
                    if (!foundStoppingSpot)
                    {
                        float windSpeed = Math.Max(Main.windSpeed.Abs() * 3f, 1.5f) * Math.Sign(Main.windSpeed);
                        if (windSpeed < 0f)
                        {
                            if (npc.velocity.X > windSpeed)
                            {
                                npc.velocity.X -= 0.025f;
                            }
                        }
                        else
                        {
                            if (npc.velocity.X < windSpeed)
                            {
                                npc.velocity.X += 0.025f;
                            }
                        }
                    }
                    else
                    {
                        canSwitchDirection = false;
                    }
                }

                if (canSwitchDirection)
                {
                    if (npc.spriteDirection == _oldSpriteDirection)
                    {
                        if (npc.velocity.X <= 0)
                        {
                            npc.direction = -1;
                            npc.spriteDirection = npc.direction;
                        }
                        else
                        {
                            npc.direction = 1;
                            npc.spriteDirection = npc.direction;
                        }
                    }
                }
            }
        }

        private void SetToBalloon()
        {
            npc.aiStyle = -2;
            npc.velocity = Vector2.Normalize(Main.MouseWorld - npc.Center);
            if (npc.velocity.X <= 0)
            {
                npc.spriteDirection = -1;
            }
            else
            {
                npc.spriteDirection = 1;
            }
            _oldSpriteDirection = npc.spriteDirection;
            //npc.velocity = new Vector2(Main.rand.NextFloat(1f, 2.5f), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
            npc.netUpdate = true;
            npc.ai[0] = 1000f;
            npc.ai[1] = 0f;
            npc.ai[2] = 0f;
            npc.ai[3] = 0f;
            npc.localAI[0] = 0f;
            npc.localAI[1] = 0f;
            npc.localAI[2] = 0f;
            npc.localAI[3] = 0f;
        }

        private void SetToTownNPC()
        {
            npc.aiStyle = 7;
            npc.noTileCollide = false;
            npc.noGravity = false;
            npc.netUpdate = true;
            npc.velocity.X = 0f;
            npc.velocity.Y = 0f;
            npc.ai[0] = 0f;
            npc.ai[1] = 0f;
            npc.ai[2] = 0f;
            npc.ai[3] = 0f;
            npc.localAI[0] = 0f;
            npc.localAI[1] = 0f;
            npc.localAI[2] = 0f;
            npc.localAI[3] = 0f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (npc.aiStyle != 7)
            {
                var texture = ModContent.GetTexture(this.GetPath("_Basket"));
                int frameX = -1;
                if (npc.spriteDirection != _oldSpriteDirection)
                {
                    _balloonFrameCounter++;
                    if (_balloonFrameCounter > 4)
                    {
                        _balloonFrameCounter = 0;
                        if (_oldSpriteDirection == -1)
                        {
                            if (_balloonFrame < 5 || _balloonFrame > 23)
                            {
                                _balloonFrame = 5;
                            }
                            else
                            {
                                _balloonFrame++;
                            }
                            if (_balloonFrame > 23)
                            {
                                _oldSpriteDirection = npc.spriteDirection;
                                _balloonFrame = 37;
                            }
                        }
                        else
                        {
                            _balloonFrame++;
                            if (_balloonFrame < 41)
                            {
                                _balloonFrame = 41;
                            }
                            if (_balloonFrame > 59)
                            {
                                _oldSpriteDirection = npc.spriteDirection;
                                _balloonFrame = 1;
                            }
                        }
                    }
                }
                else
                {
                    if (npc.spriteDirection == 1)
                    {
                        if (_balloonFrame < 37)
                        {
                            _balloonFrame = 37;
                            frameX = _balloonFrame / 18;
                        }
                        _balloonFrameCounter++;
                        if (_balloonFrameCounter > 20)
                        {
                            _balloonFrameCounter = 0;
                            _balloonFrame++;
                            if (_balloonFrame > 40)
                            {
                                _balloonFrame = 37;
                            }
                        }
                    }
                    else
                    {
                        if (_balloonFrame < 1)
                        {
                            _balloonFrame = 1;
                            frameX = 0;
                        }
                        _balloonFrameCounter++;
                        if (_balloonFrameCounter > 20)
                        {
                            _balloonFrameCounter = 0;
                            _balloonFrame++;
                            if (_balloonFrame > 4)
                            {
                                _balloonFrame = 1;
                            }
                        }
                    }
                }
                if (frameX == -1)
                {
                    frameX = _balloonFrame / 18;
                }
                if (_balloonColor == 0)
                {
                    _balloonColor = Main.rand.Next(5) + 1;
                }
                var frame = new Rectangle(texture.Width / 4 * frameX, texture.Height / 18 * (_balloonFrame % 18), texture.Width / 4, texture.Height / 18);
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition, frame, drawColor, 0f, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);

                float yOff = frame.Height / 2f;
                texture = ModContent.GetTexture(this.GetPath("_Balloon"));
                frame = new Rectangle(0, texture.Height / 5 * (_balloonColor - 1), texture.Width, texture.Height / 5);
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, -yOff + 4f), frame, drawColor, 0f, new Vector2(frame.Width / 2f, frame.Height), 1f, SpriteEffects.None, 0f);
                return false;
            }
            return base.PreDraw(spriteBatch, drawColor);
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = "balloon test";
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
                shop = true;
            else if (npc.aiStyle == 7)
                SetToBalloon();
            else
                SetToTownNPC();
        }

        public override bool CanGoToStatue(bool toKingStatue)
        {
            return toKingStatue;
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 20;
            knockback = 8f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 12;
            randExtraCooldown = 20;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ItemID.PoisonedKnife;
            attackDelay = 10;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 12f;
            randomOffset = 2f;
        }

        public static BalloonMerchant FindInstance()
        {
            int index = Find();
            if (index == -1)
                return null;
            return (BalloonMerchant)Main.npc[index].modNPC;
        }

        public static int Find()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<BalloonMerchant>())
                    return i;
            }
            return -1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.aiStyle);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.aiStyle = reader.ReadInt32();
        }
    }
}