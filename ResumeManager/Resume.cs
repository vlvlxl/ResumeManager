using System;
using System.Collections.Generic;
using System.Windows.Forms;

public class Resume
{
    public string Name { get; set; }
    public string ContactInfo { get; set; }
    public string Objective { get; set; }
    public List<string> Skills { get; set; }
    public List<WorkExperience> WorkExperiences { get; set; }
    public List<Education> Educations { get; set; }

    public Resume(string name, string contactInfo, string objective)
    {
        Name = name;
        ContactInfo = contactInfo;
        Objective = objective;
        Skills = new List<string>();
        WorkExperiences = new List<WorkExperience>();
        Educations = new List<Education>();
    }

    public void AddSkill(string skill)
    {
        Skills.Add(skill);
        MessageBox.Show($"Навык '{skill}' добавлен.");
    }

    public void AddWorkExperience(string position, string company, string period, string description)
    {
        WorkExperiences.Add(new WorkExperience(position, company, period, description));
        MessageBox.Show($"Опыт работы '{position}' в '{company}' добавлен.");
    }

    public void AddEducation(string institution, string degree, string period)
    {
        Educations.Add(new Education(institution, degree, period));
        MessageBox.Show($"Образование в '{institution}' добавлено.");
    }

    public void DisplayResume()
    {
        var resumeForm = new ResumeForm();
        resumeForm.Resume = this;
        resumeForm.ShowDialog();
    }
}