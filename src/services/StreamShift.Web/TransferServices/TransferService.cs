using Microsoft.Data.SqlClient;
using Npgsql;
using StreamShift.ApplicationContract.Enums;
using StreamShift.Domain.interfaces;
using StreamShift.Persistence.Abstract;

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

            try
            {
                // Kaynak veritabanındaki tüm tabloları al
                var getSourceTablesCommand = new NpgsqlCommand(
                    "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';",
                    sourceConnection);

                await using var sourceReader = await getSourceTablesCommand.ExecuteReaderAsync();

                while (await sourceReader.ReadAsync())
                {
                    string tableName = sourceReader.GetString(0);

                    // Tablo içeriğini oku
                    var getDataCommand = new NpgsqlCommand($"SELECT * FROM {tableName};", sourceConnection);
                    await using var tableDataReader = await getDataCommand.ExecuteReaderAsync();

                    // Hedef veritabanına tabloyu kopyala
                    while (await tableDataReader.ReadAsync())
                    {
                        // Burada hedef veritabanına veri yazma işlemini gerçekleştirin
                        var insertCommand = new NpgsqlCommand(
                            $"INSERT INTO {tableName} VALUES (12,'can1')", // Burayı tablo yapısına uygun şekilde tamamlayın
                            destinationConnection);
                        
                        // Örnek veri ekleme mantığı:
                        // insertCommand.Parameters.AddWithValue("@column1", tableDataReader["column1"]);
                        // insertCommand.Parameters.AddWithValue("@column2", tableDataReader["column2"]);
                        
                        await insertCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during data transfer: {ex.Message}", ex);
            }
        }
    }
}
