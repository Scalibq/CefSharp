// Copyright © 2017 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

namespace CefSharp.Example.RequestEventHandler
{
    public class OnRenderProcessTerminatedEventArgs : BaseRequestEventArgs
    {
        public OnRenderProcessTerminatedEventArgs(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status, int errorCode, string errorString)
            : base(chromiumWebBrowser, browser)
        {
            Status = status;
            ErrorCode = errorCode;
            ErrorString = errorString;
        }

        public CefTerminationStatus Status { get; }
        public int ErrorCode { get; }
        public string ErrorString { get; }
    }
}
