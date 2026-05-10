using System;
using System.Windows.Forms;
using System.Drawing;

public class AddJobListingForm : Form
{
    public string JobTitle { get; private set; }
    public string Company { get; private set; }
    public string Description { get; private set; }
    public string Requirements { get; private set; }
    private const int MAX_JOB_TITLE_LENGTH = 50;
    private const int MAX_COMPANY_LENGTH = 50;
    private const int MAX_DESCRIPTION_LENGTH = 500;
    private const int MAX_REQUIREMENTS_LENGTH = 500;

    public AddJobListingForm()
    {
        Text = "Добавить вакансию";
        Width = 500;
        Height = 420;
        StartPosition = FormStartPosition.CenterScreen;

        var jobTitleLabel = new Label
        {
            Text = "Название вакансии:",
            Location = new Point(10, 10),
            AutoSize = true
        };
        var jobTitleTextBox = new TextBox
        {
            Location = new Point(10, 30),
            Width = 440,
            MaxLength = MAX_JOB_TITLE_LENGTH
        };
        var companyLabel = new Label
        {
            Text = "Компания:",
            Location = new Point(10, 60),
            AutoSize = true
        };
        var companyTextBox = new TextBox
        {
            Location = new Point(10, 80),
            Width = 440,
            MaxLength = MAX_COMPANY_LENGTH
        };
        var descriptionLabel = new Label
        {
            Text = "Описание:",
            Location = new Point(10, 110),
            AutoSize = true
        };
        var descriptionPanel = new Panel
        {
            Location = new Point(10, 130),
            Width = 460,
            Height = 80
        };
        var descriptionTextBox = new TextBox
        {
            Location = new Point(0, 0),
            Width = 443,
            Height = 80,
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            MaxLength = MAX_DESCRIPTION_LENGTH,
            WordWrap = true
        };
        descriptionPanel.Controls.Add(descriptionTextBox);

        var requirementsLabel = new Label
        {
            Text = "Требования:",
            Location = new Point(10, 220),
            AutoSize = true
        };
        var requirementsPanel = new Panel
        {
            Location = new Point(10, 240),
            Width = 460,
            Height = 80
        };

        var requirementsTextBox = new TextBox
        {
            Location = new Point(0, 0),
            Width = 443,
            Height = 80,
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            MaxLength = MAX_REQUIREMENTS_LENGTH,
            WordWrap = true
        };
        requirementsPanel.Controls.Add(requirementsTextBox);

        var okButton = new Button
        {
            Text = "OK",
            Location = new Point(10, 340),
            Width = 100
        };
        var cancelButton = new Button
        {
            Text = "Отмена",
            Location = new Point(130, 340),
            Width = 100
        };

        okButton.Click += (sender, e) =>
        {
            if (string.IsNullOrWhiteSpace(jobTitleTextBox.Text))
            {
                MessageBox.Show("Название вакансии не может быть пустым.");
                return;
            }

            if (string.IsNullOrWhiteSpace(companyTextBox.Text))
            {
                MessageBox.Show("Компания не может быть пустой.");
                return;
            }

            if (string.IsNullOrWhiteSpace(descriptionTextBox.Text))
            {
                MessageBox.Show("Описание не может быть пустым.");
                return;
            }

            if (string.IsNullOrWhiteSpace(requirementsTextBox.Text))
            {
                MessageBox.Show("Требования не могут быть пустыми.");
                return;
            }

            JobTitle = jobTitleTextBox.Text;
            Company = companyTextBox.Text;
            Description = descriptionTextBox.Text;
            Requirements = requirementsTextBox.Text;
            DialogResult = DialogResult.OK;
            Close();
        };

        cancelButton.Click += (sender, e) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        Controls.Add(jobTitleLabel);
        Controls.Add(jobTitleTextBox);
        Controls.Add(companyLabel);
        Controls.Add(companyTextBox);
        Controls.Add(descriptionLabel);
        Controls.Add(descriptionPanel);
        Controls.Add(requirementsLabel);
        Controls.Add(requirementsPanel);
        Controls.Add(okButton);
        Controls.Add(cancelButton);
    }
}