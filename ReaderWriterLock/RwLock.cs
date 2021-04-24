using System;
using System.Collections.Generic;
using System.Threading;

namespace ReaderWriterLock
{
	public class RwLock : IRwLock
	{
		private readonly HashSet<object> _locks = new HashSet<object>();
		private readonly object _lockIn = new object();
	
		public void ReadLocked(Action action)
		{
			var obj = new object();
			lock (obj)
			{
				lock (_lockIn)
				{
					lock (_locks)
						_locks.Add(obj);
				}
			
				action();
				
				lock (_locks)
				{
					_locks.Remove(obj);
					if (_locks.Count == 0) 
						Monitor.Pulse(_locks);
				}
			}
		}

		public void WriteLocked(Action action)
		{
			lock (_lockIn)
			{
				lock (_locks)
				{
					if (_locks.Count > 0)
						Monitor.Wait(_locks);
					
					foreach (var obj in _locks)
						Monitor.Enter(obj);

					action();
				
					foreach (var obj in _locks)
						Monitor.Exit(obj);
				}
			}
		}
	}
}