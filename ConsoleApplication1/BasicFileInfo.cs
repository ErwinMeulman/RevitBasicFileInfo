using System;
using System.IO;
using System.IO.Packaging;
using System.Text;

namespace Filemanager
{
	public class BasicFileInfo : StorageStreamBase
	{
		private WorkSharingMode _workSharing = WorkSharingMode.Unknown;

		private string _userName = string.Empty;

		private string _centralFilePath = string.Empty;

		private string _revitBuild = string.Empty;

		private string _lastSavedpath = string.Empty;

		private int _openWorksetDefault;

		public bool IsCentralFile
		{
			get
			{
				if (this.WorkSharing != 0 && this.WorkSharing != WorkSharingMode.Unknown)
				{
					bool flag = false;
					bool num = this.WorkSharing == WorkSharingMode.Central;
					string value = Path.GetFileName(base.FileName).ToUpper();
					string text = string.Empty;
					if (this.CentralFilePath.Length > 0)
					{
						text = Path.GetFileName(this.CentralFilePath).ToUpper();
					}
					if (text.Length > 0)
					{
						flag = text.Equals(value);
					}
					return num & flag;
				}
				return false;
			}
		}

		public bool IsLocalWorkingFile
		{
			get
			{
				if (this.WorkSharing != 0 && this.WorkSharing != WorkSharingMode.Unknown)
				{
					bool flag = false;
					bool num = this.WorkSharing == WorkSharingMode.Local;
					string value = Path.GetFileName(base.FileName).ToUpper();
					string text = string.Empty;
					if (this.CentralFilePath.Length > 0)
					{
						text = Path.GetFileName(this.CentralFilePath).ToUpper();
					}
					if (text.Length > 0)
					{
						flag = text.Equals(value);
					}
					if (!num)
					{
						return !flag;
					}
					return true;
				}
				return false;
			}
		}

		public WorkSharingMode WorkSharing
		{
			get
			{
				return this._workSharing;
			}
			private set
			{
				this._workSharing = value;
			}
		}

		public string UserName
		{
			get
			{
				return this._userName;
			}
			private set
			{
				this._userName = value;
			}
		}

		public string CentralFilePath
		{
			get
			{
				return this._centralFilePath;
			}
			private set
			{
				this._centralFilePath = value;
			}
		}

		public string RevitBuild
		{
			get
			{
				return this._revitBuild;
			}
			private set
			{
				this._revitBuild = value;
			}
		}

		public ProductType Product
		{
			get
			{
				if (string.IsNullOrEmpty(this.RevitBuild))
				{
					return ProductType.Unknown;
				}
				if (this.RevitBuild.ToUpper().IndexOf("MEP") >= 0)
				{
					return ProductType.MEP;
				}
				if (this.RevitBuild.ToUpper().IndexOf("ARCHITECTURE") >= 0)
				{
					return ProductType.Architecture;
				}
				if (this.RevitBuild.ToUpper().IndexOf("STRUCTURE") >= 0)
				{
					return ProductType.Structure;
				}
				return ProductType.Unknown;
			}
		}

		public string BuildTimeStamp
		{
			get
			{
				if (string.IsNullOrEmpty(this.RevitBuild))
				{
					return string.Empty;
				}
				string[] array = this.RevitBuild.Split(':');
				if (array != null && array.Length == 2)
				{
					return array[1].Trim().Replace("(x64))", string.Empty).Replace("(x64)", string.Empty)
						.Replace(")", string.Empty)
						.Trim();
				}
				return this._revitBuild.Trim();
			}
		}

		public PlatformType Platform
		{
			get
			{
				if (string.IsNullOrEmpty(this.RevitBuild))
				{
					return PlatformType.Unknown;
				}
				if (this.RevitBuild.ToUpper().IndexOf("X64") >= 0)
				{
					return PlatformType.x64;
				}
				return PlatformType.x86;
			}
		}

		public string LastSavedpath
		{
			get
			{
				return this._lastSavedpath;
			}
			private set
			{
				this._lastSavedpath = value;
			}
		}

		public int OpenWorksetDefault
		{
			get
			{
				return this._openWorksetDefault;
			}
			private set
			{
				this._openWorksetDefault = value;
			}
		}

		public BasicFileInfo(string fileName, StorageInfo storage)
			: base(fileName, storage)
		{
			this.ReadStructuredStorageFile();
		}

		private void ParseDetailInfo(string detailInfo)
		{
			detailInfo = detailInfo.Trim();
			detailInfo.IndexOf(":");
			string oldValue = detailInfo.Substring(detailInfo.IndexOf(":") + 1);
			string text = detailInfo.Substring(0, detailInfo.IndexOf(":"));
			text = text.Trim().ToUpper().Replace(" ", string.Empty);
			text = StringUtility.PurgeUnprintableCharacters(text);
			oldValue = StringUtility.PurgeUnprintableCharacters(oldValue);
			switch (text)
			{
			default:
				if (text == "OPENWORKSETDEFAULT")
				{
					this.OpenWorksetDefault = Convert.ToInt32(oldValue.Trim());
				}
				break;
			case "WORKSHARING":
				if (string.IsNullOrEmpty(oldValue))
				{
					this.WorkSharing = WorkSharingMode.Unknown;
				}
				else
				{
					switch (oldValue.Replace(" ", string.Empty).Trim().ToUpper())
					{
					case "NOTENABLED":
						this.WorkSharing = WorkSharingMode.NotEnabled;
						break;
					case "LOCAL":
						this.WorkSharing = WorkSharingMode.Local;
						break;
					case "CENTRAL":
						this.WorkSharing = WorkSharingMode.Central;
						break;
					default:
						this.WorkSharing = WorkSharingMode.Unknown;
						break;
					}
				}
				break;
			case "USERNAME":
				this.UserName = oldValue.Trim();
				break;
			case "CENTRALFILEPATH":
				this.CentralFilePath = oldValue.Trim();
				break;
			case "REVITBUILD":
				this.RevitBuild = oldValue.Trim();
				break;
			case "LASTSAVEPATH":
				this.LastSavedpath = oldValue.Trim();
				break;
			}
		}

		public void ReadStructuredStorageFile()
		{
			if (!base.IsInitialized)
			{
				try
				{
					StreamInfo[] streams = base.Storage.GetStreams();
					foreach (StreamInfo streamInfo in streams)
					{
						if (streamInfo.Name.ToUpper().Equals("BASICFILEINFO"))
						{
							string[] array = StringUtility.ConvertStreamBytesToUnicode(streamInfo).Split(default(char));
							foreach (string text in array)
							{
								if (text.IndexOf("\r\n") >= 0)
								{
									string[] array2 = text.Split(new string[1]
									{
										"\r\n"
									}, StringSplitOptions.None);
									foreach (string detailInfo in array2)
									{
										this.ParseDetailInfo(detailInfo);
									}
								}
							}
						}
					}
				}
				catch (Exception)
				{
					base.IsInitialized = false;
				}
				base.IsInitialized = true;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string empty = string.Empty;
			try
			{
				if (this != null)
				{
					empty = "+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+" + Environment.NewLine;
					stringBuilder.Append(string.Format("FileName: <{0}>{1}", base.FileName, Environment.NewLine));
					stringBuilder.Append(empty);
					stringBuilder.Append(string.Format("BasicFileInfo Section{0}", Environment.NewLine));
					stringBuilder.Append(empty);
					stringBuilder.Append(string.Format("DocType: <{0}>{1}", base.DocType, Environment.NewLine));
					stringBuilder.Append(string.Format("WorkSharing: <{0}>{1}", this.WorkSharing, Environment.NewLine));
					stringBuilder.Append(string.Format("IsCentralFile: <{0}>{1}", this.IsCentralFile, Environment.NewLine));
					stringBuilder.Append(string.Format("UserName: <{0}>{1}", this.UserName, Environment.NewLine));
					stringBuilder.Append(string.Format("CentralFilePath: <{0}>{1}", this.CentralFilePath, Environment.NewLine));
					stringBuilder.Append(string.Format("RevitBuild: <{0}>{1}", this.RevitBuild, Environment.NewLine));
					stringBuilder.Append(string.Format("Product: <{0}>{1}", this.Product, Environment.NewLine));
					stringBuilder.Append(string.Format("Platform: <{0}>{1}", this.Platform, Environment.NewLine));
					stringBuilder.Append(string.Format("BuildTimeStamp: <{0}>{1}", this.BuildTimeStamp, Environment.NewLine));
					stringBuilder.Append(string.Format("LastSavedpath: <{0}>{1}", this.LastSavedpath, Environment.NewLine));
					stringBuilder.Append(string.Format("OpenWorksetDefault: <{0}>{1}", this.OpenWorksetDefault, Environment.NewLine));
					stringBuilder.Append(empty);
					return stringBuilder.ToString();
				}
			}
			finally
			{
				stringBuilder.Length = 0;
				stringBuilder.Capacity = 0;
				stringBuilder = null;
			}
			return string.Empty;
		}
	}
}
