using System;
using System.Windows.Forms;
using System.Drawing;

public class AddWorkExperienceForm : Form
{
    public string Position { get; private set; }
    public string Company { get; private set; }
    public string Period { get; private set; }
    public string Description { get; private set; }

    public AddWorkExperienceForm()
    {
        Text = "Добавить опыт работы";
        Width = 300;
        Height = 250;
        StartPosition = FormStartPosition.CenterScreen;

        var positionLabel = new Label { Text = "Должность:", Location = new Point(10, 10) };
        var positionTextBox = new TextBox { Location = new Point(10, 30), Width = 260 };
        var companyLabel = new Label { Text = "Компания:", Location = new Point(10, 50) };
        var companyTextBox = new TextBox { Location = new Point(10, 70), Width = 260 };
        var periodLabel = new Label { Text = "Период:", Location = new Point(10, 90) };
        var periodTextBox = new TextBox { Location = new Point(10, 110), Width = 260 };
        var descriptionLabel = new Label { Text = "Описание:", Location = new Point(10, 130) };
        var descriptionTextBox = new TextBox { Location = new Point(10, 150), Width = 260 };
        var okButton = new Button { Text = "OK", Location = new Point(10, 180), Width = 80 };
        var cancelButton = new Button { Text = "Отмена", Location = new Point(100, 180), Width = 80 };

        okButton.Click += (sender, e) =>
        {
            if (string.IsNullOrWhiteSpace(positionTextBox.Text))
            {
                MessageBox.Show("Должность не может быть пустой.");
                return;
            }
            Position = positionTextBox.Text;
            Company = companyTextBox.Text;
            Period = periodTextBox.Text;
            Description = descriptionTextBox.Text;
            DialogResult = DialogResult.OK;
            Close();
        };

        cancelButton.Click += (sender, e) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        Controls.Add(positionLabel);
        Controls.Add(positionTextBox);
        Controls.Add(companyLabel);
        Controls.Add(companyTextBox);
        Controls.Add(periodLabel);
        Controls.Add(periodTextBox);
        Controls.Add(descriptionLabel);
        Controls.Add(descriptionTextBox);
        Controls.Add(okButton);
        Controls.Add(cancelButton);
    }
}