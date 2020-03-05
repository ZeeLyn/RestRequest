using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace RestRequest.Builder
{
    internal class DownloadAsyncState
    {
        public long TotalLength { get; set; }

        public HttpWebRequest Request { get; set; }

        public string SaveFileName { get; set; }

        public Action<long, long, decimal> OnProgressChanged { get; set; }

        public Action OnCompleted { get; set; }

        public Action<string> OnError { get; set; }
        public Action<long, long> OnAborted { get; set; }

        public FileStream FileStream { get; set; }

        public CancellationToken CancellationToken { get; set; }
    }
}
