namespace InstallerApp.Composant;


[QueryProperty(nameof(Techno), "Techno")]
public partial class Page : ContentPage
{
    private ContentView _contentView;
    private string? _techno;
    public string? Techno
    {
        get => _techno;
        set
        {
            _techno = value;
            OnPropertyChanged();
            InitializeWithTechno();
        }
    }
    public Page(ContentView cnt)
	{
		InitializeComponent();
        _contentView = cnt;
		Frame.Content = cnt;
    }
    private void InitializeWithTechno()
    {
        if (string.IsNullOrEmpty(Techno))
            return;
        InstallTechno? _cc = _contentView as InstallTechno;
        _cc?.setTechno(Techno);
    }
}