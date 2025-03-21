using System.Diagnostics;
using System.Runtime.InteropServices;

namespace InstallerApp.Composant;

public partial class SelectFramework : ContentView
{
    public SelectFramework()
    {
        InitializeComponent();
    }
    public void setTechno(string techno)
    {
        try
        {
            Label_Techno.Text = techno;
            GenButton(techno);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creation: {ex.Message}");
        }
    }
    private void GenButton(string techno)
    {
        switch (techno)
        {
            case "Node":
                foreach(string titre in new List<string>() {"ExpressJs","React","Vite","ReactFlowbite"})
                {
                    layout.Add(CreateButton(techno, titre));
                }
                break;
            case "PHP":
                foreach (string titre in new List<string>() { "Symfony"})
                {
                    layout.Add(CreateButton(techno, titre));
                }
                break;
            case "Python":
                foreach (string titre in new List<string>() { "Flask","Django" })
                {
                    layout.Add(CreateButton(techno, titre));
                }
                break;

        }
    }
    private Button CreateButton(string techno, string title)
    {
        Button myButton = new Button
        {
            Text = title,
            FontSize = Device.GetNamedSize(NamedSize.Title, typeof(Button)),

        };
        myButton.Clicked += async (s, e) => { layout.Children.Clear(); await Shell.Current.GoToAsync($"//InstallTechno?Techno={techno}&Framework={title}"); };
        return myButton;
    }
    private async void Button_Retour(object sender, EventArgs e)
    {
        try
        {
            layout.Children.Clear();
            await Shell.Current.GoToAsync("//Installation");
        }
        catch (COMException ex)
        {
            Debug.WriteLine($"Error during navigation: {ex.Message}");
        }
    }
}
