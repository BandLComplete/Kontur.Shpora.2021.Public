using System;
using System.Collections.Generic;
using System.Threading;

namespace ReaderWriterLock
{
	public class RwLock : IRwLock
	{
		private readonly HashSet<object> _locks = new HashSet<object>();
	
		public void ReadLocked(Action action)
		{
			var obj = new object();
			
			lock (_locks)
				_locks.Add(obj);

			lock (obj)
				action();
			
			lock (_locks)
				_locks.Remove(obj);
		}

		public void WriteLocked(Action action)
		{
			lock (_locks)
			{
				foreach (var obj in _locks)
					Monitor.Enter(obj);

				action();
				
				foreach (var obj in _locks)
					Monitor.Exit(obj);
			}
		}
	}
}