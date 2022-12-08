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

* Before Roslyn
* Current development
* Compiler SDK vs C# language

---

# What are the possibilities

* Analyzers
* Fixers
* Code generation

---

# Analyzer / Fixer Powers

* Hints / Warnings
* IDE Integrations
* MSBuild Integrations
* dothen format -> apply auto fixers 😈
* project references VS nuget references

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

* Miazinton.analyzers
* Sonar analyzer
* Microsoft.CodeAnalysis.NetAnalyzers (included by default, named FxCop in previous life)
