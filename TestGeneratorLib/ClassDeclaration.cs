using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestGeneratorLib
{
    public struct ClassDeclaration
    {
        public string Namespace;

        public string ClassName;

        public IList<MethodDeclaration> Methods;

        public ClassDeclaration(string namepsace, string className, IList<MethodDeclaration> methods)
        {
            ClassName = className;
            Methods = methods;
            Namespace = namepsace;
        }
    }

}
