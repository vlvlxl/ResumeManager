using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

public class SelectResumeForm : Form
{
    public List<Resume> Resumes { get; set; }
    public Resume SelectedResume { get; private set; }

    public SelectResumeForm()
    {
        Text = "Выбрать резюме";
        Width = 300;
        Height = 200;
        StartPosition = FormStartPosition.CenterScreen;

        var resumesListBox = new ListBox { Location = new Point(10, 10), Width = 260, Height = 150 };
        var okButton = new Button { Text = "OK", Location = new Point(10, 170), Width = 80 };
        var cancelButton = new Button { Text = "Отмена", Location = new Point(100, 170), Width = 80 };

        okButton.Click += (sender, e) =>
        {
            if (resumesListBox.SelectedItem != null)
            {
                SelectedResume = (Resume)resumesListBox.SelectedItem;
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
            foreach (var resume in Resumes)
            {
                ((ListBox)Controls[0]).Items.Add(resume);
            }
        }
    }
}