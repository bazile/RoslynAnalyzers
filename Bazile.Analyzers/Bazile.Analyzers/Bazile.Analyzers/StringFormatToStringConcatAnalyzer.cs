using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;

namespace Bazile.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class StringFormatToStringConcatAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "StringFormatToStringConcat";

    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.StringFormatToStringConcat_AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.StringFormatToStringConcat_AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.StringFormatToStringConcat_AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
    private const string Category = "Design";

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information

        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        bool shouldReport = ShouldReport(context);
        if (shouldReport)
        {
            var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool ShouldReport(SyntaxNodeAnalysisContext context)
    {
        var invocationSyntax = (InvocationExpressionSyntax)context.Node;

        return false;
    }
}
