﻿using System;
using System.Collections.Generic;
using MagicMongoDBTool.Module;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MagicMongoDBTool
{
    public partial class frmMapReduce : System.Windows.Forms.Form
    {
        public frmMapReduce()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 数据集
        /// </summary>
        private MongoCollection _mongocol = SystemManager.GetCurrentCollection();
        private void frmMapReduce_Load(object sender, EventArgs e)
        {
            cmbForMap.SelectedIndexChanged += new EventHandler(
                (x, y) => { txtMapJs.Text = MongoDBHelper.LoadJavascript(cmbForMap.Text); }
            );
            cmbForReduce.SelectedIndexChanged += new EventHandler(
                (x, y) => { txtReduceJs.Text = MongoDBHelper.LoadJavascript(cmbForReduce.Text); }
            );

            foreach (String item in SystemManager.GetJsNameList())
            {
                cmbForMap.Items.Add(item);
                cmbForReduce.Items.Add(item);
            }

            if (!SystemManager.IsUseDefaultLanguage)
            {
                lblMapFunction.Text = SystemManager.mStringResource.GetText(MagicMongoDBTool.Module.StringResource.TextType.MapReduce_MapFunction);
                lblReduceFunction.Text = SystemManager.mStringResource.GetText(MagicMongoDBTool.Module.StringResource.TextType.MapReduce_ReduceFunction);
                lblResult.Text = SystemManager.mStringResource.GetText(MagicMongoDBTool.Module.StringResource.TextType.MapReduce_Result);
                cmdRun.Text = SystemManager.mStringResource.GetText(MagicMongoDBTool.Module.StringResource.TextType.MapReduce_Run);
                cmdSaveMapJs.Text = SystemManager.mStringResource.GetText(MagicMongoDBTool.Module.StringResource.TextType.Common_Save);
                cmdSaveReduceJs.Text = SystemManager.mStringResource.GetText(MagicMongoDBTool.Module.StringResource.TextType.Common_Save);
            }

        }
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdRun_Click(object sender, EventArgs e)
        {
            BsonJavaScript map = new BsonJavaScript(txtMapJs.Text);
            BsonJavaScript reduce = new BsonJavaScript(txtReduceJs.Text);
            //TODO:这里可能会超时，失去响应
            //需要设置SocketTimeOut
            MapReduceResult rtn = _mongocol.MapReduce(map, reduce);

            List<BsonDocument> result = new List<BsonDocument>();
            result.Add(rtn.Response);
            MongoDBHelper.FillDataToTreeView("MapReduce Result", trvResult, result,0);
            trvResult.DatatreeView.ExpandAll();
        }
        /// <summary>
        /// 保存MapJs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdSaveMapJs_Click(object sender, EventArgs e)
        {
            if (txtMapJs.Text != String.Empty)
            {
                String strJsName = MyMessageBox.ShowInput("pls Input Javascript Name：", "Save Javascript");
                MongoDBHelper.CreateNewJavascript(strJsName, txtMapJs.Text);
            }
        }
        /// <summary>
        /// 保存ReduceJs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdSaveReduceJs_Click(object sender, EventArgs e)
        {
            if (this.txtReduceJs.Text != String.Empty)
            {
                String strJsName = MyMessageBox.ShowInput("pls Input Javascript Name", "Save Javascript");
                MongoDBHelper.CreateNewJavascript(strJsName, txtReduceJs.Text);
            }
        }
    }
}
