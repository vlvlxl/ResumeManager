using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

public class SelectResumeForm : Form
{
    public List<Resume> Resumes { get; set; }
    public Resume SelectedResume { get; private set; }
    private Dictionary<int, Resume> resumeMap = new Dictionary<int, Resume>();

    public SelectResumeForm()
    {
        Text = "Выбрать резюме";
        Width = 500;
        Height = 310;
        StartPosition = FormStartPosition.CenterScreen;

        var resumesListBox = new ListBox
        {
            Location = new Point(10, 10),
            Width = 460,
            Height = 220
        };

        var okButton = new Button
        {
            Text = "OK",
            Location = new Point(10, 240),
            Width = 100
        };
        var cancelButton = new Button
        {
            Text = "Отмена",
            Location = new Point(130, 240),
            Width = 100
        };

        okButton.Click += (sender, e) =>
        {
            if (resumesListBox.SelectedItem != null)
            {
                SelectedResume = resumeMap[resumesListBox.SelectedIndex];
                DialogResult = DialogResult.OK;
                Close();
            }
        };

        cancelButton.Click += (sender, e) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        Controls.Add(resumesListBox);
        Controls.Add(okButton);
        Controls.Add(cancelButton);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (Resumes != null)
        {
            int index = 0;
            foreach (var resume in Resumes)
            {
                string displayName;
                if (string.IsNullOrWhiteSpace(resume.Objective))
                {
                    displayName = $"[{index + 1}] {resume.Name}";
                }
                else
                {
                    string objectiveText = resume.Objective;
                    int firstNewLine = objectiveText.IndexOf('\n');
                    if (firstNewLine >= 0)
                    {
                        objectiveText = objectiveText.Substring(0, firstNewLine).Trim();
                    }
                    displayName = $"[{index + 1}] {resume.Name} - {objectiveText}";
                }

                ((ListBox)Controls[0]).Items.Add(displayName);
                resumeMap[index] = resume;
                index++;
            }
        }
    }
}