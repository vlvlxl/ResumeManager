using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;
using FlaUIListBox = FlaUI.Core.AutomationElements.ListBox;
#pragma warning disable CA1861, IDE0300, IDE0017, IDE0044, IDE0060, CA1822, IDE1006

namespace ResumeManager.Tests.UI
{
    [TestClass]
    public class ResumeManagerUITests
    {
        private FlaUI.Core.Application? _app;
        private UIA3Automation? _automation;
        private Window? _mainWindow;
        private const string AppPath = @"D:\Тестирование\ResumeManager\ResumeManager\bin\Debug\ResumeManager.exe";

        [TestInitialize]
        public void TestInitialize()
        {
            _app = FlaUI.Core.Application.Launch(AppPath);
            _automation = new UIA3Automation();
            _mainWindow = WaitForElement(() => _app?.GetMainWindow(_automation), timeoutMs: 15000)?.AsWindow();
            Assert.IsNotNull(_mainWindow, "Главное окно приложения не найдено");
            _mainWindow.SetForeground();
            _mainWindow.Focus();
            Thread.Sleep(500);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            try { _app?.Close(); } catch { }
            _automation?.Dispose();
            Thread.Sleep(500);
        }

        private static AutomationElement? WaitForElement(Func<AutomationElement?> finder, int timeoutMs = 5000)
        {
            var start = DateTime.Now;
            while ((DateTime.Now - start).TotalMilliseconds < timeoutMs)
            {
                var element = finder();
                if (element != null) return element;
                Thread.Sleep(200);
            }
            return null;
        }

        private Window WaitForMessageBox(string expectedTextPart, int timeoutMs = 10000)
        {
            var start = DateTime.Now;
            while ((DateTime.Now - start).TotalMilliseconds < timeoutMs)
            {
                var modal = _app?.GetMainWindow(_automation)?.ModalWindows?.FirstOrDefault()?.AsWindow();
                if (modal != null && (modal.Name.Contains(expectedTextPart, StringComparison.OrdinalIgnoreCase) || 
                    modal.FindFirstDescendant(cf => cf.ByControlType(ControlType.Text).And(cf.ByName(expectedTextPart, PropertyConditionFlags.MatchSubstring))) != null))
                    return modal;

                var win = _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByControlType(ControlType.Window)
                    .And(cf.ByName("ResumeManager", PropertyConditionFlags.MatchSubstring)
                    .Or(cf.ByName("Успех", PropertyConditionFlags.MatchSubstring))
                    .Or(cf.ByName("Ошибка", PropertyConditionFlags.MatchSubstring))
                    .Or(cf.ByName("Внимание", PropertyConditionFlags.MatchSubstring))))?.AsWindow();
                if (win != null)
                {
                    var txt = win.FindFirstDescendant(cf => cf.ByControlType(ControlType.Text).And(cf.ByName(expectedTextPart, PropertyConditionFlags.MatchSubstring)));
                    if (txt != null) return win;
                }
                Thread.Sleep(100);
            }
            return null;
        }

        private void ClickButtonByText(string buttonText)
        {
            var button = WaitForElement(() => _mainWindow?.FindFirstDescendant(cf => cf.ByText(buttonText))?.AsButton());
            Assert.IsNotNull(button);
            button.Click();
            Thread.Sleep(300);
        }

        private void CloseMessageBox()
        {
            Thread.Sleep(300);
            var msg = _mainWindow?.ModalWindows.FirstOrDefault()?.AsWindow();
            if (msg == null)
            {
                var element = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByControlType(ControlType.Window).And(cf.ByName("ResumeManager"))));
                msg = element?.AsWindow();
            }
            if (msg == null)
            {
                var element = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByControlType(ControlType.Window)
                .And(cf.ByName("Резюме создано.", PropertyConditionFlags.MatchSubstring).Or(cf.ByName(" добавлен ", PropertyConditionFlags.MatchSubstring)).Or(cf.ByName(" Вакансия добавлена", PropertyConditionFlags.MatchSubstring)))));
                msg = element?.AsWindow();
            }
            if (msg != null)
            {
                var okBtn = msg.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton();
                if (okBtn == null) okBtn = msg.FindFirstDescendant(cf => cf.ByText("ОК"))?.AsButton();
                if (okBtn == null) okBtn = msg.FindFirstDescendant(cf => cf.ByAutomationId("2"))?.AsButton();
                if (okBtn == null) okBtn = msg.FindFirstDescendant(cf => cf.ByControlType(ControlType.Button))?.AsButton();
                okBtn?.Click();
                Thread.Sleep(200);
            }
        }

        [TestMethod]
        public void TC_LAB3_001_CreateResume_WithValidData_ShouldSucceed()
        {
            ClickButtonByText("Создать резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Создать резюме"))?.AsWindow());
            Assert.IsNotNull(form);
            var edits = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(edits);
            Assert.IsTrue(edits.Length >= 3);
            var nameBox = edits[0].AsTextBox(); nameBox.Text = "Иван Иванов";
            var contactBox = edits[1].AsTextBox(); contactBox.Text = "ivan@email.com";
            var objectiveBox = edits[2].AsTextBox(); objectiveBox.Text = "Ищу работу разработчиком";
            var okBtn = form?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton();
            Assert.IsNotNull(okBtn);
            okBtn.Click();
            Thread.Sleep(500);
            var msg = WaitForMessageBox("Резюме создано.", 10000);
            Assert.IsNotNull(msg);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_002_CreateResume_WithEmptyName_ShouldShowError()
        {
            ClickButtonByText("Создать резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Создать резюме"))?.AsWindow());
            Assert.IsNotNull(form);
            var edits = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(edits);
            var contactBox = edits[1].AsTextBox(); contactBox.Text = "ivan@email.com";
            var objectiveBox = edits[2].AsTextBox(); objectiveBox.Text = "Ищу работу разработчиком";
            var okBtn = form?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton();
            Assert.IsNotNull(okBtn);
            okBtn.Click();
            Thread.Sleep(300);
            var error = WaitForMessageBox("Имя не может быть пустым.", 10000);
            Assert.IsNotNull(error);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_003_CreateResume_WithEmptyContact_ShouldShowError()
        {
            ClickButtonByText("Создать резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Создать резюме"))?.AsWindow());
            Assert.IsNotNull(form);
            var edits = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(edits);
            var nameBox = edits[0].AsTextBox(); nameBox.Text = "Иван Иванов";
            var objectiveBox = edits[2].AsTextBox(); objectiveBox.Text = "Ищу работу разработчиком";
            var okBtn = form?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton();
            Assert.IsNotNull(okBtn);
            okBtn.Click();
            Thread.Sleep(300);
            var error = WaitForMessageBox("Контактная информация не может быть пустой.", 10000);
            Assert.IsNotNull(error);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_004_CreateResume_WithEmptyObjective_ShouldSucceed()
        {
            ClickButtonByText("Создать резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Создать резюме"))?.AsWindow());
            Assert.IsNotNull(form);
            var edits = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(edits);
            var nameBox = edits[0].AsTextBox(); nameBox.Text = "Иван Иванов";
            var contactBox = edits[1].AsTextBox(); contactBox.Text = "ivan@email.com";
            var okBtn = form?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton();
            Assert.IsNotNull(okBtn);
            okBtn.Click();
            Thread.Sleep(500);
            var msg = WaitForMessageBox("Резюме создано.", 10000);
            Assert.IsNotNull(msg);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_005_AddSkill_WithValidData_ShouldSucceed()
        {
            CreateTestResume("Тест", "t@t.com", "Цель");
            ClickButtonByText("Добавить навык");
            var selectForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByControlType(ControlType.Window).And(cf.ByName("Выбрать резюме")))?.AsWindow(), timeoutMs: 10000);
            Assert.IsNotNull(selectForm, "Форма 'Выбрать резюме' не найдена");
            var list = selectForm?.FindFirstDescendant(cf => cf.ByControlType(ControlType.List));
            Assert.IsNotNull(list, "Список резюме не найден в форме выбора");
            var listBox = list.AsListBox();
            Assert.IsTrue(listBox.Items.Length > 0, "Список резюме пуст");
            listBox.Items[0].Click();
            Thread.Sleep(200);
            var okBtn = selectForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton();
            Assert.IsNotNull(okBtn, "Кнопка 'OK' не найдена в форме выбора резюме");
            okBtn.Click();
            Thread.Sleep(300);
            var skillForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByControlType(ControlType.Window).And(cf.ByName("Добавить навык/навыки")))?.AsWindow(), timeoutMs: 10000);
            Assert.IsNotNull(skillForm, "Форма 'Добавить навык/навыки' не найдена");
            var skillElement = skillForm?.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(skillElement, "Поле ввода навыка не найдено");
            var skillTextBox = skillElement.AsTextBox();
            skillTextBox.Text = "C#";
            var skillOkBtn = skillForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton();
            Assert.IsNotNull(skillOkBtn, "Кнопка 'OK' не найдена в форме добавления навыка");
            skillOkBtn.Click();
            Thread.Sleep(500);
            var msg = WaitForMessageBox("добавлен", 10000);
            Assert.IsNotNull(msg, "Сообщение об успешном добавлении не найдено. Проверьте текст и заголовок сообщения в приложении.");
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_006_AddSkill_WithEmptySkill_ShouldShowError()
        {
            CreateTestResume("Тест", "t@t.com", "Цель");
            ClickButtonByText("Добавить навык");
            var selectForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Выбрать резюме"))?.AsWindow());
            Assert.IsNotNull(selectForm);
            var list = selectForm?.FindFirstDescendant(cf => cf.ByControlType(ControlType.List));
            Assert.IsNotNull(list);
            list.AsListBox().Items[0].Click();
            selectForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var skillForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Добавить навык/навыки"))?.AsWindow());
            Assert.IsNotNull(skillForm);
            skillForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var error = WaitForMessageBox("Навык не может быть пустым.", 10000);
            Assert.IsNotNull(error);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_007_AddSkill_WithNoResumes_ShouldShowMessage()
        {
            ClickButtonByText("Добавить навык");
            Thread.Sleep(300);
            var msg = WaitForMessageBox("Список резюме пуст.", 10000);
            Assert.IsNotNull(msg);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_008_AddWorkExperience_WithValidData_ShouldSucceed()
        {
            CreateTestResume("Тест", "t@t.com", "Цель");
            ClickButtonByText("Добавить опыт");
            Assert.IsTrue(SelectResumeFromDialog("Тест"), "Не удалось выбрать резюме из диалога");
            var expForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByControlType(ControlType.Window)
            .And(cf.ByName("Добавить опыт работы", PropertyConditionFlags.MatchSubstring)))?.AsWindow(), timeoutMs: 10000);
            Assert.IsNotNull(expForm, "Форма добавления опыта не найдена");
            var edits = expForm?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(edits);
            Assert.IsTrue(edits.Length >= 4);
            edits[0].AsTextBox().Text = "Developer";
            edits[1].AsTextBox().Text = "TechCorp";
            edits[2].AsTextBox().Text = "2023-2024";
            edits[3].AsTextBox().Text = "Разработка ПО";
            expForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(500);
            var msg = WaitForMessageBox("добавлен", 10000);
            Assert.IsNotNull(msg);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_009_AddWorkExperience_WithEmptyPosition_ShouldShowError()
        {
            CreateTestResume("Тест", "t@t.com", "Цель");
            ClickButtonByText("Добавить опыт");
            var selectForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Выбрать резюме"))?.AsWindow());
            var list = selectForm?.FindFirstDescendant(cf => cf.ByControlType(ControlType.List));
            Assert.IsNotNull(list);
            list.AsListBox().Items[0].Click();
            selectForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var expForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Добавить опыт работы"))?.AsWindow());
            var edits = expForm?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(edits);
            var compBox = edits[1].AsTextBox(); compBox.Text = "TechCorp";
            var perBox = edits[2].AsTextBox(); perBox.Text = "2023-2024";
            var descBox = edits[3].AsTextBox(); descBox.Text = "Описание";
            expForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var error = WaitForMessageBox("Должность не может быть пустой.", 10000);
            Assert.IsNotNull(error);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_010_AddWorkExperience_WithEmptyCompany_ShouldShowError()
        {
            CreateTestResume("Тест", "t@t.com", "Цель");
            ClickButtonByText("Добавить опыт");
            var selectForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Выбрать резюме"))?.AsWindow());
            var list = selectForm?.FindFirstDescendant(cf => cf.ByControlType(ControlType.List));
            Assert.IsNotNull(list);
            list.AsListBox().Items[0].Click();
            selectForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var expForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Добавить опыт работы"))?.AsWindow());
            var edits = expForm?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(edits);
            var posBox = edits[0].AsTextBox(); posBox.Text = "Developer";
            var perBox = edits[2].AsTextBox(); perBox.Text = "2023-2024";
            var descBox = edits[3].AsTextBox(); descBox.Text = "Описание";
            expForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var error = WaitForMessageBox("Компания не может быть пустой.", 10000);
            Assert.IsNotNull(error);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_011_AddWorkExperience_WithEmptyPeriod_ShouldShowError()
        {
            CreateTestResume("Тест", "t@t.com", "Цель");
            ClickButtonByText("Добавить опыт");
            var selectForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Выбрать резюме"))?.AsWindow());
            var list = selectForm?.FindFirstDescendant(cf => cf.ByControlType(ControlType.List));
            Assert.IsNotNull(list);
            list.AsListBox().Items[0].Click();
            selectForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var expForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Добавить опыт работы"))?.AsWindow());
            var edits = expForm?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(edits);
            var posBox = edits[0].AsTextBox(); posBox.Text = "Developer";
            var compBox = edits[1].AsTextBox(); compBox.Text = "TechCorp";
            var descBox = edits[3].AsTextBox(); descBox.Text = "Описание";
            expForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var error = WaitForMessageBox("Период не может быть пустым.", 10000);
            Assert.IsNotNull(error);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_012_AddWorkExperience_WithEmptyDescription_ShouldShowError()
        {
            CreateTestResume("Тест", "t@t.com", "Цель");
            ClickButtonByText("Добавить опыт");
            var selectForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Выбрать резюме"))?.AsWindow());
            var list = selectForm?.FindFirstDescendant(cf => cf.ByControlType(ControlType.List));
            Assert.IsNotNull(list);
            list.AsListBox().Items[0].Click();
            selectForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var expForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Добавить опыт работы"))?.AsWindow());
            var edits = expForm?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(edits);
            var posBox = edits[0].AsTextBox(); posBox.Text = "Developer";
            var compBox = edits[1].AsTextBox(); compBox.Text = "TechCorp";
            var perBox = edits[2].AsTextBox(); perBox.Text = "2023-2024";
            expForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var error = WaitForMessageBox("Описание не может быть пустым.", 10000);
            Assert.IsNotNull(error);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_013_AddEducation_WithValidData_ShouldSucceed()
        {
            CreateTestResume("Тест", "t@t.com", "Цель");
            ClickButtonByText("Добавить образование");
            var selectForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Выбрать резюме"))?.AsWindow());
            Assert.IsNotNull(selectForm);
            var list = selectForm?.FindFirstDescendant(cf => cf.ByControlType(ControlType.List));
            Assert.IsNotNull(list);
            list.AsListBox().Items[0].Click();
            selectForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var eduForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Добавить образование"))?.AsWindow());
            Assert.IsNotNull(eduForm);
            var edits = eduForm?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(edits);
            Assert.IsTrue(edits.Length >= 3);
            var instBox = edits[0].AsTextBox(); instBox.Text = "МГУ";
            var degBox = edits[1].AsTextBox(); degBox.Text = "Бакалавр";
            var perBox = edits[2].AsTextBox(); perBox.Text = "2020-2024";
            eduForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var msg = WaitForMessageBox("добавлено", 10000);
            Assert.IsNotNull(msg);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_014_AddEducation_WithEmptyInstitution_ShouldShowError()
        {
            CreateTestResume("Тест", "t@t.com", "Цель");
            ClickButtonByText("Добавить образование");
            var selectForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Выбрать резюме"))?.AsWindow());
            var list = selectForm?.FindFirstDescendant(cf => cf.ByControlType(ControlType.List));
            Assert.IsNotNull(list);
            list.AsListBox().Items[0].Click();
            selectForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var eduForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Добавить образование"))?.AsWindow());
            var edits = eduForm?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(edits);
            var degBox = edits[1].AsTextBox(); degBox.Text = "Бакалавр";
            var perBox = edits[2].AsTextBox(); perBox.Text = "2020-2024";
            eduForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var error = WaitForMessageBox("Учебное заведение не может быть пустым", 10000);
            Assert.IsNotNull(error);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_015_AddEducation_WithEmptyDegree_ShouldShowError()
        {
            CreateTestResume("Тест", "t@t.com", "Цель");
            ClickButtonByText("Добавить образование");
            var selectForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Выбрать резюме"))?.AsWindow());
            var list = selectForm?.FindFirstDescendant(cf => cf.ByControlType(ControlType.List));
            Assert.IsNotNull(list);
            list.AsListBox().Items[0].Click();
            selectForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var eduForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Добавить образование"))?.AsWindow());
            var edits = eduForm?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(edits);
            var instBox = edits[0].AsTextBox(); instBox.Text = "МГУ";
            var perBox = edits[2].AsTextBox(); perBox.Text = "2020-2024";
            eduForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var error = WaitForMessageBox("Степень не может быть пустой.", 10000);
            Assert.IsNotNull(error);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_016_AddEducation_WithEmptyPeriod_ShouldShowError()
        {
            CreateTestResume("Тест", "t@t.com", "Цель");
            ClickButtonByText("Добавить образование");
            var selectForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Выбрать резюме"))?.AsWindow());
            var list = selectForm?.FindFirstDescendant(cf => cf.ByControlType(ControlType.List));
            Assert.IsNotNull(list);
            list.AsListBox().Items[0].Click();
            selectForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var eduForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Добавить образование"))?.AsWindow());
            var edits = eduForm?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(edits);
            var instBox = edits[0].AsTextBox(); instBox.Text = "МГУ";
            var degBox = edits[1].AsTextBox(); degBox.Text = "Бакалавр";
            eduForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var error = WaitForMessageBox("Период не может быть пустым.", 10000);
            Assert.IsNotNull(error);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_017_DisplayResume_WithFilledData_ShouldShowAllFields()
        {
            CreateTestResumeWithFullData("Иван", "ivan@test.com", "Ищу работу разработчиком с опытом более 5 лет", new[] { "C#" }, "Dev", "Corp", "2020-2024", "Work", "Univ", "Bachelor", "2016-2020");
            ClickButtonByText("Показать резюме");
            Assert.IsTrue(SelectResumeFromDialog("Иван"), "Не удалось выбрать резюме для просмотра");
            var resumeForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByControlType(ControlType.Window)
            .And(cf.ByName("Резюме", PropertyConditionFlags.MatchSubstring)))?.AsWindow(), timeoutMs: 10000);
            Assert.IsNotNull(resumeForm);
            var textBox = resumeForm?.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(textBox);
            var text = textBox.AsTextBox().Text;
            Assert.IsTrue(text.Contains("Иван"));
            Assert.IsTrue(text.Contains("НАВЫКИ"));
            Assert.IsTrue(text.Contains("ОПЫТ РАБОТЫ"));
            Assert.IsTrue(text.Contains("ОБРАЗОВАНИЕ"));
            resumeForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
        }

        [TestMethod]
        public void TC_LAB3_018_DisplayResume_WithNoResumes_ShouldShowMessage()
        {
            ClickButtonByText("Показать резюме");
            Thread.Sleep(300);
            var msg = WaitForMessageBox("Список резюме пуст.", 10000);
            Assert.IsNotNull(msg);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_019_AddJobListing_WithValidData_ShouldSucceed()
        {
            ClickButtonByText("Добавить вакансию");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Добавить вакансию"))?.AsWindow());
            Assert.IsNotNull(form);
            var edits = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(edits);
            Assert.IsTrue(edits.Length >= 2);
            var titleBox = edits[0].AsTextBox(); titleBox.Text = "Developer";
            var compBox = edits[1].AsTextBox(); compBox.Text = "TechCorp";
            var panels = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.Pane));
            if (panels != null && panels.Length >= 2)
            {
                var descElement = panels[0].FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
                if (descElement != null) descElement.AsTextBox().Text = "Разработка ПО";
                var reqElement = panels[1].FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
                if (reqElement != null) reqElement.AsTextBox().Text = "C#, Git";
            }
            form?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(500);
            var msg = WaitForMessageBox("Вакансия добавлена.", 10000);
            Assert.IsNotNull(msg);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_020_AddJobListing_WithEmptyTitle_ShouldShowError()
        {
            ClickButtonByText("Добавить вакансию");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Добавить вакансию"))?.AsWindow());
            Assert.IsNotNull(form);
            var edits = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(edits);
            var compBox = edits[1].AsTextBox(); compBox.Text = "TechCorp";
            var panels = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.Pane));
            if (panels != null && panels.Length >= 2)
            {
                var descElement = panels[0].FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
                if (descElement != null) descElement.AsTextBox().Text = "Описание";
                var reqElement = panels[1].FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
                if (reqElement != null) reqElement.AsTextBox().Text = "Требования";
            }
            form?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var error = WaitForMessageBox("Название вакансии не может быть пустым.", 10000);
            Assert.IsNotNull(error);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_021_AddJobListing_WithEmptyCompany_ShouldShowError()
        {
            ClickButtonByText("Добавить вакансию");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Добавить вакансию"))?.AsWindow());
            Assert.IsNotNull(form);
            var edits = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(edits);
            var titleBox = edits[0].AsTextBox(); titleBox.Text = "Developer";
            var panels = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.Pane));
            if (panels != null && panels.Length >= 2)
            {
                var descElement = panels[0].FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
                if (descElement != null) descElement.AsTextBox().Text = "Описание";
                var reqElement = panels[1].FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
                if (reqElement != null) reqElement.AsTextBox().Text = "Требования";
            }
            form?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var error = WaitForMessageBox("Компания не может быть пустой.", 10000);
            Assert.IsNotNull(error);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_022_AddJobListing_WithEmptyDescription_ShouldShowError()
        {
            ClickButtonByText("Добавить вакансию");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Добавить вакансию"))?.AsWindow());
            Assert.IsNotNull(form);
            var edits = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(edits);
            var titleBox = edits[0].AsTextBox(); titleBox.Text = "Developer";
            var compBox = edits[1].AsTextBox(); compBox.Text = "TechCorp";
            var panels = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.Pane));
            if (panels != null && panels.Length >= 2)
            {
                var reqElement = panels[1].FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
                if (reqElement != null) reqElement.AsTextBox().Text = "Требования";
            }
            form?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var error = WaitForMessageBox("Описание не может быть пустым.", 10000);
            Assert.IsNotNull(error);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_023_AddJobListing_WithEmptyRequirements_ShouldShowError()
        {
            ClickButtonByText("Добавить вакансию");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Добавить вакансию"))?.AsWindow());
            Assert.IsNotNull(form);
            var edits = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(edits);
            var titleBox = edits[0].AsTextBox(); titleBox.Text = "Developer";
            var compBox = edits[1].AsTextBox(); compBox.Text = "TechCorp";
            var panels = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.Pane));
            if (panels != null && panels.Length >= 1)
            {
                var descElement = panels[0].FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
                if (descElement != null) descElement.AsTextBox().Text = "Описание";
            }
            form?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var error = WaitForMessageBox("Требования не могут быть пустыми.", 10000);
            Assert.IsNotNull(error);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_024_SearchJobs_WithNoJobs_ShouldShowMessage()
        {
            ClickButtonByText("Поиск вакансий");
            Thread.Sleep(300);
            var msg = WaitForMessageBox("Список вакансий пуст.", 10000);
            Assert.IsNotNull(msg);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB3_025_SearchJobs_WithKeyword_ShouldFilterResults()
        {
            CreateTestJobListing("Developer", "TechCorp", "Описание", "C#, Git");
            ClickButtonByText("Поиск вакансий");
            var searchForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Поиск вакансий"))?.AsWindow());
            Assert.IsNotNull(searchForm);
            var searchBox = searchForm?.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(searchBox);
            searchBox.AsTextBox().Text = "Dev";
            var searchBtn = searchForm?.FindFirstDescendant(cf => cf.ByText("Поиск"))?.AsButton();
            searchBtn?.Click();
            Thread.Sleep(500);
            var msg = WaitForMessageBox("Найдено", 10000);
            Assert.IsNotNull(msg);
            CloseMessageBox();
            searchForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
        }

        [TestMethod]
        public void TC_LAB5_001_OpenAnalysisForm_ShouldOpenSuccessfully()
        {
            CreateTestResume("Тест", "t@t.com", "Цель");
            CreateTestJobListing("Dev", "Corp", "Desc", "C#");
            ClickButtonByText("Анализ резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Анализ соответствия резюме вакансии"))?.AsWindow());
            Assert.IsNotNull(form);
            Assert.IsNotNull(form?.FindFirstDescendant(cf => cf.ByText("Анализировать")));
            Assert.IsNotNull(form?.FindFirstDescendant(cf => cf.ByText("Сохранить отчёт")));
            Assert.IsNotNull(form?.FindFirstDescendant(cf => cf.ByText("Соответствие: --%")));
        }

        [TestMethod]
        public void TC_LAB5_002_TooltipOnResume_ShouldShowAllFields()
        {
            CreateTestResumeWithFullData("Иван", "ivan@test.com", "Цель", new[] { "C#" }, "Dev", "Corp", "2020", "Work", "Univ", "Bach", "2016");
            CreateTestJobListing("Developer", "TechCorp", "Описание вакансии", "C#, Git, SQL");
            ClickButtonByText("Анализ резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Анализ соответствия резюме вакансии"))?.AsWindow(), timeoutMs: 10000);
            Assert.IsNotNull(form, "Форма анализа не открылась. Проверьте, что созданы и резюме, и вакансия.");
            var lists = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List));
            Assert.IsNotNull(lists);
            Assert.IsTrue(lists.Length >= 2, "На форме анализа не найдено два списка (резюме и вакансии)");
            var list = lists[0];
            Assert.IsNotNull(list);
            var listBox = list.AsListBox();
            Assert.IsTrue(listBox.Items.Length > 0, "Список резюме пуст");
            var item = listBox.Items[0];
            item.Click();
            Thread.Sleep(200);
            var rect = item.BoundingRectangle;
            Mouse.MoveTo(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            Thread.Sleep(800);
            var tooltipText = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf =>
                cf.ByControlType(ControlType.Text)
                  .And(cf.ByName("Имя:", PropertyConditionFlags.MatchSubstring)
                      .Or(cf.ByName("Иван", PropertyConditionFlags.MatchSubstring)
                          .Or(cf.ByName("Контакты:", PropertyConditionFlags.MatchSubstring)
                              .Or(cf.ByName("Цель:", PropertyConditionFlags.MatchSubstring)))))));
            Assert.IsNotNull(tooltipText, "Текст тултипа не найден. Проверьте, что тултип появляется при наведении.");
        }

        [TestMethod]
        public void TC_LAB5_003_TooltipOnJob_ShouldShowAllFields()
        {
            CreateTestResume("Тест", "t@t.com", "Цель с более чем двадцатью символами");
            CreateTestJobListing("Developer", "TechCorp", "Описание вакансии", "C#, Git, SQL");
            ClickButtonByText("Анализ резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Анализ соответствия резюме вакансии"))?.AsWindow(), timeoutMs: 10000);
            Assert.IsNotNull(form, "Форма анализа не открылась. Проверьте, что созданы и резюме, и вакансия.");
            var lists = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List));
            Assert.IsNotNull(lists);
            Assert.IsTrue(lists.Length >= 2, "На форме анализа не найдено два списка (резюме и вакансии)");
            var list = lists[1];
            Assert.IsNotNull(list);
            var listBox = list.AsListBox();
            Assert.IsTrue(listBox.Items.Length > 0, "Список вакансий пуст");
            var item = listBox.Items[0];
            item.Click();
            Thread.Sleep(200);
            var rect = item.BoundingRectangle;
            Mouse.MoveTo(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            Thread.Sleep(800);
            var tooltipText = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf =>
                cf.ByControlType(ControlType.Text)
                  .And(cf.ByName("Вакансия:", PropertyConditionFlags.MatchSubstring)
                      .Or(cf.ByName("Developer", PropertyConditionFlags.MatchSubstring)
                          .Or(cf.ByName("Компания:", PropertyConditionFlags.MatchSubstring)
                              .Or(cf.ByName("Описание:", PropertyConditionFlags.MatchSubstring)))))));
            Assert.IsNotNull(tooltipText, "Текст тултипа не найден. Проверьте, что тултип появляется при наведении.");
        }

        [TestMethod]
        public void TC_LAB5_004_Analyze_WithFullMatch_ShouldReturn100Percent()
        {
            CreateTestResumeWithSkills("Иван", "ivan@test.com", "Ищу работу разработчиком с опытом более 5 лет", new[] { "C#", "Git", "SQL" });
            CreateTestJobListing("Developer", "TechCorp", "Разработка", "C#, Git, SQL");
            ClickButtonByText("Анализ резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Анализ соответствия резюме вакансии"))?.AsWindow());
            var listResumes = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[0];
            var listJobs = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[1];
            Assert.IsNotNull(listResumes);
            Assert.IsNotNull(listJobs);
            listResumes.AsListBox().Items[0].Click();
            listJobs.AsListBox().Items[0].Click();
            form?.FindFirstDescendant(cf => cf.ByText("Анализировать"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var percentLabel = WaitForElement(() => form?.FindFirstDescendant(cf => cf.ByText("Соответствие: 100%")), timeoutMs: 10000);
            Assert.IsNotNull(percentLabel, "Метка процента не обновилась до 100%");
            var recBox = WaitForElement(() => form?.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit)), timeoutMs: 5000);
            Assert.IsNotNull(recBox, "Поле рекомендаций не найдено");
            string recText = recBox.AsTextBox().Text;
            bool hasExpectedRecommendations =
                recText.Contains("Добавьте данные в раздел 'Опыт работы' - это повысит шансы на трудоустройство.") &&
                recText.Contains("Добавьте данные в раздел 'Образование' - укажите учебные заведения и квалификации.");
            Assert.IsTrue(hasExpectedRecommendations,
                $"Текст рекомендаций не соответствует ожидаемому. Ожидалось: \"Добавьте данные в раздел 'Опыт работы' - это повысит шансы на трудоустройство." +
                $"\\nДобавьте данные в раздел 'Образование' - укажите учебные заведения и квалификации.\". Получено: \"{recText}\"");
        }

        [TestMethod]
        public void TC_LAB5_005_Analyze_WithPartialMatch_ShouldReturn50Percent()
        {
            CreateTestResumeWithSkills("Анна", "anna@test.com", "QA инженер с опытом тестирования более 3 лет", new[] { "Manual Testing", "SQL" });
            CreateTestJobListing("QA Engineer", "TestLab", "Тестирование", "Manual Testing, SQL, Java, C#");
            ClickButtonByText("Анализ резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Анализ соответствия резюме вакансии"))?.AsWindow());
            var listResumes = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[0];
            var listJobs = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[1];
            Assert.IsNotNull(listResumes);
            Assert.IsNotNull(listJobs);
            listResumes.AsListBox().Items[0].Click();
            listJobs.AsListBox().Items[0].Click();
            form?.FindFirstDescendant(cf => cf.ByText("Анализировать"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var percentLabel = WaitForElement(() => form?.FindFirstDescendant(cf => cf.ByText("Соответствие: 50%")));
            Assert.IsNotNull(percentLabel);
            var recBox = form?.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(recBox);
            Assert.IsTrue(recBox.AsTextBox().Text.Contains("Java") || recBox.AsTextBox().Text.Contains("C#"));
        }

        [TestMethod]
        public void TC_LAB5_006_Analyze_WithZeroMatch_ShouldReturn0Percent()
        {
            CreateTestResumeWithSkills("Петр", "petr@test.com", "Дизайнер с опытом", new[] { "Photoshop", "Figma" });
            CreateTestJobListing("Backend Dev", "CodeCo", "Серверная часть", "C#, Java");
            ClickButtonByText("Анализ резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Анализ соответствия резюме вакансии"))?.AsWindow());
            var listResumes = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[0];
            var listJobs = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[1];
            Assert.IsNotNull(listResumes);
            Assert.IsNotNull(listJobs);
            listResumes.AsListBox().Items[0].Click();
            listJobs.AsListBox().Items[0].Click();
            form?.FindFirstDescendant(cf => cf.ByText("Анализировать"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var percentLabel = WaitForElement(() => form?.FindFirstDescendant(cf => cf.ByText("Соответствие: 0%")));
            Assert.IsNotNull(percentLabel);
            var recBox = form?.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(recBox);
            Assert.IsTrue(recBox.AsTextBox().Text.Contains("Соответствие низкое"));
        }

        [TestMethod]
        public void TC_LAB5_007_Analyze_ShouldIgnoreCase()
        {
            CreateTestResumeWithSkills("Мария", "maria@test.com", "Разработчик с опытом", new[] { "c#", "GIT", " sql " });
            CreateTestJobListing("Dev", "SoftInc", "Backend", "C#, Git, SQL");
            ClickButtonByText("Анализ резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Анализ соответствия резюме вакансии"))?.AsWindow());
            var listResumes = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[0];
            var listJobs = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[1];
            Assert.IsNotNull(listResumes);
            Assert.IsNotNull(listJobs);
            listResumes.AsListBox().Items[0].Click();
            listJobs.AsListBox().Items[0].Click();
            form?.FindFirstDescendant(cf => cf.ByText("Анализировать"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var percentLabel = WaitForElement(() => form?.FindFirstDescendant(cf => cf.ByText("Соответствие: 100%")));
            Assert.IsNotNull(percentLabel);
        }

        [TestMethod]
        public void TC_LAB5_008_Analyze_ShouldIgnoreSkillOrder()
        {
            CreateTestResumeWithSkills("Тест", "t@t.com", "Цель с более чем двадцатью символами", new[] { "C#", "Git", "SQL" });
            CreateTestJobListing("Dev", "Corp", "Desc", "SQL, C#, Git");
            ClickButtonByText("Анализ резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Анализ соответствия резюме вакансии"))?.AsWindow());
            var listResumes = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[0];
            var listJobs = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[1];
            Assert.IsNotNull(listResumes);
            Assert.IsNotNull(listJobs);
            listResumes.AsListBox().Items[0].Click();
            listJobs.AsListBox().Items[0].Click();
            form?.FindFirstDescendant(cf => cf.ByText("Анализировать"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var percentLabel = WaitForElement(() => form?.FindFirstDescendant(cf => cf.ByText("Соответствие: 100%")));
            Assert.IsNotNull(percentLabel);
        }

        [TestMethod]
        public void TC_LAB5_009_Analyze_ShouldParseMultilineRequirements()
        {
            CreateTestResumeWithSkills("Тест", "t@t.com", "Цель с более чем двадцатью символами", new[] { "C#", "Git", "SQL" });
            CreateTestJobListing("Dev", "Corp", "Desc", "C#\nGit\nSQL");
            ClickButtonByText("Анализ резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Анализ соответствия резюме вакансии"))?.AsWindow());
            var listResumes = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[0];
            var listJobs = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[1];
            Assert.IsNotNull(listResumes);
            Assert.IsNotNull(listJobs);
            listResumes.AsListBox().Items[0].Click();
            listJobs.AsListBox().Items[0].Click();
            form?.FindFirstDescendant(cf => cf.ByText("Анализировать"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var percentLabel = WaitForElement(() => form?.FindFirstDescendant(cf => cf.ByText("Соответствие: 100%")));
            Assert.IsNotNull(percentLabel);
        }

        [TestMethod]
        public void TC_LAB5_010_Analyze_ShouldParseMixedFormatRequirements()
        {
            CreateTestResumeWithSkills("Тест", "t@t.com", "Цель с более чем двадцатью символами", new[] { "C#", "Git", "SQL" });
            CreateTestJobListing("Dev", "Corp", "Desc", "C#\nGit, SQL");
            ClickButtonByText("Анализ резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Анализ соответствия резюме вакансии"))?.AsWindow());
            var listResumes = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[0];
            var listJobs = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[1];
            Assert.IsNotNull(listResumes);
            Assert.IsNotNull(listJobs);
            listResumes.AsListBox().Items[0].Click();
            listJobs.AsListBox().Items[0].Click();
            form?.FindFirstDescendant(cf => cf.ByText("Анализировать"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var percentLabel = WaitForElement(() => form?.FindFirstDescendant(cf => cf.ByText("Соответствие: 100%")));
            Assert.IsNotNull(percentLabel);
        }

        [TestMethod]
        public void TC_LAB5_011_Analyze_ShouldHandleSkillsAddedOneByOne()
        {
            CreateTestResume("Тест", "t@t.com", "Цель с более чем двадцатью символами");
            AddSkillToResume("C#");
            AddSkillToResume("Git");
            AddSkillToResume("SQL");
            CreateTestJobListing("Dev", "Corp", "Desc", "C#, Git, SQL");
            ClickButtonByText("Анализ резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Анализ соответствия резюме вакансии"))?.AsWindow());
            var listResumes = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[0];
            var listJobs = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[1];
            Assert.IsNotNull(listResumes);
            Assert.IsNotNull(listJobs);
            listResumes.AsListBox().Items[0].Click();
            listJobs.AsListBox().Items[0].Click();
            form?.FindFirstDescendant(cf => cf.ByText("Анализировать"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var percentLabel = WaitForElement(() => form?.FindFirstDescendant(cf => cf.ByText("Соответствие: 100%")));
            Assert.IsNotNull(percentLabel);
        }

        [TestMethod]
        public void TC_LAB5_012_Analyze_ShouldHandleMixedSkillAddition()
        {
            CreateTestResume("Тест", "t@t.com", "Цель с более чем двадцатью символами");
            AddSkillToResume("C#");
            AddSkillToResume("Git");
            AddSkillToResume("SQL, C++, MS Word");
            CreateTestJobListing("Dev", "Corp", "Desc", "C#, Git, SQL, C++, MS Word");
            ClickButtonByText("Анализ резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Анализ соответствия резюме вакансии"))?.AsWindow());
            var listResumes = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[0];
            var listJobs = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[1];
            Assert.IsNotNull(listResumes);
            Assert.IsNotNull(listJobs);
            listResumes.AsListBox().Items[0].Click();
            listJobs.AsListBox().Items[0].Click();
            form?.FindFirstDescendant(cf => cf.ByText("Анализировать"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var percentLabel = WaitForElement(() => form?.FindFirstDescendant(cf => cf.ByText("Соответствие: 100%")));
            Assert.IsNotNull(percentLabel);
        }

        [TestMethod]
        public void TC_LAB5_013_Analyze_WithShortObjective_ShouldShowRecommendation()
        {
            CreateTestResumeWithSkills("Тест", "t@t.com", "Цель", new[] { "C#", "Git", "SQL" });
            CreateTestJobListing("Dev", "Corp", "Desc", "C#, Git, SQL");
            ClickButtonByText("Анализ резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Анализ соответствия резюме вакансии"))?.AsWindow());
            var listResumes = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[0];
            var listJobs = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[1];
            Assert.IsNotNull(listResumes);
            Assert.IsNotNull(listJobs);
            listResumes.AsListBox().Items[0].Click();
            listJobs.AsListBox().Items[0].Click();
            form?.FindFirstDescendant(cf => cf.ByText("Анализировать"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var recBox = form?.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(recBox);
            Assert.IsTrue(recBox.AsTextBox().Text.Contains("Цель"));
        }

        [TestMethod]
        public void TC_LAB5_014_Analyze_WithNoSkills_ShouldReturn0Percent()
        {
            CreateTestResume("Тест", "t@t.com", "Цель с более чем двадцатью символами");
            CreateTestJobListing("Dev", "Corp", "Desc", "C#, Git, SQL");
            ClickButtonByText("Анализ резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Анализ соответствия резюме вакансии"))?.AsWindow());
            var listResumes = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[0];
            var listJobs = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[1];
            Assert.IsNotNull(listResumes);
            Assert.IsNotNull(listJobs);
            listResumes.AsListBox().Items[0].Click();
            listJobs.AsListBox().Items[0].Click();
            form?.FindFirstDescendant(cf => cf.ByText("Анализировать"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var percentLabel = WaitForElement(() => form?.FindFirstDescendant(cf => cf.ByText("Соответствие: 0%")));
            Assert.IsNotNull(percentLabel);
        }

        [TestMethod]
        public void TC_LAB5_015_Analyze_WithNoExperience_ShouldShowRecommendation()
        {
            CreateTestResumeWithSkills("Тест", "t@t.com", "Цель с более чем двадцатью символами", new[] { "C#", "Git", "SQL" });
            CreateTestJobListing("Dev", "Corp", "Desc", "C#, Git, SQL");
            ClickButtonByText("Анализ резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Анализ соответствия резюме вакансии"))?.AsWindow());
            var listResumes = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[0];
            var listJobs = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[1];
            Assert.IsNotNull(listResumes);
            Assert.IsNotNull(listJobs);
            listResumes.AsListBox().Items[0].Click();
            listJobs.AsListBox().Items[0].Click();
            form?.FindFirstDescendant(cf => cf.ByText("Анализировать"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var recBox = form?.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(recBox);
            Assert.IsTrue(recBox.AsTextBox().Text.Contains("Опыт работы"));
        }

        [TestMethod]
        public void TC_LAB5_016_Analyze_WithNoEducation_ShouldShowRecommendation()
        {
            CreateTestResumeWithSkills("Тест", "t@t.com", "Цель с более чем двадцатью символами", new[] { "C#", "Git", "SQL" });
            CreateTestJobListing("Dev", "Corp", "Desc", "C#, Git, SQL");
            ClickButtonByText("Анализ резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Анализ соответствия резюме вакансии"))?.AsWindow());
            var listResumes = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[0];
            var listJobs = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List))[1];
            Assert.IsNotNull(listResumes);
            Assert.IsNotNull(listJobs);
            listResumes.AsListBox().Items[0].Click();
            listJobs.AsListBox().Items[0].Click();
            form?.FindFirstDescendant(cf => cf.ByText("Анализировать"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var recBox = form?.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(recBox);
            Assert.IsTrue(recBox.AsTextBox().Text.Contains("Образование"));
        }

        [TestMethod]
        public void TC_LAB5_017_SaveReport_ShouldCreateFile()
        {
            CreateTestResumeWithSkills("Иван", "ivan@test.com", "Цель с более чем двадцатью символами", new[] { "C#" });
            CreateTestJobListing("Developer", "TechCorp", "Описание", "C#");
            ClickButtonByText("Анализ резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Анализ соответствия резюме вакансии"))?.AsWindow(), timeoutMs: 10000);
            Assert.IsNotNull(form, "Форма анализа не открылась");
            var allLists = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.List));
            Assert.IsNotNull(allLists);
            Assert.IsTrue(allLists.Length >= 2);
            var listResumes = allLists[0].AsListBox();
            var listJobs = allLists[1].AsListBox();
            var resumeItem = listResumes.Items[0];
            resumeItem.Click();
            var jobItem = listJobs.Items[0];
            jobItem.Click();
            Thread.Sleep(200);
            form?.FindFirstDescendant(cf => cf.ByText("Анализировать"))?.AsButton()?.Click();
            Thread.Sleep(300);
            form?.FindFirstDescendant(cf => cf.ByText("Сохранить отчёт"))?.AsButton()?.Click();
            Thread.Sleep(1000);

            var saveDialog = WaitForElement(() =>
                _automation?.GetDesktop().FindFirstDescendant(cf =>
                    cf.ByControlType(ControlType.Window)
                      .And(cf.ByName("Сохранить отчёт анализа", PropertyConditionFlags.MatchSubstring)
                           .Or(cf.ByAutomationId("1120"))))?.AsWindow(),
                timeoutMs: 15000);

            if (saveDialog != null)
            {
                var fileNameBox = saveDialog.FindFirstDescendant(cf =>
                    cf.ByControlType(ControlType.ComboBox).Or(cf.ByControlType(ControlType.Edit))
                    .And(cf.ByAutomationId("1148").Or(cf.ByName("Имя файла:", PropertyConditionFlags.MatchSubstring))));

                if (fileNameBox != null)
                {
                    fileNameBox.Click();
                    Thread.Sleep(200);
                    Keyboard.Press(VirtualKeyShort.CONTROL);
                    Keyboard.Release(VirtualKeyShort.CONTROL);
                    Keyboard.Press(VirtualKeyShort.DELETE);
                    Keyboard.Type("Test.txt");
                }
                Thread.Sleep(300);

                var saveBtn = saveDialog.FindFirstDescendant(cf =>
                    cf.ByControlType(ControlType.Button)
                      .And(cf.ByName("Сохранить").Or(cf.ByAutomationId("1"))));
                saveBtn?.Click();
                Thread.Sleep(1000);
            }
            else
            {
                Thread.Sleep(2000);
                Keyboard.Press(VirtualKeyShort.ENTER);
                Thread.Sleep(1000);
            }

            var successMsg = WaitForElement(() =>
            {
                var win = _automation?.GetDesktop().FindFirstDescendant(cf =>
                    cf.ByControlType(ControlType.Window)
                      .And(cf.ByName("Успех", PropertyConditionFlags.MatchSubstring)
                           .Or(cf.ByName("ResumeManager", PropertyConditionFlags.MatchSubstring))));
                if (win != null)
                {
                    var txt = win.FindFirstDescendant(cf =>
                        cf.ByControlType(ControlType.Text)
                          .And(cf.ByName("Отчёт успешно сохранён.", PropertyConditionFlags.MatchSubstring)));
                    if (txt != null) return win;
                }
                return null;
            }, timeoutMs: 15000);

            Assert.IsNotNull(successMsg, "Сообщение 'Отчёт успешно сохранён.' не найдено");

            var okBtn = successMsg?.FindFirstDescendant(cf =>
                cf.ByControlType(ControlType.Button)
                  .And(cf.ByName("OK").Or(cf.ByAutomationId("2"))));
            okBtn?.Click();
            Thread.Sleep(300);
        }

        [TestMethod]
        public void TC_LAB5_018_SaveReport_WithoutAnalysis_ShouldShowWarning()
        {
            CreateTestResume("Тест", "t@t.com", "Цель");
            CreateTestJobListing("Dev", "Corp", "Desc", "C#");
            ClickButtonByText("Анализ резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Анализ соответствия резюме вакансии"))?.AsWindow());
            form?.FindFirstDescendant(cf => cf.ByText("Сохранить отчёт"))?.AsButton()?.Click();
            Thread.Sleep(300);
            var warning = WaitForMessageBox("Сначала выполните анализ.", 10000);
            Assert.IsNotNull(warning);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB5_019_OpenAnalysis_WithNoResumes_ShouldShowMessage()
        {
            CreateTestJobListing("Dev", "Corp", "Desc", "C#");
            ClickButtonByText("Анализ резюме");
            Thread.Sleep(300);
            var msg = WaitForMessageBox("Список резюме пуст.", 10000);
            Assert.IsNotNull(msg);
            CloseMessageBox();
        }

        [TestMethod]
        public void TC_LAB5_020_OpenAnalysis_WithNoJobs_ShouldShowMessage()
        {
            CreateTestResume("Тест", "t@t.com", "Цель");
            ClickButtonByText("Анализ резюме");
            Thread.Sleep(300);
            var msg = WaitForMessageBox("Список вакансий пуст.", 10000);
            Assert.IsNotNull(msg);
            CloseMessageBox();
        }

        private void CreateTestResume(string name, string contact, string objective)
        {
            ClickButtonByText("Создать резюме");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Создать резюме"))?.AsWindow());
            Assert.IsNotNull(form);
            var edits = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(edits);
            var nameBox = edits[0].AsTextBox(); nameBox.Text = name;
            var contactBox = edits[1].AsTextBox(); contactBox.Text = contact;
            var objectiveBox = edits[2].AsTextBox(); objectiveBox.Text = objective;
            var okBtn = form?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton();
            Assert.IsNotNull(okBtn);
            okBtn.Click();
            Thread.Sleep(300);
            CloseMessageBox();
        }

        private void CreateTestResumeWithFullData(string name, string contact, string objective, string[] skills, string position, string company, string period, string description, string institution, string degree, string eduPeriod)
        {
            CreateTestResume(name, contact, objective);
            foreach (var skill in skills) AddSkillToResume(skill);
            AddWorkExperience(position, company, period, description);
            AddEducation(institution, degree, eduPeriod);
        }

        private void CreateTestResumeWithSkills(string name, string contact, string objective, string[] skills)
        {
            CreateTestResume(name, contact, objective);
            foreach (var skill in skills) AddSkillToResume(skill);
        }

        private void AddSkillToResume(string skill)
        {
            ClickButtonByText("Добавить навык");
            Assert.IsTrue(SelectResumeFromDialog(), "Не удалось выбрать резюме для добавления навыка");
            var skillForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByControlType(ControlType.Window).And(cf.ByName("Добавить навык", PropertyConditionFlags.MatchSubstring)))?.AsWindow(), timeoutMs: 10000);
            Assert.IsNotNull(skillForm);
            var skillElement = skillForm?.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
            if (skillElement != null)
            {
                var textBox = skillElement.AsTextBox();
                if (textBox != null) textBox.Text = skill;
            }
            skillForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(400);
            CloseMessageBox();
        }

        private void AddWorkExperience(string position, string company, string period, string description)
        {
            ClickButtonByText("Добавить опыт");
            Assert.IsTrue(SelectResumeFromDialog(), "Не удалось выбрать резюме для добавления опыта");
            var expForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByControlType(ControlType.Window).And(cf.ByName("Добавить опыт работы", PropertyConditionFlags.MatchSubstring)))?.AsWindow(), timeoutMs: 10000);
            Assert.IsNotNull(expForm);
            var edits = expForm?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            if (edits != null && edits.Length >= 4)
            {
                var posBox = edits[0].AsTextBox();
                var compBox = edits[1].AsTextBox();
                var perBox = edits[2].AsTextBox();
                var descBox = edits[3].AsTextBox();
                if (posBox != null) posBox.Text = position;
                if (compBox != null) compBox.Text = company;
                if (perBox != null) perBox.Text = period;
                if (descBox != null) descBox.Text = description;
            }
            expForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(400);
            CloseMessageBox();
        }

        private void AddEducation(string institution, string degree, string period)
        {
            ClickButtonByText("Добавить образование");
            Assert.IsTrue(SelectResumeFromDialog(), "Не удалось выбрать резюме для добавления образования");
            var eduForm = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByControlType(ControlType.Window).And(cf.ByName("Добавить образование", PropertyConditionFlags.MatchSubstring)))?.AsWindow(), timeoutMs: 10000);
            Assert.IsNotNull(eduForm);
            var edits = eduForm?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            if (edits != null && edits.Length >= 3)
            {
                var instBox = edits[0].AsTextBox();
                var degBox = edits[1].AsTextBox();
                var perBox = edits[2].AsTextBox();
                if (instBox != null) instBox.Text = institution;
                if (degBox != null) degBox.Text = degree;
                if (perBox != null) perBox.Text = period;
            }
            eduForm?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(400);
            CloseMessageBox();
        }

        private void CreateTestJobListing(string title, string company, string description, string requirements)
        {
            ClickButtonByText("Добавить вакансию");
            var form = WaitForElement(() => _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByText("Добавить вакансию"))?.AsWindow());
            Assert.IsNotNull(form);
            var edits = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
            if (edits != null && edits.Length >= 2)
            {
                edits[0].AsTextBox().Text = title;
                edits[1].AsTextBox().Text = company;
            }
            var panels = form?.FindAllDescendants(cf => cf.ByControlType(ControlType.Pane));
            if (panels != null && panels.Length >= 2)
            {
                var descElement = panels[0].FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
                if (descElement != null) descElement.AsTextBox().Text = description;
                var reqElement = panels[1].FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
                if (reqElement != null) reqElement.AsTextBox().Text = requirements;
            }
            form?.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()?.Click();
            Thread.Sleep(300);
            CloseMessageBox();
        }

        private Window? GetModalWindowByTitle(string titlePart, int timeoutMs = 10000)
        {
            var start = DateTime.Now;
            while ((DateTime.Now - start).TotalMilliseconds < timeoutMs)
            {
                var modal = _mainWindow?.ModalWindows.FirstOrDefault(w => w.Name.Contains(titlePart, StringComparison.OrdinalIgnoreCase))?.AsWindow();
                if (modal != null)
                {
                    modal.SetForeground();
                    return modal;
                }
                var desktopModal = _automation?.GetDesktop().FindFirstDescendant(cf => cf.ByControlType(ControlType.Window).And(cf.ByName(titlePart, PropertyConditionFlags.MatchSubstring)))?.AsWindow();
                if (desktopModal != null)
                {
                    desktopModal.SetForeground();
                    return desktopModal;
                }
                Thread.Sleep(100);
            }
            return null;
        }

        private bool SelectListItem(FlaUIListBox listBox, int index)
        {
            if (listBox == null || index < 0 || index >= listBox.Items.Length) return false;
            var item = listBox.Items[index];
            item.Focus();
            Thread.Sleep(100);
            try
            {
                item.Click();
                return true;
            }
            catch
            {
                if (item.Patterns.Invoke.IsSupported)
                {
                    var invokePattern = item.Patterns.Invoke.Pattern;
                    invokePattern?.Invoke();
                    return true;
                }
                return false;
            }
        }

        private bool SelectResumeFromDialog(string resumeNamePart = null, int timeoutMs = 10000)
        {
            var selectForm = GetModalWindowByTitle("резюме", timeoutMs);
            if (selectForm == null) return false;
            Thread.Sleep(300);
            var list = selectForm.FindFirstDescendant(cf => cf.ByControlType(ControlType.List))?.AsListBox();
            if (list == null || list.Items.Length == 0) return false;
            int selectedIndex = 0;
            if (!string.IsNullOrEmpty(resumeNamePart))
            {
                for (int i = 0; i < list.Items.Length; i++)
                {
                    if (list.Items[i].Name.Contains(resumeNamePart, StringComparison.OrdinalIgnoreCase))
                    {
                        selectedIndex = i;
                        break;
                    }
                }
            }
            if (!SelectListItem(list, selectedIndex)) return false;
            Thread.Sleep(200);
            var okBtn = selectForm.FindFirstDescendant(cf => cf.ByText("OK").Or(cf.ByText("ОК")))?.AsButton();
            if (okBtn == null) okBtn = selectForm.FindFirstDescendant(cf => cf.ByAutomationId("1"))?.AsButton();
            if (okBtn == null) okBtn = selectForm.FindAllDescendants(cf => cf.ByControlType(ControlType.Button)).FirstOrDefault(b => b.Name == "OK" || b.Name == "ОК")?.AsButton();
            if (okBtn != null)
            {
                okBtn.Click();
                Thread.Sleep(300);
                return true;
            }
            return false;
        }
    }
}