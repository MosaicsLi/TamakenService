using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TamakenService.Log;

namespace TamakenService.Services.TextFileFlter
{
    public class ThreadWorker
    {
        HashSet<string> _batch;
        private NlogService _nlogService;
        public ThreadWorker(HashSet<string> batch, NlogService nlogService)
        {
            _batch=batch;
            _nlogService=nlogService;
        }
        public HashSet<ReadSampleData> Run()
        {
            HashSet<ReadSampleData> sampleData=new HashSet<ReadSampleData>();
            Parallel.ForEach(_batch, file =>
            {
                // 這裡的 SampleData 使用區域變數以避免競爭條件
                var localSampleData = new ReadSampleData(_nlogService);

                _nlogService.WriteLine($"執行續讀取欲掃描之SampleData 路徑: {file}");
                localSampleData.FilePath = file;
                _nlogService.WriteLine($"執行續localSampleData 設定完畢");
                sampleData.Add(localSampleData);
            });
            return sampleData;
        }
    }
}
