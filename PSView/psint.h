/*****************************************************
	File: psint.h
	Written By: Brent Scriver

	Abstract:  Interface definition for passing the
		methods needed to update the progress bar.
		The implementation of the interface can then
		ensure the Invoke method is called as 
		appropriate.
******************************************************/

#pragma once
using namespace System;

namespace PSView
{
	// This just feels like a hack, but it works.  I originally was passing the windows form 
	// object for the progress bar directly and having it modify the values however I've found
	// out that updates to the UI needs to be done on the same thread as the owner of the UI 
	// requiring the use of Invoke (or it's async variations).
	// So this interface is implemented by the ProgressBar UI object so that I can update the 
	// UI from a different thread.  The implementation there uses Invoke to ensure it is 
	// updated correctly
	public __gc __interface IProgressIndicator
	{
        void SetMaximum(int maximum);
		void SetCurrent(int progress);
	};

};