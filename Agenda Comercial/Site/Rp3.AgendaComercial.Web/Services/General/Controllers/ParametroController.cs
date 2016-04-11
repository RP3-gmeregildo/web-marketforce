using Rp3.AgendaComercial.Web.Services.General.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Services.General.Controllers
{
    public class ParametroController : AgendaComercial.Web.Services.Controllers.ApiAgendaComercialController
    {
        // GET: Parametro
        public IHttpActionResult GetAll()
        {
            Parametro parametro = new Parametro();
            DataBase.ParametroHelper.Load();
            parametro.DefaultInternationalPhoneNumberCode = DataBase.ParametroHelper.DefaultInternationalPhoneNumberCode;
            parametro.HoraFinTrackingPositionTicks = DataBase.ParametroHelper.HoraFinTrackingPosition.Ticks;
            parametro.HoraInicioTrackingPositionTicks = DataBase.ParametroHelper.HoraInicioTrackingPosition.Ticks;
            parametro.MinutosIntervaloTrackingPosition = DataBase.ParametroHelper.MinutosIntervaloTrackingPosition;
            parametro.MarcacionDistance = DataBase.ParametroHelper.MarcacionDistance;
            parametro.AgenteUbicacion1 = DataBase.ParametroHelper.AgenteUbicacion1;
            parametro.AgenteUbicacion2 = DataBase.ParametroHelper.AgenteUbicacion2;
            parametro.AgenteUbicacion3 = DataBase.ParametroHelper.AgenteUbicacion3;

            return Ok(parametro);
        }
    }
}