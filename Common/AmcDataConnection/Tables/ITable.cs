
namespace AMCDatabase
{
    interface ITable
    {
        string TableName { get; set; }
        string KeyValue { get; set; }
        string KeyIdentifier { get; set; }
        
        //string GetWhereSQL();
        void AddColumns();
        //void UpdateTable();

        bool InitTable();

    }
}
