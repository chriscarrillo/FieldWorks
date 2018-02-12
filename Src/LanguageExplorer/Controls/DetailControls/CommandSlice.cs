// Copyright (c) 2004-2013 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml.Linq;

namespace LanguageExplorer.Controls.DetailControls
{
	/// <summary>
	/// Class that shows a button (or hyperlink someday) that
	/// runs some arbitrary command, based on its ID.
	/// </summary>
	internal class CommandSlice : Slice
	{
		/// <summary>
		/// Store the Command object that knows what to do.
		/// </summary>
		private XElement m_cmdNode;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="node">The "deParams" node in some XDE file.</param>
		internal CommandSlice(XElement node)
		{
			Debug.Assert(node != null);
			var cmdNode = node.Element("command");
			Debug.Assert(cmdNode != null);
			m_cmdNode = cmdNode;
			var btn = new Button
			{
				FlatStyle = FlatStyle.Popup
			};
			btn.Click += btn_Click;
			Control = btn;
		}

		#region IDisposable override

		/// <summary>
		/// Executes in two distinct scenarios.
		///
		/// 1. If disposing is true, the method has been called directly
		/// or indirectly by a user's code via the Dispose method.
		/// Both managed and unmanaged resources can be disposed.
		///
		/// 2. If disposing is false, the method has been called by the
		/// runtime from inside the finalizer and you should not reference (access)
		/// other managed objects, as they already have been garbage collected.
		/// Only unmanaged resources can be disposed.
		/// </summary>
		/// <param name="disposing"></param>
		/// <remarks>
		/// If any exceptions are thrown, that is fine.
		/// If the method is being done in a finalizer, it will be ignored.
		/// If it is thrown by client code calling Dispose,
		/// it needs to be handled by fixing the bug.
		///
		/// If subclasses override this method, they should call the base implementation.
		/// </remarks>
		protected override void Dispose(bool disposing)
		{
			// Must not be run more than once.
			if (IsDisposed)
			{
				return;
			}

			if (disposing)
			{
				// Dispose managed resources here.
				var btn = Control as Button;
				if (btn != null)
				{
					btn.Click -= btn_Click;
				}
			}

			// Dispose unmanaged resources here, whether disposing is true or false.
			m_cmdNode = null;

			base.Dispose(disposing);
		}

		#endregion IDisposable override

		public override void RegisterWithContextHelper()
		{
			CheckDisposed();
			if (Control != null)//grouping nodes do not have a control
			{
#if RANDYTODO
				// TODO: Skip it for now, and figure out what to do with those context menus
				Publisher.Publish("RegisterHelpTargetWithId", new object[]{Control, ConfigurationNode.Attribute("label").Value, HelpId});
#endif
			}
		}

#if RANDYTODO
		protected override string HelpId
		{
			get { return m_command.Id; }
		}

		/// <summary>
		/// Override, so we can get the command object.
		/// </summary>
		public override XCore.Mediator Mediator
		{
			get
			{
				CheckDisposed();
				return base.Mediator;
			}
			set
			{
				CheckDisposed();
				base.Mediator = value;
				m_command = (Command)value.CommandSet[m_cmdNode.Attributes["cmdID"].Value];
				Debug.Assert(m_command != null);
				Control.Text = m_command.Label.Replace("_", null);
				Button b = (Button)Control;

				b.Width = 130;
			}
		}
#endif

		/// <summary>
		/// Handle click event on the button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Click(object sender, EventArgs e)
		{
#if RANDYTODO
			m_command.InvokeCommand();
#endif
		}
	}
}