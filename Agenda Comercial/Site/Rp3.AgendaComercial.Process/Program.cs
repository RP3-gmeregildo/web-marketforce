using Rp3.AgendaComercial.Models;
using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models.General.View;
using Rp3.AgendaComercial.Models.Ruta;
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
    class Program
    {
        static void Main(string[] args)
        {
            Rp3.Security.Cryptography.KeyFileName = "key";

            ContextService db = new Models.ContextService();

            string mode = "ANYNOTIFICATION";

            //if (args.Length > 0)
            //    mode = args[0];

            switch (mode)
            {
                case "AGENDA": Executor.Agenda(db); break;
                case "NOTIFICACIONMARCACION": Executor.NotificacionMarcacion(db); break;
                case "INFORMETRAZABILIDAD": Executor.GenerateInformeTrazabilidad(db, true); break;
                case "INFORMETRAZABILIDADALL": Executor.GenerateInformeTrazabilidad(db, true, null, new DateTime(2016, 1, 13), new DateTime(2016, 1, 18)); break;
                case "OPORTUNIDADATRASADA": Executor.NotificacionOportunidadesAtrasadas(db); break;
                case "GETLOG": Executor.GetLog(db, "66,54"); break;
                case "ANYNOTIFICATION": Executor.SendAnyNotification(db, 26); break;
            }
        }
    }
}
