using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NsNodes
{
	public delegate void NsNodeTrackerEventHandler(object sender, NodeTrackerEventArgs e);
	/// <summary>
	/// Interface for views that will do the actual mouse tracking
	/// </summary>
	public interface ITracker
	{
		bool ClearTrackers();
		event NsNodeTrackerEventHandler TrackerDown;
		event NsNodeTrackerEventHandler TrackerDrag;
		event NsNodeTrackerEventHandler TrackerUp;
		event NsNodeTrackerEventHandler TrackerDetach;
	}
	/// <summary>
	/// Interface for objects that are going to be tracking the mouse via an ITracker view
	/// </summary>
	public interface ITrackable
	{
		/// <summary>
		/// Should attach this object's handlers to all desired ITracker events
		/// </summary>
		/// <param name="tracker">the view to attach to</param>
		void AttachTracker(ITracker tracker);
	}
}
