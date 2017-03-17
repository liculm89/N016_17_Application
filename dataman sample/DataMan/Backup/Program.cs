/*
 * Copyright (c) 2009 by Cognex Corporation, Natick, MA USA
 * All rights reserved. This material contains unpublished,
 * copyrighted work, which includes confidential and proprietary
 * information of Cognex.
 */

/***
 * DataMan SDK Sample program.
 * 
 * Revision: $Revision: 1.1 $
 *
 ***/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Cognex.DataMan.SDK;

namespace Cognex.DataMan.SDK.Sample
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
#if !WindowsCE
        [STAThread]
#endif
        static void Main()
        {
#if !WindowsCE
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#endif
            Application.Run(new MainForm());
        }
    }
}
