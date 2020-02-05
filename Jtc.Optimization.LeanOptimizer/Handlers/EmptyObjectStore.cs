using QuantConnect.Interfaces;
using QuantConnect.Packets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.LeanOptimizer
{
    public class EmptyObjectStore : IObjectStore
    {
        public event EventHandler<ObjectStoreErrorRaisedEventArgs> ErrorRaised;

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string key)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public IEnumerator<KeyValuePair<string, byte[]>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public string GetFilePath(string key)
        {
            throw new NotImplementedException();
        }

        public void Initialize(string algorithmName, int userId, int projectId, string userToken, Controls controls)
        {
        }

        public byte[] ReadBytes(string key)
        {
            throw new NotImplementedException();
        }

        public bool SaveBytes(string key, byte[] contents)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
