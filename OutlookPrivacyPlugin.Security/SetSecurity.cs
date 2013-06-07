//-----------------------------------------------------------------------
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;

namespace CustomActions
{
    [RunInstaller(true)]
    [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public sealed partial class SetSecurity : Installer
    {
        public SetSecurity()
        {
            InitializeComponent();
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            // Call the base implementation.
            base.Install(stateSaver);

            string allUsersString = this.Context.Parameters["allUsers"];
            string solutionCodeGroupName = this.Context.Parameters["solutionCodeGroupName"];
            string solutionCodeGroupDescription = this.Context.Parameters["solutionCodeGroupDescription"];
            string targetDir = this.Context.Parameters["targetDir"];
            string assemblyName = this.Context.Parameters["assemblyName"];
            string assemblyCodeGroupName = this.Context.Parameters["assemblyCodeGroupName"];
            string assemblyCodeGroupDescription = this.Context.Parameters["assemblyCodeGroupDescription"];

            // Note that a code group with solutionCodeGroupName name is created in the 
            // Install method and removed in the Rollback and Uninstall methods.
            // The solutionCodeGroupName must be a unique name to ensure that the 
            // correct code group is removed during Rollback and Uninstall.

            if (String.IsNullOrEmpty(solutionCodeGroupName))
                throw new InstallException("Cannot set the security policy. The specified solution code group name is not valid.");
            if (String.IsNullOrEmpty(solutionCodeGroupDescription))
                throw new InstallException("Cannot set the security policy. The specified solution code group description is not valid.");
            if (String.IsNullOrEmpty(targetDir))
                throw new InstallException("Cannot set the security policy. The specified target directory is not valid.");
            if (String.IsNullOrEmpty(assemblyName))
                throw new InstallException("Cannot set the security policy. The specified assembly name is not valid.");
            if (String.IsNullOrEmpty(assemblyCodeGroupName))
                throw new InstallException("Cannot set the security policy. The specified assembly code group name is not valid.");
            if (String.IsNullOrEmpty(assemblyCodeGroupDescription))
                throw new InstallException("Cannot set the security policy. The specified assembly code group description is not valid.");
            if (stateSaver == null)
                throw new ArgumentNullException("stateSaver");

            try
            {
                bool allUsers = String.Equals(allUsersString, "1");
                string assemblyPath = Path.Combine(targetDir, assemblyName);

                // Note that Install method may be invoked during Repair mode and the code group 
                // may already exist.
                // To prevent adding of another code group, remove the code group if it exists.
                try
                {
                    // The solutionCodeGroupName must be a unique name; otherwise, the method might delete wrong code group.
                    CaspolSecurityPolicyCreator.RemoveSecurityPolicy(allUsers, solutionCodeGroupName);
                }
                catch {}

                CaspolSecurityPolicyCreator.AddSecurityPolicy(
                    allUsers,
                    solutionCodeGroupName,
                    solutionCodeGroupDescription,
                    assemblyPath,
                    assemblyCodeGroupName,
                    assemblyCodeGroupDescription);
                stateSaver.Add("allUsers", allUsers);

            }
            catch (Exception ex)
            {
                throw new InstallException("Cannot set the security policy.", ex);
            }
        }

        public override void Rollback(System.Collections.IDictionary savedState)
        {
            // Call the base implementation.
            base.Rollback(savedState);

            // Check whether the "allUsers" property is saved.
            // If it is not set, the Install method did not set the security policy.
            if ((savedState == null) || (savedState["allUsers"] == null))
                return;

            // The solutionCodeGroupName must be a unique name; otherwise, the method might delete wrong code group.
            string solutionCodeGroupName = this.Context.Parameters["solutionCodeGroupName"];
            if (String.IsNullOrEmpty(solutionCodeGroupName))
                throw new InstallException("Cannot remove the security policy. The specified solution code group name is not valid.");

            try
            {
                bool allUsers = (bool) savedState["allUsers"];
                CaspolSecurityPolicyCreator.RemoveSecurityPolicy(allUsers, solutionCodeGroupName);
            }
            catch (Exception ex)
            {
                throw new InstallException("Cannot remove the security policy.", ex);
            }
        }


        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            // Call the base implementation.
            base.Uninstall(savedState);

            // Check whether the "allUsers" property is saved.
            // If it is not set, the Install method did not set the security policy.
            if ((savedState == null) || (savedState["allUsers"] == null))
                return;

            // The solutionCodeGroupName must be a unique name; otherwise, the method might delete wrong code group.
            string solutionCodeGroupName = this.Context.Parameters["solutionCodeGroupName"];
            if (String.IsNullOrEmpty(solutionCodeGroupName))
                throw new InstallException("Cannot remove the security policy. The specified solution code group name is not valid.");

            try
            {
                bool allUsers = (bool)savedState["allUsers"];
                CaspolSecurityPolicyCreator.RemoveSecurityPolicy(allUsers, solutionCodeGroupName);
            }
            catch (Exception ex)
            {
                // Note that throwing an exception might stop the uninstall process.
                // To inform the user and stop the uninstall process, throw an exception.
                // To continue the uninstall, do not throw the exception.
                throw new InstallException("Cannot remove the security policy.", ex);
            }
        }
    }
}