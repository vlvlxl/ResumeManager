using System;
using System.Windows.Forms;
using System.Drawing;

public class AddWorkExperienceForm : Form
{
    public string Position { get; private set; }
    public string Company { get; private set; }
    public string Period { get; private set; }
    public string Description { get; private set; }
    private const int MAX_POSITION_LENGTH = 20;
    private const int MAX_COMPANY_LENGTH = 30;
    private const int MAX_PERIOD_LENGTH = 30;
    private const int MAX_DESCRIPTION_LENGTH = 500;

    public AddWorkExperienceForm()
    {
        Text = "Добавить опыт работы";
        Width = 500;
        Height = 330;      
        StartPosition = FormStartPosition.CenterScreen;

        var positionLabel = new Label
        {
            Text = "Должность:",
            Location = new Point(10, 10),
            AutoSize = true,
        };
        var positionTextBox = new TextBox
        {
            Location = new Point(10, 30),
            Width = 460,
            MaxLength = MAX_POSITION_LENGTH
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
            Width = 460,
            MaxLength = MAX_COMPANY_LENGTH
        };
        var periodLabel = new Label
        {
            Text = "Период:",
            Location = new Point(10, 110),
            AutoSize = true
        };
        var periodTextBox = new TextBox
        {
            Location = new Point(10, 130),
            Width = 460,
            MaxLength = MAX_PERIOD_LENGTH
        };
        var descriptionLabel = new Label
        {
            Text = "Описание:",
            Location = new Point(10, 160),
            AutoSize = true
        };
        var descriptionTextBox = new TextBox
        {
            Location = new Point(10, 180),
            Width = 460,   
            Height = 60,
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            MaxLength = MAX_DESCRIPTION_LENGTH
        };
        var okButton = new Button
        {
            Text = "OK",
            Location = new Point(10, 260), 
            Width = 100
        };
        var cancelButton = new Button
        {
            Text = "Отмена",
            Location = new Point(130, 260), 
            Width = 100
        };

        okButton.Click += (sender, e) =>
        {
            if (string.IsNullOrWhiteSpace(positionTextBox.Text))
            {
                MessageBox.Show("Должность не может быть пустой.");
                return;
            }

            if (string.IsNullOrWhiteSpace(companyTextBox.Text))
            {
                MessageBox.Show("Компания не может быть пустой.");
                return;
            }

            if (string.IsNullOrWhiteSpace(periodTextBox.Text))
            {
                MessageBox.Show("Период не может быть пустым.");
                return;
            }

            if (string.IsNullOrWhiteSpace(descriptionTextBox.Text))
            {
                MessageBox.Show("Описание не может быть пустым.");
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