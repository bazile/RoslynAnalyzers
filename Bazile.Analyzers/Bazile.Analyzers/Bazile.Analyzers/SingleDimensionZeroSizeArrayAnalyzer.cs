﻿using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Bazile.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SingleDimensionZeroSizeArrayAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "SingleDimensionZeroSizeArray";
    //public const string DiagnosticId = "BazileAnalyzers";

    // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
    // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.SingleDimensionZeroSizeArray_AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.SingleDimensionZeroSizeArray_AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.SingleDimensionZeroSizeArray_AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
    private const string Category = "Performance";

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information

        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ArrayCreationExpression);
    }

    private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        bool shouldReport = ShouldReport(context);
        if (shouldReport)
        {
            //var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);
            var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }

    //private static void AnalyzeSymbol(SymbolAnalysisContext context)
    //{
    //    // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
    //    var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

    //    // Find just those named type symbols with names containing lowercase letters.
    //    if (namedTypeSymbol.Name.ToCharArray().Any(char.IsLower))
    //    {
    //        // For all such symbols, produce a diagnostic.
    //        var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

    //        context.ReportDiagnostic(diagnostic);
    //    }
    //}

    private static bool ShouldReport(SyntaxNodeAnalysisContext context)
    {
        bool result = false;

        var arraySyntax = (ArrayCreationExpressionSyntax)context.Node;
        var rankSpecifiers = arraySyntax.Type.RankSpecifiers;
        var sizes = rankSpecifiers[0].Sizes;
        if (
            rankSpecifiers.Count == 1
            && sizes.Count == 1
            && sizes[0] is LiteralExpressionSyntax rankLiteralSyntax
            && rankLiteralSyntax.Token.Kind() == SyntaxKind.NumericLiteralToken)
        {
            int size = (int)rankLiteralSyntax.Token.Value;
            result = size == 0;
        }

        return result;
    }
}
