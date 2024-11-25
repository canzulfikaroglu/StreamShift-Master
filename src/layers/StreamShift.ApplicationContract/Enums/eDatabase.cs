using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamShift.ApplicationContract.Enums
{
    public enum eDatabaseSource : byte
    {
        MsSqlServer = 0,
        Postgres = 1,
        Sqlite = 2,
        Mongodb = 3,
        Redis = 4,
        Excel = 5,
        Json = 6,
        Xml = 7
    }
    public enum eDatabaseTarget : byte
    {
        MsSqlServer = 0,
        Postgres = 1,
        Sqlite = 2,
        Mongodb = 3,
        Redis = 4,
        Excel = 5,
        Json = 6,
        Xml = 7
    }
}
