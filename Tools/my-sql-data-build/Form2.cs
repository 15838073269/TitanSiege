using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MySqlDataBuild
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            dbtext.Text = Persist.I.sqliteDBPath;
            path.Text = Persist.I.sqliteGeneratePath;
            comboBox1.SelectedIndex = Persist.I.namespaceIndex1;
            nameSpaceText.Enabled = comboBox1.SelectedIndex == 2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var result =  folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK) 
            {
                path.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                dbtext.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BuildNew();
            MessageBox.Show("生成成功!");
        }

        private void BuildNew()
        {
            string dbName = Path.GetFileNameWithoutExtension(dbtext.Text);
            string dbUpp = dbName.ToUpperFirst();
            string connStr = $"Data Source='{dbtext.Text}';";
            var connect = new SQLiteConnection(connStr);
            connect.Open();
            var cmd = new SQLiteCommand();
            var dbTable = new DataTable();
            cmd.CommandText = $"SELECT name FROM sqlite_master where type='table' order by name";
            cmd.Connection = connect;
            using (var sdr = cmd.ExecuteReader())
            {
                dbTable.Load(sdr);
            }
            var tableNames = new List<StringEntiy>();
            foreach (DataRow row in dbTable.Rows)
            {
                var des = row[0].ToString();
                var des1 = des.ToUpperFirst();
                tableNames.Add(new StringEntiy(des, des1));
            }
            var rpcMaskDic = new Dictionary<string, string>();
            foreach (var tableName in tableNames)
            {
                var table = new DataTable();
                //var table1 = new DataTable();
                cmd.CommandText = $"SELECT * FROM {tableName.source} limit 0;";
                cmd.Connection = connect;
                using (var sdr = cmd.ExecuteReader())
                {
                    table.Load(sdr);
                }
                //cmd.CommandText = $"select COLUMN_NAME,column_comment from INFORMATION_SCHEMA.Columns where table_name='{tableName.source}' and table_schema='{dbName}'";
                //cmd.Connection = connect;
                //using (var sdr = cmd.ExecuteReader())
                //{
                //    table1.Load(sdr);
                //}
                var codeText = Properties.Resources.source_code;
                var treats = codeText.Split(new string[] { "[TREAT]" }, StringSplitOptions.RemoveEmptyEntries);
                codeText = treats[0] + treats[2] + treats[3];
                if (comboBox1.SelectedIndex == 1)
                {
                    codeText = codeText.Replace("{NAMESPACE_BEGIN}", $"namespace {dbUpp}\r\n{{");
                    codeText = codeText.Replace("{NAMESPACE_END}", "}");
                }
                else if (comboBox1.SelectedIndex == 2)
                {
                    codeText = codeText.Replace("{NAMESPACE_BEGIN}", $"namespace {nameSpaceText.Text}\r\n{{");
                    codeText = codeText.Replace("{NAMESPACE_END}", "}");
                }
                else
                {
                    codeText = codeText.Replace("{NAMESPACE_BEGIN}", "");
                    codeText = codeText.Replace("{NAMESPACE_END}", "");
                }
                codeText = codeText.Replace("{TYPENAME}", $"{tableName.newString}Data");
                var pkeys = table.PrimaryKey;
                if (pkeys.Length == 0)
                    throw new Exception($"{table.TableName}表必须设置Key, 每个表必须设置一个Key");
                codeText = codeText.Replace("{KETTYPE}", $"{GetCodeType(pkeys[0].DataType)}");
                codeText = codeText.Replace("{KETTYPETRUE}", $"{pkeys[0].DataType.FullName}");
                var colName1 = pkeys[0].ColumnName.ToLowerFirst();
                var colName2 = pkeys[0].ColumnName.ToUpperFirst();
                codeText = codeText.Replace("{KEYNAME1}", $"{colName1}");
                codeText = codeText.Replace("{KEYNAME2}", $"{colName2}");
                codeText = codeText.Replace("{TABLENAME}", $"{tableName.source}");
                codeText = codeText.Replace("{DBNAME}", $"{dbUpp}DB");
                codeText = codeText.Replace("{COUNT}", $"{table.Columns.Count}");
                codeText = codeText.Replace("{PARAMETER}", "SQLiteParameter");
                codeText = codeText.Replace("{CLIENT}", $"{dbUpp}Sync.Client");

                var codeTexts = codeText.Split(new string[] { "[split]" }, 0);

                //var dic = new Dictionary<string, string>();
                //foreach (DataRow item in table1.Rows)
                //    dic[item.ItemArray[0].ToString()] = item.ItemArray[1].ToString();

                //codeTexts[0] = codeTexts[0].Replace("{KEYNOTE}", $"{dic[pkeys[0].ColumnName]}");
                codeTexts[0] = codeTexts[0].Replace("{USING}", "System.Data.SQLite");

                var sb = new StringBuilder(codeTexts[0]);
                var sb1 = new StringBuilder();
                var sb2 = new StringBuilder();
                var sb3 = new StringBuilder();
                var sb4 = new StringBuilder();
                var sb5 = new StringBuilder();

                for (int i = 0; i < table.Columns.Count; i++)
                {
                    DataColumn cell = table.Columns[i];

                    var cellName = cell.ColumnName;
                    var fieldName1 = cellName.ToLowerFirst();
                    var fieldName2 = cellName.ToUpperFirst();

                    var indexCode = codeTexts[3].Replace("{INDEX}", $"{i}");
                    indexCode = indexCode.Replace("{INDEXNAME}", $"{cellName}");
                    sb1.Append(indexCode);

                    var indexCode1 = codeTexts[5].Replace("{INDEX}", $"{i}");
                    indexCode1 = indexCode1.Replace("{INDEXNAME}", $"{cellName}");
                    sb2.Append(indexCode1);

                    var indexCode2 = codeTexts[7].Replace("{INDEX}", $"{i}");
                    indexCode2 = indexCode2.Replace("{INDEXNAME}", $"{cellName}");
                    indexCode2 = indexCode2.Replace("{INDEXTYPE}", $"{GetCodeType(cell.DataType)}");
                    sb3.Append(indexCode2);

                    //var indexCode3 = codeTexts[9].Replace("{FIELDNAME1}", $"{cellName}");
                    //sb4.Append(indexCode3);

                    var indexCode4 = codeTexts[11].Replace("{INDEX}", $"{i}");
                    indexCode4 = indexCode4.Replace("{FIELDTYPE}", $"{GetCodeType(cell.DataType)}");
                    indexCode4 = indexCode4.Replace("{FIELDNAME1}", $"{cellName}");
                    sb5.Append(indexCode4);

                    if (cell == pkeys[0])
                        continue;

                    var fieldCode = codeTexts[1].Replace("{FIELDTYPE}", $"{GetCodeType(cell.DataType)}");
                    fieldCode = fieldCode.Replace("{FIELDTYPETRUE}", $"{cell.DataType.FullName}");
                    fieldCode = fieldCode.Replace("{FIELDNAME1}", $"{fieldName1}");
                    fieldCode = fieldCode.Replace("{FIELDNAME2}", $"{fieldName2}{(cell.DataType == typeof(byte[]) ? "Bytes" : "")}");
                    fieldCode = fieldCode.Replace("{KEYNAME1}", $"{colName1}");
                    fieldCode = fieldCode.Replace("{PUBLIC}", $"{(cell.DataType == typeof(byte[]) ? "internal" : "public")}");
                    fieldCode = fieldCode.Replace("{INDEX}", i.ToString());

                    fieldCode = fieldCode.Replace("{VARSET}", cell.DataType == typeof(byte[]) ? $"object bytes = {fieldName1};" : "");
                    fieldCode = fieldCode.Replace("{VARSET1}", cell.DataType == typeof(byte[]) ? $"bytes" : fieldName1);

                    var code = Type.GetTypeCode(cell.DataType);
                    if (code != TypeCode.Object & code != TypeCode.String)
                    {
                        fieldCode = fieldCode.Replace("{ADDORCUTROW}", "");
                    }
                    else if (cell.DataType == typeof(string))
                    {
                        fieldCode = fieldCode.Replace("{ADDORCUTROW}", $"if(value==null) value = string.Empty;");
                    }
                    else if (cell.DataType == typeof(byte[]))
                    {
                        fieldCode = fieldCode.Replace("{ADDORCUTROW}", $"if(value==null) value = new byte[0];");
                    }
                    else
                    {
                        fieldCode = fieldCode.Replace("{ADDORCUTROW}", "");
                    }

                    int ii = i;
                    var rpcenumName = cell.ColumnName.ToUpperAll();
                J: if (!rpcMaskDic.ContainsKey(rpcenumName))
                        rpcMaskDic.Add(rpcenumName, ""/*dic[cell.ColumnName]*/);
                    else
                    {
                        rpcenumName += (ii + 1).ToString();
                        ii++;
                        goto J;
                    }

                    fieldCode = fieldCode.Replace("{RPCHASHTYPE}", $"{dbUpp}HashProto");
                    fieldCode = fieldCode.Replace("{RPCHASHVALUE}", $"{rpcenumName}");
                    //fieldCode = fieldCode.Replace("{NOTE}", $"{dic[cell.ColumnName]}");
                    //fieldCode = fieldCode.Replace("{NOTE1}", $"{dic[cell.ColumnName]} --同步到数据库");
                    //fieldCode = fieldCode.Replace("{NOTE2}", $"{dic[cell.ColumnName]} --同步带有Key字段的值到服务器Player对象上，需要处理");
                    //fieldCode = fieldCode.Replace("{NOTE3}", $"{dic[cell.ColumnName]} --同步到数据库");
                    //fieldCode = fieldCode.Replace("{NOTE4}", $"{dic[cell.ColumnName]} --同步带有Key字段的值到服务器Player对象上，需要处理");

                    sb.Append(fieldCode);
                }

                sb.Append(codeTexts[2]);
                sb.Append(sb1);
                sb.Append(codeTexts[4]);
                sb.Append(sb2);
                sb.Append(codeTexts[6]);
                sb.Append(sb3);
                sb.Append(codeTexts[8]);
                sb.Append(codeTexts[9]);
                sb.Append(sb4);
                sb.Append(codeTexts[10]);
                sb.Append(sb5);
                sb.Append(codeTexts[12]);

                var codeText1 = sb.ToString();

                string path;
                if (string.IsNullOrEmpty(this.path.Text))
                    path = AppDomain.CurrentDomain.BaseDirectory + tableName.newString + "Data.cs";
                else
                    path = this.path.Text + "\\" + tableName.newString + "Data.cs";
                File.WriteAllText(path, sb.ToString());
            }
            BuildDBNew(dbUpp, tableNames, rpcMaskDic);
        }

        private string GetCodeType(Type type)
        {
            var code = Type.GetTypeCode(type);
            if (code != TypeCode.Object)
                return code.ToString();
            return type.FullName;
        }

        public void BuildDBNew(string db, List<StringEntiy> tableNames, Dictionary<string, string> rpcMaskDic)
        {
            var sb1 = new StringBuilder();
            sb1.AppendLine($"public enum {db}HashProto : ushort");
            sb1.AppendLine("{");
            bool f = false;
            foreach (var item in rpcMaskDic)
            {
                sb1.AppendLine($"    /// <summary>{item.Value}</summary>");
                sb1.AppendLine($"    {item.Key}{(f ? "" : $" = 4096")},");
                f = true;
            }
            sb1.AppendLine("}");
            var codeText15 = $@"NAMESPACE_BEGIN
    public class {db}Sync
    {{
        private static Net.Client.ClientBase client;
        /// <summary>
        /// 设置同步到服务器的客户端对象, 如果不设置, 则默认是ClientBase.Instance对象
        /// </summary>
        public static Net.Client.ClientBase Client
        {{
            get
            {{
                if (client == null)
                    client = Net.Client.ClientBase.Instance;
                return client;
            }}
            set => client = value;
        }}
    }}
NAMESPACE_END";
            var db1 = db + "DB";
            var codeText = Properties.Resources.db_source;
            if (comboBox1.SelectedIndex == 1)
            {
                codeText = codeText.Replace("{NAMESPACE_BEGIN}", $"namespace {db}\r\n{{");
                codeText = codeText.Replace("{NAMESPACE_END}", "}");
                codeText15 = codeText15.Replace("NAMESPACE_BEGIN", $"namespace {db}\r\n{{");
                codeText15 = codeText15.Replace("NAMESPACE_END", "}");
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                codeText = codeText.Replace("{NAMESPACE_BEGIN}", $"namespace {nameSpaceText.Text}\r\n{{");
                codeText = codeText.Replace("{NAMESPACE_END}", "}");
                codeText15 = codeText15.Replace("NAMESPACE_BEGIN", $"namespace {nameSpaceText.Text}\r\n{{");
                codeText15 = codeText15.Replace("NAMESPACE_END", "}");
            }
            else
            {
                codeText = codeText.Replace("{NAMESPACE_BEGIN}", "");
                codeText = codeText.Replace("{NAMESPACE_END}", "");
                codeText15 = codeText15.Replace("NAMESPACE_BEGIN", "");
                codeText15 = codeText15.Replace("NAMESPACE_END", "");
            }

            codeText = codeText.Replace("Database='{DBNAME1}';Data Source='{DS}';Port={PORT};User Id='{IDNAME}';Password='{IDPWD}';charset='utf8';pooling=true;useCompression=true;allowBatch=true;connectionTimeout=1200;", $"Data Source='{dbtext.Text}';");

            codeText = codeText.Replace("{DBNAME}", $"{db1}");
            codeText = codeText.Replace("{DBNAME1}", $"{dbtext.Text}");
            //codeText = codeText.Replace("{DS}", $"{iptext.Text}");
            //codeText = codeText.Replace("{PORT}", $"{porttext.Text}");
            //codeText = codeText.Replace("{IDNAME}", $"{usertext.Text}");
            //codeText = codeText.Replace("{IDPWD}", $"{pwdtext.Text}");
            codeText = codeText.Replace("{USING}", "System.Data.SQLite");
            codeText = codeText.Replace("{CONNECT}", "SQLiteConnection");
            codeText = codeText.Replace("{COMMAND}", "SQLiteCommand");
            codeText = codeText.Replace("conn.Ping();//长时间没有连接后断开连接检查状态", "");
            codeText = codeText.Replace("MySqlParameter", "SQLiteParameter");
            codeText = codeText.Replace("{TRANSACTION}", "SQLiteTransaction");

            var codeTexts = codeText.Split(new string[] { "[split]" }, 0);

            var sb = new StringBuilder(codeTexts[0]);
            var sb2 = new StringBuilder();

            foreach (var tableName in tableNames)
            {
                var indexCode = codeTexts[1].Replace("{TABLENAME}", $"{tableName.source}");
                indexCode = indexCode.Replace("{TABLENAME1}", $"{tableName.newString}");
                sb2.Append(indexCode);
            }

            sb.Append(sb2);
            sb.Append(codeTexts[2]);

            string path, path1, path2;
            if (string.IsNullOrEmpty(this.path.Text))
            {
                path = AppDomain.CurrentDomain.BaseDirectory + db1 + ".cs";
                path1 = AppDomain.CurrentDomain.BaseDirectory + $"{db}HashProto.cs";
                path2 = AppDomain.CurrentDomain.BaseDirectory + $"{db}Sync.cs";
            }
            else
            {
                path = this.path.Text + "\\" + db1 + ".cs";
                path1 = this.path.Text + "\\" + $"{db}HashProto.cs";
                path2 = this.path.Text + "/" + $"{db}Sync.cs";
            }
            File.WriteAllText(path, sb.ToString());
            File.WriteAllText(path1, sb1.ToString());
            File.WriteAllText(path2, codeText15);
        }

        private void dbtext_TextChanged(object sender, EventArgs e)
        {
            Persist.I.sqliteDBPath = ((TextBox)sender).Text;
            Persist.SaveData();
        }

        private void path_TextChanged(object sender, EventArgs e)
        {
            Persist.I.sqliteGeneratePath = ((TextBox)sender).Text;
            Persist.SaveData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            nameSpaceText.Enabled = comboBox1.SelectedIndex == 2;
            Persist.I.namespaceIndex1 = comboBox1.SelectedIndex;
            Persist.SaveData();
        }
    }
}
