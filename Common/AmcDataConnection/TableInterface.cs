using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMCDatabase
{
    interface TableInterface
    {
        string TableName { get; set; }
        string KeyValue { get; set; }
        string KeyIdentifier { get; set; }
        

        //string GetWhereSQL();
        void AddColums();
        void UpdateTable();
    }
}
