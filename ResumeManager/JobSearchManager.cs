using System;
using System.Collections.Generic;
using System.Windows.Forms;

public class JobSearchManager
{
    private List<Resume> resumes = new List<Resume>();
    private List<JobListing> jobListings = new List<JobListing>();

    public void CreateResume()
    {
        var createResumeForm = new CreateResumeForm();
        createResumeForm.ShowDialog();
        if (createResumeForm.DialogResult == DialogResult.OK)
        {
            var resume = new Resume(
                createResumeForm.Name,
                createResumeForm.ContactInfo,
                createResumeForm.Objective);
            resumes.Add(resume);
            MessageBox.Show("Резюме создано.");
        }
    }

    public void AddSkillToResume()
    {
        if (resumes.Count == 0)
        {
            MessageBox.Show("Список резюме пуст.");
            return;
        }

        var selectResumeForm = new SelectResumeForm();
        selectResumeForm.Resumes = resumes;
        selectResumeForm.ShowDialog();
        if (selectResumeForm.DialogResult == DialogResult.OK)
        {
            var selectedResume = selectResumeForm.SelectedResume;
            var addSkillForm = new AddSkillForm();
            addSkillForm.ShowDialog();
            if (addSkillForm.DialogResult == DialogResult.OK)
            {
                selectedResume.AddSkill(addSkillForm.Skill);
            }
        }
    }

    public void AddWorkExperienceToResume()
    {
        if (resumes.Count == 0)
        {
            MessageBox.Show("Список резюме пуст.");
            return;
        }

        var selectResumeForm = new SelectResumeForm();
        selectResumeForm.Resumes = resumes;
        selectResumeForm.ShowDialog();
        if (selectResumeForm.DialogResult == DialogResult.OK)
        {
            var selectedResume = selectResumeForm.SelectedResume;
            var addWorkExperienceForm = new AddWorkExperienceForm();
            addWorkExperienceForm.ShowDialog();
            if (addWorkExperienceForm.DialogResult == DialogResult.OK)
            {
                selectedResume.AddWorkExperience(
                    addWorkExperienceForm.Position,
                    addWorkExperienceForm.Company,
                    addWorkExperienceForm.Period,
                    addWorkExperienceForm.Description);
            }
        }
    }

    public void AddEducationToResume()
    {
        if (resumes.Count == 0)
        {
            MessageBox.Show("Список резюме пуст.");
            return;
        }

        var selectResumeForm = new SelectResumeForm();
        selectResumeForm.Resumes = resumes;
        selectResumeForm.ShowDialog();
        if (selectResumeForm.DialogResult == DialogResult.OK)
        {
            var selectedResume = selectResumeForm.SelectedResume;
            var addEducationForm = new AddEducationForm();
            addEducationForm.ShowDialog();
            if (addEducationForm.DialogResult == DialogResult.OK)
            {
                selectedResume.AddEducation(
                    addEducationForm.Institution,
                    addEducationForm.Degree,
                    addEducationForm.Period);
            }
        }
    }

    public void DisplayResume()
    {
        if (resumes.Count == 0)
        {
            MessageBox.Show("Список резюме пуст.");
            return;
        }

        var selectResumeForm = new SelectResumeForm();
        selectResumeForm.Resumes = resumes;
        selectResumeForm.ShowDialog();
        if (selectResumeForm.DialogResult == DialogResult.OK)
        {
            var selectedResume = selectResumeForm.SelectedResume;
            selectedResume.DisplayResume();
        }
    }

    public void AddJobListing()
    {
        var addJobListingForm = new AddJobListingForm();
        addJobListingForm.ShowDialog();
        if (addJobListingForm.DialogResult == DialogResult.OK)
        {
            var jobListing = new JobListing(
                addJobListingForm.JobTitle,
                addJobListingForm.Company,
                addJobListingForm.Description,
                addJobListingForm.Requirements);
            jobListings.Add(jobListing);
            MessageBox.Show("Вакансия добавлена.");
        }
    }

    public void SearchJobListings()
    {
        if (jobListings.Count == 0)
        {
            MessageBox.Show("Список вакансий пуст.");
            return;
        }

        var searchJobListingForm = new SearchJobListingForm();
        searchJobListingForm.JobListings = jobListings;
        searchJobListingForm.ShowDialog();
    }
}