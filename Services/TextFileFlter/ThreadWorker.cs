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
    public class ThreadWorker
    {
        private HashSet<string> _batch;
        private Hashtable _SNPMathFeatureHashtable;
        private NlogService _nlogService;
        public ThreadWorker(HashSet<string> batch, Hashtable SNPMathFeatureHashtable, NlogService nlogService)
        {
            _batch = batch;
            _SNPMathFeatureHashtable = SNPMathFeatureHashtable;
            _nlogService = nlogService;
        }
        public HashSet<ExportSampleData> Run()
        {
            HashSet<ExportSampleData> sampleData = new HashSet<ExportSampleData>();
            try
            {
                Parallel.ForEach(_batch, file =>
                {
                    // 這裡的 SampleData 使用區域變數以避免競爭條件
                    var localSampleData = new ReadSampleData(_nlogService);

                    _nlogService.WriteLine($"執行續讀取欲掃描之SampleData 路徑: {file}");
                    localSampleData.FilePath = file;
                    _nlogService.WriteLine($"執行續localSampleData 路徑: {file} 設定完畢");

                    _nlogService.WriteLine($"執行續 開始設定 localExportSampleData 路徑: {file}");
                    var localExportSampleData = new ExportSampleData()
                    {
                        FilePath = localSampleData.FilePath,
                        SampleID = localSampleData.SampleData.SampleID,
                        SNPDataHashtable = localSampleData.SNPDataToHash(),
                        SNPDataMathHash = localSampleData.SNPDataToMathHashtable(_SNPMathFeatureHashtable)
                    };
                    sampleData.Add(localExportSampleData);
                    _nlogService.WriteLine($"執行續 localExportSampleData 設定完畢 路徑: {file}");

                    _nlogService.WriteLine($"執行續 開始清空 localSampleData 路徑： {file}");
                    localSampleData.SampleData = null;
                    localSampleData = null;
                    _nlogService.WriteLine($"執行續 localSampleData 清空完畢 路徑： {file}");
                });
            }
            catch (Exception ex)
            {
                _nlogService.WriteLine(ex.Message, true);
                _nlogService.WriteLine($"以下是該批次的檔案路徑");
                foreach( var item in _batch) 
                {
                    _nlogService.WriteLine(item);
                }
            }
            return sampleData;
        }
    }
}
