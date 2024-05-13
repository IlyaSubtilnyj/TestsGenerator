using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGeneratorLib
{
    internal class SAXTraversal: CSharpSyntaxWalker
    {
        public IList<ClassDeclaration> ClassesInfo
        { get; } = new List<ClassDeclaration>();

        private string? _namespace = null;

        public override void VisitFileScopedNamespaceDeclaration(FileScopedNamespaceDeclarationSyntax node)
        {
            _namespace = node.Name.ToString();

            foreach (var childNode in node.ChildNodes())
            {
                Visit(childNode);
            }
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            _namespace = node.Name.ToString();

            foreach (var childNode in node.ChildNodes())
            {
                Visit(childNode);
            }
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            SyntaxToken className = node
                .ChildTokens()
                .First(token => token.IsKind(SyntaxKind.IdentifierToken));

            List<MethodDeclaration> methods = node
                .ChildNodes()
                .Where(n => n.IsKind(SyntaxKind.MethodDeclaration) && n.DescendantTokens().Any(n => n.IsKind(SyntaxKind.PublicKeyword))) //sorting by public accessability
                .Select(n =>
                    new MethodDeclaration(
                        n.ChildTokens().First(t => t.IsKind(SyntaxKind.IdentifierToken)).ToString(), //method name
                        n.DescendantNodes().Where(n => n.IsKind(SyntaxKind.Parameter)) //parameters
                            .Select(n =>
                                new ParameterDeclaration(
                                    n.ChildNodes().First().ChildTokens().First().ToString(), //type as first chield token of parameter (string test)
                                    n.ChildTokens().First().ToString()
                                )
                            ).ToList(),
                        n.DescendantNodes().First().ToString() //method return type
                    )
                ).ToList();

            ClassesInfo.Add(
                new ClassDeclaration(
                    _namespace!,
                    className.ToString(),
                    methods
                )
            );
        }

    }
}
