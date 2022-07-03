using System.Data.SQLite;
using System.Text;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;

public enum DataType
{
    INT,
    FLOAT,
    STRING
}
public class CSVRead
{

    private List<List<string>> data=new List<List<string>>();

    List<DataType> dataType = new List<DataType>();
    int width = 0;
    int height = 0;
    public bool LoadCSV(string _filePath)
    {
        // 読み込みたいCSVファイルのパスを指定して開く
        StreamReader sr = new StreamReader(_filePath);
        int i = 0;
        // 末尾まで繰り返す
        while (!sr.EndOfStream)
        {
            // CSVファイルの一行を読み込む
            string line = sr.ReadLine();
            // 読み込んだ一行をカンマ毎に分けて配列に格納する
            string[] values = line.Split(',');

            //横幅を保存しておく
            width = values.Length;

            data.Add(new List<string>());
            data[i].AddRange(new List<string>(values));

            //変数型を設定する
            if (i == 1)
            {
                for (int h = 0; h < values.Length; h++)
                {
                    if (values[h].Contains("int"))
                    {
                        dataType.Add(DataType.INT);
                    }
                    else if (values[h].Contains("float"))
                    {
                        dataType.Add(DataType.FLOAT);
                    }
                    else if (values[h].Contains("string"))
                    {
                        dataType.Add(DataType.STRING);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            i++;
        }
        height = i;
        return true;
    }

    public string GetData(int _row,int _colum)
    {
        return data[_row][_colum];
    } 
    public DataType GetDataType(int _id)
    {
        return dataType[_id];
    }
    public int GetWidth()
    {
        return width;
    }
    public int GetHeight()
    {
        return height;
    }

}