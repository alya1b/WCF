using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfService
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        [OperationContract]
        void SaveDB(string path);
        [OperationContract]
        void OpenDB(string path);
        [OperationContract]
        bool AddTable(string TableName);
        [OperationContract]
        void DeleteTable(int tind);
        [OperationContract]
        bool AddColumn(int TableIndex, string ColumnName, string ColumnType);
        [OperationContract]
        void DeleteColumn(int tind, int cind);
        [OperationContract]
        bool RenameColumn(int TableIndex, int ColumnIndex, string newColumnName);
        [OperationContract]
        bool AddRow(int TableIndex);
        [OperationContract]
        void DeleteRow(int tind, int rind);
        [OperationContract]
        bool ChangeValue(string newValue, int tind, int cind, int rind);
        [OperationContract]
        List<string> GetTableNameList();
        [OperationContract]
        string GetCurrentDBName();
        [OperationContract]
        Table GetTable(int index);
        [OperationContract]
        bool CreateDB(string DBName);

        [OperationContract]
        Table Merge_Tables(string tableName1, string tableName2);
    }

    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
