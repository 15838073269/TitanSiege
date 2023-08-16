using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace MySqlDataBuild
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            dbcb.Text = Persist.I.sqliteDBPath;
            pathCb.Text = Persist.I.sqliteGeneratePath;
            comboBox1.SelectedIndex = Persist.I.namespaceIndex1;
            nameSpaceText.Enabled = comboBox1.SelectedIndex == 2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var result =  folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK) 
            {
                pathCb.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                dbcb.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BuildNew();
            MessageBox.Show("生成成功!");
        }



        private void BuildNew()
        {
            var assebly = typeof(SQLiteConnection).Assembly;
            var type = assebly.GetType("System.Data.SQLite.SQLiteConnection");
            var type1 = assebly.GetType("System.Data.SQLite.SQLiteCommand");
            string dbName = Path.GetFileNameWithoutExtension(dbcb.Text);
            string dbUpp = dbName.ToUpperFirst();
            string connStr = $"Data Source='{dbcb.Text}';";
            var connect = (DbConnection)Activator.CreateInstance(type, new object[] { connStr }); //new MySqlConnection(connStr);
            connect.Open();
            var cmd = (DbCommand)Activator.CreateInstance(type1); //new MySqlCommand();
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
            var rpcMaskDic = new Dictionary<string, Dictionary<string, string[]>>();
            foreach (var tableName in tableNames)
            {
                var table = new DataTable();
                var table1 = new DataTable();
                cmd.CommandText = $"SELECT * FROM {tableName.source} limit 0;";
                cmd.Connection = connect;
                using (var sdr = cmd.ExecuteReader())
                {
                    table.Load(sdr);
                }
                //cmd.CommandText = $"select COLUMN_NAME,column_comment,character_maximum_length from INFORMATION_SCHEMA.Columns where table_name='{tableName.source}' and table_schema='{dbName}'";
                //cmd.Connection = connect;
                //using (var sdr = cmd.ExecuteReader())
                //{
                //    table1.Load(sdr);
                //}
                var codeText = Properties.Resources.tableTemplate;
                var treats = codeText.Split(new string[] { "[TREAT]" }, StringSplitOptions.RemoveEmptyEntries);
                codeText = treats[0] + treats[1] + treats[3];
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
                codeText = codeText.Replace("{CLIENT}", $"{dbUpp}DBEvent");

                var codeTexts = codeText.Split(new string[] { "[split]" }, 0);

                var dic = new Dictionary<string, string[]>();
                foreach (DataColumn item in table.Columns)
                {
                    var length = "65536";
                    if (string.IsNullOrEmpty(length)) length = "0";
                    dic[item.ColumnName] = new string[] { "", length };
                }
                //foreach (DataRow item in table1.Rows)
                //{
                //    var length = item.ItemArray[2].ToString();
                //    if (string.IsNullOrEmpty(length)) length = "0";
                //    dic[item.ItemArray[0].ToString()] = new string[] { item.ItemArray[1].ToString(), length };
                //}

                codeTexts[0] = codeTexts[0].Replace("{KEYNOTE}", $"{dic[pkeys[0].ColumnName][0]}");
                codeTexts[0] = codeTexts[0].Replace("{USING}", "System.Data.SQLite");
                codeTexts[0] = codeTexts[0].Replace("{RPCHASHTYPE}", $"{dbUpp}HashProto");

                var sb = new StringBuilder(codeTexts[0]);
                var sb1 = new StringBuilder();
                var sb2 = new StringBuilder();
                var sb3 = new StringBuilder();
                var sb4 = new StringBuilder();
                var sb5 = new StringBuilder();

                for (int i = 0; i < table.Columns.Count; i++)
                {
                    var cell = table.Columns[i];
                    var isKey = cell == pkeys[0];

                    var cellName = cell.ColumnName;
                    var fieldName1 = cellName.ToLowerFirst();
                    var fieldName2 = cellName.ToUpperFirst();

                    var indexCode = codeTexts[3].Replace("{INDEX}", $"{i}");
                    indexCode = indexCode.Replace("{INDEXNAME}", $"{fieldName1}");
                    indexCode = indexCode.Replace("{TEXTLENGTH}", $"{dic[cell.ColumnName][1]}");
                    sb1.Append(indexCode);

                    var indexCode1 = codeTexts[5].Replace("{INDEX}", $"{i}");
                    indexCode1 = indexCode1.Replace("{INDEXNAME}", $"{(isKey ? fieldName1 : $"{fieldName1}.Value")}");
                    sb2.Append(indexCode1);

                    bool isArray = cell.DataType == typeof(byte[]);
                    var isAtk = !isArray & cell.DataType != typeof(bool) & cell.DataType != typeof(string) & cell.DataType != typeof(DateTime);
                    var indexCode2 = codeTexts[7].Replace("{INDEX}", $"{i}");
                    if (isKey)
                        indexCode2 = indexCode2.Replace("{ADDORCUTROW}", $"this.{fieldName1} = ({GetCodeType(cell.DataType)})value;");
                    else
                        indexCode2 = indexCode2.Replace("{ADDORCUTROW}", $"Check{fieldName2}{(isArray ? "Bytes" : "")}Value(({GetCodeType(cell.DataType)})value, -1);");
                    sb3.Append(indexCode2);

                    var indexCode4 = codeTexts[11].Replace("{INDEX}", $"{i}");
                    indexCode4 = indexCode4.Replace("{FIELDTYPE}", $"{GetCodeType(cell.DataType)}");
                    indexCode4 = indexCode4.Replace("{FIELDTYPE1}", $"{GetCodeType1(cell.DataType)}Obs");
                    indexCode4 = indexCode4.Replace("{FIELDNAME1}", $"{fieldName1}");
                    indexCode4 = indexCode4.Replace("{INIT}", isKey ? $"this.{fieldName1} = {fieldName1};" : $"Check{fieldName2}{(isArray ? "Bytes" : "")}Value({(isArray ? $"Convert.FromBase64String(Encoding.ASCII.GetString({fieldName1}))" : $"{fieldName1}")}, -1);");
                    sb5.Append(indexCode4);

                    if (isKey)
                        continue;

                    var rpcenumName = (table.TableName + "_" + cell.ColumnName).ToUpperAll();
                    if (!rpcMaskDic.TryGetValue(table.TableName, out var dict))
                        rpcMaskDic.Add(table.TableName, dict = new Dictionary<string, string[]>());
                    dict.Add(rpcenumName, dic[cell.ColumnName]);

                    var fieldCode = codeTexts[1].Replace("{FIELDTYPE}", $"{GetCodeType(cell.DataType)}");
                    fieldCode = fieldCode.Replace("{FIELDTYPE1}", $"{GetCodeType1(cell.DataType)}Obs");
                    fieldCode = fieldCode.Replace("{FIELDTYPETRUE}", $"{cell.DataType.FullName}");
                    fieldCode = fieldCode.Replace("{FIELDNAME1}", $"{fieldName1}");
                    fieldCode = fieldCode.Replace("{FIELDNAME2}", $"{fieldName2}{(isArray ? "Bytes" : "")}");
                    fieldCode = fieldCode.Replace("{KEYNAME1}", $"{colName1}");
                    fieldCode = fieldCode.Replace("{PUBLIC}", $"{(isArray ? "internal" : "public")}");
                    fieldCode = fieldCode.Replace("{INDEX}", i.ToString());

                    fieldCode = fieldCode.Replace("{VARSET}", isArray ? $"object bytes = {fieldName1}.Value;" : "");
                    fieldCode = fieldCode.Replace("{VARSET1}", (isKey ? fieldName1 : isArray ? $"bytes" : $"{fieldName1}.Value"));

                    fieldCode = fieldCode.Replace("{ISATK}", isAtk.ToString().ToLower());

                    fieldCode = fieldCode.Replace("{RPCHASHTYPE}", $"{dbUpp}HashProto");
                    fieldCode = fieldCode.Replace("{RPCHASHVALUE}", $"{rpcenumName}");
                    fieldCode = fieldCode.Replace("{NOTE}", $"{dic[cell.ColumnName][0]}");
                    fieldCode = fieldCode.Replace("{NOTE1}", $"{dic[cell.ColumnName][0]} --同步到数据库");
                    fieldCode = fieldCode.Replace("{NOTE2}", $"{dic[cell.ColumnName][0]} --同步带有Key字段的值到服务器Player对象上，需要处理");
                    fieldCode = fieldCode.Replace("{NOTE3}", $"{dic[cell.ColumnName][0]} --同步当前值到服务器Player对象上，需要处理");
                    fieldCode = fieldCode.Replace("{NOTE4}", $"{dic[cell.ColumnName][0]} --获得属性观察对象");

                    fieldCode = fieldCode.Replace("{JUDGE}", isArray ? "" : $"if (this.{fieldName1}.Value == value)\r\n                return;");

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
                sb.Append(codeTexts[14]);
                sb.Append(codeTexts[15]);

                var codeText1 = sb.ToString();

                string path;
                if (string.IsNullOrEmpty(pathCb.Text))
                    path = AppDomain.CurrentDomain.BaseDirectory;
                else
                    path = pathCb.Text + "\\";
                if (checkBox3.Checked)
                    path += dbUpp + "DB";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                path += "\\" + (checkBox2.Checked ? dbUpp + "_" : "") + tableName.newString + "Data.cs";
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

        private string GetCodeType1(Type type)
        {
            var code = Type.GetTypeCode(type);
            if (code != TypeCode.Object)
                return code.ToString();
            if (type == typeof(byte[]))
                return "Bytes";
            return type.FullName;
        }

        public void BuildDBNew(string db, List<StringEntiy> tableNames, Dictionary<string, Dictionary<string, string[]>> rpcMaskDic)
        {
            var sb1 = new StringBuilder();
            sb1.AppendLine($"public enum {db}HashProto : ushort");
            sb1.AppendLine("{");
            int index = 4096;
            foreach (var item in rpcMaskDic)
            {
                bool f = false;
                foreach (var item1 in item.Value)
                {
                    sb1.AppendLine($"    /// <summary>{item1.Value[0]}</summary>");
                    sb1.AppendLine($"    {item1.Key}{(f ? "" : $" = {index}")},");
                    f = true;
                }
                index += 1000;
            }
            sb1.AppendLine("}");

            var codeText15 = $@"NAMESPACE_BEGIN
    public class {db}DBEvent
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

        /// <summary>
        /// 当服务器属性同步给客户端, 如果需要同步属性到客户端, 需要监听此事件, 并且调用发送给客户端
        /// 参数1: 要发送给哪个客户端
        /// 参数2: cmd
        /// 参数3: methodHash
        /// 参数4: pars
        /// </summary>
        public static System.Action<Net.Server.NetPlayer, byte, ushort, object[]> OnSyncProperty;

        SYNC_KEY
    }}
NAMESPACE_END";
            var db1 = db + "DB";
            var codeText = Properties.Resources.databaseTemplate;
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
            codeText = codeText.Replace("{DBNAME}", $"{db1}");
            codeText = codeText.Replace("{DBNAME1}", $"{dbcb.Text}");
            //codeText = codeText.Replace("{DS}", $"{iptext.Text}");
            //codeText = codeText.Replace("{PORT}", $"{porttext.Text}");
            //codeText = codeText.Replace("{IDNAME}", $"{usertext.Text}");
            //codeText = codeText.Replace("{IDPWD}", $"{pwdtext.Text}");
            codeText = codeText.Replace("{USING}", "System.Data.SQLite");
            codeText = codeText.Replace("{CONNECT}", "SQLiteConnection");
            codeText = codeText.Replace("{COMMAND}", "SQLiteCommand");
            codeText = codeText.Replace("{TRANSACTION}", "SQLiteTransaction");
            codeText = codeText.Replace("{PING}", "");

            var codeTexts = codeText.Split(new string[] { "[split]" }, 0);

            var sb = new StringBuilder(codeTexts[0]);
            var sb2 = new StringBuilder();
            var sb3 = new StringBuilder();

            foreach (var tableName in tableNames)
            {
                var indexCode = codeTexts[1].Replace("{TABLENAME}", $"{tableName.source}");
                indexCode = indexCode.Replace("{TABLENAME1}", $"{tableName.newString}");
                sb2.Append(indexCode);

                sb3.Append($"/// <summary>{tableName.newString}Data类对象属性同步id索引</summary>\r\n\t\tpublic static int {tableName.newString}Data_SyncID = 0;\r\n\t\t");
            }

            sb.Append(sb2);
            sb.Append(codeTexts[2]);
            sb.Append(codeTexts[4]);
            sb.Append(codeTexts[5]);

            codeText15 = codeText15.Replace("SYNC_KEY", sb3.ToString());

            string path, path1, path2;
            if (string.IsNullOrEmpty(pathCb.Text))
            {
                path = AppDomain.CurrentDomain.BaseDirectory + db1 + ".cs";
                path1 = AppDomain.CurrentDomain.BaseDirectory + $"{db}HashProto.cs";
                path2 = AppDomain.CurrentDomain.BaseDirectory + $"{db}DBEvent.cs";
            }
            else
            {
                path = pathCb.Text + "/" + db1 + ".cs";
                path1 = pathCb.Text + "/" + $"{db}HashProto.cs";
                path2 = pathCb.Text + "/" + $"{db}DBEvent.cs";
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
