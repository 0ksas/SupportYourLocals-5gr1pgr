﻿using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Common.Formatting;

namespace localhostUI.Backend.DataManagement
{
    class DraftFileWriter : IDataWriter
    {
        private string message;
        private readonly string fileName;
        public void Write(DataList data)
        {
            try
            {
                string jsonstr = Json.FromList(DataList.ToList(data));
                File.WriteAllText(fileName, jsonstr);
            }
            catch (Exception e)
            {
                message = e.Message;
            }
        }

        public string GetMessage()
        {
            return message;
        }
        public DraftFileWriter(string fileName)
        {
            this.fileName = fileName;
        }
    }
}
