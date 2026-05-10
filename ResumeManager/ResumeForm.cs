using System;
using System.Windows.Forms;
using System.Drawing;

public class ResumeForm : Form
{
    public Resume Resume { get; set; }

    private TextBox resumeTextBox;

    public ResumeForm()
    {
        Text = "Резюме";
        Width = 600;
        Height = 570;
        StartPosition = FormStartPosition.CenterScreen;

        resumeTextBox = new TextBox
        {
            Location = new Point(10, 10),
            Width = 560,
            Height = 470,
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            ReadOnly = true,
            Font = new Font("Segoe UI", 10)
        };

        var okButton = new Button { Text = "OK", Location = new Point(10, 490), Width = 100 };
        okButton.Click += (sender, e) => { DialogResult = DialogResult.OK; Close(); };
        
        Controls.Add(resumeTextBox);
        Controls.Add(okButton);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (Resume != null)
        {
            var nl = Environment.NewLine;
            var resumeText = "РЕЗЮМЕ" + nl + nl +
                           "ЛИЧНАЯ ИНФОРМАЦИЯ" + nl +
                           $"Имя: {Resume.Name}" + nl +
                           $"Контактная информация: {Resume.ContactInfo}" + nl +
                           $"Цель: ";
            if (string.IsNullOrWhiteSpace(Resume.Objective))
            {
                resumeText += "-" + nl + nl;
            }
            else
            {
                resumeText += nl + $"{Resume.Objective}" + nl + nl;
            }
            
            resumeText += "НАВЫКИ" + nl;
            if (Resume.Skills.Count > 0)
            {
                for (int i = 0; i < Resume.Skills.Count; i++)
                    resumeText += $"{i + 1}. {Resume.Skills[i]}" + nl;
            }
            else
            {
                resumeText += "Нет навыков" + nl;
            }

            resumeText += nl + "ОПЫТ РАБОТЫ" + nl;

            if (Resume.WorkExperiences.Count > 0)
            {
                for (int i = 0; i < Resume.WorkExperiences.Count; i++)
                {
                    var exp = Resume.WorkExperiences[i];
                    resumeText += $"{i + 1}. {exp.Position}" + nl +
                                 $"   Компания: {exp.Company}" + nl +
                                 $"   Период: {exp.Period}" + nl +
                                 $"   Описание: {exp.Description}" + nl + nl;
                }
            }
            else
            {
                resumeText += "Нет опыта работы" + nl;
            }

            resumeText += nl + "ОБРАЗОВАНИЕ" + nl;

            if (Resume.Educations.Count > 0)
            {
                for (int i = 0; i < Resume.Educations.Count; i++)
                {
                    var edu = Resume.Educations[i];
                    resumeText += $"{i + 1}. {edu.Institution}" + nl +
                                 $"   Степень: {edu.Degree}" + nl +
                                 $"   Период: {edu.Period}" + nl + nl;
                }
            }
            else
            {
                resumeText += "Нет образования" + nl;
            }

            resumeTextBox.Text = resumeText;
        }
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        resumeTextBox.SelectionStart = 0;
        resumeTextBox.SelectionLength = 0;
        resumeTextBox.ScrollToCaret();
    }
}