using Rp3.AgendaComercial.Models;
using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models.Ruta;
using Rp3.AgendaComercial.Models.Ruta.View;
using Rp3.AgendaComercial.Process.Clases;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Process
{
    public class RutaOptima
    {
        static ContextService db = new Models.ContextService();

        public static List<ProgramacionPreview> GetRutaOptima(List<ProgramacionPreview> agendasProgramar, Calendario calendar)
        {
            if (agendasProgramar.Count > 0)
            {
                db.ParametroHelper.Load();

                List<ProgramacionPreview> agendaOrdenada;
                ZonaDetalle zonaDetalle = null;
                try
                {
                    int contDirecciones = 0;
                    ProgramacionPreview first = null;

                    do
                    {
                        if (agendasProgramar[contDirecciones].IdCiudad.HasValue)
                        {
                            int idGeopoliticalStructure = agendasProgramar[contDirecciones].IdCiudad.Value;
                            zonaDetalle = db.ZonaDetalles.Get(p => p.IdGeopoliticalStructure == idGeopoliticalStructure, includeProperties: "Zona").FirstOrDefault();

                            if (zonaDetalle != null && zonaDetalle.Zona.LatitudPuntoPartida.HasValue && zonaDetalle.Zona.LongitudPuntoPartida.HasValue)
                            {
                                first = new ProgramacionPreview()
                                {
                                    Partida = true,
                                    Titulo = "Punto de Partida",
                                    Latitud = zonaDetalle.Zona.LatitudPuntoPartida,
                                    Longitud = zonaDetalle.Zona.LongitudPuntoPartida
                                };
                            }
                        }
                        contDirecciones++;
                        if (contDirecciones == agendasProgramar.Count)
                        {
                            first = new ProgramacionPreview()
                            {
                                Partida = true,
                                Titulo = "Punto de Partida",
                                Latitud = double.Parse(db.ParametroHelper.LatitudeDefault),
                                Longitud = double.Parse(db.ParametroHelper.LongitudeDefault)
                            };
                        }
                    } 
                    while (first == null);

                    //Creo una lista con los elementos que no tengan coordenadas, y los elimino de la lista principal
                    List<ProgramacionPreview> sinCoordenadas = new List<ProgramacionPreview>();

                    foreach (ProgramacionPreview ag in agendasProgramar)
                    {
                        if (!ag.Latitud.HasValue || ag.Latitud.Value == 0 ||
                            !ag.Longitud.HasValue || ag.Longitud.Value == 0)
                            sinCoordenadas.Add(ag);
                    }

                    foreach (ProgramacionPreview elim in sinCoordenadas)
                        agendasProgramar.Remove(elim);

                    //Obtengo la primera ruta
                    int distancia = -1;
                    ProgramacionPreview agendaSgte = null;
                    agendaOrdenada = new List<ProgramacionPreview>() { first };
                    while (agendasProgramar.Count() > 0)
                    {
                        foreach (ProgramacionPreview ag in agendasProgramar)
                        {
                            int distanciaActual = GetDistancia(first.Latitud.Value, first.Longitud.Value,
                                                                ag.Latitud.Value, ag.Longitud.Value);
                            if (distancia == -1 || distancia > distanciaActual)
                            {
                                distancia = distanciaActual;
                                agendaSgte = ag;
                            }
                        }
                        agendaOrdenada.Add(agendaSgte);
                        distancia = -1;
                        first = agendaSgte;
                        agendaSgte = null;
                        agendasProgramar.Remove(first);
                    }

                    //Agrego al final las agendas sin coordenadas
                    agendaOrdenada.AddRange(sinCoordenadas);

                    return agendaOrdenada;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            return null;
        }

        public static bool GetRutaOptima(Models.ContextService db, List<Agenda> agendasProgramar, Calendario calendar)
        {
            if (agendasProgramar.Count > 0)
            {
                List<Agenda> agendaOrdenada;
                ZonaDetalle zonaDetalle = null;
                    int contDirecciones = 0;
                    Agenda first = null;
                    do
                    {
                        if (agendasProgramar[contDirecciones].IdCiudad.HasValue)
                        {
                            int idGeopoliticalStructure = agendasProgramar[contDirecciones].IdCiudad.Value;
                            zonaDetalle = db.ZonaDetalles.Get(p => p.IdGeopoliticalStructure == idGeopoliticalStructure, includeProperties: "Zona").FirstOrDefault();
                            if (zonaDetalle != null && zonaDetalle.Zona.LatitudPuntoPartida.HasValue && zonaDetalle.Zona.LongitudPuntoPartida.HasValue)
                            {
                                first = new Agenda()
                                {
                                    IdAgenda = 0,
                                    ClienteDireccion = new Rp3.AgendaComercial.Models.General.ClienteDireccion()
                                    {
                                        IdClienteDireccion = 0,
                                        Latitud = zonaDetalle.Zona.LatitudPuntoPartida,
                                        Longitud = zonaDetalle.Zona.LongitudPuntoPartida,
                                    },
                                    IdRuta = 0,
                                };
                            }
                        }
                        contDirecciones++;
                        if (contDirecciones == agendasProgramar.Count)
                        {
                            first = new Agenda()
                            {
                                IdAgenda = 0,
                                ClienteDireccion = new Rp3.AgendaComercial.Models.General.ClienteDireccion()
                                {
                                    IdClienteDireccion = 0,
                                    Latitud = double.Parse(db.ParametroHelper.LatitudeDefault.Replace('.', ',')),
                                    Longitud = double.Parse(db.ParametroHelper.LongitudeDefault.Replace('.', ',')),
                                },
                                IdRuta = 0,
                            };
                        }
                    } while (first == null);

                    //Creo una lista con los elementos que no tengan coordenadas, y los elimino de la lista principal
                    List<Agenda> sinCoordenadas = new List<Agenda>();
                    foreach (Agenda ag in agendasProgramar)
                    {
                        if (!ag.ClienteDireccion.Latitud.HasValue || ag.ClienteDireccion.Latitud.Value == 0 ||
                            !ag.ClienteDireccion.Longitud.HasValue || ag.ClienteDireccion.Longitud.Value == 0)
                            sinCoordenadas.Add(ag);
                    }
                    foreach (Agenda elim in sinCoordenadas)
                        agendasProgramar.Remove(elim);

                    //Obtengo la primera ruta
                    int distancia = -1;
                    Agenda agendaSgte = null;
                    agendaOrdenada = new List<Agenda>();
                    while (agendasProgramar.Count() > 0)
                    {
                        foreach (Agenda ag in agendasProgramar)
                        {
                            int distanciaActual = GetDistancia(first.ClienteDireccion.Latitud.Value, first.ClienteDireccion.Longitud.Value,
                                                                ag.ClienteDireccion.Latitud.Value, ag.ClienteDireccion.Longitud.Value);
                            if (distancia == -1 || distancia > distanciaActual)
                            {
                                distancia = distanciaActual;
                                agendaSgte = ag;
                            }
                        }
                        agendaOrdenada.Add(agendaSgte);
                        distancia = -1;
                        first = agendaSgte;
                        agendaSgte = null;
                        agendasProgramar.Remove(first);
                    }

                    //Agrego al final las agendas sin coordenadas
                    agendaOrdenada.AddRange(sinCoordenadas);


                    DateTime date = agendaOrdenada[0].FechaInicio.Value;

                    //foreach (DiasNoLaborable dia in calendar.DiasNoLaborables)
                    //{
                    //    if (dia.Fecha.Day == date.Day && dia.Fecha.Month == date.Month)
                    //    {
                    //        foreach (Agenda agdSv in agendaOrdenada)
                    //        {
                    //            agdSv.FechaInicio = agdSv.FechaInicio.Value.AddDays(1);
                    //            db.Agendas.Update(agdSv);
                    //            db.Save();
                    //        }

                    //        return true;
                    //    }
                    //}

                    var weekday = getDayOfWeek((int)date.DayOfWeek);
                    var diaLaboral = calendar.DiasLaborales.Where(p => p.IdDia == weekday + "").FirstOrDefault();
                    DateTime horaInicial1 = agendaOrdenada[0].FechaInicio.Value;
                    DateTime? horaInicial2 = null;
                    DateTime horaFin1 = agendaOrdenada[0].FechaInicio.Value;
                    DateTime? horaFin2 = null;
                    if (diaLaboral.EsLaboral)
                    {
                        DateTime setter = DateTime.ParseExact(diaLaboral.HoraInicio1.Replace('h', ':'), "HH:mm", CultureInfo.InvariantCulture);
                        int pru = setter.Hour;
                        horaInicial1 = horaInicial1.AddHours(setter.Hour);
                        horaInicial1 = horaInicial1.AddMinutes(setter.Minute);

                        setter = DateTime.ParseExact(diaLaboral.HoraFin1.Replace('h', ':'), "HH:mm", CultureInfo.InvariantCulture);
                        horaFin1 = horaFin1.AddHours(setter.Hour);
                        horaFin1 = horaFin1.AddMinutes(setter.Minute);

                        if (!string.IsNullOrWhiteSpace(diaLaboral.HoraInicio2))
                        {
                            setter = DateTime.ParseExact(diaLaboral.HoraInicio2.Replace('h', ':'), "HH:mm", CultureInfo.InvariantCulture);
                            horaInicial2 = agendaOrdenada[0].FechaInicio.Value;
                            horaInicial2 = horaInicial2.Value.AddHours(setter.Hour);
                            horaInicial2 = horaInicial2.Value.AddMinutes(setter.Minute);

                            setter = DateTime.ParseExact(diaLaboral.HoraFin2.Replace('h', ':'), "HH:mm", CultureInfo.InvariantCulture);
                            horaFin2 = agendaOrdenada[0].FechaInicio.Value;
                            horaFin2 = horaFin2.Value.AddHours(setter.Hour);
                            horaFin2 = horaFin2.Value.AddMinutes(setter.Minute);
                        }
                    }
                    //else
                    //{
                    //    //se guardan agendas para otro dia laborable
                    //    foreach (Agenda agdSv in agendaOrdenada)
                    //    {
                    //        agdSv.FechaInicio = agdSv.FechaInicio.Value.AddDays(1);
                    //        db.Agendas.Update(agdSv);
                    //        db.Save();
                    //    }

                    //    return true;
                    //}

                    int tiempoMovilizacion = 0;
                    if (zonaDetalle != null && zonaDetalle.Zona.TiempoMovilizacion != 0)
                        tiempoMovilizacion = zonaDetalle.Zona.TiempoMovilizacion;
                    else
                        tiempoMovilizacion = int.Parse(db.ParametroHelper.DefaultRouteTime);

                    bool primerHorario = true;
                    foreach (Agenda agdSv in agendaOrdenada)
                    {
                        if (horaInicial1 > horaFin1 && primerHorario)
                        {
                            primerHorario = false;
                            if (horaInicial2 != null)
                            {
                                TimeSpan minutos = horaInicial1.Subtract(horaFin1);
                                horaInicial2 = horaInicial2 = horaInicial2.Value.AddMinutes(minutos.TotalMinutes);
                            }
                        }
                        if (primerHorario)
                        {
                            horaInicial1 = horaInicial1.AddMinutes(tiempoMovilizacion);
                            agdSv.TiempoViaje = tiempoMovilizacion;
                            agdSv.FechaInicio = horaInicial1;
                            agdSv.FechaInicioOriginal = horaInicial1;
                            if (agdSv.Duracion.HasValue)
                                horaInicial1 = horaInicial1.AddMinutes(agdSv.Duracion.Value);
                            else
                                horaInicial1 = horaInicial1.AddMinutes(30);
                            agdSv.FechaFin = horaInicial1;
                            agdSv.FechaFinOriginal = horaInicial1;
                        }
                        else if (horaInicial2 != null)
                        {
                            horaInicial2 = horaInicial2.Value.AddMinutes(tiempoMovilizacion);
                            agdSv.TiempoViaje = tiempoMovilizacion;
                            agdSv.FechaInicio = horaInicial2;
                            agdSv.FechaInicioOriginal = horaInicial2;
                            if (agdSv.Duracion.HasValue)
                                horaInicial2 = horaInicial2.Value.AddMinutes(agdSv.Duracion.Value);
                            else
                                horaInicial2 = horaInicial2.Value.AddMinutes(30);
                            agdSv.FechaFin = horaInicial2;
                            agdSv.FechaFinOriginal = horaInicial2;
                        }
                        else
                        {
                            agdSv.FechaInicio = agdSv.FechaInicio.Value.AddDays(1);
                            db.Agendas.Update(agdSv);
                            db.Save();
                            continue;
                        }

                        //Grabar Agenda con Horario
                        agdSv.EstadoAgenda = Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Pendiente;
                        db.Agendas.Update(agdSv);
                        db.Save();


                    }
            }

            return true;
        }

        public static string GetAddress(double Lat, double Lon)
        {
            string sURL;
            sURL = "http://maps.googleapis.com/maps/api/geocode/json?latlng=";
            sURL += Lat.ToString().Replace(',', '.') + "," + Lon.ToString().Replace(',', '.');
            sURL += "&sensor=false";

            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sURL);

            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(GoogleAddress));

            StreamReader objReader = new StreamReader(objStream);
            GoogleAddress data = (GoogleAddress)ser.ReadObject(objStream);

            if (data.results == null || data.results.Count == 0)
            {
                return null;
            }

            return data.results[0].formatted_address;
        }

        public static int GetDistancia(double origenLat, double origenLon, double destinoLat, double destinoLon)
        {
            string sURL;
            sURL = "http://maps.googleapis.com/maps/api/directions/json?origin=";
            sURL += origenLat.ToString().Replace(',', '.') + "," + origenLon.ToString().Replace(',', '.');
            sURL += "&destination=";
            sURL += destinoLat.ToString().Replace(',', '.') + "," + destinoLon.ToString().Replace(',', '.');
            sURL += "&sensor=false&mode=driving";

            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sURL);

            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(GoogleData));

            StreamReader objReader = new StreamReader(objStream);
            GoogleData data = (GoogleData)ser.ReadObject(objStream);
            
            if(data.routes == null || data.routes.Count == 0)
            {
                return 0;
            }

            return data.routes[0].legs[0].distance.value;
        }

        public static void WriteError(int idProcess, int idStep, Exception ex)
        {
            ProcessLog log = new ProcessLog();
            log.IdProcess = idProcess;
            log.IdProcessStep = idStep;
            log.AsignarId();
            log.ErrorLog = ex.Message;
            log.UsrIng = "admin";
            log.FecIng = DateTime.Now;
            db.ProcessLogs.Insert(log);
            db.Save();
        }

        public static void WriteError(int idProcess, int idStep, string ex)
        {
            ProcessLog log = new ProcessLog();
            log.IdProcess = idProcess;
            log.IdProcessStep = idStep;
            log.AsignarId();
            log.ErrorLog = ex;
            log.UsrIng = "admin";
            log.FecIng = DateTime.Now;
            db.ProcessLogs.Insert(log);
            db.Save();
        }

        public static int getDayOfWeek(int i)
        {
            switch(i)
            {
                case 0: return 7; 
                case 1: return 1;
                case 2: return 2;
                case 3: return 3;
                case 4: return 4;
                case 5: return 5;
                case 6: return 6;
                default: return 1;
            }
            
        }
    }
}
