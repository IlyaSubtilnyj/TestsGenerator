using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGeneratorLib
{
    internal class TestClass
    {
        public string name;

        public string content;

        public TestClass(string name, string content)
        {
            this.name = name;
            this.content = content;
        }

        public void test(string lol) { }
    }
}
