using System;
using System.Windows.Forms;
using System.Drawing;

public class JobSearchForm : Form
{
    private JobSearchManager jobSearchManager;

    public JobSearchForm()
    {
        Text = "Управление резюме и поиском работы";
        Width = 440;
        Height = 250;
        StartPosition = FormStartPosition.CenterScreen;
        jobSearchManager = new JobSearchManager();
        CreateControls();
        this.Shown += (s, e) =>
        {
            this.ActiveControl = null;
            this.Focus();
        };
    }

    private void CreateControls()
    {
        var createResumeButton = new Button
        {
            Location = new Point(30, 20),
            Text = "Создать резюме",
            Size = new Size(120, 25)
        };
        createResumeButton.Click += (sender, e) => jobSearchManager.CreateResume();

        var addSkillButton = new Button
        {
            Location = new Point(160, 20),
            Text = "Добавить навык",
            Size = new Size(100, 25)
        };
        addSkillButton.Click += (sender, e) => jobSearchManager.AddSkillToResume();

        var addWorkExperienceButton = new Button
        {
            Location = new Point(270, 20),
            Text = "Добавить опыт",
            Size = new Size(120, 25)
        };
        addWorkExperienceButton.Click += (sender, e) => jobSearchManager.AddWorkExperienceToResume();

        var addEducationButton = new Button
        {
            Location = new Point(30, 50),
            Text = "Добавить образование",
            Size = new Size(120, 35)
        };
        addEducationButton.Click += (sender, e) => jobSearchManager.AddEducationToResume();

        var displayResumeButton = new Button
        {
            Location = new Point(160, 50),
            Text = "Показать резюме",
            Size = new Size(100, 35)
        };
        displayResumeButton.Click += (sender, e) => jobSearchManager.DisplayResume();

        var addJobListingButton = new Button
        {
            Location = new Point(270, 50),
            Text = "Добавить вакансию",
            Size = new Size(120, 35)
        };
        addJobListingButton.Click += (sender, e) => jobSearchManager.AddJobListing();

        var searchJobListingsButton = new Button
        {
            Location = new Point(30, 88),
            Text = "Поиск вакансий",
            Size = new Size(120, 25)
        };
        searchJobListingsButton.Click += (sender, e) => jobSearchManager.SearchJobListings();

        var analysisButton = new Button
        {
            Text = "Анализ резюме",
            Location = new Point(160, 88),
            Size = new Size(100, 25),
        };
        analysisButton.Click += (sender, e) => jobSearchManager.AnalysisForm();
        this.Controls.Add(analysisButton);

        Controls.Add(createResumeButton);
        Controls.Add(addSkillButton);
        Controls.Add(addWorkExperienceButton);
        Controls.Add(addEducationButton);
        Controls.Add(displayResumeButton);
        Controls.Add(addJobListingButton);
        Controls.Add(searchJobListingsButton);
    }
}