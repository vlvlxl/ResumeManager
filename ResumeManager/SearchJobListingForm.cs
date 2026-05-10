using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

public class SearchJobListingForm : Form
{
    public List<JobListing> JobListings { get; set; }
    public WrapListBox JobListingsListBox { get; private set; }
    private TextBox searchTextBox;
    private Button searchButton;
    private Button resetButton;
    private const int MAX_SEARCH_LENGTH = 20;

    public SearchJobListingForm()
    {
        Text = "Поиск вакансий";
        Width = 510;
        Height = 400;
        StartPosition = FormStartPosition.CenterScreen;

        var searchLabel = new Label 
        { 
            Text = "Поиск по ключевому слову:", 
            Location = new Point(10, 10), 
            AutoSize = true 
        };
        searchTextBox = new TextBox 
        { 
            Location = new Point(10, 30), 
            Width = 300, 
            MaxLength = MAX_SEARCH_LENGTH 
        };

        searchButton = new Button 
        { 
            Text = "Поиск", 
            Location = new Point(320, 28), 
            Width = 80 
        };
        searchButton.Click += (sender, e) => PerformSearch();
        
        resetButton = new Button 
        { 
            Text = "Сбросить", 
            Location = new Point(405, 28), 
            Width = 70 
        };
        resetButton.Click += (sender, e) => ResetSearch();

        JobListingsListBox = new WrapListBox
        {
            Location = new Point(10, 60),
            Width = 470,
            Height = 260,
            Font = new Font("Segoe UI", 9)
        };

        var okButton = new Button 
        { 
            Text = "OK", 
            Location = new Point(10, 330), 
            Width = 80 
        };

        okButton.Click += (sender, e) =>
        {
            DialogResult = DialogResult.OK;
            Close();
        };

        this.Click += SearchJobListingForm_Click;
        searchLabel.Click += (sender, e) => ClearListBoxSelection();
        searchTextBox.Click += (sender, e) => ClearListBoxSelection();
        searchButton.Click += (sender, e) => ClearListBoxSelection();
        resetButton.Click += (sender, e) => ClearListBoxSelection();
        okButton.Click += (sender, e) => ClearListBoxSelection();
        
        JobListingsListBox.MouseDown += (sender, e) =>
        {
            
        };

        Controls.Add(searchLabel);
        Controls.Add(searchTextBox);
        Controls.Add(searchButton);
        Controls.Add(resetButton);
        Controls.Add(JobListingsListBox);
        Controls.Add(okButton);
    }

    private void SearchJobListingForm_Click(object sender, EventArgs e)
    {
        ClearListBoxSelection();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (JobListings != null)
        {
            foreach (var jobListing in JobListings)
            {
                JobListingsListBox.Items.Add(FormatJobListing(jobListing));
            }
        }
    }

    private string FormatJobListing(JobListing job)
    {
        return $"{job.JobTitle}; {job.Company}\n" +
               $"Описание: {job.Description}\n" +
               $"Требования: {job.Requirements}";
    }

    public class WrapListBox : ListBox
    {
        private const int ITEM_MARGIN = 5;

        public WrapListBox()
        {
            this.DrawMode = DrawMode.OwnerDrawVariable;
            this.HorizontalScrollbar = false;
            this.ScrollAlwaysVisible = true;
            this.IntegralHeight = false;
            this.DoubleBuffered = true;
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            base.OnMeasureItem(e);

            if (e.Index >= 0 && e.Index < this.Items.Count)
            {
                string text = this.Items[e.Index].ToString();
                SizeF textSize = e.Graphics.MeasureString(text, this.Font, this.ClientSize.Width - 5);
                e.ItemHeight = (int)textSize.Height + ITEM_MARGIN;
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= this.Items.Count)
                return;

            string text = this.Items[e.Index].ToString();
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            if (isSelected)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(0, 120, 212)))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }
            }
            else
            {
                e.DrawBackground();
            }

            using (Brush brush = new SolidBrush(isSelected ? Color.White : e.ForeColor))
            {
                Rectangle bounds = new Rectangle(
                    e.Bounds.X + 2,
                    e.Bounds.Y,
                    this.ClientSize.Width - 9,
                    e.Bounds.Height - ITEM_MARGIN
                );
                e.Graphics.DrawString(text, this.Font, brush, bounds);
            }

            if (e.Index < this.Items.Count - 1)
            {
                using (Pen pen = new Pen(Color.LightGray, 1))
                {
                    int lineY = e.Bounds.Bottom - ITEM_MARGIN / 2;
                    e.Graphics.DrawLine(pen, e.Bounds.X + 2, lineY, e.Bounds.Right - 3, lineY);
                }
            }

            if (isSelected)
            {
                Rectangle focusBounds = new Rectangle(
                    e.Bounds.X,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height - ITEM_MARGIN
                );
                ControlPaint.DrawFocusRectangle(e.Graphics, focusBounds);
            }
        }
    }

    private void PerformSearch()
    {
        string keyword = searchTextBox.Text.Trim().ToLower();

        JobListingsListBox.Items.Clear();

        if (string.IsNullOrEmpty(keyword))
        {
            if (JobListings != null)
            {
                foreach (var jobListing in JobListings)
                {
                    JobListingsListBox.Items.Add(FormatJobListing(jobListing));
                }
            }
            return;
        }

        if (JobListings != null)
        {
            var filteredJobs = JobListings.Where(job =>
                job.JobTitle.ToLower().Contains(keyword) ||
                job.Company.ToLower().Contains(keyword) ||
                job.Description.ToLower().Contains(keyword) ||
                job.Requirements.ToLower().Contains(keyword)
            ).ToList();

            if (filteredJobs.Count == 0)
            {
                JobListingsListBox.Items.Add("Вакансии не найдены");
                MessageBox.Show($"Вакансии по запросу \"{keyword}\" не найдены.");
            }
            else
            {
                foreach (var jobListing in filteredJobs)
                {
                    JobListingsListBox.Items.Add(FormatJobListing(jobListing));
                }
                MessageBox.Show($"Найдено вакансий: {filteredJobs.Count}");
            }
        }
        ClearListBoxSelection();
    }

    private void ResetSearch()
    {
        searchTextBox.Text = string.Empty;
        JobListingsListBox.Items.Clear();

        if (JobListings != null)
        {
            foreach (var jobListing in JobListings)
            {
                JobListingsListBox.Items.Add(FormatJobListing(jobListing));
            }
        }
        ClearListBoxSelection();
        searchTextBox.Focus();
    }

    private void ClearListBoxSelection()
    {
        if (JobListingsListBox.SelectedIndex != -1)
        {
            JobListingsListBox.ClearSelected();
        }
    }
}