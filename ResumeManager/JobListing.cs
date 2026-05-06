using System;

public class JobListing
{
    public string JobTitle { get; set; }
    public string Company { get; set; }
    public string Description { get; set; }
    public string Requirements { get; set; }

    public JobListing(string jobTitle, string company, string description, string requirements)
    {
        JobTitle = jobTitle;
        Company = company;
        Description = description;
        Requirements = requirements;
    }

    public override string ToString()
    {
        return $"{JobTitle} в {Company}\nОписание: {Description}\nТребования: {Requirements}";
    }
}