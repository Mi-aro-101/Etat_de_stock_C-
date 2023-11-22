using System;
using System.Web.Mvc;
using Etat_de_stock_client.Models;
using System.Threading.Tasks;

namespace Etat_de_stock_client.Controllers
{
    public class SortieController : Controller
    {
        // GET: Sortie
        public ActionResult Index(string errormessage)
        {
            Magasin[] magasins = new Magasin().FindAll();
            Article[] articles = new Article().FindAll();

            ViewBag.magasins = magasins;
            ViewBag.articles = articles;
            ViewBag.error = errormessage;

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> InsererSortie()
        {
            string idArticle = Request.Form["article"];
            string idMagasin = Request.Form["magasin"];
            string quantitestring = Request.Form["quantite"];
            string dateSortieString = Request.Form["dateSortie"];

            // Transform the input value into object
            Article article = new Article().FindById(idArticle);
            Magasin magasin = new Magasin().FindById(idMagasin)[0];
            double quantite = Double.Parse(quantitestring);
            DateTime dateSortie = Utils.ParseToDade(dateSortieString);

            string message = "";
            Sortie sortie = new Sortie();
            try
            {
                message = await sortie.InsertSortie(article, magasin, quantite, dateSortie);
            }
            catch(Exception e)
            {
                message = e.Message;
            }

            ViewBag.message = message;

            return View();
        }
    }
}