using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace SourceGenerators;

internal static class IncrementalUtils {
    public static IEnumerable<ParameterInfo> GetMethodParameters(this IMethodSymbol method) {
        foreach (IParameterSymbol param in method.Parameters) {
            string fullDisplayString = param.ToDisplayString();
            string name = param.Name;
            string type = param.Type.ToDisplayString();
            string defaultValue = "";
            if (param.HasExplicitDefaultValue) {
                defaultValue = param.ExplicitDefaultValue?.ToString() ?? "default";
            }
            yield return new ParameterInfo(fullDisplayString, name, type, defaultValue);
        }
    }
}
