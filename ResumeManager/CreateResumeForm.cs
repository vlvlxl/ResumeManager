using System;
using System.Windows.Forms;
using System.Drawing;

public class CreateResumeForm : Form
{
    public string Name { get; private set; }
    public string ContactInfo { get; private set; }
    public string Objective { get; private set; }
    private const int MAX_NAME_LENGTH = 50;
    private const int MAX_CONTACT_INFO_LENGTH = 50;
    private const int MAX_OBJECTIVE_LENGTH = 500;

    public CreateResumeForm()
    {
        Text = "Создать резюме";
        Width = 505;       
        Height = 320;      
        StartPosition = FormStartPosition.CenterScreen;

        var nameLabel = new Label
        {
            Text = "Имя:",
            Location = new Point(10, 10),    
            AutoSize = true
        };
        var nameTextBox = new TextBox
        {
            Location = new Point(10, 30),    
            Width = 470,                     
            Height = 25,
            MaxLength = MAX_NAME_LENGTH
        };
        var contactLabel = new Label
        {
            Text = "Контактная информация:",
            Location = new Point(10, 65),    
            AutoSize = true
        };
        var contactTextBox = new TextBox
        {
            Location = new Point(10, 85),    
            Width = 470,                     
            Height = 25,
            MaxLength = MAX_CONTACT_INFO_LENGTH
        };
        var objectiveLabel = new Label
        {
            Text = "Цель:",
            Location = new Point(10, 120),     
            AutoSize = true
        };
        var objectiveTextBox = new TextBox
        {
            Location = new Point(10, 145),   
            Width = 470,                     
            Height = 80,                    
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            MaxLength = MAX_OBJECTIVE_LENGTH
        };
        var okButton = new Button
        {
            Text = "OK",
            Location = new Point(10, 240),   
            Width = 100,                    
            Height = 30
        };
        var cancelButton = new Button
        {
            Text = "Отмена",
            Location = new Point(120, 240), 
            Width = 100,
            Height = 30
        };

        okButton.Click += (sender, e) =>
        {
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("Имя не может быть пустым.");
                return;
            }
            if (string.IsNullOrWhiteSpace(contactTextBox.Text))
            {
                MessageBox.Show("Контактная информация не может быть пустой.");
                return;
            }
            Name = nameTextBox.Text;
            ContactInfo = contactTextBox.Text;
            Objective = objectiveTextBox.Text;
            DialogResult = DialogResult.OK;
            Close();
        };

        cancelButton.Click += (sender, e) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        Controls.Add(nameLabel);
        Controls.Add(nameTextBox);
        Controls.Add(contactLabel);
        Controls.Add(contactTextBox);
        Controls.Add(objectiveLabel);
        Controls.Add(objectiveTextBox);
        Controls.Add(okButton);
        Controls.Add(cancelButton);
    }
}