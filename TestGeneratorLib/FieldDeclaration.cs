﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGeneratorLib
{
    public struct FieldDeclaration
    {
        public string Type;

        public string Name;

        public FieldDeclaration(string type, string name)
        {
            Name = name;
            Type = type;
        }
    }
}
