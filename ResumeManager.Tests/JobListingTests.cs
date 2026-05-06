using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResumeManager;

namespace ResumeManager.Tests
{
    [TestClass]
    public class JobListingTests
    {
        [TestMethod]
        public void Constructor_SetsAllProperties()
        {
            var job = new JobListing("Developer", "TechCorp", "Code", "C#");

            Assert.AreEqual("Developer", job.JobTitle);
            Assert.AreEqual("TechCorp", job.Company);
            Assert.AreEqual("Code", job.Description);
            Assert.AreEqual("C#", job.Requirements);
        }

        [TestMethod]
        public void ToString_ReturnsFormattedString()
        {
            var job = new JobListing("Developer", "TechCorp", "Code", "C#");
            var result = job.ToString();

            StringAssert.Contains(result, "Developer");
            StringAssert.Contains(result, "TechCorp");
        }

        [TestMethod]
        public void Constructor_WithEmptyFields_CreatesObject()
        {
            var job = new JobListing("", "", "", "");

            Assert.IsNotNull(job);
            Assert.AreEqual("", job.JobTitle);
        }

        [TestMethod]
        public void ToString_WithEmptyFields_ReturnsString()
        {
            var job = new JobListing("", "", "", "");
            var result = job.ToString();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(string));
        }
    }
}