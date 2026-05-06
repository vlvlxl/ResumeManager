using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResumeManager;

namespace ResumeManager.Tests
{
    [TestClass]
    public class ResumeTests
    {
        [TestMethod]
        public void Constructor_WithValidData_InitializesProperties()
        {
            var resume = new Resume("Иван Иванов", "ivan@email.com", "Ищу работу");

            Assert.AreEqual("Иван Иванов", resume.Name);
            Assert.AreEqual("ivan@email.com", resume.ContactInfo);
            Assert.AreEqual("Ищу работу", resume.Objective);
            Assert.IsNotNull(resume.Skills);
            Assert.IsNotNull(resume.WorkExperiences);
            Assert.IsNotNull(resume.Educations);
            Assert.AreEqual(0, resume.Skills.Count);
            Assert.AreEqual(0, resume.WorkExperiences.Count);
            Assert.AreEqual(0, resume.Educations.Count);
        }

        [TestMethod]
        public void SkillsList_InitiallyEmpty()
        {
            var resume = new Resume("Test", "test@test.com", "Objective");
            Assert.IsNotNull(resume.Skills);
            Assert.AreEqual(0, resume.Skills.Count);
        }

        [TestMethod]
        public void WorkExperiencesList_InitiallyEmpty()
        {
            var resume = new Resume("Test", "test@test.com", "Objective");
            Assert.IsNotNull(resume.WorkExperiences);
            Assert.AreEqual(0, resume.WorkExperiences.Count);
        }

        [TestMethod]
        public void EducationsList_InitiallyEmpty()
        {
            var resume = new Resume("Test", "test@test.com", "Objective");
            Assert.IsNotNull(resume.Educations);
            Assert.AreEqual(0, resume.Educations.Count);
        }

        [TestMethod]
        public void AddSkill_AddsSkillToList()
        {
            var resume = new Resume("Test", "test@test.com", "Objective");
            resume.AddSkill("C#");
            Assert.AreEqual(1, resume.Skills.Count);
            Assert.AreEqual("C#", resume.Skills[0]);
        }

        [TestMethod]
        public void AddSkill_MultipleSkills_AddsAll()
        {
            var resume = new Resume("Test", "test@test.com", "Objective");
            resume.AddSkill("C#");
            resume.AddSkill("Git");
            resume.AddSkill("SQL");

            Assert.AreEqual(3, resume.Skills.Count);
        }

        [TestMethod]
        public void AddWorkExperience_AddsExperienceToList()
        {
            var resume = new Resume("Test", "test@test.com", "Objective");
            resume.AddWorkExperience("Developer", "Company", "2023-2024", "Description");

            Assert.AreEqual(1, resume.WorkExperiences.Count);
            Assert.AreEqual("Developer", resume.WorkExperiences[0].Position);
        }

        [TestMethod]
        public void AddEducation_AddsEducationToList()
        {
            var resume = new Resume("Test", "test@test.com", "Objective");
            resume.AddEducation("МГУ", "Бакалавр", "2020-2024");

            Assert.AreEqual(1, resume.Educations.Count);
            Assert.AreEqual("МГУ", resume.Educations[0].Institution);
        }
    }
}