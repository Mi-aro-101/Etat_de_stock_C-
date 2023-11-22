using System;
using System.Net.Http;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Etat_de_stock_client.Models
{

    public class Mouvement
    {
        public virtual Magasin magasin { get; set; }
        public virtual Article article { get; set; }
        public double quantite { get; set; }

        public int GetNextValSeq()
        {
            string className = this.GetType().Name;
            string seqName = $"{className}_seq";
            var sql = $"SELECT nextval('{seqName}')";

            int val = 0;

            using (StockContext ctx = new StockContext())
            {
                using (var command = ctx.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    ctx.Database.OpenConnection();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            val = reader.GetInt32(0);
                        }
                    }
                }
            }

            return val;
        }

        public string ConstructPrimaryKey(string prefix)
        {
            string primarykey = prefix;
            int nextSeq = this.GetNextValSeq();
            int length = 7 - 3 - nextSeq.ToString().Length;
            int i = 0;
            while(i < length)
            {
                primarykey += "0";
                i++;
            }

            return primarykey + nextSeq;
        }
    }
}