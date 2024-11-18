using System.Text;
using CommunityToolkit.Maui.Storage;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Xsl;
using XMLManager.Parser;
using XMLManager.Person;

namespace GUI;

public partial class MainPage
{
	private const string SchemaFileName = "schema.xsd";
	private const string TransformSchemaFileName = "schema.xsl";

    private IParser _parser;

    private string _appDataDirectory;

    private FileResult _chosenFile;

    private readonly IFileSaver _fileSaver;

    private readonly IFilePicker _filePicker;

    private XmlReaderSettings _validationSettings;

    private XslCompiledTransform _exporter;

    public MainPage()
    {
        InitializeComponent();
        _fileSaver = FileSaver.Default;
        _filePicker = FilePicker.Default;

        ParserPicker.SelectedIndex = 0;
    }

    private async void ExitButton_Clicked(object sender, EventArgs e)
	{
		var option = await DisplayAlert("Confirm exit", "Are you sure you want to exit?", "Yes", "No");
		if (option)
		{
			Environment.Exit(0);
		}
	}

	private void ImportValidationSchema(string filepath)
	{
		var schema = new XmlSchemaSet();
		schema.Add("", filepath);

		_validationSettings = new XmlReaderSettings
		{
			Schemas = schema,
			ValidationType = ValidationType.Schema
		};
	}

	private async void OpenButton_Clicked(object sender, EventArgs e)
	{
		if (_validationSettings == null)
		{
			if (_appDataDirectory is null) {
				InitAppData();
			}

			if (_appDataDirectory != null && !Path.Exists(Path.Combine(_appDataDirectory, SchemaFileName)))
			{
				await CopyFileToAppDataDirectory(SchemaFileName);
			}

			ImportValidationSchema(Path.Combine(_appDataDirectory, SchemaFileName));
		}

		var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>> {
			{ DevicePlatform.MacCatalyst, ["xml"] }
		});

		var options = new PickOptions { PickerTitle = "Select xml file", FileTypes = customFileType };
		_chosenFile = await _filePicker.PickAsync(options);

		await ValidateFile();
	}

	private async void ExportButton_Clicked(object sender, EventArgs e)
	{
		if (_chosenFile == null)
		{
			await DisplayAlert("Error", "Input file is not chosen", "Ok");
			return;
		}

		if (_exporter == null)
		{
			if (!Path.Exists(Path.Combine(_appDataDirectory, TransformSchemaFileName)))
			{
				await CopyFileToAppDataDirectory(TransformSchemaFileName);
			}

			_exporter = new XslCompiledTransform();
			_exporter.Load(Path.Combine(_appDataDirectory, TransformSchemaFileName));
		}

		using var stream = new MemoryStream(Encoding.Default.GetBytes(""));
		var filename = _chosenFile.FileName.Split(".")[0] + ".html";

		var result = await _fileSaver.SaveAsync(filename, stream, new CancellationTokenSource().Token);

		try
		{
			_exporter.Transform(_chosenFile.FullPath, result.FilePath);
		}
		catch(Exception exception)
		{
			await DisplayAlert("Error", $"Failed to export file: {exception}", "Ok");
			return;
		}

		await DisplayAlert("Success", "File was successfully exported", "Ok");
	}

	private async void FindButton_Clicked(object sender, EventArgs e)
	{
		if (_chosenFile == null)
		{
			await DisplayAlert("Error", "Input file is not chosen", "Ok");
			return;
		}

		if (_parser == null)
		{
			await DisplayAlert("Error", "Parser type is not chosen", "Ok");
			return;
		}

		var filterOptions = CollectFilters();

		var results = _parser.Find(filterOptions);

		ClearResults();
		for (var i = 0; i < results.Count; i++)
		{
			var person = results[i];
			ResultGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

			CreateLabel(i, 0, person.Name.FirstName + " " + person.Name.LastName);
			CreateLabel(i, 1, person.Faculty);
			CreateLabel(i, 2, person.Department);
			CreateLabel(i, 3, person.Role);
			CreateLabel(i, 4, person.Salary);
			CreateLabel(i, 5, person.TimeTenure);
		}
	}

	private void ClearButton_Clicked(object sender, EventArgs e)
	{
		ClearFilters();
	}

	private async void Parser_Selected(object sender, EventArgs e)
	{
		_parser = ParserPicker.SelectedIndex switch
		{
			0 => new SaxParser(),
			1 => new DomParser(),
			2 => new LinqParser(),
			_ => throw new NotSupportedException("Parser type not supported")
		};

		await ValidateFile();
	}

	private Filters CollectFilters()
	{
		var filters = new Filters();
		if (NameCheckbox.IsChecked)
		{
			filters.FullName = NameEntry.Text;
		}

		if (FacultyCheckbox.IsChecked)
		{
			filters.Faculty = FacultyEntry.Text;
		}

		if (DepartmentCheckbox.IsChecked)
		{
			filters.Department = DepartmentEntry.Text;
		}

		if (RoleCheckbox.IsChecked)
		{
			filters.Role = RoleEntry.Text;
		}

		if (SalaryCheckbox.IsChecked)
		{
			filters.Salary = SalaryEntry.Text;
		}

		if (TimeTenureCheckbox.IsChecked)
		{
			filters.TimeTenure = TimeTenureEntry.Text;
		}

		return filters;
	}

	private void ClearFilters()
	{
		NameEntry.Text = "";
		NameCheckbox.IsChecked = false;
		DepartmentEntry.Text = "";
		DepartmentCheckbox.IsChecked = false;
		FacultyEntry.Text = "";
		FacultyCheckbox.IsChecked = false;
		RoleEntry.Text = "";
		RoleCheckbox.IsChecked = false;
		SalaryEntry.Text = "";
		SalaryCheckbox.IsChecked = false;
		TimeTenureEntry.Text = "";
		TimeTenureCheckbox.IsChecked = false;
	}

	private async Task ValidateFile()
	{
		if (_parser == null || _chosenFile == null)
		{
			return;
		}
		if (_parser.Load(await _chosenFile.OpenReadAsync(), _validationSettings))
		{
			return;
		}

		_chosenFile = null;
		await DisplayAlert("Invalid file", "The file does not satisfy XSD Schema", "Ok");
	}

	private void ClearResults()
	{
		ResultGrid.Children.Clear();
		ResultGrid.RowDefinitions.Clear();
	}

	private void CreateLabel(int row, int column, string text)
	{
		var label = new Label
		{
			Text = text,
			VerticalOptions = LayoutOptions.Center,
			HorizontalOptions = LayoutOptions.Center,
			FontSize = 17
		};
		ResultGrid.SetRow(label, row);
		ResultGrid.SetColumn(label, column);
		ResultGrid.Children.Add(label);
	}

	private async Task CopyFileToAppDataDirectory(string filename)
	{
		await using var inputStream = await FileSystem.Current.OpenAppPackageFileAsync(filename);

		var targetFile = Path.Combine(_appDataDirectory, filename);

		using FileStream outputStream = File.Create(targetFile);
		await inputStream.CopyToAsync(outputStream);
	}

	private void InitAppData()
	{
		_appDataDirectory = Path.Combine(FileSystem.AppDataDirectory);

		if (Path.Exists(_appDataDirectory)) return;

		while (!Path.Exists(_appDataDirectory))
		{
			_appDataDirectory = Directory.GetParent(_appDataDirectory)?.FullName;
		}

		_appDataDirectory = Path.Combine(_appDataDirectory, "Lab2");
		if (!Path.Exists(_appDataDirectory))
		{
			Directory.CreateDirectory(_appDataDirectory);
		}
	}
}