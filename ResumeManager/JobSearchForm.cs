using System;
using System.Windows.Forms;
using System.Drawing;

public class JobSearchForm : Form
{
    private JobSearchManager jobSearchManager;

    public JobSearchForm()
    {
        Text = "Управление резюме и поиском работы";
        Width = 400;
        Height = 250;
        StartPosition = FormStartPosition.CenterScreen;
        jobSearchManager = new JobSearchManager();
        CreateControls();
    }

    private void CreateControls()
    {
        var createResumeButton = new Button
        {
            Location = new Point(10, 20),
            Text = "Создать резюме",
            Size = new Size(120, 25)
        };
        createResumeButton.Click += (sender, e) => jobSearchManager.CreateResume();

        var addSkillButton = new Button
        {
            Location = new Point(140, 20),
            Text = "Добавить навык",
            Size = new Size(100, 25)
        };
        addSkillButton.Click += (sender, e) => jobSearchManager.AddSkillToResume();

        var addWorkExperienceButton = new Button
        {
            Location = new Point(250, 20),
            Text = "Добавить опыт",
            Size = new Size(120, 25)
        };
        addWorkExperienceButton.Click += (sender, e) => jobSearchManager.AddWorkExperienceToResume();

        var addEducationButton = new Button
        {
            Location = new Point(10, 50),
            Text = "Добавить образование",
            Size = new Size(120, 25)
        };
        addEducationButton.Click += (sender, e) => jobSearchManager.AddEducationToResume();

        var displayResumeButton = new Button
        {
            Location = new Point(140, 50),
            Text = "Показать резюме",
            Size = new Size(100, 25)
        };
        displayResumeButton.Click += (sender, e) => jobSearchManager.DisplayResume();

        var addJobListingButton = new Button
        {
            Location = new Point(250, 50),
            Text = "Добавить вакансию",
            Size = new Size(100, 25)
        };
        addJobListingButton.Click += (sender, e) => jobSearchManager.AddJobListing();

        var searchJobListingsButton = new Button
        {
            Location = new Point(10, 80),
            Text = "Поиск вакансий",
            Size = new Size(120, 25)
        };
        searchJobListingsButton.Click += (sender, e) => jobSearchManager.SearchJobListings();

        Controls.Add(createResumeButton);
        Controls.Add(addSkillButton);
        Controls.Add(addWorkExperienceButton);
        Controls.Add(addEducationButton);
        Controls.Add(displayResumeButton);
        Controls.Add(addJobListingButton);
        Controls.Add(searchJobListingsButton);
    }
}