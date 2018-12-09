using System;
using System.IO;
using System.IO.Packaging;
using System.Reflection;

namespace ConsoleApplication1
{
	public class StructuredStorageRoot : IDisposable
	{
		private StorageInfo _storageRoot;

		public StorageInfo BaseRoot
		{
			get
			{
				return this._storageRoot;
			}
		}

		public StructuredStorageRoot(Stream stream)
		{
			try
			{
				this._storageRoot = (StorageInfo)StructuredStorageRoot.InvokeStorageRootMethod(null, "CreateOnStream", stream);
			}
			catch (Exception innerException)
			{
				throw new StructuredStorageException("Cannot get StructuredStorageRoot", innerException);
			}
		}

		public StructuredStorageRoot(string fileName)
		{
			try
			{
				this._storageRoot = (StorageInfo)StructuredStorageRoot.InvokeStorageRootMethod(null, "Open", fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			catch (Exception innerException)
			{
				throw new StructuredStorageException("Cannot get StructuredStorageRoot", innerException);
			}
		}

		private static object InvokeStorageRootMethod(StorageInfo storageRoot, string methodName, params object[] methodArgs)
		{
			Type type = typeof(StorageInfo).Assembly.GetType("System.IO.Packaging.StorageRoot", true, false);
			return type.InvokeMember(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, storageRoot, methodArgs);
		}

		private void CloseStorageRoot()
		{
			StructuredStorageRoot.InvokeStorageRootMethod(this._storageRoot, "Close");
		}

		public void Dispose()
		{
			this.CloseStorageRoot();
		}
	}
}
