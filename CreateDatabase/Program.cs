using System.Data.SQLite;
using System.Text;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace DB
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //ダイアログからDB化するCSVを選択
            OpenFile of = new OpenFile();
            string csvFilePath = of.GetOpenFile();

            //CSVを読み込む
            CSVRead csvRead = new CSVRead();
            if (csvRead.LoadCSV(csvFilePath) == false)
            {

            }

            //CSV名からデータベース名を決定する
            string databaseName = "default.db";
            string[] words = csvFilePath.Split('\\');
            words = words[words.Length - 1].Split('.');
            databaseName = words[0] + ".db";
            //すでに同じ名前のデータベースが存在する場合名前を変更する
            int sameNameCount = 0;
            while (File.Exists(databaseName))
            {
                sameNameCount++;
                databaseName = words[0] + "(" + sameNameCount + ")" + ".db";
            }

            var sqlConnectionSb_UTF8 = new SQLiteConnectionStringBuilder { DataSource = databaseName };

            //DB作成
            using (var cn = new SQLiteConnection(sqlConnectionSb_UTF8.ToString()))
            {
                cn.Open();//DB接続

                using (var cmd = new SQLiteCommand(cn))
                {
                    //テーブル作成
                    //テーブル名はCSV名と同じものにする
                    string cmdText = "CREATE TABLE IF NOT EXISTS " + words[0] + 
                        "(id INTEGER NOT NULL PRIMARY KEY,";
                    for(int i=0;i< csvRead.GetWidth(); i++)
                    {
                        //テーブル
                        cmdText += csvRead.GetData(0, i);//名前を決定する

                        switch (csvRead.GetDataType(i))
                        {
                            case DataType.INT:
                                cmdText += " INTEGER NOT NULL";
                                break;
                            case DataType.FLOAT:
                                cmdText += " FLOAT NOT NULL";
                                break;
                            case DataType.STRING:
                                cmdText += " TEXT NOT NULL";
                                break;
                        }
                        if (i != csvRead.GetWidth() - 1)
                        {
                            cmdText += ",";
                        }
                    }
                    cmdText += ")";
                    cmd.CommandText = cmdText;
                    cmd.ExecuteNonQuery();//コマンドの使用

                    //cmd.CommandText = "INSERT INTO english(no, lemma, frequency, japanese) VALUES(" + $"{no}, {lemma}, '{frequency}', '{japanese}')";
                    //cmd.ExecuteNonQuery();//コマンドの使用

                    //データベースにデータを代入する
                    for (int h = 2; h < csvRead.GetHeight(); h++)
                    {
                        cmdText = "INSERT INTO " + words[0] + "(id, ";
                        for (int i = 0; i < csvRead.GetWidth(); i++)
                        {

                            cmdText += " "+csvRead.GetData(0, i);//名前指定
                            if (i != csvRead.GetWidth() - 1)
                            {
                                cmdText += ",";
                            }
                        }
                        cmdText += ") VALUES( ";

                        cmdText += $"'{h}',";
                        for (int i = 0; i < csvRead.GetWidth(); i++)
                        {
                            if(csvRead.GetDataType(i)== DataType.STRING)
                            {

                            }

                            cmdText += $" '{csvRead.GetData(h, i)}'";
                            if (i != csvRead.GetWidth() - 1)
                            {
                                cmdText += ",";
                            }
                        }
                        cmdText += ")";
                        cmd.CommandText = cmdText;
                        cmd.ExecuteNonQuery();//コマンドの使用
                    }

                    //先頭10個を描画する
                    cmd.CommandText = "SELECT * FROM "+ words[0];
                    using (var reader = cmd.ExecuteReader())
                    {
                        var dump = reader.DumpQuery();

                        Console.WriteLine(dump);
                    }
                }
            }
        }

        public string ConvertEncoding(string src, System.Text.Encoding destEnc)
        {
            byte[] src_temp = System.Text.Encoding.ASCII.GetBytes(src);
            byte[] dest_temp = System.Text.Encoding.Convert(System.Text.Encoding.ASCII, destEnc, src_temp);
            string ret = destEnc.GetString(dest_temp);
            return ret;
        }

    }

    public static class SQLiteExtension
    {

        /// <summary>
        /// 値の描画
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static string DumpQuery(this SQLiteDataReader reader)
        {
            var i = 0;
            var sb = new StringBuilder();
            while (reader.Read())
            {
                //実際の値を描画
                sb.AppendLine(string.Join("\t", Enumerable.Range(0, reader.FieldCount).Select(x => reader.GetValue(x))));
                i++;
            }

            return sb.ToString();
        }
    }


}