using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TamakenService.Models.CubicAPI
{
    public class TextFileFilterInPutModel
    {
        public string SNPPath {  get; set; }
        public string SamplePathList { get; set;}
        public string SamplePath { get; set;}
        public string OutPutPath { get; set;}

    }
}
