using Rp3.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Drawing;
using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Web.Services.Controllers;
using Rp3.AgendaComercial.Models.General.View;

namespace Rp3.AgendaComercial.Web.Services.General.Controllers
{
    public class TareaController : ApiAgendaComercialController
    {
        [ApiAuthorization]
        public IHttpActionResult Get(long? idTarea = null, long? ultimaActualizacion = null)
        {
            if(Agente==null || !Agente.IdRuta.HasValue)
            {
                return Ok(new List<Models.Tarea>());
            }

            DateTime? fecMod = null;
            if (ultimaActualizacion != null)
            {
                fecMod = new DateTime(ultimaActualizacion.Value);
            }
            
            var models = DataBase.Tareas.GetSync(Agente.IdRuta.Value, null, fecMod);

            List<Models.Tarea> data = new List<Models.Tarea>();

            Assign(models, data);

            foreach (var tarea in data)
            {
                var model = models.Where(p => p.IdTarea == tarea.IdTarea).SingleOrDefault();

                tarea.TareaActividades = new List<Models.TareaActividad>();

                foreach (var act in model.TareaActividades)
                {
                    var actividad = new Models.TareaActividad()
                    {
                        IdTarea = model.IdTarea,
                        IdTareaActividad = act.IdTareaActividad,
                        IdTareaActividadPadre = act.IdTareaActividadPadre,
                        Descripcion = act.Descripcion,
                        IdTipoActividad = act.IdTipoActividad,
                        Tipo = act.TipoActividad.Tipo,
                        Orden = act.Orden,
                        Limite = act.Limite,
                        TareaOpciones = new List<Models.TareaOpcion>()
                    };

                    if (actividad.IdTipoActividad == Rp3.AgendaComercial.Models.Constantes.TipoActividad.DefaultMultipleSeleccion ||
                        actividad.IdTipoActividad == Rp3.AgendaComercial.Models.Constantes.TipoActividad.DefaultSeleccion)
                    {
                        var opciones = DataBase.TipoActividadOpciones.Get(p => p.IdTipoActividad == actividad.IdTipoActividad);

                        foreach (var opc in opciones)
                        {
                            actividad.TareaOpciones.Add(new Models.TareaOpcion()
                            {
                                IdTarea = model.IdTarea,
                                IdTareaActividad = act.IdTareaActividad,
                                //IdTareaOpcion = opc.IdTareaOpcion,
                                Descripcion = opc.Descripcion,
                                Orden = opc.Orden
                            });
                        }
                    }
                    else if (actividad.Tipo == Rp3.AgendaComercial.Models.Constantes.TipoActividad.MultipleSeleccion ||
                        actividad.Tipo == Rp3.AgendaComercial.Models.Constantes.TipoActividad.Seleccion)
                    {
                        foreach (var opc in act.TipoActividad.TipoActividadOpciones)
                        {
                            actividad.TareaOpciones.Add(new Models.TareaOpcion()
                            {
                                IdTarea = model.IdTarea,
                                IdTareaActividad = act.IdTareaActividad,
                                IdTareaOpcion = opc.IdTipoActividadOpcion,
                                Descripcion = opc.Descripcion,
                                Orden = opc.Orden
                            });
                        }
                    }

                    tarea.TareaActividades.Add(actividad);
                }
            }

            return Ok(data);
        }

        [ApiAuthorization]
        public IHttpActionResult GetTareaActualizacionCliente(long? idTarea = null)
        {
            var actCampos = new ActualizacionCliente();
            var opciones = DataBase.TareaClienteActualizaciones.Get(p => p.IdTarea == idTarea.Value).FirstOrDefault();
            var data = DataBase.TareaClienteActualizacionCampos.Get(p => p.IdTarea == idTarea.Value, includeProperties: "Parametro").ToList();
            actCampos.IdTarea = opciones.IdTarea;
            actCampos.PermitirCreacion = opciones.PermitirCreacion;
            actCampos.PermitirModificacion = opciones.PermitirModificacion;
            actCampos.SiempreEditarEnGestion = opciones.SiempreEditarEnGestion;
            actCampos.SoloFaltantesEditarEnGestion = opciones.SoloFaltantesEditarEnGestion;
            actCampos.Campos = new List<Campo>();
            foreach(TareaClienteActualizacionCampo campo in data)
            {
                Campo setter = new Campo();
                setter.IdCampo = campo.IdCampo;
                setter.C = campo.Creacion;
                setter.M = campo.Modificacion;
                setter.G = campo.Gestion;
                setter.O = campo.Parametro.EsObligatorio;
                actCampos.Campos.Add(setter);
            }
            return Ok(actCampos);
        }
    }
}