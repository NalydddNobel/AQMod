namespace Aequus.Core.DataSets;

public interface IDataEntry {
    System.String Name { get; set; }

    System.Boolean ValidEntry { get; }
    System.Boolean VanillaEntry { get; }

    void Initialize();
}

public interface IDataEntry<T> : IDataEntry {
    T Id { get; set; }
}