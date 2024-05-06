using System.Collections.Generic;

namespace Aequus.Common.Particles.New;

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
        if (_activeSystems.Count == 0) {
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
    }
}