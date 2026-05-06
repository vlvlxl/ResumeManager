using System;
using System.Windows.Forms;
using System.Drawing;

public class AddEducationForm : Form
{
    public string Institution { get; private set; }
    public string Degree { get; private set; }
    public string Period { get; private set; }

    public AddEducationForm()
    {
        Text = "Добавить образование";
        Width = 300;
        Height = 200;
        StartPosition = FormStartPosition.CenterScreen;

        var institutionLabel = new Label { Text = "Учебное заведение:", Location = new Point(10, 10) };
        var institutionTextBox = new TextBox { Location = new Point(10, 30), Width = 260 };
        var degreeLabel = new Label { Text = "Степень:", Location = new Point(10, 50) };
        var degreeTextBox = new TextBox { Location = new Point(10, 70), Width = 260 };
        var periodLabel = new Label { Text = "Период:", Location = new Point(10, 90) };
        var periodTextBox = new TextBox { Location = new Point(10, 110), Width = 260 };
        var okButton = new Button { Text = "OK", Location = new Point(10, 140), Width = 80 };
        var cancelButton = new Button { Text = "Отмена", Location = new Point(100, 140), Width = 80 };

        okButton.Click += (sender, e) =>
        {
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