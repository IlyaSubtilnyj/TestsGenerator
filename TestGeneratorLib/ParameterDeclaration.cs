﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGeneratorLib
{
    public struct ParameterDeclaration
    {
        public string Type;

        public string Name;

        public ParameterDeclaration(string type, string name)
        {
            Type = type;

            Name = name;
        }
    }

}
