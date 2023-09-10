using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaterialRecord
{
    public class MaterialTypeEntity
    {
        private string _ID;
        private string _TypeName;
        private string _Remark;
        private string _CreateEmpID;
        private string _CreateName;
        private string _CreateTime;
        private string _UpdateEmpID;
        private string _UpdateName;
        private string _UpdateTime;

        public string ID { get => _ID; set => _ID = value; }
        public string TypeName { get => _TypeName; set => _TypeName = value; }
        public string Remark { get => _Remark; set => _Remark = value; }
        public string CreateEmpID { get => _CreateEmpID; set => _CreateEmpID = value; }
        public string CreateName { get => _CreateName; set => _CreateName = value; }
        public string CreateTime { get => _CreateTime; set => _CreateTime = value; }
        public string UpdateEmpID { get => _UpdateEmpID; set => _UpdateEmpID = value; }
        public string UpdateName { get => _UpdateName; set => _UpdateName = value; }
        public string UpdateTime { get => _UpdateTime; set => _UpdateTime = value; }
    }
}