using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResumeManager;

namespace ResumeManager.Tests
{
    [TestClass]
    public class WorkExperienceTests
    {
        [TestMethod]
        public void Constructor_SetsAllProperties()
        {
            var exp = new WorkExperience("Developer", "TechCorp", "2023-2024", "Code");

            Assert.AreEqual("Developer", exp.Position);
            Assert.AreEqual("TechCorp", exp.Company);
            Assert.AreEqual("2023-2024", exp.Period);
            Assert.AreEqual("Code", exp.Description);
        }

        [TestMethod]
        public void ToString_ReturnsFormattedString()
        {
            var exp = new WorkExperience("Developer", "TechCorp", "2023-2024", "Code");
            var result = exp.ToString();

            StringAssert.Contains(result, "Developer");
            StringAssert.Contains(result, "TechCorp");
            StringAssert.Contains(result, "2023-2024");
        }

        [TestMethod]
        public void Constructor_WithEmptyFields_CreatesObject()
        {
            var exp = new WorkExperience("", "", "", "");

            Assert.IsNotNull(exp);
            Assert.AreEqual("", exp.Position);
            Assert.AreEqual("", exp.Company);
        }

        [TestMethod]
        public void ToString_WithEmptyFields_ReturnsString()
        {
            var exp = new WorkExperience("", "", "", "");
            var result = exp.ToString();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(string));
        }
    }
}