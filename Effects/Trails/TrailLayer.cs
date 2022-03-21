using System.Collections.Generic;

namespace AQMod.Effects.Trails
{
    public sealed class TrailLayer<TTrail> where TTrail : Trail // possibly create a class called "EffectLayerType" which does all of this automatically?
    {
        internal List<TTrail> _trails;

        public TrailLayer()
        {
            Initialize();
        }

        public void NewTrail(TTrail trail)
        {
            trail.OnAdd();
            _trails.Add(trail);
        }

        public void Initialize()
        {
            _trails = new List<TTrail>();
        }

        public void UpdateTrails()
        {
            AQMod.Trails.UpdateTrails(_trails);
        }

        public void Render()
        {
            AQMod.Trails.RenderTrails(_trails);
        }
    }
}