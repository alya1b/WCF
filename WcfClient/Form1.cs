using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WcfClient.ServiceReference1;
using WcfService;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WcfClient
{
    public partial class Form1 : Form
    {
        Service1Client manager = new Service1Client();
        
        string cellOldValue = "";
        string cellNewValue = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string DBName = ShowInputDialog("", "Введіть назву бази даних:");
            manager.CreateDB(DBName);
            tabControl.TabPages.Clear();
            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();

        }
        private string ShowInputDialog(string caption, string text)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 300 };
            Button confirmation = new Button() { Text = "Зберегти", Left = 250, Width = 100, Top = 100, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : null;
        }
        private (string textBoxValue1, string textBoxValue2) ShowInputDialog2(string caption, string text)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox1 = new TextBox() { Left = 50, Top = 50, Width = 300 };
            TextBox textBox2 = new TextBox() { Left = 50, Top = 83, Width = 300 };
            Button confirmation = new Button() { Text = "Зберегти", Left = 250, Width = 100, Top = 116, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox1);
            prompt.Controls.Add(textBox2);
            prompt.Controls.Add(confirmation);

            if (prompt.ShowDialog() == DialogResult.OK)
            {
                string textBoxValue1 = textBox1.Text;
                string textBoxValue2 = textBox2.Text;
                return (textBoxValue1, textBoxValue2);
            }
            else
            {
                return (null, null);
            }
        }

        private (string comboBoxValue, string textBoxValue) ShowColumnDialog(string caption, string text)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 250,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 300 };

            ComboBox comboBox = new ComboBox() { Left = 50, Top = 80, Width = 300 };
            comboBox.Items.AddRange(new string[] { "String", "Integer", "Char", "Real", "CharInvl", "StrinhInvl" });
            comboBox.SelectedIndex = 0;

            Button confirmation = new Button() { Text = "Зберегти", Left = 250, Width = 100, Top = 140, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(comboBox);
            prompt.Controls.Add(confirmation);

            if (prompt.ShowDialog() == DialogResult.OK)
            {
                string comboBoxValue = comboBox.SelectedItem.ToString();
                string textBoxValue = textBox.Text;
                return (comboBoxValue, textBoxValue);
            }
            else
            {
                return (null, null);
            }
        }



        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream;

            sfdSaveDB.Filter = "tdb files (*.tdb)|*.tdb";
            sfdSaveDB.FilterIndex = 1;
            sfdSaveDB.RestoreDirectory = true;

            if (sfdSaveDB.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = sfdSaveDB.OpenFile()) != null)
                {
                    myStream.Close();

                    manager.SaveDB(sfdSaveDB.FileName);
                }
            }
        }
        void VisualTable(Table t)
        {
            try
            {
                dataGridView.Rows.Clear();
                dataGridView.Columns.Clear();

                if (t == null)
                {
                    return;
                }

                foreach (Column c in t.TableColumnsList)
                {

                    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                    column.Name = c.ColumnName;
                    column.HeaderText = c.ColumnName;
                    dataGridView.Columns.Add(column);
                }

                foreach (Row r in t.TableRowsList)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    foreach (string s in r.RowValuesList)
                    {
                        DataGridViewCell cell = new DataGridViewTextBoxCell();
                        cell.Value = s;
                        row.Cells.Add(cell);
                    }

                    dataGridView.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
               // MessageBox.Show("Виникла помилка: " + ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofdOpenDB.Filter = "tdb files (*.tdb)|*.tdb";
            ofdOpenDB.FilterIndex = 1;
            ofdOpenDB.RestoreDirectory = true;

            if (ofdChooseFilePath.ShowDialog() == DialogResult.OK)
            {
                manager.OpenDB(ofdChooseFilePath.FileName);

            }

            tabControl.TabPages.Clear();
            List<string> buf = manager.GetTableNameList().ToList();
            foreach (string s in buf)
                tabControl.TabPages.Add(s);

            int ind = tabControl.SelectedIndex;
            if (ind != -1) VisualTable(manager.GetTable(ind));
        }


        private void addTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string userInput = ShowInputDialog("", "Введіть назву таблиці:");

            if (manager.AddTable(userInput))
            {
                tabControl.TabPages.Add(userInput);
            }
        }

        private void deleteTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl.TabCount == 0) return;
            try
            {
                manager.DeleteTable(tabControl.SelectedIndex);
                tabControl.TabPages.RemoveAt(tabControl.SelectedIndex);
            }
            catch { }
            if (tabControl.TabCount == 0) return;

            int ind = tabControl.SelectedIndex;
            if (ind != -1) VisualTable(manager.GetTable(ind));

        }

        private void addColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (string comboBoxValue, string textBoxValue) result = ShowColumnDialog("", "Введіть назву колонки:");
            string selectedValue = result.comboBoxValue;
            string enteredText = result.textBoxValue;
            if (manager.AddColumn(tabControl.SelectedIndex, enteredText, selectedValue))
            {

                int ind = tabControl.SelectedIndex;
                if (ind != -1) VisualTable(manager.GetTable(ind));
            }
        }

        private void deleteColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int currentTabIndex = tabControl.SelectedIndex;

            if (currentTabIndex >= 0 && dataGridView.Columns.Count > 0)
            {
                int currentColumnIndex = dataGridView.CurrentCell != null ? dataGridView.CurrentCell.ColumnIndex : -1;

                if (currentColumnIndex >= 0 && currentColumnIndex < dataGridView.Columns.Count)
                {
                    try
                    {
                        manager.DeleteColumn(currentTabIndex, currentColumnIndex);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Помилка при видаленні колонки: " + ex.Message);
                    }

                    VisualTable(manager.GetTable(currentTabIndex));
                }
                else
                {
                    MessageBox.Show("Виберіть коректну колонку для видалення.");
                }
            }
            else
            {
                MessageBox.Show("Немає можливих для видалення колонок.");
            }
        }

        private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (manager.AddRow(tabControl.SelectedIndex))
            {

                int ind = tabControl.SelectedIndex;
                if (ind != -1) VisualTable(manager.GetTable(ind));
            }
        }

        private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView.Rows.Count == 0) return;
            try
            {
                manager.DeleteRow(tabControl.SelectedIndex, dataGridView.CurrentCell.RowIndex);
            }
            catch { }

            int ind = tabControl.SelectedIndex;
            if (ind != -1) VisualTable(manager.GetTable(ind));
        }

        private void dataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.RowIndex < dataGridView.Rows.Count && e.ColumnIndex < dataGridView.Columns.Count)
            {
                object cellValue = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (cellValue != null)
                {
                    cellOldValue = cellValue.ToString();
                }
                else
                {
                    cellOldValue = string.Empty;
                }
            }
            else
            {
                cellOldValue = string.Empty;
            }
        }

        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            
            cellNewValue = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            if (manager.ChangeValue(cellNewValue, tabControl.SelectedIndex, e.ColumnIndex, e.RowIndex))
            {
                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cellNewValue;
            }
            else {
                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cellOldValue;
               // MessageBox.Show("Error: incorrect type");
            }


            int ind = tabControl.SelectedIndex;
            if (ind != -1) VisualTable(manager.GetTable(ind));
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ind = tabControl.SelectedIndex;
            if (ind != -1) VisualTable(manager.GetTable(ind));
        }

        private void renameColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int currentTabIndex = tabControl.SelectedIndex;

            if (currentTabIndex >= 0 && dataGridView.Columns.Count > 0)
            {
                int currentColumnIndex = dataGridView.CurrentCell != null ? dataGridView.CurrentCell.ColumnIndex : -1;

                if (currentColumnIndex >= 0 && currentColumnIndex < dataGridView.Columns.Count)
                {
                    string selectedColumnName = dataGridView.Columns[currentColumnIndex].Name;

                    string newColumnName = ShowInputDialog("Перейменувати колонку", "Введіть нову назву:");

                    if (!string.IsNullOrEmpty(newColumnName))
                    {
                        bool success = manager.RenameColumn(currentTabIndex, currentColumnIndex, newColumnName);

                        if (success)
                        {
                            VisualTable(manager.GetTable(currentTabIndex));
                        }
                        else
                        {
                            MessageBox.Show("Переконайтесь що назва колонки унікальна.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Введіть правильну назву колонки.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Введіть правильну назву колонки.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Немає колонок для перейменування.", "Детальніше", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void mergeTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (string textBoxValue1, string textBoxValue2) result = ShowInputDialog2("", "Введіть назви таблиць:");
            string TableValue1 = result.textBoxValue1;
            string TableValue2 = result.textBoxValue2;
            Table margedTable = manager.Merge_Tables(TableValue1, TableValue2);
            if (margedTable != null)
            {
                TabPage tp = new TabPage();
                tp.Text = margedTable.TableName;
                tabControl.TabPages.Add(tp);
                int ind = tabControl.SelectedIndex;
                if (ind != -1) VisualTable(manager.GetTable(ind));

            }
        }

        //------------------------------------------

         //DBManager manager = new DBManager(); 
    }
}