using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models.General.View;
using Rp3.AgendaComercial.Models.Marcacion;
using Rp3.AgendaComercial.Models.Oportunidad;
using Rp3.AgendaComercial.Models.Pedido;
using Rp3.AgendaComercial.Models.Ruta;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models
{
    public class Context : Rp3.Data.DbConnection.DbContextManager
    {
        public Context(string connectionString)
            : base(connectionString)
        {
        }

        public DbSet<TareaResumenView> TareaResumenViews { get; set; }
        public DbSet<Frecuencia> Frecuencias { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<Canal> Canales { get; set; }
        public DbSet<Region> Regiones { get; set; }
        public DbSet<TipoCliente> TipoClientes { get; set; }
        public DbSet<Zona> Zonas { get; set; }
        public DbSet<ZonaDetalle> ZonaDetalles { get; set; }
        public DbSet<TipoCartera> TipoCarteras { get; set; }
        public DbSet<Agente> Agentes { get; set; }
        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<ClienteView> ClienteViews { get; set; }
        public DbSet<ClienteDireccion> ClienteDirecciones { get; set; }

        public DbSet<ClienteContacto> ClienteContactos { get; set; }
        public DbSet<Lote> Lotes { get; set; }
        public DbSet<LoteDetalle> LoteDetalles { get; set; }
        public DbSet<LoteTipoCliente> LoteTipoClientes { get; set; }
        public DbSet<LoteCanal> LoteCanales { get; set; }
        public DbSet<LoteZona> LoteZonas { get; set; }
        public DbSet<Ruta.Ruta> Rutas { get; set; }
        public DbSet<RutaDetalle> RutaDetalles { get; set; }
        public DbSet<RutaIncluir> RutaIncluirs { get; set; }
        public DbSet<RutaExcluir> RutaExcluirs { get; set; }
        public DbSet<RutaLote> RutaLotes { get; set; }
        public DbSet<AgenteUbicacion> AgenteUbicaciones { get; set; }
        public DbSet<Reunion> Reuniones { get; set; }
        public DbSet<ReunionAsistente> ReunionAsistentes { get; set; }
        public DbSet<Memo> Memos { get; set; }
        public DbSet<MemoDestinatario> MemoDestinatarios { get; set; }
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<TareaActividad> TareaActividades { get; set; }
        
        public DbSet<TareaRutaAplica> TareaRutaAplicas { get; set; }
        public DbSet<TipoActividad> TipoActividades { get; set; }
        public DbSet<TipoActividadOpcion> TipoActividadOpciones { get; set; }
        public DbSet<ProgramacionRuta> ProgramacionRutas { get; set; }
        public DbSet<ProgramacionRutaDetalle> ProgramacionRutaDetalles { get; set; }
        public DbSet<ProgramacionRutaTarea> ProgramacionRutaTareas { get; set; }
        public DbSet<Agenda> Agendas { get; set; }
        public DbSet<AgendaTarea> AgendaTareas { get; set; }
        public DbSet<AgendaTareaActividad> AgendaTareaActividades { get; set; }
        public DbSet<AgendaMedia> AgendaMedias { get; set; }
        public DbSet<Calendario> Calendarios { get; set; }
        public DbSet<DiaLaboral> DiasLaborales { get; set; }
        public DbSet<DiasNoLaborable> DiasNoLaborables { get; set; }
        public DbSet<Parametro> Parametros { get; set; }
        public DbSet<General.Process> Processes { get; set; }
        public DbSet<ProcessStep> ProcessSteps { get; set; }
        public DbSet<ProcessLog> ProcessLogs { get; set; }
        public DbSet<TareaClienteActualizacionCampo> TareaClienteActualizacionCampos { get; set; }
        public DbSet<ParametroClienteCampo> ParametroClienteCampos { get; set; }
        public DbSet<TareaClienteActualizacion> TareaClienteActualizaciones { get; set; }
        public DbSet<Oportunidad.Oportunidad> Oportunidades { get; set; }
        public DbSet<OportunidadContacto> OportunidadContactos { get; set; }
        public DbSet<OportunidadMedia> OportunidadMedias { get; set; }
        public DbSet<OportunidadResponsable> OportunidadResponsables { get; set; }
        public DbSet<OportunidadTarea> OportunidadTareas { get; set; }
        public DbSet<OportunidadTareaActividad> OportunidadTareaActividad { get; set; }
        public DbSet<Etapa> Etapas { get; set; }
        public DbSet<EtapaTarea> EtapaTareas { get; set; }

        public DbSet<Permiso> Permisos { get; set; }

        public DbSet<Marcacion.Marcacion> Marcaciones { get; set; }

        public DbSet<InformeTrazabilidad> InformeTrazabilidads { get; set; }

        public DbSet<OportunidadBitacora> OportunidadBitacoras { get; set; }
        public DbSet<OportunidadTipo> OportunidadTipos { get; set; }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Descuento> Descuentos { get; set; }
        public DbSet<Pedido.Pedido> Pedidos { get; set; }
        public DbSet<PedidoDetalle> PedidoDetalles { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<SubCategoria> SubCategorias { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Agente>().HasMany(p => p.Agentes).WithOptional(p => p.Supervisor);

            modelBuilder.Entity<Agente>().HasOptional(p => p.Usuario);

            modelBuilder.Entity<Region>().HasMany(p => p.Zonas).WithRequired(p => p.Region);

            modelBuilder.Entity<Cargo>().HasMany(p => p.Agentes).WithRequired(p => p.Cargo);

            modelBuilder.Entity<Zona>().HasMany(p => p.ZonaDetalles).WithRequired(p => p.Zona);

            modelBuilder.Entity<Zona>().HasMany(p => p.ZonaGeocercas).WithRequired(p => p.Zona);

            modelBuilder.Entity<Zona>().HasMany(p => p.ZonaClienteGeocercas).WithRequired(p => p.Zona);

            modelBuilder.Entity<ZonaClienteGeocerca>().HasRequired(p => p.ClienteDireccion);

            modelBuilder.Entity<Cliente>().HasOptional(p => p.TipoIdentificacion);

            modelBuilder.Entity<Cliente>().HasOptional(p => p.ClienteDato).WithRequired(p => p.Cliente);

            modelBuilder.Entity<Cliente>().HasMany(p => p.ClienteDirecciones).WithRequired(p => p.Cliente);

            modelBuilder.Entity<Cliente>().HasMany(p => p.ClienteContactos).WithRequired(p => p.Cliente);

            modelBuilder.Entity<ClienteContacto>().HasOptional(p => p.ClienteDireccion);

            modelBuilder.Entity<ClienteDireccion>().HasOptional(p => p.Ciudad);

            modelBuilder.Entity<TipoCliente>().HasMany(p => p.Clientes).WithOptional(p => p.TipoCliente);

            modelBuilder.Entity<Canal>().HasMany(p => p.Clientes).WithOptional(p => p.Canal);

            modelBuilder.Entity<Lote>().HasMany(p => p.LoteDetalles).WithRequired(p => p.Lote);

            modelBuilder.Entity<Lote>().HasMany(p => p.LoteTipoClientes).WithRequired(p => p.Lote);

            modelBuilder.Entity<Lote>().HasMany(p => p.LoteCanales).WithRequired(p => p.Lote);

            modelBuilder.Entity<Lote>().HasMany(p => p.LoteZonas).WithRequired(p => p.Lote);

            modelBuilder.Entity<LoteDetalle>().HasRequired(p => p.ClienteDireccion);

            modelBuilder.Entity<LoteTipoCliente>().HasRequired(p => p.TipoCliente);

            modelBuilder.Entity<LoteCanal>().HasRequired(p => p.Canal);

            modelBuilder.Entity<LoteZona>().HasRequired(p => p.Zona);

            modelBuilder.Entity<Ruta.Ruta>().HasMany(p => p.Agentes).WithOptional(p => p.Ruta);

            modelBuilder.Entity<Ruta.Ruta>().HasMany(p => p.RutaDetalles).WithRequired(p => p.Ruta);

            modelBuilder.Entity<Ruta.Ruta>().HasMany(p => p.RutaIncluirs).WithRequired(p => p.Ruta);

            modelBuilder.Entity<Ruta.Ruta>().HasMany(p => p.RutaExcluirs).WithRequired(p => p.Ruta);

            modelBuilder.Entity<Ruta.Ruta>().HasMany(p => p.RutaLotes).WithRequired(p => p.Ruta);

            modelBuilder.Entity<Ruta.Ruta>().HasOptional(p => p.Calendario);


            modelBuilder.Entity<RutaDetalle>().HasRequired(p => p.ClienteDireccion);

            modelBuilder.Entity<RutaIncluir>().HasRequired(p => p.ClienteDireccion);

            modelBuilder.Entity<RutaExcluir>().HasRequired(p => p.ClienteDireccion);

            modelBuilder.Entity<RutaLote>().HasRequired(p => p.Lote);

            modelBuilder.Entity<Agente>().HasMany(p => p.AgenteUbicaciones).WithRequired(p => p.Agente);

            modelBuilder.Entity<AgenteUltimaUbicacion>().HasRequired(p => p.Agente);

            modelBuilder.Entity<Reunion>().HasMany(p => p.ReunionAsistentes).WithRequired(p => p.Reunion);

            modelBuilder.Entity<ReunionAsistente>().HasRequired(p => p.Agente);

            modelBuilder.Entity<Memo>().HasMany(p => p.MemoDestinatarios).WithRequired(p => p.Memo);

            modelBuilder.Entity<MemoDestinatario>().HasRequired(p => p.Agente);

            modelBuilder.Entity<Tarea>().HasOptional(p => p.TareaClienteActualizacion);

            modelBuilder.Entity<Tarea>().HasMany(p => p.TareaActividades).WithRequired(p => p.Tarea);

            modelBuilder.Entity<Tarea>().HasMany(p => p.TareaRutaAplicas).WithRequired(p => p.Tarea);

            modelBuilder.Entity<Tarea>().HasMany(p => p.TareaClienteActualizacionCampos).WithRequired(p => p.Tarea);

            modelBuilder.Entity<TareaActividad>().HasRequired(p => p.TipoActividad);

            modelBuilder.Entity<TareaRutaAplica>().HasRequired(p => p.Ruta);

            modelBuilder.Entity<TipoActividad>().HasMany(p => p.TipoActividadOpciones).WithRequired(p => p.TipoActividad);

            modelBuilder.Entity<ProgramacionRuta>().HasRequired(p => p.Ruta);

            //modelBuilder.Entity<ProgramacionRuta>().HasMany(p => p.ProgramacionRutaDetalles).WithRequired(p => p.ProgramacionRuta);

            //modelBuilder.Entity<ProgramacionRuta>().HasMany(p => p.Agendas).WithOptional(p => p.ProgramacionRuta);

            modelBuilder.Entity<ProgramacionRutaDetalle>().HasRequired(p => p.ClienteDireccion);

            //modelBuilder.Entity<Agenda>().HasOptional(p => p.ProgramacionRuta);

            modelBuilder.Entity<Agenda>().HasRequired(p => p.Ruta);

            modelBuilder.Entity<Agenda>().HasRequired(p => p.ClienteDireccion);

            modelBuilder.Entity<Agenda>().HasOptional(p => p.ClienteContacto);

            modelBuilder.Entity<Agenda>().HasMany(p => p.AgendaTareas).WithRequired(p => p.Agenda);

            modelBuilder.Entity<Agenda>().HasMany(p => p.AgendaMedias).WithRequired(p => p.Agenda);

            modelBuilder.Entity<AgendaTarea>().HasRequired(p => p.Tarea);

            modelBuilder.Entity<AgendaTarea>().HasMany(p => p.AgendaTareaActividades).WithRequired(p => p.AgendaTarea);

            modelBuilder.Entity<AgendaTareaActividad>().HasRequired(p => p.TipoActividad);

            modelBuilder.Entity<Calendario>().HasMany(p => p.DiasLaborales).WithRequired(p=> p.Calendario);

            modelBuilder.Entity<Calendario>().HasMany(p => p.DiasNoLaborables).WithRequired(p => p.Calendario);

            modelBuilder.Entity<TareaClienteActualizacionCampo>().HasRequired(p => p.Parametro);


            modelBuilder.Entity<Etapa>().HasMany(p => p.EtapaTareas).WithRequired(p => p.Etapa);

            modelBuilder.Entity<Etapa>().HasRequired(p => p.OportunidadTipo);

            modelBuilder.Entity<EtapaTarea>().HasRequired(p => p.Tarea);

            modelBuilder.Entity<Oportunidad.Oportunidad>().HasMany(p => p.OportunidadContactos).WithRequired(p => p.Oportunidad);

            modelBuilder.Entity<Oportunidad.Oportunidad>().HasMany(p => p.OportunidadMedias).WithRequired(p => p.Oportunidad);

            modelBuilder.Entity<Oportunidad.Oportunidad>().HasMany(p => p.OportunidadResponsables).WithRequired(p => p.Oportunidad);

            modelBuilder.Entity<Oportunidad.Oportunidad>().HasMany(p => p.OportunidadTareas).WithRequired(p => p.Oportunidad);

            modelBuilder.Entity<Oportunidad.Oportunidad>().HasMany(p => p.OportunidadEtapas).WithRequired(p => p.Oportunidad);

            modelBuilder.Entity<Oportunidad.Oportunidad>().HasMany(p => p.OportunidadBitacoras).WithRequired(p => p.Oportunidad);

            modelBuilder.Entity<Oportunidad.Oportunidad>().HasRequired(p => p.OportunidadTipo);

            modelBuilder.Entity<Oportunidad.Oportunidad>().HasRequired(p => p.Agente);

            modelBuilder.Entity<Oportunidad.OportunidadEtapa>().HasMany(p => p.OportunidadTareas).WithRequired(p => p.OportunidadEtapa);

            modelBuilder.Entity<Oportunidad.OportunidadTarea>().HasMany(p => p.OportunidadTareaActividads).WithRequired(p => p.OportunidadTarea);

            modelBuilder.Entity<Oportunidad.OportunidadTarea>().HasRequired(p => p.Tarea);


            modelBuilder.Entity<Agente>().HasOptional(p => p.Grupo);//.WithOptionalPrincipal();

            modelBuilder.Entity<Grupo>().HasRequired(p => p.Calendario);

            modelBuilder.Entity<Marcacion.Marcacion>().HasRequired(p => p.Agente);

            modelBuilder.Entity<Marcacion.Marcacion>().HasRequired(p => p.Grupo);

            modelBuilder.Entity<Marcacion.Marcacion>().HasOptional(p => p.Permiso);

            modelBuilder.Entity<Marcacion.Permiso>().HasOptional(p => p.Agente);

            modelBuilder.Entity<Marcacion.Permiso>().HasOptional(p => p.Grupo);

            modelBuilder.Entity<Producto>().HasMany(p => p.Descuentos).WithRequired(p => p.Producto);

            modelBuilder.Entity<Pedido.Pedido>().HasOptional(p => p.Agenda);

            modelBuilder.Entity<Pedido.Pedido>().HasRequired(p => p.Cliente);

            modelBuilder.Entity<Categoria>().HasMany(p => p.SubCategorias).WithRequired(p => p.Categoria);

        }
    }
}


