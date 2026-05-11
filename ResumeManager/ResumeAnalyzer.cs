using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ResumeManager
{
    public class AnalysisResult
    {
        public int MatchPercentage { get; set; }
        public List<string> MissingSkills { get; set; } = new List<string>();
        public List<string> Recommendations { get; set; } = new List<string>();
        public string GeneratedReport { get; set; } = string.Empty;
    }

    public static class ResumeAnalyzer
    {
        public static AnalysisResult Analyze(Resume resume, JobListing job)
        {
            char[] separators = new[] { ',', ';', '\n', '\r' };

            var resumeSkillsForComparison = new HashSet<string>(
                resume.Skills
                    .SelectMany(s => s.Split(separators, StringSplitOptions.RemoveEmptyEntries))
                    .Select(s => s.Trim().ToLower())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Distinct()
            );

            var jobReqsOriginal = (job.Requirements ?? "")
                .Split(separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Trim())
                .Where(r => !string.IsNullOrEmpty(r))
                .Distinct()
                .ToList();

            var jobReqsForComparison = jobReqsOriginal
                .Select(r => r.ToLower())
                .ToList();

            var missingSkillsOriginal = new List<string>();
            var missingSkillsForReport = new List<string>();

            for (int i = 0; i < jobReqsOriginal.Count; i++)
            {
                var reqOriginal = jobReqsOriginal[i];
                var reqLower = jobReqsForComparison[i];

                var reqKeywords = reqLower.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                bool found = false;

                foreach (var resumeSkill in resumeSkillsForComparison)
                {
                    if (reqKeywords.Any(keyword => resumeSkill.Contains(keyword)))
                    {
                        found = true;
                        break;
                    }
                    if (reqLower.Contains(resumeSkill))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    missingSkillsOriginal.Add(reqOriginal);
                    missingSkillsForReport.Add(reqOriginal);
                }
            }

            int total = jobReqsOriginal.Count;
            int matched = total - missingSkillsOriginal.Count;
            int percentage = total > 0 ? (matched * 100) / total : 0;

            var result = new AnalysisResult
            {
                MatchPercentage = percentage,
                MissingSkills = missingSkillsForReport
            };

            if (percentage < 40)
                result.Recommendations.Add("Соответствие низкое.");
            if (missingSkillsForReport.Any())
                result.Recommendations.Add("Добавьте в резюме недостающие навыки: " + string.Join(", ", missingSkillsForReport));
            if (string.IsNullOrWhiteSpace(resume.Objective) || resume.Objective.Length < 20)
                result.Recommendations.Add("Дополните раздел 'Цель' конкретными формулировками.");

            if (resume.WorkExperiences == null || resume.WorkExperiences.Count == 0)
                result.Recommendations.Add("Добавьте данные в раздел 'Опыт работы' - это повысит шансы на трудоустройство.");
            if (resume.Educations == null || resume.Educations.Count == 0)
                result.Recommendations.Add("Добавьте данные в раздел 'Образование' - укажите учебные заведения и квалификации.");

            if (!result.Recommendations.Any())
                result.Recommendations.Add("Резюме полностью соответствует требованиям. Рекомендаций нет.");

            result.GeneratedReport = "Отчёт анализа соответствия" + "\n\n" +
                                     "ДАННЫЕ КАНДИДАТА:" + "\n" +
                                     "Имя: " + (string.IsNullOrWhiteSpace(resume.Name) ? "Не указано" : resume.Name) + "\n" +
                                     "Контакты: " + (string.IsNullOrWhiteSpace(resume.ContactInfo) ? "Не указаны" : resume.ContactInfo) + "\n" +
                                     "Цель: " + (string.IsNullOrWhiteSpace(resume.Objective) ? "Не указана" : resume.Objective) + "\n\n" +

                                     "ВАКАНСИЯ:" + "\n" +
                                     "Название: " + job.JobTitle + "\n" +
                                     "Компания: " + job.Company + "\n\n" +

                                     "РЕЗУЛЬТАТЫ АНАЛИЗА:" + "\n" +
                                     "Дата анализа: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "\n" +
                                     "Процент соответствия: " + percentage + "%" + "\n" +
                                     "Отсутствующие навыки: " + (missingSkillsForReport.Any() ? string.Join(", ", missingSkillsForReport) : "Нет") + "\n\n" +

                                     "РЕКОМЕНДАЦИИ:" + "\n" +
                                     string.Join("\n", result.Recommendations.Select(r => "• " + r));

            return result;
        }

        public static void SaveReport(string filePath, string reportContent)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Путь к файлу не может быть пустым.");

            if (string.IsNullOrWhiteSpace(reportContent))
                throw new ArgumentException("Содержимое отчёта не может быть пустым.");

            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(filePath, reportContent, Encoding.UTF8);
        }
    }
}