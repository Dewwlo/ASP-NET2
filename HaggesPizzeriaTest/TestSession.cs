using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HaggesPizzeriaTest
{
    internal class TestSession : ISession
    {
        private Dictionary<string, byte[]> _pizzeria
            = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

        public IEnumerable<string> Keys { get { return _pizzeria.Keys; } }

        public string Id { get; set; }

        public bool IsAvailable { get; } = true;

        public void Clear()
        {
            _pizzeria.Clear();
        }

        public Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(0);
        }

        public Task LoadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(0);
        }

        public void Remove(string key)
        {
            _pizzeria.Remove(key);
        }

        public void Set(string key, byte[] value)
        {
            _pizzeria[key] = value;
        }

        public bool TryGetValue(string key, out byte[] value)
        {
            return _pizzeria.TryGetValue(key, out value);
        }
    }
}
