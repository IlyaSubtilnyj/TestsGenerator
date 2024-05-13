using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestGeneratorLib
{
    public struct MethodDeclaration
    {
        public string ReturnType;

        public string Name;

        public IList<ParameterDeclaration> Parameters;

        public MethodDeclaration(string name, IList<ParameterDeclaration> parameters, string returnType)
        {
            Name = name;
            Parameters = parameters;
            ReturnType = returnType;
        }
    }

}
