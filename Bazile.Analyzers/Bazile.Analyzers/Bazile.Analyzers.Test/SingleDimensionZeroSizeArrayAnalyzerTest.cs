using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Bazile.Analyzers.Test.CSharpCodeFixVerifier<
    Bazile.Analyzers.SingleDimensionZeroSizeArrayAnalyzer,
    Bazile.Analyzers.BazileAnalyzersCodeFixProvider>;

namespace Bazile.Analyzers.Test;

[TestClass]
public class SingleDimensionZeroSizeArrayAnalyzerTest
{
    [TestMethod]
    public async Task SingleDimensionalArray01_VerifyCodeFixAsync()
    {
        const string source = @"using System; class Program { void Foo() {
    int[] array = new int[0];
}}";

        // TODO: what about usings?>
        const string codeFix = @"using System; class Program { void Foo() {
    int[] array = Array.Empty<int>();
}}";

        //var expected = VerifyCS.Diagnostic("BazileAnalyzers").WithLocation(0).WithArguments("TypeName");
        var expected = VerifyCS.Diagnostic("SingleDimensionZeroSizeArray").WithSpan(2, 19, 2, 29);
        await VerifyCS.VerifyCodeFixAsync(source, expected, codeFix);
    }

    [TestMethod]
    public async Task SingleDimensionalArray02_VerifyCodeFixAsync()
    {
        const string source = @"using System; class Program { void Foo() {
    var array = new int[0];
}}";

        const string codeFix = @"using System; class Program { void Foo() {
    var array = Array.Empty<int>();
}}"; // what about usings?>

        //var expected = VerifyCS.Diagnostic("BazileAnalyzers").WithLocation(0).WithArguments("TypeName");
        var expected = VerifyCS.Diagnostic("SingleDimensionZeroSizeArray").WithSpan(2, 17, 2, 27);
        await VerifyCS.VerifyCodeFixAsync(source, expected, codeFix);
    }

    [TestMethod]
    public async Task SingleDimensionalArray03_VerifyCodeFixAsync()
    {
        const string source = @"using System; class Program { void Foo(int[] array) {
    Foo(new int[0]);
}}";

        const string codeFix = @"using System; class Program { void Foo(int[] array) {
    Foo(Array.Empty<int>());
}}";

        //var expected = VerifyCS.Diagnostic("BazileAnalyzers").WithLocation(0).WithArguments("TypeName");
        var expected = VerifyCS.Diagnostic("SingleDimensionZeroSizeArray").WithSpan(2, 9, 2, 19);
        await VerifyCS.VerifyCodeFixAsync(source, expected, codeFix);
    }

    [TestMethod]
    public async Task EmptySource_Is_Ignored()
    {
        const string source = "";

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task SingleDimensionalArray_WithConstSize_Is_Ignored() // ???
    {
        const string source = @"class Program { void Foo() {
    const int size = 0;
    int[] array = new int[size];
}}";

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task SingleDimensionalArray_WithVariableSize_Is_Ignored()
    {
        const string source = @"class Program { void Foo() {
    int size = int.Parse(System.Console.ReadLine());
    int[] array = new int[size];
}}";

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task SingleDimensionalArray_WithNonZeroSize_Is_Ignored()
    {
        const string source = @"class Program { void Foo() {
    int[] array = new int[5];
}}";

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task TwoDimensionalArray_Is_Ignored()
    {
        const string source = @"class Program { void Foo() {
    int[,] array = new int[0,0];
}}";

        await VerifyCS.VerifyAnalyzerAsync(source);
    }
}
