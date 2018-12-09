using System;

namespace ConsoleApplication1
{
	public class StructuredStorageException : Exception
	{
		public StructuredStorageException()
		{
		}

		public StructuredStorageException(string message)
			: base(message)
		{
		}

		public StructuredStorageException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
