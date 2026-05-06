using System;
using System.Windows.Forms;
using System.Drawing;

public class AddSkillForm : Form
{
    public string Skill { get; private set; }

    public AddSkillForm()
    {
        Text = "Добавить навык";
        Width = 300;
        Height = 120;
        StartPosition = FormStartPosition.CenterScreen;

        var skillLabel = new Label { Text = "Навык:", Location = new Point(10, 10) };
        var skillTextBox = new TextBox { Location = new Point(10, 30), Width = 260 };
        var okButton = new Button { Text = "OK", Location = new Point(10, 60), Width = 80 };
        var cancelButton = new Button { Text = "Отмена", Location = new Point(100, 60), Width = 80 };

        okButton.Click += (sender, e) =>
        {
            Skill = skillTextBox.Text;
            DialogResult = DialogResult.OK;
            Close();
        };

        cancelButton.Click += (sender, e) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        Controls.Add(skillLabel);
        Controls.Add(skillTextBox);
        Controls.Add(okButton);
        Controls.Add(cancelButton);
    }
}