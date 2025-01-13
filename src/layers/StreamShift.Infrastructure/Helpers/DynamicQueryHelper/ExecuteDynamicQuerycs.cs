using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace StreamShift.Infrastructure.Helpers.DynamicQueryHelper
{
    public class ExecuteDynamicQuerycs
    {


public async Task<List<dynamic>> ExecuteDynamicQuery(DbContext dbContext, string query)
    {
        var connection = dbContext.Database.GetDbConnection();

        try
        {
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = query;

            using var reader = await command.ExecuteReaderAsync();
            var results = new List<dynamic>();

            while (await reader.ReadAsync())
            {
                dynamic row = new ExpandoObject();
                var rowDict = (IDictionary<string, object>)row;

                for (var i = 0; i < reader.FieldCount; i++)
                {
                    rowDict[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                }
                   
                   
                results.Add(row);
            }
               
            return results;

        }
        finally
        {
            await connection.CloseAsync();
        }
    }
}
}
