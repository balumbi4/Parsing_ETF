using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Parsing
{
    internal class EtfData
    {
        public string Name;
        public double? Last;
        public double? ChangePercent;
        public double? ChangeAbs;
        public DateTime Date;
        public string ISIN;
        public double? BidFirst;
        public int? BidLast;
        public double? AskFirst;
        public int? AskLast;
        public int? Total;
        public char Status;    
    }
}