using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace ConversorHolyrics;

public partial class MainWindow : Window
{
    private FileKind _currentKind;
    private string? _selectedFilePath;

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
        SectionTitle.Text = kind == FileKind.Video ? "Vídeo" : "PDF";
        SelectFileButton.Content = kind == FileKind.Video ? "Selecionar vídeo" : "Selecionar PDF";
        SelectedFileText.Text = "Nenhum arquivo selecionado";
        SelectedFileText.ToolTip = null;
        HomePanel.Visibility = Visibility.Collapsed;
        SelectionPanel.Visibility = Visibility.Visible;
    }

    private void SelectFile_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = _currentKind == FileKind.Video ? "Selecionar vídeo" : "Selecionar PDF",
            Filter = _currentKind == FileKind.Video
                ? "Arquivos de vídeo|*.mp4;*.mov;*.avi;*.mkv;*.wmv;*.webm;*.m4v|Todos os arquivos|*.*"
                : "Arquivos PDF|*.pdf",
            Multiselect = false,
            CheckFileExists = true
        };

        if (dialog.ShowDialog(this) == true)
        {
            _selectedFilePath = dialog.FileName;
            SelectedFileText.Text = Path.GetFileName(_selectedFilePath);
            SelectedFileText.ToolTip = _selectedFilePath;
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
