
using System;
using System.Collections.Generic;
using System.Text;
using Outlook = Microsoft.Office.Interop.Outlook;

// Helper wrapper classes used to monitor items.
// See http://www.outlookcode.com/codedetail.aspx?id=1734
namespace OutlookPrivacyPlugin
{
	#region Base class wrapper
	/// <summary>
	/// Delegate signature to inform the application about disposing objects.
	/// </summary>
	/// <param name="id">The unique ID of the closed object.</param>
	/// <param name="o">The closed object.</param>
	public delegate void OutlookWrapperDisposeDelegate(Guid id, object o);

	/// <summary>
	/// The OutlookWrapper Class itself has a unique ID, the wrapped object and a closed event.
	/// </summary>
	internal abstract class OutlookWrapper
	{
		/// <summary>
		/// The event occurs when the monitored object is disposed.
		/// </summary>
		public event OutlookWrapperDisposeDelegate Dispose = null;

		/// <summary>
		/// The pointer to the wrapped object.
		/// </summary>
		protected object _wrapped;

		/// <summary>
		/// The unique ID of the wrapped object.
		/// </summary>
		private Guid _Id;
		public Guid Id { get { return _Id; } private set { _Id = value; } }

		/// <summary>
		/// Handle the close of the wrapped object.
		/// </summary>
		protected void OnClosed()
		{
			if (Dispose != null) { Dispose(Id, this); }
			_wrapped = null;
		}

		/// <summary>
		/// The constructor (what else)
		/// </summary>
		/// <param name="o">The pointer to the object to monitor</param>
		public OutlookWrapper(object o)
		{
			Id = Guid.NewGuid();
			_wrapped = o;
		}
	}
	#endregion

	#region Explorer wrapper
	/// <summary>
	/// Delegate signature to handle (some) explorer events.
	/// </summary>
	/// <param name="explorer">the explorer for which the event is fired</param>
	public delegate void ExplorerActivateDelegate(Outlook.Explorer explorer);
	public delegate void ExplorerDeactivateDelegate(Outlook.Explorer explorer);
	public delegate void ExplorerViewSwitchDelegate(Outlook.Explorer explorer);
	public delegate void ExplorerSelectionChangeDelegate(Outlook.Explorer explorer);
	public delegate void ExplorerCloseDelegate(Outlook.Explorer explorer);

	/// <summary>
	/// 
	/// </summary>
	internal class ExplorerWrapper : OutlookWrapper
	{
		/// <summary>
		/// Public exlorer events.
		/// </summary>
		public event ExplorerActivateDelegate Activate = null;
		public event ExplorerDeactivateDelegate Deactivate = null;
		public event ExplorerViewSwitchDelegate ViewSwitch = null;
		public event ExplorerCloseDelegate Close = null;
		public event ExplorerSelectionChangeDelegate SelectionChange = null;

		public ExplorerWrapper(Outlook.Explorer explorer)
			: base(explorer)
		{
			ConnectEvents();
		}

		private void ConnectEvents()
		{
			Outlook.Explorer explorer = _wrapped as Outlook.Explorer;

			// Hookup explorer events
			((Outlook.ExplorerEvents_10_Event)explorer).Activate += new Outlook.ExplorerEvents_10_ActivateEventHandler(ExplorerWrapper_Activate);
			((Outlook.ExplorerEvents_10_Event)explorer).Deactivate += new Outlook.ExplorerEvents_10_DeactivateEventHandler(ExplorerWrapper_Deactivate);
			((Outlook.ExplorerEvents_10_Event)explorer).ViewSwitch += new Outlook.ExplorerEvents_10_ViewSwitchEventHandler(ExplorerWrapper_ViewSwitch);
			((Outlook.ExplorerEvents_10_Event)explorer).Close += new Outlook.ExplorerEvents_10_CloseEventHandler(ExplorerWrapper_Close);
			((Outlook.ExplorerEvents_10_Event)explorer).SelectionChange += new Outlook.ExplorerEvents_10_SelectionChangeEventHandler(ExplorerWrapper_SelectionChange);
		}

		void ExplorerWrapper_Close()
		{
			if (Close != null) { Close(_wrapped as Outlook.Explorer); }
			DisconnectEvents();
			GC.Collect();
			GC.WaitForPendingFinalizers();
			OnClosed();
		}

		void ExplorerWrapper_SelectionChange()
		{
			if (SelectionChange != null) { SelectionChange(_wrapped as Outlook.Explorer); }
		}

		void ExplorerWrapper_ViewSwitch()
		{
			if (ViewSwitch != null) { ViewSwitch(_wrapped as Outlook.Explorer); }
		}

		void ExplorerWrapper_Deactivate()
		{
			if (Deactivate != null) { Deactivate(_wrapped as Outlook.Explorer); }
		}

		private void ExplorerWrapper_Activate()
		{
			if (Activate != null) { Activate(_wrapped as Outlook.Explorer); }
		}

		private void DisconnectEvents()
		{
			Outlook.Explorer explorer = _wrapped as Outlook.Explorer;

			// Unhook events from the explorer
			((Outlook.ExplorerEvents_10_Event)explorer).Activate -= new Outlook.ExplorerEvents_10_ActivateEventHandler(ExplorerWrapper_Activate);
			((Outlook.ExplorerEvents_10_Event)explorer).Deactivate -= new Outlook.ExplorerEvents_10_DeactivateEventHandler(ExplorerWrapper_Deactivate);
			((Outlook.ExplorerEvents_10_Event)explorer).ViewSwitch -= new Outlook.ExplorerEvents_10_ViewSwitchEventHandler(ExplorerWrapper_ViewSwitch);
			((Outlook.ExplorerEvents_10_Event)explorer).Close -= new Outlook.ExplorerEvents_10_CloseEventHandler(ExplorerWrapper_Close);
			((Outlook.ExplorerEvents_10_Event)explorer).SelectionChange -= new Outlook.ExplorerEvents_10_SelectionChangeEventHandler(ExplorerWrapper_SelectionChange);
		}
	}
	#endregion

	#region Inspector wrapper
	/// <summary>
	/// Delegate signature to handle (some) inspector events.
	/// </summary>
	/// <param name="inspector">the inspector for which the event is fired</param>
	public delegate void InspectorCloseDelegate(Outlook.Inspector inspector);

	/// <summary>
	/// The wrapper class to warp an Inspector objet.
	/// </summary>
	internal class InspectorWrapper : OutlookWrapper
	{
		/// <summary>
		/// Public inspector events.
		/// </summary>
		public event InspectorCloseDelegate Close = null;

		/// <summary>
		/// The constructor
		/// </summary>
		/// <param name="inspector">the inspector object to monitor</param>
		public InspectorWrapper(Outlook.Inspector inspector)
			: base(inspector)
		{
			ConnectEvents();
		}

		/// <summary>
		/// Connect inspector events, hookup the close event.
		/// </summary>
		private void ConnectEvents()
		{
			Outlook.Inspector inspector = _wrapped as Outlook.Inspector;

			// Hookup inspector events
			((Outlook.InspectorEvents_Event)inspector).Close += new Outlook.InspectorEvents_CloseEventHandler(InspectorWrapper_Close);
		}

		/// <summary>
		/// The close event handler fired when the inspector closes.
		/// </summary>
		void InspectorWrapper_Close()
		{
			if (Close != null) { Close(_wrapped as Outlook.Inspector); }
			DisconnectEvents();
			GC.Collect();
			GC.WaitForPendingFinalizers();
			OnClosed();
		}

		/// <summary>
		/// Disconnect inspector events, unhook close event.
		/// </summary>
		protected virtual void DisconnectEvents()
		{
			Outlook.Inspector inspector = _wrapped as Outlook.Inspector;

			// Unhook events from the inspector
			((Outlook.InspectorEvents_Event)inspector).Close -= new Outlook.InspectorEvents_CloseEventHandler(InspectorWrapper_Close);
		}
	}
	#endregion

	#region MailItem wrapper
	/// <summary>
	/// Delegate signature to handle (some) mailItem events.
	/// </summary>
	/// <param name="mailItem">the mailItem for which the event is fired</param>
	/// <param name="Cancel">False when the event occurs. If the event procedure sets this argument to True,
	/// the open operation is not completed and the inspector is not displayed.</param>
	public delegate void MailItemInspectorOpenDelegate(Outlook.MailItem mailItem, ref bool Cancel);
	public delegate void MailItemInspectorSaveDelegate(Outlook.MailItem mailItem, ref bool Cancel);
	public delegate void MailItemInspectorCloseDelegate(Outlook.MailItem mailItem, ref bool Cancel);
	public delegate void MailItemInspectorReplyDelegate(Outlook.MailItem mailItem, ref bool Cancel);
	public delegate void MailItemInspectorReplyAllDelegate(Outlook.MailItem mailItem, ref bool Cancel);

	/// <summary>
	/// The wrapper class to monitor a mailItem.
	/// </summary>
	internal class MailItemInspector : InspectorWrapper
	{
		/// <summary>
		/// Private member(s).
		/// </summary>
		private Outlook.MailItem _mailItem = null;

		/// <summary>
		/// Public mailItem events.
		/// </summary>
		public event MailItemInspectorOpenDelegate Open = null;
		public event MailItemInspectorSaveDelegate Save = null;
		public event MailItemInspectorCloseDelegate MyClose = null;
		public event MailItemInspectorReplyDelegate Reply = null;
		public event MailItemInspectorReplyAllDelegate ReplyAll = null;

		/// <summary>
		/// The constructor to record the associate mailItem and register events.
		/// </summary>
		/// <param name="inspector"></param>
		public MailItemInspector(Outlook.Inspector inspector)
			: base(inspector)
		{
			_mailItem = inspector.CurrentItem as Outlook.MailItem;
			if (_mailItem == null)
				throw new Exception("Not a mailItem in the provided inspector");
			ConnectEvents();
		}

		/// <summary>
		/// Connect mailItem events, hookup the open, write and close events.
		/// </summary>
		private void ConnectEvents()
		{
			((Outlook.ItemEvents_10_Event)_mailItem).Open += new Outlook.ItemEvents_10_OpenEventHandler(MailItemInspector_Open);
			((Outlook.ItemEvents_10_Event)_mailItem).Close += new Outlook.ItemEvents_10_CloseEventHandler(MailItemInspector_Close);
			((Outlook.ItemEvents_10_Event)_mailItem).Write += new Outlook.ItemEvents_10_WriteEventHandler(MailItemInspector_Write);
			((Outlook.ItemEvents_10_Event)_mailItem).Reply += new Outlook.ItemEvents_10_ReplyEventHandler(MailItemInspector_Reply);
			((Outlook.ItemEvents_10_Event)_mailItem).ReplyAll += new Outlook.ItemEvents_10_ReplyAllEventHandler(MailItemInspector_ReplyAll);
		}

		void MailItemInspector_ReplyAll(object Response, ref bool Cancel)
		{
			if (ReplyAll != null)
				ReplyAll(_mailItem, ref Cancel);
		}

		void MailItemInspector_Reply(object Response, ref bool Cancel)
		{
			if (Reply != null)
				Reply(_mailItem, ref Cancel);
		}

		/// <summary>
		/// MailItem events: Open, Write and MyClose.
		/// Calls the registered application mailItem events.
		/// </summary>
		/// <param name="Cancel">False when the event occurs. If the event procedure sets this argument to True,
		/// the open operation is not completed and the inspector is not displayed.</param>
		private void MailItemInspector_Open(ref bool Cancel)
		{
			if (Open != null) 
				Open(_mailItem, ref Cancel);
		}

		private void MailItemInspector_Write(ref bool Cancel)
		{
			if (Save != null) 
				Save(_mailItem, ref Cancel);
		}

		private void MailItemInspector_Close(ref bool Cancel)
		{
			if (MyClose != null) 
				MyClose(_mailItem, ref Cancel);
		}

		/// <summary>
		/// Disconnect mailItem events, unhook open, write and close events.
		/// </summary>
		protected override void DisconnectEvents()
		{
			((Outlook.ItemEvents_10_Event)_mailItem).Open -= new Outlook.ItemEvents_10_OpenEventHandler(MailItemInspector_Open);
			((Outlook.ItemEvents_10_Event)_mailItem).Close -= new Outlook.ItemEvents_10_CloseEventHandler(MailItemInspector_Close);
			((Outlook.ItemEvents_10_Event)_mailItem).Write -= new Outlook.ItemEvents_10_WriteEventHandler(MailItemInspector_Write);
			((Outlook.ItemEvents_10_Event)_mailItem).Reply -= new Outlook.ItemEvents_10_ReplyEventHandler(MailItemInspector_Reply);
			((Outlook.ItemEvents_10_Event)_mailItem).ReplyAll -= new Outlook.ItemEvents_10_ReplyAllEventHandler(MailItemInspector_ReplyAll);

			base.DisconnectEvents();
		}
	}
	#endregion
}
