using Etat_de_stock_client.Models;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etat_de_stock_client.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public async Task<ActionResult> Articles()
        {
            WebServiceClient Ws = new WebServiceClient("http://localhost:8080/");
            string endpoint = "api/entree/interval";

            var parameter = new Dictionary<string, string>
            {
                {"date1", "2022-06-01" },
                {"date2", "2024-01-01" },
                {"idArticle", "RIZ0002" },
                {"idMagasin", "MGS0002" }
            };


            string response = await Ws.CallWebService(endpoint, parameter);

            ViewBag.Articles = response;

            return View();
        }
    }
}