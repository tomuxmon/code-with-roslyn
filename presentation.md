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
* MSBuild Integrations ([severity levels](https://learn.microsoft.com/en-us/visualstudio/code-quality/roslyn-analyzers-overview?view=vs-2022#severity-levels-of-analyzers))
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

* on linux
* bare bone analyzer
* bare bone fixer

---

# Usefull tools

* [sharplab.io](https://sharplab.io/)
* Avalonia ILSpy (code generation check)
* Visual Studio extension

---

# Demo time!

* `retrun Task.CompletedTask();`
* `Task.Wait();` [Obsolete] -> replace with `ConfigureAwait(false).GetAwaiter().GetResult();`
* calculate cognitive load

---

# Ready to use Analyzers

* Microsoft.CodeAnalysis.NetAnalyzers (included by default, named FxCop in previous life)
* [Roslynator.Analyzers](https://www.nuget.org/packages/Roslynator.Analyzers/)
* [StyleCop.Analyzers](https://www.nuget.org/packages/StyleCop.Analyzers/)
* [Meziantou.Analyzer](https://www.nuget.org/packages/Meziantou.Analyzer/)
* [SonarAnalyzer.CSharp](https://www.nuget.org/packages/SonarAnalyzer.CSharp/)
* know something else? Shout it at me :)
