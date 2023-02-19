using System.Xml;

namespace TrackingMarques;

public partial class MainPage : ContentPage
{
    XmlDocument doc { get; set; }
    XmlElement punts;
    int numeroDePunts = 0;
    private CancellationTokenSource _cancelTokenSource;

    public MainPage()
    {
        numeroDePunts = 0;
        doc = new XmlDocument();
        InitializeComponent();
        LabelPunts.Text = $"Punts introduïts: 0";
    }
    private async void IniciBtn_Clicked(object sender, EventArgs e)
    {
        doc = new XmlDocument();
        numeroDePunts = 0;
        LabelPunts.Text = $"Punts introduïts: {numeroDePunts}";
        XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        XmlElement root = doc.DocumentElement;
        doc.InsertBefore(xmlDeclaration, root);
        punts = doc.CreateElement(string.Empty, "punts", string.Empty);
        doc.AppendChild(punts);
        await anyadirWPT("Punt Inici de la ruta");
        PuntInteresBtn.IsEnabled = true;
    }

    private async void PuntInteresBtn_Clicked(object sender, EventArgs e)
    {
        IniciBtn.IsEnabled = false;
        PuntInteresBtn.IsEnabled = false;
        FinalBtn.IsEnabled = false;
        await anyadirWPT(PuntInteresEntry.Text);
        PuntInteresEntry.Text = null;
        IniciBtn.IsEnabled = true;
        PuntInteresBtn.IsEnabled = true;
        FinalBtn.IsEnabled = true;
    }

    private async void FinalBtn_Clicked(object sender, EventArgs e)
    {
        await anyadirWPT("Final de la ruta");
        PuntInteresBtn.IsEnabled = false;
        FinalBtn.IsEnabled = false;
        string ruta = "/storage/emulated/0/Documents/";
        string fichero = "ruta";
        string extension = "xml";
        int numeroFichero = 0;
        if (File.Exists($"{ruta}{fichero}.{extension}"))
        {
            numeroFichero++;
            while (File.Exists($"{ruta}{fichero} ({numeroFichero}).{extension}"))
            {
                numeroFichero++;
            }
            doc.Save($"{ruta}{fichero} ({numeroFichero}).{extension}");
        }
        else
        {
            doc.Save($"{ruta}{fichero}.{extension}");
        }
    }

    private async Task anyadirWPT(string nombre)
    {
        GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(5));

        _cancelTokenSource = new CancellationTokenSource();

        Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

        XmlElement punt = doc.CreateElement(string.Empty, "punt", string.Empty);

        XmlElement dataHora = doc.CreateElement(string.Empty, "dataHora", string.Empty);
        string fechaXML = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
        XmlText textFecha = doc.CreateTextNode(fechaXML);

        XmlElement elevacio = doc.CreateElement(string.Empty, "elevacio", string.Empty);
        XmlText textElevacio = doc.CreateTextNode(location.Altitude?.ToString());
        elevacio.AppendChild(textElevacio);

        XmlElement latitut = doc.CreateElement(string.Empty, "latitut", string.Empty);
        XmlText textLatitut = doc.CreateTextNode(location.Latitude.ToString());
        latitut.AppendChild(textLatitut);

        XmlElement longitut = doc.CreateElement(string.Empty, "longitut", string.Empty);
        XmlText textLongitut = doc.CreateTextNode(location.Longitude.ToString());
        longitut.AppendChild(textLongitut);

        XmlElement nom = doc.CreateElement(string.Empty, "nom", string.Empty);
        XmlText textNom = doc.CreateTextNode(nombre);


        nom.AppendChild(textNom);
        punt.AppendChild(nom);
        punt.AppendChild(latitut);
        punt.AppendChild(longitut);
        punt.AppendChild(elevacio);
        dataHora.AppendChild(textFecha);
        punt.AppendChild(dataHora);
        punts.AppendChild(punt);
        numeroDePunts++;
        LabelPunts.Text = $"Punts introduïts: {numeroDePunts}";
    }
}

