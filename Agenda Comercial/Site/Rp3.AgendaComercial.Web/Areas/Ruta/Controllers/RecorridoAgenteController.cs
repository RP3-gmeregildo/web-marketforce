using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rp3.Data.Models.Security;
using Rp3.Web.Mvc.Application.Security.Models;
using Rp3.Web.Mvc;
using Rp3.AgendaComercial.Models.Ruta;
using Rp3.Web.Mvc.Html;
using Rp3.AgendaComercial.Models.General;

namespace Rp3.AgendaComercial.Web.Ruta.Controllers
{
    public class RecorridoAgenteController : AgendaComercial.Web.Controllers.AgendaComercialController
    {
        //
        // GET: /Ruta/AgenteRuta/

        [Rp3.Web.Mvc.Authorize("RECORRIDOAGENTE", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index(int? IdAgente, long? FechaInicioTicks, long? FechaFinTicks)
        {            
            var model = new RecorridoAgente();
            model.Ubicaciones = new List<Ubicacion>();
            model.IncluirRecorrido = true;
            Rp3.AgendaComercial.Web.Ruta.RecorridoAgenteResult result = new RecorridoAgenteResult();
            result.Clientes = new List<ClienteAgendaUbicacion>();
            result.Recorrido = new List<Ubicacion>();

            model.RecorridoResult = result;
            model.IdByAgente = IdAgente;
            
            if (IdAgente == null)
            {
                model.FechaInicio = GetCurrentDateTime().Date;// model.FechaFin.Value.AddDays(-30);
                if (Agente.EsAdministrador)
                    model.Agentes = DataBase.Agentes.Get(p =>
                        p.Estado == Models.Constantes.Estado.Activo).ToList();
                else
                    model.Agentes = DataBase.Agentes.Get(p =>
                        p.IdSupervisor == Agente.IdAgente &&
                        p.Estado == Models.Constantes.Estado.Activo).ToList();
            }
            else
            {
                model.Agentes = DataBase.Agentes.Get(p => p.IdAgente == IdAgente.Value).ToList();
                model.FechaInicio = new DateTime(FechaInicioTicks.Value);
                model.FechaInicioByAgente = new DateTime(FechaInicioTicks.Value).Date.ToString("dd/MM/yyyy");
                model.FechaFinByAgente = new DateTime(FechaFinTicks.Value).Date.ToString("dd/MM/yyyy");
            }

            return View(model);
        }

        [ChildAction]
        [Rp3.Web.Mvc.Authorize("RECORRIDOAGENTE", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult GetFechasRecorrido(int idAgente, DateTime? fechaInicioByAgente, DateTime? fechaFinByAgente)
        {
            List<DateTime> fechas = new List<DateTime>();
            DateTime fechaFin = GetCurrentDateTime().Date.AddDays(1).AddSeconds(-1);
            DateTime fechaInicio = fechaFin.AddDays(-30);
            if (fechaInicioByAgente != null)
            {
                fechaInicio = fechaInicioByAgente.Value;
                fechaFin = fechaFinByAgente.Value.Date.AddDays(1).AddSeconds(-1);
                fechas = DataBase.AgenteUbicaciones.GetFechasRecorrido(idAgente, fechaInicio, fechaFin).OrderBy(p => p).ToList();
            }
            else
                fechas = DataBase.AgenteUbicaciones.GetFechasRecorrido(idAgente, fechaInicio, fechaFin);

            return PartialView("_Fechas", fechas);
        }

        [Rp3.Web.Mvc.Authorize("RECORRIDOAGENTE", "QUERY", "AGENDACOMERCIAL")]
           
        public ActionResult GetUbicaciones(int? idAgente, long? fecha, bool incluirClientes, bool incluirRecorrido, bool incluirGestion)
        {
            RecorridoAgenteResult result = new RecorridoAgenteResult();

            result.IdAgente = idAgente;
            result.Clientes = new List<ClienteAgendaUbicacion>();
            result.Recorrido = new List<Ubicacion>();

            result.RadioDistancia = 100;
            result.Fecha = new DateTime(fecha ?? 0);
            result.IncluirGestion = incluirGestion;
            result.IncluirClientes = incluirClientes;
            result.IncluirRecorrido = incluirRecorrido;

            if (idAgente != null && fecha != null)
            {
                DateTime fechaConsulta = new DateTime(fecha ?? 0);

                try
                {
                    List<Ubicacion> model = new List<Ubicacion>();                                     

                    DateTime fechaFin = fechaConsulta.AddDays(1).Date.AddSeconds(-1);
                    DateTime fechaInicio = fechaConsulta.Date;

                    if (incluirRecorrido)
                    {
                        var agenteUbicaciones = DataBase.AgenteUbicaciones.Get(p =>
                            p.Fecha >= fechaInicio && p.Fecha <= fechaFin
                            && p.IdAgente == idAgente).OrderBy(p => p.Fecha).ToList();

                        DataBase.ParametroHelper.Load();

                        result.RadioDistancia = DataBase.ParametroHelper.VisitDistance;

                        model = Ubicacion.Recorrido(agenteUbicaciones, DataBase.ParametroHelper.RoutedDistance);

                        result.Recorrido.AddRange(model);
                    }

                    if (incluirClientes || incluirGestion)
                    {
                        var agente = DataBase.Agentes.GetByID(idAgente);
                        
                        if (agente != null && agente.IdRuta != null)
                        {
                            var agendas = DataBase.Agendas.Get(p=> p.IdRuta == agente.IdRuta
                                && p.FechaInicio >= fechaInicio && p.FechaInicio <= fechaFin
                                && p.EstadoAgenda != Models.Constantes.EstadoAgenda.Eliminado, includeProperties:
                                "ClienteDireccion.Cliente,EstadoAgendaGeneralValue,ClienteContacto");
                                                        

                            foreach (var item in agendas)
                            {
                                ClienteAgendaUbicacion app = new ClienteAgendaUbicacion()
                                {                                    
                                    Latitud = item.ClienteDireccion.Latitud,
                                    Longitud = item.ClienteDireccion.Longitud,
                                    MarkerIndex = 0,
                                    Estado = item.EstadoAgendaGeneralValue.Content,
                                    ColorEstado = item.EstadoAgendaGeneralValue.Reference01,
                                    Direccion = item.ClienteDireccion.DescriptionName,
                                    Fecha = item.FechaInicio.Value,
                                    FechaInicioGestion = item.FechaInicioGestion,
                                    FechaFinGestion = item.FechaFinGestion,
                                    IdAgenda = item.IdAgenda,
                                    IdRuta = item.IdRuta,
                                    LongitudGestion = item.Longitud,
                                    LatitudGestion = item.Latitud                                    
                                };

                                if (item.EstadoAgenda == Models.Constantes.EstadoAgenda.NoVisitado)                                
                                    app.Advertencia = true;

                                if (item.EstadoAgenda == Models.Constantes.EstadoAgenda.Visitada)
                                    app.Realizada = true;

                                if(app.LongitudGestion.HasValue && app.LatitudGestion.HasValue && app.Latitud.HasValue && app.Longitud.HasValue && app.LongitudGestion!=0 && app.LatitudGestion!=0)
                                    app.DistanciaGestion = Ubicacion.Distance(app.LongitudGestion.Value, app.LatitudGestion.Value, app.Longitud.Value, app.Latitud.Value) * 1000;//Transformar a metros

                                if (app.DistanciaGestion.HasValue && app.DistanciaGestion > 0 && app.DistanciaGestion > result.RadioDistancia)
                                    app.Advertencia = true;

                                if (app.Realizada && (app.LongitudGestion.HasValue || app.LongitudGestion == 0))
                                    app.Advertencia = true;

                                if (item.ClienteContacto != null)
                                    app.Titulo = item.ClienteContacto.NombresCompletos;
                                else
                                    app.Titulo = item.ClienteDireccion.Cliente.NombresCompletos;

                                if (item.ClienteContacto != null && !string.IsNullOrEmpty(item.ClienteContacto.FotoMin))
                                    app.FotoPath = item.ClienteContacto.FotoMin;
                                else if (!string.IsNullOrEmpty(item.ClienteDireccion.Cliente.FotoMin))
                                    app.FotoPath = item.ClienteDireccion.Cliente.FotoMin;
                                
                                
                                result.Clientes.Add(app);
                            }

                        }
                    }

                    return PartialView("_Map", result);
                }
                catch
                {
                    this.AddDefaultErrorMessage();
                }
            }

            return PartialView("_Map", result);             
        }
    }
}
