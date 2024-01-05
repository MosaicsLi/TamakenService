using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TamakenService.Models.TextFileFilter
{
    public class SampleModel
    {
        public string SampleID { get; set; }
        public HashSet<SNPData> SNPData { get; set;}
    }
    public class SNPData
    {
        public string SNP { get; set; }
        public string Allele1 { get; set; }
        public string Allele2 { get; set; }

    }
    public class ExportSampleData
    {
        public string SampleID { get; set; }
        public string FilePath { get; set; }
        public Hashtable SNPDataHashtable { get; set; }
        public Hashtable SNPDataMathHash { get; set; }

    }
}
