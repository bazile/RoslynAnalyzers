using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Bazile.Analyzers.Test.CSharpCodeFixVerifier<
    Bazile.Analyzers.StringFormatToStringConcatAnalyzer,
    Bazile.Analyzers.BazileAnalyzersCodeFixProvider>;
using AnalyzerUnderTest = Bazile.Analyzers.StringFormatToStringConcatAnalyzer;

namespace Bazile.Analyzers.Test;

[TestClass]
public class StringFormatToStringConcatAnalyzerTest
{
    [TestMethod]
    public async Task ObjectKeywordIsReported()
    {
        const string source = @"string s1 = ""aaa"", s2 = ""bbb"";
string result = string.Format(""{ 0}{1}"", s1, s2)";

        var expected = CSharpCodeFixVerifier<AnalyzerUnderTest, BazileAnalyzersCodeFixProvider>.Diagnostic(AnalyzerUnderTest.DiagnosticId).WithSpan(2, 5, 2, 34);
        await CSharpCodeFixVerifier<AnalyzerUnderTest, BazileAnalyzersCodeFixProvider>.VerifyAnalyzerAsync(source, expected);
    }
}
