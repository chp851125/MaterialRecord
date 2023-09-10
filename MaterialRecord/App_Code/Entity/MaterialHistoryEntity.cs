using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaterialRecord
{
    public class MaterialHistoryEntity
    {
        private string _GUID;
        private string _Identifier;
        private string _DoAction;
        private string _MaterialType;
        private string _ID;
        private string _MaterialName;
        private int _UseCount;
        private int _EditCount;
        private string _Factory;
        private string _MaterialState;
        private string _RefNumber;
        private string _Remark;
        private string _CreateEmpID;
        private string _CreateName;
        private string _CreateTime;
        private string _UpdateEmpID;
        private string _UpdateName;
        private string _UpdateTime;

        public string GUID { get => _GUID; set => _GUID = value; }
        public string Identifier { get => _Identifier; set => _Identifier = value; }
        public string DoAction { get => _DoAction; set => _DoAction = value; }
        public string MaterialType { get => _MaterialType; set => _MaterialType = value; }
        public string ID { get => _ID; set => _ID = value; }
        public string MaterialName { get => _MaterialName; set => _MaterialName = value; }
        public int UseCount { get => _UseCount; set => _UseCount = value; }
        public int EditCount { get => _EditCount; set => _EditCount = value; }
        public string Factory { get => _Factory; set => _Factory = value; }
        public string MaterialState { get => _MaterialState; set => _MaterialState = value; }
        public string RefNumber { get => _RefNumber; set => _RefNumber = value; }
        public string Remark { get => _Remark; set => _Remark = value; }
        public string CreateEmpID { get => _CreateEmpID; set => _CreateEmpID = value; }
        public string CreateName { get => _CreateName; set => _CreateName = value; }
        public string CreateTime { get => _CreateTime; set => _CreateTime = value; }
        public string UpdateEmpID { get => _UpdateEmpID; set => _UpdateEmpID = value; }
        public string UpdateName { get => _UpdateName; set => _UpdateName = value; }
        public string UpdateTime { get => _UpdateTime; set => _UpdateTime = value; }
    }
}