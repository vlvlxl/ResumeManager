using System;

public class Education
{
    public string Institution { get; set; }
    public string Degree { get; set; }
    public string Period { get; set; }

    public Education(string institution, string degree, string period)
    {
        Institution = institution;
        Degree = degree;
        Period = period;
    }

    public override string ToString()
    {
        return $"{Institution} - {Degree} ({Period})";
    }
}