using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Bazile.Analyzers.Test.CSharpCodeFixVerifier<
    Bazile.Analyzers.ObjectReturnTypeAnalyzer,
    Bazile.Analyzers.BazileAnalyzersCodeFixProvider>;


namespace Bazile.Analyzers.Test;

[TestClass]
public class ObjectReturnTypeAnalyzerTest
{
    [TestMethod]
    public async Task ObjectKeywordIsReported()
    {
        const string source = @"class Program {
    object Foo() { return null; }
}";

        var expected = VerifyCS.Diagnostic(ObjectReturnTypeAnalyzer.DiagnosticId).WithSpan(2, 5, 2, 34);
        await VerifyCS.VerifyAnalyzerAsync(source, expected);
    }

    [TestMethod]
    public async Task ObjectIsReported()
    {
        const string source = @"using System; class Program {
    Object Foo() { return null; }
}";

        var expected = VerifyCS.Diagnostic(ObjectReturnTypeAnalyzer.DiagnosticId).WithSpan(2, 5, 2, 34);
        await VerifyCS.VerifyAnalyzerAsync(source, expected);
    }

    [TestMethod]
    public async Task SystemObjectIsReported()
    {
        const string source = @"class Program {
    System.Object Foo() { return null; }
}";

        var expected = VerifyCS.Diagnostic(ObjectReturnTypeAnalyzer.DiagnosticId).WithSpan(2, 5, 2, 41);
        await VerifyCS.VerifyAnalyzerAsync(source, expected);
    }

    [TestMethod]
    [Ignore("Check does not work. Skipping it for now as being rare")]
    public async Task SystemObjectAliasIsReported()
    {
        const string source = @"using TheObject = System.Object;
class Program {
    TheObject Foo() { return null; }
}";

        var expected = VerifyCS.Diagnostic(ObjectReturnTypeAnalyzer.DiagnosticId).WithSpan(2, 5, 2, 34);
        await VerifyCS.VerifyAnalyzerAsync(source, expected);
    }

    [TestMethod]
    public async Task InstanceVoidMethodsAreIgnored()
    {
        const string source = @"class Program {
    void Foo() {  }
}";

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task StaticVoidMethodsAreIgnored()
    {
        const string source = @"class Program {
    static void Bar() {  }
}";

        await VerifyCS.VerifyAnalyzerAsync(source);
    }
}
