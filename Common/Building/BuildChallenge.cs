using Aequus.Common.Building.Passes;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Common.Building {
    public abstract class BuildChallenge : ModType {
        public int Type { get; private set; }
        private readonly List<TileScanPass> Passes;

        public BuildChallenge() {
            Passes = new();
        }

        protected sealed override void Register() {
            Type = LoaderManager.Get<BuildChallengeLoader>().Register(this);
            LoadPasses();
        }

        protected T AddPass<T>(T pass) where T : TileScanPass {
            pass.index = Passes.Count;
            Passes.Add(pass);
            return pass;
        }
        public abstract void LoadPasses();

        public sealed override void SetupContent() {
            SetStaticDefaults();
        }

        public void InvokePass(ScanResults[] results, ref ScanInfo info, TileScanPass pass) {
            results[pass.index] = pass.Scan(ref info);
        }
        public ScanResults[] Scan(ref ScanInfo info) {
            var result = new ScanResults[Passes.Count];
            Scan(result, ref info);
            return result;
        }
        public abstract void Scan(ScanResults[] results, ref ScanInfo info);
    }
}