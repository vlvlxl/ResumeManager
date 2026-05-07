using System;
using System.Windows.Forms;
using System.Drawing;

public class AddJobListingForm : Form
{
    public string JobTitle { get; private set; }
    public string Company { get; private set; }
    public string Description { get; private set; }
    public string Requirements { get; private set; }

    public AddJobListingForm()
    {
        Text = "Добавить вакансию";
        Width = 300;
        Height = 300;
        StartPosition = FormStartPosition.CenterScreen;

        var jobTitleLabel = new Label { Text = "Название вакансии:", Location = new Point(10, 10) };
        var jobTitleTextBox = new TextBox { Location = new Point(10, 30), Width = 260 };
        var companyLabel = new Label { Text = "Компания:", Location = new Point(10, 50) };
        var companyTextBox = new TextBox { Location = new Point(10, 70), Width = 260 };
        var descriptionLabel = new Label { Text = "Описание:", Location = new Point(10, 90) };
        var descriptionTextBox = new TextBox { Location = new Point(10, 110), Width = 260 };
        var requirementsLabel = new Label { Text = "Требования:", Location = new Point(10, 130) };
        var requirementsTextBox = new TextBox { Location = new Point(10, 150), Width = 260 };
        var okButton = new Button { Text = "OK", Location = new Point(10, 240), Width = 80 };
        var cancelButton = new Button { Text = "Отмена", Location = new Point(100, 240), Width = 80 };

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
        Controls.Add(descriptionTextBox);
        Controls.Add(requirementsLabel);
        Controls.Add(requirementsTextBox);
        Controls.Add(okButton);
        Controls.Add(cancelButton);
    }
}