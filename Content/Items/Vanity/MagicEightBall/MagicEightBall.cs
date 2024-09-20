using Aequus.Common.Entities.Players;
using Aequus.Common.Items;
using Aequus.Common.Net;
using Aequus.Common.Utilities.Helpers;
using Aequus.Systems;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Items.Vanity.MagicEightBall;

[LegacyName("CelesitalEightBall")]
public class MagicEightBall : ModItem, ICooldownItem, ICustomHeldItemGraphics {
    const byte Invalid = byte.MaxValue;

    const string TagKey = "Text2";
    const byte MaxText = 20;
    internal static readonly LocalizedText[] Text = new LocalizedText[MaxText];

    byte _textId = Invalid;

    int ICooldownItem.CooldownTime => 60;
    bool ICooldownItem.ShowCooldownTip => false;
    bool ICooldownItem.HasCooldownShakeEffect => false;

    static Color TextColor => Color.Lerp(Main.mouseColor, Color.White, 0.5f) with { A = byte.MaxValue };

    static SoundStyle UseSound;

    public override void SetStaticDefaults() {
        UseSound = AequusSounds.NewMultisound(AequusSounds.UseEightBall0, 4);
        for (int i = 0; i < Text.Length; i++) {
            Text[i] = this.GetLocalization($"Result.{i}");
        }
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.rare = ItemRarityID.Green;
        Item.useAnimation = 20;
        Item.useTime = 20;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.value = Item.sellPrice(silver: 20);
        Item.holdStyle = ItemHoldStyleID.HoldFront;
        Item.UseSound = UseSound;
        Item.noUseGraphic = true;
        _textId = Invalid;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.Lerp(lightColor, Color.White, 0.5f);
    }

    public override bool CanUseItem(Player player) {
        return !this.HasCooldown(player);
    }

    public override bool? UseItem(Player player) {
        if (player.itemAnimation > 3) {
            return false;
        }

        if (Main.myPlayer == player.whoAmI) {
            var old = _textId;
            while (old == _textId) {
                _textId = (byte)Main.rand.Next(Text.Length);
            }

            Instance<MagicEightBallSpawnRequest>().Send(player.whoAmI, _textId, TextColor);
        }
        this.SetCooldown(player);
        return true;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        if (!Text.IndexInRange(_textId)) {
            return;
        }

        string text = Text[_textId].Value;
        Color textColor = TextColor;
        tooltips.AddTooltip(new TooltipLine(Mod, "EightBall", text) { OverrideColor = textColor });
    }

    public override void NetSend(BinaryWriter writer) {
        writer.Write(_textId);
    }

    public override void NetReceive(BinaryReader reader) {
        _textId = reader.ReadByte();
    }

    public override void SaveData(TagCompound tag) {
        if (_textId != Invalid) {
            tag[TagKey] = _textId;
        }
    }

    public override void LoadData(TagCompound tag) {
        _textId = tag.GetOrDefault(TagKey, Invalid);
        ParseLegacySaveData(tag);
    }

    void ParseLegacySaveData(TagCompound tag) {
        // Legacy string id.
        if (tag.TryGet("Text", out string legacyText) && !string.IsNullOrWhiteSpace(legacyText)) {
            int index = legacyText.LastIndexOf('.');
            string numberText = "";
            for (index++; index < legacyText.Length; index++) {
                if (!char.IsNumber(legacyText[index])) {
                    break;
                }

                numberText += legacyText[index];
            }

            if (byte.TryParse(numberText, NumberStyles.None, TextHelper.InvariantCulture, out byte parseResult)) {
                _textId = parseResult;
            }
        }

        // Legacy sbyte id.
        if (tag.TryGet("id", out byte legacyId)) {
            sbyte legacyIdValue = new BitsConvert.Bit8(legacyId).ToSByte;
            if (legacyIdValue < 0) {
                _textId = Invalid;
            }
            else {
                _textId = (byte)legacyIdValue;
            }
        }
    }

    public override void UseItemFrame(Player player) {
        player.bodyFrame.Y = player.bodyFrame.Height * 3;
    }

    void ICustomHeldItemGraphics.Draw(ref PlayerDrawSet info) {
        Item item = info.heldItem;
        Texture2D texture = AequusTextures.MagicEightBall_Held;
        Player player = info.drawPlayer;
        Rectangle itemFrame = texture.Bounds;

        Vector2 drawLocation = info.ItemLocation;
        Vector2 drawOffset = Vector2.Zero;
        if (player.itemAnimation > 0) {
            // Animation timer
            float x = (player.itemAnimationMax - player.itemAnimation) * 0.9f;

            drawOffset.X += 18f - MathF.Cos(x % MathHelper.PiOver2) * 2f;
            drawOffset.Y += MathF.Sin(x) * 6f + 4f;
        }

        drawLocation += player.GetDrawOffset(drawOffset);

        Color color = item.GetAlpha(Lighting.GetColor(drawLocation.ToTileCoordinates())) * (1f - info.shadow);
        Vector2 origin = itemFrame.Size() / 2f;
        float rotation = player.bodyRotation;
        SpriteEffects effects = player.GetSpriteEffect();

        info.DrawDataCache.Add(new DrawData(texture, (drawLocation - Main.screenPosition).Floor(), itemFrame, color, rotation, origin, 1f, effects, 0f));
    }
}

public class MagicEightBallSpawnRequest : PacketHandler {
    public override PacketType LegacyPacketType => PacketType.EightBal;

    public void Send(int player, byte id, Color textColor) {
        var packet = GetPacket();

        packet.Write(player);
        packet.Write(id);
        packet.WriteRGB(textColor);

        SendPacket(packet);
    }

    public override void Receive(BinaryReader reader, int sender) {
        int player = reader.ReadInt32();
        byte id = reader.ReadByte();
        Color clr = reader.ReadRGB();


        // Server will broadcast to all players.
        if (Main.netMode == NetmodeID.Server) {
            Send(player, id, clr);
        }
        // Clients will create the popup.
        else if (Main.player.IndexInRange(player) && Main.player[player].active) {
            CreatePopup(Main.player[player], id, clr);
        }
    }

    void CreatePopup(Player player, byte id, Color color) {
        PushUpCombatText(player.Top);
        string textValue = MagicEightBall.Text[id].Value;
        var spawnRectangle = new Rectangle((int)player.position.X, (int)player.position.Y + 10, player.width, player.height / 3);
        int t = CombatText.NewText(spawnRectangle, color, textValue, true);
        Main.combatText[t].position.X = player.Center.X - FontAssets.CombatText[1].Value.MeasureString(textValue).X / 2f;
        Main.combatText[t].rotation *= 0.33f;
    }

    static void PushUpCombatText(Vector2 position) {
        var r = new Rectangle((int)position.X - 300, (int)position.Y - 160, 500, 180);
        float highestY = 0f;
        float lowestY = float.MaxValue;
        int combatTextId = -1;

    Reiterate:
        for (int i = 0; i < Main.maxCombatText; i++) {
            if (!Main.combatText[i].active || Main.combatText[i].position.Y <= highestY || Main.combatText[i].position.Y >= lowestY || !r.Contains(Main.combatText[i].position.ToPoint())) {
                continue;
            }

            combatTextId = i;
            highestY = Main.combatText[i].position.Y;
        }

        if (combatTextId != -1) {
            var combatText = Main.combatText[combatTextId];
            combatText.velocity.Y = Math.Min(combatText.velocity.Y, -8f);

            combatTextId = -1;
            position.Y = combatText.position.Y;
            highestY = 0f;
            lowestY = combatText.position.Y - 1f;
            goto Reiterate;
        }
    }
}