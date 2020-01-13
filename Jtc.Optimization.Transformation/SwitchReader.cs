using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.Transformation
{
    public class SwitchReader : IDisposable
    {
        private readonly StreamReader _streamReader;
        private readonly StringReader _stringReader;

        public SwitchReader(StreamReader streamReader)
        {
            _streamReader = streamReader;
        }

        public SwitchReader(StringReader stringReader)
        {
            _stringReader = stringReader;
        }

        public async Task<string> ReadLineAsync()
        {
            return _streamReader != null ? await _streamReader.ReadLineAsync() : await _stringReader.ReadLineAsync();
        }

        public void Dispose()
        {
            _stringReader?.Dispose();
            _streamReader?.Dispose();
        }
    }
}
