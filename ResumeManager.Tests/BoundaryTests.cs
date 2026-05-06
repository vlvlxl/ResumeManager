using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResumeManager;

namespace ResumeManager.Tests
{
    [TestClass]
    public class BoundaryTests
    {
        [TestMethod]
        public void Resume_SkillsList_InitiallyEmpty()
        {
            var resume = new Resume("Test", "test@test.com", "Objective");
            Assert.IsNotNull(resume.Skills);
            Assert.AreEqual(0, resume.Skills.Count);
        }

        [TestMethod]
        public void Resume_WorkExperiencesList_InitiallyEmpty()
        {
            var resume = new Resume("Test", "test@test.com", "Objective");
            Assert.IsNotNull(resume.WorkExperiences);
            Assert.AreEqual(0, resume.WorkExperiences.Count);
        }

        [TestMethod]
        public void Resume_EducationsList_InitiallyEmpty()
        {
            var resume = new Resume("Test", "test@test.com", "Objective");
            Assert.IsNotNull(resume.Educations);
            Assert.AreEqual(0, resume.Educations.Count);
        }

        [TestMethod]
        public void WorkExperience_ToString_WithEmptyFields_ReturnsFormatted()
        {
            var exp = new WorkExperience("", "", "", "");
            var result = exp.ToString();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Education_ToString_WithEmptyFields_ReturnsFormatted()
        {
            var edu = new Education("", "", "");
            var result = edu.ToString();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void JobListing_ToString_WithEmptyFields_ReturnsFormatted()
        {
            var job = new JobListing("", "", "", "");
            var result = job.ToString();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Resume_WithValidData_CreatesSuccessfully()
        {
            var resume = new Resume("Иван", "ivan@email.com", "Цель");
            Assert.IsNotNull(resume);
            Assert.AreEqual("Иван", resume.Name);
        }

        [TestMethod]
        public void Resume_WithEmptyName_CreatesSuccessfully()
        {
            var resume = new Resume("", "ivan@email.com", "Цель");
            Assert.IsNotNull(resume);
            Assert.AreEqual("", resume.Name); 
        }
    }
}