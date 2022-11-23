using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Bazile.Analyzers.Test.CSharpCodeFixVerifier<
    Bazile.Analyzers.SingleDimensionZeroSizeArrayAnalyzer,
    Bazile.Analyzers.BazileAnalyzersCodeFixProvider>;

namespace Bazile.Analyzers.Test
{
    [TestClass]
    public class BazileAnalyzersUnitTest
    {
        [TestMethod]
        public async Task SingleDimensionalArray_VerifyCodeFixAsync()
        {
            const string source = @"class Program { void Foo() {
    int[] array = new int[0];
}}";

            const string codeFix = @"int[] array = Array.Empty<int>(0)"; // what about usings?>

            //var expected = VerifyCS.Diagnostic("BazileAnalyzers").WithLocation(0).WithArguments("TypeName");
            var expected = VerifyCS.Diagnostic("SingleDimensionZeroSizeArray");
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
}
