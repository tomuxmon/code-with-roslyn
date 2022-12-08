using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace min_analyzer {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class SomeAnalyzer : DiagnosticAnalyzer {

        private static readonly DiagnosticDescriptor someDescriptor = new(
            "S01", "Method body token count",
            "Method body contains '{0}' tokens. Reduce it to 40!",
            "Security", DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(someDescriptor);

        public override void Initialize(AnalysisContext ctx) {
            ctx.EnableConcurrentExecution();
            ctx.ConfigureGeneratedCodeAnalysis(
                GeneratedCodeAnalysisFlags.Analyze |
                GeneratedCodeAnalysisFlags.ReportDiagnostics);
            ctx.RegisterSyntaxNodeAction(
                AnalyzeMethodSyntaxNode,
                SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeMethodSyntaxNode(SyntaxNodeAnalysisContext ctx) {
            if (ctx.Node is MethodDeclarationSyntax myx) {
                var node_count = myx.Body.DescendantNodes().Count();
                if (node_count > 40) {
                    ctx.ReportDiagnostic(
                        Diagnostic.Create(
                            someDescriptor,
                            myx.Body.GetLocation(),
                            node_count));
                }
            }
        }
    }
}
