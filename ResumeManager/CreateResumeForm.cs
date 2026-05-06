using System;
using System.Windows.Forms;
using System.Drawing;

public class CreateResumeForm : Form
{
    public string Name { get; private set; }
    public string ContactInfo { get; private set; }
    public string Objective { get; private set; }

    public CreateResumeForm()
    {
        Text = "Создать резюме";
        Width = 300;
        Height = 200;
        StartPosition = FormStartPosition.CenterScreen;

        var nameLabel = new Label { Text = "Имя:", Location = new Point(10, 10) };
        var nameTextBox = new TextBox { Location = new Point(10, 30), Width = 260 };
        var contactLabel = new Label { Text = "Контактная информация:", Location = new Point(10, 50) };
        var contactTextBox = new TextBox { Location = new Point(10, 70), Width = 260 };
        var objectiveLabel = new Label { Text = "Цель:", Location = new Point(10, 90) };
        var objectiveTextBox = new TextBox { Location = new Point(10, 110), Width = 260 };
        var okButton = new Button { Text = "OK", Location = new Point(10, 140), Width = 80 };
        var cancelButton = new Button { Text = "Отмена", Location = new Point(100, 140), Width = 80 };

        okButton.Click += (sender, e) =>
        {
            Name = nameTextBox.Text;
            ContactInfo = contactTextBox.Text;
            Objective = objectiveTextBox.Text;
            DialogResult = DialogResult.OK;
            Close();
        };

        cancelButton.Click += (sender, e) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        Controls.Add(nameLabel);
        Controls.Add(nameTextBox);
        Controls.Add(contactLabel);
        Controls.Add(contactTextBox);
        Controls.Add(objectiveLabel);
        Controls.Add(objectiveTextBox);
        Controls.Add(okButton);
        Controls.Add(cancelButton);
    }
}