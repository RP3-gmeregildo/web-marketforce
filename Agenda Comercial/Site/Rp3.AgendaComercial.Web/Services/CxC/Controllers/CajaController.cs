using Rp3.AgendaComercial.Models.CxC;
using Rp3.AgendaComercial.Models.Transaccion;
using Rp3.AgendaComercial.Web.Services.CxC.Models;
using Rp3.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Rp3.AgendaComercial.Web.Services.CxC.Controllers
{
    public class CajaController : AgendaComercial.Web.Services.Controllers.ApiAgendaComercialController
    {
        [ApiAuthorization]
        public IHttpActionResult GetCaja()
        {
            Caja caja = new Caja();

            var cajaSetter = DataBase.Cajas.GetSingleOrDefault(p => p.IdCaja == Agente.IdCaja, includeProperties: "CajaFormasPago, CajaFormasPago.FormaPago");
            var cajaControlActivo = DataBase.CajaControles.GetSingleOrDefault(p => p.IdCaja == Agente.IdCaja && p.Activo == true);
            
            caja.IdCaja = cajaSetter.IdCaja;
            caja.MaximoDiasApertura = cajaSetter.MaximoDiasApertura;
            caja.Nombre = cajaSetter.Nombre;
            caja.SecuenciaRecibo = cajaSetter.SecuenciaRecibo;

            CxC.Models.CajaControl cajaControl = new CxC.Models.CajaControl();
            cajaControl.IdCaja = cajaControlActivo.IdCaja;
            cajaControl.IdControlCaja = cajaControlActivo.IdControlCaja;
            cajaControl.MontoApertura = cajaControlActivo.MontoApertura;
            cajaControl.MontoCierre = cajaControlActivo.MontoCierre;
            cajaControl.FechaApertura = cajaControlActivo.FechaApertura;
            cajaControl.FechaCierre = cajaControlActivo.FechaCierre;
            caja.CajaControl = cajaControl;

            List<CxC.Models.FormasPago> formasPago = new List<FormasPago>();
            foreach (CajaFormaPago fp in cajaSetter.CajaFormasPago)
            {
                CxC.Models.FormasPago formaPago = new FormasPago();
                formaPago.IdFormaPago = fp.FormaPago.IdFormaPago;
                formaPago.Nombre = fp.FormaPago.Nombre;
                formasPago.Add(formaPago);
            }

            caja.FormasPago = formasPago;

            return Ok(caja);
        }
    }
}