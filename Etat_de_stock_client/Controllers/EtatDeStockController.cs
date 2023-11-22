using Etat_de_stock_client.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Etat_de_stock_client.Controllers
{
    public class EtatDeStockController : Controller
    {
        // GET: EtatDeStock
        public ActionResult Index()
        {
            ViewBag.Magasins = new Magasin().FindAll();

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ConsulterEtatDeStock()
        {
            string idmagasin = Request.Form["magasin"];
            string datestring1 = Request.Form["dateInitiale"];
            string datestring2 = Request.Form["dateFinale"];

            DateTime date1 = Utils.ParseToDade(datestring1);
            DateTime date2 = Utils.ParseToDade(datestring2);

            // if the user did not specifiy a magasin then find all article in all magasin and get their EtatDeStock
            Magasin[] magasins = new Magasin().FindAll();
            // Else just give the given Magasin
            if (!idmagasin.Equals(""))
            {
                magasins = new Magasin().FindById(idmagasin.ToString());
            }

            // For test
            Article article = new Article("RIZ0002", "Vary mena", 1);
            Magasin magasin = new Magasin("MGS0002");

            EtatDeStock etat = new EtatDeStock();
            Stock[] stocks = await etat.GetStock(date1, date2, magasins);

            Entree[] response = await new Entree(new WebServiceClient("http://localhost:8080/")).GetEntreeInterval(date1, date2, article.idArticle, magasin.idMagasin);

            ViewBag.dateDebut = date1;
            ViewBag.dateFin = date2;
            ViewBag.stocks = stocks;
            ViewBag.magasins = magasins;
            ViewBag.response = response;

            return View();
        }
    }
}