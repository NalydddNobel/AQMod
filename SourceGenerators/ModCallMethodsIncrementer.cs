using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceGenerators;

[Generator]
internal class ModCallMethodsIncrementer : IIncrementalGenerator {
    void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context) {
#if DEBUG
        //if (!System.Diagnostics.Debugger.IsAttached) {
        //    System.Diagnostics.Debugger.Launch();
        //}
#endif

        var modCallMethods = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => IsSyntaxTargetForGeneration(s),
                static (ctx, _) => GetGenTarget(ctx))
            .Where(static m => m is not null);

        context.RegisterSourceOutput(modCallMethods,
          static (spc, source) => Execute(source, spc));
    }

    static bool IsSyntaxTargetForGeneration(SyntaxNode node) {
        return node is MethodDeclarationSyntax;
    }

    static ModCallInfo? GetGenTarget(GeneratorSyntaxContext context) {
        var method = (MethodDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(method) is not IMethodSymbol methodSymbol) {
            return null;
        }

        foreach (AttributeData attr in methodSymbol.GetAttributes()) {
            string fullName = attr.AttributeClass.ToString();

            if (fullName == ModCallAttribute.FullyQualifiedName) {
                return CollectGenInfo(methodSymbol, attr);
            }
        }


        return null;
    }

    static ModCallInfo? CollectGenInfo(IMethodSymbol method, AttributeData attr) {
        string callName = $"ERROR_{method.Name}";
        ModCallAttribute.GetAttributeParams(attr, ref callName);

        string call = $"global::{method.ContainingType}.{method.Name}";

        string returnType = method.ReturnType.ToString() ?? "void";
        if (returnType != "void") {
            call = $"return {call}";
        }

        return new ModCallInfo(callName, returnType, call, IncrementalUtils.GetMethodParameters(method));
    }

    static void Execute(ModCallInfo? info, SourceProductionContext context) {
        if (!(info is { } value)) {
            return;
        }

        /*
        if (value.Name == null) {
            context.ReportDiagnostic(Diagnostic.Create(, Location.None));
        }
        */

        string code = string.Empty;
        string returnDefault = "return";

        if (value.FullyQualifiedReturnType != "void") {
            returnDefault += $" default";
        }

        int index = 1;
        foreach (ParameterInfo param in value.Parameters) {
            string typeWithoutNullableShorthand = param.FullyQualifiedType.TrimEnd('?');
            if (param.HasDefaultValue) {
                code +=
                $$"""
                        {{param.FullyQualifiedType}} {{param.Name}} = default;
                        if (args.Length > {{index}}) {
                            if (args[{{index}}] is {{typeWithoutNullableShorthand}} optionalValue) {
                                {{param.Name}} = optionalValue;
                            }
                            else {
                                LogError($"Optional Mod Call Parameter index {{index}} (\"{{param.Name}}\") did not match Type \"{{typeWithoutNullableShorthand}}\".");
                            }
                        }


                """;
            }
            else {
                code +=
                $$"""
                        if (args[{{index}}] is not {{param.FullDisplay}}) {
                            LogError($"Mod Call Parameter index {{index}} (\"{{param.Name}}\") did not match Type \"{{param.FullyQualifiedType}}\".");
                            {{returnDefault}};
                        }


                """;
            }

            index++;
        }

        code += $"        {value.MethodCode}({string.Join(", ", value.Parameters.Select(p => p.Name).ToArray())});";

        string result =
        $$"""
            namespace {{Namespace}};

            public partial class ModCallHandler {
                [System.Runtime.CompilerServices.CompilerGenerated]
                static {{value.FullyQualifiedReturnType}} {{value.Name}}(object[] args) {
            {{code}}
                }
            }
            """;

        context.AddSource($"ModCalls.{value.Name}.g.cs", SourceText.From(result, Encoding.UTF8));
    }

    record struct ModCallInfo(string Name, string FullyQualifiedReturnType, string MethodCode, IEnumerable<ParameterInfo> Parameters);
}

//[Generator]
//internal class ModCallsIncrementer : IIncrementalGenerator {
//    public const string Namespace = "Aequus";
//    public const string ModCallAttributeFullName = $"{Namespace}.{nameof(ModCallAttribute)}";

//    void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context) {
//#if DEBUG
//        //if (!System.Diagnostics.Debugger.IsAttached) {
//        //    System.Diagnostics.Debugger.Launch();
//        //}
//#endif

//        // Create marker attribute.
//        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
//            $"{nameof(ModCallAttribute)}.g.cs",
//            SourceText.From(ModCallAttribute.Quine, Encoding.UTF8)));

//        var modCallMethods = context.SyntaxProvider
//            .CreateSyntaxProvider(
//                static (s, _) => IsSyntaxTargetForGeneration(s),
//                static (ctx, _) => GetGenTarget(ctx))
//            .Where(static m => m is not null);

//        context.RegisterSourceOutput(modCallMethods,
//          static (spc, source) => Execute(source, spc));
//    }

//    static bool IsSyntaxTargetForGeneration(SyntaxNode node) {
//        return node is MethodDeclarationSyntax;
//    }

//    static ModCallInfo? GetGenTarget(GeneratorSyntaxContext context) {
//        var method = (MethodDeclarationSyntax)context.Node;

//        if (context.SemanticModel.GetDeclaredSymbol(method) is not IMethodSymbol methodSymbol) {
//            return null;
//        }

//        foreach (AttributeData attr in methodSymbol.GetAttributes()) {
//            string fullName = attr.AttributeClass.ToString();

//            if (fullName == ModCallAttributeFullName) {
//                return CollectGenInfo(methodSymbol, attr);
//            }
//        }


//        return null;
//    }

//    static ModCallInfo? CollectGenInfo(IMethodSymbol method, AttributeData attr) {
//        string callName = string.Empty;
//        if (!attr.ConstructorArguments.IsEmpty) {
//            ImmutableArray<TypedConstant> args = attr.ConstructorArguments;

//            foreach (TypedConstant arg in args) {
//                if (arg.Kind == TypedConstantKind.Error) {

//                    return null;
//                }
//            }

//            switch (args.Length) {
//                case 1:
//                    callName = (string)args[0].Value;
//                    break;
//            }
//        }

//        if (!attr.NamedArguments.IsEmpty) {
//            foreach (KeyValuePair<string, TypedConstant> arg in attr.NamedArguments) {
//                TypedConstant typedConstant = arg.Value;
//                if (typedConstant.Kind == TypedConstantKind.Error) {
//                    return null;
//                }

//                switch (arg.Key) {
//                    case "Name":
//                        callName = (string)typedConstant.Value;
//                        break;
//                }
//            }
//        }

//        string call = $"global::{method.ContainingType}.{method.Name}";

//        string returnType = method.ReturnType.ToString() ?? "void";
//        if (returnType != "void") {
//            call = $"return {call}";
//        }

//        return new ModCallInfo(callName, returnType, call, IncrementalUtils.GetMethodParameters(method));
//    }

//    static void Execute(ModCallInfo? info, SourceProductionContext context) {
//        if (!(info is { } value)) {
//            return;
//        }

//        /*
//        if (value.Name == null) {
//            context.ReportDiagnostic(Diagnostic.Create(, Location.None));
//        }
//        */

//        string result =
//        $$"""
//            namespace {{Namespace}};

//            public partial class ModCallHandler {
//                static {{value.FullyQualifiedReturnType}} {{value.Name}}({{string.Join(", ", value.Parameters.Select(p => p.FullDisplay).ToArray())}}) {
//                    {{value.MethodCode}}({{string.Join(", ", value.Parameters.Select(p => p.Name).ToArray())}});
//                }
//            }
//            """;

//        context.AddSource($"ModCalls.{value.Name}.g.cs", SourceText.From(result, Encoding.UTF8));
//    }

//    record struct ModCallInfo(string Name, string FullyQualifiedReturnType, string MethodCode, IEnumerable<ParameterInfo> Parameters);
//}
