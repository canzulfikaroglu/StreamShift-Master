using Microsoft.EntityFrameworkCore;
using StreamShift.Application.DTOs;
using StreamShift.Domain.DatabaseEnums;
using StreamShift.Infrastructure.ContextFactories.Abstract;
using StreamShift.Infrastructure.Extension;
using StreamShift.Infrastructure.Services.TransferService.Abstract;
using StreamShift.Infrastructure.Helpers.DynamicQueryHelper;
using MongoDB.Bson;
using Microsoft.Data.SqlClient.DataClassification;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MongoDB.Bson.IO;


namespace StreamShift.Infrastructure.Services.TransferService.Concrete
{
    public class TransferService : ITransferService
    {
        private readonly IDbContextFactory _contextFactory;

        public TransferService(IDbContextFactory dbContextFactory)
        {
            _contextFactory = dbContextFactory;
        }
        public async Task TransferDatabase(string sourceConnectionString, eDatabase sourceType, string destinationConnectionString, eDatabase destinationType)
        {//breakpoint yerim
            using (var _sourceDbContext = _contextFactory.CreateDbContext(sourceConnectionString, sourceType))
            {
                var sql = sourceType.GetSchemaQuery().Trim();
                var sourceSchema = await _sourceDbContext.Database.SqlQueryRaw<TableSchema>(sql).ToListAsync();//tablo adlarını listeye aldık.

                using (var _destinationDbContext = _contextFactory.CreateDbContext(destinationConnectionString, destinationType))
                {
                //    QueryExtensions.TableSequenceCreate(sourceSchema, _destinationDbContext);//sekans oluşturur

                    var schemas = sourceSchema.GroupBy(x => x.SchemaName).Select(x => x.Key).ToList();
                    foreach (var schema in schemas)
                    {
                        var tables = sourceSchema.Where(x => x.SchemaName == schema).GroupBy(x => x.TableName).Select(x => x.Key).ToList();
                        foreach (var table in tables)
                        {
                            var existResult = destinationType.TableExistsQuery(table);
                            var isExsist = _destinationDbContext.Database.SqlQueryRaw<TableExist>(existResult).ToList();
                            if (isExsist != null)
                            {
                                if (isExsist.Count() > 0)
                                {
                                    if (!isExsist.FirstOrDefault().Exist)
                                    {
                                        var columns = sourceSchema.Where(x => x.SchemaName == schema && x.TableName == table).Select(x => new TableSchema
                                        {
                                            ColumnName = x.ColumnName,
                                            DataType = x.DataType,
                                            MaxLength = x.MaxLength,
                                            SchemaName = x.SchemaName,
                                            IsPrimaryKey = x.IsPrimaryKey,
                                            TableName = x.TableName,
                                            DefaultValue = x.DefaultValue,
                                            IsNotNull = x.IsNotNull
                                        });
                                        var createTableQuery = destinationType.GetCreateTableQuery(table, columns);
                                        await _destinationDbContext.Database.ExecuteSqlRawAsync(createTableQuery);
                                        var deneme = new ExecuteDynamicQuerycs();
                                        var selectQuery = $"SELECT * FROM {schema}.{table}";
                                        var rows = await deneme.ExecuteDynamicQuery(_sourceDbContext, selectQuery);
                                        //   QueryExtensions.GetInsertTable(sourceSchema, _destinationDbContext,rows);
                                        //kaynak veritabanındaki verileri forech döngüsü ile konsola yazdırma 
                                        string insert = "";
                                        //  QueryExtensions.InsertPreparation(rows, out insert, table);
                                        int count = 0;
                                        string columnName = "";
                                        string dataName = "";
                                        string toplukolonlar = "";
                                        string topludegerler = "";

                                        
                                        for (int i = 0; i < rows.Count; i++)
                                        {
                                            Console.WriteLine("index sayisi" + i);
                                          
                                            foreach (var pair in rows[i])
                                            {
                                                
                                             
                                           //       Console.WriteLine("kolon adı: "+pair.Key+ " deger: " +pair.Value);


                                                QueryExtensions.InsertPreparation(pair.Key,pair.Value,rows, table,out columnName,out dataName);

                                            //, ve '' ları fonksiyonun içinde ayarlayıp aşşağıda insert komutuna eşitleyip execute edicez

                                                toplukolonlar = toplukolonlar+","+ columnName;
                                                topludegerler = topludegerler+","+ dataName;

                                                //burada gelen key ve value değerlerini alıp düzenlemesi yapılacak 


                                            }
                                            if (toplukolonlar.Length > 0 && toplukolonlar[0] == ',')
                                            {
                                                // İlk karakterdeki virgülü kaldır
                                                toplukolonlar = toplukolonlar.Substring(1);
                                            //    Console.WriteLine("İlk karakterdeki virgül kaldırıldı. Yeni string: " + toplukolonlar);
                                            }
                                            if (topludegerler.Length > 0 && topludegerler[0] == ',')
                                            {
                                                // İlk karakterdeki virgülü kaldır
                                                topludegerler = topludegerler.Substring(1);
                                            //    Console.WriteLine("İlk karakterdeki virgül kaldırıldı. Yeni string: " + topludegerler);
                                            }
                                            insert = $@"INSERT INTO {table} ({toplukolonlar}) VALUES ({topludegerler})";
                                            Console.WriteLine(insert);
                                            var insertDatabase = _destinationDbContext.Database.ExecuteSqlRaw(insert);
                                            //    Console.WriteLine("kolonlar : " + toplukolonlar + " data : " + topludegerler);
                                            topludegerler = "";
                                            toplukolonlar = "";

                                            //insert komutu burada çalışcak
                                        } 
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}