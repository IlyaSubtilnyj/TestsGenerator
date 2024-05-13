using TestGeneratorLib;

namespace TestsGeneratorTests
{
    [TestClass]
    public class TestsGenerator
    {
        private TestGenerator generator             = new TestGenerator();
        public readonly string ActualDirectory      = "D:\\workspace\\Visual_Studio_workspace\\studing_workspace\\SppForthLab\\TestsGeneratorTests\\stuff\\actual\\";
        public readonly string ExpectedDirectory    = "D:\\workspace\\Visual_Studio_workspace\\studing_workspace\\SppForthLab\\TestsGeneratorTests\\stuff\\expected\\";
        public readonly string OutputDirectory      = "D:\\workspace\\Visual_Studio_workspace\\studing_workspace\\SppForthLab\\TestsGeneratorLab\\output\\";

        [TestMethod]
        public void Generator_WhenNoMethodsInClass()
        {
            string expected = File.ReadAllText(ExpectedDirectory + "Expected1.cs").Replace("\r", "");

            var task = generator.Process(
                 new List<string>
                    {
                        ActualDirectory +  "Actual1.cs"
                    },
                    OutputDirectory
                );

            task.Wait();

            string actual = File.ReadAllText(OutputDirectory + "Actual1Tests.cs").Replace("\r", "");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Generator_WhenMethodsExistInClass()
        {
            string expected = File.ReadAllText(ExpectedDirectory + "Expected2.cs").Replace("\r", "");

            var task = generator.Process(
                 new List<string>
                    {
                        ActualDirectory +  "Actual2.cs"
                    },
                    OutputDirectory
                );

            task.Wait();

            string actual = File.ReadAllText(OutputDirectory + "Actual2Tests.cs").Replace("\r", "");

            Assert.AreEqual(expected, actual);
        }
    }
}