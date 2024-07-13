﻿// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Security;
using Windows.Win32.System.JobObjects;

namespace ChromaControl.SDK.OpenRGB.Internal;

internal sealed class Job : IDisposable
{
    private readonly SafeFileHandle _jobHandle;

    public Job()
    {
        _jobHandle = CreateJob();

        SetJobInfo(_jobHandle);
    }

    private static unsafe SafeFileHandle CreateJob()
    {
        var securityAttributes = new SECURITY_ATTRIBUTES
        {
            bInheritHandle = false,
            lpSecurityDescriptor = IntPtr.Zero.ToPointer(),
            nLength = (uint)Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES))
        };

#pragma warning disable CA1416 // Validate platform compatibility
        var jobHandle = PInvoke.CreateJobObject(securityAttributes, "ChromaControl.SDK.OpenRGB");
#pragma warning restore CA1416 // Validate platform compatibility

        if (jobHandle.IsInvalid)
        {
            jobHandle.Dispose();

            var lastError = Marshal.GetLastWin32Error();
            throw new Win32Exception(lastError);
        }

        return jobHandle;
    }

    private static unsafe void SetJobInfo(SafeFileHandle jobHandle)
    {
        var info = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION
        {
            BasicLimitInformation = new()
            {
                LimitFlags = JOB_OBJECT_LIMIT.JOB_OBJECT_LIMIT_DIE_ON_UNHANDLED_EXCEPTION
                           | JOB_OBJECT_LIMIT.JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE
            }
        };

        var infoSize = (uint)Marshal.SizeOf<JOBOBJECT_EXTENDED_LIMIT_INFORMATION>();

#pragma warning disable CA1416 // Validate platform compatibility
        var setInfoResult = PInvoke.SetInformationJobObject(jobHandle, JOBOBJECTINFOCLASS.JobObjectExtendedLimitInformation, &info, infoSize);
#pragma warning restore CA1416 // Validate platform compatibility

        if (!setInfoResult)
        {
            var lastError = Marshal.GetLastWin32Error();
            throw new Win32Exception(lastError);
        }
    }

    public void AssignProcess(Process process)
    {
#pragma warning disable CA1416 // Validate platform compatibility
        var assignProcessResult = PInvoke.AssignProcessToJobObject(_jobHandle, process.SafeHandle);
#pragma warning restore CA1416 // Validate platform compatibility

        if (!assignProcessResult)
        {
            var lastError = Marshal.GetLastWin32Error();
            throw new Win32Exception(lastError);
        }
    }

    public void Dispose()
    {
        _jobHandle?.Dispose();

        GC.SuppressFinalize(this);
    }
}
