using AQMod.Assets;
using AQMod.Content.Players;
using AQMod.Items;
using AQMod.Localization;
using AQMod.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace AQMod
{
    public static class AQUtils
    {
        public static Vector2 TrueMouseworld => Vector2.Transform(Main.ReverseGravitySupport(Main.MouseScreen, 0f), Matrix.Invert(Main.GameViewMatrix.ZoomMatrix)) + Main.screenPosition;

        public static class Perspective
        {
            public const float Z_VIEW = -20f;

            internal static Vector2 GetParralaxPosition(Vector2 origin, float z)
            {
                var viewPos = new Vector2(Main.screenPosition.X + Main.screenWidth / 2f, Main.screenPosition.Y + Main.screenHeight / 2f);
                return new Vector2(origin.X - (1f - (-Z_VIEW / (z - Z_VIEW))) * (origin.X - viewPos.X), origin.Y - (1f - (-Z_VIEW / (z - Z_VIEW))) * (origin.Y - viewPos.Y));
            }

            public static float GetParralaxScale(float originalScale, float z)
            {
                return originalScale * (-Z_VIEW / (z - Z_VIEW));
            }
        }

        public sealed class BatchData 
        {
            private static FieldInfo spriteSortModeField;
            private static FieldInfo blendStateField;
            private static FieldInfo depthStencilStateField;
            private static FieldInfo rasterizerStateField;
            private static FieldInfo samplerStateField;
            private static FieldInfo customEffectField;
            private static FieldInfo transformMatrixField;

            public SpriteSortMode spriteSortMode;
            public BlendState blendState;
            public DepthStencilState depthStencilState;
            public RasterizerState rasterizerState;
            public SamplerState samplerState;
            public Effect customEffect;
            public Matrix transformMatrix;

            public BatchData(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, 
                RasterizerState rasterizerState, Effect effect, Matrix transformMatrix)
            {
                spriteSortMode = sortMode;
                this.blendState = blendState;
                this.samplerState = samplerState;
                this.depthStencilState = depthStencilState;
                this.rasterizerState = rasterizerState;
                customEffect = effect;
                this.transformMatrix = transformMatrix;
            }

            public BatchData(SpriteBatch spriteBatch) : 
                this(spriteSortModeField.GetValue<SpriteSortMode>(spriteBatch),
                    blendStateField.GetValue<BlendState>(spriteBatch),
                    samplerStateField.GetValue<SamplerState>(spriteBatch),
                    depthStencilStateField.GetValue<DepthStencilState>(spriteBatch),
                    rasterizerStateField.GetValue<RasterizerState>(spriteBatch),
                    customEffectField.GetValue<Effect>(spriteBatch),
                    transformMatrixField.GetValue<Matrix>(spriteBatch))
            {
            }

            public void Begin(SpriteBatch spriteBatch)
            {
                spriteBatch.Begin(spriteSortMode, blendState, samplerState, depthStencilState, rasterizerState, customEffect, transformMatrix);
            }

            internal static void Load()
            {
                var t = typeof(SpriteBatch);
                var flags = BindingFlags.NonPublic | BindingFlags.Instance;
                spriteSortModeField = t.GetField(nameof(spriteSortMode), flags);
                blendStateField = t.GetField(nameof(blendState), flags);
                samplerStateField = t.GetField(nameof(samplerState), flags);
                depthStencilStateField = t.GetField(nameof(depthStencilState), flags);
                rasterizerStateField = t.GetField(nameof(rasterizerState), flags);
                customEffectField = t.GetField(nameof(customEffect), flags);
                transformMatrixField = t.GetField(nameof(transformMatrix), flags);
            }

            internal static void Unload()
            {
                spriteSortModeField = null;
                blendStateField = null;
                samplerStateField = null;
                depthStencilStateField = null;
                rasterizerStateField = null;
                customEffectField = null;
                transformMatrixField = null;
            }
        }

        public struct ArrayInterpreter<T>
        {
            public T[] Arr;

            public ArrayInterpreter(T value)
            {
                Arr = new T[1] { value };
            }

            public ArrayInterpreter(T[] value)
            {
                Arr = value;
            }

            public static implicit operator ArrayInterpreter<T>(T value)
            {
                return new ArrayInterpreter<T>(value);
            }

            public static implicit operator ArrayInterpreter<T>(T[] value)
            {
                return new ArrayInterpreter<T>(value);
            }
        }

        internal struct ItemGlowmask : GlowmaskData.IWorld, GlowmaskData.IInventory, GlowmaskData.IPlayerHeld
        {
            private Func<Color> getColor;

            public ItemGlowmask(Func<Color> getColor)
            {
                this.getColor = getColor;
            }

            private Color GetColor()
            {
                if (getColor != null)
                    return getColor();
                return new Color(250, 250, 250, 0);
            }

            void GlowmaskData.IWorld.Draw(GlowmaskData glowmask, Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
            {
                var drawCoordinates = new Vector2(item.position.X - Main.screenPosition.X + glowmask.Tex.Width / 2 + item.width / 2 - glowmask.Tex.Width / 2, item.position.Y - Main.screenPosition.Y + glowmask.Tex.Height / 2 + item.height - glowmask.Tex.Height + 2f);
                var drawFrame = new Rectangle(0, 0, glowmask.Tex.Width, glowmask.Tex.Height);
                var drawRotation = rotation;
                var origin = Main.itemTexture[item.type].Size() / 2;
                var drawData = new DrawData(glowmask.Tex, drawCoordinates, drawFrame, GetColor(), drawRotation, origin, scale, SpriteEffects.None, 0);
                drawData.Draw(Main.spriteBatch);
            }

            void GlowmaskData.IInventory.Draw(GlowmaskData glowmask, Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
            {
                Main.spriteBatch.Draw(glowmask.Tex, position, frame, GetColor(), 0f, origin, scale, SpriteEffects.None, 0f);
            }

            void GlowmaskData.IPlayerHeld.Draw(GlowmaskData glowmask, Player player, AQPlayer aQPlayer, Item item, PlayerDrawInfo info)
            {
                var texture = glowmask.Tex;
                if (item.useStyle == ItemUseStyleID.HoldingOut)
                {
                    if (Item.staff[item.type])
                    {
                        float drawRotation3 = info.drawPlayer.itemRotation + 0.785f * info.drawPlayer.direction;
                        int offsetX1 = 0;
                        int offsetY = 0;
                        var origin3 = new Vector2(0f, Main.itemTexture[info.drawPlayer.inventory[info.drawPlayer.selectedItem].type].Height);
                        if (info.drawPlayer.gravDir == -1f)
                        {
                            if (info.drawPlayer.direction == -1)
                            {
                                drawRotation3 += 1.57f;
                                origin3 = new Vector2(Main.itemTexture[info.drawPlayer.inventory[info.drawPlayer.selectedItem].type].Width, 0f);
                                offsetX1 -= Main.itemTexture[info.drawPlayer.inventory[info.drawPlayer.selectedItem].type].Width;
                            }
                            else
                            {
                                drawRotation3 -= 1.57f;
                                origin3 = Vector2.Zero;
                            }
                        }
                        else if (info.drawPlayer.direction == -1)
                        {
                            origin3 = new Vector2(Main.itemTexture[info.drawPlayer.inventory[info.drawPlayer.selectedItem].type].Width, Main.itemTexture[info.drawPlayer.inventory[info.drawPlayer.selectedItem].type].Height);
                            offsetX1 -= Main.itemTexture[info.drawPlayer.inventory[info.drawPlayer.selectedItem].type].Width;
                        }
                        Vector2 holdoutOrigin = Vector2.Zero;
                        ItemLoader.HoldoutOrigin(info.drawPlayer, ref holdoutOrigin);
                        var drawCoordinates3 = new Vector2((int)(info.itemLocation.X - Main.screenPosition.X + origin3.X + offsetX1), (int)(info.itemLocation.Y - Main.screenPosition.Y + offsetY));
                        var drawFrame3 = new Rectangle(0, 0, texture.Width, texture.Height);
                        origin3 += holdoutOrigin;
                        Main.playerDrawData.Add(new DrawData(texture, drawCoordinates3, drawFrame3, GetColor(), drawRotation3, origin3, item.scale, info.spriteEffects, 0));
                        return;
                    }
                    var spriteEffects = (SpriteEffects)(player.gravDir != 1f ? player.direction != 1 ? 3 : 2 : player.direction != 1 ? 1 : 0);
                    var offset = new Vector2(texture.Width / 2, texture.Height / 2);
                    Vector2 holdoutOffset = item.modItem.HoldoutOffset().GetValueOrDefault(new Vector2(10f, 0f)) * player.gravDir;
                    int offsetX = (int)holdoutOffset.X;
                    offset.Y += holdoutOffset.Y;
                    var origin2 = player.direction == -1 ? new Vector2(texture.Width + offsetX, texture.Height / 2) : new Vector2(-offsetX, texture.Height / 2);
                    var drawCoordinates2 = new Vector2((int)(player.itemLocation.X - Main.screenPosition.X + offset.X), (int)(player.itemLocation.Y - Main.screenPosition.Y + offset.Y));
                    var drawFrame2 = new Rectangle(0, 0, texture.Width, texture.Height);
                    var drawRotation2 = player.itemRotation;
                    Main.playerDrawData.Add(new DrawData(texture, drawCoordinates2, drawFrame2, GetColor(), drawRotation2, origin2, item.scale, spriteEffects, 0));
                    return;
                }
                if (player.gravDir == -1f)
                {
                    var drawCoordinates2 = new Vector2((int)(info.itemLocation.X - Main.screenPosition.X), (int)(info.itemLocation.Y - Main.screenPosition.Y));
                    var drawFrame2 = new Rectangle(0, 0, texture.Width, texture.Height);
                    var drawRotation2 = player.itemRotation;
                    var origin2 = new Vector2(texture.Width * 0.5f - texture.Width * 0.5f * player.direction, 0f);
                    Main.playerDrawData.Add(new DrawData(texture, drawCoordinates2, drawFrame2, GetColor(), drawRotation2, origin2, item.scale, info.spriteEffects, 0));
                    return;
                }
                var drawCoordinates = new Vector2((int)(info.itemLocation.X - Main.screenPosition.X), (int)(info.itemLocation.Y - Main.screenPosition.Y));
                var drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
                var drawRotation = player.itemRotation;
                var origin = new Vector2(texture.Width * 0.5f - texture.Width * 0.5f * player.direction, texture.Height);
                Main.playerDrawData.Add(new DrawData(texture, drawCoordinates, drawFrame, GetColor(), drawRotation, origin, item.scale, info.spriteEffects, 0));
            }
        }

        public static bool IsTalkingTo<T>(this Player player) where T : ModNPC
        {
            return IsTalkingTo(player, ModContent.NPCType<T>());
        }
        public static bool IsTalkingTo(this Player player, int npcType)
        {
            return player.talkNPC != -1 && Main.npc[player.talkNPC].type == npcType;
        }

        public static T GetValue<T>(this FieldInfo field, object obj)
        {
            return (T)field.GetValue(obj);
        }

        public static bool IsReferenceType(Type type)
        {
            return !type.IsValueType && !type.IsEnum && type != typeof(string);
        }

        public static T DeepCopy<T>(this T obj)
        {
            return DeepCopyTo(obj, (T)Activator.CreateInstance(typeof(T)), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }
        public static T DeepCopy<T>(this T obj, BindingFlags flags)
        {
            return DeepCopyTo(obj, (T)Activator.CreateInstance(typeof(T)), flags);
        }
        public static T DeepCopyTo<T>(this T obj, T myObj)
        {
            return DeepCopyTo(obj, myObj, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }
        public static T DeepCopyTo<T>(this T obj, T myObj, BindingFlags flags)
        {
            Type t = obj.GetType();
            var l = AQMod.GetInstance().Logger;
            var fields = t.GetFields(flags);
            //l.Debug("writing fields");
            foreach (var f in fields)
            {
                //l.Debug(f.FieldType.Name + " " + f.Name);
                if (!IsReferenceType(f.FieldType))
                {
                    f.SetValue(myObj, f.GetValue(obj));
                }
                else
                {
                    object value = f.GetValue(obj);
                    if (value == null)
                    {
                        f.SetValue(myObj, null);
                    }
                    else if (value.GetType() == typeof(Array))
                    {
                        f.SetValue(myObj, DeepCopyArray((Array)value));
                    }
                    else
                    {
                        try
                        {
                            var newValue = Activator.CreateInstance(value.GetType());
                            f.SetValue(myObj, value.DeepCopyTo(newValue));
                        }
                        catch (Exception ex)
                        {
                            l.Error("Error when cloning field {" + f.FieldType.Name + " " + f.Name + "}", ex);
                            f.SetValue(myObj, null);
                        }
                    }
                }
            }
            var properties = t.GetProperties(flags);
            //l.Debug("writing properties");
            foreach (var p in properties)
            {
                //l.Debug(p.PropertyType.Name + " " + p.Name);
                if (p.CanWrite)
                {
                    if (!IsReferenceType(p.PropertyType))
                    {
                        p.SetValue(myObj, p.GetValue(obj, null), null);
                    }
                    else
                    {
                        object value = p.GetValue(obj, null);
                        if (value == null)
                        {
                            p.SetValue(myObj, null, null);
                        }
                        else
                        {
                            try
                            {
                                var newValue = Activator.CreateInstance(value.GetType());
                                p.SetValue(myObj, value.DeepCopyTo(newValue), null);
                            }
                            catch (Exception ex)
                            {
                                l.Error("Error when cloning property {" + p.PropertyType.Name + " " + p.Name + "}", ex);
                                p.SetValue(myObj, null, null);
                            }
                        }
                    }
                }
            }
            return myObj;
        }
        public static object DeepCopyArray(Array array)
        {
            return null;
        }

        public static void DrawPlayerFull(Player player)
        {
            if (Main.gamePaused)
            {
                player.PlayerFrame();
            }
            if (player.ghost)
            {
                return;
            }
            Vector2 position = default(Vector2);
            if (player.inventory[player.selectedItem].flame || player.head == 137 || player.wings == 22)
            {
                player.itemFlameCount--;
                if (player.itemFlameCount <= 0)
                {
                    player.itemFlameCount = 5;
                    for (int k = 0; k < 7; k++)
                    {
                        player.itemFlamePos[k].X = (float)Main.rand.Next(-10, 11) * 0.15f;
                        player.itemFlamePos[k].Y = (float)Main.rand.Next(-10, 1) * 0.35f;
                    }
                }
            }
            if (player.armorEffectDrawShadowEOCShield)
            {
                int num = player.eocDash / 4;
                if (num > 3)
                {
                    num = 3;
                }
                for (int l = 0; l < num; l++)
                {
                    Main.instance.DrawPlayer(player, player.shadowPos[l], player.shadowRotation[l], player.shadowOrigin[l], 0.5f + 0.2f * (float)l);
                }
            }
            if (player.invis)
            {
                player.armorEffectDrawOutlines = false;
                player.armorEffectDrawShadow = false;
                player.armorEffectDrawShadowSubtle = false;
                position = player.position;
                if (player.aggro <= -750)
                {
                    Main.instance.DrawPlayer(player, position, player.fullRotation, player.fullRotationOrigin, 1f);
                }
                else
                {
                    player.invis = false;
                    Main.instance.DrawPlayer(player, position, player.fullRotation, player.fullRotationOrigin);
                    player.invis = true;
                }
            }
            if (player.armorEffectDrawOutlines)
            {
                _ = player.position;
                if (!Main.gamePaused)
                {
                    player.ghostFade += player.ghostDir * 0.075f;
                }
                if ((double)player.ghostFade < 0.1)
                {
                    player.ghostDir = 1f;
                    player.ghostFade = 0.1f;
                }
                else if ((double)player.ghostFade > 0.9)
                {
                    player.ghostDir = -1f;
                    player.ghostFade = 0.9f;
                }
                float num5 = player.ghostFade * 5f;
                for (int m = 0; m < 4; m++)
                {
                    float num6;
                    float num7;
                    switch (m)
                    {
                        default:
                            num6 = num5;
                            num7 = 0f;
                            break;
                        case 1:
                            num6 = 0f - num5;
                            num7 = 0f;
                            break;
                        case 2:
                            num6 = 0f;
                            num7 = num5;
                            break;
                        case 3:
                            num6 = 0f;
                            num7 = 0f - num5;
                            break;
                    }
                    position = new Vector2(player.position.X + num6, player.position.Y + player.gfxOffY + num7);
                    Main.instance.DrawPlayer(player, position, player.fullRotation, player.fullRotationOrigin, player.ghostFade);
                }
            }
            if (player.armorEffectDrawOutlinesForbidden)
            {
                _ = player.position;
                if (!Main.gamePaused)
                {
                    player.ghostFade += player.ghostDir * 0.025f;
                }
                if ((double)player.ghostFade < 0.1)
                {
                    player.ghostDir = 1f;
                    player.ghostFade = 0.1f;
                }
                else if ((double)player.ghostFade > 0.9)
                {
                    player.ghostDir = -1f;
                    player.ghostFade = 0.9f;
                }
                float num8 = player.ghostFade * 5f;
                for (int n = 0; n < 4; n++)
                {
                    float num9;
                    float num10;
                    switch (n)
                    {
                        default:
                            num9 = num8;
                            num10 = 0f;
                            break;
                        case 1:
                            num9 = 0f - num8;
                            num10 = 0f;
                            break;
                        case 2:
                            num9 = 0f;
                            num10 = num8;
                            break;
                        case 3:
                            num9 = 0f;
                            num10 = 0f - num8;
                            break;
                    }
                    position = new Vector2(player.position.X + num9, player.position.Y + player.gfxOffY + num10);
                    Main.instance.DrawPlayer(player, position, player.fullRotation, player.fullRotationOrigin, player.ghostFade);
                }
            }
            if (player.armorEffectDrawShadowBasilisk)
            {
                int num11 = (int)(player.basiliskCharge * 3f);
                for (int num12 = 0; num12 < num11; num12++)
                {
                    Main.instance.DrawPlayer(player, player.shadowPos[num12], player.shadowRotation[num12], player.shadowOrigin[num12], 0.5f + 0.2f * (float)num12);
                }
            }
            else if (player.armorEffectDrawShadow)
            {
                for (int num2 = 0; num2 < 3; num2++)
                {
                    Main.instance.DrawPlayer(player, player.shadowPos[num2], player.shadowRotation[num2], player.shadowOrigin[num2], 0.5f + 0.2f * (float)num2);
                }
            }
            if (player.armorEffectDrawShadowLokis)
            {
                for (int num3 = 0; num3 < 3; num3++)
                {
                    Main.instance.DrawPlayer(player, Vector2.Lerp(player.shadowPos[num3], player.position + new Vector2(0f, player.gfxOffY), 0.5f), player.shadowRotation[num3], player.shadowOrigin[num3], MathHelper.Lerp(1f, 0.5f + 0.2f * (float)num3, 0.5f));
                }
            }
            if (player.armorEffectDrawShadowSubtle)
            {
                for (int num4 = 0; num4 < 4; num4++)
                {
                    position.X = player.position.X + (float)Main.rand.Next(-20, 21) * 0.1f;
                    position.Y = player.position.Y + (float)Main.rand.Next(-20, 21) * 0.1f + player.gfxOffY;
                    Main.instance.DrawPlayer(player, position, player.fullRotation, player.fullRotationOrigin, 0.9f);
                }
            }
            if (player.shadowDodge)
            {
                player.shadowDodgeCount += 1f;
                if (player.shadowDodgeCount > 30f)
                {
                    player.shadowDodgeCount = 30f;
                }
            }
            else
            {
                player.shadowDodgeCount -= 1f;
                if (player.shadowDodgeCount < 0f)
                {
                    player.shadowDodgeCount = 0f;
                }
            }
            if (player.shadowDodgeCount > 0f)
            {
                _ = player.position;
                position.X = player.position.X + player.shadowDodgeCount;
                position.Y = player.position.Y + player.gfxOffY;
                Main.instance.DrawPlayer(player, position, player.fullRotation, player.fullRotationOrigin, 0.5f + (float)Main.rand.Next(-10, 11) * 0.005f);
                position.X = player.position.X - player.shadowDodgeCount;
                Main.instance.DrawPlayer(player, position, player.fullRotation, player.fullRotationOrigin, 0.5f + (float)Main.rand.Next(-10, 11) * 0.005f);
            }
            position = player.position;
            position.Y += player.gfxOffY;
            //if (player.stoned)
            //{
            //    Main.instance.DrawPlayerStoned(player, position);
            //}
            // else if (!player.invis)
            if (!player.invis)
            {
                Main.instance.DrawPlayer(player, position, player.fullRotation, player.fullRotationOrigin);
            }
        }

        public static void FillOther<T>(this T[] arr, T[] arr2, int start = 0)
        {
            int length = arr.Length > arr2.Length ? arr.Length : arr2.Length;
            if (length + start > arr.Length)
            {
                length = arr.Length - length;
            }
            for (int i = start; i < length; i++)
            {
                arr2[i] = arr[i];
            }
        }

        public static int ShootProj(Player player, Item item, Vector2 location, Vector2 velocity, int projType, int projDamage, float projKB, Vector2? setMousePos)
        {
            var mouseScreen = Main.MouseScreen;

            if (setMousePos != null)
            {
                var mousePos = setMousePos.Value - Main.screenPosition;
                Main.mouseX = (int)mousePos.X;
                Main.mouseX = (int)mousePos.Y;
            }

            int result;
            if (PlayerHooks.Shoot(player, item, ref location, ref velocity.X, ref velocity.Y, ref projType, ref projDamage, ref projKB) &&
                ItemLoader.Shoot(item, player, ref location, ref velocity.X, ref velocity.Y, ref projType, ref projDamage, ref projKB))
            {
                result = Projectile.NewProjectile(location, velocity, projType, projDamage, projKB, player.whoAmI);
            }
            else
            {
                result = -2;
            }

            Main.mouseX = (int)mouseScreen.X;
            Main.mouseY = (int)mouseScreen.Y;
            return result;
        }

        public static bool AddUnless<T>(this List<T> list, T thingToAdd, T unless = default(T))
        {
            if (thingToAdd.Equals(unless))
            {
                return false;
            }
            list.Add(thingToAdd);
            return true;
        }

        public static string TypeName<T>()
        {
            return TypeName(typeof(T));
        }

        public static string TypeName(this Type type)
        {
            if (type.DeclaringType == null)
                return type.Name;

            return TypeName(type.DeclaringType) + "." + type.Name;
        }

        public static int CheckForPlayers(Rectangle rectangle)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead && rectangle.Intersects(Main.player[i].getRect()))
                {
                    return i;
                }
            }
            return -1;
        }

        public static Color MultColorsThenDiv(Color color1, Color color2)
        {
            return new Color(color1.R / 255f * (color2.R / 255f), color1.G / 255f * (color2.G / 255f), color1.B / 255f * (color2.B / 255f), color1.A / 255f * (color2.A / 255f));
        }

        public static float Brightness(this Color color)
        {
            return (color.R + color.G + color.B) / (255f * 3f);
        }

        public static HashSet<T> Combine<T>(params HashSet<T>[] sets)
        {
            var value = new HashSet<T>();
            foreach (var set in sets)
            {
                foreach (var item in set)
                {
                    if (!value.Contains(item))
                    {
                        value.Add(item);
                        //AQMod.GetInstance().Logger.Debug(item);
                    }
                }
            }
            return value;
        }

        public static List<T> ToList<T>(this T[] arr)
        {
            var list = new List<T>();
            for (int i = 0; i < arr.Length; i++)
            {
                //AQMod.GetInstance().Logger.Debug("{i:" + i + ", " + arr[i] + "}");
                list.Add(arr[i]);
            }

            return list;
        }

        public static string GetTextValue(params ValueTuple<GameCulture, string>[] values)
        {
            string english = "Invalid Name";
            foreach (var t in values)
            {
                if (t.Item1.LegacyId == Language.ActiveCulture.LegacyId)
                {
                    return t.Item2;
                }
                if (t.Item1.LegacyId == GameCulture.English.LegacyId)
                {
                    english = t.Item2;
                }
            }
            return english;
        }

        public static ModTranslation Add(this ModTranslation text, string value, GameCulture culture)
        {
            text.AddTranslation(culture, value);
            return text;
        }

        public static void Glowmask(this ModItem item)
        {
            if (!Main.dedServ)
            {
                Glowmask(item, new Color(250, 250, 250, 0));
            }
        }

        public static void Glowmask(this ModItem item, Color brightness, bool inv = false)
        {
            if (!Main.dedServ)
            {
                var glowmask = new ItemGlowmask(() => brightness);
                CustomGlowmask(item, glowmask, inv ? glowmask : ((GlowmaskData.IInventory)null), glowmask);
            }
        }

        public static void Glowmask(this ModItem item, Func<Color> brightness)
        {
            if (!Main.dedServ)
            {
                var glowmask = new ItemGlowmask(brightness);
                CustomGlowmask(item, glowmask, null, glowmask);
            }
        }

        public static void Glowmask(this ModItem item, Texture2D texture)
        {
            if (!Main.dedServ)
            {
                Glowmask(item, texture, new Color(250, 250, 250, 0));
            }
        }

        public static void Glowmask(this ModItem item, Texture2D texture, Color brightness)
        {
            if (!Main.dedServ)
            {
                var glowmask = new ItemGlowmask(() => brightness);
                CustomGlowmask(item, texture, glowmask, null, glowmask);
            }
        }

        public static void Glowmask(this ModItem item, Texture2D texture, Func<Color> brightness)
        {
            if (!Main.dedServ)
            {
                var glowmask = new ItemGlowmask(brightness);
                CustomGlowmask(item, texture, glowmask, null, glowmask);
            }
        }

        internal static void CustomGlowmask(this ModItem item, GlowmaskData.IWorld world = null, GlowmaskData.IInventory inv = null, GlowmaskData.IPlayerHeld held = null)
        {
            if (!Main.dedServ)
            {
                CustomGlowmask(item, ModContent.GetTexture(item.GetPath("_Glow")), world, inv, held);
            }
        }

        internal static void CustomGlowmask(this ModItem item, Texture2D texture, GlowmaskData.IWorld world = null, GlowmaskData.IInventory inv = null, GlowmaskData.IPlayerHeld held = null)
        {
            if (!Main.dedServ)
            {
                if (GlowmaskData.ItemToGlowmask == null)
                {
                    GlowmaskData.ItemToGlowmask = new Dictionary<int, GlowmaskData>();
                }
                GlowmaskData.ItemToGlowmask.Add(item.item.type, new GlowmaskData(texture, world, inv, held));
            }
        }

        public static Item Instance<T>(bool newInstance = true) where T : ModItem
        {
            return Instance(ModContent.ItemType<T>(), newInstance: newInstance);
        }

        public static Item Instance(int type, bool newInstance = true)
        {
            if (!newInstance && type >= Main.maxItemTypes)
            {
                return ItemLoader.GetItem(type).item;
            }
            var item = new Item();
            item.SetDefaults(type);
            return item;
        }

        public static int AmtAccSlots(this Player player)
        {
            return 8 + player.extraAccessorySlots;
        }

        public const int GrapplingHookIndex = 4;
        public static Item GrapplingHook(this Player player)
        {
            return player.miscEquips[GrapplingHookIndex];
        }

        public static byte MultClamp(byte b, float mult)
        {
            return MultClamp(b, mult, byte.MinValue, byte.MaxValue);
        }

        public static byte MultClamp(byte b, float mult, byte max)
        {
            return MultClamp(b, mult, byte.MinValue, max);
        }

        public static byte MultClamp(byte b, float mult, byte min, byte max)
        {
            return (byte)MathHelper.Clamp(b * mult, min, max);
        }

        public static TextureAsset GetTextureAsset(this Mod mod, string path)
        {
            return new TextureAsset(mod, path);
        }

        public static NoHitting NoHit(this NPC npc)
        {
            return npc.GetGlobalNPC<NoHitting>();
        }

        public static float FromByte(byte value, float maximum)
        {
            return value * maximum / 255f;
        }
        public static float FromByte(byte value, float minimum, float maximum)
        {
            return minimum + value * (maximum - minimum) / 255f;
        }

        public static T GetValueOrDefault<T>(object value, T defaultValue)
        {
            return value != null && (value is T wantedValue) ? wantedValue : default;
        }

        public static void RemoveRepeatingIndices(List<int> list)
        {
            var indexList = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                if (indexList.Contains(list[i]))
                {
                    list.RemoveAt(i);
                    i--;
                }
                else
                {
                    indexList.Add(list[i]);
                }
            }
        }

        public static PlayerDrawEffects FX(this Player player)
        {
            return player.GetModPlayer<PlayerDrawEffects>();
        }

        public static PlayerBiomes Biomes(this Player player)
        {
            return player.GetModPlayer<PlayerBiomes>();
        }

        public static List<T> CutNullIndicesToList<T>(this T[] arr) where T : class
        {
            var list = new List<T>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != null)
                    list.Add(arr[i]);
            }
            return list;
        }

        public static T[] CutNullIndices<T>(this T[] arr) where T : class
        {
            return CutNullIndicesToList(arr).ToArray();
        }

        public static T2[] Convert<T, T2>(this T[] arr, Func<T, T2> toOtherType)
        {
            var arr2 = new T2[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr2[i] = toOtherType(arr[i]);
            }
            return arr2;
        }

        public static int DoCount<T>(this T[] arr, Func<T, bool> validItem)
        {
            int count = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (validItem(arr[i]))
                    count++;
            }
            return count;
        }

        public static Rectangle KeepInWorld(this Rectangle rectangle, int fluff = 10)
        {
            if (rectangle.X < fluff)
            {
                rectangle.X = fluff;
            }
            else if (rectangle.X + rectangle.Width > Main.maxTilesX - fluff)
            {
                rectangle.X = Main.maxTilesX - fluff - rectangle.Width;
            }
            if (rectangle.Y < fluff)
            {
                rectangle.Y = fluff;
            }
            else if (rectangle.Y + rectangle.Height > Main.maxTilesY - fluff)
            {
                rectangle.Y = Main.maxTilesY - fluff - rectangle.Height;
            }
            return rectangle;
        }

        public static string SpillArray<T>(this T[] array)
        {
            string text = "Nothing is inside this array.";
            for (int i = 0; i < array.Length; i++)
            {
                if (i == 0)
                {
                    text = array[0].ToString();
                }
                else
                {
                    text += ", " + (array[i] == null ? "null value" : array[i].ToString());
                }
            }
            return text;
        }

        public static float Wave(float time, float minimum, float maximum)
        {
            return minimum + ((float)Math.Sin(time) + 1f) / 2f * (maximum - minimum);
        }

        public static void CyclePositions(Vector2[] oldPos, Vector2 newPos)
        {
            for (int i = oldPos.Length - 1; i > 0; i--)
            {
                oldPos[i] = oldPos[i - 1];
            }
            oldPos[0] = newPos;
        }

        public static Vector2[] AsAddAll(this Vector2[] v, Vector2 sub)
        {
            var clone = new Vector2[v.Length];
            for (int i = 0; i < v.Length; i++)
            {
                clone[i] = new Vector2(v[i].X + sub.X, v[i].Y + sub.Y);
            }
            return clone;
        }

        public static List<Vector2> AsAddAll(this List<Vector2> v, Vector2 sub)
        {
            var clone = new List<Vector2>();
            for (int i = 0; i < v.Count; i++)
            {
                clone.Add(new Vector2(v[i].X + sub.X, v[i].Y + sub.Y));
            }
            return clone;
        }

        public static void AddAll(this Vector2[] v, Vector2 sub)
        {
            for (int i = 0; i < v.Length; i++)
            {
                v[i] = new Vector2(v[i].X + sub.X, v[i].Y + sub.Y);
            }
        }

        public static void AddAll(this List<Vector2> v, Vector2 add)
        {
            for (int i = 0; i < v.Count; i++)
            {
                v[i] = new Vector2(v[i].X + add.X, v[i].Y + add.Y);
            }
        }

        public static Item ItemInHand(this Player player)
        {
            if (!Main.mouseItem.IsAir)
                return Main.mouseItem;
            return player.HeldItem;
        }

        public static TEnum ToEnum<TEnum>(this ushort number) where TEnum : Enum
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), number);
        }

        public static TEnum ToEnum<TEnum>(this int number) where TEnum : Enum
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), number);
        }

        public static Texture2D GetTextureobj<T>(string extra)
        {
            return ModContent.GetTexture(GetPath<T>(extra));
        }

        public static Texture2D GetTextureobj(this object obj, string extra)
        {
            return ModContent.GetTexture(obj.GetPath(extra));
        }

        public static Texture2D GetTextureobj(this Type t, string extra)
        {
            return ModContent.GetTexture(t.GetPath(extra));
        }

        public static Texture2D GetTextureobj<T>()
        {
            return typeof(T).GetTextureobj();
        }

        public static Texture2D GetTextureobj(this object obj)
        {
            return obj.GetType().GetTextureobj();
        }

        public static Texture2D GetTextureobj(this Type t)
        {
            return ModContent.GetTexture(GetPath(t));
        }

        public static void UseImageSize(this MiscShaderData data, Vector2 imageSize)
        {
            data.Shader.Parameters["uImageSize0"].SetValue(imageSize);
        }

        /// <summary>
        /// Gets a frame of the projectile's sprite. Use this only for drawing, since this uses the projectile's texture.
        /// </summary>
        /// <param name="Projectile"></param>
        /// <returns></returns>
        public static Rectangle ProjFrame(this Projectile Projectile, int frameX, int totalFramesX, int paddingX = 2, int paddingY = 2)
        {
            var texture = Projectile.GetTextureobj();
            int frameWidth = texture.Height / totalFramesX;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            return new Rectangle(frameWidth * frameX, frameHeight * Projectile.frame, frameWidth - paddingX, frameHeight - paddingY);
        }

        /// <summary>
        /// Gets a frame of the projectile's sprite. Use this only for drawing, since this uses the projectile's texture.
        /// </summary>
        /// <param name="Projectile"></param>
        /// <returns></returns>
        public static Rectangle ProjFrame(this Projectile Projectile, int padding = 2)
        {
            var texture = Projectile.GetTextureobj();
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            return new Rectangle(0, frameHeight * Projectile.frame, texture.Width, frameHeight - padding);
        }

        public static bool PositionOnScreen(Vector2 position, float size)
        {
            var normal = Vector2.Normalize(new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f) - position);
            position += normal * size;
            return PositionOnScreen(position);
        }

        public static bool PositionOnScreen(Vector2 position)
        {
            if (position.X < -20 || position.X > Main.screenWidth + 20)
                return false;
            if (position.Y < -20 || position.Y > Main.screenHeight + 20)
                return false;
            return true;
        }

        /// <summary>
        /// Gets the median of light 
        /// </summary>
        /// <param name="position">The center of the object</param>
        /// <param name="size">The size in tile coordinates</param>
        /// <returns></returns>
        public static Color GetLightingSection(Vector2 position, int size = 10)
        {
            Vector3 lighting = Vector3.Zero;
            float amount = 0f;
            int realSize = size / 2;
            Point tilePosition = position.ToTileCoordinates();
            tilePosition.Fluffize(10 + realSize);
            for (int i = tilePosition.X - realSize; i <= tilePosition.X + realSize; i++)
            {
                for (int j = tilePosition.Y - realSize; j <= tilePosition.Y + realSize; j++)
                {
                    lighting += Lighting.GetColor(i, j).ToVector3();
                    amount += 1f;
                }
            }
            if (amount == 0f)
                return Color.White;
            return new Color(lighting / amount);

        }

        public static Color Minimize(this Color color, float value)
        {
            return color.Minimize((byte)(int)(value * 255f));
        }

        public static Color Minimize(this Color color, int value)
        {
            return color.Minimize((byte)value);
        }

        public static Color Minimize(this Color color, byte value)
        {
            if (color.R < value)
                color.R = value;
            if (color.G < value)
                color.G = value;
            if (color.B < value)
                color.B = value;
            return color;
        }

        public static Point FluffizePoint(Point point, int fluff = 10)
        {
            point.Fluffize(fluff);
            return point;
        }

        public static void Fluffize(this ref Point point, int fluff = 10)
        {
            if (point.X < fluff)
            {
                point.X = fluff;
            }
            else if (point.X > Main.maxTilesX - fluff)
            {
                point.X = Main.maxTilesX - fluff;
            }
            if (point.Y < fluff)
            {
                point.Y = fluff;
            }
            else if (point.Y > Main.maxTilesY - fluff)
            {
                point.Y = Main.maxTilesY - fluff;
            }
        }

        public static Vector2 GetSwordTipOffset(Player player, Item item)
        {
            return new Vector2(item.width * player.direction, -item.height * player.gravDir).RotatedBy(player.itemRotation + player.fullRotation) * item.scale;
        }

        public static string AddZerosToUnreachedDigits(int number, int zerosCount)
        {
            if (number == 0)
            {
                string zeros = "";
                for (int i = 0; i < zerosCount; i++)
                {
                    zeros += "0";
                }
                return zeros;
            }
            int digits = number / 10 + 1;
            string text = "";
            for (int i = 0; i < zerosCount - digits; i++)
            {
                text += "0";
            }
            text += number.ToString();
            return text;
        }

        public static string TimeText2(double time)
        {
            int seconds = (int)(time / 60);
            int minutes = seconds / 60;
            seconds %= 60;
            int hours = minutes / 60;
            minutes %= 60;
            return AddZerosToUnreachedDigits(hours, 2) + ":" + AddZerosToUnreachedDigits(minutes, 2) + ":" + AddZerosToUnreachedDigits(seconds, 2);
        }

        public static string TimeText3(double time)
        {
            int seconds = (int)(time / 60);
            int minutes = seconds / 60;
            seconds %= 60;
            return AddZerosToUnreachedDigits(minutes, 2) + ":" + AddZerosToUnreachedDigits(seconds, 2);
        }

        public static string TimeText(double time)
        {
            string text = "AM";
            if (!Main.dayTime)
                time += 54000.0;
            time = time / 86400.0 * 24.0;
            time = time - 7.5 - 12.0;
            if (time < 0.0)
                time += 24.0;
            if (time >= 12.0)
                text = "PM";
            int intTime = (int)time;
            double deltaTime = time - intTime;
            deltaTime = (int)(deltaTime * 60.0);
            string text2 = string.Concat(deltaTime);
            if (deltaTime < 10.0)
                text2 = "0" + text2;
            if (intTime > 12)
                intTime -= 12;
            if (intTime == 0)
                intTime = 12;
            return string.Concat(intTime, ":", text2, " ", text);
        }

        public static int GetIntOrDefault(this TagCompound tag, string key, int defaultValue)
        {
            if (tag.ContainsKey(key))
                return tag.GetInt(key);
            return defaultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segments">Index 0 is the origin vector</param>
        /// <param name="distanceBetweenSegments">The distance between each point</param>
        /// <param name="target">The target position for the segment chain</param>
        public static Vector3[] Fabrik3D(this Vector3[] segments, float distanceBetweenSegments, Vector3 target)
        {
            var origin = segments[0];
            segments[segments.Length - 1] = target;
            for (int i = segments.Length - 1; i > 0; i--)
            {
                segments[i - 1] = segments[i] + Vector3.Normalize(segments[i - 1] - segments[i]) * distanceBetweenSegments;
            }
            segments[0] = origin;
            for (int i = 0; i < segments.Length - 1; i++)
            {
                segments[i + 1] = segments[i] + Vector3.Normalize(segments[i + 1] - segments[i]) * distanceBetweenSegments;
            }
            return segments;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segments">Index 0 is the origin vector</param>
        /// <param name="distanceBetweenSegments">The distance between each point</param>
        /// <param name="target">The target position for the segment chain</param>
        public static Vector2[] Fabrik2D(this Vector2[] segments, float distanceBetweenSegments, Vector2 target)
        {
            var origin = segments[0];
            segments[segments.Length - 1] = target;
            for (int i = segments.Length - 1; i > 0; i--)
            {
                segments[i - 1] = segments[i] + Vector2.Normalize(segments[i - 1] - segments[i]) * distanceBetweenSegments;
            }
            segments[0] = origin;
            for (int i = 0; i < segments.Length - 1; i++)
            {
                segments[i + 1] = segments[i] + Vector2.Normalize(segments[i + 1] - segments[i]) * distanceBetweenSegments;
            }
            return segments;
        }

        public static Color colorLerps(Color[] colors, float time)
        {
            int index = (int)time;
            return Color.Lerp(colors[index % colors.Length], colors[(index + 1) % colors.Length], time % 1f);
        }

        public static bool CanNPCBeHitByProjectile(NPC npc, Projectile projectile)
        {
            if (npc.dontTakeDamage || ((projectile.usesLocalNPCImmunity || projectile.usesIDStaticNPCImmunity) && (!projectile.usesLocalNPCImmunity || projectile.localNPCImmunity[npc.whoAmI] != 0) && (!projectile.usesIDStaticNPCImmunity || !Projectile.IsNPCImmune(projectile.type, npc.whoAmI))))
                return false;
            return true;
        }

        public static void Sort<T>(ref T[] array, Comparison<T> comparison)
        {
            var arrayAsList = new List<T>(array);
            arrayAsList.Sort(comparison);
            array = arrayAsList.ToArray();
        }

        public static void DrawLine(Vector2 start, Vector2 end, int width, Color color)
        {
            var difference = end - start;
            Main.spriteBatch.Draw(Tex.Pixel, start, null, color, difference.ToRotation() - MathHelper.PiOver2, new Vector2(0.5f, 0f), new Vector2(width, difference.Length()), SpriteEffects.None, 0f);
        }

        public static T[][] CreateSameLengthArrayArray<T>(int length1, int length2)
        {
            var array = new T[length1][];
            for (int i = 0; i < length1; i++)
            {
                array[i] = new T[length2];
            }
            return array;
        }

        public static byte[] ObjectToByteArray(this object obj)
        {
            if (obj == null)
                return null;
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        public static string GetPath2<T>(this T type, string extra) where T : class
        {
            return GetPath2<T>() + extra;
        }


        /// <summary>
        /// Similar to GetPath, except it tries to get an instance of the class T and check if it has a "Name" property, if so, use that as the end of the path instead of the type name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetPath2<T>(this T type) where T : class
        {
            return GetPath2<T>();
        }

        public static string GetPath2<T>(string extra) where T : class
        {
            return GetPath2<T>() + extra;
        }


        /// <summary>
        /// Similar to GetPath, except it tries to get an instance of the class T and check if it has a "Name" property, if so, use that as the end of the path instead of the type name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetPath2<T>() where T : class
        {
            return typeof(T).Namespace.Replace('.', '/') + "/" + GetName2<T>();
        }

        public static string GetName2<T>(this T type) where T : class
        {
            return GetName2<T>();
        }

        public static string GetName2<T>() where T : class
        {
            try
            {
                var instance = ModContent.GetInstance<T>();
                if (instance != null)
                {
                    var nameField = typeof(T).GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
                    if (nameField != null)
                    {
                        return (string)nameField.GetGetMethod(nonPublic: false).Invoke(instance, null);
                    }
                }
            }
            catch
            {

            }
            return typeof(T).Name;
        }

        public static string GetPath<T>()
        {
            return GetPath(typeof(T));
        }

        public static string GetPath<T>(string extra)
        {
            return GetPath(typeof(T), extra);
        }

        public static string GetPath(this object o)
        {
            return GetPath(o.GetType());
        }

        public static string GetPath(this object o, string extra)
        {
            return GetPath(o.GetType(), extra);
        }

        public static string GetPath(Type t)
        {
            return t.Namespace.Replace('.', '/') + "/" + t.Name;
        }

        public static string GetPath(Type t, string extra)
        {
            return GetPath(t) + extra;
        }

        public static bool Check2x2ThenCut(int x, int y)
        {
            if ((!Framing.GetTileSafely(x, y).active() || Main.tileCut[Main.tile[x, y].type]) &&
                (!Framing.GetTileSafely(x + 1, y).active() || Main.tileCut[Main.tile[x + 1, y].type]) &&
                (!Framing.GetTileSafely(x, y + 1).active() || Main.tileCut[Main.tile[x, y + 1].type]) &&
                (!Framing.GetTileSafely(x + 1, y + 1).active() || Main.tileCut[Main.tile[x + 1, y + 1].type]))
            {
                WorldGen.KillTile(x, y);
                WorldGen.KillTile(x + 1, y);
                WorldGen.KillTile(x, y + 1);
                WorldGen.KillTile(x + 1, y + 1);
                return true;
            }
            return false;
        }

        public static bool IsCloseEnoughTo(this float comparison, float intendedValue, float closeEnoughMargin = 1f)
        {
            return (comparison - intendedValue).Abs() <= closeEnoughMargin;
        }

        public static string Info_GetDepthMeter(float worldY)
        {
            int depth = (int)(worldY * 2f / 16f - Main.worldSurface * 2.0);
            float num17 = Main.maxTilesX / 4200;
            num17 *= num17;
            int num18 = 1200;
            float space = (float)(((Main.screenPosition.Y + Main.screenHeight / 2) / 16f - (65f + 10f * num17)) / (Main.worldSurface / 5.0));
            string textValue = worldY > (Main.maxTilesY - 204) * 16 ? Language.GetTextValue("GameUI.LayerUnderworld") : worldY > Main.rockLayer * 16.0 + num18 / 2 + 16.0 ? Language.GetTextValue("GameUI.LayerCaverns") : depth > 0 ? Language.GetTextValue("GameUI.LayerUnderground") : !(space >= 1f) ? Language.GetTextValue("GameUI.LayerSpace") : Language.GetTextValue("GameUI.LayerSurface");
            depth = Math.Abs(depth);
            return (depth != 0 ? Language.GetTextValue("GameUI.Depth", depth) : Language.GetTextValue("GameUI.DepthLevel")) + " " + textValue;
        }

        public static string Info_GetCompass(float worldX)
        {
            int x = (int)(worldX * 2f / 16f - Main.maxTilesX);
            return x > 0 ? Language.GetTextValue("GameUI.CompassEast", x) : x >= 0 ? Language.GetTextValue("GameUI.CompassCenter") : Language.GetTextValue("GameUI.CompassWest", -x);
        }

        public static Color ToColor(this Vector4 value)
        {
            return new Color(value.X, value.Y, value.Z, value.W);
        }

        public static bool HasDoubleJumpLeft(this Player player)
        {
            return player.jumpAgainCloud || player.jumpAgainBlizzard || player.jumpAgainSandstorm || player.jumpAgainFart || player.jumpAgainSail || player.jumpAgainUnicorn;
        }

        public static bool IsMinion(this Projectile projectile)
        {
            return projectile.minion && !ProjectileID.Sets.MinionShot[projectile.type] && !ProjectileID.Sets.SentryShot[projectile.type] && !projectile.sentry && ProjectileID.Sets.MinionSacrificable[projectile.type] && projectile.minionSlots > 0f && Main.projPet[projectile.type];
        }

        public static Point Get2x2_16x16FrameTopLeft(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            if (tile.frameX % 36 != 0)
                i--;
            if (tile.frameY != 0)
                j--;
            return new Point(i, j);
        }

        public static Color GetItemRarityColor(int itemID)
        {
            var item = new Item();
            item.SetDefaults(itemID);
            return GetItemRarityColor(item);
        }

        public static Color GetItemRarityColor(Item item)
        {
            return InvokeTooltipLineAndGetColor("ItemName", item.type).GetValueOrDefault(GetRarityColor(item.rare));
        }

        public static Color? InvokeTooltipLineAndGetColor(string lineName, int itemID)
        {
            int useless1 = -1;
            int tooltipAmount = 1;
            bool[] useless2 = new bool[] { false, };
            bool[] useless6 = new bool[] { false, };
            string[] useless3 = new string[] { lineName, };
            string[] useless5 = new string[] { lineName, };
            var item = new Item();
            item.SetDefaults(itemID);
            var lines = ItemLoader.ModifyTooltips(item, ref tooltipAmount, useless5, ref useless3, ref useless6, ref useless2, ref useless1, out Color?[] overrideColor);
            for (int i = 0; i < tooltipAmount; i++)
            {
                var t = lines[i];
                if (t.mod == "Terraria" && t.Name == "ItemName")
                    return t.overrideColor != null ? (Color?)(Color)overrideColor[i] : null;
            }
            return null;
        }

        public static Color GetRarityColor(int rarity)
        {
            switch (rarity)
            {
                default:
                    return new Color(255, 255, 255, 255);

                case ItemRarityID.Blue:
                    return Colors.RarityBlue;

                case ItemRarityID.Green:
                    return Colors.RarityGreen;

                case ItemRarityID.Orange:
                    return Colors.RarityOrange;

                case ItemRarityID.LightRed:
                    return Colors.RarityRed;

                case ItemRarityID.Pink:
                    return Colors.RarityPink;

                case ItemRarityID.LightPurple:
                    return Colors.RarityPurple;

                case ItemRarityID.Lime:
                    return Colors.RarityLime;

                case ItemRarityID.Yellow:
                    return Colors.RarityYellow;

                case ItemRarityID.Cyan:
                    return Colors.RarityCyan;

                // TODO: look into the vanilla source for the Red and Purple rarities

                case ItemRarityID.Gray:
                    return Colors.RarityTrash;

                case ItemRarityID.Expert:
                    return Main.DiscoColor;
            }
        }

        public static DrawData DrawRectangle_Data(Rectangle rectangle, Color color, Vector2 adjustment)
        {
            return new DrawData(Tex.Pixel, new Vector2(rectangle.X, rectangle.Y) + adjustment, null, color, 0f, new Vector2(0f, 0f), new Vector2(rectangle.Width, rectangle.Height), SpriteEffects.None, 0);
        }

        public static void DrawRectangle(Rectangle rectangle, Color color, Vector2 adjustment)
        {
            Main.spriteBatch.Draw(Tex.Pixel, new Vector2(rectangle.X, rectangle.Y) + adjustment, null, color, 0f, new Vector2(0f, 0f), new Vector2(rectangle.Width, rectangle.Height), SpriteEffects.None, 0f);
        }

        public static void UpdateFilter(bool active, string name, Vector2 position = default(Vector2), params object[] args)
        {
            if (active != Filters.Scene[name].IsActive())
            {
                if (active)
                {
                    Filters.Scene[name].Activate(position, args);
                }
                else
                {
                    Filters.Scene[name].Deactivate(args);
                }
            }
        }

        public static void UpdateSky(bool active, string name)
        {
            if (active != SkyManager.Instance[name].IsActive())
            {
                if (active)
                {
                    SkyManager.Instance.Activate(name, default(Vector2));
                }
                else
                {
                    SkyManager.Instance.Deactivate(name);
                }
            }
        }

        public static void UpdateOverlay(bool active, string name)
        {
            if (Overlays.Scene[name] != null && active != (Overlays.Scene[name].Mode != OverlayMode.Inactive))
            {
                if (active)
                {
                    Overlays.Scene.Activate(name);
                }
                else
                {
                    Overlays.Scene[name].Deactivate();
                }
            }
        }

        public static void PrepareForTeleport(this Player player)
        {
            player.grappling[0] = -1;
            player.grapCount = 0;
            for (int p = 0; p < 1000; p++)
            {
                if (Main.projectile[p].active && Main.projectile[p].owner == player.whoAmI && Main.projectile[p].aiStyle == 7)
                    Main.projectile[p].Kill();
            }
        }

        public static int NextVRand(this UnifiedRandom rand, int min, int max)
        {
            return min + rand.Next(max - min + 1);
        }

        public static float GetParabola(float min, float max, float x)
        {
            float xGradient = (x - min) / (float)(max - min);
            return 1f - (float)Math.Pow(1f - xGradient, 2);
        }

        public static void RectangleMethod(this Rectangle rect, Utils.PerLinePoint method)
        {
            for (int i = rect.X; i < rect.X + rect.Width; i++)
            {
                for (int j = rect.Y; j < rect.Y + rect.Height; j++)
                {
                    method(i, j);
                }
            }
        }

        public static void PointAtoPointB(int x, int y, int x2, int y2, Utils.PerLinePoint method)
        {
            int xDir = x > x2 ? -1 : 1;
            int yDir = y > y2 ? -1 : 1;
            int xDifference = (x - x2).Abs();
            int yDifference = (y - y2).Abs();
            for (int i = 0; i < xDifference + 1; i++)
            {
                method(x + xDir * i, y);
            }
            for (int i = 0; i < yDifference + 1; i++)
            {
                method(x + xDir * xDifference, y + yDir * i);
            }
        }

        public static Color UseR(this Color color, int R) => new Color(R, color.G, color.B, color.A);
        public static Color UseR(this Color color, float R) => new Color((int)(R * 255), color.G, color.B, color.A);

        public static Color UseG(this Color color, int G) => new Color(color.R, G, color.B, color.A);
        public static Color UseG(this Color color, float G) => new Color(color.R, (int)(G * 255), color.B, color.A);

        public static Color UseB(this Color color, int B) => new Color(color.R, color.G, B, color.A);
        public static Color UseB(this Color color, float B) => new Color(color.R, color.G, (int)(B * 255), color.A);

        public static Color UseA(this Color color, int alpha) => new Color(color.R, color.G, color.B, alpha);
        public static Color UseA(this Color color, float alpha) => new Color(color.R, color.G, color.B, (int)(alpha * 255));

        public static string GetKeybindNames(this ModHotKey key, int keyValue = 0)
        {
            List<string> keys = key.GetAssignedKeys();
            if (keys == null || keys.Count == 0)
            {
                return Language.GetTextValue(AQText.Key + "Common.UnassignedKey" + keyValue);
            }
            else
            {
                if (keys.Count == 1)
                    return keys[0];
                string textValue = "";
                int index = 0;
                while (true)
                {
                    textValue += keys[index];
                    if (index == keys.Count - 1)
                        return textValue;
                    textValue += ", ";
                    index++;
                }
            }
        }

        public static void SetItemHoldout(this Player player, float rotation, int direction)
        {
            player.itemRotation = rotation;
            if (player.direction != direction)
                player.ChangeDir(direction);
            if (direction == 1)
                player.itemRotation -= MathHelper.Pi;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colors">The colors</param>
        /// <param name="amount">Automatically wraps based on the array's length</param>
        /// <returns></returns>
        public static Color LerpBetween(Color[] colors, float amount)
        {
            int index = (int)(amount % colors.Length);
            return Color.Lerp(colors[index], colors[(index + 1) % (colors.Length - 1)], amount % 1f);
        }

        public static Vector2[] GetCircle(Vector2 center, float radius, int amount = 20)
        {
            var points = new Vector2[amount];
            float rot = MathHelper.TwoPi / amount;
            float j = 0f;
            for (int i = 0; i < amount; i++)
            {
                points[i] = center + radius * new Vector2((float)Math.Cos(j), (float)Math.Sin(j));
                j += rot;
            }
            return points;
        }

        public static bool GetBit(this byte b, byte bit)
        {
            return (b & 1 << bit) != 0;
        }

        public static byte SetBit(this byte b, byte bit, bool value)
        {
            return value ? (b |= (byte)(1 << bit)) : (b &= (byte)~(1 << bit));
        }

        public static float GetHue(this Color color)
        {
            float min = Math.Min(Math.Min(color.R, color.G), color.B);
            float max = Math.Max(Math.Max(color.R, color.G), color.B);
            if (min == max)
                return 0;
            float hue = 0f;
            if (max == color.R)
            {
                hue = (color.G - color.B) / (max - min);
            }
            else if (max == color.G)
            {
                hue = 2f + (color.B - color.R) / (max - min);
            }
            else
            {
                hue = 4f + (color.R - color.G) / (max - min);
            }
            hue *= 60;
            if (hue < 0)
                hue += 360;
            return hue;
        }

        public static int Abs(this int value)
        {
            return value >= 0 ? value : value * -1;
        }

        public static float Abs(this float value)
        {
            return value >= 0 ? value : value * -1f;
        }

        public static Vector2 RandomPosition(this Projectile projectile, int sizeDecrease = 0, UnifiedRandom rand = null)
        {
            return RandomPosition(new Rectangle((int)projectile.position.X + sizeDecrease, (int)projectile.position.Y + sizeDecrease, projectile.width - sizeDecrease * 2, projectile.height - sizeDecrease * 2), rand);
        }

        public static Vector2 RandomPosition(this Rectangle rectangle, UnifiedRandom rand = null)
        {
            rand = rand ?? Main.rand;
            return new Vector2(rectangle.X + rand.NextFloat(rectangle.Width), rectangle.Y + rand.NextFloat(rectangle.Height));
        }

        public static int RoundUp(this float value)
        {
            int down = (int)value;
            if (value - down > 0f)
            {
                return down + 1;
            }
            return down;
        }

        public static void WriteItem(this BinaryWriter writer, Item item)
        {
            writer.Write(item.type);
            writer.Write(item.stack);
            writer.Write(item.prefix);

            if (item.type >= Main.maxItemTypes)
            {
                item.modItem.NetSend(writer);
            }
        }

        public static Item ReadItem(this BinaryReader reader)
        {
            var item = new Item();
            item.SetDefaults(reader.ReadInt32());
            item.stack = reader.ReadInt32();
            item.Prefix(reader.ReadByte());

            if (item.type >= Main.maxItemTypes)
            {
                item.modItem.NetRecieve(reader);
            }
            return item;
        }
    }
}