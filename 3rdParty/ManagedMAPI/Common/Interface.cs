#region File Info
//
// File       : interface.cs
// Description: MAPI Common Interface Definitions
// Package    : ManagedMAPI
//
// Authors    : Fred Song
//
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ManagedMAPI
{
    /// <summary>
    /// Manages objects associated with a MAPI logon session.
    /// </summary>
    [
      ComImport, ComVisible(false),
      InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
      Guid("00020300-0000-0000-C000-000000000046")
   ]

    public interface IMAPISession
    {
        /// <summary>
        /// Returns a MAPIERROR structure that contains information about the previous session error.
        /// </summary>
        /// <param name="hResult">A handle to the error value generated in the previous method call.</param>
        /// <param name="ulFlags">A bitmask of flags that controls the type of strings returned.</param>
        /// <param name="lppMAPIError">A pointer to a pointer to a MAPIERROR structure that contains version, component, and context information for the error. </param>
        /// <returns>S_OK, if the call succeeded and has returned the expected value or values; otherwise, failed.</returns>
        HRESULT GetLastError(int hResult, uint ulFlags, out IntPtr lppMAPIError);
        /// <summary>
        /// Provides access to the message store table that contains information about all the message stores in the session profile.
        /// </summary>
        /// <param name="ulFlags">A bitmask of flags that determines the format for columns that are character strings.</param>
        /// <param name="lppTable">A pointer to a pointer to the message store table.</param>
        /// <returns>S_OK, if the table was successfully returned; otherwise, failed.</returns>
        HRESULT GetMsgStoresTable(uint ulFlags, out IntPtr lppTable);
        /// <summary>
        /// Opens a message store and returns an IMsgStore pointer for further access.
        /// </summary>
        /// <param name="ulUIParam">A handle to the parent window of the common address dialog box and other related displays.</param>
        /// <param name="cbEntryID">The byte count in the entry identifier pointed to by the lpEntryID parameter.</param>
        /// <param name="lpEntryID">A pointer to the entry identifier of the message store to be opened. The lpEntryID parameter must not be NULL.</param>
        /// <param name="lpInterface">A pointer to the interface identifier (IID) that represents the interface to be used to access the message store.</param>
        /// <param name="ulFlags">A bitmask of flags that controls how the object is opened.</param>
        /// <param name="lppMDB">Pointer to a pointer of the message store.</param>
        /// <returns>S_OK, if the message store was successfully opened; otherwise, failed.</returns>
        HRESULT OpenMsgStore(uint ulUIParam, uint cbEntryID, IntPtr lpEntryID, IntPtr lpInterface, uint ulFlags, out IntPtr lppMDB);
        /// <summary>
        /// Opens the MAPI integrated address book, returning an IAddrBook pointer for further access.
        /// </summary>
        /// <param name="ulUIParam">A handle to the parent window of the common address dialog box and other related displays.</param>
        /// <param name="lpInterface">A pointer to the interface identifier (IID) that represents the interface to be used to access the address book.</param>
        /// <param name="ulFlags">A bitmask of flags that controls the opening of the address book.</param>
        /// <param name="lppAdrBook">A pointer to a pointer to the address book.</param>
        /// <returns>S_OK, if the address book was successfully opened; otherwise, failed.</returns>
        HRESULT OpenAddressBook(uint ulUIParam, IntPtr lpInterface, uint ulFlags, out IntPtr lppAdrBook);
        /// <summary>
        /// Opens a section of the current profile and returns an IProfSect pointer for further access.
        /// </summary>
        /// <param name="lpUID">pointer to the MAPIUID structure that identifies the profile section.</param>
        /// <param name="lpInterface">A pointer to the interface identifier (IID) that represents the interface to be used to access the profile section. </param>
        /// <param name="ulFlags">A bitmask of flags that controls access to the profile section. </param>
        /// <param name="lppProfSect">A pointer to a pointer to the profile section.</param>
        /// <returns>S_OK, if the profile section was successfully opened; otherwise, failed.</returns>
        HRESULT OpenProfileSection(ref Guid lpUID, ref Guid lpInterface, uint ulFlags, out IntPtr lppProfSect);
        /// <summary>
        /// Provides access to the status table, a table that contains information about all the MAPI resources in the session.
        /// </summary>
        /// <param name="ulFlags">A bitmask of flags that determines the format for columns that are character strings.</param>
        /// <param name="lppTable">A pointer to a pointer to the status table.</param>
        /// <returns>S_OK, if the table was successfully returned; otherwise, failed.</returns>
        HRESULT GetStatusTable(uint ulFlags, out IntPtr lppTable);
        /// <summary>
        /// Opens an object and returns an interface pointer for additional access.
        /// </summary>
        /// <param name="cbEntryID">The byte count in the entry identifier pointed to by the lpEntryID parameter.</param>
        /// <param name="lpEntryID">A pointer to the entry identifier of the object to open.</param>
        /// <param name="lpInterface">A pointer to the interface identifier (IID) that represents the interface to be used to access the opened object.</param>
        /// <param name="ulFlags">A bitmask of flags that controls how the object is opened. </param>
        /// <param name="lpulObjType">The type of the opened object</param>
        /// <param name="lppUnk">A pointer to a pointer to the opened object.</param>
        /// <returns>S_OK, if the object was opened successfully; otherwise, failed</returns>
        HRESULT OpenEntry(uint cbEntryID, IntPtr lpEntryID, IntPtr lpInterface, uint ulFlags, out uint lpulObjType, out IntPtr lppUnk);
        /// <summary>
        /// Compares two entry identifiers to determine whether they refer to the same entry in a message store.
        /// </summary>
        /// <param name="cbEntryID1">The byte count in the entry identifier pointed to by the lpEntryID1 parameter.</param>
        /// <param name="lpEntryID1">A pointer to the first entry identifier to be compared.</param>
        /// <param name="cbEntryID2">The byte count in the entry identifier pointed to by the lpEntryID2 parameter.</param>
        /// <param name="lpEntryID2">A pointer to the second entry identifier to be compared.</param>
        /// <param name="ulFlags">Reserved; must be zero.</param>
        /// <param name="lpulResult">true if the two entry identifiers refer to the same object; otherwise, false</param>
        /// <returns>S_OK, if the comparison was successful; otherwise, failed.</returns>
        HRESULT CompareEntryIDs(uint cbEntryID1, IntPtr lpEntryID1, uint cbEntryID2, IntPtr lpEntryID2, uint ulFlags, out bool lpulResult);
        /// <summary>
        /// Registers to receive notification of specified events that affect the message store.
        /// </summary>
        /// <param name="cbEntryID">The byte count in the entry identifier pointed to by the lpEntryID parameter.</param>
        /// <param name="lpEntryID">A pointer to the entry identifier of the folder or message about which notifications should be generated, or null. If lpEntryID is set to NULL, Advise registers for notifications on the entire message store.</param>
        /// <param name="ulEventMask">A mask of values that indicate the types of notification events that the caller is interested in and should be included in the registration.</param>
        /// <param name="pAdviseSink">A pointer to an advise sink object to receive the subsequent notifications.</param>
        /// <param name="lpulConnection">A pointer to a nonzero number that represents the connection between the caller's advise sink object and the session.</param>
        /// <returns>S_OK, if the registration was successful; otherwise, failed.</returns>
        [PreserveSig]
        HRESULT Advise(uint cbEntryID, IntPtr lpEntryID, uint ulEventMask, IntPtr pAdviseSink, out uint lpulConnection);
        /// <summary>
        /// Cancels the sending of notifications previously set up with a call to the IMsgStore::Advise method.
        /// </summary>
        /// <param name="ulConnection">The connection number associated with an active notification registration.</param>
        /// <returns>S_OK, if the registration was successfully canceled; otherwise, failed.</returns>
        [PreserveSig]
        HRESULT Unadvise(uint ulConnection);
        /// <exclude/>
        HRESULT MessageOptions(uint ulUIParam, uint ulFlags, [MarshalAs(UnmanagedType.LPWStr)] string lpszAdrType, IntPtr lpMessage);
        /// <exclude/>
        HRESULT QueryDefaultMessageOpt([MarshalAs(UnmanagedType.LPWStr)] string lpszAdrType, uint ulFlags, out uint lpcValues, out IntPtr lppOptions);
        /// <exclude/>
        HRESULT EnumAdrTypes(uint ulFlags, out uint lpcAdrTypes, out IntPtr lpppszAdrTypes);
        /// <summary>
        /// Returns the entry identifier of the object that provides the primary identity for the session.
        /// </summary>
        /// <param name="lpcbEntryID">A pointer to the byte count in the entry identifier pointed to by the lppEntryID parameter.</param>
        /// <param name="lppEntryID">A pointer to a pointer to the entry identifier of the object that provides the primary identity.</param>
        /// <returns>S_OK, if the primary identity was successfully returned; otherwise, failed.</returns>
        HRESULT QueryIdentity(out uint lpcbEntryID, out IntPtr lppEntryID);
        /// <summary>
        /// Ends a MAPI session.
        /// </summary>
        /// <param name="ulUIParam">A handle to the parent window of any dialog boxes or windows to be displayed. </param>
        /// <param name="ulFlags">A bitmask of flags that control the logoff operation. </param>
        /// <param name="ulReserved">Reserved; must be zero.</param>
        /// <returns>S_OK, if the logoff operation was successful; otherwise, failed.</returns>
        HRESULT Logoff(uint ulUIParam, uint ulFlags, uint ulReserved);
        /// <summary>
        /// Establishes a message store as the default message store for the session.
        /// </summary>
        /// <param name="ulFlags">A bitmask of flags that controls the setting of the default message store.</param>
        /// <param name="cbEntryID">The byte count in the entry identifier pointed to by the lpEntryID parameter.</param>
        /// <param name="lpEntryID">A pointer to the entry identifier of the message store that is intended as the default.</param>
        /// <returns>S_OK, if the call succeeded and returned the expected value or values; otherwise, failed.</returns>
        HRESULT SetDefaultStore(uint ulFlags, uint cbEntryID, IntPtr lpEntryID);
        /// <summary>
        /// Returns an IMsgServiceAdmin pointer for making changes to message services. 
        /// </summary>
        /// <param name="ulFlags">Reserved; must be zero.</param>
        /// <param name="lppServiceAdmin">A pointer to a pointer to a message service administration object.</param>
        /// <returns>S_OK, if a pointer to a message service administration object was successfully returned; otherwise, failed.</returns>
        HRESULT AdminServices(uint ulFlags, out IntPtr lppServiceAdmin);
        /// <summary>
        /// Displays a form.
        /// </summary>
        /// <param name="ulUIParam">A handle to the parent window of the form.</param>
        /// <param name="lpMsgStore">A pointer to the message store that contains the folder pointed to by the lpParentFolder parameter. </param>
        /// <param name="lpParentFolder">A pointer to the folder in which the message associated with the ulMessageToken parameter was created. </param>
        /// <param name="lpInterface">A pointer to the interface identifier (IID) that represents the interface to be used to access the message that is displayed in the form.</param>
        /// <param name="ulMessageToken">The token that is associated with the message to be displayed in the form. </param>
        /// <param name="lpMessageSent">Reserved; must be NULL.</param>
        /// <param name="ulFlags">A bitmask of flags that controls how and whether the message is saved.</param>
        /// <param name="ulMessageStatus">A bitmask of flags copied from the PR_MSG_STATUS property of the message associated with the token in the ulMessageToken parameter.</param>
        /// <param name="ulMessageFlags">A bitmask of flags copied from the PR_MESSAGE_FLAGS property of the message associated with the token in the ulMessageToken parameter. </param>
        /// <param name="ulAccess">A flag that indicates the permission level for the message that is displayed in the form. </param>
        /// <param name="lpszMessageClass">The message class of the message being displayed in the form.</param>
        /// <returns>S_OK, if the form was successfully displayed; otherwise, failed.</returns>
        HRESULT ShowForm(uint ulUIParam, IntPtr lpMsgStore, IntPtr lpParentFolder, ref Guid lpInterface, uint ulMessageToken,
        IntPtr lpMessageSent, uint ulFlags, uint ulMessageStatus, uint ulMessageFlags, uint ulAccess, [MarshalAs(UnmanagedType.LPWStr)] string lpszMessageClass);
        /// <summary>
        /// Creates a numeric token that the IMAPISession::ShowForm method uses to access a message.
        /// </summary>
        /// <param name="lpInterface">A pointer to the interface identifier (IID) that represents the interface to be used to access the message.</param>
        /// <param name="lpMessage">A pointer to the message to be displayed in the form.</param>
        /// <param name="lpulMessageToken">A pointer to a message token, which is used by the IMAPISession::ShowForm method to access the message pointed to by lpMessage.</param>
        /// <returns>S_OK, if the form preparation was successful; otherwise, failed.</returns>
        HRESULT PrepareForm(ref Guid lpInterface, IntPtr lpMessage, out uint lpulMessageToken);
    }

    /// <summary>
    /// Provides a read-only view of a table. IMAPITable is used by clients and service providers to manipulate the way a table appears. 
    /// </summary>
    [Guid("00020301-0000-0000-c000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMAPITable
    {
        /// <summary>
        /// Returns a MAPIERROR structure containing information about the previous error on the table.
        /// </summary>
        /// <param name="hResult">HRESULT containing the error generated in the previous method call.</param>
        /// <param name="ulFlags">Bitmask of flags that controls the type of the returned strings. </param>
        /// <param name="lppMAPIError">Pointer to a pointer to the returned MAPIERROR structure containing version, component, and context information for the error.</param>
        /// <returns>S_OK, if the call succeeded and has returned the expected value or values; otherwise, failed.</returns>
        HRESULT GetLastError(int hResult, uint ulFlags, out IntPtr lppMAPIError);
        /// <summary>
        /// Registers an advise sink object to receive notification of specified events affecting the table.
        /// </summary>
        /// <param name="ulEventMask">Value indicating the type of event that will generate the notification.</param>
        /// <param name="lpAdviseSink">Pointer to an advise sink object to receive the subsequent notifications. This advise sink object must have been already allocated.</param>
        /// <param name="lpulConnection">Pointer to a nonzero value that represents the successful notification registration.</param>
        /// <returns>S_OK, if the notification registration successfully completed; otherwise, failed.</returns>
        HRESULT Advise(uint ulEventMask, IntPtr lpAdviseSink, IntPtr lpulConnection);
        /// <summary>
        /// Cancels the sending of notifications previously set up with a call to the IMAPITable::Advise method.
        /// </summary>
        /// <param name="ulConnection">The number of the registration connection returned by a call to IMAPITable::Advise.</param>
        /// <returns>S_OK, if the call succeeded; otherwise, failed.</returns>
        HRESULT Unadvise(uint ulConnection);
        /// <summary>
        /// Returns the table's status and type.
        /// </summary>
        /// <param name="lpulTableStatus">Pointer to a value indicating the status of the table.</param>
        /// <param name="lpulTableType">Pointer to a value that indicates the table's type.</param>
        /// <returns>S_OK, if the table's status was successfully returned; otherwise, failed.</returns>
        HRESULT GetStatus(IntPtr lpulTableStatus, IntPtr lpulTableType);
        /// <summary>
        /// Defines the particular properties and order of properties to appear as columns in the table.
        /// </summary>
        /// <param name="lpPropTagArray">Pointer to an array of property tags identifying properties to be included as columns in the table. </param>
        /// <param name="ulFlags">Bitmask of flags that controls the return of an asynchronous call to SetColumns.</param>
        /// <returns>S_OK, if the column setting operation was successful; otherwise, failed.</returns>
        HRESULT SetColumns([MarshalAs(UnmanagedType.LPArray)] uint[] lpPropTagArray, uint ulFlags);
        /// <summary>
        /// Returns a list of columns for the table.
        /// </summary>
        /// <param name="ulFlags">Bitmask of flags that indicates which column set should be returned.</param>
        /// <param name="lpPropTagArray">Pointer to an SPropTagArray structure containing the property tags for the column set.</param>
        /// <returns>S_OK, if the column set was successfully returned; otherwise, failed.</returns>
        HRESULT QueryColumns(uint ulFlags, IntPtr lpPropTagArray);
        /// <summary>
        /// Returns the total number of rows in the table. 
        /// </summary>
        /// <param name="ulFlags">Reserved; must be zero.</param>
        /// <param name="lpulCount">Pointer to the number of rows in the table.</param>
        /// <returns>S_OK, if the row count was successfully returned; otherwise, failed.</returns>
        HRESULT GetRowCount(uint ulFlags, out uint lpulCount);
        /// <summary>
        /// Moves the cursor to a specific position in the table.
        /// </summary>
        /// <param name="bkOrigin">The bookmark identifying the starting position for the seek operation.</param>
        /// <param name="lRowCount">The signed count of the number of rows to move, starting from the bookmark identified by the bkOrigin parameter.</param>
        /// <param name="lplRowsSought">If lRowCount is a valid pointer on input, lplRowsSought points to the number of rows that were processed in the seek operation, the sign of which indicates the direction of search, forward or backward. If lRowCount is negative, then lplRowsSought is negative.</param>
        /// <returns>S_OK, if the seek operation was successful; otherwise, failed.</returns>
        HRESULT SeekRow(int bkOrigin, int lRowCount, out IntPtr lplRowsSought);
        /// <summary>
        /// Moves the cursor to an approximate fractional position in the table. 
        /// </summary>
        /// <param name="ulNumerator">The numerator of the fraction representing the table position</param>
        /// <param name="ulDenominator">The denominator of the fraction representing the table position</param>
        /// <returns>S_OK, if the seek operation was successful; otherwise, failed.</returns>
        HRESULT SeekRowApprox(uint ulNumerator, uint ulDenominator);
        /// <summary>
        /// Retrieves the current table row position of the cursor, based on a fractional value.
        /// </summary>
        /// <param name="lpulRow">Pointer to the number of the current row.</param>
        /// <param name="lpulNumerator">Pointer to the numerator for the fraction identifying the table position.</param>
        /// <param name="lpulDenominator">Pointer to the denominator for the fraction identifying the table position.</param>
        /// <returns>S_OK, if the method returned valid values in lpulRow, lpulNumerator, and lpulDenominator; otherwise, failed.</returns>
        HRESULT QueryPosition(IntPtr lpulRow, IntPtr lpulNumerator, IntPtr lpulDenominator);
        /// <summary>
        /// Finds the next row in a table that matches specific search criteria and moves the cursor to that row.
        /// </summary>
        /// <param name="lpRestriction">A pointer to an SRestriction structure that describes the search criteria.</param>
        /// <param name="BkOrigin">A bookmark identifying the row where FindRow should begin its search.</param>
        /// <param name="ulFlags">A bitmask of flags that controls the direction of the search.</param>
        /// <returns>S_OK, if the find operation was successful; otherwise, failed.</returns>
        HRESULT FindRow(out IntPtr lpRestriction, uint BkOrigin, uint ulFlags);
        /// <summary>
        /// Applies a filter to a table, reducing the row set to only those rows matching the specified criteria.
        /// </summary>
        /// <param name="lpRestriction">Pointer to an SRestriction structure defining the conditions of the filter. Passing NULL in the lpRestriction parameter removes the current filter.</param>
        /// <param name="ulFlags">Bitmask of flags that controls the timing of the restriction operation.</param>
        /// <returns>S_OK, if the filter was successfully applied; otherwise, failed.</returns>
        HRESULT Restrict(out IntPtr lpRestriction, uint ulFlags);
        /// <summary>
        /// Creates a bookmark at the table's current position.
        /// </summary>
        /// <param name="lpbkPosition">Pointer to the returned 32-bit bookmark value. This bookmark can later be passed in a call to the IMAPITable::SeekRow method</param>
        /// <returns>S_OK, if the call succeeded and has returned the expected value or values; otherwise, failed.</returns>
        HRESULT CreateBookmark(IntPtr lpbkPosition);
        /// <summary>
        /// Releases the memory associated with a bookmark.
        /// </summary>
        /// <param name="bkPosition">The bookmark to be freed, created by calling the IMAPITable::CreateBookmark method.</param>
        /// <returns>S_OK, if the bookmark was successfully freed; otherwise, failed.</returns>
        HRESULT FreeBookmark(IntPtr bkPosition);
        /// <summary>
        /// Orders the rows of the table, depending on sort criteria.
        /// </summary>
        /// <param name="lpSortCriteria">Pointer to an SSortOrderSet structure that contains the sort criteria to apply.</param>
        /// <param name="ulFlags">Bitmask of flags that controls the timing of the IMAPITable::SortTable operation.</param>
        /// <returns>S_OK, if the sort operation was successful; otherwise, failed.</returns>
        HRESULT SortTable(IntPtr lpSortCriteria, int ulFlags);
        /// <summary>
        /// Retrieves the current sort order for a table.
        /// </summary>
        /// <param name="lppSortCriteria">Pointer to a pointer to the SSortOrderSet structure holding the current sort order.</param>
        /// <returns>S_OK, if the current sort order was successfully returned; otherwise, failed.</returns>
        HRESULT QuerySortOrder(IntPtr lppSortCriteria);
        /// <summary>
        /// Returns one or more rows from a table, beginning at the current cursor position.
        /// </summary>
        /// <param name="lRowCount">Maximum number of rows to be returned.</param>
        /// <param name="ulFlags">Bitmask of flags that control how rows are returned.</param>
        /// <param name="lppRows">Pointer to a pointer to an SRowSet structure holding the table rows.</param>
        /// <returns>S_OK, if the rows were successfully returned; otherwise, failed.</returns>
        HRESULT QueryRows(int lRowCount, uint ulFlags, out IntPtr lppRows);
        /// <summary>
        /// Stops any asynchronous operations currently in progress for the table.
        /// </summary>
        /// <returns>S_OK, if one or more asynchronous operations have been stopped; otherwise, failed.</returns>
        HRESULT Abort();
        /// <summary>
        /// Expands a collapsed table category, adding the leaf or lower-level heading rows belonging to the category to the table view.
        /// </summary>
        /// <param name="cbInstanceKey">The count of bytes in the PR_INSTANCE_KEY property pointed to by the pbInstanceKey parameter.</param>
        /// <param name="pbInstanceKey">A pointer to the PR_INSTANCE_KEY property that identifies the heading row for the category.</param>
        /// <param name="ulRowCount">The maximum number of rows to return in the lppRows parameter. </param>
        /// <param name="ulFlags">Reserved; must be zero.</param>
        /// <param name="lppRows">A pointer to an SRowSet structure receiving the first (up to ulRowCount) rows that have been inserted into the table view as a result of the expansion.</param>
        /// <param name="lpulMoreRows">A pointer to the total number of rows that were added to the table view.</param>
        /// <returns>S_OK, if the category was expanded successfully; otherwise, failed.</returns>
        HRESULT ExpandRow(uint cbInstanceKey, IntPtr pbInstanceKey, uint ulRowCount, uint ulFlags, IntPtr lppRows, IntPtr lpulMoreRows);
        /// <summary>
        /// Collapses an expanded table category, removing any lower-level headings and leaf rows belonging to the category from the table view.
        /// </summary>
        /// <param name="cbInstanceKey">The count of bytes in the PR_INSTANCE_KEY property pointed to by the pbInstanceKey parameter.</param>
        /// <param name="pbInstanceKey">A pointer to the PR_INSTANCE_KEY property that identifies the heading row for the category. </param>
        /// <param name="ulFlags">Reserved; must be zero.</param>
        /// <param name="lpulRowCount">A pointer to the total number of rows that are being removed from the table view.</param>
        /// <returns>S_OK, if the collapse operation has succeeded; otherwise, failed.</returns>
        HRESULT CollapseRow(uint cbInstanceKey, IntPtr pbInstanceKey, uint ulFlags, IntPtr lpulRowCount);
        /// <summary>
        /// Suspends processing until one or more asynchronous operations in progress on the table have completed.
        /// </summary>
        /// <param name="ulFlags">Reserved; must be zero.</param>
        /// <param name="ulTimeout">Maximum number of milliseconds to wait for the asynchronous operation or operations to complete.</param>
        /// <param name="lpulTableStatus">On input, either a valid pointer or NULL. On output, if lpulTableStatus is a valid pointer, it points to the most recent status of the table. </param>
        /// <returns>S_OK, if the wait operation was successful; otherwise, failed.</returns>
        HRESULT WaitForCompletion(uint ulFlags, uint ulTimeout, IntPtr lpulTableStatus);
        /// <summary>
        /// Returns the data that is needed to rebuild the current collapsed or expanded state of a categorized table.
        /// </summary>
        /// <param name="ulFlags">Reserved; must be zero.</param>
        /// <param name="cbInstanceKey">The count of bytes in the instance key pointed to by the lpbInstanceKey parameter.</param>
        /// <param name="lpbInstanceKey">A pointer to the PR_INSTANCE_KEY property of the row at which the current collapsed or expanded state should be rebuilt. </param>
        /// <param name="lpcbCollapseState">A pointer to the count of structures pointed to by the lppbCollapseState parameter.</param>
        /// <param name="lppbCollapseState">A pointer to a pointer to structures that contain data that describes the current table view.</param>
        /// <returns>S_OK, if the state for the categorized table was successfully saved; otherwise, failed.</returns>
        HRESULT GetCollapseState(uint ulFlags, uint cbInstanceKey, IntPtr lpbInstanceKey, IntPtr lpcbCollapseState, IntPtr lppbCollapseState);
        /// <summary>
        /// Rebuilds the current expanded or collapsed state of a categorized table using data that was saved by a prior call to the IMAPITable::GetCollapseState method.
        /// </summary>
        /// <param name="ulFlags">Reserved; must be zero.</param>
        /// <param name="cbCollapseState">Count of bytes in the structure pointed to by the pbCollapseState parameter.</param>
        /// <param name="pbCollapseState">Pointer to the structures containing the data needed to rebuild the table view.</param>
        /// <param name="lpbkLocation">Pointer to a bookmark identifying the row in the table at which the collapsed or expanded state should be rebuilt. </param>
        /// <returns>S_OK, if the state of the categorized table was successfully rebuilt; otherwise, failed.</returns>
        HRESULT SetCollapseState(uint ulFlags, uint cbCollapseState, IntPtr pbCollapseState, IntPtr lpbkLocation);

    }

    /// <summary>
    /// The IMAPIAdviseSink interface is used to implement an Advise Sink object for handling notifications.
    /// </summary>
    [
       ComImport, ComVisible(false),
       InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
       Guid("00020302-0000-0000-C000-000000000046")
    ]
    public interface IMAPIAdviseSink
    {
        /// <summary>
        /// The OnNotify method responds to a notification by performing one or more tasks, which depend on the object generating the notification, and type of event.
        /// </summary>
        /// <param name="cNotify">Ignored</param>
        /// <param name="lpNotifications">Reference to one NOTIFICATION structure that provides information about the events that have occurred.</param>
        /// <returns>S_OK, if the notification was processed successfully; otherwise, failed.</returns>
        HRESULT OnNotify(uint cNotify, IntPtr lpNotifications);
    }

    /// <summary>
    /// Provides access to message store information and to messages and folders.
    /// </summary>
    [
        ComImport, ComVisible(false),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid("00020306-0000-0000-C000-000000000046")
    ]
    public interface IMsgStore
    {
        /// <exclude/>
        HRESULT GetLastError(int hResult, uint ulFlags, out IntPtr lppMAPIError);
        /// <exclude/>
        HRESULT SaveChanges(uint ulFlags);
        /// <exclude/>
        HRESULT GetProps([In, MarshalAs(UnmanagedType.LPArray)] uint[] lpPropTagArray, uint ulFlags, out uint lpcValues, ref IntPtr lppPropArray);
        /// <exclude/>
        HRESULT GetPropList(uint ulFlags, out IntPtr lppPropTagArray);
        /// <exclude/>
        HRESULT OpenProperty(uint ulPropTag, ref Guid lpiid, uint ulInterfaceOptions, uint ulFlags, out IntPtr lppUnk);
        /// <exclude/>
        HRESULT SetProps(uint cValues, IntPtr lpPropArray, out IntPtr lppProblems);
        /// <exclude/>
        HRESULT DeleteProps(IntPtr lpPropTagArray, out IntPtr lppProblems);
        /// <exclude/>
        HRESULT CopyTo(uint ciidExclude, ref Guid rgiidExclude, [In, MarshalAs(UnmanagedType.LPArray)] uint[] lpExcludeProps, IntPtr ulUIParam,
            IntPtr lpProgress, ref Guid lpInterface, IntPtr lpDestObj, uint ulFlags, IntPtr lppProblems);
        /// <exclude/>
        HRESULT CopyProps(IntPtr lpIncludeProps, uint ulUIParam, IntPtr lpProgress, ref Guid lpInterface,
            IntPtr lpDestObj, uint ulFlags, out IntPtr lppProblems);
        /// <exclude/>
        HRESULT GetNamesFromIDs(out IntPtr lppPropTags, ref Guid lpPropSetGuid, uint ulFlags,
            out uint lpcPropNames, out IntPtr lpppPropNames);
        /// <exclude/>
        HRESULT GetIDsFromNames(uint cPropNames, ref IntPtr lppPropNames, uint ulFlags, out IntPtr lppPropTags);
        /// <summary>
        /// Registers to receive notification of specified events that affect the message store.
        /// </summary>
        /// <param name="cbEntryID">The byte count in the entry identifier pointed to by the lpEntryID parameter.</param>
        /// <param name="lpEntryID">A pointer to the entry identifier of the folder or message about which notifications should be generated, or null. If lpEntryID is set to NULL, Advise registers for notifications on the entire message store.</param>
        /// <param name="ulEventMask">A mask of values that indicate the types of notification events that the caller is interested in and should be included in the registration.</param>
        /// <param name="pAdviseSink">A pointer to an advise sink object to receive the subsequent notifications.</param>
        /// <param name="lpulConnection">A pointer to a nonzero number that represents the connection between the caller's advise sink object and the session.</param>
        /// <returns>S_OK, if the registration was successful; otherwise, failed.</returns>
        [PreserveSig]
        HRESULT Advise(uint cbEntryID, IntPtr lpEntryID, uint ulEventMask, [In, MarshalAs(UnmanagedType.Interface)] IMAPIAdviseSink pAdviseSink, out uint lpulConnection);
        /// <summary>
        /// Cancels the sending of notifications previously set up with a call to the IMsgStore::Advise method.
        /// </summary>
        /// <param name="ulConnection">The connection number associated with an active notification registration.</param>
        /// <returns>S_OK, if the registration was successfully canceled; otherwise, failed.</returns>
        [PreserveSig]
        HRESULT Unadvise(uint ulConnection);
        /// <summary>
        /// Compares two entry identifiers to determine whether they refer to the same entry in a message store.
        /// </summary>
        /// <param name="cbEntryID1">The byte count in the entry identifier pointed to by the lpEntryID1 parameter.</param>
        /// <param name="lpEntryID1">A pointer to the first entry identifier to be compared.</param>
        /// <param name="cbEntryID2">The byte count in the entry identifier pointed to by the lpEntryID2 parameter.</param>
        /// <param name="lpEntryID2">A pointer to the second entry identifier to be compared.</param>
        /// <param name="ulFlags">Reserved; must be zero.</param>
        /// <param name="lpulResult">true if the two entry identifiers refer to the same object; otherwise, false</param>
        /// <returns>S_OK, if the comparison was successful; otherwise, failed.</returns>
        [PreserveSig]
        HRESULT CompareEntryIDs(uint cbEntryID1, IntPtr lpEntryID1, uint cbEntryID2, IntPtr lpEntryID2, uint ulFlags, out bool lpulResult);
        /// <summary>
        /// Opens a folder or message and returns an interface pointer for further access. 
        /// </summary>
        /// <param name="cbEntryID">The byte count in the entry identifier pointed to by the lpEntryID parameter.</param>
        /// <param name="lpEntryID">A pointer to the entry identifier of the object to open, or NULL. If lpEntryID is set to NULL, OpenEntry opens the root folder for the message store.</param>
        /// <param name="lpInterface">A pointer to the interface identifier (IID) that represents the interface to be used to access the opened object. </param>
        /// <param name="ulFlags">A bitmask of flags that controls how the object is opened.</param>
        /// <param name="lpulObjType">A pointer to the type of the opened object.</param>
        /// <param name="lppUnk">A pointer to a pointer to the opened object.</param>
        /// <returns>S_OK, if the call succeeded and has returned the expected value or values; otherwise, failed</returns>
        [PreserveSig]
        HRESULT OpenEntry(uint cbEntryID, IntPtr lpEntryID, IntPtr lpInterface, uint ulFlags, out uint lpulObjType, out IntPtr lppUnk);
        /// <summary>
        /// Establishes a folder as the destination for incoming messages of a particular message class.
        /// </summary>
        /// <param name="lpszMessageClass">A message class is associated with the new receive folder</param>
        /// <param name="ulFlags">A bitmask of flags that controls the type of the text in the passed-in strings.</param>
        /// <param name="cbEntryID">The byte count in the entry identifier pointed to by the lpEntryID parameter.</param>
        /// <param name="lpEntryID">pointer to the entry identifier of the folder to establish as the receive folder.If the lpEntryID parameter is set to NULL, SetReceiveFolder replaces the current receive folder with the message store's default.</param>
        /// <returns>S_OK, if a receive folder was successfully established; otherwise, failed</returns>
        [PreserveSig]
        HRESULT SetReceiveFolder(string lpszMessageClass, uint ulFlags, uint cbEntryID, IntPtr lpEntryID);
        /// <summary>
        /// Obtains the folder that was established as the destination for incoming messages of a specified message class or as the default receive folder for the message store.
        /// </summary>
        /// <param name="lpszMessageClass">A message class is associated with a receive folder</param>
        /// <param name="ulFlags">A bitmask of flags that controls the type of the passed-in and returned strings. </param>
        /// <param name="cbEntryID">A pointer to the byte count in the entry identifier pointed to by the lppEntryID parameter.</param>
        /// <param name="lppEntryID">A pointer to a pointer to the entry identifier for the requested receive folder.</param>
        /// <param name="lppszExplicitClass">A pointer to a pointer to the message class that explicitly sets as its receive folder the folder pointed to by lppEntryID.</param>
        /// <returns>S_OK, if the receive folder was successfully returned; otherwise, failed</returns>
        [PreserveSig]
        HRESULT GetReceiveFolder([MarshalAs(UnmanagedType.LPWStr)]string lpszMessageClass, uint ulFlags, out uint cbEntryID, out IntPtr lppEntryID, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lppszExplicitClass);
        /// <summary>
        /// Attempts to remove a message from the outgoing queue.
        /// </summary>
        /// <param name="cbEntryID">The byte count in the entry identifier pointed to by the lpEntryID parameter.</param>
        /// <param name="lpEntryID">A pointer to the entry identifier of the message to remove from the outgoing queue. </param>
        /// <param name="ulFlags">Reserved; must be zero.</param>
        /// <returns>S_OK, if the message was successfully removed from the outgoing queue; otherwise, failed</returns>
        [PreserveSig]
        HRESULT AbortSubmit(uint cbEntryID, IntPtr lpEntryID, uint ulFlags);
    }
}
