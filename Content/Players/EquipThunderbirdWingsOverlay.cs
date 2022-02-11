using AQMod.Items.Accessories.Wings;
using AQMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AQMod.Content.Players
{
    public class EquipThunderbirdWingsOverlay : EquipWingsOverlay
    {
        public EquipThunderbirdWingsOverlay() : base(AQUtils.GetPath<Thunderbird>("_Wings"))
        {
        }

        public override void Draw(PlayerDrawInfo info)
        {
            GetBasicPlayerDrawInfo(info, out Vector2 wingPosition, out var frame, out var effects, out float opacity, 4);
            var clr = Narrizuul.TextColor() * opacity * 0.2f;
            var texture = Asset.Value;
            var wave = AQUtils.Wave(Main.GlobalTime * 10f, 0f, 1f);
            wingPosition.X -= info.drawPlayer.direction * 9.5f;
            wingPosition.Y += 19f;
            var n = new Vector2(wave, 0f).RotatedBy(Main.GlobalTime * 2.9f);
            clr.A = 0;
            for (int i = 1; i <= 4; i++)
            {
                Main.playerDrawData.Add(new DrawData(texture, wingPosition + n * i, frame, clr, info.drawPlayer.bodyRotation, frame.Size() / 2f, 1f, effects, 0) { shader = info.wingShader });
                Main.playerDrawData.Add(new DrawData(texture, wingPosition - n * i, frame, clr, info.drawPlayer.bodyRotation, frame.Size() / 2f, 1f, effects, 0) { shader = info.wingShader });
            }
        }
    }
}