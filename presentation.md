---
marp: true
theme: gaia
_class: lead
style: |
    section{
        color: #002030;
    }
backgroundImage: url('background.jpg')
---

![bg left:40% 80%](roslyn_logo.png)

# Code with Roslyn

Write C# code analyzers

slides at: https://github.com/tomuxmon/code-with-roslyn

---

# What is Roslyn

* Before Roslyn ([System.CodeDom](https://learn.microsoft.com/en-us/dotnet/api/system.codedom))
* [Roslyn](https://en.wikipedia.org/wiki/Roslyn_(compiler)) development (starting 2010 - go prod 2015)
* Compiler SDK vs C# language

<!-- 
System.CodeDom:
only usefull in code generation but not parsing (parsig still a black box)
still need too use CSC to emit IL

Roslyn development started in early 2010.
At 2015 reached maturity and was moved to github.

Roslyn isboth a compiler and an API to create code analysis or other IDE tooling
Also canbe used for code generation.
 -->

---

# What are the possibilities

* *Analyzers* - inspect code and emit warnings in your IDE
* *Fixers* - offer code change suggestions to your IDE
* Code generation - generate code based on your code AST.

---

# Analyzer / Fixer Powers

* Hints / Warnings
* IDE Integrations ([VSCode specifics](https://www.strathweb.com/2019/04/roslyn-analyzers-in-code-fixes-in-omnisharp-and-vs-code/) enableAnalyzersSupport: true)
* MSBuild Integrations ([EnableNETAnalyzers](https://learn.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props#enablenetanalyzers), [severity levels](https://learn.microsoft.com/en-us/visualstudio/code-quality/roslyn-analyzers-overview?view=vs-2022#severity-levels-of-analyzers))
* dothen format -> apply auto fixes 😈

```ini
[*.cs]
dotnet_diagnostic.CA1822.severity = error

[*.MyGenerated.cs]
generated_code = true
```

<!-- 
hints / warnings can be customized on rule basis
VSCode needs additional settinngs

to exclude running analysis ongenerated code you can
also include editor config settings.

 -->

---

# dotnet new analyzer

* minimal analyzer
* minimal fixer

---

# Required nuget packages

```xml
  <ItemGroup>

    <PackageReference Include="Microsoft.CodeAnalysis.Analyzers" Version="3.3.3">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
    </PackageReference>

    <PackageReference Include="Microsoft.CodeAnalysis.BannedApiAnalyzers" Version="3.3.3">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
    </PackageReference>

    <PackageReference Include="Microsoft.CodeAnalysis.CSharp.Workspaces" Version="4.4.0" />
    
  </ItemGroup>
```

---

# Minimal analyzer - outer view

```csharp
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace min_analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SomeAnalyzer : DiagnosticAnalyzer {
    // ...snip... 
}
```

---

# Minimal analyzer - diagnostic descriptor

```csharp
private static readonly DiagnosticDescriptor someDescriptor = new(
    "S01", "Method body token count",
    "Method body contains '{0}' tokens. Reduce it to 40!",
    "Security", DiagnosticSeverity.Warning, true);

public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
    ImmutableArray.Create(someDescriptor);
```

---

# Minimal analyzer - register action

```csharp
public override void Initialize(AnalysisContext ctx) {
    ctx.EnableConcurrentExecution();
    ctx.ConfigureGeneratedCodeAnalysis(
        GeneratedCodeAnalysisFlags.Analyze |
        GeneratedCodeAnalysisFlags.ReportDiagnostics);
    ctx.RegisterSyntaxNodeAction(
        AnalyzeMethodSyntaxNode,
        SyntaxKind.MethodDeclaration);
}
```

---

# Minimal analyzer - report diagnostic

```csharp
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
```

---

# Minimal fixer - outer view

```csharp
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
    // ...snip...
}
```

---

# Minimal fixer - fix what?

```csharp
// The name as it will appear in the light bulb menu
private const string FixTitle = "Reduce method size (start it from scratch)";

public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("S01");

public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;
```

---

# Minimal fixer - register fix

```csharp
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
```

---

# Minimal fixer - replace operation

```csharp
private static async Task<Document> FixAsync(
    Document doc, 
    MethodDeclarationSyntax methodSyntax, 
    CancellationToken ct
) {
    var oldRoot = await doc.GetSyntaxRootAsync(ct);
    var newRoot = oldRoot.ReplaceNode(methodSyntax.Body, SyntaxFactory.Block());
    return doc.WithSyntaxRoot(newRoot);
}
```

---

# Usefull tools

* [sharplab.io](https://sharplab.io/)
* [Visual Studio extension](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/syntax-visualizer?tabs=csharp)

---

# Demo time

---

# Articles about Roslyn

* [Tutorial: Write your first analyzer and code fix](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/tutorials/how-to-write-csharp-analyzer-code-fix)
* [Writing a Roslyn analyzer](https://www.meziantou.net/writing-a-roslyn-analyzer.htm)
* [How to write a Roslyn Analyzer](https://devblogs.microsoft.com/dotnet/how-to-write-a-roslyn-analyzer/)
* [Roslyn analyzers and code-aware library for ImmutableArrays](https://learn.microsoft.com/en-us/visualstudio/extensibility/roslyn-analyzers-and-code-aware-library-for-immutablearrays?view=vs-2022)
* [C# - Adding a Code Fix to Your Roslyn Analyzer](https://learn.microsoft.com/en-us/archive/msdn-magazine/2015/february/csharp-adding-a-code-fix-to-your-roslyn-analyzer)

---

# Ready to use Analyzers

* Microsoft.CodeAnalysis.NetAnalyzers (included by default, named FxCop in previous life)
* [Roslynator.Analyzers](https://www.nuget.org/packages/Roslynator.Analyzers/)
* [StyleCop.Analyzers](https://www.nuget.org/packages/StyleCop.Analyzers/)
* [Meziantou.Analyzer](https://www.nuget.org/packages/Meziantou.Analyzer/)
* [SonarAnalyzer.CSharp](https://www.nuget.org/packages/SonarAnalyzer.CSharp/)
* know something else? Shout it at me :)

---

# Thank you for listening 🎇

Now Questions!
