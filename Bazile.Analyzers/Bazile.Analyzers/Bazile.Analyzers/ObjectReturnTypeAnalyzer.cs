using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Bazile.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ObjectReturnTypeAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "ObjectReturnType";

    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.ObjectReturnType_AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.ObjectReturnType_AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.ObjectReturnType_AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
    private const string Category = "Design";

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information

        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration);
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
        var methodSyntax = (MethodDeclarationSyntax)context.Node;
        switch (methodSyntax.ReturnType)
        {
            // object keyword
            case PredefinedTypeSyntax predefinedTypeSyntax 
                when predefinedTypeSyntax.Keyword.Kind() == SyntaxKind.ObjectKeyword:
            
            // Object
            case IdentifierNameSyntax { Identifier.Text: "Object" }:
            
            // System.Object
            case QualifiedNameSyntax { 
                Left: IdentifierNameSyntax { Identifier.Text: "System" },
                Right: IdentifierNameSyntax { Identifier.Text: "Object" }
            }:
                return true;
        }

        return false;
    }
}
