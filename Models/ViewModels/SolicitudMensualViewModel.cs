using System.ComponentModel.DataAnnotations;

namespace VN_Center.Models.ViewModels
{
  public class SolicitudMensualViewModel
  {
    public string MesNombre { get; set; } = string.Empty; // Ej: "Ene 2023"
    public int Cantidad { get; set; }
    public int Anio { get; set; } // Para ordenar si es necesario
    public int Mes { get; set; }  // Para ordenar si es necesario
  }
}
