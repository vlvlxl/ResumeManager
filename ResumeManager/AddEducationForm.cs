using System;
using System.Windows.Forms;
using System.Drawing;

public class AddEducationForm : Form
{
    public string Institution { get; private set; }
    public string Degree { get; private set; }
    public string Period { get; private set; }
    private const int MAX_INSTITUTION_LENGTH = 50;
    private const int MAX_DEGREE_LENGTH = 50;
    private const int MAX_PERIOD_LENGTH = 50;

    public AddEducationForm()
    {
        Text = "Добавить образование";
        Width = 500;       
        Height = 240;       
        StartPosition = FormStartPosition.CenterScreen;

        var institutionLabel = new Label
        {
            Text = "Учебное заведение:",
            Location = new Point(10, 10),
            AutoSize = true
        };
        var institutionTextBox = new TextBox
        {
            Location = new Point(10, 30),
            Width = 460,
            MaxLength = MAX_INSTITUTION_LENGTH
        };
        var degreeLabel = new Label
        {
            Text = "Степень:",
            Location = new Point(10, 60),
            AutoSize = true
        };
        var degreeTextBox = new TextBox
        {
            Location = new Point(10, 80),
            Width = 460,
            MaxLength = MAX_DEGREE_LENGTH
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
        var okButton = new Button
        {
            Text = "OK",
            Location = new Point(10, 170),  
            Width = 100
        };
        var cancelButton = new Button
        {
            Text = "Отмена",
            Location = new Point(130, 170),  
            Width = 100
        };

        okButton.Click += (sender, e) =>
        {
            if (string.IsNullOrWhiteSpace(institutionTextBox.Text))
            {
                MessageBox.Show("Учебное заведение не может быть пустым.");
                return;
            }

            if (string.IsNullOrWhiteSpace(degreeTextBox.Text))
            {
                MessageBox.Show("Степень не может быть пустой.");
                return;
            }

            if (string.IsNullOrWhiteSpace(periodTextBox.Text))
            {
                MessageBox.Show("Период не может быть пустым.");
                return;
            }

            Institution = institutionTextBox.Text;
            Degree = degreeTextBox.Text;
            Period = periodTextBox.Text;
            DialogResult = DialogResult.OK;
            Close();
        };

        cancelButton.Click += (sender, e) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        Controls.Add(institutionLabel);
        Controls.Add(institutionTextBox);
        Controls.Add(degreeLabel);
        Controls.Add(degreeTextBox);
        Controls.Add(periodLabel);
        Controls.Add(periodTextBox);
        Controls.Add(okButton);
        Controls.Add(cancelButton);
    }
}