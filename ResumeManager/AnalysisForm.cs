using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

namespace ResumeManager
{
    public class ResumeListItem
    {
        public Resume Resume { get; }
        public string DisplayText { get; }

        public ResumeListItem(int index, Resume resume)
        {
            Resume = resume;
            DisplayText = $"[{index}] {resume.Name}";
        }

        public override string ToString() => DisplayText;
    }

    public class JobListItem
    {
        public JobListing Job { get; }
        public string DisplayText { get; }

        public JobListItem(int index, JobListing job)
        {
            Job = job;
            DisplayText = $"[{index}] {job.JobTitle}";
        }

        public override string ToString() => DisplayText;
    }

    public partial class AnalysisForm : Form
    {
        private ListBox listResumes;
        private ListBox listJobs;
        private Button buttonAnalyze;
        private Button buttonSave;
        private Label labelPercent;
        private TextBox textRecommendations;
        private ToolTip toolTip;
        private Label labelResumes;
        private Label labelJobs;

        private readonly List<Resume> _resumes;
        private readonly List<JobListing> _jobs;
        private AnalysisResult _currentResult;

        private int _lastResumeIndex = -1;
        private int _lastJobIndex = -1;

        private const int MAX_TOOLTIP_WIDTH = 30;

        public AnalysisForm(List<Resume> resumes, List<JobListing> jobs)
        {
            _resumes = resumes ?? new List<Resume>();
            _jobs = jobs ?? new List<JobListing>();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Анализ соответствия резюме вакансии";
            this.Size = new Size(630, 465);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            toolTip = new ToolTip
            {
                AutoPopDelay = 10000,
                InitialDelay = 50,
                ReshowDelay = 20,
                ShowAlways = true,
                IsBalloon = false,
                UseFading = true
            };

            labelResumes = new Label();
            labelResumes.Text = "Список резюме:";
            labelResumes.Location = new Point(10, 10);
            labelResumes.Size = new Size(290, 20);
            labelResumes.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            labelResumes.ForeColor = Color.DarkBlue;

            listResumes = new ListBox();
            listResumes.Location = new Point(10, 32);
            listResumes.Size = new Size(290, 180);
            listResumes.DisplayMember = "DisplayText";
            listResumes.DataSource = CreateResumeListItems(_resumes);
            listResumes.MouseMove += ListResumes_MouseMove;

            labelJobs = new Label();
            labelJobs.Text = "Список вакансий:";
            labelJobs.Location = new Point(310, 10);
            labelJobs.Size = new Size(290, 20);
            labelJobs.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            labelJobs.ForeColor = Color.DarkGreen;

            listJobs = new ListBox();
            listJobs.Location = new Point(310, 32);
            listJobs.Size = new Size(290, 180);
            listJobs.DisplayMember = "DisplayText";
            listJobs.DataSource = CreateJobListItems(_jobs);
            listJobs.MouseMove += ListJobs_MouseMove;

            buttonAnalyze = new Button();
            buttonAnalyze.Text = "Анализировать";
            buttonAnalyze.Location = new Point(250, 225);
            buttonAnalyze.Size = new Size(120, 30);
            buttonAnalyze.Click += analysisButton_Click;

            buttonSave = new Button();
            buttonSave.Text = "Сохранить отчёт";
            buttonSave.Location = new Point(460, 225);
            buttonSave.Size = new Size(140, 30);
            buttonSave.Click += saveButton_Click;

            labelPercent = new Label();
            labelPercent.Text = "Соответствие: --%";
            labelPercent.Location = new Point(10, 270);
            labelPercent.AutoSize = true;
            labelPercent.Font = new Font("Segoe UI", 11, FontStyle.Bold);

            textRecommendations = new TextBox();
            textRecommendations.Location = new Point(10, 300);
            textRecommendations.Size = new Size(580, 120);
            textRecommendations.Multiline = true;
            textRecommendations.ReadOnly = true;
            textRecommendations.ScrollBars = ScrollBars.Vertical;
            textRecommendations.Font = new Font("Segoe UI", 9);

            this.Controls.AddRange(new Control[]
            {
                labelResumes, listResumes, labelJobs, listJobs,
                buttonAnalyze, buttonSave, labelPercent, textRecommendations
            });
        }

        private string WrapText(string text, int maxWidth)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); 
            var lines = new List<string>();
            var currentLine = "";


            foreach (var word in words)
            {
                string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                if (testLine.Length > maxWidth && !string.IsNullOrEmpty(currentLine))
                {
                    lines.Add(currentLine);
                    currentLine = word;
                }
                else
                {
                    currentLine = testLine;
                }
            }
            if (!string.IsNullOrEmpty(currentLine))
                lines.Add(currentLine);

            return string.Join(Environment.NewLine, lines);
        }

        private List<ResumeListItem> CreateResumeListItems(List<Resume> resumes)
        {
            var result = new List<ResumeListItem>();
            for (int i = 0; i < resumes.Count; i++)
                result.Add(new ResumeListItem(i + 1, resumes[i]));
            return result;
        }

        private List<JobListItem> CreateJobListItems(List<JobListing> jobs)
        {
            var result = new List<JobListItem>();
            for (int i = 0; i < jobs.Count; i++)
                result.Add(new JobListItem(i + 1, jobs[i]));
            return result;
        }

        private void ListResumes_MouseMove(object sender, MouseEventArgs e)
        {
            int index = listResumes.IndexFromPoint(e.Location);

            if (index == _lastResumeIndex) return;
            _lastResumeIndex = index;

            if (index >= 0 && index < _resumes.Count)
            {
                var resume = _resumes[index];

                string workExpText = " не указан";
                if (resume.WorkExperiences != null && resume.WorkExperiences.Count > 0)
                {
                    var lines = new List<string>();
                    foreach (var w in resume.WorkExperiences)
                    {
                        lines.Add($"  - {w.Position} в {w.Company} ({w.Period})");
                    }
                    workExpText = "\n" + string.Join("\n", lines);
                }

                string educationText = " не указано";
                if (resume.Educations != null && resume.Educations.Count > 0)
                {
                    var lines = resume.Educations.Select(edu => $"  - {edu.Degree} в {edu.Institution} ({edu.Period})");
                    educationText = "\n" + string.Join("\n", lines);
                }

                string tip = $"Имя: {resume.Name}\n" +
                            $"\nКонтакты: {WrapText(resume.ContactInfo, MAX_TOOLTIP_WIDTH)}\n" +
                            $"\nЦель: {WrapText(resume.Objective, MAX_TOOLTIP_WIDTH)}\n" +
                            $"\nНавыки: {(resume.Skills.Any() ? WrapText(string.Join(", ", resume.Skills), MAX_TOOLTIP_WIDTH) : "не указаны")}\n" +
                            $"\nОпыт работы:{workExpText}\n" +
                            $"\nОбразование:{educationText}";
                toolTip.SetToolTip(listResumes, tip);
            }
            else
            {
                toolTip.SetToolTip(listResumes, string.Empty);
            }
        }

        private void ListJobs_MouseMove(object sender, MouseEventArgs e)
        {
            int index = listJobs.IndexFromPoint(e.Location);

            if (index == _lastJobIndex) return;
            _lastJobIndex = index;

            if (index >= 0 && index < _jobs.Count)
            {
                var job = _jobs[index];
                string tip = $"Вакансия: {job.JobTitle}\n" +
                            $"Компания: {job.Company}\n" +
                            $"Описание: {WrapText(job.Description, MAX_TOOLTIP_WIDTH)}\n" +
                            $"Требования: {WrapText(job.Requirements, MAX_TOOLTIP_WIDTH)}";

                toolTip.SetToolTip(listJobs, tip);
            }
            else
            {
                toolTip.SetToolTip(listJobs, string.Empty);
            }
        }

        private void analysisButton_Click(object sender, EventArgs e)
        {
            var resumeItem = listResumes.SelectedItem as ResumeListItem;
            var jobItem = listJobs.SelectedItem as JobListItem;

            var resume = resumeItem.Resume;
            var job = jobItem.Job;

            _currentResult = ResumeAnalyzer.Analyze(resume, job);

            labelPercent.Text = "Соответствие: " + _currentResult.MatchPercentage + "%";
            textRecommendations.Text = _currentResult.Recommendations.Count > 0
                ? string.Join(Environment.NewLine, _currentResult.Recommendations)
                : "Рекомендации отсутствуют. Резюме полностью соответствует требованиям.";
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (_currentResult == null)
            {
                MessageBox.Show("Сначала выполните анализ.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Текстовые файлы (*.txt)|*.txt";
                sfd.DefaultExt = "txt";
                sfd.Title = "Сохранить отчёт анализа";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ResumeAnalyzer.SaveReport(sfd.FileName, _currentResult.GeneratedReport);
                        MessageBox.Show("Отчёт успешно сохранён.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка сохранения: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}