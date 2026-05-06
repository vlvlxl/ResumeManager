using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResumeManager;

namespace ResumeManager.Tests
{
    [TestClass]
    public class JobSearchManagerTests
    {
        private JobSearchManager _manager;

        [TestInitialize]
        public void SetUp()
        {
            _manager = new JobSearchManager();
        }

        [TestMethod]
        public void Constructor_InitializesManager()
        {
            Assert.IsNotNull(_manager);
        }

        [TestMethod]
        public void Manager_CreatesSuccessfully()
        {
            Assert.IsInstanceOfType(_manager, typeof(JobSearchManager));
        }
    }
}