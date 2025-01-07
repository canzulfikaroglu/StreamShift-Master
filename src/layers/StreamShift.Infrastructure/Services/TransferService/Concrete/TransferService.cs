using Microsoft.EntityFrameworkCore;
using StreamShift.Application.DTOs;
using StreamShift.Domain.DatabaseEnums;
using StreamShift.Infrastructure.ContextFactories.Abstract;
using StreamShift.Infrastructure.Extension;
using StreamShift.Infrastructure.Services.TransferService.Abstract;
using StreamShift.Infrastructure.Helpers.DynamicQueryHelper;
using MongoDB.Bson;


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
        {
            using (var _sourceDbContext = _contextFactory.CreateDbContext(sourceConnectionString, sourceType))
            {
                var sql = sourceType.GetSchemaQuery().Trim();
                var sourceSchema = await _sourceDbContext.Database.SqlQueryRaw<TableSchema>(sql).ToListAsync();//tablo adlarını listeye aldık.


                using (var _destinationDbContext = _contextFactory.CreateDbContext(destinationConnectionString, destinationType))
                {

                    QueryExtensions.TableSequenceCreate(sourceSchema, _destinationDbContext);//sekans oluşturur


                    var schemas = sourceSchema.GroupBy(x => x.SchemaName).Select(x => x.Key).ToList();
                    foreach (var schema in schemas)
                    {
                        var tables = sourceSchema.Where(x => x.SchemaName == schema).GroupBy(x => x.TableName).Select(x => x.Key).ToList();
                        foreach (var table in tables)
                        {
                            //   foreach(var value in values)
                            // {

                            //}
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

                                        var keyValueList = new List<Dictionary<string, object>>();
                                        new Dictionary<string, object>();
                                        foreach (var row in rows) { 
                                        
                                       
                                        
                                        }
                                      
        




                                            //foreach (var row in rows)
                                            //{
                                            //    // Satırı dinamik bir objeye dönüştür.
                                            //    dynamic dynamicRow = DynamicDataHandler.ParseRowToDynamic(row);

                                            //    // Dinamik nesneyi kullanarak işlem yapabilirsiniz.
                                            //    Console.WriteLine(dynamicRow.Column1);
                                            //    Console.WriteLine(dynamicRow.Column2);

                                            //    //var selectQuery = $"SELECT * FROM {table}";
                                            //    //var data = await _sourceDbContext.Database.SqlQueryRaw(selectQuery).ToListAsync();
                                            //    //foreach (var row in data)
                                            //    //{
                                            //    //    var insertQuery = destinationType.GetInsertQuery(table, columns, row);
                                            //    //    await _destinationDbContext.Database.ExecuteSqlRawAsync(insertQuery);
                                            //    //}
                                            //}
                                        
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