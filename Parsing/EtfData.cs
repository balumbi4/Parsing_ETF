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
        public DateTime? Date;
        public string ISIN;
        public double? Bid;
        public int? BidVolume;
        public double? Ask;
        public int? AskVolume;
        public int? Total;
        public char Status;
    }
}