using System.Reflection;
using System.Threading.Tasks.Dataflow;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace TestGeneratorLib
{
    public class TestGenerator
    {
        public Task Process(List<string> targetFiles, string outputDirectory)
        {

            TransformBlock<string, string> readingBlock = new(
                async path => await File.ReadAllTextAsync(path)
            );

            TransformManyBlock<string, TestClass> processingBlock
                = new(ProcessFile);

            ActionBlock<TestClass> writingBlock = new(
                async info => await File.WriteAllTextAsync(Path.Combine(outputDirectory, info.name + ".cs"), info.content)
            );

            readingBlock.LinkTo(processingBlock, new() { PropagateCompletion = true });
            processingBlock.LinkTo(writingBlock, new() { PropagateCompletion = true });

            foreach (var target in targetFiles)
            {
                readingBlock.Post(target);
            }

            readingBlock.Complete();

            return writingBlock.Completion;
        }

        private IEnumerable<TestClass> ProcessFile(string content)
        {
            IList<ClassDeclaration> infos = GetClassDeclarations(content);

            return infos.Select(i =>
                new TestClass(i.ClassName + "Tests", TestBuilder.Build(i)))
                    .ToList();
        }

        private IList<ClassDeclaration> GetClassDeclarations(string fileContent)
        {

            SAXTraversal traversal = new();
            traversal.Visit(CSharpSyntaxTree.ParseText(fileContent).GetRoot());
            return traversal.ClassesInfo;
        }
    }
}