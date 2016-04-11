using Rp3.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.General.View
{
    public class ParametroHelper : Repository
    {     
        public class Codigo
        {
            public const string HoraInicioTrackingPosition = "HORAINICIOTRACKINGPOSITION";
            public const string HoraFinTrackingPosition = "HORAFINTRACKINGPOSITION";
            public const string MinutosIntervaloTrackingPosition = "MININTERVALTRACKINGPOSITION";
            public const string DefaultInternationalPhoneNumberCode = "DEFAULTINTPHONENUMBERCODE";
            public const string LatitudeDefault = "LATITUDEGENERAL";
            public const string LongitudeDefault = "LONGITUDEEGENERAL";
            public const string DefaultRouteTime = "DEFAULTROUTETIME";
            public const string RoutedDistance = "ROUTEDISTANCE";
            public const string VisitDistance = "VISITDISTANCE";
            public const string MarcacionDistance = "MARCACIONDISTANCE";

            public const string MinutoAtrasoDia = "MINUTOSATRASODIA";
            public const string MinutoAtrasoMes = "MINUTOSATRASOMES";
            public const string AusenciaSinJustificar = "AUSENCIASINJUSTIFICAR";
            public const string MinMarcacionEficiencia = "MINMARCACIONEFICIENCIA";
            public const string MaxMarcacionEficiencia = "MAXMARCACIONEFICIENCIA";

            public const string MinutoNotificacionMarcacion = "MINNOTIFICACIONMARCACION";

            public const string AgenteUbicacion1 = "AGENTEUBICACION1";
            public const string AgenteUbicacion2 = "AGENTEUBICACION2";
            public const string AgenteUbicacion3 = "AGENTEUBICACION3";
        }

        public enum Tipo
        {
            Texto = 0,
            NumeroEntero = 1,
            NumeroDecimal = 2,
            Hora = 3,
            TextoNumero = 4
        }

        public ParametroHelper(DbContext context)
            : base(context)
        {

        }  
        

        public void Load()
        {
            parametros = ((Context)context).Parametros.ToList();
        }

        private List<Parametro> parametros;

        private int? agenteUbicacion1;
        public int AgenteUbicacion1
        {
            get
            {
                if (!agenteUbicacion1.HasValue)
                {
                    string valor = parametros.SingleOrDefault(p => p.IdParametro == Codigo.AgenteUbicacion1).Valor;
                    agenteUbicacion1 = 1;
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        agenteUbicacion1 = Convert.ToInt32(valor);
                    }
                }

                return agenteUbicacion1.Value;
            }
        }

        private int? agenteUbicacion2;
        public int AgenteUbicacion2
        {
            get
            {
                if (!agenteUbicacion2.HasValue)
                {
                    string valor = parametros.SingleOrDefault(p => p.IdParametro == Codigo.AgenteUbicacion2).Valor;
                    agenteUbicacion2 = 24;
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        agenteUbicacion2 = Convert.ToInt32(valor);
                    }
                }

                return agenteUbicacion2.Value;
            }
        }

        private int? agenteUbicacion3;
        public int AgenteUbicacion3
        {
            get
            {
                if (!agenteUbicacion3.HasValue)
                {
                    string valor = parametros.SingleOrDefault(p => p.IdParametro == Codigo.AgenteUbicacion3).Valor;
                    agenteUbicacion3 = 48;
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        agenteUbicacion3 = Convert.ToInt32(valor);
                    }
                }

                return agenteUbicacion3.Value;
            }
        }

        private DateTime? horaInicioTrackingPosition;
        public DateTime HoraInicioTrackingPosition 
        { 
            get
            {
                if (!horaInicioTrackingPosition.HasValue)
                {
                    string valor = parametros.SingleOrDefault(p => p.IdParametro == Codigo.HoraInicioTrackingPosition).Valor;
                    horaInicioTrackingPosition = DateTime.Now.Date;
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        var parts = valor.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        horaInicioTrackingPosition = DateTime.Now.Date.AddHours(Convert.ToInt32(parts[0])).AddMinutes(Convert.ToInt32(parts[1]));
                    }
                }

                return horaInicioTrackingPosition.Value;
            }
        }

        private DateTime? horaFinTrackingPosition;
        public DateTime HoraFinTrackingPosition
        {
            get
            {
                if (!horaFinTrackingPosition.HasValue)
                {
                    string valor = parametros.SingleOrDefault(p => p.IdParametro == Codigo.HoraFinTrackingPosition).Valor;
                    horaFinTrackingPosition = DateTime.Now.Date.AddDays(1).AddMinutes(-1);
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        var parts = valor.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        horaFinTrackingPosition = DateTime.Now.Date.AddHours(Convert.ToInt32(parts[0])).AddMinutes(Convert.ToInt32(parts[1]));
                    }
                }

                return horaFinTrackingPosition.Value;
            }
        }

        private int? minutosIntervaloTrackingPosition;
        public int MinutosIntervaloTrackingPosition
        {
            get
            {
                if (!minutosIntervaloTrackingPosition.HasValue)
                {
                    string valor = parametros.SingleOrDefault(p => p.IdParametro == Codigo.MinutosIntervaloTrackingPosition).Valor;
                    minutosIntervaloTrackingPosition = 15;
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        minutosIntervaloTrackingPosition = Convert.ToInt32(valor);
                    }
                }

                return minutosIntervaloTrackingPosition.Value;
            }
        }

        private string defaultInternationalPhoneNumberCode;
        public string DefaultInternationalPhoneNumberCode
        {
            get
            {
                if (defaultInternationalPhoneNumberCode == null)
                {
                    string valor = parametros.SingleOrDefault(p => p.IdParametro == Codigo.DefaultInternationalPhoneNumberCode).Valor;
                    defaultInternationalPhoneNumberCode = "";
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        defaultInternationalPhoneNumberCode = valor;
                    }
                }

                return defaultInternationalPhoneNumberCode;
            }
        }

        private string latitudeDefault;
        public string LatitudeDefault
        {
            get
            {
                if (latitudeDefault == null)
                {
                    string valor = parametros.SingleOrDefault(p => p.IdParametro == Codigo.LatitudeDefault).Valor;
                    latitudeDefault = "";
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        latitudeDefault = valor;
                    }
                }

                return latitudeDefault;
            }
        }

        private string longitudeDefault;
        public string LongitudeDefault
        {
            get
            {
                if (longitudeDefault == null)
                {
                    string valor = parametros.SingleOrDefault(p => p.IdParametro == Codigo.LongitudeDefault).Valor;
                    longitudeDefault = "";
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        longitudeDefault = valor;
                    }
                }

                return longitudeDefault;
            }
        }

        private string defaultRouteTime;
        public string DefaultRouteTime
        {
            get
            {
                if (defaultRouteTime == null)
                {
                    string valor = parametros.SingleOrDefault(p => p.IdParametro == Codigo.DefaultRouteTime).Valor;
                    defaultRouteTime = "";
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        defaultRouteTime = valor;
                    }
                }

                return defaultRouteTime;
            }
        }

        private double routedDistance;
        public double RoutedDistance
        {
            get
            {
                if (routedDistance == 0)
                {
                    string valor = parametros.SingleOrDefault(p => p.IdParametro == Codigo.RoutedDistance).Valor;
                    routedDistance = 0;
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        double.TryParse(valor, out routedDistance);
                        routedDistance = routedDistance / 100;
                    }
                }

                return routedDistance;
            }
        }

        private double visitDistance;
        public double VisitDistance
        {
            get
            {
                if (visitDistance == 0)
                {
                    string valor = parametros.SingleOrDefault(p => p.IdParametro == Codigo.VisitDistance).Valor;
                    visitDistance = 0;
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        double.TryParse(valor, out visitDistance);
                        //visitDistance = visitDistance / 10;
                    }
                }

                return visitDistance;
            }
        }

        private double marcacionDistance;
        public double MarcacionDistance
        {
            get
            {
                if (marcacionDistance == 0)
                {
                    string valor = parametros.SingleOrDefault(p => p.IdParametro == Codigo.MarcacionDistance).Valor;
                    marcacionDistance = 0;
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        double.TryParse(valor, out marcacionDistance);
                        //visitDistance = visitDistance / 10;
                    }
                }

                return marcacionDistance;
            }
        }

        private double minutoAtrasoDia;
        public double MinutoAtrasoDia
        {
            get
            {
                if (minutoAtrasoDia == 0)
                {
                    string valor = parametros.SingleOrDefault(p => p.IdParametro == Codigo.MinutoAtrasoDia).Valor;
                    minutoAtrasoDia = 0;
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        double.TryParse(valor, out minutoAtrasoDia);
                        //visitDistance = visitDistance / 10;
                    }
                }

                return minutoAtrasoDia;
            }
        }

        private double minutoAtrasoMes;
        public double MinutoAtrasoMes
        {
            get
            {
                if (minutoAtrasoMes == 0)
                {
                    string valor = parametros.SingleOrDefault(p => p.IdParametro == Codigo.MinutoAtrasoMes).Valor;
                    minutoAtrasoMes = 0;
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        double.TryParse(valor, out minutoAtrasoMes);
                        //visitDistance = visitDistance / 10;
                    }
                }

                return minutoAtrasoMes;
            }
        }

        private double ausenciaSinJustificar;
        public double AusenciaSinJustificar
        {
            get
            {
                if (ausenciaSinJustificar == 0)
                {
                    string valor = parametros.SingleOrDefault(p => p.IdParametro == Codigo.AusenciaSinJustificar).Valor;
                    ausenciaSinJustificar = 0;
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        double.TryParse(valor, out ausenciaSinJustificar);
                        //visitDistance = visitDistance / 10;
                    }
                }

                return ausenciaSinJustificar;
            }
        }

        private double minMarcacionEficiencia;
        public double MinMarcacionEficiencia
        {
            get
            {
                if (minMarcacionEficiencia == 0)
                {
                    string valor = parametros.SingleOrDefault(p => p.IdParametro == Codigo.MinMarcacionEficiencia).Valor;
                    minMarcacionEficiencia = 0;
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        double.TryParse(valor, out minMarcacionEficiencia);
                        //visitDistance = visitDistance / 10;
                    }
                }

                return minMarcacionEficiencia;
            }
        }
        private double maxMarcacionEficiencia;
        public double MaxMarcacionEficiencia
        {
            get
            {
                if (maxMarcacionEficiencia == 0)
                {
                    string valor = parametros.SingleOrDefault(p => p.IdParametro == Codigo.MaxMarcacionEficiencia).Valor;
                    maxMarcacionEficiencia = 0;
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        double.TryParse(valor, out maxMarcacionEficiencia);
                        //visitDistance = visitDistance / 10;
                    }
                }

                return maxMarcacionEficiencia;
            }
        }

        private double minutoNotificacionMarcacion;
        public double MinutoNotificacionMarcacion
        {
            get
            {
                if (minutoNotificacionMarcacion == 0)
                {
                    string valor = parametros.SingleOrDefault(p => p.IdParametro == Codigo.MinutoNotificacionMarcacion).Valor;
                    minutoNotificacionMarcacion = 0;
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        double.TryParse(valor, out minutoNotificacionMarcacion);
                        //visitDistance = visitDistance / 10;
                    }
                }

                return minutoNotificacionMarcacion;
            }
        }

    }
}
