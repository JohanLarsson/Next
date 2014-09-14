namespace Next.FeedCommands
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    internal class MessageReader : IDisposable
    {
        private readonly MemoryStream _innerStream = new MemoryStream(4096);
        private readonly StreamReader _innerReader;
        private readonly StreamWriter _innerWriter;
        private bool _isDisposed = false;

        private BufferedStream _bufferedStream;
        public MessageReader(Stream stream)
        {
            _innerReader = new StreamReader(stream);
            _innerWriter = new StreamWriter(_innerStream);
        }

        public async Task ReadMessageAsync()
        {
            while (true)
            {
                throw new NotImplementedException("message");
                
                //_innerReader.ReadBlockAsync(_buffer);
                //await _innerReader.ReadAsync(buffer, 0, 1).ConfigureAwait(false);
                //if (buffer[0] == 13)
                //{
                //    _innerStream.Position = 0;
                //    return;
                //}
                //_innerWriter.Write(buffer[0]);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_isDisposed)
            {
                if (_innerReader != null)
                {
                    _innerReader.Dispose();
                }
                if (_innerWriter != null)
                {
                    _innerWriter.Dispose();
                }
            }
        }
    }
}
