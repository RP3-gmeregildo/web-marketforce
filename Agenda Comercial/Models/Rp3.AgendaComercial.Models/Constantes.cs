using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models
{
    public class Constantes
    {
        public const string DateFormat = "dd/MM/yyyy HH:mm";
        public const int IdTodosMisAgentes = -1;

        public class GrupoClienteCampo
        {
            public const int PersonaNatural = 1;
            public const int PersonaJuridica = 2;
            public const int Persona = 3;
            public const int Direccion = 4;
            public const int Contacto = 5;
        }
       
        public class AgendaGroupMode
        {
            public const string Calendario = "D";
            public const string Cliente = "C";
            public const string Agente = "A";
        }

        public class OportunidadGroupMode
        {
            public const string Calendario = "D";
            public const string Agente = "A";
            public const string Estado = "E";
            public const string Listado = "L";
        }

        public class ProfileFotosSize
        {
            public const int ProfilePictureWidth = 300;
            public const int ProfilePictureHeight = 300;
            public const int ProfilePictureOtherProportion = 525;

            public const int ProfilePictuteMedWidth = 200;
            public const int ProfilePictuteMedHeight = 200;
            public const int ProfilePictureMedOtherProportion = 350;

            public const int ProfilePictuteSmaWidth = 150;
            public const int ProfilePictuteSmaHeight = 150;
            public const int ProfilePictureSmaOtherProportion = 263;

            public const int ProfilePictuteMinWidth = 100;
            public const int ProfilePictuteMinHeight = 100;
            public const int ProfilePictureMinOtherProportion = 175;

            public const int MaxSizeUploadFile = 5242880;//2 MB;
            public const string ClienteImagePath = "~/Content/AgendaComercial/img/clientes";
            public const string ClienteImageSavePath = "/Content/AgendaComercial/img/clientes/";     
        }

        public class AgendaMedia
        {
            public const string FilePath = "~/Content/AgendaComercial/img/agenda";
            public const string FileSavePath = "/Content/AgendaComercial/img/agenda/";
        }

        public class OportunidadMedia
        {
            public const string FilePath = "~/Content/AgendaComercial/img/oportunidades";
            public const string FileSavePath = "/Content/AgendaComercial/img/oportunidades/";
            public const string NewFilePath = @"Content\AgendaComercial\img\oportunidades";
        }

        public class ProductoMedia
        {
            public const string FilePath = "~/Content/AgendaComercial/img/productos";
            public const string FileSavePath = "/Content/AgendaComercial/img/productos/";
            public const string NewFilePath = @"Content\AgendaComercial\img\productos";
        }

        public class LogsMedia
        {
            public const string NewFilePath = @"Content\AgendaComercial\img\log";
        }

        public class MotivosNoGestion
        {            
            public const short Tabla = 1030;
        }

        public class MotivosReprogramacion
        {
            public const short Tabla = 1031;
        }

        public class TiposAgendaMedia
        {
            public const string Imagenes = "IMG";            
            public const short Tabla = 1029;
        }
        public class Estado
        {
            public const string Activo = "A";
            public const string Inactivo = "I";
            public const string Eliminado = "ELIM";
            public const short Tabla = 1001;
        }
        public class EstadoCivil
        {
            public const string Soltero = "S";
            public const string Casado = "C";
            public const string Divorciado = "D";
            public const string Viudo = "V";
            public const string UnionLibre = "U";
            public const short Tabla = 1002;
        }
        public class Genero
        {
            public const string Masculino = "M";
            public const string Femenino = "F";
            public const short Tabla = 1003;
        }

        public class TipoDireccion
        {
            public const string Domicilio = "D";
            public const string Trabajo = "T";
            public const string Sucursal = "S";
            public const short Tabla = 1004;
        }

        public class Importancia
        {
            public const string Alta = "A";
            public const string Media = "M";
            public const string Baja = "B";

            public const short Tabla = 1005;
        }

        public class TipoReunion
        {
            public const string Normal = "N";

            public const short Tabla = 1006;
        }

        public class EstadoReunion
        {
            public const string Activa = "A";
            public const string Cancelada = "C";

            public const short Tabla = 1007;
        }

        public class TipoMemo
        {
            public const string Normal = "N";

            public const short Tabla = 1008;
        }

        public class EstadoMemo
        {
            public const string Activo = "A";
            public const string Cancelado = "C";

            public const short Tabla = 1009;
        }

        public class TipoTarea
        {
            public const string Encuesta = "E";
            public const string Revision = "R";
            public const string Actividad = "A";
            public const string ActualizacionClientes = "ADC";
            public const string CheckListOportunidad = "CO";

            public const short Tabla = 1010;
        }

        public class TipoActividad
        {
            public const string Checkbox = "C";
            public const string MultipleSeleccion = "M";
            public const string Seleccion = "S";
            public const string Texto = "T";
            public const string Grupo = "G";

            public const int DefaultCheckbox = 3;
            public const int DefaultMultipleSeleccion = 5;
            public const int DefaultSeleccion = 4;
            public const int DefaultTexto = 2;
            public const int DefaultGrupo = 1;

            public const short Tabla = 1011;
        }

        public class Duracion
        {
            public const string _15min = "15";
            public const string _30min = "30";
            public const string _45min = "45";
            public const string _60min = "60";
            public const string _90min = "90";
            public const string _120min = "120";
            public const string _150min = "150";
            public const string _180min = "180";  

            public const short Tabla = 1012;
        }

        public class EstadoAgenda
        {
            public const string Cancelada = "C";
            public const string Gestionada = "G";
            public const string NoVisitado = "NV";
            public const string Pendiente = "P";
            public const string Reprogramada = "R";
            public const string Visitada = "V";
            public const string SinProgramar = "SP";
            public const string Eliminado = "ELIM";
            

            public const short Tabla = 1013;
        }

        public class OrigenAgenda
        {
            public const string Movil = "M";
            public const string Web = "W";
            public const string Automatica = "A";

            public const short Tabla = 1034;
        }


        public class EstadoTarea
        {
            public const string Pendiente = "P";
            public const string Realizada = "R";

            public const short Tabla = 1014;
        }

        public class PatronProgramacion
        {
            public const string Diario = "D";
            public const string Semanal = "S";
            public const string Mensual = "M";
            public const short Tabla = 1032;
        }

        public class DiasSemana
        {
            public const string Lunes = "1";
            public const string Martes = "2";
            public const string Miercoles = "3";
            public const string Jueves = "4";
            public const string Viernes = "5";
            public const string Sabado = "6";
            public const string Domingo = "7";
            public const short Tabla = 1033;
        }

        public class TipoPersona
        {
            public const string Natural = "N";
            public const string Juridica = "J";
            public const string JuridicaPublica = "P";
            public const short Tabla = 1022;
        }

        public class GeopoliticalStructureType
        {
            public const int Pais = 1;
            public const int Provincia = 2;
            public const int Ciudad = 3;
            public const int Parroquia = 4;
        }

        public class IdentificationType
        {
            public const int RUC = 1;
            public const int Cedula = 2;
            public const int Pasaporte = 3;
        }

        public class TipoZona
        {
            public const string EstructuraGeopolitica = "P";
            public const string Geocerca = "G";

            public const short Tabla = 1036;
        }

        public class EstadoOportunidad
        {
            public const string Abierta = "A";
            public const string Concretado = "C";
            public const string NoConcretada = "NC";
            public const string Suspendida = "S";
            public const string Eliminado = "ELIM";

            public const short Tabla = 1037;
        }

        public class EstadoEtapaOportunidad
        {
            public const string Pendiente = "P";
            public const string Iniciada = "I";
            public const string Finalizada = "F";

            public const short Tabla = 1038;
        }

        public class DiasInactividad
        {
            public const short Tabla = 1039;
        }

        public class TipoResponsable
        {
            public const string Creador = "C";
            public const string Gestor = "G";
            public const string Interesado = "I";

            public const short Tabla = 1040;
        }

        public class TipoMarcacion
        {
            public const string InicioJornada1 = "J1";
            public const string FinJornada1 = "J2";
            public const string InicioJornada2 = "J3";
            public const string FinJornada2 = "J4";

            public const short Tabla = 1044;
        }

        public class TipoPermiso
        {
            public const short Tabla = 1041;

            public const string Atraso = "A";
            public const string Ausencia = "F";
        }

        public class MotivoPermiso
        {
            public const short Tabla = 1042;
        }

        public class EstadoPermiso
        {
            public const short Tabla = 1043;

            public const string Aprobado = "A";
            public const string Pendiente = "P";
            public const string Rechazado = "R"; 
            public const string Cancelado = "C"; 
        }

        public class TipoEntradaTrazabilidad
        {
            public const short Tabla = 1045;
            public const string Indeterminado = "(Indeterminado)";

            public const string Detenido = "DETENIDO";
            public const string EnMovimiento = "MOVIMIENTO";
            public const string Gestion = "GESTION";
            public const string PrimeraJornada = "J1";
            public const string Break = "J2";
            public const string SegundaJornada = "J3";
            public const string FinJornada = "J4";
        }

        public class TipoDescuento
        {
            public const short Tabla = 1046;

            public const string Porcentaje = "P";
            public const string Valor = "V";

        }

        public class EstadoPedido
        {
            public const short Tabla = 1047;

            public const string Pendiente = "P";
            public const string Cerrado = "C";

        }
    }
}

