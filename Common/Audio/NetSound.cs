using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.Audio
{
    public abstract class NetSound : ModType
    {
        public byte Type { get; internal set; }
        public SoundStyle Style { get; private set; }

        protected sealed override void Register()
        {
            NetSoundLoader.Register(this);
            InitSound();
        }

        public sealed override void SetupContent()
        {
            SetStaticDefaults();
        }

        protected abstract SoundStyle InitDefaultSoundStyle();

        protected void GetNetDefaults(BinaryReader reader, byte plr, out Vector2? location, out float? volumeOverride, out float? pitchOverride, out float? pitchVarianceOverride)
        {
            var bb = (BitsByte)reader.ReadByte();
            location = null;
            volumeOverride = null;
            pitchOverride = null;
            pitchVarianceOverride = null;
            if (bb[0])
            {
                location = new(reader.ReadSingle(), reader.ReadSingle());
            }
            if (bb[1])
            {
                volumeOverride = reader.ReadSingle();
            }
            if (bb[2])
            {
                pitchOverride = reader.ReadSingle();
            }
            if (bb[3])
            {
                pitchVarianceOverride = reader.ReadSingle();
            }
        }

        public virtual void NetPlay(BinaryReader reader, byte plr)
        {
            GetNetDefaults(reader, plr, out var loc, out var volumeOverride, out var pitchOverride, out var pitchVarianceOverride);
            if (Main.netMode == NetmodeID.Server)
            {
                SendSound(loc, plr, volumeOverride, pitchOverride, pitchVarianceOverride);
                return;
            }

            PlaySoundWithOverrides(loc, volumeOverride, pitchOverride, pitchVarianceOverride);
        }

        protected void SendSound(Vector2? where, byte plr, float? volumeOverride = null, float? pitchOverride = null, float? pitchVarianceOverride = null)
        {
            var packet = Aequus.GetPacket(PacketType.SyncSound);
            BitsByte bb = new(where != null, volumeOverride != null, pitchOverride != null, pitchVarianceOverride != null);
            packet.Write(Type);
            packet.Write(plr);

            if (bb[0])
            {
                packet.Write(where.Value.X);
                packet.Write(where.Value.Y);
            }
            if (bb[1])
            {
                packet.Write(volumeOverride.Value);
            }
            if (bb[2])
            {
                packet.Write(pitchOverride.Value);
            }
            if (bb[3])
            {
                packet.Write(pitchVarianceOverride.Value);
            }
            packet.Write(bb);

            packet.Send();
        }

        protected void PlaySoundWithOverrides(Vector2? where, float? volumeOverride = null, float? pitchOverride = null, float? pitchVarianceOverride = null)
        {
            var sound = Style;
            sound.Volume = volumeOverride ?? sound.Volume;
            sound.Pitch = pitchOverride ?? sound.Pitch;
            sound.PitchVariance = pitchVarianceOverride ?? sound.PitchVariance;
            SoundEngine.PlaySound(sound, where);
        }

        public virtual void Play(Vector2? where, float? volumeOverride = null, float? pitchOverride = null, float? pitchVarianceOverride = null)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                PlaySoundWithOverrides(where, volumeOverride, pitchOverride, pitchVarianceOverride);
                return;
            }

            SendSound(where, (byte)Main.myPlayer, volumeOverride, pitchOverride, pitchVarianceOverride);
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            PlaySoundWithOverrides(where, volumeOverride, pitchOverride, pitchVarianceOverride);
        }

        internal void InitSound()
        {
            Style = InitDefaultSoundStyle();
        }
    }
}