using System.Windows.Forms;

using System.Data.SQLite;
using System.Text;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

class OpenFile
{
    /// <summary>
    /// ダイアログからファイル名を取得する
    /// </summary>
    /// <returns></returns>
    public string GetOpenFile()
    {
        //オープンファイルダイアログを生成する
        OpenFileDialog op = new OpenFileDialog();
        op.Title = "ファイルを開く";
        op.InitialDirectory = @"C:\";
        op.FileName = @"hoge.csv";
        op.Filter = "CSVファイル(*.csv)|*.csv;|すべてのファイル(*.*)|*.*";
        op.FilterIndex = 1;

        //オープンファイルダイアログを表示する
        DialogResult result = op.ShowDialog();

        if (result == DialogResult.OK)
        {
            return op.FileName;
        }
        else if (result == DialogResult.Cancel)
        {
            //「キャンセル」ボタンまたは「×」ボタンが選択された時の処理
        }
        return "";
    }
}