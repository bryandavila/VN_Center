// VN_Center/Models/ViewModels/ResultadosBusquedaViewModel.cs
using VN_Center.Models.Entities; // Asegúrate que este using apunte a tus entidades
using System.Collections.Generic;

namespace VN_Center.Models.ViewModels // Asegúrate que el namespace sea este
{
  public class ResultadosBusquedaViewModel
  {
    public string? TerminoBusqueda { get; set; }

    // Lista para cada tipo de entidad que quieres incluir en los resultados
    public List<UsuariosSistema> UsuariosEncontrados { get; set; } = new List<UsuariosSistema>();
    public List<Beneficiarios> BeneficiariosEncontrados { get; set; } = new List<Beneficiarios>();
    public List<ProgramasProyectosONG> ProgramasEncontrados { get; set; } = new List<ProgramasProyectosONG>();
    public List<Solicitudes> SolicitudesVolPasEncontradas { get; set; } = new List<Solicitudes>();
    public List<SolicitudesInformacionGeneral> SolicitudesInfoEncontradas { get; set; } = new List<SolicitudesInformacionGeneral>();
    public List<Comunidades> ComunidadesEncontradas { get; set; } = new List<Comunidades>();
    public List<GruposComunitarios> GruposEncontrados { get; set; } = new List<GruposComunitarios>();
    // Puedes añadir más listas para otras entidades según necesites

    public bool HayResultados => UsuariosEncontrados.Any() ||
                                 BeneficiariosEncontrados.Any() ||
                                 ProgramasEncontrados.Any() ||
                                 SolicitudesVolPasEncontradas.Any() ||
                                 SolicitudesInfoEncontradas.Any() ||
                                 ComunidadesEncontradas.Any() ||
                                 GruposEncontrados.Any();
  }
}
