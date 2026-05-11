using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using ResumeManager;
using System.Text;
using System;

namespace ResumeManager.Tests
{
    [TestClass]
    public class ResumeAnalyzerTests
    {
        private Resume CreateTestResume(string name, string contactInfo, string objective, List<string> skills, List<WorkExperience> workExperiences = null, List<Education> educations = null)
        {
            var resume = new Resume(name, contactInfo, objective);
            resume.Skills = skills ?? new List<string>();
            resume.WorkExperiences = workExperiences ?? new List<WorkExperience>();
            resume.Educations = educations ?? new List<Education>();
            return resume;
        }

        private JobListing CreateTestJob(string title, string company, string description, string requirements)
        {
            return new JobListing(title, company, description, requirements);
        }

        private WorkExperience CreateTestWorkExperience(string position = "Должность", string company = "Компания", string period = "2020-2023", string description = "Описание")
        {
            return new WorkExperience(position, company, period, description);
        }

        private Education CreateTestEducation(string institution = "Институт", string degree = "Степень", string period = "2016-2020")
        {
            return new Education(institution, degree, period);
        }

        [TestMethod]
        public void Analyze_PartialMatch_ShouldReturnCorrectPercentage()
        {
            var resume = CreateTestResume("Анна", "anna@test.com", "Инженер",
                new List<string> { "Testing", "SQL" });
            var job = CreateTestJob("Engineer", "TestLab", "Тестирование", "Testing, SQL, Selenium, Jira");

            var result = ResumeAnalyzer.Analyze(resume, job);

            Assert.AreEqual(50, result.MatchPercentage);
            Assert.AreEqual(2, result.MissingSkills.Count);
            Assert.IsTrue(result.MissingSkills.Contains("Selenium"));
            Assert.IsTrue(result.MissingSkills.Contains("Jira"));
        }

        [TestMethod]
        public void Analyze_ZeroMatch_ShouldReturn0Percent()
        {
            var resume = CreateTestResume("Петр", "petr@test.com", "Дизайнер",
                new List<string> { "Photoshop", "Figma" });
            var job = CreateTestJob("Backend Dev", "CodeCo", "Серверная часть", "C#, Java, Paint, Excel");

            var result = ResumeAnalyzer.Analyze(resume, job);

            Assert.AreEqual(0, result.MatchPercentage);
            Assert.AreEqual(4, result.MissingSkills.Count);
        }

        [TestMethod]
        public void Analyze_ShouldIgnoreCase_WhenComparingSkills()
        {
            var resume = CreateTestResume("Мария", "maria@test.com", "Разработчик",
                new List<string> { "c#", "GIT", "sql" });
            var job = CreateTestJob("Junior Dev", "SoftInc", "Backend", "C#, Git, SQL");

            var result = ResumeAnalyzer.Analyze(resume, job);

            Assert.AreEqual(100, result.MatchPercentage);
            Assert.AreEqual(0, result.MissingSkills.Count);
        }

        [TestMethod]
        public void Analyze_ShouldParseCommaSeparatedSkillsInResume()
        {
            var resume = CreateTestResume("Ольга", "olga@test.com", "Аналитик",
                new List<string> { "SQL, Python, Excel" });
            var job = CreateTestJob("Data Analyst", "DataCorp", "Анализ данных", "SQL, Python, Excel");

            var result = ResumeAnalyzer.Analyze(resume, job);

            Assert.AreEqual(100, result.MatchPercentage);
        }

        [TestMethod]
        public void Analyze_EmptyResumeSkills_ShouldReturn0Percent()
        {
            var resume = CreateTestResume("Дмитрий", "dmitry@test.com", "Начинающий", new List<string>());
            var job = CreateTestJob("Junior Dev", "StartUp", "Вход в профессию", "C#, Git");

            var result = ResumeAnalyzer.Analyze(resume, job);

            Assert.AreEqual(0, result.MatchPercentage);
            Assert.AreEqual(2, result.MissingSkills.Count);
        }

        [TestMethod]
        public void Analyze_LowPercentage_ShouldGenerateLowMatchRecommendation()
        {
            var resume = CreateTestResume("Елена", "elena@test.com", "Цель", new List<string> { "C#" });
            var job = CreateTestJob("Senior Dev", "BigCorp", "Сложная разработка", "C#, Java, Python, Docker, Kubernetes, AWS");

            var result = ResumeAnalyzer.Analyze(resume, job);

            Assert.IsTrue(result.MatchPercentage < 40);
            Assert.IsTrue(result.Recommendations.Contains("Соответствие низкое."));
        }

        [TestMethod]
        public void Analyze_NoWorkExperience_ShouldGenerateExperienceRecommendation()
        {
            var resume = CreateTestResume("Алексей", "alex@test.com", "Ищу работу",
                new List<string> { "C#", "Git" }, new List<WorkExperience>(), new List<Education>());
            var job = CreateTestJob("Dev", "Company", "Описание", "C#, Git");

            var result = ResumeAnalyzer.Analyze(resume, job);

            Assert.IsTrue(result.Recommendations.Contains("Добавьте данные в раздел 'Опыт работы' - это повысит шансы на трудоустройство."));
        }

        [TestMethod]
        public void Analyze_NoEducation_ShouldGenerateEducationRecommendation()
        {
            var resume = CreateTestResume("Наталья", "nataly@test.com", "Цель",
                new List<string> { "C#" }, new List<WorkExperience> { CreateTestWorkExperience() }, new List<Education>());
            var job = CreateTestJob("Dev", "Company", "Описание", "C#");

            var result = ResumeAnalyzer.Analyze(resume, job);

            Assert.IsTrue(result.Recommendations.Contains("Добавьте данные в раздел 'Образование' - укажите учебные заведения и квалификации."));
        }

        [TestMethod]
        public void Analyze_ShortObjective_ShouldGenerateObjectiveRecommendation()
        {
            var resume = CreateTestResume("Сергей", "sergey@test.com", "Цель",
                new List<string> { "C#" });
            var job = CreateTestJob("Dev", "Company", "Описание", "C#");

            var result = ResumeAnalyzer.Analyze(resume, job);

            Assert.IsTrue(result.Recommendations.Contains("Дополните раздел 'Цель' конкретными формулировками."));
        }

        [TestMethod]
        public void SaveReport_ValidPath_ShouldCreateFile()
        {
            string testPath = Path.Combine(Path.GetTempPath(), "test_report.txt");
            string content = "Тестовый отчёт";

            ResumeAnalyzer.SaveReport(testPath, content);

            Assert.IsTrue(File.Exists(testPath));
            Assert.AreEqual(content, File.ReadAllText(testPath, Encoding.UTF8));

            File.Delete(testPath);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SaveReport_EmptyPath_ShouldThrowException()
        {
            ResumeAnalyzer.SaveReport("", "Content");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SaveReport_EmptyContent_ShouldThrowException()
        {
            ResumeAnalyzer.SaveReport("path.txt", "");
        }

        [TestMethod]
        public void Analyze_GeneratedReport_ShouldContainCandidateAndJobData()
        {
            var resume = CreateTestResume("Иван Иванов", "ivan@test.com", "Разработчик C#", new List<string> { "C#" });
            var job = CreateTestJob("Backend Developer", "TechCorp", "Разработка", "C#");

            var result = ResumeAnalyzer.Analyze(resume, job);

            Assert.IsTrue(result.GeneratedReport.Contains("Иван Иванов"));
            Assert.IsTrue(result.GeneratedReport.Contains("ivan@test.com"));
            Assert.IsTrue(result.GeneratedReport.Contains("Backend Developer"));
            Assert.IsTrue(result.GeneratedReport.Contains("TechCorp"));
            Assert.IsTrue(result.GeneratedReport.Contains("100%"));
        }
    }
}