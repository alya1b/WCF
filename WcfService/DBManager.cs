using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    [DataContract]
    public class DBManager
    {
        [DataMember]
        Database database;

        public bool CreateDB(string DBName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(DBName))
                    throw new ArgumentException("Назва бази даних не може бути пустою.");

                database = new Database(DBName);
                return true;
            }
            catch (ArgumentException ex)
            {

                return false; 
            }
            catch (Exception ex)
            {

                return false; 
            }
        }

        private const char sep = '$';
        private const char space = '#';
        public void SaveDB(string path)
        {
            try
            {
                if (database == null || database.DBTablesList.Count == 0)
                {
                    throw new ArgumentException("Error.");
                    return;
                }

                StreamWriter sw = new StreamWriter(path);

                sw.WriteLine(database.DBName);
                foreach (Table t in database.DBTablesList)
                {
                    sw.WriteLine(sep);
                    sw.WriteLine(t.TableName);
                    foreach (Column c in t.TableColumnsList)
                    {
                        sw.Write(c.ColumnName + space);
                    }
                    sw.WriteLine();
                    foreach (Column c in t.TableColumnsList)
                    {
                        sw.Write(c.TypeName + space);
                    }
                    sw.WriteLine();
                    foreach (Row r in t.TableRowsList)
                    {
                        foreach (string s in r.RowValuesList)
                        {
                            sw.Write(s + space);
                        }
                        sw.WriteLine();
                    }
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error.");
            }
        }

        public void OpenDB(string path)
        {
            StreamReader sr = new StreamReader(path);
            string file = sr.ReadToEnd();
            string[] parts = file.Split(sep);

            database = new Database(parts[0]);

            for (int i = 1; i < parts.Length; ++i)
            {
                parts[i] = parts[i].Replace("\r\n", "\r");
                List<string> buf = parts[i].Split('\r').ToList();
                buf.RemoveAt(0);
                buf.RemoveAt(buf.Count - 1);

                if (buf.Count > 0)
                {
                    database.DBTablesList.Add(new Table(buf[0]));
                }
                if (buf.Count > 2)
                {
                    string[] cname = buf[1].Split(space);
                    string[] ctype = buf[2].Split(space);
                    int length = cname.Length - 1;
                    for (int j = 0; j < length; ++j)
                    {
                        database.DBTablesList[i - 1].TableColumnsList.Add(new Column(cname[j], ctype[j]));
                    }

                    for (int j = 3; j < buf.Count; ++j)
                    {
                        database.DBTablesList[i - 1].TableRowsList.Add(new Row());
                        List<string> values = buf[j].Split(space).ToList();
                        values.RemoveAt(values.Count - 1);
                        database.DBTablesList[i - 1].TableRowsList.Last().RowValuesList.AddRange(values);
                    }
                }
            }

            sr.Close();
        }

        public bool AddTable(string TableName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TableName))
                    throw new ArgumentException("Таблиця не може бути порожньою.");

                if (database == null)
                    throw new InvalidOperationException("База даних не створена.");

                database.DBTablesList.Add(new Table(TableName));
                return true;
            }
            catch (ArgumentException ex)
            {
                return false;
            }
            catch (InvalidOperationException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public void DeleteTable(int tind)
        {
            database.DBTablesList.RemoveAt(tind);
        }

        public bool AddColumn(int TableIndex, string ColumnName, string ColumnType)
        {

            try
            {
                if (database == null)
                    throw new InvalidOperationException("База даних не створена.");


                if (database.DBTablesList.Count <= 0)
                    throw new InvalidOperationException("Таблиця не створена.");
                if (string.IsNullOrWhiteSpace(ColumnName))
                    throw new ArgumentException("Назва колонки не може бути порожньою.");

                Table table = database.DBTablesList[TableIndex];
                table.TableColumnsList.Add(new Column(ColumnName, ColumnType));

                if (table.TableRowsList.Count == 0)
                {
                    table.TableRowsList.Add(new Row());

                    for (int i = 0; i < table.TableColumnsList.Count; ++i)
                    {
                        table.TableRowsList[0].RowValuesList.Add("");
                    }
                }
                else
                {
                    for (int i = 0; i < table.TableRowsList.Count; ++i)
                    {
                        table.TableRowsList[i].RowValuesList.Add("");
                    }
                }

                return true;
            }
            catch (InvalidOperationException ex)
            {
                return false;
            }
            catch (ArgumentException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public void DeleteColumn(int tind, int cind)
        {
            if (database == null || tind < 0 || tind >= database.DBTablesList.Count)
                return;

            Table table = database.DBTablesList[tind];
            if (cind < 0 || cind >= table.TableColumnsList.Count)
                return;

            string columnNameToDelete = table.TableColumnsList[cind].ColumnName;

            table.TableColumnsList.RemoveAt(cind);

            foreach (Row r in table.TableRowsList)
            {
                if (cind < r.RowValuesList.Count)
                {
                    r.RowValuesList.RemoveAt(cind);
                }
            }
        }

        public bool RenameColumn(int TableIndex, int ColumnIndex, string newColumnName)
        {
            try
            {
                if (database == null)
                    throw new InvalidOperationException("База даних не створена.");

                if (TableIndex < 0 || TableIndex >= database.DBTablesList.Count)
                    throw new ArgumentOutOfRangeException("Неправильний індекс таблиці.");

                Table table = database.DBTablesList[TableIndex];
                if (ColumnIndex < 0 || ColumnIndex >= table.TableColumnsList.Count)
                    throw new ArgumentOutOfRangeException("Неправильний індекс колонки.");

                if (string.IsNullOrWhiteSpace(newColumnName))
                    throw new ArgumentException("Назва не може бути порожньою.");

                if (table.TableColumnsList.Any(column => column.ColumnName == newColumnName))
                    throw new ArgumentException("Колонка з такою назвою вже існує.");

                table.TableColumnsList[ColumnIndex].ColumnName = newColumnName;

                return true;
            }
            catch (InvalidOperationException ex)
            {
                return false;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return false;
            }
            catch (ArgumentException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool AddRow(int TableIndex)
        {
            try
            {
                if (database == null) return false;
                if (database.DBTablesList.Count <= 0) return false;
                if (database.DBTablesList[TableIndex].TableColumnsList.Count <= 0) return false;

                database.DBTablesList[TableIndex].TableRowsList.Add(new Row());
                for (int i = 0; i < database.DBTablesList[TableIndex].TableColumnsList.Count; ++i)
                {
                    database.DBTablesList[TableIndex].TableRowsList.Last().RowValuesList.Add("");
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void DeleteRow(int tind, int rind)
        {
            if (database == null || tind < 0 || tind >= database.DBTablesList.Count)
                return; 

            Table table = database.DBTablesList[tind];
            if (rind < 0 || rind >= table.TableRowsList.Count)
                return; 

            if (table.TableRowsList.Count > 1) 
            {
                table.TableRowsList.RemoveAt(rind);
            }
        }

        public bool ChangeValue(string newValue, int tind, int cind, int rind)
        {
            if (tind >= 0 && tind < database.DBTablesList.Count &&
                cind >= 0 && cind < database.DBTablesList[tind].TableColumnsList.Count &&
                rind >= 0 && rind < database.DBTablesList[tind].TableRowsList.Count)
            {
                if (database.DBTablesList[tind].TableColumnsList[cind].ColumnType.Validation(newValue))
                {
                    database.DBTablesList[tind].TableRowsList[rind].RowValuesList[cind] = newValue;
                    return true;
                }
            }

            return false;
        }

        public List<string> GetTableNameList()
        {
            List<string> res = new List<string>();
            foreach (Table t in database.DBTablesList)
                res.Add(t.TableName);
            return res;
        }

        public string GetCurrentDBName()
        {
            if (database != null)
            {
                return database.DBName;
            }
            else
            {
                return "База даних не відкрита";
            }
        }

        public Table GetTable(int index)
        {
            if (index == -1) index = 0;
            return database.DBTablesList[index];
        }

        public Table Merge_Tables(string tableName1, string tableName2)
        {
            // Find tables by their names
            Table table1 = database.DBTablesList.FirstOrDefault(t => t.TableName == tableName1);
            Table table2 = database.DBTablesList.FirstOrDefault(t => t.TableName == tableName2);

            if (table1 == null || table2 == null)
            {
                // Return null if either table is not found
                return null;
            }

            // Check if tables have the same number of columns
            if (table1.TableColumnsList.Count != table2.TableColumnsList.Count)
            {
                // Return null if columns count doesn't match
                return null;
            }

            // Check if columns match by name and type
            for (int i = 0; i < table1.TableColumnsList.Count; i++)
            {
                if (table1.TableColumnsList[i].ColumnName != table2.TableColumnsList[i].ColumnName ||
                    table1.TableColumnsList[i].TypeName != table2.TableColumnsList[i].TypeName)
                {
                    // Return null if any column doesn't match
                    return null;
                }
            }

            // Create a new merged table
            Table mergedTable = new Table("MergedTable");

            // Copy columns from table1 to mergedTable
            foreach (var column in table1.TableColumnsList)
            {
                mergedTable.TableColumnsList.Add(new Column(column.ColumnName, column.ColumnType));
            }

            // Merge rows by stacking them
            mergedTable.TableRowsList.AddRange(table1.TableRowsList);
            mergedTable.TableRowsList.AddRange(table2.TableRowsList);
            database.DBTablesList.Add(mergedTable);
            return mergedTable;
        }

    }
}
