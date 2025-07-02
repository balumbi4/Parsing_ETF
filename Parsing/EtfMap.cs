using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsing
{
    internal class EtfMap : ClassMap<EtfData>
    {
        public EtfMap()
        {
            Map(m => m.Name)
                .Name("Name");
            Map(m => m.Last)
                .Name("Last");
            Map(m => m.ChangeRercent)
                .Name("ChangeRercent");
            Map(m => m.ChangeAbs)
                .Name("ChangeAbs");
            Map(m => m.Date)
                .Name("Date");
            Map(m => m.ISIN)
                .Name("ISIN");
            Map(m => m.BidFirst)
                .Name("BidFirst")
                .Convert(args => args.Value.BidFirst?.ToString() ?? "-");  // Если LastPrice == null → "-"
            Map(m => m.BidLast)
                .Name("BidLast")
                .Convert(args => args.Value.BidLast?.ToString() ?? "-");  // Если LastPrice == null → "-"
            Map(m => m.AskFirst)
                .Name("AskFirst")
                .Convert(args => args.Value.AskFirst?.ToString() ?? "-");  // Если LastPrice == null → "-"
            Map(m => m.AskLast)
                .Name("AskLast")
                .Convert(args => args.Value.AskLast?.ToString() ?? "-");  // Если LastPrice == null → "-"
            Map(m => m.Total)
                .Name("Total")
                .Convert(args => args.Value.Total?.ToString() ?? "-");  // Если LastPrice == null → "-"
            Map(m => m.Status)
                .Name("Status");
        }
    }
}
