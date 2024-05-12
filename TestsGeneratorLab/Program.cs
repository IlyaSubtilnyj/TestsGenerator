using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestsGeneratorLab
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {

                await test();
            } catch (Exception ex)
            {

            }
            Console.WriteLine("Hello, World!");
        }

        public static async Task test()
        {

            // Simulate an asynchronous operation that takes some time
            await Task.Delay(2000);

            string code = @"
                using System;

                public class MyClass
                {
                    public void MyMethod()
                    {
                        Console.WriteLine(""Hello, Roslyn!"");
                    }
                }
            ";

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();

            Console.WriteLine("Syntax analysis successful!");
        }
    }
}