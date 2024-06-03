namespace Aequus.Core.Structures;

public interface IProvider<TValue> {
    TValue GetColor();
}

public interface IProvider<TValue, TContext> {
    TValue GetColor(TContext context);
}
