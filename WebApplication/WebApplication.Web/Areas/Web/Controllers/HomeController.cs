using Microsoft.AspNetCore.Mvc;

namespace WebApplication.App.Web.Areas.Web.Controllers
{
	[Area("Web")]

	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
