namespace InstallerApp.Composant;


[QueryProperty(nameof(Techno), "Techno")]
[QueryProperty(nameof(Framework), "Framework")]
public partial class Page : ContentPage
{
    private ContentView _contentView;
    private string? _techno;
    public string? Techno
    {
        get => _techno;
        set
        {
            try
            {
                _techno = value;
                OnPropertyChanged();
                InitializeWithTechno();
            }
            catch { }
        }
    }
    private string? _framework;
    public string? Framework
    {
        get => _framework;
        set
        {
            try
            {
                _framework = value;
                OnPropertyChanged();
                InitializeWithFramework();
            }
            catch { }
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
        //InstallTechno? _cc = _contentView as InstallTechno;

        //if (_cc != null)
        //    _cc?.Fr(Techno);
        //else
        //{
            SelectFramework? _cc2 = _contentView as SelectFramework;
            _cc2?.setTechno(Techno);
        //}
    }
    private void InitializeWithFramework()
    {
        if (string.IsNullOrEmpty(Framework))
            return;
        InstallTechno? _cc = _contentView as InstallTechno;

        if (_cc != null)
            _cc?.setFramework(Framework);
    }
}