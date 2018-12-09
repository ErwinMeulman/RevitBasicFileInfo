using System;
using System.IO;
using System.IO.Packaging;
using System.Text;

namespace ConsoleApplication1
{
	internal class Program
	{
		private const string StreamName = "BasicFileInfo";

		private static void Main(string[] args)
		{
			string text = (1 == args.Length) ? args[0] : "pathToRevitFile";
			if (!StructuredStorageUtils.IsFileStucturedStorage(text))
			{
				throw new NotSupportedException("File is not a structured storage file");
			}
			byte[] rawBasicFileInfo = Program.GetRawBasicFileInfo(text);
			string @string = Encoding.Unicode.GetString(rawBasicFileInfo);
			string[] array = @string.Split(new string[2]
			{
				"\0",
				"\r\n"
			}, StringSplitOptions.RemoveEmptyEntries);
			string[] array2 = array;
			foreach (string value in array2)
			{
				Console.WriteLine(value);
			}
			Console.WriteLine("Big-endian:");
			@string = Encoding.BigEndianUnicode.GetString(rawBasicFileInfo);
			array = @string.Split(new string[2]
			{
				"\0",
				"\r\n"
			}, StringSplitOptions.RemoveEmptyEntries);
			string[] array3 = array;
			foreach (string value2 in array3)
			{
				Console.WriteLine(value2);
			}
		}

		private static byte[] GetRawBasicFileInfo(string revitFileName)
		{
			if (!StructuredStorageUtils.IsFileStucturedStorage(revitFileName))
			{
				throw new NotSupportedException("File is not a structured storage file");
			}
			using (StructuredStorageRoot structuredStorageRoot = new StructuredStorageRoot(revitFileName))
			{
				if (!structuredStorageRoot.BaseRoot.StreamExists("BasicFileInfo"))
				{
					throw new NotSupportedException(string.Format("File doesn't contain {0} stream", "BasicFileInfo"));
				}
				StreamInfo streamInfo = structuredStorageRoot.BaseRoot.GetStreamInfo("BasicFileInfo");
				using (Stream stream = streamInfo.GetStream(FileMode.Open, FileAccess.Read))
				{
					byte[] array = new byte[stream.Length];
					stream.Read(array, 0, array.Length);
					return array;
				}
			}
		}
	}
}
