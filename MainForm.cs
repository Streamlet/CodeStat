using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Streamlet.CodeStat
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private StatModel StatFolder(string filename, ref DataTable dt, int curPathLen)
        {
            DirectoryInfo di = new DirectoryInfo(filename);
            StatModel sm = new StatModel("");

            if (di.Exists)
            {
                if (di.FullName.Length == curPathLen)
                {
                    dt.Rows.Add("\\", 0, 0, 0, 0);
                }
                else
                {
                    dt.Rows.Add(di.FullName.Substring(curPathLen), 0, 0, 0, 0);
                }
                int index = dt.Rows.Count - 1;

                try
                {
                    DirectoryInfo[] dis = di.GetDirectories();

                    foreach (DirectoryInfo subdi in dis)
                    {
                        StatModel m = StatFolder(subdi.FullName, ref dt, curPathLen);
                        sm.BlankLines += m.BlankLines;
                        sm.CodeLines += m.CodeLines;
                        sm.CommentLines += m.CommentLines;
                    }
                }
                catch { }

                try
                {
                    FileInfo[] fis = di.GetFiles();

                    foreach (FileInfo fi in fis)
                    {
                        StatModel m = StatFile(fi.FullName, ref dt, curPathLen);
                        if (m.IsValid)
                        {
                            sm.BlankLines += m.BlankLines;
                            sm.CodeLines += m.CodeLines;
                            sm.CommentLines += m.CommentLines;
                        }
                    }
                }
                catch { }

                if (sm.TotalLines > 0)
                {

                    dt.Rows[index][1] = sm.BlankLines;
                    dt.Rows[index][2] = sm.CodeLines;
                    dt.Rows[index][3] = sm.CommentLines;
                    dt.Rows[index][4] = sm.TotalLines;
                }
                else
                {
                    dt.Rows.RemoveAt(index);
                }
            }

            return sm;
        }

        private StatModel StatFile(string filename, ref DataTable dt, int curPathLen)
        {
            FileInfo fi = new FileInfo(filename);
            StatModel sm = Stat.StatFile(fi.FullName);

            if (sm.IsValid)
            {
                dt.Rows.Add(fi.FullName.Substring(curPathLen),
                    sm.BlankLines, sm.CodeLines, sm.CommentLines, sm.TotalLines);

            }

            return sm;
        }

        private void browseFolderButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                infoTextBox.Text = "正在统计, 请稍候...";
                dataGridView.DataSource = null;
                exportButton.Enabled = false;
                this.Refresh();

                DataTable dt = new DataTable();
                dt.Columns.Add("文件名");
                dt.Columns.Add("空白行");
                dt.Columns.Add("代码行");
                dt.Columns.Add("注释行");
                dt.Columns.Add("总行数");
                StatFolder(folderBrowserDialog.SelectedPath, ref dt, folderBrowserDialog.SelectedPath.Length);
                dataGridView.DataSource = dt;
                infoTextBox.Text = "文件夹: " + folderBrowserDialog.SelectedPath;
                if (dataGridView.RowCount > 0)
                {
                    exportButton.Enabled = true;
                }
            }
        }

        private void openFileButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                infoTextBox.Text = "正在统计, 请稍候...";
                dataGridView.DataSource = null;
                exportButton.Enabled = false;
                this.Refresh();

                DataTable dt = new DataTable();
                dt.Columns.Add("文件名");
                dt.Columns.Add("空白行");
                dt.Columns.Add("代码行");
                dt.Columns.Add("注释行");
                dt.Columns.Add("总行数");
                foreach (string filename in openFileDialog.FileNames)
                {
                    StatFile(filename, ref dt, filename.LastIndexOf("\\"));
                }
                dataGridView.DataSource = dt;
                if (openFileDialog.FileNames.Length > 1)
                {
                    infoTextBox.Text = "文件夹: " + openFileDialog.FileName.Substring(0, openFileDialog.FileName.LastIndexOf("\\"));
                }
                else
                {
                    infoTextBox.Text = "文件: " + openFileDialog.FileName;
                }
                if (dataGridView.RowCount > 0)
                {
                    exportButton.Enabled = true;
                }
            }

        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                copyToolStripMenuItem.Enabled = true;
            }
            else
            {
                copyToolStripMenuItem.Enabled = false;
            }
        }

        private void dataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip.Show(dataGridView, e.Location);
            }
        }

        private void dataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (e.Button == MouseButtons.Right && !dataGridView.Rows[e.RowIndex].Selected)
            {
                foreach (DataGridViewRow row in dataGridView.SelectedRows)
                {
                    row.Selected = false;
                }

                dataGridView.Rows[e.RowIndex].Selected = true;
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                sb.Append((string)row.Cells[0].Value + "\t" 
                    + (string)row.Cells[1].Value + "\t"
                    + (string)row.Cells[2].Value + "\t"
                    + (string)row.Cells[3].Value + "\t" 
                    + (string)row.Cells[4].Value + "\r\n");
            }

            Clipboard.SetText(sb.ToString());
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                switch (saveFileDialog.FilterIndex)
                { 
                    case 1:
                        SaveToCSVFile(saveFileDialog.FileName);
                        break;
                    case 2:
                    case 3:
                        SaveToTextFile(saveFileDialog.FileName);
                        break;
                }
            }
        }

        private void SaveToExcelXML(string filename)
        {
            //            
        }

        private void SaveToExcelBinary(string filename)
        {
            //
        }

        private string CSVReplace(string str)
        {
            if (str.IndexOf("\"") > 0 || str.IndexOf(" ") > 0 || str.IndexOf(",") > 0 || str.IndexOf("\n") > 0)
            {
                str.Replace("\"", "\"\"");
                return "\"" + str + "\"";
            }
            return str;
        }

        private void SaveToCSVFile(string filename)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filename, false, Encoding.Default))
                {
                    sw.WriteLine(CSVReplace(infoTextBox.Text));
                    sw.WriteLine("文件名,空白行,代码行,注释行,总行数");
                    foreach (DataGridViewRow row in dataGridView.Rows)
                    {
                        sw.WriteLine(CSVReplace((string)row.Cells[0].Value) + ","
                            + (string)row.Cells[1].Value + ","
                            + (string)row.Cells[2].Value + ","
                            + (string)row.Cells[3].Value + ","
                            + (string)row.Cells[4].Value);
                    }
                    sw.Close();
                }
            }
            catch
            {
                MessageBox.Show("保存文件失败。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void SaveToTextFile(string filename)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filename, false, Encoding.Default))
                {
                    sw.WriteLine(infoTextBox.Text);
                    sw.WriteLine("文件名\t空白行\t代码行\t注释行\t总行数");
                    foreach (DataGridViewRow row in dataGridView.Rows)
                    {
                        sw.WriteLine((string)row.Cells[0].Value + "\t"
                            + (string)row.Cells[1].Value + "\t"
                            + (string)row.Cells[2].Value + "\t"
                            + (string)row.Cells[3].Value + "\t"
                            + (string)row.Cells[4].Value);
                    }
                    sw.Close();
                }
            }
            catch
            {
                MessageBox.Show("保存文件失败。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
		}

		private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.streamlet.com.cn/", null);
		}
    }
}
