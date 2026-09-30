using System.Diagnostics;
using System.IO;
using System.Windows;
using ConvertEasy.Services;
using Microsoft.Win32;

namespace ConvertEasy;

public partial class MainWindow : Window
{
    private FileKind _currentKind;
    private string? _selectedFilePath;
    private string? _resultFolder;
    private bool _isConverting;
    private readonly VideoConversionService _videoConversionService = new();

    public MainWindow()
    {
        InitializeComponent();
    }

    private void VideoOption_Click(object sender, RoutedEventArgs e) => ShowSelection(FileKind.Video);

    private void PdfOption_Click(object sender, RoutedEventArgs e) => ShowSelection(FileKind.Pdf);

    private void ShowSelection(FileKind kind)
    {
        _currentKind = kind;
        _selectedFilePath = null;
        _resultFolder = null;
        SectionTitle.Text = kind == FileKind.Video ? "Vídeo" : "PDF";
        SelectFileButton.Content = kind == FileKind.Video ? "Selecionar vídeo" : "Selecionar PDF";
        SelectedFileText.Text = "Nenhum arquivo selecionado";
        SelectedFileText.ToolTip = null;
        StatusText.Text = kind == FileKind.Video
            ? "Selecione um vídeo para começar."
            : "A conversão de PDF estará disponível em uma próxima versão.";
        StatusText.ToolTip = null;
        ConvertButton.IsEnabled = false;
        OpenFolderButton.Visibility = Visibility.Collapsed;
        HomePanel.Visibility = Visibility.Collapsed;
        SelectionPanel.Visibility = Visibility.Visible;
    }

    private void SelectFile_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = _currentKind == FileKind.Video ? "Selecionar vídeo" : "Selecionar PDF",
            Filter = _currentKind == FileKind.Video
                ? "Arquivos de vídeo|*.mp4;*.mov;*.avi;*.mkv;*.wmv;*.webm;*.m4v;*.mpg;*.mpeg;*.mts;*.m2ts"
                : "Arquivos PDF|*.pdf",
            Multiselect = false,
            CheckFileExists = true
        };

        if (dialog.ShowDialog(this) == true)
        {
            _selectedFilePath = dialog.FileName;
            SelectedFileText.Text = Path.GetFileName(_selectedFilePath);
            SelectedFileText.ToolTip = _selectedFilePath;
            _resultFolder = null;
            OpenFolderButton.Visibility = Visibility.Collapsed;
            StatusText.ToolTip = null;

            if (_currentKind == FileKind.Video)
            {
                ConvertButton.IsEnabled = VideoConversionService.IsSupportedVideo(_selectedFilePath);
                StatusText.Text = ConvertButton.IsEnabled
                    ? "Vídeo pronto para converter."
                    : "Não foi possível selecionar este vídeo. Escolha outro arquivo.";
            }
        }
    }

    private async void Convert_Click(object sender, RoutedEventArgs e)
    {
        if (_currentKind != FileKind.Video ||
            _selectedFilePath is null ||
            !VideoConversionService.IsSupportedVideo(_selectedFilePath) ||
            _isConverting)
        {
            return;
        }

        _isConverting = true;
        SelectFileButton.IsEnabled = false;
        ConvertButton.IsEnabled = false;
        BackButton.IsEnabled = false;
        OpenFolderButton.Visibility = Visibility.Collapsed;
        ConversionActivity.Visibility = Visibility.Visible;
        StatusText.Text = "Convertendo vídeo. Aguarde...";
        StatusText.ToolTip = null;

        try
        {
            var outputPath = await _videoConversionService.ConvertAsync(_selectedFilePath);
            _resultFolder = Path.GetDirectoryName(outputPath);
            StatusText.Text = "Conversão concluída com sucesso.";
            OpenFolderButton.Visibility = Visibility.Visible;
        }
        catch (VideoConversionException exception)
        {
            StatusText.Text = exception.Message;
            StatusText.ToolTip = string.IsNullOrEmpty(exception.LogPath)
                ? null
                : $"Detalhes para diagnóstico: {exception.LogPath}";
        }
        finally
        {
            _isConverting = false;
            SelectFileButton.IsEnabled = true;
            BackButton.IsEnabled = true;
            ConvertButton.IsEnabled = _selectedFilePath is not null &&
                                      VideoConversionService.IsSupportedVideo(_selectedFilePath);
            ConversionActivity.Visibility = Visibility.Collapsed;
        }
    }

    private void OpenFolder_Click(object sender, RoutedEventArgs e)
    {
        if (_resultFolder is null || !Directory.Exists(_resultFolder))
        {
            StatusText.Text = "Não foi possível localizar a pasta Resultado.";
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo(_resultFolder) { UseShellExecute = true });
        }
        catch (Exception)
        {
            StatusText.Text = "Não foi possível abrir a pasta Resultado.";
        }
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        SelectionPanel.Visibility = Visibility.Collapsed;
        HomePanel.Visibility = Visibility.Visible;
        SelectedFileText.ToolTip = null;
    }

    private enum FileKind
    {
        Video,
        Pdf
    }
}
