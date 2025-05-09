using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VN_Center.Models;

namespace VN_Center.Controllers;

public class AuthController : Controller
{
  public IActionResult ForgotPasswordBasic() => View();
  public IActionResult LoginBasic() => View();
  public IActionResult RegisterBasic() => View();
}
