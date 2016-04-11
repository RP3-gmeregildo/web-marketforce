using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Rp3.AgendaComercial.Models.Ruta;
using Rp3.AgendaComercial.Models.Ruta.View;
using Rp3.AgendaComercial.Models.General;

namespace Rp3.AgendaComercial.Web.Ruta
{
    public class AgendaConsulta
    {
        public bool ReadOnly { get; set; }
        //CALENDARIO
        public List<Appointment> Appointments { get; set; }

        //LISTADO
        public List<AgendaListado> AgendaListados { get; set; }

        //LISTADO
        public List<AgendaCategoria> AgendaCategorias { get; set; }

        //MODAL
        public List<AgendaClientes> AgendaClientes { get; set; }        
        public List<ModalParam> ModalParam { get; set; }

        //INSERT & EDIT
        public List<ComboGroup> ComboGroups { get; set; }
        public List<ComboCliente> ComboClientes { get; set; }
        public List<ComboContacto> ComboContactos { get; set; }
        public List<ComboDireccion> ComboDirecciones { get; set; }
        public List<ComboTarea> ComboTarea { get; set; }

        //OTHER
        public AgendaUpdates ListaAgendaUpdates { get; set; }

        public List<Ubicacion> Ubicaciones { get; set; }
        public int idPedido { get; set; }

    }
   

}