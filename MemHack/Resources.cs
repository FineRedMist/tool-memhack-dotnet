using System;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Threading;

namespace MemHack
{
	/// <summary>
	/// Summary description for Resources.
	/// </summary>
	public class Resources
	{
		static ResourceManager rm = new ResourceManager("MemHack.strings", typeof(Resources).Assembly);
		static public bool ValidateResources = false;
		static ResourceSet rs = null;
		static string GetString(string name)
		{
			string s;
			lock(rm)
			{
				// ValidateResources allows a caller to ensure that they have configured all the string
				// values used in the UI.  It will throw an exception containing the locale if it can't find
				// the resources and it will throw an exception if it attempts to load a string that 
				// isn't defined in the resource set (and Test ensures that all strings are defined).
				if(ValidateResources && rs == null)
				{
					rs = rm.GetResourceSet(Thread.CurrentThread.CurrentCulture, false, false);
					if(rs == null)
						throw new System.NotImplementedException(Thread.CurrentThread.CurrentCulture.ToString());
				}
			}
			// ResourceSet.GetString doesn't quite follow the same semantics as ResourceManager.GetString
			// The former won't fall back to what is in the application resource manifest while the latter will
			// so if ValidateResources is off, we want to ensure we use that fallback since the manifest
			// does have them all defined.
			if(ValidateResources)
				s = rs.GetString(name);
			else
				s = rm.GetString(name);
			if(s == null || s == "")
				throw new System.NotImplementedException(name);
			return s;
		}

		public static string FindFirst
		{
			get{return GetString("FindFirst");}
		}

		public static string FindNext
		{
			get{return GetString("FindNext");}
		}

		public static string ValueLabel
		{
			get{return GetString("ValueLabel");}
		}

		public static string AddressesFoundLabel
		{
			get{return GetString("AddressesFoundLabel");}
		}

		public static string MessagesLabel
		{
			get{return GetString("MessagesLabel");}
		}

		public static string NoAddressesFound
		{
			get{return GetString("NoAddressesFound");}
		}

		public static string Set
		{
			get{return GetString("Set");}
		}

		public static string SearchSizeLabel
		{
			get{return GetString("SearchSizeLabel");}
		}

		public static string Unknown
		{
			get{return GetString("Unknown");}
		}

		public static string Bytes8
		{
			get{return GetString("8Bytes");}
		}

		public static string Bytes4
		{
			get{return GetString("4Bytes");}
		}

		public static string Bytes2
		{
			get{return GetString("2Bytes");}
		}

		public static string Bytes1
		{
			get{return GetString("1Byte");}
		}

		public static string FindFirstValue
		{
			get{return GetString("FindFirstValue");}
		}

		public static string Address
		{
			get{return GetString("Address");}
		}

		public static string Value
		{
			get{return GetString("Value");}
		}

		public static string MemoryHacker
		{
			get{return GetString("MemoryHacker");}
		}

		public static string MemoryHackerFormatString
		{
			get{return GetString("MemoryHackerFormatString");}
		}

		public static string SearchSize1Byte
		{
			get{return GetString("SearchSize1Byte");}
		}

		public static string SearchSize2Bytes
		{
			get{return GetString("SearchSize2Bytes");}
		}

		public static string SearchSize4Bytes
		{
			get{return GetString("SearchSize4Bytes");}
		}

		public static string SearchSize8Bytes
		{
			get{return GetString("SearchSize8Bytes");}
		}

		public static string SearchingForFormatString
		{
			get{return GetString("SearchingForFormatString");}
		}

		public static string FindFirstFoundFormatString
		{
			get{return GetString("FindFirstFoundFormatString");}
		}

		public static string FindFirstFailedFormatString
		{
			get{return GetString("FindFirstFailedFormatString");}
		}

		public static string FindNextFoundFormatString
		{
			get{return GetString("FindNextFoundFormatString");}
		}

		public static string FindNextFailedFormatString
		{
			get{return GetString("FindNextFailedFormatString");}
		}

		public static string FindFirstCanceled
		{
			get{return GetString("FindFirstCanceled");}
		}

		public static string FindNextCanceled
		{
			get{return GetString("FindNextCanceled");}
		}

		public static string SetValue
		{
			get{return GetString("SetValue");}
		}

		public static string ErrorInvalidAddressFormatString
		{
			get{return GetString("ErrorInvalidAddressFormatString");}
		}

		public static string ValueAtAddressFormatString
		{
			get{return GetString("ValueAtAddressFormatString");}
		}

		public static string ValueAtAddressSetFormatString
		{
			get{return GetString("ValueAtAddressSetFormatString");}
		}

		public static string ErrorValueNotSetFormatString
		{
			get{return GetString("ErrorValueNotSetFormatString");}
		}

		public static string ErrorSetValueFailedFormatString
		{
			get{return GetString("ErrorSetValueFailedFormatString");}
		}

		public static string SetValueCanceled
		{
			get{return GetString("SetValueCanceled");}
		}

		public static string SelectNext
		{
			get{return GetString("SelectNext");}
		}

		public static string ErrorFailedToConvert
		{
			get{return GetString("ErrorFailedToConvert");}
		}

		public static string ErrorProcessSelect
		{
			get{return GetString("ErrorProcessSelect");}
		}

		public static string ErrorProcessThisOne
		{
			get{return GetString("ErrorProcessThisOne");}
		}

		public static string ErrorProcessUnmodifiable
		{
			get{return GetString("ErrorProcessUnmodifiable");}
		}

		public static string ErrorProcessOpenFailed
		{
			get{return GetString("ErrorProcessOpenFailed");}
		}

		public static string Select
		{
			get{return GetString("Select");}
		}

		public static string RunningProgramsLabel
		{
			get{return GetString("RunningProgramsLabel");}
		}

		public static string ProcessNameLabel
		{
			get{return GetString("ProcessNameLabel");}
		}

		public static string ID
		{
			get{return GetString("ID");}
		}

		public static string Name
		{
			get{return GetString("Name");}
		}

		public static string Title
		{
			get{return GetString("Title");}
		}

		public static string IdleProcessName
		{
			get{return GetString("IdleProcessName");}
		}

		public static string SystemName
		{
			get{return GetString("SystemName");}
		}

		public static string ProgressTitle
		{
			get{return GetString("ProgressTitle");}
		}

		public static string ToolTip_ProcessList
		{
			get{return GetString("ToolTip_ProcessList");}
		}

		public static string ToolTip_FriendlyNames
		{
			get{return GetString("ToolTip_FriendlyNames");}
		}

		public static string ToolTip_SelectProcess
		{
			get{return GetString("ToolTip_SelectProcess");}
		}

		public static string ToolTip_ApplicationPath
		{
			get{return GetString("ToolTip_ApplicationPath");}
		}

		public static string ToolTip_MemHackDlg
		{
			get{return GetString("ToolTip_MemHackDlg");}
		}

		public static string ToolTip_AddressList
		{
			get{return GetString("ToolTip_AddressList");}
		}

		public static string ToolTip_FindFirst
		{
			get{return GetString("ToolTip_FindFirst");}
		}

		public static string ToolTip_FindNext
		{
			get{return GetString("ToolTip_FindNext");}
		}

		public static string ToolTip_Set
		{
			get{return GetString("ToolTip_Set");}
		}

		public static string ToolTip_Messages
		{
			get{return GetString("ToolTip_Messages");}
		}

		public static string ToolTip_MemDisplayDlg
		{
			get{return GetString("ToolTip_MemDisplayDlg");}
		}

		public static string ToolTip_FindFirst1Byte
		{
			get{return GetString("ToolTip_FindFirst1Byte");}
		}

		public static string ToolTip_FindFirst2Bytes
		{
			get{return GetString("ToolTip_FindFirst2Bytes");}
		}

		public static string ToolTip_FindFirst4Bytes
		{
			get{return GetString("ToolTip_FindFirst4Bytes");}
		}

		public static string ToolTip_FindFirst8Bytes
		{
			get{return GetString("ToolTip_FindFirst8Bytes");}
		}

		public static string ToolTip_FindFirstValue
		{
			get{return GetString("ToolTip_FindFirstValue");}
		}

		public static string ToolTip_FindFirstDlg
		{
			get{return GetString("ToolTip_FindFirstDlg");}
		}

		public static string ToolTip_FindFirstButton
		{
			get{return GetString("ToolTip_FindFirstButton");}
		}

		public static string ToolTip_FindNextDlg
		{
			get{return GetString("ToolTip_FindNextDlg");}
		}

		public static string ToolTip_FindNextValue
		{
			get{return GetString("ToolTip_FindNextValue");}
		}

		public static string ToolTip_FindNextButton
		{
			get{return GetString("ToolTip_FindNextButton");}
		}

		public static string ToolTip_SetDlg
		{
			get{return GetString("ToolTip_SetDlg");}
		}

		public static string ToolTip_SetValue
		{
			get{return GetString("ToolTip_SetValue");}
		}

		public static string ToolTip_SetButton
		{
			get{return GetString("ToolTip_SetButton");}
		}

		public static string ToolTip_ProgressFormatString
		{
			get{return GetString("ToolTip_ProgressFormatString");}
		}

		public static void Test()
		{
			// If there is localization information, I like to be sure it is all defined correctly--useful for
			// external localizers to ensure that all the strings are defined correctly.
			MethodInfo [] mis = typeof(Resources).GetMethods(BindingFlags.Public | BindingFlags.Static);
			object [] paramList = new object [0];
			string res = "";
			foreach(MethodInfo mi in mis)
			{
				if(mi.ReturnType != typeof(string) ||
					mi.GetParameters().Length != 0 ||
					mi.IsSpecialName == false || 
					!mi.Name.StartsWith("get_"))
					continue;
				// System.Diagnostics.Debug.WriteLine("Testing: " + mi.Name);
				res = (string) mi.Invoke(null, paramList);
			}
		}

		~Resources()
		{
			rm.ReleaseAllResources();
		}
	}
}
