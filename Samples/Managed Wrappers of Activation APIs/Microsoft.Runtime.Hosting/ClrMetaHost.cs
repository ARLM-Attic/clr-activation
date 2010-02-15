// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Runtime.Hosting.Interop;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace Microsoft.Runtime.Hosting {

    /// <summary>
    /// Managed abstraction of the functionality provided by ICLRMetaHost.
    /// </summary>
    public static class ClrMetaHost {

        [ThreadStatic]
        static IClrMetaHost _MetaHost;
        static IClrMetaHost MetaHost {
            get {
                if (_MetaHost == null) {
                    _MetaHost = HostingInteropHelper.GetClrMetaHost<IClrMetaHost>();
                }
                return _MetaHost;
            }
        }

        //ordering of initializers matters
        [ThreadStatic]
        static ClrRuntimeInfo _CurrentRuntime;

        /// <summary>
        /// An enumeration of the installed runtimes
        /// </summary>
        public static IEnumerable<ClrRuntimeInfo> InstalledRuntimes {
            get {
                return EnumerateRuntimesFromEnumUnknown(() => MetaHost.EnumerateInstalledRuntimes());
            }
        }

        /// <summary>
        /// Enumerates the runtimes loaded in <paramref name="process"/>
        /// </summary>
        /// <param name="process">The process to inspect</param>
        public static IEnumerable<ClrRuntimeInfo> GetLoadedRuntimes(Process process) {
            return EnumerateRuntimesFromEnumUnknown(() => MetaHost.EnumerateLoadedRuntimes(process.Handle));
        }

        /// <summary>
        /// Enumerates the runtimes loaded in the current process
        /// </summary>
        public static IEnumerable<ClrRuntimeInfo> GetLoadedRuntimesInCurrentProcess() {
            return GetLoadedRuntimes(Process.GetCurrentProcess());
        }

        /// <summary>
        /// Internal helper to enumerate the contents of an IEnumUnknown that contains IClrRuntimeInfo
        /// </summary>
        /// <param name="enumUnknownFunc"></param>
        /// <returns></returns>
        static IEnumerable<ClrRuntimeInfo> EnumerateRuntimesFromEnumUnknown(Func<IEnumUnknown> enumUnknownFunc) {
            //clone and reset the IEnumUnknown to give the right enumerable semantics
            IEnumUnknown enumUnknown = enumUnknownFunc();

            object[] objs = new object[1];
            int fetched;
            while (0 == enumUnknown.Next(1, objs, out fetched)) {
                Debug.Assert(fetched == 1, "fetch == 1");
                yield return new ClrRuntimeInfo((IClrRuntimeInfo)objs[0]);
            }
        }

        /// <summary>
        /// Gets the "built-against" version from an assembly's CLR header.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetRuntimeVersionFromAssembly(string path) {
            var versionBuffer = new BufferData();
            BufferData.DoBufferAction(() => MetaHost.GetVersionFromFile(path, versionBuffer, ref versionBuffer.Length),
                versionBuffer);
            return versionBuffer.ToString();
        }

        /// <summary>
        /// Gets the <see cref="ClrRuntimeInfo"/> corresponding to the current runtime.
        /// That is, the runtime executing currently.
        /// </summary>
        public static ClrRuntimeInfo CurrentRuntime {
            get {
                if (_CurrentRuntime == null) {
                    _CurrentRuntime = GetRuntime(RuntimeEnvironment.GetSystemVersion());
                }
                return _CurrentRuntime;
            }
        }

        internal static IClrRuntimeInfo GetRuntimeInternal(string version) {
            return (IClrRuntimeInfo)MetaHost.GetRuntime(version, typeof(IClrRuntimeInfo).GUID);
        }

        /// <summary>
        /// Gets the <see cref="ClrRuntimeInfo"/> corresponding to a particular version string.
        /// </summary>
        public static ClrRuntimeInfo GetRuntime(string version) {
            return new ClrRuntimeInfo(GetRuntimeInternal(version));
        }

        /// <summary>
        /// Gets the <see cref="ClrRuntimeInfo"/> corresponding to the runtime that is bound to the v2 and prior
        /// "legacy" APIs.
        /// </summary>
        /// <returns></returns>
        public static ClrRuntimeInfo GetLegacyRuntime() {
            var info = (IClrRuntimeInfo)MetaHost.QueryLegacyV2RuntimeBinding(typeof(IClrRuntimeInfo).GUID);
            return info == null ? null : new ClrRuntimeInfo(info);
        }

        /// <summary>
        /// Exits the process with the given exit code
        /// </summary>
        public static void ExitProcess(int exitCode) {
            MetaHost.ExitProcess(exitCode);
        }
    }
}
