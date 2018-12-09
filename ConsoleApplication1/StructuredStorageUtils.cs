using System.IO;
using System.Runtime.InteropServices;

namespace ConsoleApplication1
{
	public static class StructuredStorageUtils
	{
		[DllImport("ole32.dll")]
		private static extern int StgIsStorageFile([MarshalAs(UnmanagedType.LPWStr)] string pwcsName);

		public static bool IsFileStucturedStorage(string fileName)
		{
			switch (StructuredStorageUtils.StgIsStorageFile(fileName))
			{
			case 0:
				return true;
			case 1:
				return false;
			default:
				throw new FileNotFoundException("File not found", fileName);
			}
		}
	}
}
