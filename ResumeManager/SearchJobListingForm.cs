using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

public class SearchJobListingForm : Form
{
    public List<JobListing> JobListings { get; set; }
    public ListBox JobListingsListBox { get; private set; }

    public SearchJobListingForm()
    {
        Text = "Поиск вакансий";
        Width = 400;
        Height = 300;
        StartPosition = FormStartPosition.CenterScreen;

        JobListingsListBox = new ListBox { Location = new Point(10, 10), Width = 360, Height = 260 };
        var okButton = new Button { Text = "OK", Location = new Point(10, 280), Width = 80 };
        var cancelButton = new Button { Text = "Отмена", Location = new Point(100, 280), Width = 80 };

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
}