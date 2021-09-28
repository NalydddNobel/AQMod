using AQMod.Assets.Enumerators;
using AQMod.Assets.PlayerLayers;
using AQMod.Common.Config;
using AQMod.Common.IO;
using AQMod.Content.CursorDyes;
using AQMod.Content.Dusts;
using AQMod.Items;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Common
{
    public class GraphicsPlayer : ModPlayer
    {
        public const int MAX_ARMOR = 20;
        public const int DYE_WRAP = MAX_ARMOR / 2;
        public const int FRAME_HEIGHT = 56;
        public const int FRAME_COUNT = 20;
        public const float CELESTE_Z_MULT = 0.0157f;

        public const int ARACHNOTRON_OLD_POS_LENGTH = 10;

        public float quality;
        public float intensity;
        public float parallax;

        public byte arachnotronGlowTimer;

        public int headOverlay = -1;
        public int mask = -1;
        public int cHeadOverlay;
        public int cMask;
        public int cCelesteTorus;
        public int specialHead = -1;
        public int specialBody = -1;

        public int CursorDyeID { get; private set; } = 0;
        public string CursorDye { get; private set; } = "";

        public void SetCursorDye(int type)
        {
            if (type <= CursorDyeLoader.ID.None || type > AQMod.CursorDyes.Count)
            {
                CursorDyeID = CursorDyeLoader.ID.None;
                CursorDye = "";
            }
            else
            {
                CursorDyeID = type;
                var cursorDye = AQMod.CursorDyes.GetContent(type);
                CursorDye = AQStringCodes.EncodeName(cursorDye.Mod, cursorDye.Name);
            }
        }

        public bool mothmanMaskSpecial;
        public Color cataEyeColor;

        public const int CARRY_MONOXIDER = 0;

        public byte hatCarryType;
        public byte hatMinionCarry;

        public int oldPosLength;
        public Vector2[] oldPosVisual;

        public Vector3[] celesteTorusPositions;

        public int GetOldPosCountMaxed(int maxCount)
        {
            int count = 0;
            for (; count < maxCount; count++)
            {
                if (oldPosVisual[count] == default(Vector2))
                    break;
            }
            return count;
        }

        public override void Initialize()
        {
            headOverlay = -1;
            mask = -1;
            CursorDyeID = 0;
            cHeadOverlay = 0;
            cMask = 0;
            cCelesteTorus = 0;
            specialHead = -1;
            specialBody = -1;
            oldPosLength = 0;
            oldPosVisual = null;
            hatMinionCarry = 0;
            cataEyeColor = new Color(50, 155, 255, 0);
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["CursorDye"] = CursorDye,
            };
        }

        public override void Load(TagCompound tag)
        {
            string dyeKey = tag.GetString("CursorDye");
            if (!string.IsNullOrEmpty(dyeKey) && AQStringCodes.DecodeName(dyeKey, out string cursorDyeMod, out string cursorDyeName))
            {

                SetCursorDye(AQMod.CursorDyes.GetContentID(cursorDyeMod, cursorDyeName));
            }
            else
            {
                SetCursorDye(CursorDyeLoader.ID.None);
            }
        }

        public override void ResetEffects()
        {
            headOverlay = -1;
            mask = -1;
            cHeadOverlay = 0;
            cMask = 0;
            cCelesteTorus = 0;
            specialHead = -1;
            specialBody = -1;
            oldPosLength = 0;
            hatMinionCarry = 0;
            cataEyeColor = new Color(50, 155, 255, 0);
        }

        public override void UpdateDead()
        {
            hatMinionCarry = 0;
            oldPosLength = 0;
            oldPosVisual = null;
        }

        public override void PostUpdateBuffs()
        {
            hatMinionCarry = 0;
            var monoxider = ModContent.ProjectileType<Projectiles.Minions.Monoxider>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.type == monoxider && p.ai[0] > 0f)
                    hatMinionCarry++;
            }
        }

        public override void UpdateVanityAccessories()
        {
            for (int i = 0; i < MAX_ARMOR; i++)
            {
                if (player.armor[i].type <= Main.maxItemTypes)
                    continue;
                bool hidden = i < 10 && player.hideVisual[i];
                if (player.armor[i].modItem is IUpdateEquipVisuals update && !hidden)
                    update.UpdateEquipVisuals(player, this, i);
            }
            if (player.GetModPlayer<AQPlayer>().monoxiderBird)
                headOverlay = (int)PlayerHeadOverlayID.MonoxideHat;
        }

        private Vector2 getCataDustSpawnPos(int gravityOffset, int headFrame)
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
            int d = Dust.NewDust(spawnPos + new Vector2(0f, -6f), 6, 6, ModContent.DustType<MonoDust>(), 0, 0, 0, cataEyeColor);
            if (Main.rand.NextBool(600))
            {
                Main.dust[d].velocity = player.velocity.RotatedBy(Main.rand.NextFloat(-0.025f, 0.025f)) * 2;
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

        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (drawInfo.shadow == 0f)
            {
                for (int i = 0; i < DYE_WRAP; i++)
                {
                    if (player.armor[i].type > Main.maxItemTypes && !player.hideVisual[i] && player.armor[i].modItem is IUpdateEquipVisuals updateVanity)
                        updateVanity.UpdateEquipVisuals(player, this, i);
                }
                for (int i = DYE_WRAP; i < MAX_ARMOR; i++)
                {
                    if (player.armor[i].type > Main.maxItemTypes && player.armor[i].modItem is IUpdateEquipVisuals updateVanity)
                        updateVanity.UpdateEquipVisuals(player, this, i);
                }
                if (oldPosLength > 1)
                {
                    if (oldPosVisual == null || oldPosVisual.Length != oldPosLength)
                        oldPosVisual = new Vector2[oldPosLength];
                    if (!Main.gameMenu && Main.instance.IsActive && !Main.gamePaused)
                    {
                        for (int i = oldPosLength - 1; i > 0; i--)
                        {
                            oldPosVisual[i] = oldPosVisual[i - 1];
                        }
                        oldPosVisual[0] = player.position;
                    }
                }
                int gravityOffset = 0;
                int headFrame = player.bodyFrame.Y / FRAME_HEIGHT;
                if (player.gravDir == -1)
                    gravityOffset = 8;
                switch ((PlayerMaskID)mask)
                {
                    case PlayerMaskID.CataMask:
                    {
                        if (cMask > 0)
                            cataEyeColor = new Color(100, 100, 100, 0);
                        if (!player.mount.Active && !player.merman && !player.wereWolf && player.statLife == player.statLifeMax2)
                        {
                            mothmanMaskSpecial = true;
                            float dustAmount = (Main.rand.Next(2, 3) + 1) * ModContent.GetInstance<AQConfigClient>().EffectQuality;
                            if (dustAmount < 1f)
                            {
                                if (Main.rand.NextFloat(dustAmount) > 0.1f)
                                    CataEyeDust(getCataDustSpawnPos(gravityOffset, headFrame));
                            }
                            else
                            {
                                var spawnPos = getCataDustSpawnPos(gravityOffset, headFrame);
                                for (int i = 0; i < dustAmount; i++)
                                {
                                    CataEyeDust(spawnPos);
                                }
                            }
                        }
                    }
                    break;
                }
            }
        }

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            var aQPlayer = drawInfo.drawPlayer.GetModPlayer<AQPlayer>();
            var drawPlayer = drawInfo.drawPlayer.GetModPlayer<GraphicsPlayer>();
            if (aQPlayer.blueSpheres)
            {
                celesteTorusPositions = new Vector3[AQPlayer.MaxCelesteTorusOrbs];
                for (int i = 0; i < AQPlayer.MaxCelesteTorusOrbs; i++)
                {
                    celesteTorusPositions[i] = aQPlayer.GetCelesteTorusPositionOffset(i);
                }
            }
        }

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            int i = layers.FindIndex((p) => p.mod.Equals("Terraria") && p.Name.Equals("Head"));
            if (i != -1)
            {
                PlayerDrawLayerInstances.postDrawHead.visible = true;
                layers.Insert(i + 1, PlayerDrawLayerInstances.postDrawHead);
            }
            i = layers.FindIndex((p) => p.mod.Equals("Terraria") && p.Name.Equals("Body"));
            if (i != -1)
            {
                PlayerDrawLayerInstances.postDrawBody.visible = true;
                layers.Insert(i + 1, PlayerDrawLayerInstances.postDrawBody);
            }
            i = layers.FindIndex((p) => p.mod.Equals("Terraria") && p.Name.Equals("HeldItem"));
            if (i != -1)
            {
                PlayerDrawLayerInstances.postDrawHeldItem.visible = true;
                layers.Insert(i + 1, PlayerDrawLayerInstances.postDrawHeldItem);
            }
            i = layers.FindIndex((p) => p.mod.Equals("Terraria") && p.Name.Equals("Wings"));
            if (i != -1)
            {
                PlayerDrawLayerInstances.postDrawWings.visible = true;
                layers.Insert(i + 1, PlayerDrawLayerInstances.postDrawWings);
            }
            layers.Insert(0, PlayerDrawLayerInstances.preDraw);
            layers.Add(PlayerDrawLayerInstances.postDraw);
        }

        public override void ModifyDrawHeadLayers(List<PlayerHeadLayer> layers)
        {
            layers.Add(PlayerDrawLayerInstances.postDrawHeadHead);
        }
    }
}