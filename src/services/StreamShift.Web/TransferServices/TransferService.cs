using Npgsql;
using StreamShift.ApplicationContract.Enums;
using StreamShift.Domain.interfaces;
using StreamShift.Persistence.Abstract;

namespace StreamShift.Web.TransferServices
{
    public class TransferService<TEntity> : ITransferService<TEntity> where TEntity : class
    {
        public Task TransferData(string sourceConnectionString, string destinationConnectionString, eDatabase DatabaseType)
        {      //kaynak veritabanı bağlantısı sağlandı
            using NpgsqlConnection SourceConnection = new NpgsqlConnection(sourceConnectionString);
               //hedef veritabanı bağlantısı sağlandı
            using NpgsqlConnection Destinationconnection = new NpgsqlConnection(sourceConnectionString);
            //shema isimleri statik olarak çekilecek
            /*select schema_name
   from information_schema.schemata where schema_name='public';*/

            //string dbname = NpgsqlCommand($"SELECT current_database() AS DatabaseName;");
          //  string dbname = Database.GetDbConnection().Database;

            throw new NotImplementedException();
        }
    }
}
