using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VN_Center.Models;

namespace VN_Center.Controllers;

public class TablesController : Controller
{
  public IActionResult Basic() => View();
}
