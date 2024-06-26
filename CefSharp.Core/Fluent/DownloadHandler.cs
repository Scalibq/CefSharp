// Copyright © 2021 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System.IO;

namespace CefSharp.Fluent
{
    /// <summary>
    /// Called before a download begins in response to a user-initiated action
    /// (e.g. alt + link click or link click that returns a `Content-Disposition:
    /// attachment` response from the server).
    /// </summary>
    /// <param name="chromiumWebBrowser">the ChromiumWebBrowser control</param>
    /// <param name="browser">The browser instance</param>
    /// <param name="url">is the target download URL</param>
    /// <param name="requestMethod">is the target method (GET, POST, etc)</param>
    /// <returns>Return true to proceed with the download or false to cancel the download.</returns>
    public delegate bool CanDownloadDelegate(IWebBrowser chromiumWebBrowser, IBrowser browser, string url, string requestMethod);

    /// <summary>
    /// Called before a download begins.
    /// </summary>
    /// <param name="chromiumWebBrowser">the ChromiumWebBrowser control</param>
    /// <param name="browser">The browser instance</param>
    /// <param name="downloadItem">Represents the file being downloaded.</param>
    /// <param name="callback">Callback interface used to asynchronously continue a download.</param>
    /// <returns>Return true and execute <paramref name="callback"/> either
    /// asynchronously or in this method to continue or cancel the download.
    /// Return false to proceed with default handling (cancel with Alloy style,
    /// download shelf with Chrome style).</returns>
    public delegate bool OnBeforeDownloadDelegate(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback);

    /// <summary>
    /// Called when a download's status or progress information has been updated. This may be called multiple times before and after <see cref="IDownloadHandler.OnBeforeDownload"/>.
    /// </summary>
    /// <param name="chromiumWebBrowser">the ChromiumWebBrowser control</param>
    /// <param name="browser">The browser instance</param>
    /// <param name="downloadItem">Represents the file being downloaded.</param>
    /// <param name="callback">The callback used to Cancel/Pause/Resume the process</param>
    public delegate void OnDownloadUpdatedDelegate(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback);

    /// <summary>
    /// A <see cref="IDownloadHandler"/> implementation used by <see cref="DownloadHandlerBuilder"/>
    /// to provide a fluent means of creating a <see cref="IDownloadHandler"/>.
    /// </summary>
    public class DownloadHandler : Handler.DownloadHandler
    {
        private CanDownloadDelegate canDownload;
        private OnBeforeDownloadDelegate onBeforeDownload;
        private OnDownloadUpdatedDelegate onDownloadUpdated;

        /// <summary>
        /// Create a new DownloadHandler Builder
        /// </summary>
        /// <returns>Fluent DownloadHandler Builder</returns>
        public static DownloadHandlerBuilder Create()
        {
            return new DownloadHandlerBuilder();
        }

        /// <summary>
        /// Creates a new <see cref="IDownloadHandler"/> instances
        /// where all downloads are automatically downloaded to the specified folder.
        /// No dialog is dispolayed to the user.
        /// </summary>
        /// <param name="folder">folder where files are download.</param>
        /// <param name="downloadUpdated">optional delegate for download updates, track progress, completion etc.</param>
        /// <returns><see cref="IDownloadHandler"/> instance.</returns>
        public static IDownloadHandler UseFolder(string folder, OnDownloadUpdatedDelegate downloadUpdated = null)
        {
            return Create()
                .OnBeforeDownload((chromiumWebBrowser, browser, item, callback) =>
                {
                    using (callback)
                    {
                        var path = Path.Combine(folder, item.SuggestedFileName);
                        
                        callback.Continue(path, showDialog: false);
                    }

                    return true;
                })
                .OnDownloadUpdated(downloadUpdated)
                .Build();
        }

        /// <summary>
        /// Creates a new <see cref="IDownloadHandler"/> instances
        /// where a default "Save As" dialog is displayed to the user.
        /// </summary>
        /// <param name="downloadUpdated">optional delegate for download updates, track progress, completion etc.</param>
        /// <returns><see cref="IDownloadHandler"/> instance.</returns>
        public static IDownloadHandler AskUser(OnDownloadUpdatedDelegate downloadUpdated = null)
        {
            return Create()
                .OnBeforeDownload((chromiumWebBrowser, browser, item, callback) =>
                {
                    using (callback)
                    {
                        callback.Continue("", showDialog: true);
                    }

                    return true;
                })
                .OnDownloadUpdated(downloadUpdated)
                .Build();
        }

        /// <summary>
        /// Use <see cref="Create"/> to create a new instance of the fluent builder
        /// </summary>
        internal DownloadHandler()
        {

        }

        internal void SetCanDownload(CanDownloadDelegate action)
        {
            canDownload = action;
        }

        internal void SetOnBeforeDownload(OnBeforeDownloadDelegate action)
        {
            onBeforeDownload = action;
        }

        internal void SetOnDownloadUpdated(OnDownloadUpdatedDelegate action)
        {
            onDownloadUpdated = action;
        }

        /// <inheritdoc/>
        protected override bool CanDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, string url, string requestMethod)
        {
            return canDownload?.Invoke(chromiumWebBrowser, browser, url, requestMethod) ?? true;
        }

        /// <inheritdoc/>
        protected override bool OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            return onBeforeDownload?.Invoke(chromiumWebBrowser, browser, downloadItem, callback) ?? false;
        }

        /// <inheritdoc/>
        protected override void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            onDownloadUpdated?.Invoke(chromiumWebBrowser, browser, downloadItem, callback);
        }
    }
}
