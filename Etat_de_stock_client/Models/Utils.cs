using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Etat_de_stock_client.Models
{
    public class Utils
    {
        /// <summary>
        /// Order an array of EtatEntree[]  to desc by it's date
        /// </summary>
        /// <param name="etatentrees"></param>
        /// <returns></returns>
        public static EtatEntree[] ToDesc(EtatEntree[] etatentrees)
        {
            EtatEntree[] results = etatentrees.OrderByDescending(obj => obj.dateEtat).ToArray();

            return results;
        }

        /// <summary>
        /// Calculate the sum of qte of a Entree or Sortie
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mouvement"></param>
        /// <returns></returns>
        public static double SumQte<T>(T[] mouvement)where T : Mouvement
        {
            double result = 0;

            foreach(T t in mouvement)
            {
                result += t.quantite;
            }

            return result;
        }

        /// <summary>
        /// Parse a string to date
        /// </summary>
        /// <param name="date"></param>
        /// <returns>the DateTime type of the string in arg</returns>
        public static DateTime ParseToDade(string date)
        {
            DateTime parsedDate;
            if (DateTime.TryParse(date, out parsedDate))
            {
                return parsedDate;
            }
            else
            {
                throw new Exception("Parsing date failed");
            }
        }

        /// <summary>
        /// Recursively call the next EtatEntree if the first called is dried up
        /// </summary>
        /// <param name="article"></param>
        /// <param name="magasin"></param>
        /// <param name="quantite"></param>
        /// <param name="dateSortie"></param>
        /// <param name="etatentrees"></param>
        /// <param name="i"></param>
        /// <param name="sortie"></param>
        /// <returns>The size of the EtatEntree affected by the mouvement sortie</returns>
        public static Int16 Execute(Article article, Magasin magasin, double quantite, DateTime dateSortie, EtatEntree[] etatentrees, Int16 i, Sortie sortie)
        {
            double reste = etatentrees[i].quantiteReste - quantite;
            if (reste < 0)
            {
                etatentrees[i].quantiteReste = 0;
                quantite = -1 * reste;
                etatentrees[i].dateEtat = dateSortie;
                etatentrees[i].sortie = sortie;
                i++;
                Execute(article, magasin, quantite, dateSortie, etatentrees, i, sortie);
            }
            else
            {
                etatentrees[i].quantiteReste = reste;
                etatentrees[i].dateEtat = dateSortie;
                etatentrees[i].sortie = sortie;
                return i;
            }

            return i;
        }
    }
}