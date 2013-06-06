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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace CustomActions
{
    [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    internal sealed class CaspolSecurityPolicyCreator
    {
        private CaspolSecurityPolicyCreator() {}

        internal static void AddSecurityPolicy(
            bool   machinePolicyLevel,
            string solutionCodeGroupName,
            string solutionCodeGroupDescription,
            string assemblyPath,
            string assemblyCodeGroupName,
            string assemblyCodeGroupDescription)
        {
            string frameworkFolder = GetFrameworkFolder();

            string solutionInstallationLocation = Path.GetDirectoryName(assemblyPath);
            string solutionInstallationUrl = Path.Combine(solutionInstallationLocation, "*");

            string policyLevel;
            string parentCodeGroup;
            if (machinePolicyLevel)
            {
                policyLevel = "-m"; // Use Machine-level policy.
                parentCodeGroup = "My_Computer_Zone"; // Use My_Computer_Zone for assemblies installed on the computer.
            }
            else
            {
                policyLevel = "-u"; // Use User-level policy.
                parentCodeGroup = "All_Code";
            }

            // Add the solution code group. Grant no permission at this level.
            string arguments = policyLevel + " -q -ag " + parentCodeGroup + " -url \"" + solutionInstallationUrl + "\" Nothing -n \"" + solutionCodeGroupName + "\" -d \"" + solutionCodeGroupDescription + "\"";
            try
            {
                RunCaspolCommand(frameworkFolder, arguments);
            }
            catch (Exception ex)
            {
                string error = String.Format("Cannot create the security code group '{0}'.", solutionCodeGroupName);
                throw new Exception(error, ex);
            }

            // Add the assembly code group. Grant FullTrust permissions to the main assembly.
            try
            {
                // Use the assembly strong name as the membership condition.
                // Ensure that the assembly is strong-named to give it full trust.
                AssemblyName assemblyName = Assembly.LoadFile(assemblyPath).GetName();
                arguments = policyLevel + " -q -ag \"" + solutionCodeGroupName + "\" -strong -file \"" + assemblyPath + "\" \"" + assemblyName.Name + "\" \"" + assemblyName.Version.ToString(4) + "\" FullTrust -n \"" + assemblyCodeGroupName + "\" -d \"" + assemblyCodeGroupDescription + "\"";

                RunCaspolCommand(frameworkFolder, arguments);
            }
            catch (Exception ex)
            {
                try
                {
                    // Clean the solutionCodeGroupName.
                    RemoveSecurityPolicy(machinePolicyLevel, solutionCodeGroupName);
                }
                catch {}

                string error = String.Format("Cannot create the security code group '{0}'.", assemblyCodeGroupName);
                throw new Exception(error, ex);
            }
        }

        internal static void RemoveSecurityPolicy(
            bool   machinePolicyLevel,
            string solutionCodeGroupName)
        {
            string frameworkFolder = GetFrameworkFolder();
            string policyLevel;
            if (machinePolicyLevel)
                policyLevel = "-m"; // Use Machine-level policy.
            else
                policyLevel = "-u"; // Use User-level policy.

            string arguments = policyLevel + " -q -rg \"" + solutionCodeGroupName + "\"";
            RunCaspolCommand(frameworkFolder, arguments);
        }

        private static string GetFrameworkFolder()
        {
            // Get the targeted Framework folder.
            Version version = new Version(2, 0, 50727);
            return GetRuntimeInstallationDirectory(version, true);
        }

        private static void RunCaspolCommand(string frameworkFolder, string arguments)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(Path.Combine(frameworkFolder, "caspol.exe"));
            processStartInfo.CreateNoWindow = true;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.WorkingDirectory = frameworkFolder;
            processStartInfo.Arguments = arguments;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.UseShellExecute = false;

            Process process = Process.Start(processStartInfo);
            string caspolOutputMessage = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            int exitCode = 0;
            if (process.HasExited)
                exitCode = process.ExitCode;

            if (exitCode != 0)
            {
                string message = null;
                if (!String.IsNullOrEmpty(caspolOutputMessage))
                {
                    String[] outputMessageLines = caspolOutputMessage.Split('\n');
                    for (int i = 2; i < outputMessageLines.Length; i++)
                    {
                        string line = outputMessageLines[i].Trim();
                        if (!String.IsNullOrEmpty(line))
                        {
                            message = line;
                            break;
                        }
                    }
                }
                if (String.IsNullOrEmpty(message))
                    message = "Cannot run the Code Access Security Policy tool (caspol.exe).";

                throw new ApplicationException(message);
            }
        }

        [Flags]
        private enum RuntimeInfo : uint
        {
            UpgradeVersion = 0x1,           // RUNTIME_INFO_UPGRADE_VERSION
            RequestIA64 = 0x2,              // RUNTIME_INFO_REQUEST_IA64
            RequestAmd64 = 0x4,             // RUNTIME_INFO_REQUEST_AMD64
            RequestX86 = 0x8,               // RUNTIME_INFO_REQUEST_X86
            DoNotReturnDirectory = 0x10,    // RUNTIME_INFO_DONT_RETURN_DIRECTORY
            DoNotReturnVersion = 0x20,      // RUNTIME_INFO_DONT_RETURN_VERSION
            DoNotShowErrorDialog = 0x40     // RUNTIME_INFO_DONT_SHOW_ERROR_DIALOG
        }

        [DllImport("mscoree.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = true, SetLastError = false)]
        private static extern int /* [HRESULT] */ GetRequestedRuntimeInfo(
            string /* [LPCWSTR] */ pExe,
            string /* [LPCWSTR] */ pwszVersion,
            string /* [LPCWSTR] */ pConfigurationFile,
            uint /* [DWORD] */ startupFlags,
            RuntimeInfo /* [DWORD] */ runtimeInfoFlags,
            StringBuilder /* [LPWSTR] */ pDirectory,
            uint /* [DWORD] */ dwDirectory,
            out uint /* [DWORD *] */ dwDirectoryLength,
            StringBuilder /* [LPWSTR] */ pVersion,
            uint /* [DWORD] */ cchBuffer,
            out uint /* [DWORD *] */ dwLength
            );

        /// <summary>
        /// Returns the installation directory of the specified .NET runtime.
        /// </summary>
        /// <param name="version">
        /// The version of the runtime.
        /// </param>
        /// <param name="upgradeVersion">
        /// True to return the installation directory of the nearest compatible runtime version, or false for an exact match.
        /// </param>
        /// <returns>
        /// The .NET runtime installation directory.
        /// </returns>
        private static string GetRuntimeInstallationDirectory(Version version, bool upgradeVersion)
        {
            string versionString = "v" + version.ToString(3);
            RuntimeInfo runtimeInfo = RuntimeInfo.DoNotShowErrorDialog;
            if (upgradeVersion)
                runtimeInfo |= RuntimeInfo.UpgradeVersion;

            StringBuilder runtimeDirectory = new StringBuilder(270);
            StringBuilder runtimeVersion = new StringBuilder("v65535.65535.65535".Length);
            uint runtimeDirectoryLength;
            uint runtimeVersionLength;
            int errorCode = GetRequestedRuntimeInfo(null, versionString, null, 0, runtimeInfo, runtimeDirectory, (uint)runtimeDirectory.Capacity, out runtimeDirectoryLength, runtimeVersion, (uint)runtimeVersion.Capacity, out runtimeVersionLength);
            Marshal.ThrowExceptionForHR(errorCode);
            return Path.Combine(runtimeDirectory.ToString(), runtimeVersion.ToString());
        }
    }
}
