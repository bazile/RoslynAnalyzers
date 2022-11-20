using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Bazile.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SingleDimensionZeroSizeArrayAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SingleDimensionZeroSizeArray";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            //// TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            //// See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            //context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);

            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ArrayCreationExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var x = (ArrayCreationExpressionSyntax)context.Node;
            if (x.Type.RankSpecifiers.Count > 1) return;
            if (x.Type.RankSpecifiers[0].Sizes[0] is not LiteralExpressionSyntax y) return;

            //(NumericLiteral)y.Token
            if (y.Token.Kind() == SyntaxKind.NumericLiteralToken) return;

            int size = (int)y.Token.Value;
            if (size != 0) return;

            //var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);
            //context.ReportDiagnostic(diagnostic);
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
    }
}
