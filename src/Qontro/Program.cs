using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

Console.WriteLine("Qontro Export v1.1.0");
Console.WriteLine("by Maximilian Goldacker - 2024\n\n");
Console.Write("Enter Qontro url or press enter for 'https://www14.qontro.com/': ");
var url = Console.ReadLine();

if (string.IsNullOrEmpty(url)) url = "https://www14.qontro.com/";

Console.Write("\nEnter username: ");
var user = Console.ReadLine();

Console.Write("\nEnter passwort: ");

var password = string.Empty;
ConsoleKey key;
do
{
    var keyInfo = Console.ReadKey(intercept: true);
    key = keyInfo.Key;

    if (key == ConsoleKey.Backspace && password.Length > 0)
    {
        Console.Write("\b \b");
        password = password[0..^1];
    }
    else if (!char.IsControl(keyInfo.KeyChar))
    {
        Console.Write("*");
        password += keyInfo.KeyChar;
    }
} while (key != ConsoleKey.Enter);

Console.WriteLine("\n\nStarting chrome...");
ChromeDriverService service = ChromeDriverService.CreateDefaultService();
service.SuppressInitialDiagnosticInformation = true;
IWebDriver driver = new ChromeDriver(service);

driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

Console.WriteLine("Choose an option: [1] - export creditors, [2] - export suppliers");
var choosedOption = Console.ReadKey();

var headers = new List<string>();
var exportRows = new List<List<string>>();
List<string> exportRow;
var headerInitialized = false;

Login();

switch (choosedOption.Key)
{
    case ConsoleKey.NumPad1:
    case ConsoleKey.D1:
        ExportCreditors();
        break;

    case ConsoleKey.NumPad2:
    case ConsoleKey.D2:
        ExportSuppliers();
        break;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"\nExport finished.");

void Login()
{
    driver.Navigate().GoToUrl(url);
    var submitButton = driver.FindElement(By.Name("Login"));
    var userField = driver.FindElement(By.Name("User__Name"));
    var passField = driver.FindElement(By.Name("User__Pass"));
    userField.SendKeys(user);
    passField.SendKeys(password);
    submitButton.Click();
}

void ExportCreditors()
{
    Console.WriteLine("\nNavigating to creditors...");
    var creditorsMenu = driver.FindElement(By.Id("Creditors"));
    creditorsMenu.Click();
    var enquiryButton = driver.FindElements(By.ClassName("OUTER_MENU_ITEM"))[1];
    enquiryButton.Click();
    var iSearchButton = driver.FindElement(By.Name("Search2"));
    iSearchButton.Click();

    Console.WriteLine("\nStarting export of creditors...");
    Thread.Sleep(2000);
    var rowsCount = driver.FindElements(By.TagName("tr")).Skip(2).ToList().Count;

    for (var i = 0; i < rowsCount; i++)
    {
        Console.WriteLine($"\nStarting export of creditor {i + 1} of {rowsCount}. Please wait...");
        exportRow = new List<string>();

        var row = driver.FindElements(By.TagName("tr")).Skip(2 + i).First();
        if (row.GetAttribute("class").Contains("footer")) continue;

        var columns = row.FindElements(By.TagName("td"));

        if (!headerInitialized)
        {
            headers.Add("Last Payment");
        }

        exportRow.Add(columns[2].Text);

        if (!headerInitialized)
        {
            headers.Add("Balance Owing");
        }

        exportRow.Add(columns[8].Text);

        var firstColumn = columns.First();
        var link = firstColumn.FindElement(By.TagName("a"));
        link.Click();

        var changesButton = driver.FindElement(By.Name("Show_Changes"));
        var isChangesButtonActive = changesButton.Enabled;
        changesButton.Click();

        if (isChangesButtonActive)
        {
            var changesTable = driver.FindElements(By.TagName("table"))[1];
            var changesRow = changesTable.FindElements(By.TagName("tr"));

            var lastUpdate = changesRow[changesRow.Count - 2];
            var lastUpdateColumns = lastUpdate.FindElements(By.TagName("td"));

            if (!headerInitialized)
            {
                headers.Add("Date And Time");
                headers.Add("User");
            }

            exportRow.Add(lastUpdateColumns[1].Text);
            exportRow.Add(lastUpdateColumns[3].Text);
        }

        else
        {
            exportRow.Add(string.Empty);
            exportRow.Add(string.Empty);
        }

        var maintainButton = driver.FindElement(By.Name("Maintain_Creditor"));
        maintainButton.Click();

        ExportForm();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

        headerInitialized = true;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nExported creditor {i + 1} of {rowsCount} successfully");
        Console.ForegroundColor = ConsoleColor.White;
        exportRows.Add(exportRow);

        if (isChangesButtonActive)
        {
            driver.Navigate().Back();
        }

        driver.Navigate().Back();
        driver.Navigate().Back();
    }

    Console.WriteLine("\nWriting export file...");

    var csv = File.Open("./creditors.csv", FileMode.Create);
    using var fw = new StreamWriter(csv);
    fw.WriteLine(string.Join(';', headers));

    foreach (var cred in exportRows)
    {
        fw.WriteLine(string.Join(";", cred));
    }

    fw.Flush();
    fw.Close();
}

void ExportSuppliers()
{
    Console.WriteLine("\nNavigating to suppliers...");
    var creditorsMenu = driver.FindElement(By.Id("Stock"));
    creditorsMenu.Click();
    var enquiryButton = driver.FindElements(By.ClassName("OUTER_MENU_ITEM"))[2];
    enquiryButton.Click();
    var iSearchButton = driver.FindElement(By.Name("Search2"));
    iSearchButton.Click();

    Console.WriteLine("\nStarting export of suppliers...");
    Thread.Sleep(2000);
    var rowsCount = driver.FindElements(By.TagName("tr")).Skip(2).ToList().Count;

    for (var i = 0; i < rowsCount; i++)
    {
        Console.WriteLine($"\nStarting export of supplier {i + 1} of {rowsCount}. Please wait...");
        exportRow = new List<string>();

        var row = driver.FindElements(By.TagName("tr")).Skip(2 + i).First();
        if (row.GetAttribute("class").Contains("footer")) continue;
        var columns = row.FindElements(By.TagName("td"));

        if (!headerInitialized)
        {
            headers.Add("Linked Stock");
        }

        exportRow.Add(columns.Last().Text);

        var firstColumn = columns.First();
        var link = firstColumn.FindElement(By.TagName("a"));
        link.Click();

        var maintainButton = driver.FindElement(By.Name("Maintain_Supplier"));
        maintainButton.Click();

        ExportForm();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

        headerInitialized = true;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nExported creditor {i + 1} of {rowsCount} successfully");
        Console.ForegroundColor = ConsoleColor.White;
        exportRows.Add(exportRow);
        driver.Navigate().Back();
        driver.Navigate().Back();
    }

    Console.WriteLine("\nWriting export file...");

    var csv = File.Open("./suppliers.csv", FileMode.Create);
    using var fw = new StreamWriter(csv);
    fw.WriteLine(string.Join(';', headers));

    foreach (var cred in exportRows)
    {
        fw.WriteLine(string.Join(";", cred));
    }

    fw.Flush();
    fw.Close();
}


void ExportForm()
{
    var table = driver.FindElement(By.TagName("table"));
    var rows = table.FindElements(By.TagName("tr"));

    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);

    foreach (var row in rows)
    {
        ExportFormRow(row);
    }
}

void ExportFormRow(IWebElement row)
{
    if (row.FindElements(By.TagName("table")).Count != 0) return;
    var columns = row.FindElements(By.TagName("td"));

    foreach (var column in columns)
    {
        ExportColumn(column);
    }
}

void ExportColumn(IWebElement column)
{
    if (column.GetAttribute("class") == "label" && !headerInitialized)
    {
        headers.Add(column.Text.Replace("\r\n", " "));
    }

    else
    {
        var elements = column.FindElements(By.XPath(".//*"));

        foreach (var element in elements)
        {
            if (element.GetAttribute("class") == "label" && !headerInitialized)
            {
                headers.Add(element.Text.Replace("\r\n", " "));
            }
            else if (element.TagName == "input" && element.GetAttribute("type") == "text")
            {
                exportRow.Add(element.GetAttribute("value"));
            }
            else if (element.TagName == "input" && element.GetAttribute("type") == "radio" && element.Selected)
            {
                exportRow.Add(element.GetAttribute("value"));
            }
            else if (element.TagName == "select")
            {
                var selectedOption = element.FindElements(By.TagName("option")).FirstOrDefault(o => o.Selected);
                exportRow.Add(selectedOption?.Text ?? string.Empty);
            }

            if (headers.Count < exportRow.Count) headers.Add(string.Empty);
        }
    }
}