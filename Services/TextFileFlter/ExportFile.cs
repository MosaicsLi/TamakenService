using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TamakenService.Log;
using TamakenService.Models.TextFileFilter;

namespace TamakenService.Services.TextFileFlter
{
    public class ExportFile
    {
        private List<string> SNPIndexList;
        private Hashtable ASAToGWASTable;
        private NlogService nlogService;
        public ExportFile(List<string> _SNPIndexList, Hashtable _ASAToGWASTable, NlogService _nlogService)
        {
            SNPIndexList = _SNPIndexList;
            nlogService = _nlogService;
            ASAToGWASTable = _ASAToGWASTable;
        }
        public void SaveSetToMathCSV(HashSet<ExportSampleData> sampleSet, string outputPath)
        {
            foreach (var sample in sampleSet)
            {
                nlogService.WriteLine($"執行續開始輸出 數據資料");
                SaveToMathCSV(sample, outputPath);
                nlogService.WriteLine($"輸出 {sample.SampleID} 數據資料完畢");
            }
        }
        public void SaveSetToCSV(HashSet<ExportSampleData> sampleSet, string outputPath)
        {
            foreach (var sample in sampleSet)
            {
                nlogService.WriteLine($"執行續開始輸出 基因資料");
                SaveToCSV(sample, outputPath);
                nlogService.WriteLine($"輸出 {sample.SampleID} 基因完畢");
            }
        }
        private void writeTitle(StreamWriter writer, List<string> SNPIndexList)
        {
            StringBuilder lineBuilder = new StringBuilder();
            lineBuilder.Append($"SampleID_GWAS");
            foreach (string SNP in SNPIndexList)
            {
                lineBuilder.Append($",{ASAToGWASTable[SNP]}");
            }
            writer.WriteLine(lineBuilder.ToString());
            lineBuilder.Clear();

            lineBuilder.Append($"SampleID_ASA");
            foreach (string SNP in SNPIndexList)
            {
                lineBuilder.Append($",{SNP}");
            }
            lineBuilder.Append($",Path");
            writer.WriteLine(lineBuilder.ToString());
        }
        public void SaveToCSV(ExportSampleData sample, string outputPath)
        {
            try
            {
                var SampleSNPSet = sample.SNPDataHashtable;
                bool fileExists = File.Exists(outputPath);
                using (StreamWriter writer = new StreamWriter(outputPath, true))
                {
                    if (!fileExists)
                    {
                        writeTitle(writer, SNPIndexList);
                    }

                    StringBuilder lineBuilder = new StringBuilder();
                    lineBuilder.Append($"{sample.SampleID}");
                    nlogService.WriteLine($"正在輸出 Sample:{sample.SampleID} 的基因資料 Sample位置 {sample.FilePath}");
                    foreach (string SNP in SNPIndexList)
                    {

                        lineBuilder.Append(",");
                        if (SampleSNPSet.ContainsKey(SNP))
                        {
                            lineBuilder.Append($"{SampleSNPSet[SNP]}");
                        }
                    }

                    lineBuilder.Append($",{sample.FilePath}");
                    writer.WriteLine(lineBuilder.ToString());
                }
                SampleSNPSet = null;
            }
            catch (Exception ex)
            {
                nlogService.WriteLine($"SNPs:{SNPIndexList.ToString()}");
                nlogService.WriteLine($"ASAToGWASTable:{ASAToGWASTable.Count}");
                nlogService.WriteLine($"outputPath:{outputPath}");
                nlogService.WriteLine($"Exception:{ex.Message}");
            }
        }
        public void SaveToMathCSV(ExportSampleData sample, string outputPath)
        {
            try
            {
                var SampleSNPSet = sample.SNPDataMathHash;
                bool fileExists = File.Exists(outputPath);
                using (StreamWriter writer = new StreamWriter(outputPath, true))
                {
                    if (!fileExists)
                    {
                        writeTitle(writer, SNPIndexList);
                    }

                    StringBuilder lineBuilder = new StringBuilder();
                    lineBuilder.Append($"{sample.SampleID}");
                    nlogService.WriteLine($"正在輸出 Sample:{sample.SampleID} 的數據資料 Sample位置 {sample.FilePath}");
                    foreach (string SNP in SNPIndexList)
                    {
                        lineBuilder.Append($",");
                        if (SampleSNPSet.ContainsKey(SNP))
                        {
                            lineBuilder.Append($"{SampleSNPSet[SNP]}");
                        }
                    }
                    lineBuilder.Append($",{sample.FilePath}");
                    writer.WriteLine(lineBuilder.ToString());
                }
                SampleSNPSet = null;
            }
            catch (Exception ex)
            {
                nlogService.WriteLine($"SNPs:{SNPIndexList.ToString()}");
                nlogService.WriteLine($"ASAToGWASTable:{ASAToGWASTable.Count}");
                nlogService.WriteLine($"outputPath:{outputPath}");
                nlogService.WriteLine($"Exception:{ex.Message}");
            }
        }
    }
}
