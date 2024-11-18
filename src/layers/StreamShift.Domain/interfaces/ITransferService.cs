using Microsoft.AspNetCore.Mvc;
using StreamShift.ApplicationContract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;

namespace StreamShift.Domain.interfaces
{
    public interface ITransferService<TEntity> where TEntity : class
    {
        Task TransferData(string sourceConnectionString, string targetConnectionString,eDatabase databaseType);
    }

}
