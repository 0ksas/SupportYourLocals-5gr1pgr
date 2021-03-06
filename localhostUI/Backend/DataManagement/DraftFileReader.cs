﻿using Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Common.Formatting;

namespace localhostUI.Backend.DataManagement
{
    class DraftFileReader : IDataReader
    {
        private string message;
        private const string fileName = "FileDrafts.json";


        public void Read(out DataList data)
        {
            try
            {
                string jsonstr = File.ReadAllText(fileName);
                data = DataList.FromList(Json.ToList(jsonstr));
            }
            catch (FileNotFoundException e)
            {
                File.Create(fileName);
                File.WriteAllText(fileName, "[]");
                data = new DataList();
            }
            catch (Exception e)
            {
                message = e.Message;
                data = null;
            }
        }

        public string GetMessage()
        {
            return message;
        }
    }
}
