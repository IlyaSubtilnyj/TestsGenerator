using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestGeneratorLib;

namespace TestsGeneratorLab
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
           
            TestGenerator generator = new TestGenerator();
            Task task = generator.Process(
                    new List<string> 
                    { 
                        @"D:\workspace\Visual_Studio_workspace\studing_workspace\SppForthLab\TestGeneratorLib\TestClass.cs" 
                    }, 
                    @"D:\workspace\Visual_Studio_workspace\studing_workspace\SppForthLab\TestsGeneratorLab\output"
                );
            task.Wait();
        }
    }
}