
using System;
using System.Collections.Generic;
using System.Text;

using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;

namespace OutlookPrivacyPlugin
{
  /// <summary>
  /// GnuPG CommandBar wrapper class
  /// </summary>
  internal class GnuPGCommandBar
  {
    /// <summary>
    /// Contants and members.
    /// </summary>
    private const string _cmdBarName = "OutlookGnuPG";
    private Outlook.Explorer _explorer = null;
    private Office.CommandBar _commandBar = null;

    // Button list/dictionary.
    private Dictionary<string, Office.CommandBarButton> _buttons = new Dictionary<string, Office.CommandBarButton>();

    /// <summary>
    /// Public access to some properties.
    /// </summary>
    internal Office.CommandBar CommandBar
    {
      get { return _commandBar; }
    }
    internal Outlook.Explorer Explorer
    {
      get { return _explorer; }
    }

    /// <summary>
    /// The constructor
    /// </summary>
    /// <param name="activeExplorer">The Outlook active explorer containing the CommandBar(s).</param>
    public GnuPGCommandBar(Outlook.Explorer activeExplorer)
    {
      _explorer = activeExplorer;
    }

    /// <summary>
    /// Helper function to find a named CommandBar
    /// </summary>
    /// <param name="name">CommandBar name</param>
    /// <returns>The CommandBar found or null.</returns>
    private Office.CommandBar Find(String name)
    {
      foreach (Office.CommandBar bar in _explorer.CommandBars)
        if (bar.Name == name)
          return bar;
      return null;
    }

    /// <summary>
    /// Remove the GnuPG CommandBar, if any.
    /// </summary>
    /// <param name="explorer"></param>
    internal void Remove()
    {
      Office.CommandBar bar = Find(_cmdBarName);
      if (bar == null)
        return;
      bar.Delete();
    }

    /// <summary>
    /// Add a new CommandBar
    /// </summary>
    internal void Add()
    {
      if (_explorer == null)
        return;

      _commandBar = Find(_cmdBarName);
      if (_commandBar == null)
      {
        Office.CommandBars bars = _explorer.CommandBars;
        _commandBar = bars.Add(_cmdBarName, Office.MsoBarPosition.msoBarTop, false, true);
      }
      _commandBar.Visible = true;

      foreach (string btn in new string[] { "About", "Settings", "Decrypt", "Verify" })
      {
        _buttons.Add(btn, (Office.CommandBarButton)_commandBar.Controls.Add(Office.MsoControlType.msoControlButton,
                                                                            Type.Missing, Type.Missing, 1, true));
        _buttons[btn].Style = Office.MsoButtonStyle.msoButtonIconAndCaption;
        _buttons[btn].Caption = btn;
        _buttons[btn].Tag = "GnuPG" + btn;
      }

      // http://www.kebabshopblues.co.uk/2007/01/04/visual-studio-2005-tools-for-office-commandbarbutton-faceid-property/
      _buttons["Decrypt"].FaceId = 718;
      _buttons["Verify"].FaceId = 719;
      _buttons["About"].FaceId = 700;
      _buttons["Settings"].FaceId = 2144;

      _buttons["Decrypt"].Picture = ImageConverter.Convert(Properties.Resources.lock_edit);
      _buttons["Verify"].Picture = ImageConverter.Convert(Properties.Resources.link_edit);
      _buttons["About"].Picture = ImageConverter.Convert(Properties.Resources.Logo);
      _buttons["Settings"].Picture = ImageConverter.Convert(Properties.Resources.database_gear);
    }


    /// <summary>
    /// Return a given button by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    internal Office.CommandBarButton GetButton(string name)
    {
      if (false == _buttons.ContainsKey(name))
        return null;
      return _buttons[name];
    }

    /// <summary>
    /// Save the CommandBar position in application property settings.
    /// </summary>
    /// <param name="settings"></param>
    internal void SavePosition(Properties.Settings settings)
    {
      settings.BarLeft = _commandBar.Left;
      settings.BarPosition = (int)_commandBar.Position;
      settings.BarPositionSaved = true;
      settings.BarRowIndex = _commandBar.RowIndex;
      settings.BarTop = _commandBar.Top;
      settings.Save();
    }

    /// <summary>
    /// Set the CommandBar position from application property settings.
    /// </summary>
    /// <param name="settings"></param>
    internal void RestorePosition(Properties.Settings settings)
    {
      // Position the bar
      if (settings.BarPositionSaved)
      {
        _commandBar.Position = (Office.MsoBarPosition)settings.BarPosition;
        _commandBar.RowIndex = settings.BarRowIndex;
        _commandBar.Top = settings.BarTop;
        _commandBar.Left = settings.BarLeft;
      }
      else
      {
        Office.CommandBar standardBar = Find("standard");
        if (standardBar != null)
        {
          int oldPos = standardBar.Left;
          _commandBar.RowIndex = standardBar.RowIndex;
          _commandBar.Left = standardBar.Left + standardBar.Width;
          _commandBar.Position = Office.MsoBarPosition.msoBarTop;
          standardBar.Left = oldPos;
        }
        else
        {
          _commandBar.Position = Office.MsoBarPosition.msoBarTop;
        }
      }
    }
  }
}
