using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VN_Center.Models;

namespace VN_Center.Controllers;

public class FormLayoutsController : Controller
{
public IActionResult Horizontal() => View();
public IActionResult Vertical() => View();
}
