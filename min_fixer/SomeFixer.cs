using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;

namespace min_fixer;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public class SomeFixer : CodeFixProvider {

    // The name as it will appear in the light bulb menu
    private const string FixTitle = "Reduce method size (start it from scratch)";

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("S01");

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext ctx) {
        var root = await ctx.Document.GetSyntaxRootAsync(ctx.CancellationToken);
        var diagnostic = ctx.Diagnostics.First();
        var methodSyntax = root.
            FindToken(diagnostic.Location.SourceSpan.Start).
            Parent.
            AncestorsAndSelf().
            OfType<MethodDeclarationSyntax>().
            First();

        ctx.RegisterCodeFix(
            CodeAction.Create(
                title: FixTitle,
                createChangedDocument: c => FixAsync(ctx.Document, methodSyntax, c),
                equivalenceKey: FixTitle),
            diagnostic);
    }

    private static async Task<Document> FixAsync(Document doc, MethodDeclarationSyntax methodSyntax, CancellationToken ct) {
        var oldRoot = await doc.GetSyntaxRootAsync(ct);
        var newRoot = oldRoot.ReplaceNode(methodSyntax.Body, SyntaxFactory.Block());
        return doc.WithSyntaxRoot(newRoot);
    }
}
