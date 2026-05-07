using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

public class SearchJobListingForm : Form
{
    public List<JobListing> JobListings { get; set; }
    public ListBox JobListingsListBox { get; private set; }
    private TextBox searchTextBox;
    private Button searchButton;

    public SearchJobListingForm()
    {
        Text = "Поиск вакансий";
        Width = 450;
        Height = 450;
        StartPosition = FormStartPosition.CenterScreen;

        var searchLabel = new Label { Text = "Поиск по ключевому слову:", Location = new Point(10, 10), AutoSize = true };
        searchTextBox = new TextBox { Location = new Point(10, 30), Width = 300 };

        searchButton = new Button { Text = "Поиск", Location = new Point(320, 28), Width = 80 };
        searchButton.Click += (sender, e) => PerformSearch();

        JobListingsListBox = new ListBox { Location = new Point(10, 60), Width = 400, Height = 260 };

        var okButton = new Button { Text = "OK", Location = new Point(10, 330), Width = 80 };
        var cancelButton = new Button { Text = "Отмена", Location = new Point(100, 330), Width = 80 };

        okButton.Click += (sender, e) =>
        {
            DialogResult = DialogResult.OK;
            Close();
        };

        cancelButton.Click += (sender, e) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        Controls.Add(searchLabel);
        Controls.Add(searchTextBox);
        Controls.Add(searchButton);
        Controls.Add(JobListingsListBox);
        Controls.Add(okButton);
        Controls.Add(cancelButton);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (JobListings != null)
        {
            foreach (var jobListing in JobListings)
            {
                JobListingsListBox.Items.Add(jobListing.ToString());
            }
        }
    }

    private void PerformSearch()
    {
        string keyword = searchTextBox.Text.Trim().ToLower();

        JobListingsListBox.Items.Clear();

        if (string.IsNullOrEmpty(keyword))
        {
            if (JobListings != null)
            {
                foreach (var jobListing in JobListings)
                {
                    JobListingsListBox.Items.Add(jobListing.ToString());
                }
            }
            return;
        }

        if (JobListings != null)
        {
            var filteredJobs = JobListings.Where(job =>
                job.JobTitle.ToLower().Contains(keyword) ||
                job.Company.ToLower().Contains(keyword) ||
                job.Description.ToLower().Contains(keyword) ||
                job.Requirements.ToLower().Contains(keyword)
            ).ToList();

            if (filteredJobs.Count == 0)
            {
                JobListingsListBox.Items.Add("Вакансии не найдены");
                MessageBox.Show($"Вакансии по запросу \"{keyword}\" не найдены.");
            }
            else
            {
                foreach (var jobListing in filteredJobs)
                {
                    JobListingsListBox.Items.Add(jobListing.ToString());
                }
                MessageBox.Show($"Найдено вакансий: {filteredJobs.Count}");
            }
        }
    }
}