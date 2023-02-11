using System.Xml;

namespace TrackingMarques;

public partial class MainPage : ContentPage
{
    XmlDocument doc { get; set; }
    XmlElement gpx;
    private CancellationTokenSource _cancelTokenSource;

    public MainPage()
    {
        InitializeComponent();
        doc = new XmlDocument();

    }
    private async void IniciBtn_Clicked(object sender, EventArgs e)
    {
        doc = new XmlDocument();
        XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        XmlElement root = doc.DocumentElement;
        doc.InsertBefore(xmlDeclaration, root);
        gpx = doc.CreateElement(string.Empty, "gpx", string.Empty);
        gpx.SetAttribute("version", "1.1");
        gpx.SetAttribute("creator", "Institut Montilivi");
        gpx.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
        gpx.SetAttribute("xmlns", "http://www.topografix.com/GPX/1/1");
        gpx.SetAttribute("xsi:schemaLocation", "http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd");
        gpx.SetAttribute("xmlns:gpxtpx", "http://www.garmin.com/xmlschemas/TrackPointExtension/v1");
        doc.AppendChild(gpx);
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

        XmlElement wpt = doc.CreateElement(string.Empty, "wpt", string.Empty);
        wpt.SetAttribute("lat", location.Latitude.ToString());
        wpt.SetAttribute("lon", location.Longitude.ToString());


        XmlElement time = doc.CreateElement(string.Empty, "time", string.Empty);
        string fechaXML = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
        XmlText textFecha = doc.CreateTextNode(fechaXML);

        XmlElement ele = doc.CreateElement(string.Empty, "ele", string.Empty);
        XmlText textEle = doc.CreateTextNode("3.4");
        ele.AppendChild(textEle);

        XmlElement nom = doc.CreateElement(string.Empty, "nom", string.Empty);
        XmlText textNom = doc.CreateTextNode(nombre);
        nom.AppendChild(textNom);

        /*XmlElement extensions = doc.CreateElement(string.Empty, "extensions", string.Empty);
        XmlElement trackPointExtension = doc.CreateElement(string.Empty, "gpxtpx:TrackPointExtension", string.Empty);
        XmlElement hr = doc.CreateElement(string.Empty, "gpxtpx:hr", string.Empty);
        XmlText textHr = doc.CreateTextNode("171");
        hr.AppendChild(textHr);
        trackPointExtension.AppendChild(hr);
        extensions.AppendChild(trackPointExtension);


        wpt.AppendChild(extensions);*/
        wpt.AppendChild(nom);
        wpt.AppendChild(ele);
        time.AppendChild(textFecha);
        wpt.AppendChild(time);
        gpx.AppendChild(wpt);
    }
}

