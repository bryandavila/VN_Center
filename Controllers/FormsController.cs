using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VN_Center.Models;

namespace VN_Center.Controllers;

public class FormsController : Controller
{
  public IActionResult BasicInputs() => View();
  public IActionResult InputGroups() => View();
}
