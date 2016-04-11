using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Web.Areas.Consulta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Areas.Consulta.Controllers
{
    public class GeocercaController : Rp3.AgendaComercial.Web.Controllers.AgendaComercialController
    {
        // GET: Consulta/Geocerca

        [Rp3.Web.Mvc.Authorize("GEOCERCA", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            InicializarEdit();

            var model = new Geocerca();
            model.GeoZonas = new List<Zona>();
            model.Zonas = GetZonaListDetail(((List<Zona>)ViewBag.Zonas).Select(p => p.IdZona.ToString()).ToArray<string>());//new List<UbicacionAgenteDetalle>();

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("GEOCERCA", "QUERY", "AGENDACOMERCIAL")]
        [HttpPost]
        public ActionResult Index(Geocerca model, string[] zonas)
        {
            InicializarEdit();

            model.Zonas = GetZonaListDetail(zonas);

            try
            {
                var idZonas = model.Zonas.Select(p => p.IdZona).ToArray<int>();

                var geoZonas = DataBase.Zonas.Get(p => p.Tipo == Rp3.AgendaComercial.Models.Constantes.TipoZona.Geocerca &&
                    p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo && idZonas.Contains(p.IdZona), 
                    includeProperties: "ZonaGeocercas").ToList();

                model.GeoZonas = geoZonas;

                return View(model);
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            model.GeoZonas = new List<Zona>();
            return View(model);
        }

        public List<GeocercaZonaDetalle> GetZonaListDetail(string[] zonas)
        {
            List<GeocercaZonaDetalle> listEdit = new List<GeocercaZonaDetalle>();

            if (zonas != null)
            {
                foreach (var insert in zonas.Where(p => p != "false"))
                {
                    string[] keyParts = insert.Split('-');

                    GeocercaZonaDetalle detalle = new GeocercaZonaDetalle()
                    {
                        IdZona = Convert.ToInt32(keyParts[0])
                    };

                    listEdit.Add(detalle);
                }
            }

            return listEdit;
        }

        private void InicializarEdit()
        {
            List<Zona> zonas = DataBase.Zonas.Get(p => p.Tipo == Rp3.AgendaComercial.Models.Constantes.TipoZona.Geocerca &&
                p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo).ToList();

            //SetMarkers(zonas);

            ViewBag.Zonas = zonas;
        }
    }
}