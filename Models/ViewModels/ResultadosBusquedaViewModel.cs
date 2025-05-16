// VN_Center/Models/ViewModels/ResultadosBusquedaViewModel.cs
using VN_Center.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace VN_Center.Models.ViewModels
{
  public class ResultadosBusquedaViewModel
  {
    public string? TerminoBusqueda { get; set; }

    public List<UsuariosSistema> UsuariosEncontrados { get; set; } = new List<UsuariosSistema>();
    public List<Beneficiarios> BeneficiariosEncontrados { get; set; } = new List<Beneficiarios>();
    public List<ProgramasProyectosONG> ProgramasEncontrados { get; set; } = new List<ProgramasProyectosONG>();
    public List<Solicitudes> SolicitudesVolPasEncontradas { get; set; } = new List<Solicitudes>();
    public List<SolicitudesInformacionGeneral> SolicitudesInfoEncontradas { get; set; } = new List<SolicitudesInformacionGeneral>();
    public List<Comunidades> ComunidadesEncontradas { get; set; } = new List<Comunidades>();
    public List<GruposComunitarios> GruposEncontrados { get; set; } = new List<GruposComunitarios>();
    public List<EvaluacionesPrograma> EvaluacionesEncontradas { get; set; } = new List<EvaluacionesPrograma>();
    public List<ParticipacionesActivas> ParticipacionesEncontradas { get; set; } = new List<ParticipacionesActivas>();

    // --- NUEVAS LISTAS PARA RESULTADOS DE MÓDULOS DE ADMINISTRADOR ---
    public List<RegistrosAuditoria> AuditoriaEncontrada { get; set; } = new List<RegistrosAuditoria>();
    public List<NivelesIdioma> NivelesIdiomaEncontrados { get; set; } = new List<NivelesIdioma>();
    public List<BeneficiariosProgramasProyectos> BeneficiariosEnProgramasEncontrados { get; set; } = new List<BeneficiariosProgramasProyectos>();
    // --- FIN DE NUEVAS LISTAS ---


    public bool HayResultados =>
        UsuariosEncontrados.Any() ||
        BeneficiariosEncontrados.Any() ||
        ProgramasEncontrados.Any() ||
        SolicitudesVolPasEncontradas.Any() ||
        SolicitudesInfoEncontradas.Any() ||
        ComunidadesEncontradas.Any() ||
        GruposEncontrados.Any() ||
        EvaluacionesEncontradas.Any() ||
        ParticipacionesEncontradas.Any() ||
        AuditoriaEncontrada.Any() || // <--- AÑADIDO A LA COMPROBACIÓN
        NivelesIdiomaEncontrados.Any() || // <--- AÑADIDO A LA COMPROBACIÓN
        BeneficiariosEnProgramasEncontrados.Any(); // <--- AÑADIDO A LA COMPROBACIÓN
  }
}
