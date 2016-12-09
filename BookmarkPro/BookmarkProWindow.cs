//------------------------------------------------------------------------------
// <copyright file="BookmarkProWindow.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace BookmarkPro
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using EnvDTE;
    using EnvDTE80;
    using System.IO.Packaging;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("68209b32-6baa-4b9a-8dfb-5e0c239d3aca")]
    public class BookmarkProWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkProWindow"/> class.
        /// </summary>
        public BookmarkProWindow() : base(null)
        {
            //IVsSolutionLoadEvents vsSolutionLdEvents;
            SVsSolution s;

            this.Caption = "BookmarkProWindow";

            //TODO: SET UP EVENT LISTENER FOR SOLUTION AND PROJECT(S) LOADING AND GET IDS; https://msdn.microsoft.com/en-us/library/ee462384.aspx
            //TODO: SET UP EVENT LISTENER FOR SOLUTION AND PROJECT(S) UNLOADING/CLOSING, AND GET IDS

            //CHECK IF A SOLUTION AND PROJECT(S) ARE ALREADY LOADED AND GET IDS


            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new BookmarkProWindowControl();
            TestSolution();
        }
        public void TestSolution()
        {
            //DTE dte = (DTE)GetService(typeof(DTE));
            var _dte2 = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE)) as DTE2;
            var soln = _dte2.Solution;

            //the directory that the solution projects are saved in.
            var path = soln.FullName.Remove(soln.FullName.LastIndexOf('\\'));

            var projects = soln.Projects;

        }

        //TODO: LOAD SOLUTION BOOKMARKS METHOD

        //TODO: UNLOAD SOLUTION BOOKMARKS METHOD

        //TODO: CLEAR CONTROL DATA
    }
}
