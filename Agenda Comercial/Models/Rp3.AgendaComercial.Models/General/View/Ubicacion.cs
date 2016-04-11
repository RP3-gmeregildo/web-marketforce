using Rp3.AgendaComercial.Models.Ruta;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Rp3.AgendaComercial.Models.General
{
    public interface IUbicacion
    {
        int IdUbicacion { get; set; }
        double? Latitud { get; set; }
        double? Longitud { get; set; }
        string Titulo { get; }
        int MarkerIndex { get; set; }
        int MarkerZIndex { get; }
        int MarkerStart { get; }
    }

    public class Ubicacion : IUbicacion
    {
        public static double DefaultLatitud
        {
            get { return -2.139343; }
        }
        public static double DefaultLongitud
        {
            get { return -79.90108; }
        }
        public int IdUbicacion { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public string Titulo { get; set; }
        public int MarkerIndex { get; set; }
        public int MarkerZIndex { get { return 1000 - MarkerIndex; } }
        public int MarkerStart { get { if (MarkerIndex > 0) return 30 * (MarkerIndex - 1); else return 0; } }

        const double PIx = Math.PI;
        const double RADIO = 6378.16;

        public bool ReadOnly { get; set; }

        public string Color { get; set; }

        public static List<Ubicacion> Recorrido(List<AgenteUbicacion> list, double routedDistance)
        {
            List<Ubicacion> model = new List<Ubicacion>();

            for (int i = list.Count - 1; i > 0; i--)
            {
                var actual = list[i];
                var anterior = list[i - 1];

                if (actual.Latitud == anterior.Latitud && actual.Longitud == anterior.Longitud)
                {
                    if (actual.FechaHasta == null)
                        anterior.FechaHasta = actual.Fecha;
                    else
                        anterior.FechaHasta = actual.FechaHasta;

                    list.Remove(actual);
                }
                else
                {
                    var distance = Ubicacion.Distance(actual.Longitud ?? 0, actual.Latitud ?? 0, anterior.Longitud ?? 0, anterior.Latitud ?? 0);

                    if (distance < routedDistance)//0.050)
                    {
                        if (actual.FechaHasta == null)
                            anterior.FechaHasta = actual.Fecha;
                        else
                            anterior.FechaHasta = actual.FechaHasta;

                        list.Remove(actual);
                    }
                }
            }

            foreach (var item in list)
            {
                string titulo = String.Empty;
                string tiempo = String.Empty;

                if (item.FechaHasta == null || item.Fecha.ToString("HH:mm") == item.FechaHasta.Value.ToString("HH:mm"))
                    titulo = item.Fecha.ToString("HH:mm");
                else
                {
                    titulo = String.Format("{0} - {1}", item.Fecha.ToString("HH:mm"), item.FechaHasta.Value.ToString("HH:mm"));

                    var dif = item.FechaHasta.Value - item.Fecha;

                    if (dif.Hours > 0)
                        tiempo = String.Format("{0} H.", dif.Hours);

                    if (dif.Minutes > 0)
                    {
                        if (!String.IsNullOrEmpty(tiempo))
                            tiempo += ", ";

                        tiempo += String.Format("{0} Min.", dif.Minutes);
                    }

                    if (!String.IsNullOrEmpty(tiempo))
                        titulo = String.Format("{0} ({1})", titulo, tiempo);
                }

                model.Add(new Ubicacion()
                {
                    Titulo = titulo,
                    Latitud = item.Latitud,
                    Longitud = item.Longitud
                });
            }

            SetMarkers(model);

            return model;
        }

        public static void SetMarkers(IEnumerable<IUbicacion> ubicaciones)
        {
            int markerIndex = 1;

            foreach (var item in ubicaciones)
            {
                item.MarkerIndex = markerIndex;
                markerIndex++;

                if (markerIndex >= 100)
                    markerIndex = 100;
            }
        }

        public static double Radians(double x)
        {
            return x * PIx / 180;
        }

        public static double Distance(double lon1, double lat1, double lon2, double lat2)
        {
            double R = 6371; // km
            double dLat = Radians(lat2 - lat1);
            double dLon = Radians(lon2 - lon1);
            lat1 = Radians(lat1);
            lat2 = Radians(lat2);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c;

            return d;
        }

        public static string ToStaticMap(List<Ubicacion> model, dynamic viewBag)
        {
            StringBuilder uri = new StringBuilder("https://maps.googleapis.com/maps/api/staticmap?");

            int count = model.Where(p => p.Latitud != null && p.Longitud != null && p.Latitud != 0 && p.Longitud != 0).Count();
            int countMarkerIndex = model.Where(p => p.Latitud != null && p.Longitud != null && p.Longitud != 0 && p.Latitud != 0 && p.MarkerIndex != 0).Count();
            var firstPoint = model.Where(p => p.Latitud != null && p.Longitud != null && p.Longitud != 0 && p.Latitud != 0 && p.MarkerIndex != 0).OrderBy(p => p.MarkerIndex).FirstOrDefault();
            var lastPoint = model.Where(p => p.Latitud != null && p.Longitud != null && p.Longitud != 0 && p.Latitud != 0 && p.MarkerIndex != 0).OrderBy(p => p.MarkerIndex).LastOrDefault();

            if (viewBag.MapRoute == null) viewBag.MapRoute = false;
            if (viewBag.MapStart == null) viewBag.MapStart = false;
            if (viewBag.MapRadiusMode == null) viewBag.MapRadiusMode = false;
            if (viewBag.MapRadius == null) viewBag.MapRadius = 0;
            if (viewBag.SuppressRouteMarkers == null) viewBag.SuppressRouteMarkers = false;
            if (viewBag.TravelMode == null) viewBag.TravelMode = "ROADMAP";
            if (viewBag.ShowMapTitle == null) viewBag.ShowMapTitle = false;
            if (viewBag.MapWidth == null) viewBag.MapWidth = 150;
            if (viewBag.MapHeight == null) viewBag.MapHeight = 100;
            if (viewBag.UrlBase == null) viewBag.UrlBase = "http://www.rp3marketforce.com";

            uri.AppendFormat("size={0}x{1}", viewBag.MapWidth, viewBag.MapHeight);

            if (viewBag.SuppressRouteMarkers)
            {
                if (firstPoint != null && !viewBag.MapRadiusMode) firstPoint.MarkerIndex = 1;
                if (lastPoint != null && !viewBag.MapRadiusMode) lastPoint.MarkerIndex = 1;
            }
            int zoom = 14;
            if (count == 1) uri.AppendFormat("&zoom={0}", zoom);
            uri.AppendFormat("&maptype={0}&format=png&visual_refresh=true&sensor=TRUE_OR_FALSE", viewBag.TravelMode);

            if (count == 0) uri.AppendFormat("&center={0},{1}&zoom={2}", Ubicacion.DefaultLatitud, Ubicacion.DefaultLongitud, zoom);

            foreach (var marker in model.Where(p => p.Latitud != null && p.Longitud != null && p.Longitud != 0 && p.Latitud != 0))
            {
                uri.Append("&markers=");
                if (!viewBag.SuppressRouteMarkers)
                {
                    if (viewBag.MapStart && marker == firstPoint) uri.AppendFormat("icon:{0}", string.Format("{0}{1}", viewBag.UrlBase, "/Content/AgendaComercial/img/style/markerstart.png"));
                }
                else
                {
                    if (marker == firstPoint && marker.MarkerIndex == 1)
                    {
                        if (!viewBag.MapRadiusMode)
                            uri.AppendFormat("icon:{0}", string.Format("{0}{1}", viewBag.UrlBase, "/Content/AgendaComercial/img/style/markerstart.png"));
                        else
                            uri.AppendFormat("icon:{0}", string.Format("{0}{1}", viewBag.UrlBase, "/Content/AgendaComercial/img/style/markeruser.png"));
                    }
                    else if (marker == lastPoint)
                    {
                        if (!viewBag.MapRadiusMode)
                            uri.AppendFormat("icon:{0}", string.Format("{0}{1}", viewBag.UrlBase, "/Content/AgendaComercial/img/style/markerend.png"));
                        else
                            uri.AppendFormat("icon:{0}", string.Format("{0}{1}", viewBag.UrlBase, "/Content/AgendaComercial/img/style/markervisit.png"));
                    }
                    else
                        uri.AppendFormat("icon:{0}", string.Format("{0}{1}", viewBag.UrlBase, "/Content/AgendaComercial/img/style/markerpoint.png"));
                }
                uri.AppendFormat("|{0},{1}", marker.Latitud, marker.Longitud);
                //uri.AppendFormat("{0},{1}", marker.Latitud, marker.Longitud);
            }
            if (count > 1)
            {
                uri.Append("&path=color:0x0000ff|weight:5");
                foreach (var marker in model.Where(p => p.Latitud != null && p.Longitud != null && p.Longitud != 0 && p.Latitud != 0))
                    uri.AppendFormat("|{0},{1}", marker.Latitud, marker.Longitud);
            }
            if (count == 1)
                uri.AppendFormat("&center={0},{1}", model[0].Latitud, model[0].Longitud);
            return uri.ToString();
        }

        public static Bitmap ToStaticMapImage(List<Ubicacion> model, dynamic viewBag)
        {
            //if (model == null || model.Count == 0) return new Bitmap(1, 1);
            string url = Ubicacion.ToStaticMap(model, viewBag);
            try
            {
                using (WebClient request = new WebClient())
                {
                    byte[] data = request.DownloadData(url);
                    Bitmap image = null;
                    using (Stream stream = new MemoryStream(data))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        image = Bitmap.FromStream(stream) as Bitmap;
                    }
                    return image;
                }
            }
            catch
            {
                return new Bitmap(viewBag.MapWidth, viewBag.MapHeight);
            }
        }
    }
}