using System;
using System.Windows.Forms;
using System.Drawing;

public class ResumeForm : Form
{
    public Resume Resume { get; set; }

    public ResumeForm()
    {
        Text = "Резюме";
        Width = 400;
        Height = 400;
        StartPosition = FormStartPosition.CenterScreen;

        var resumeTextBox = new TextBox { Location = new Point(10, 10), Width = 360, Height = 360, Multiline = true };
        var okButton = new Button { Text = "OK", Location = new Point(10, 380), Width = 80 };
        var cancelButton = new Button { Text = "Отмена", Location = new Point(100, 380), Width = 80 };

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

        Controls.Add(resumeTextBox);
        Controls.Add(okButton);
        Controls.Add(cancelButton);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (Resume != null)
        {
            var resumeText = $"Имя: {Resume.Name}\nКонтактная информация: {Resume.ContactInfo}\nЦель: {Resume.Objective}\n\nНавыки:\n{string.Join("\n", Resume.Skills)}\n\nОпыт работы:\n{string.Join("\n", Resume.WorkExperiences)}\n\nОбразование:\n{string.Join("\n", Resume.Educations)}";
            ((TextBox)Controls[0]).Text = resumeText;
        }
    }
}