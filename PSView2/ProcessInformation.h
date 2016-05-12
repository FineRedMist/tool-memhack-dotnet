#pragma once

using namespace ProcessTools;

namespace PSView2
{

	public ref class ProcessInformation
	{
	protected:
		UInt32 dwID;
		bool fModifiable;
		bool fIdleProcess;
		bool fSystem;
		bool fIsService;
		String^ szName;
		String^ szFullPath;
		String^ szUser;
		List<ProcessFriendlyName^>^ rgFriendlyNames;

		ProcessInformation();

	public:
		ProcessInformation(DWORD dwProcessID, String^ szProcessName);
		void Sort();
		void Add(ProcessFriendlyName^ name);
		String^ DefaultFriendlyName();

		property DWORD ID
		{
			DWORD get();
		}
		property String^ Name
		{
			String^ get();
		}

		property bool IsService
		{
			bool get();
			void set(bool value);
		}
		property bool Modifiable
		{
			bool get();
			void set(bool value);
		}
		property String^ FullPath
		{
			String^ get();
			void set(String^ value);
		}

		property int Count
		{
			int get();
		}
		ProcessFriendlyName^ FriendlyName(int idx);
	};
}
