using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VN_Center.Models;

namespace VN_Center.Controllers;

public class DashboardsController : Controller
{
  public IActionResult Index() => View();
}
