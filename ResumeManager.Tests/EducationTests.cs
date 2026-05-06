using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResumeManager;

namespace ResumeManager.Tests
{
    [TestClass]
    public class EducationTests
    {
        [TestMethod]
        public void Constructor_SetsAllProperties()
        {
            var edu = new Education("МГУ", "Бакалавр", "2020-2024");

            Assert.AreEqual("МГУ", edu.Institution);
            Assert.AreEqual("Бакалавр", edu.Degree);
            Assert.AreEqual("2020-2024", edu.Period);
        }

        [TestMethod]
        public void ToString_ReturnsFormattedString()
        {
            var edu = new Education("МГУ", "Бакалавр", "2020-2024");
            var result = edu.ToString();

            StringAssert.Contains(result, "МГУ");
            StringAssert.Contains(result, "Бакалавр");
        }

        [TestMethod]
        public void Constructor_WithEmptyFields_CreatesObject()
        {
            var edu = new Education("", "", "");

            Assert.IsNotNull(edu);
            Assert.AreEqual("", edu.Institution);
        }

        [TestMethod]
        public void ToString_WithEmptyFields_ReturnsString()
        {
            var edu = new Education("", "", "");
            var result = edu.ToString();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(string));
        }
    }
}