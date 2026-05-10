using System;
using System.Windows.Forms;
using System.Drawing;

public class AddSkillForm : Form
{
    public string Skill { get; private set; }
    private const int MAX_SKILL_LENGTH = 50;

    public AddSkillForm()
    {
        Text = "Добавить навык";
        Width = 450;       
        Height = 140;      
        StartPosition = FormStartPosition.CenterScreen;

        var skillLabel = new Label
        {
            Text = "Навык:",
            Location = new Point(10, 10),
            AutoSize = true
        };
        var skillTextBox = new TextBox
        {
            Location = new Point(10, 30),
            Width = 415,
            MaxLength = MAX_SKILL_LENGTH
        };
        var okButton = new Button
        {
            Text = "OK",
            Location = new Point(10, 70),
            Width = 100
        };
        var cancelButton = new Button
        {
            Text = "Отмена",
            Location = new Point(130, 70),  
            Width = 100
        };

        okButton.Click += (sender, e) =>
        {
            if (string.IsNullOrWhiteSpace(skillTextBox.Text))
            {
                MessageBox.Show("Навык не может быть пустым.");
                return;
            }

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