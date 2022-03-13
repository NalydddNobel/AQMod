using AQMod.Assets;
using AQMod.Common.Graphics;
using AQMod.Common.ID;
using AQMod.Common.Utilities.Colors;
using AQMod.Dusts;
using AQMod.Effects;
using AQMod.Effects.Particles;
using AQMod.Items;
using AQMod.Items.Accessories.Vanity;
using AQMod.Items.Armor.Arachnotron;
using AQMod.Projectiles;
using AQMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AQMod.Content.Players
{
    public sealed class PlayerDrawEffects : ModPlayer
    {
        public const int ArmorFrameHeight = 56;
        public const int ArmorFrameCount = 20;

        public const int Draw_ArachnotronArmorOldPositionLength = 8;
        public const float Draw_CelesteZMultiplier = 0.0157f;

        public const string Path_Masks = "AQMod/Content/Players/Masks/Mask_";
        public const string Path_HeadAccs = "AQMod/Content/Players/HeadAccs/HeadAcc_";

        public static int MyOldPositionsLengthCache;
        public static Vector2[] ClientOldPositionsCache;
        public static bool MyArachnotronHeadTrail;
        public static bool MyArachnotronBodyTrail;

        public Vector3[] InterstellarOrbOffsetsForDrawing;
        internal static Color MothmanMaskEyeColorDefault = new Color(50, 155, 255, 0);
        internal static Color MothmanMaskEyeColorShadowScale => new Color(90 + (int)(Math.Cos(Main.GlobalTime * 10f) * 30), 25, 140 - (int)(Math.Sin(Main.GlobalTime * 10f) * 30), 0);
        internal static Color MothmanMaskEyeColorMolten = new Color(50, 155, 255, 0);
        public Color MothmanMaskEyeColor;

        public byte headAcc = PlayerHeadAccID.None;
        public byte mask = PlayerMaskID.None;
        public int cHeadAcc;
        public int cMask;
        public int cCelesteTorus;

        private bool initUpdate;
        public static IColorGradient NalydGradient { get; private set; }
        public IColorGradient NalydGradientPersonal { get; private set; }
        public IColorGradient ThunderbirdGradient { get; private set; }
        public IColorGradient BaguetteGradient { get; private set; }

        #region Player Layers
        internal static readonly PlayerHeadLayer PostDrawHead_Head = new PlayerHeadLayer("AQMod", "PostDraw", (info) =>
        {
            var player = info.drawPlayer;
            AQPlayer aQPlayer = info.drawPlayer.GetModPlayer<AQPlayer>();
            var drawingPlayer = info.drawPlayer.GetModPlayer<PlayerDrawEffects>();
            if (drawingPlayer.mask > 0)
            {
                var drawData = new DrawData(ModContent.GetTexture(Path_Masks + drawingPlayer.mask), new Vector2((int)(info.drawPlayer.position.X - Main.screenPosition.X - info.drawPlayer.bodyFrame.Width / 2 + info.drawPlayer.width / 2), (int)(info.drawPlayer.position.Y - Main.screenPosition.Y + info.drawPlayer.height - info.drawPlayer.bodyFrame.Height)) + info.drawPlayer.headPosition + info.drawOrigin, info.drawPlayer.bodyFrame, info.armorColor, info.drawPlayer.headRotation, info.drawOrigin, info.scale, info.spriteEffects, 0);
                GameShaders.Armor.Apply(drawingPlayer.cMask, player, drawData);
                drawData.Draw(Main.spriteBatch);
                Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            }

            if (drawingPlayer.headAcc > 0)
            {
                var drawData = new DrawData(ModContent.GetTexture(Path_HeadAccs + drawingPlayer.headAcc), new Vector2((int)(info.drawPlayer.position.X - Main.screenPosition.X - info.drawPlayer.bodyFrame.Width / 2 + info.drawPlayer.width / 2), (int)(info.drawPlayer.position.Y - Main.screenPosition.Y + info.drawPlayer.height - info.drawPlayer.bodyFrame.Height)) + info.drawPlayer.headPosition + info.drawOrigin, info.drawPlayer.bodyFrame, info.armorColor, info.drawPlayer.headRotation, info.drawOrigin, info.scale, info.spriteEffects, 0);
                if (drawingPlayer.headAcc == PlayerHeadAccID.FishyFins)
                {
                    drawData.color = player.skinColor;
                }
                drawData.position = new Vector2((int)drawData.position.X, (int)drawData.position.Y);
                GameShaders.Armor.Apply(drawingPlayer.cHeadAcc, player, drawData);
                drawData.Draw(Main.spriteBatch);
                Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            }
        });

        internal static readonly PlayerLayer PreDraw = new PlayerLayer("AQMod", "PreDraw", (info) =>
        {
            int whoAmI = info.drawPlayer.whoAmI;
            var player = info.drawPlayer;
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            var drawEffects = player.GetModPlayer<PlayerDrawEffects>();
            if (info.shadow == 0f)
            {
                if (aQPlayer.blueSpheres && drawEffects.InterstellarOrbOffsetsForDrawing != null)
                {
                    var texture = TextureGrabber.GetProjectile(ModContent.ProjectileType<CelesteTorusCollider>());
                    var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                    var orig = frame.Size() / 2f;
                    for (int i = 0; i < AQPlayer.MaxCelesteTorusOrbs; i++)
                    {
                        var position = aQPlayer.GetCelesteTorusPositionOffset(i);
                        float layerValue = AQUtils.Perspective.GetParralaxScale(1f, drawEffects.InterstellarOrbOffsetsForDrawing[i].Z * Draw_CelesteZMultiplier);
                        if (layerValue < 1f)
                        {
                            var center = info.position + new Vector2(player.width / 2 + (int)position.X, player.height / 2 + (int)position.Y);
                            Main.playerDrawData.Add(new DrawData(texture, AQUtils.Perspective.GetParralaxPosition(center, drawEffects.InterstellarOrbOffsetsForDrawing[i].Z * AQPlayer.CELESTE_Z_MULT) - Main.screenPosition, frame, Lighting.GetColor((int)(center.X / 16f), (int)(center.Y / 16f)), 0f, orig, AQUtils.Perspective.GetParralaxScale(aQPlayer.celesteTorusScale, drawEffects.InterstellarOrbOffsetsForDrawing[i].Z * AQPlayer.CELESTE_Z_MULT), SpriteEffects.None, 0) { shader = drawEffects.cCelesteTorus, ignorePlayerRotation = true });
                        }
                    }
                }
                if (aQPlayer.chomper || aQPlayer.piranhaPlant)
                {
                    int count = 0;
                    int type = ModContent.ProjectileType<Chomper>();
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && (Main.projectile[i].type == type) && Main.projectile[i].owner == info.drawPlayer.whoAmI)
                        {
                            var texture = TextureGrabber.GetProjectile(Main.projectile[i].type);
                            int frameHeight = texture.Height / Main.projFrames[Main.projectile[i].type];
                            var frame = new Rectangle(0, 0, texture.Width, frameHeight - 2);
                            var textureOrig = frame.Size() / 2f;
                            var drawPosition = Main.projectile[i].Center;
                            frame = new Rectangle(0, frameHeight * Main.projectile[i].frame, texture.Width, frameHeight - 2);
                            var drawColor = Lighting.GetColor((int)drawPosition.X / 16, (int)drawPosition.Y / 16);
                            float rotation;
                            if (Main.projectile[i].spriteDirection == -1 && (int)Main.projectile[i].ai[1] > 0f)
                            {
                                rotation = Main.projectile[i].rotation - MathHelper.Pi;
                            }
                            else
                            {
                                rotation = Main.projectile[i].rotation;
                            }
                            DrawChomperChain(info.drawPlayer, Main.projectile[i], drawPosition, drawColor);
                            //if (Main.projectile[i].modProjectile is PiranhaPlant)
                            //{
                            //    var piranhaPlant = (PiranhaPlant)Main.projectile[i].modProjectile;
                            //    if (piranhaPlant.eatingDelay != 0 && piranhaPlant.eatingDelay < 35)
                            //    {
                            //        float intensity = (10 - piranhaPlant.eatingDelay) / 2.5f * AQConfigClient.Instance.EffectIntensity;
                            //        drawPosition.X += Main.rand.NextFloat(-intensity, intensity);
                            //        drawPosition.Y += Main.rand.NextFloat(-intensity, intensity);
                            //    }
                            //}
                            //else
                            {
                                var chomperHead = (Chomper)Main.projectile[i].modProjectile;
                                if (chomperHead.eatingDelay != 0 && chomperHead.eatingDelay < 35)
                                {
                                    float intensity = (10 - chomperHead.eatingDelay) / 2.5f * AQConfigClient.Instance.EffectIntensity;
                                    drawPosition.X += Main.rand.NextFloat(-intensity, intensity);
                                    drawPosition.Y += Main.rand.NextFloat(-intensity, intensity);
                                }
                            }
                            drawPosition = new Vector2((int)(drawPosition.X - Main.screenPosition.X), (int)(drawPosition.Y - Main.screenPosition.Y));
                            Main.playerDrawData.Add(
                                new DrawData(texture, drawPosition, frame, drawColor, rotation, textureOrig, Main.projectile[i].scale, Main.projectile[i].spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0)
                                { ignorePlayerRotation = true });
                            if (count == 0)
                            {
                                if (aQPlayer.monoxiderBird)
                                {
                                    var monoxiderHat = TextureGrabber.GetItem(ModContent.ItemType<MonoxideHat>());
                                    var hatPos = new Vector2(drawPosition.X, drawPosition.Y) + new Vector2(0f, -Main.projectile[i].height / 2).RotatedBy(Main.projectile[i].rotation);
                                    var monoxiderHatOrig = monoxiderHat.Size() / 2f;
                                    hatPos = new Vector2((int)hatPos.X, (int)hatPos.Y);
                                    Main.playerDrawData.Add(
                                        new DrawData(monoxiderHat, hatPos, null, drawColor, Main.projectile[i].rotation, monoxiderHatOrig, Main.projectile[i].scale, SpriteEffects.None, 0)
                                        { ignorePlayerRotation = true });
                                    Main.playerDrawData.Add(
                                        new DrawData(ModContent.GetTexture(AQUtils.GetPath<MonoxideHat>() + "_Glow"), hatPos, null, new Color(250, 250, 250, 0), Main.projectile[i].rotation, monoxiderHatOrig, Main.projectile[i].scale, SpriteEffects.None, 0)
                                        { ignorePlayerRotation = true });
                                    int headFrame = player.bodyFrame.Y / ArmorFrameHeight;
                                    if (player.gravDir == -1)
                                        hatPos.Y += player.height + 8f;
                                    MonoxiderMinion.DrawHead(player, aQPlayer, hatPos, ignorePlayerRotation: true);
                                }
                            }
                            count++;
                        }
                    }
                }
            }
        });
        private static void DrawChomperChain(Player player, Projectile chomper, Vector2 drawPosition, Color drawColor)
        {
            var chomperHead = (Chomper)chomper.modProjectile;
            int frameWidth = 16;
            var frame = new Rectangle(0, 0, frameWidth - 2, 20);
            var origin = frame.Size() / 2f;
            float offset = chomper.width / 2f + frame.Height / 2f;
            var texture = ModContent.GetTexture(AQUtils.GetPath<Chomper>("_Chain"));
            Main.playerDrawData.Add(new DrawData(texture, new Vector2(drawPosition.X + chomper.width / 2 * -chomper.spriteDirection, drawPosition.Y), frame, drawColor, 0f, origin, chomper.scale, chomper.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0)
            { ignorePlayerRotation = true });
            int height = frame.Height - 4;
            frame.Y += 2;
            var playerCenter = player.Center;
            var chainStart = chomper.Center + new Vector2((chomper.width / 2 + 4) * -chomper.spriteDirection, 0f);
            var velo = Vector2.Normalize(Vector2.Lerp(chainStart + new Vector2(0f, height * 4f) - playerCenter, player.velocity, 0.5f)) * height;
            var position = playerCenter;
            var rand = new UnifiedRandom(chomper.whoAmI + player.name.GetHashCode());
            if (AQConfigClient.Instance.EffectQuality >= 1f)
            {
                for (int i = 0; i < 50; i++)
                {
                    Main.playerDrawData.Add(new DrawData(
                        texture, new Vector2((int)(position.X - Main.screenPosition.X), (int)(position.Y - Main.screenPosition.Y)), new Rectangle(frame.X + frameWidth + frameWidth * rand.Next(3), frame.Y, frame.Width, frame.Height), Lighting.GetColor((int)(position.X / 16), (int)(position.Y / 16f)), velo.ToRotation() + MathHelper.PiOver2, origin, 1f, SpriteEffects.None, 0)
                    { ignorePlayerRotation = true });
                    velo = Vector2.Normalize(Vector2.Lerp(velo, chainStart - position, 0.01f + MathHelper.Clamp(1f - Vector2.Distance(chainStart, position) / 100f, 0f, 0.99f))) * height;
                    if (Vector2.Distance(position, chainStart) <= height)
                        break;
                    velo = velo.RotatedBy(Math.Sin(Main.GlobalTime * 6f + i * 0.5f + chomper.whoAmI + rand.NextFloat(-0.02f, 0.02f)) * 0.1f * AQConfigClient.Instance.EffectIntensity);
                    position += velo;
                    float gravity = MathHelper.Clamp(1f - Vector2.Distance(chainStart, position) / 60f, 0f, 1f);
                    velo.Y += gravity * 3f;
                    velo.Normalize();
                    velo *= height;
                }
            }
            else
            {
                if (AQConfigClient.Instance.EffectQuality < 0.2f)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        Main.playerDrawData.Add(new DrawData(
                            texture, new Vector2((int)(position.X - Main.screenPosition.X), (int)(position.Y - Main.screenPosition.Y)), new Rectangle(frame.X + frameWidth + frameWidth * (i % 3), frame.Y, frame.Width, frame.Height), Lighting.GetColor((int)(position.X / 16), (int)(position.Y / 16f)), velo.ToRotation() + MathHelper.PiOver2, origin, 1f, SpriteEffects.None, 0)
                        { ignorePlayerRotation = true });
                        velo = Vector2.Normalize(Vector2.Lerp(velo, chainStart - position, 0.01f + MathHelper.Clamp(1f - Vector2.Distance(chainStart, position) / 100f, 0f, 0.99f))) * height;
                        if (Vector2.Distance(position, chainStart) <= height)
                            break;
                        position += velo;
                        float gravity = MathHelper.Clamp(1f - Vector2.Distance(chainStart, position) / 60f, 0f, 1f);
                        velo.Y += gravity * 3f;
                        velo.Normalize();
                        velo *= height;
                    }
                }
                else
                {
                    for (int i = 0; i < 50; i++)
                    {
                        Main.playerDrawData.Add(new DrawData(
                            texture, new Vector2((int)(position.X - Main.screenPosition.X), (int)(position.Y - Main.screenPosition.Y)), new Rectangle(frame.X + frameWidth + frameWidth * rand.Next(3), frame.Y, frame.Width, frame.Height), Lighting.GetColor((int)(position.X / 16), (int)(position.Y / 16f)), velo.ToRotation() + MathHelper.PiOver2, origin, 1f, SpriteEffects.None, 0)
                        { ignorePlayerRotation = true });
                        velo = Vector2.Normalize(Vector2.Lerp(velo, chainStart - position, 0.01f + MathHelper.Clamp(1f - Vector2.Distance(chainStart, position) / 100f, 0f, 0.99f))) * height;
                        if (Vector2.Distance(position, chainStart) <= height)
                            break;
                        position += velo;
                        float gravity = MathHelper.Clamp(1f - Vector2.Distance(chainStart, position) / 60f, 0f, 1f);
                        velo.Y += gravity * 3f;
                        velo.Normalize();
                        velo *= height;
                    }
                }
            }
            rand = new UnifiedRandom(chomper.whoAmI + 2 + player.name.GetHashCode());
            texture = ModContent.GetTexture(AQUtils.GetPath<Chomper>("_Leaves"));
            frame.Y -= 2;
            int numLeaves = rand.Next(4) + 3;
            float leafRotation = chomper.rotation;
            if (chomper.spriteDirection == -1 && chomper.rotation.Abs() > MathHelper.PiOver2)
                leafRotation -= MathHelper.Pi;
            float rotOff = MathHelper.PiOver2 / numLeaves;
            float rotStart = leafRotation - MathHelper.PiOver4;
            for (int i = 0; i < numLeaves; i++)
            {
                var leavesPos = drawPosition + new Vector2((offset - rand.NextFloat(2f)) * -chomper.spriteDirection, 0f).RotatedBy(rotStart + rotOff * i) - Main.screenPosition;
                leafRotation = (drawPosition - Main.screenPosition - leavesPos).ToRotation();
                Main.playerDrawData.Add(new DrawData(texture, new Vector2((int)leavesPos.X, leavesPos.Y), new Rectangle(frame.X + frameWidth * rand.Next(4), frame.Y, frame.Width, frame.Height), drawColor, leafRotation + MathHelper.PiOver2, origin, chomper.scale + rand.NextFloat(0.2f), SpriteEffects.None, 0)
                { ignorePlayerRotation = true });
            }
        }


        internal static readonly PlayerLayer PostDraw = new PlayerLayer("AQMod", "PostDraw", (info) =>
        {
            int whoAmI = info.drawPlayer.whoAmI;
            var aQMod = AQMod.GetInstance();
            var player = info.drawPlayer;
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            var drawEffects = player.GetModPlayer<PlayerDrawEffects>();
            if (Main.myPlayer == info.drawPlayer.whoAmI && info.shadow == 0f)
            {
                bool updateOldPos = true;
                if (ShouldDrawOldPos(info.drawPlayer))
                {
                    if (ClientOldPositionsCache != null && ClientOldPositionsCache.Length >= MyOldPositionsLengthCache)
                    {
                        if (MyArachnotronHeadTrail)
                        {
                            if (info.shadow == 0f)
                            {
                                var headOff = new Vector2(-info.drawPlayer.bodyFrame.Width / 2 + (float)(info.drawPlayer.width / 2), info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + 10f) + info.drawPlayer.headPosition + info.headOrigin;
                                var clr = new Color(255, 255, 255, 0) * (1f - info.shadow);
                                var drawDiff = info.position - info.drawPlayer.position;
                                var texture = ModContent.GetTexture(AQUtils.GetPath2<AArachnotronVisor>("_HeadGlow"));
                                int count = GetOldPosCountMaxed(Draw_ArachnotronArmorOldPositionLength);
                                var clrMult = 1f / count;
                                for (int i = 0; i < count; i++)
                                {
                                    float colorMult = 0.5f * (1f - (float)Math.Sin(Main.GlobalTime * 8f - i * 0.314f) * 0.2f);
                                    var drawData = new DrawData(texture, new Vector2((int)(ClientOldPositionsCache[i].X - Main.screenPosition.X), (int)(ClientOldPositionsCache[i].Y - Main.screenPosition.Y)) + drawDiff + headOff, info.drawPlayer.bodyFrame, clr * (clrMult * (count - i)) * colorMult, info.drawPlayer.bodyRotation, info.bodyOrigin, 1f, info.spriteEffects, 0) { shader = info.headArmorShader };
                                    Main.playerDrawData.Add(drawData);
                                }
                            }
                        }
                        if (MyArachnotronBodyTrail)
                        {
                            var bodyOff = new Vector2(-info.drawPlayer.bodyFrame.Width / 2 + (float)(info.drawPlayer.width / 2), info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + 4f) + info.drawPlayer.bodyPosition + new Vector2(info.drawPlayer.bodyFrame.Width / 2, info.drawPlayer.bodyFrame.Height / 2);
                            var clr = new Color(255, 255, 255, 0) * (1f - info.shadow);
                            var drawDiff = info.position - info.drawPlayer.position;
                            var texture = ModContent.GetTexture(AQUtils.GetPath2<ABArachnotronRibcage>("_BodyGlow"));
                            int count = GetOldPosCountMaxed(Draw_ArachnotronArmorOldPositionLength);
                            if (info.shadow == 0f)
                            {
                                var clrMult = 1f / count;
                                for (int i = 0; i < count; i++)
                                {
                                    float colorMult = 0.5f * (1f - (float)Math.Sin(Main.GlobalTime * 8f - i * 0.314f) * 0.2f);
                                    var drawData = new DrawData(texture, new Vector2((int)(ClientOldPositionsCache[i].X - Main.screenPosition.X), (int)(ClientOldPositionsCache[i].Y - Main.screenPosition.Y)) + drawDiff + bodyOff, info.drawPlayer.bodyFrame, clr * (clrMult * (count - i)) * colorMult, info.drawPlayer.bodyRotation, info.bodyOrigin, 1f, info.spriteEffects, 0) { shader = info.bodyArmorShader };
                                    Main.playerDrawData.Add(drawData);
                                }
                            }
                        }
                    }
                }
                else
                {
                    updateOldPos = false;
                }
                if (updateOldPos)
                {
                    if (AQGraphics.GameWorldActive && MyOldPositionsLengthCache > 0)
                    {
                        if (ClientOldPositionsCache == null || ClientOldPositionsCache.Length != MyOldPositionsLengthCache)
                            ClientOldPositionsCache = new Vector2[MyOldPositionsLengthCache];
                        for (int i = MyOldPositionsLengthCache - 1; i > 0; i--)
                        {
                            ClientOldPositionsCache[i] = ClientOldPositionsCache[i - 1];
                        }
                        ClientOldPositionsCache[0] = player.position;
                    }
                }
                else
                {
                    ClientOldPositionsCache = null;
                }
            }

            if (info.shadow == 0f && aQPlayer.blueSpheres && drawEffects.InterstellarOrbOffsetsForDrawing != null)
            {
                var texture = TextureGrabber.GetProjectile(ModContent.ProjectileType<CelesteTorusCollider>());
                var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                var orig = frame.Size() / 2f;
                for (int i = 0; i < AQPlayer.MaxCelesteTorusOrbs; i++)
                {
                    var position = aQPlayer.GetCelesteTorusPositionOffset(i);
                    float layerValue = AQUtils.Perspective.GetParralaxScale(1f, drawEffects.InterstellarOrbOffsetsForDrawing[i].Z * Draw_CelesteZMultiplier);
                    if (layerValue >= 1f)
                    {
                        var center = info.position + new Vector2(player.width / 2 + (int)position.X, player.height / 2 + (int)position.Y);
                        Main.playerDrawData.Add(new DrawData(texture, AQUtils.Perspective.GetParralaxPosition(center, drawEffects.InterstellarOrbOffsetsForDrawing[i].Z * Draw_CelesteZMultiplier) - Main.screenPosition, frame, Lighting.GetColor((int)(center.X / 16f), (int)(center.Y / 16f)), 0f, orig, AQUtils.Perspective.GetParralaxScale(aQPlayer.celesteTorusScale, drawEffects.InterstellarOrbOffsetsForDrawing[i].Z * AQPlayer.CELESTE_Z_MULT), SpriteEffects.None, 0) { shader = drawEffects.cCelesteTorus, ignorePlayerRotation = true });
                    }
                }
            }
        });

        internal static readonly PlayerLayer PostDrawHeldItem = new PlayerLayer("AQMod", "PostDrawHeldItem", PlayerLayer.HeldItem, (info) =>
        {
            var player = info.drawPlayer;
            Item item = player.inventory[player.selectedItem];

            if (info.shadow != 0f || player.frozen || ((player.itemAnimation <= 0 || item.useStyle == 0) &&
            (item.holdStyle <= 0 || player.pulley)) || item.type <= ItemID.None || player.dead || item.noUseGraphic ||
            (item.noWet && player.wet) || item.type < Main.maxItemTypes)
            {
                return;
            }

            item.GetGlobalItem<AQItem>().Glowmask.DrawHeld(player, player.GetModPlayer<AQPlayer>(), item, info);
        });

        internal static readonly PlayerLayer PostDrawHead = new PlayerLayer("AQMod", "PostDrawHead", PlayerLayer.Head, (info) =>
        {
            var player = info.drawPlayer;
            var aQPlayer = info.drawPlayer.GetModPlayer<AQPlayer>();
            var drawing = info.drawPlayer.GetModPlayer<PlayerDrawEffects>();
            float opacity = 1f - info.shadow;
            const float MagicOffsetForReversedGravity = 8f;
            int headFrame = info.drawPlayer.bodyFrame.Y / ArmorFrameHeight;
            float gravityOffset = 0f;
            AQMod.ArmorOverlays.InvokeArmorOverlay(EquipLayering.Head, info);
            if (info.drawPlayer.gravDir == -1)
                gravityOffset = MagicOffsetForReversedGravity;
            if (drawing.mask > 0)
            {
                var position = new Vector2((int)(info.position.X - Main.screenPosition.X - info.drawPlayer.bodyFrame.Width / 2 + info.drawPlayer.width / 2), (int)(info.position.Y - Main.screenPosition.Y + info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + gravityOffset)) + info.drawPlayer.headPosition + info.headOrigin;
                var color = Lighting.GetColor((int)info.position.X / 16, (int)(info.position.Y + gravityOffset) / 16) * opacity;
                int shader = drawing.cMask;
                switch (drawing.mask)
                {
                    default:
                        {
                            Main.playerDrawData.Add(new DrawData(ModContent.GetTexture(Path_Masks + drawing.mask), position, info.drawPlayer.bodyFrame, color, info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = shader, });
                        }
                        break;

                    case PlayerMaskID.CataMask:
                        {
                            if (drawing.cMask > 0)
                                drawing.MothmanMaskEyeColor = new Color(100, 100, 100, 0);
                            Main.playerDrawData.Add(new DrawData(ModContent.GetTexture(Path_Masks + drawing.mask), position, info.drawPlayer.bodyFrame, color, info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = shader, });
                            if (player.statLife == player.statLifeMax2 && info.drawPlayer.headRotation == 0)
                            {
                                var texture = LegacyTextureCache.Lights[LightTex.Spotlight240x66];
                                var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                                var orig = frame.Size() / 2f;
                                var scale = new Vector2((float)(Math.Sin(Main.GlobalTime * 10f) + 1f) * 0.04f + 0.2f, 0.1f);
                                var eyeGlowPos = position + new Vector2(2f * player.direction, Main.OffsetsPlayerHeadgear[headFrame].Y);
                                var eyeGlowColor = drawing.MothmanMaskEyeColor;
                                var value = AQUtils.GetParabola(0.25f, 0.45f, scale.X) * 0.5f;
                                var config = ModContent.GetInstance<AQConfigClient>();
                                var colorMult = ModContent.GetInstance<AQConfigClient>().EffectIntensity * (1f - info.shadow);
                                Main.playerDrawData.Add(new DrawData(texture, eyeGlowPos, frame, eyeGlowColor * colorMult, 0f, orig, scale, info.spriteEffects, 0) { shader = drawing.cMask, });
                                Main.playerDrawData.Add(new DrawData(texture, eyeGlowPos, frame, eyeGlowColor * 0.3f * colorMult, 0f, orig, scale * (1.1f + value * 2), info.spriteEffects, 0) { shader = shader, });
                                if (ModContent.GetInstance<AQConfigClient>().EffectQuality > 0.5f)
                                {
                                    Main.playerDrawData.Add(new DrawData(texture, eyeGlowPos, frame, eyeGlowColor * 0.35f * colorMult, MathHelper.PiOver4, orig, scale * (1f - value) * 0.75f, info.spriteEffects, 0) { shader = shader, });
                                    Main.playerDrawData.Add(new DrawData(texture, eyeGlowPos, frame, eyeGlowColor * 0.35f * colorMult, -MathHelper.PiOver4, orig, scale * (1f - value) * 0.75f, info.spriteEffects, 0) { shader = shader, });
                                    Main.playerDrawData.Add(new DrawData(texture, eyeGlowPos, frame, eyeGlowColor * 0.2f * colorMult, MathHelper.PiOver2, orig, scale * (1f - value), info.spriteEffects, 0) { shader = shader, });
                                }
                                Main.playerDrawData.Add(new DrawData(texture, eyeGlowPos, frame, eyeGlowColor * colorMult, MathHelper.PiOver2, orig, scale * 0.5f, info.spriteEffects, 0) { shader = shader, });
                                if (ModContent.GetInstance<AQConfigClient>().EffectIntensity > 1.5f && ModContent.GetInstance<AQConfigClient>().EffectQuality > 0.5f)
                                    Main.playerDrawData.Add(new DrawData(texture, eyeGlowPos, frame, eyeGlowColor * 0.15f * colorMult, 0f, orig, scale * (2f + value * 3f), info.spriteEffects, 0) { shader = shader, });
                            }
                        }
                        break;
                }
            }
            if (drawing.headAcc > 0)
            {
                Vector2 position = new Vector2((int)(info.position.X - Main.screenPosition.X - info.drawPlayer.bodyFrame.Width / 2 + info.drawPlayer.width / 2), (int)(info.position.Y - Main.screenPosition.Y + info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + gravityOffset)) + info.drawPlayer.headPosition + info.headOrigin;
                Color color = Lighting.GetColor((int)info.position.X / 16, (int)info.position.Y / 16) * opacity;
                if (aQPlayer.omoriEffect)
                {
                    color = Color.White;
                }
                int shader = drawing.cHeadAcc;
                switch (drawing.headAcc)
                {
                    default:
                        {
                            Main.playerDrawData.Add(new DrawData(ModContent.GetTexture(Path_HeadAccs + drawing.headAcc), position, info.drawPlayer.bodyFrame, color, info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = shader, });
                        }
                        break;

                    case PlayerHeadAccID.MonoxideHat:
                        {
                            Main.playerDrawData.Add(new DrawData(ModContent.GetTexture(Path_HeadAccs + drawing.headAcc), position, info.drawPlayer.bodyFrame, color, info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = shader, });
                            Main.playerDrawData.Add(new DrawData(ModContent.GetTexture(Path_HeadAccs + PlayerHeadAccID.MonoxideHatGlow), position, info.drawPlayer.bodyFrame, new Color(opacity * 0.99f, opacity * 0.99f, opacity * 0.99f, 0f), info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = shader, });
                            if (aQPlayer.monoxiderBird && !aQPlayer.chomper)
                            {
                                var hatPos = position;
                                if (player.gravDir == -1)
                                {
                                    hatPos.Y += player.height + Main.OffsetsPlayerHeadgear[headFrame].Y + 8f;
                                }
                                else
                                {
                                    hatPos.Y += Main.OffsetsPlayerHeadgear[headFrame].Y;
                                }
                                MonoxiderMinion.DrawHead(player, aQPlayer, hatPos, ignorePlayerRotation: false);
                            }
                        }
                        break;

                    case PlayerHeadAccID.FishyFins:
                        {
                            var drawData = new DrawData(ModContent.GetTexture(Path_HeadAccs + drawing.headAcc), position, info.drawPlayer.bodyFrame, Lighting.GetColor((int)info.position.X / 16, (int)info.position.Y / 16, info.drawPlayer.skinColor) * opacity, info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = shader, };
                            if (aQPlayer.omoriEffect)
                            {
                                if (drawing.headAcc == PlayerHeadAccID.FishyFins)
                                {
                                    drawData.color = Color.White * opacity;
                                }
                                else
                                {
                                    drawData.color = Color.Black * opacity;
                                }
                            }
                            Main.playerDrawData.Add(drawData);
                        }
                        break;
                }
            }
        });

        internal static readonly PlayerLayer PostDrawBody = new PlayerLayer("AQMod", "PostDrawBody", PlayerLayer.Body, (info) =>
        {
            AQMod.ArmorOverlays.InvokeArmorOverlay(EquipLayering.Body, info);
        });

        internal static readonly PlayerLayer PostDrawWings = new PlayerLayer("AQMod", "PostDrawWings", PlayerLayer.Wings, (info) =>
        {
            if (info.shadow == 0f)
            {
                Player player = info.drawPlayer;
                if (!player.dead)
                {
                    AQMod.ArmorOverlays.InvokeArmorOverlay(EquipLayering.Wings, info);
                }
            }
        });
        #endregion

        private void InitUpdate(string name = null)
        {
            NalydGradient = new ColorWaveGradient(10f, Color.Violet, Color.MediumPurple);
            NalydGradientPersonal = NalydGradient;
            ThunderbirdGradient = new ColorWaveGradient(10f, new Color(255, 120, 200), new Color(170, 80, 200));
            BaguetteGradient = new ColorWaveGradient(10f, new Color(255, 222, 150), new Color(170, 130, 80));
            if (Main.myPlayer == player.whoAmI)
            {
                FX.cameraFocusNPC = -1;
            }
        }
        public override void Initialize()
        {
            InitUpdate(player.name);
            initUpdate = true;
            ResetDrawingInfo();
        }

        public override void ResetEffects()
        {
            if (initUpdate)
            {
                InitUpdate(player.name);
                initUpdate = false;
            }
            ResetDrawingInfo();
        }

        private void ResetDrawingInfo()
        {
            headAcc = PlayerHeadAccID.None;
            mask = PlayerMaskID.None;
            cHeadAcc = 0;
            cMask = 0;
            cCelesteTorus = 0;
            MothmanMaskEyeColor = MothmanMaskEyeColorDefault;
        }

        private void UpdateCameraFocus()
        {
            FX.cameraFocus = false;
            if (FX.cameraFocusNPC != -1)
            {
                if (Main.npc[FX.cameraFocusNPC].active)
                {
                    FX.cameraFocus = true;
                    FX.CameraFocus = Main.npc[FX.cameraFocusNPC].Center;
                }
                else
                {
                    FX.cameraFocusNPC = -1;
                }
            }
            if (FX.CameraFocus != Vector2.Zero)
            {
                if (FX.cameraFocusLerp <= 0.001f)
                {
                    FX.cameraFocusLerp = 0.001f;
                }
                if (!FX.cameraFocus && FX.cameraFocusReset > 0)
                {
                    FX.cameraFocusReset--;
                    FX.cameraFocus = true;
                }
                if (FX.cameraFocus)
                {
                    Main.screenPosition = Vector2.Lerp(Main.screenPosition, FX.CameraFocus - new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f), FX.cameraFocusLerp);
                    if (FX.cameraFocusLerp < 1f)
                    {
                        FX.cameraFocusLerp *= 1.5f;
                        if (FX.cameraFocusLerp > 1f)
                        {
                            FX.cameraFocusLerp = 1f;
                        }
                    }
                }
                else
                {
                    Main.screenPosition = Vector2.Lerp(Main.screenPosition, FX.CameraFocus - new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f), FX.cameraFocusLerp);
                    FX.cameraFocusLerp *= 0.86f;
                    if (FX.cameraFocusLerp < 0.001f)
                    {
                        FX.cameraFocusLerp = 0f;
                        FX.CameraFocus = Vector2.Zero;
                    }
                }
            }
        }
        private void UpdateScreenShakes()
        {
            var offset = Vector2.Zero;
            foreach (var s in FX.ScreenShakes)
            {
                if (s.Value.Active)
                {
                    offset += s.Value.GetOffset();
                }
            }
            Main.screenPosition += new Vector2((int)offset.X, (int)offset.Y);
        }
        public override void ModifyScreenPosition()
        {
            if (!Main.gamePaused && Main.instance.IsActive)
            {
                UpdateCameraFocus();
                UpdateScreenShakes();
            }
        }

        private static void ModifyDrawInfo_Omori(ref PlayerDrawInfo drawInfo)
        {
            drawInfo.faceColor = Color.White;
            drawInfo.eyeWhiteColor = Color.White;
            drawInfo.hairColor = Color.Black;
            drawInfo.eyeColor = Color.Black;

            drawInfo.bodyColor = Color.Black;
            drawInfo.shirtColor = Color.Black;
            drawInfo.underShirtColor = Color.Black;
            drawInfo.upperArmorColor = Color.White;
            drawInfo.middleArmorColor = Color.Black;
            drawInfo.lowerArmorColor = Color.Black;

            drawInfo.legColor = Color.Black;
            drawInfo.pantsColor = Color.Black;
            drawInfo.shoeColor = Color.Black;

            drawInfo.armGlowMaskColor = Color.White * (drawInfo.armGlowMaskColor.A / 255f);
            drawInfo.bodyGlowMaskColor = Color.White * (drawInfo.bodyGlowMaskColor.A / 255f);
            drawInfo.legGlowMaskColor = Color.White * (drawInfo.bodyGlowMaskColor.A / 255f);
        }
        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            var aQPlayer = drawInfo.drawPlayer.GetModPlayer<AQPlayer>();
            if (aQPlayer.omoriEffect)
            {
                ModifyDrawInfo_Omori(ref drawInfo);
            }
        }

        private static void DrawEffects_MonoxideHatCheck(AQPlayer aQPlayer, PlayerDrawEffects effects)
        {
            if (!aQPlayer.chomper && aQPlayer.monoxiderBird)
                effects.headAcc = PlayerHeadAccID.MonoxideHat;
        }
        private static void DrawEffects_UnusedShimmering(PlayerDrawInfo drawInfo)
        {
            for (int i = 0; i < 4; i++)
            {
                int d = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), drawInfo.drawPlayer.width + 4, drawInfo.drawPlayer.height + 4, ModContent.DustType<UltimateEnergyDust>(), drawInfo.drawPlayer.velocity.X * 0.4f, drawInfo.drawPlayer.velocity.Y * 0.4f, 100, default(Color), Main.rand.NextFloat(0.45f, 1f));
                Main.dust[d].velocity *= 2.65f;
                Main.dust[d].velocity.Y -= 2f;
                Main.playerDrawDust.Add(d);
            }
            Lighting.AddLight(drawInfo.drawPlayer.Center, 1f, 1f, 1f);
        }
        private static void DrawEffects_BlueFire(PlayerDrawInfo drawInfo)
        {
            if (Main.netMode != NetmodeID.Server && AQGraphics.GameWorldActive)
            {
                for (int i = 0; i < 3; i++)
                {
                    var pos = drawInfo.position - new Vector2(2f, 2f);
                    var rect = new Rectangle((int)pos.X, (int)pos.Y, drawInfo.drawPlayer.width + 4, drawInfo.drawPlayer.height + 4);
                    var dustPos = new Vector2(Main.rand.Next(rect.X, rect.X + rect.Width), Main.rand.Next(rect.Y, rect.Y + rect.Height));
                    Particle.PostDrawPlayers.AddParticle(
                        new EmberParticle(dustPos, new Vector2((drawInfo.drawPlayer.velocity.X + Main.rand.NextFloat(-3f, 3f)) * 0.3f, ((drawInfo.drawPlayer.velocity.Y + Main.rand.NextFloat(-3f, 3f)) * 0.4f).Abs() - 2f),
                        new Color(0.5f, Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextFloat(0.8f, 1f), 0f), Main.rand.NextFloat(0.2f, 1.2f)));
                }
            }
            Lighting.AddLight(drawInfo.drawPlayer.Center, 0.4f, 0.4f, 1f);
        }
        private static void DrawEffects_CataMaskDust(PlayerDrawInfo drawInfo, PlayerDrawEffects effects, int gravityOffset, int headFrame)
        {
            if (effects.cMask > 0)
                effects.MothmanMaskEyeColor = new Color(100, 100, 100, 0);
            if (!drawInfo.drawPlayer.mount.Active && !drawInfo.drawPlayer.merman && !drawInfo.drawPlayer.wereWolf && drawInfo.drawPlayer.statLife == drawInfo.drawPlayer.statLifeMax2)
            {
                float dustAmount = (Main.rand.Next(2, 3) + 1) * ModContent.GetInstance<AQConfigClient>().EffectQuality;
                if (dustAmount < 1f)
                {
                    if (Main.rand.NextFloat(dustAmount) > 0.1f)
                        effects.CataEyeDust(getCataDustSpawnPos(drawInfo.drawPlayer, gravityOffset, headFrame));
                }
                else
                {
                    var spawnPos = getCataDustSpawnPos(drawInfo.drawPlayer, gravityOffset, headFrame);
                    for (int i = 0; i < dustAmount; i++)
                    {
                        effects.CataEyeDust(spawnPos);
                    }
                }
            }
        }
        private static void DrawEffects_BlueSpheres(AQPlayer aQPlayer, PlayerDrawEffects effects)
        {
            effects.InterstellarOrbOffsetsForDrawing = new Vector3[5];
            for (int i = 0; i < 5; i++)
            {
                effects.InterstellarOrbOffsetsForDrawing[i] = aQPlayer.GetCelesteTorusPositionOffset(i);
            }
        }
        private static void DrawEffects_ResetMyPlayersSpecialDrawingData()
        {
            MyOldPositionsLengthCache = 0;
            MyArachnotronHeadTrail = false;
            MyArachnotronBodyTrail = false;
        }
        private static void DrawEffects_UpdateEquipVisuals(Player player, AQPlayer aQPlayer, PlayerDrawEffects effects)
        {
            for (int i = 0; i < AQPlayer.MaxDye; i++)
            {
                if (player.armor[i].type > Main.maxItemTypes && !player.hideVisual[i] && player.armor[i].modItem is IUpdateVanity updateVanity)
                    updateVanity.UpdateVanitySlot(player, aQPlayer, effects, i);
            }
            for (int i = AQPlayer.MaxDye; i < AQPlayer.MaxArmor; i++)
            {
                if (player.armor[i].type > Main.maxItemTypes && player.armor[i].modItem is IUpdateVanity updateVanity)
                    updateVanity.UpdateVanitySlot(player, aQPlayer, effects, i);
            }
        }
        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            var aQPlayer = drawInfo.drawPlayer.GetModPlayer<AQPlayer>();
            var effects = drawInfo.drawPlayer.GetModPlayer<PlayerDrawEffects>();
            if (drawInfo.shadow == 0f)
            {
                if (Main.myPlayer == drawInfo.drawPlayer.whoAmI)
                {
                    DrawEffects_ResetMyPlayersSpecialDrawingData();
                }
                if (aQPlayer.shimmering)
                {
                    DrawEffects_UnusedShimmering(drawInfo);
                    fullBright = true;
                }
                if (aQPlayer.blueFire)
                {
                    DrawEffects_BlueFire(drawInfo);
                    fullBright = true;
                }
                DrawEffects_UpdateEquipVisuals(drawInfo.drawPlayer, aQPlayer, effects);
                DrawEffects_MonoxideHatCheck(aQPlayer, effects);
                int gravityOffset = 0;
                int headFrame = player.bodyFrame.Y / ArmorFrameHeight;
                if (player.gravDir == -1)
                    gravityOffset = 8;
                if (effects.mask == PlayerMaskID.CataMask)
                {
                    DrawEffects_CataMaskDust(drawInfo, effects, gravityOffset, headFrame);
                }
                if (aQPlayer.blueSpheres)
                {
                    DrawEffects_BlueSpheres(aQPlayer, effects);
                }
            }
        }

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            int i = layers.FindIndex((p) => p.mod.Equals("Terraria") && p.Name.Equals("Head"));
            if (i != -1)
            {
                PostDrawHead.visible = true;
                layers.Insert(i + 1, PostDrawHead);
            }
            i = layers.FindIndex((p) => p.mod.Equals("Terraria") && p.Name.Equals("Body"));
            if (i != -1)
            {
                PostDrawBody.visible = true;
                layers.Insert(i + 1, PostDrawBody);
            }
            i = layers.FindIndex((p) => p.mod.Equals("Terraria") && p.Name.Equals("Wings"));
            if (i != -1)
            {
                PostDrawWings.visible = true;
                layers.Insert(i + 1, PostDrawWings);
            }
            i = layers.FindIndex((p) => p.mod.Equals("Terraria") && p.Name.Equals("HeldItem"));
            if (i != -1)
            {
                PostDrawHeldItem.visible = true;
                layers.Insert(i + 1, PostDrawHeldItem);
            }
            PreDraw.visible = true;
            layers.Insert(0, PreDraw);
            PostDraw.visible = true;
            layers.Add(PostDraw);
        }

        public override void ModifyDrawHeadLayers(List<PlayerHeadLayer> layers)
        {
            PostDrawHead_Head.visible = true;
            layers.Add(PostDrawHead_Head);
        }

        private static Vector2 getCataDustSpawnPos(Player player, int gravityOffset, int headFrame)
        {
            var spawnPos = new Vector2((int)(player.position.X + player.width / 2) - 3f, (int)(player.position.Y + 12f + gravityOffset) + Main.OffsetsPlayerHeadgear[headFrame].Y) + player.headPosition;
            if (player.direction == -1)
                spawnPos.X -= 4f;
            spawnPos.X -= 0.6f;
            spawnPos.Y -= 0.6f;
            return spawnPos;
        }

        private void CataEyeDust(Vector2 spawnPos)
        {
            int d = Dust.NewDust(spawnPos + new Vector2(0f, -6f), 6, 6, ModContent.DustType<MonoDust>(), 0, 0, 0, MothmanMaskEyeColor);
            if (Main.rand.NextBool(600))
            {
                Main.dust[d].velocity = player.velocity.RotatedBy(Main.rand.NextFloat(-0.025f, 0.025f)) * 1.5f;
                Main.dust[d].velocity.X += Main.windSpeed * 20f + player.velocity.X / -2f;
                Main.dust[d].velocity.Y -= Main.rand.NextFloat(8f, 16f);
                Main.dust[d].scale *= Main.rand.NextFloat(0.65f, 2f);
            }
            else
            {
                Main.dust[d].velocity = player.velocity * 1.1f;
                Main.dust[d].velocity.X += Main.windSpeed * 2.5f + player.velocity.X / -2f;
                Main.dust[d].velocity.Y -= Main.rand.NextFloat(4f, 5.65f);
                Main.dust[d].scale *= Main.rand.NextFloat(0.95f, 1.4f);
            }

            Main.dust[d].shader = GameShaders.Armor.GetSecondaryShader(cMask, player);
            Main.playerDrawDust.Add(d);
        }

        public static bool ShouldDrawOldPos(Player player)
        {
            if (player.mount.Active || player.frozen || player.stoned || player.GetModPlayer<PlayerDrawEffects>().mask > 0)
                return false;
            return true;
        }

        public static int GetOldPosCountMaxed(int maxCount)
        {
            int count = 0;
            for (; count < maxCount; count++)
            {
                if (ClientOldPositionsCache[count] == default(Vector2))
                    break;
            }
            return count;
        }
    }
}