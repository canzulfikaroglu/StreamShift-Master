using Microsoft.Data.SqlClient;
using Npgsql;
using StreamShift.ApplicationContract.Enums;
using StreamShift.Domain.interfaces;
using StreamShift.Persistence.Abstract;
using System.Data.Common;

namespace StreamShift.Web.TransferServices
{
    public class TransferService<TEntity> : ITransferService<TEntity> where TEntity : class
    {
        public async Task TransferData(string sourceConnectionString, string destinationConnectionString, eDatabaseSource DatabaseTypeSource, eDatabaseTarget DatabaseTypeTarget)
        {
            // Kaynak veritabanı bağlantısı
            await using var sourceConnection = new NpgsqlConnection(sourceConnectionString);
            await sourceConnection.OpenAsync();

            // Hedef veritabanı bağlantısı
            await using var destinationConnection = new NpgsqlConnection(destinationConnectionString);
            await destinationConnection.OpenAsync();
            sourceConnection.Open();
            //int pageSize = 1000; // Her seferde 1000 satır alın
            //int currentPage = 0;

            //while (true)
            //{
            //    var rows = await sourceConnection.GetRowsAsync(currentPage * pageSize, pageSize);
            //    if (rows.Count == 0)
            //        break;

            //    await destinationConnection.InsertRowsAsync(rows);
            //    currentPage++;
            //}
            // await destinationConnectionString.InsertRow
            //örnek veritabanı insert komutu
            await using var dataSource = NpgsqlDataSource.Create(sourceConnectionString);
            await using (var cmd = dataSource.CreateCommand("insert into \"musteriler\" values (11,'can','zulfikaroglu','can_azulfikaroglu@hotmail.com')"))
            {
                
                await cmd.ExecuteNonQueryAsync();
            }

        }
    }
}
