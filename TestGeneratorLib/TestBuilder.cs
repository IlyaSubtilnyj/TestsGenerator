using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TestGeneratorLib
{
    internal class TestBuilder
    {
        public static string Build(ClassDeclaration classInfo)
        {

            List<MemberDeclarationSyntax> classMembers = new();
            classMembers.AddRange(
                classInfo.Methods.Select(CreateDefaultTestMethod)
            );

            return CompilationUnit()
                .WithUsings(
                    List<UsingDirectiveSyntax>(
                        new UsingDirectiveSyntax[]
                        {
                             UsingDirective(ParseName("Microsoft.VisualStudio.TestTools.UnitTesting"))
                                .WithUsingKeyword(Token(SyntaxKind.UsingKeyword).WithTrailingTrivia(Space))
                                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken).WithTrailingTrivia(LineFeed)),
                        }))
                .WithMembers(
                    SingletonList<MemberDeclarationSyntax>(
                        NamespaceDeclaration(
                            QualifiedName(
                                IdentifierName(classInfo.Namespace),
                                IdentifierName(
                                    Identifier(
                                        TriviaList(),
                                        "Tests",
                                        TriviaList(
                                            LineFeed)))))
                        .WithNamespaceKeyword(
                            Token(
                                TriviaList(
                                    LineFeed),
                                SyntaxKind.NamespaceKeyword,
                                TriviaList(
                                    Space)))
                        .WithOpenBraceToken(
                            Token(
                                TriviaList(),
                                SyntaxKind.OpenBraceToken,
                                TriviaList(
                                    LineFeed)))
                        .WithMembers(
                            SingletonList<MemberDeclarationSyntax>(
                                ClassDeclaration(
                                    Identifier(
                                        TriviaList(),
                                        classInfo.ClassName + "Tests",
                                        TriviaList(
                                            LineFeed)))
                                .WithAttributeLists(
                                    SingletonList<AttributeListSyntax>(
                                        AttributeList(
                                            SingletonSeparatedList<AttributeSyntax>(
                                                Attribute(
                                                    IdentifierName("TestClass"))))
                                        .WithOpenBracketToken(
                                            Token(
                                                TriviaList(
                                                    Whitespace("    ")),
                                                SyntaxKind.OpenBracketToken,
                                                TriviaList()))
                                        .WithCloseBracketToken(
                                            Token(
                                                TriviaList(),
                                                SyntaxKind.CloseBracketToken,
                                                TriviaList(
                                                    LineFeed)))))
                                .WithModifiers(
                                    TokenList(
                                        Token(
                                            TriviaList(
                                                Whitespace("    ")),
                                            SyntaxKind.PublicKeyword,
                                            TriviaList(
                                                Space))))
                                .WithKeyword(
                                    Token(
                                        TriviaList(),
                                        SyntaxKind.ClassKeyword,
                                        TriviaList(
                                            Space)))
                                .WithOpenBraceToken(
                                    Token(
                                        TriviaList(
                                            Whitespace("    ")),
                                        SyntaxKind.OpenBraceToken,
                                        TriviaList(
                                            LineFeed)))
                                .WithMembers(
                                    List<MemberDeclarationSyntax>(
                                        classMembers
                                    )
                                )
                                .WithCloseBraceToken(
                                    Token(
                                        TriviaList(
                                            Whitespace("    ")),
                                        SyntaxKind.CloseBraceToken,
                                        TriviaList(
                                            LineFeed)))))
                        .WithCloseBraceToken(
                            Token(
                                TriviaList(),
                                SyntaxKind.CloseBraceToken,
                                TriviaList(
                                    LineFeed))))).ToString();
        
        }

        public StatementSyntax CreateLocalVariable(ParameterDeclaration parameterInfo)
        {

            return
                    LocalDeclarationStatement(
                        VariableDeclaration(
                            IdentifierName(
                                Identifier(
                                    TriviaList(
                                        Whitespace("            ")),
                                    parameterInfo.Type,
                                    TriviaList(
                                        Space))))
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                VariableDeclarator(
                                    Identifier(
                                        TriviaList(),
                                        parameterInfo.Name,
                                        TriviaList(
                                            Space)))
                                .WithInitializer(
                                    EqualsValueClause(
                                        DefaultExpression(
                                            IdentifierName(parameterInfo.Type)))
                                    .WithEqualsToken(
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.EqualsToken,
                                            TriviaList(
                                                Space)))))))
                    .WithSemicolonToken(
                        Token(
                            TriviaList(),
                            SyntaxKind.SemicolonToken,
                            TriviaList(
                                LineFeed)));
        }

        public ArgumentListSyntax CreateMethodArgumentList(IEnumerable<ParameterDeclaration> parameters)
        {
            return
                ArgumentList(
                    SeparatedList<ArgumentSyntax>(
                        parameters
                        .SelectMany<ParameterDeclaration, SyntaxNodeOrToken>(i =>

                        new SyntaxNodeOrToken[]
                        {
                            Argument(
                                IdentifierName(i.Name)),
                            Token(
                                TriviaList(),
                                SyntaxKind.CommaToken,
                                TriviaList(
                                    Space))
                        })
                        .Take(..^1)
                        .ToArray()
                    )
                );
        }

        public IEnumerable<StatementSyntax> CreateArrangeSection(IEnumerable<ParameterDeclaration> parameters)
        {
            List<StatementSyntax> nodes = parameters
                .Select(CreateLocalVariable)
                .ToList();

            return nodes;
        }

        public List<StatementSyntax> CreateVoidActSection(
            MethodDeclaration methodInfo,
            string classFieldName
            )
        {
            ArgumentListSyntax methodArgs = CreateMethodArgumentList(methodInfo.Parameters);

            return new List<StatementSyntax> {
                ExpressionStatement(
                    InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName(
                                    Identifier(
                                            TriviaList(
                                                LineFeed,
                                                Whitespace("            ")
                                            ),
                                            classFieldName,
                                            TriviaList()
                                    )
                                ),
                                IdentifierName(methodInfo.Name)))
                        .WithArgumentList(
                            methodArgs
                        ))
                .WithSemicolonToken(
                    Token(
                        TriviaList(),
                        SyntaxKind.SemicolonToken,
                        TriviaList(
                            LineFeed)))
            };
        }

        public List<StatementSyntax> CreateNotVoidActSection(
            MethodDeclaration methodInfo,
            string classFieldName
            )
        {
            ArgumentListSyntax methodArgs = CreateMethodArgumentList(methodInfo.Parameters);

            return new List<StatementSyntax> {
                LocalDeclarationStatement(
                        VariableDeclaration(
                                IdentifierName(
                                    Identifier(
                                        TriviaList(
                                            LineFeed,
                                            Whitespace("            ")
                                        ),
                                        methodInfo.ReturnType,
                                        TriviaList(
                                            Space))))
                            .WithVariables(
                                SingletonSeparatedList<VariableDeclaratorSyntax>(
                                    VariableDeclarator(
                                            Identifier(
                                                TriviaList(),
                                                "actual",
                                                TriviaList(
                                                    Space)))
                                        .WithInitializer(
                                            EqualsValueClause(
                                                    InvocationExpression(
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                IdentifierName(classFieldName),
                                                                IdentifierName(methodInfo.Name)))
                                                        .WithArgumentList(
                                                            methodArgs
                                                        ))
                                                .WithEqualsToken(
                                                    Token(
                                                        TriviaList(),
                                                        SyntaxKind.EqualsToken,
                                                        TriviaList(
                                                            Space)))))))
                    .WithSemicolonToken(
                        Token(
                            TriviaList(),
                            SyntaxKind.SemicolonToken,
                            TriviaList(
                                LineFeed,
                                LineFeed)))
            };
        }

        public List<StatementSyntax> CreateNotVoidAssertSection(MethodDeclaration methodInfo)
        {
            List<StatementSyntax> nodes =

            new List<StatementSyntax>
            {
                LocalDeclarationStatement(
                        VariableDeclaration(
                                IdentifierName(
                                    Identifier(
                                        TriviaList(
                                                Whitespace("            ")
                                            ),
                                        methodInfo.ReturnType,
                                        TriviaList(
                                            Space))))
                            .WithVariables(
                                SingletonSeparatedList<VariableDeclaratorSyntax>(
                                    VariableDeclarator(
                                            Identifier(
                                                TriviaList(),
                                                "expected",
                                                TriviaList(
                                                    Space)))
                                        .WithInitializer(
                                            EqualsValueClause(
                                                    DefaultExpression(
                                                        IdentifierName(methodInfo.ReturnType)))
                                                .WithEqualsToken(
                                                    Token(
                                                        TriviaList(),
                                                        SyntaxKind.EqualsToken,
                                                        TriviaList(
                                                            Space)))))))
                    .WithSemicolonToken(
                        Token(
                            TriviaList(),
                            SyntaxKind.SemicolonToken,
                            TriviaList(
                                LineFeed))),

                ExpressionStatement(
                    InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName(
                                    Identifier(
                                        TriviaList(
                                            Whitespace("            ")),
                                        "Assert",
                                        TriviaList())),
                                IdentifierName("AreEqual")))
                        .WithArgumentList(
                            ArgumentList(
                                SeparatedList<ArgumentSyntax>(
                                    new SyntaxNodeOrToken[]{
                                    Argument(
                                        IdentifierName("actual")),
                                    Token(
                                        TriviaList(),
                                        SyntaxKind.CommaToken,
                                        TriviaList(
                                            Space)),
                                    Argument(
                                        IdentifierName("expected"))}))))
                    .WithSemicolonToken(
                        Token(
                            TriviaList(),
                            SyntaxKind.SemicolonToken,
                            TriviaList(
                                LineFeed))),

                ExpressionStatement(
                        InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName(
                                        Identifier(
                                            TriviaList(Whitespace("            ")),
                                            "Assert",
                                            TriviaList()
                                        )
                                    ),
                                    IdentifierName("Fail")))
                            .WithArgumentList(
                                ArgumentList(
                                    SingletonSeparatedList<ArgumentSyntax>(
                                        Argument(
                                            LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                Literal("autogenerated")))))))
                    .WithSemicolonToken(
                        Token(
                            TriviaList(),
                            SyntaxKind.SemicolonToken,
                            TriviaList(
                                LineFeed)))
            };

            return nodes;
        }

        public List<StatementSyntax> CreateVoidAssertSection()
        {
            return new List<StatementSyntax> {
                ExpressionStatement(
                    InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName(
                                    Identifier(
                                        TriviaList(
                                            LineFeed,
                                            Whitespace("            ")
                                        ),
                                        "Assert",
                                        TriviaList()
                                    )
                                ),
                                IdentifierName("Fail")))
                        .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList<ArgumentSyntax>(
                                    Argument(
                                        LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal("autogenerated")))))))
                .WithSemicolonToken(
                    Token(
                        TriviaList(),
                        SyntaxKind.SemicolonToken,
                        TriviaList(
                            LineFeed)))
            };
        }

        public IEnumerable<FieldDeclarationSyntax> CreateFieldDeclarations(List<FieldDeclaration> parameters, FieldDeclaration classFieldInfo)
        {
            List<FieldDeclarationSyntax> res = parameters.Select(param =>
                FieldDeclaration(
                    VariableDeclaration(
                        GenericName(
                            Identifier("Mock"))
                        .WithTypeArgumentList(
                            TypeArgumentList(
                                SingletonSeparatedList<TypeSyntax>(
                                    IdentifierName(param.Type)))
                            .WithGreaterThanToken(
                                Token(
                                    TriviaList(),
                                    SyntaxKind.GreaterThanToken,
                                    TriviaList(
                                        Space)))))
                    .WithVariables(
                        SingletonSeparatedList<VariableDeclaratorSyntax>(
                            VariableDeclarator(
                                Identifier(param.Name)))))
                .WithModifiers(
                    TokenList(
                        Token(
                            TriviaList(
                                Whitespace("        ")),
                            SyntaxKind.PrivateKeyword,
                            TriviaList(
                                Space))))
                .WithSemicolonToken(
                    Token(
                        TriviaList(),
                        SyntaxKind.SemicolonToken,
                        TriviaList(
                            LineFeed)))
            ).ToList();

            res.Add(
                FieldDeclaration(
                        VariableDeclaration(
                                IdentifierName(
                                    Identifier(
                                        TriviaList(),
                                        classFieldInfo.Type,
                                        TriviaList(
                                            Space))))
                            .WithVariables(
                                SingletonSeparatedList<VariableDeclaratorSyntax>(
                                    VariableDeclarator(
                                        Identifier(classFieldInfo.Name)))))
                    .WithModifiers(
                        TokenList(
                            Token(
                                TriviaList(
                                    Whitespace("        ")),
                                SyntaxKind.PrivateKeyword,
                                TriviaList(
                                    Space))))
                    .WithSemicolonToken(
                        Token(
                            TriviaList(),
                            SyntaxKind.SemicolonToken,
                            TriviaList(
                                LineFeed)))
            );

            return res;
        }

        public MethodDeclarationSyntax CreateInitializeMethod(List<FieldDeclaration> fields, FieldDeclaration classFieldInfo)
        {
            List<StatementSyntax> methodStatements = new();
            methodStatements.AddRange(
                        fields.Select(field =>
                            ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    IdentifierName(
                                        Identifier(
                                            TriviaList(
                                                Whitespace("            ")),
                                            field.Name,
                                            TriviaList(
                                                Space))),
                                    ObjectCreationExpression(
                                        GenericName(
                                            Identifier("Mock"))
                                        .WithTypeArgumentList(
                                            TypeArgumentList(
                                                SingletonSeparatedList<TypeSyntax>(
                                                    IdentifierName(field.Type)))))
                                    .WithNewKeyword(
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.NewKeyword,
                                            TriviaList(
                                                Space)))
                                    .WithArgumentList(
                                        ArgumentList()))
                                .WithOperatorToken(
                                    Token(
                                        TriviaList(),
                                        SyntaxKind.EqualsToken,
                                        TriviaList(
                                            Space))))
                            .WithSemicolonToken(
                                Token(
                                    TriviaList(),
                                    SyntaxKind.SemicolonToken,
                                    TriviaList(
                                        LineFeed)))
                        )
                    );

            methodStatements.Add(
                    ExpressionStatement(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName(
                            Identifier(
                                TriviaList(
                                    Whitespace("            ")),
                                classFieldInfo.Name,
                                TriviaList(
                                    Space))),
                        ObjectCreationExpression(
                            IdentifierName(classFieldInfo.Type))
                        .WithNewKeyword(
                            Token(
                                TriviaList(),
                                SyntaxKind.NewKeyword,
                                TriviaList(
                                    Space)))
                        .WithArgumentList(
                            ArgumentList(
                                SeparatedList<ArgumentSyntax>(
                                    fields.SelectMany<FieldDeclaration, SyntaxNodeOrToken>(f =>

                                        new SyntaxNodeOrToken[]
                                        {
                                            Argument(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName(f.Name),
                                                    IdentifierName("Object")
                                                )
                                            ),
                                            Token(
                                                TriviaList(),
                                                SyntaxKind.CommaToken,
                                                TriviaList(Space)
                                            )
                                        }
                                    )
                                    .Take(..^1)
                                    .ToArray()
                                ))))
                    .WithOperatorToken(
                        Token(
                            TriviaList(),
                            SyntaxKind.EqualsToken,
                            TriviaList(
                                Space))))
                .WithSemicolonToken(
                    Token(
                        TriviaList(),
                        SyntaxKind.SemicolonToken,
                        TriviaList(
                            LineFeed)))
            );

            return
                    MethodDeclaration(
                        PredefinedType(
                            Token(
                                TriviaList(),
                                SyntaxKind.VoidKeyword,
                                TriviaList(
                                    Space))),
                        Identifier("Initialization"))
                    .WithAttributeLists(
                        SingletonList<AttributeListSyntax>(
                            AttributeList(
                                SingletonSeparatedList<AttributeSyntax>(
                                    Attribute(
                                        IdentifierName("TestInitialize"))))
                            .WithOpenBracketToken(
                                Token(
                                    TriviaList(
                                        new[]{
                                    LineFeed,
                                    Whitespace("        ")}),
                                    SyntaxKind.OpenBracketToken,
                                    TriviaList()))
                            .WithCloseBracketToken(
                                Token(
                                    TriviaList(),
                                    SyntaxKind.CloseBracketToken,
                                    TriviaList(
                                        LineFeed)))))
                    .WithModifiers(
                        TokenList(
                            Token(
                                TriviaList(
                                    Whitespace("        ")),
                                SyntaxKind.PublicKeyword,
                                TriviaList(
                                    Space))))
                    .WithParameterList(
                        ParameterList()
                        .WithCloseParenToken(
                            Token(
                                TriviaList(),
                                SyntaxKind.CloseParenToken,
                                TriviaList(
                                    LineFeed))))
                    .WithBody(
                        Block(
                                methodStatements
                        )
                        .WithOpenBraceToken(
                            Token(
                                TriviaList(
                                    Whitespace("        ")),
                                SyntaxKind.OpenBraceToken,
                                TriviaList(
                                    LineFeed)))
                        .WithCloseBraceToken(
                            Token(
                                TriviaList(
                                    Whitespace("        ")),
                                SyntaxKind.CloseBraceToken,
                                TriviaList())));
        }

        public static MethodDeclarationSyntax CreateDefaultTestMethod(MethodDeclaration methodInfo)
        {
            return
                    MethodDeclaration(
                        PredefinedType(
                            Token(
                                TriviaList(),
                                SyntaxKind.VoidKeyword,
                                TriviaList(
                                    Space))),
                        Identifier(methodInfo.Name + "Test"))
                    .WithAttributeLists(
                        SingletonList<AttributeListSyntax>(
                            AttributeList(
                                SingletonSeparatedList<AttributeSyntax>(
                                    Attribute(
                                        IdentifierName("TestMethod"))))
                            .WithOpenBracketToken(
                                Token(
                                    TriviaList(
                                        Whitespace("        ")),
                                    SyntaxKind.OpenBracketToken,
                                    TriviaList()))
                            .WithCloseBracketToken(
                                Token(
                                    TriviaList(),
                                    SyntaxKind.CloseBracketToken,
                                    TriviaList(
                                        LineFeed)))))
                    .WithModifiers(
                        TokenList(
                            Token(
                                TriviaList(
                                    Whitespace("        ")),
                                SyntaxKind.PublicKeyword,
                                TriviaList(
                                    Space))))
                    .WithParameterList(
                        ParameterList()
                        .WithCloseParenToken(
                            Token(
                                TriviaList(),
                                SyntaxKind.CloseParenToken,
                                TriviaList(
                                    LineFeed))))
                    .WithBody(
                        Block(
                            SingletonList<StatementSyntax>(
                                ExpressionStatement(
                                    InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName(
                                                Identifier(
                                                    TriviaList(
                                                        Whitespace("            ")),
                                                    "Assert",
                                                    TriviaList())),
                                            IdentifierName("Fail")))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SingletonSeparatedList<ArgumentSyntax>(
                                                Argument(
                                                    LiteralExpression(
                                                        SyntaxKind.StringLiteralExpression,
                                                        Literal("autogenerated")))))))
                                .WithSemicolonToken(
                                    Token(
                                        TriviaList(),
                                        SyntaxKind.SemicolonToken,
                                        TriviaList(
                                            LineFeed)))))
                        .WithOpenBraceToken(
                            Token(
                                TriviaList(
                                    Whitespace("        ")),
                                SyntaxKind.OpenBraceToken,
                                TriviaList(
                                    LineFeed)))
                        .WithCloseBraceToken(
                            Token(
                                TriviaList(
                                    Whitespace("        ")),
                                SyntaxKind.CloseBraceToken,
                                TriviaList(
                                    LineFeed))));
        }

        public MethodDeclarationSyntax CreateTestMethod(MethodDeclaration methodInfo, FieldDeclaration classField)
        {
            List<StatementSyntax> methodBody = new();
            methodBody.AddRange(
                CreateArrangeSection(methodInfo.Parameters)
            );

            if (!methodInfo.ReturnType.Equals("void"))
            {
                methodBody.AddRange(
                    CreateNotVoidActSection(methodInfo, classField.Name)
                );
                methodBody.AddRange(
                    CreateNotVoidAssertSection(methodInfo)
                );
            }
            else
            {
                methodBody.AddRange(
                    CreateVoidActSection(methodInfo, classField.Name)
                );
                methodBody.AddRange(
                    CreateVoidAssertSection()
                );
            }

            return
                    MethodDeclaration(
                        PredefinedType(
                            Token(
                                TriviaList(),
                                SyntaxKind.VoidKeyword,
                                TriviaList(
                                    Space))),
                        Identifier(methodInfo.Name))
                    .WithAttributeLists(
                        SingletonList<AttributeListSyntax>(
                            AttributeList(
                                SingletonSeparatedList<AttributeSyntax>(
                                    Attribute(
                                        IdentifierName("TestMethod"))))
                            .WithOpenBracketToken(
                                Token(
                                    TriviaList(
                                        new[]{
                                        LineFeed,
                                        LineFeed,
                                        Whitespace("        ")}),
                                    SyntaxKind.OpenBracketToken,
                                    TriviaList()))
                            .WithCloseBracketToken(
                                Token(
                                    TriviaList(),
                                    SyntaxKind.CloseBracketToken,
                                    TriviaList(
                                        LineFeed)))))
                    .WithModifiers(
                        TokenList(
                            Token(
                                TriviaList(
                                    Whitespace("        ")),
                                SyntaxKind.PublicKeyword,
                                TriviaList(
                                    Space))))
                    .WithParameterList(
                        ParameterList()
                        .WithCloseParenToken(
                            Token(
                                TriviaList(),
                                SyntaxKind.CloseParenToken,
                                TriviaList(
                                    LineFeed))))
                    .WithBody(
                        Block(
                            methodBody
                        )
                        .WithOpenBraceToken(
                            Token(
                                TriviaList(
                                    Whitespace("        ")),
                                SyntaxKind.OpenBraceToken,
                                TriviaList(
                                    LineFeed)))
                        .WithCloseBraceToken(
                            Token(
                                TriviaList(
                                    Whitespace("        ")),
                                SyntaxKind.CloseBraceToken,
                                TriviaList(
                                    LineFeed))));
        }
    }
}
