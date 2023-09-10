using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaterialRecord
{
    public class WasLogEntity
    {
        private string _exeMethod;
        private string _exeScreen;
        private string _exeButton;
        private string _exeMsg;
        private string _memo;
        private string _updateEmp;
        private string _updateDate;
        private string _updateTime;
        private string _status;

        public string ExeMethod { get => _exeMethod; set => _exeMethod = value; }
        public string ExeScreen { get => _exeScreen; set => _exeScreen = value; }
        public string ExeButton { get => _exeButton; set => _exeButton = value; }
        public string ExeMsg { get => _exeMsg; set => _exeMsg = value; }
        public string Memo { get => _memo; set => _memo = value; }
        public string UpdateEmp { get => _updateEmp; set => _updateEmp = value; }
        public string UpdateDate { get => _updateDate; set => _updateDate = value; }
        public string UpdateTime { get => _updateTime; set => _updateTime = value; }
        public string Status { get => _status; set => _status = value; }
    }
}