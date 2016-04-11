using Rp3.AgendaComercial.Models;
using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models.Oportunidad;
using Rp3.AgendaComercial.Models.Ruta;
using Rp3.AgendaComercial.Process;
using Rp3.Web.Mvc.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Process
{
    public class Executor
    {

        private static List<InformeTrazabilidad> TrazabilidadUbicacionesProcess(ContextService db, bool insert = false, int? IdAgente = null, DateTime? FechaInicio = null, DateTime? FechaFin = null, bool? totalizar = false)
        {
            long index = InformeTrazabilidad.Max();

            List<InformeTrazabilidad> model = new List<InformeTrazabilidad>();

            var listUbicacion = db.AgenteUbicaciones.Get(p =>
                                                         p.Fecha >= FechaInicio && p.Fecha <= FechaFin &&
                                                        (IdAgente == null || p.IdAgente == IdAgente)).OrderBy(p => p.Fecha).ToList();

            var listAgente = listUbicacion.Select(p => p.IdAgente).Distinct();

            db.ParametroHelper.Load();

            foreach (var currentIdAgente in listAgente)
            {
                var list = listUbicacion.Where(p => p.IdAgente == currentIdAgente).ToList();

                foreach (var item in list)
                    item.Ubicacion = String.Format("{0}|{1}|{2}", item.Fecha.ToString("HH:mm"), item.Latitud, item.Longitud);


                foreach (var item in list)
                    item.Reportes = 1;

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

                        anterior.Reportes += actual.Reportes;

                        list.Remove(actual);
                    }
                    else
                    {
                        var distance = Ubicacion.Distance(actual.Longitud ?? 0, actual.Latitud ?? 0, anterior.Longitud ?? 0, anterior.Latitud ?? 0);

                        anterior.Distancia += distance;

                        if (distance < db.ParametroHelper.RoutedDistance)//0.050)
                        {
                            if (actual.FechaHasta == null)
                                anterior.FechaHasta = actual.Fecha;
                            else
                                anterior.FechaHasta = actual.FechaHasta;

                            anterior.Reportes += actual.Reportes;

                            list.Remove(actual);
                        }
                    }
                }

                foreach (var item in list)
                {
                    if (item.FechaHasta != null && item.Fecha.ToString("yyyy-MM-dd HH:mm") == item.FechaHasta.Value.ToString("yyyy-MM-dd HH:mm"))
                        item.FechaHasta = item.Fecha;

                    if (item.Reportes == 1 || item.FechaHasta == null || item.Fecha == item.FechaHasta ||
                        (item.FechaHasta.Value - item.Fecha).TotalMinutes < db.ParametroHelper.MinutosIntervaloTrackingPosition)
                        item.EsMovimiento = true;
                }

                for (int i = list.Count - 1; i > 0; i--)
                {
                    var actual = list[i];
                    var anterior = list[i - 1];

                    if (actual.EsMovimiento && anterior.EsMovimiento)//if (actual.Reportes == 1 && anterior.Reportes == 1)
                    {
                        if (actual.FechaHasta == null)
                            anterior.FechaHasta = actual.Fecha;
                        else
                            anterior.FechaHasta = actual.FechaHasta;

                        anterior.Distancia += actual.Distancia;
                        //anterior.Reportes += actual.Reportes;

                        anterior.Ubicacion = String.Format("{0}~{1}", anterior.Ubicacion, actual.Ubicacion);

                        list.Remove(actual);
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    AgenteUbicacion anterior = null;
                    AgenteUbicacion siguiente = null;

                    AgenteUbicacion actual = list[i];

                    if (actual.EsMovimiento)
                    {
                        if (i - 1 >= 0)
                            anterior = list[i - 1];

                        if (i + 1 < list.Count)
                            siguiente = list[i + 1];

                        if (anterior != null)
                        {
                            actual.Fecha = anterior.FechaHasta ?? anterior.Fecha;
                            //actual.Ubicacion = String.Format("{0}~{1}", anterior.Ubicacion, actual.Ubicacion);

                            var array = anterior.Ubicacion.Split('~');

                            actual.Ubicacion = String.Format("{0}~{1}", array[array.Count() - 1], actual.Ubicacion);
                        }

                        if (siguiente != null)
                        {
                            var array = siguiente.Ubicacion.Split('~');

                            actual.FechaHasta = siguiente.Fecha;
                            //actual.Ubicacion = String.Format("{0}~{1}", actual.Ubicacion, siguiente.Ubicacion);
                            actual.Ubicacion = String.Format("{0}~{1}", actual.Ubicacion, array[0]);
                        }
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    AgenteUbicacion siguiente = null;
                    AgenteUbicacion actual = list[i];

                    if (!actual.EsMovimiento)
                    {
                        if (i + 1 < list.Count)
                            siguiente = list[i + 1];

                        if (siguiente != null && !siguiente.EsMovimiento)
                        {
                            var distance = Ubicacion.Distance(actual.Longitud ?? 0, actual.Latitud ?? 0, siguiente.Longitud ?? 0, siguiente.Latitud ?? 0);

                            var movimiento = new AgenteUbicacion()
                            {
                                IdAgente = actual.IdAgente,
                                Fecha = actual.FechaHasta ?? actual.Fecha,
                                FechaHasta = siguiente.Fecha,
                                Distancia = distance,
                                Latitud = actual.Latitud,
                                Longitud = actual.Longitud,
                                Ubicacion = String.Format("{0}|{1}|{2}~{3}|{4}|{5}",
                                    actual.Fecha.ToString("HH:mm"), actual.Latitud, actual.Longitud,
                                    siguiente.Fecha.ToString("HH:mm"), siguiente.Latitud, siguiente.Longitud),
                                EsMovimiento = true
                            };

                            list.Insert(i + 1, movimiento);
                        }
                    }
                }

                foreach (var item in list)
                {
                    string titulo = String.Empty;
                    string tiempo = String.Empty;

                    double TotalMinutes = 0;

                    if (item.FechaHasta == null || item.Fecha.ToString("HH:mm") == item.FechaHasta.Value.ToString("HH:mm"))
                        titulo = item.Fecha.ToString("HH:mm");
                    else
                    {
                        titulo = String.Format("{0} - {1}", item.Fecha.ToString("HH:mm"), item.FechaHasta.Value.ToString("HH:mm"));

                        var dif = item.FechaHasta.Value - item.Fecha;

                        TotalMinutes = dif.TotalMinutes;

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

                    model.Add(new InformeTrazabilidad()
                    {
                        IdInformeTrazabilidad = index,
                        IdAgente = currentIdAgente,
                        Fecha = item.Fecha.Date,
                        HoraEntrada = item.Fecha.ToString("HH:mm"),
                        Tiempo = !String.IsNullOrEmpty(tiempo) ? tiempo : "0 min.",
                        HoraSalida = item.FechaHasta != null ? item.FechaHasta.Value.ToString("HH:mm") : item.Fecha.ToString("HH:mm"),
                        Distancia = item.Distancia,
                        EsMovimiento = item.EsMovimiento,
                        Minutos = TotalMinutes,
                        Ubicacion = item.Ubicacion.Replace(',', '.'),
                        Latitud = item.Latitud ?? 0,
                        Longitud = item.Longitud ?? 0,
                        TipoTabla = Constantes.TipoEntradaTrazabilidad.Tabla,
                        Tipo = item.EsMovimiento ? Constantes.TipoEntradaTrazabilidad.EnMovimiento : Constantes.TipoEntradaTrazabilidad.Detenido
                    });

                    index++;
                }
            }

            if (totalizar.HasValue && !totalizar.Value)
                foreach (var item in model.Where(p => !p.EsMovimiento))
                    item.Direccion = Process.RutaOptima.GetAddress(item.Latitud, item.Longitud);

            if (insert)
            {
                db.AgenteUbicaciones.DeleteInformeTrazabilidad(FechaInicio.Value, FechaFin.Value);

                foreach (var item in model)
                    db.InformeTrazabilidads.Insert(item);

                db.Save();
            }

            return model;
        }

        private static List<InformeTrazabilidad> TrazabilidadAgendasProcess(ContextService db, bool insert = false, int? IdAgente = null, DateTime? FechaInicio = null, DateTime? FechaFin = null)
        {
            long index = InformeTrazabilidad.Max();
            List<InformeTrazabilidad> model = new List<InformeTrazabilidad>();

            var listAgenda = db.Agendas.Get(p =>
                                            p.FechaInicioGestion.HasValue &&
                                            p.FechaInicioGestion >= FechaInicio && p.FechaInicioGestion <= FechaFin &&
                                            p.EstadoAgenda == "V" &&
                                           (IdAgente == null || p.IdAgente == IdAgente)).OrderBy(p => p.FechaInicioGestion).ToList();

            var listAgente = listAgenda.Select(p => p.IdAgente).Distinct();

            foreach (var currentIdAgente in listAgente)
            {
                var list = listAgenda.Where(p => p.IdAgente == currentIdAgente).ToList();

                list.ForEach(p => p.Ubicacion = String.Format("{0}|{1}|{2}", p.FechaInicioGestion.Value.ToString("HH:mm"), p.Latitud, p.Longitud));

                list.ForEach(p => p.Reportes = 1);

                foreach (var item in list)
                {
                    string titulo = String.Empty;
                    string tiempo = String.Empty;

                    double TotalMinutes = 0;

                    if (item.FechaFinGestion == null || item.FechaInicioGestion.Value.ToString("HH:mm") == item.FechaFinGestion.Value.ToString("HH:mm"))
                        titulo = item.FechaInicioGestion.Value.ToString("HH:mm");
                    else
                    {
                        titulo = String.Format("{0} - {1}", item.FechaInicioGestion.Value.ToString("HH:mm"), item.FechaFinGestion.Value.ToString("HH:mm"));

                        var dif = item.FechaFinGestion.Value - item.FechaInicioGestion.Value;

                        TotalMinutes = dif.TotalMinutes;

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
                    model.Add(new InformeTrazabilidad()
                    {
                        IdInformeTrazabilidad = index,
                        IdAgente = currentIdAgente,
                        Fecha = item.FechaInicioGestion.Value.Date,
                        HoraEntrada = item.FechaInicioGestion.Value.ToString("HH:mm"),
                        Tiempo = !String.IsNullOrEmpty(tiempo) ? tiempo : "0 min.",
                        HoraSalida = item.FechaFinGestion.HasValue ? item.FechaFinGestion.Value.ToString("HH:mm") : string.Empty,
                        Distancia = 0.00D,
                        EsMovimiento = false,
                        Minutos = TotalMinutes,
                        Ubicacion = item.Ubicacion.Replace(',', '.'),
                        Direccion = item.DireccionTrazabilidad,
                        Latitud = item.Latitud ?? 0,
                        Longitud = item.Longitud ?? 0,
                        TipoTabla = Constantes.TipoEntradaTrazabilidad.Tabla,
                        Tipo = Constantes.TipoEntradaTrazabilidad.Gestion,
                        IdRuta = item.IdRuta,
                        IdAgenda = item.IdAgenda
                    });

                    index++;
                }
            }
            if (insert)
            {
                model.ForEach(p => db.InformeTrazabilidads.Insert(p));
                db.Save();
            }

            return model;
        }

        private static List<InformeTrazabilidad> TrazabilidadMarcacionesProcess(ContextService db, bool insert = false, int? IdAgente = null, DateTime? FechaInicio = null, DateTime? FechaFin = null, bool? totalizar = false)
        {
            long index = InformeTrazabilidad.Max();
            List<InformeTrazabilidad> model = new List<InformeTrazabilidad>();

            var listAgenda = db.Marcacions.Get(p =>
                                            p.Fecha >= FechaInicio && p.Fecha <= FechaFin &&
                                           (IdAgente == null || p.IdAgente == IdAgente)).OrderBy(p => p.Fecha).ToList();

            var listAgente = listAgenda.Select(p => p.IdAgente).Distinct();

            foreach (var currentIdAgente in listAgente)
            {
                                var list = listAgenda.Where(p => p.IdAgente == currentIdAgente).ToList();

                list.ForEach(p => p.Ubicacion = String.Format("{0}|{1}|{2}", p.HoraInicio.HasValue ? p.HoraInicio.Value.ToString("HH:mm") : p.HoraFin.HasValue ? p.HoraFin.Value.ToString("HH:mm") : p.Fecha.ToString("HH:mm"), p.Latitud, p.Longitud));


                list.ForEach(p => p.Reportes = 1);

                foreach (var item in list)
                {
                    model.Add(new InformeTrazabilidad()
                    {
                        IdInformeTrazabilidad = index,
                        IdAgente = currentIdAgente,
                        Fecha = item.Fecha.Date,
                        HoraEntrada = item.HoraInicio.HasValue ? item.HoraInicio.Value.ToString("HH:mm") : item.HoraFin.HasValue ? item.HoraFin.Value.ToString("HH:mm") : string.Empty,
                        Tiempo = "0 min.",
                        HoraSalida = item.HoraFin.HasValue ? item.HoraFin.Value.ToString("HH:mm") : item.HoraInicio.HasValue ? item.HoraInicio.Value.ToString("HH:mm") : string.Empty,
                        Distancia = 0.00D,
                        EsMovimiento = false,
                        Minutos = 0.00D,
                        Ubicacion = item.Ubicacion.Replace(',', '.'),
                        Latitud = item.Latitud ?? 0,
                        Longitud = item.Longitud ?? 0,
                        TipoTabla = Constantes.TipoEntradaTrazabilidad.Tabla,
                        Tipo = item.Tipo,
                        IdMarcacion = item.IdMarcacion
                    });

                    index++;
                }
            }
            if(totalizar.HasValue && !totalizar.Value)
                foreach (var item in model.Where(p => !p.EsMovimiento))
                    item.Direccion = Process.RutaOptima.GetAddress(item.Latitud, item.Longitud);

            if (insert)
            {
                model.ForEach(p => db.InformeTrazabilidads.Insert(p));
                db.Save();
            }

            return model;
        }
        public static void GetLog(ContextService db, string ids)
        {
            List<int> list_ids = new List<int>();
            var splitted = ids.Split(',');
            foreach(var id in splitted)
            {
                try
                {
                    list_ids.Add(int.Parse(id));
                }
                catch(Exception ex)
                { }
            }
            var devices = db.Devices.Get(p => list_ids.Contains(p.IdUsuario)).ToList();
            foreach (var device in devices)
            {
                AndroidNotificationPusher.PushNotification(device.GCMId, "OBTAINING LOG", "", "LOG");
            }
        }
        public static List<InformeTrazabilidad> GenerateInformeTrazabilidad(ContextService db, bool insert = false, int? IdAgente = null, DateTime? FechaInicio = null, DateTime? FechaFin = null, bool? totalizar = false)
        {
            if (FechaInicio == null)
            {
                FechaFin = DateTime.Now.AddDays(-1).Date.AddDays(1).Date.AddSeconds(-1);
                FechaInicio = DateTime.Now.AddDays(-1).Date;
            }

            List<InformeTrazabilidad> model = new List<InformeTrazabilidad>();

            model.AddRange(TrazabilidadUbicacionesProcess(db, insert, IdAgente, FechaInicio, FechaFin, totalizar));
            model.AddRange(TrazabilidadAgendasProcess(db, insert, IdAgente, FechaInicio, FechaFin));
            model.AddRange(TrazabilidadMarcacionesProcess(db, insert, IdAgente, FechaInicio, FechaFin, totalizar));

            return model;
        }

        public static void NotificacionOportunidadesAtrasadas(ContextService db)
        {
            var oportunidades = db.Oportunidades.Get(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.EstadoOportunidad.Abierta).ToList();
            foreach(Oportunidad opt in oportunidades)
            {
                var etapa = opt.OportunidadEtapas.FirstOrDefault(p => p.IdEtapa == opt.IdEtapa);
                if (etapa.Etapa.Dias != 0 && etapa.Etapa.Dias != null)
                {
                    var dias = (etapa.FechaInicio.Value - DateTime.Now).TotalDays;
                    dias = dias * -1;
                    if (dias > etapa.Etapa.Dias)
                    {
                        db.Oportunidades.NotificaciónAtrasada(opt.IdOportunidad);
                    }
                }
            }
        }

        public static void NotificacionMarcacion(ContextService db)
        {
            db.ParametroHelper.Load();

            var agentes = db.Agentes.Get(p => p.Estado == Models.Constantes.Estado.Activo && p.IdGrupo != null && p.IdUsuario != null);

            DateTime fecha = DateTime.Now.Date;

            foreach (var agente in agentes)
            {
                bool esDiaLaboral = db.Agentes.EsDiaLaboral(agente.Grupo.IdCalendario, fecha);

                if (esDiaLaboral)
                {
                    var marcacionInicio = db.Marcacions.Get(p => p.IdAgente == agente.IdAgente && p.Fecha == fecha && p.Tipo == Models.Constantes.TipoMarcacion.InicioJornada1).FirstOrDefault();
                    var marcacionFin = db.Marcacions.Get(p => p.IdAgente == agente.IdAgente && p.Fecha == fecha && p.Tipo == Models.Constantes.TipoMarcacion.FinJornada2).FirstOrDefault();

                    if (marcacionInicio == null || marcacionFin == null)
                    {
                        int numdia = (int)fecha.DayOfWeek;

                        if (numdia == 0)
                            numdia = 7;

                        var dia = agente.Grupo.Calendario.DiasLaborales.Where(p => p.IdDia == Convert.ToString(numdia) && p.EsLaboral).FirstOrDefault();

                        if (dia != null)
                        {
                            int hini = Convert.ToInt32(dia.HoraInicio1.Substring(0, 2));
                            int mini = Convert.ToInt32(dia.HoraInicio1.Substring(3, 2));

                            DateTime horainicio = fecha.AddHours(hini).AddMinutes(mini);

                            int hfin = Convert.ToInt32(dia.HoraFin1.Substring(0, 2));
                            int mfin = Convert.ToInt32(dia.HoraFin1.Substring(3, 2));

                            if (dia.HoraFin2 != null)
                            {
                                hfin = Convert.ToInt32(dia.HoraFin2.Substring(0, 2));
                                mfin = Convert.ToInt32(dia.HoraFin2.Substring(3, 2));
                            }

                            DateTime horafin = fecha.AddHours(hfin).AddMinutes(mfin);

                            if (marcacionInicio == null && DateTime.Now > horainicio && (DateTime.Now - horainicio).TotalMinutes > db.ParametroHelper.MinutoNotificacionMarcacion)
                            {
                                var device = db.Devices.Get(p => p.IdUsuario == agente.IdUsuario).FirstOrDefault();

                                if (device != null && !String.IsNullOrEmpty(device.GCMId))
                                {
                                    AndroidNotificationPusher.PushNotification(device.GCMId, "No ha marcado el inicio de su Jornada", String.Format("Por favor marque el inicio de su jornada, o ingrese una Justificación."), "MARCACION");
                                }
                            }
                            else if (marcacionFin == null && DateTime.Now > horafin && (DateTime.Now - horafin).TotalMinutes > db.ParametroHelper.MinutoNotificacionMarcacion)
                            {
                                var device = db.Devices.Get(p => p.IdUsuario == agente.IdUsuario).FirstOrDefault();

                                if (device != null && !String.IsNullOrEmpty(device.GCMId))
                                {
                                    AndroidNotificationPusher.PushNotification(device.GCMId, "No ha marcado el fin de su Jornada", String.Format("Por favor marque el fin de su jornada."), "MARCACION");
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void SendAnyNotification(ContextService db, int idUsuario)
        {
            var device = db.Devices.Get(p => p.IdUsuario == idUsuario).FirstOrDefault();

            if (device != null && !String.IsNullOrEmpty(device.GCMId))
            {
                AndroidNotificationPusher.PushNotification(device.GCMId, "No ha marcado el inicio de su Jornada", String.Format("Por favor marque el inicio de su jornada, o ingrese una Justificación."), "MARCACION");
            }
        }

        public static void Agenda(ContextService db, int? IdRuta = null)
        {
            Calendario calendar;
            List<int> rutas;

            db.ParametroHelper.Load();
            db.Agendas.ExecuteAutomaticAgendas(IdRuta);

            calendar = db.Calendarios.Get(p => p.EsDefault == true, includeProperties: "DiasLaborales, DiasNoLaborables").FirstOrDefault();
            rutas = db.Rutas.Get(p => (IdRuta == null || p.IdRuta == IdRuta) && p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo).Select(p => p.IdRuta).ToList();
            
            if (calendar == null)
                RutaOptima.WriteError(1, 1, "No existe calendario por default.");

            WriteResult(db, 1, 1, "OK");

            //Consulta los ids de Rutas activas y las recorro para ver las agendas de cada ruta
            foreach (int idRuta in rutas)
            {
                var agendas = db.Agendas.Get(p => p.EstadoAgenda == Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.SinProgramar &&
                    p.IdRuta == idRuta, includeProperties: "ClienteDireccion").OrderBy(p => p.FechaInicioOriginal).ThenBy(p => p.IdRuta).ToList();

                List<Agenda> agendasProgramar = new List<Agenda>();
                Agenda anterior = null;

                foreach (Agenda setter in agendas)
                {
                    if (anterior != null && anterior.FechaInicio != setter.FechaInicio)
                    {
                        if (!RutaOptima.GetRutaOptima(db, agendasProgramar, calendar))
                            return;

                        agendasProgramar = new List<Agenda>();
                    }

                    agendasProgramar.Add(setter);
                    anterior = setter;
                }

                //En el caso de que tenga agendas pendientes en la lista, les genero su ruta optima
                if (agendasProgramar.Count > 0)
                    RutaOptima.GetRutaOptima(db, agendasProgramar, calendar);
            }

            WriteResult(db, 1, 2, "OK");
            WriteResult(db, 1, 3, "OK");
        }

        private static void WriteResult(ContextService db, int idProcess, int idStep, String message)
        {
            ProcessLog log = new ProcessLog();
            log.IdProcess = idProcess;
            log.IdProcessStep = idStep;
            log.AsignarId();
            log.Result = message;
            log.UsrIng = "admin";
            log.FecIng = DateTime.Now;
            db.ProcessLogs.Insert(log);
            db.Save();
        }
    }
}
