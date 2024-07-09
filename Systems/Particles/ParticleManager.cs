using AequusRemake.Core.Debug;
using System.Collections.Generic;

namespace AequusRemake.Core.Structures.Particles;

/// <summary>
/// Handles and updates particle systems.
/// </summary>
[Autoload(Side = ModSide.Client)]
public class ParticleManager : ModSystem {
    private static readonly List<IParticleSystem> _registeredSystems = new();
    internal readonly LinkedList<IParticleSystem> _activeSystems = new();

    public ParticleManager Instance { get; private set; }

    internal static void Register(IParticleSystem system) {
        _registeredSystems.Add(system);
    }

    public override void Load() {
        Instance = this;
    }

    public override void PreUpdateEntities() {
        DiagnosticsMenu.TrackNumber(DiagnosticsMenu.TrackerType.ParticleScenes, _activeSystems.Count);
        DiagnosticsMenu.StartStopwatch();

        if (_activeSystems.Count == 0) {
            DiagnosticsMenu.EndStopwatch(DiagnosticsMenu.TimerType.Particles);
            return;
        }

        LinkedListNode<IParticleSystem> systemNode = _activeSystems.First;
        do {
            IParticleSystem system = systemNode.Value;
            system.Update();
            if (!system.Active) {
                system.Deactivate();
                _activeSystems.Remove(systemNode);
            }
        }
        while ((systemNode = systemNode.Next) != null);

        DiagnosticsMenu.EndStopwatch(DiagnosticsMenu.TimerType.Particles);
    }
}