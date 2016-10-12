// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace NuGet.Protocol.Core.Types
{
    /// <summary>
    /// The type of filter to apply to the search.
    /// </summary>
    /// <remarks>
    /// 
    /// Suppose the very latest version is prerelease.
    /// 
    ///   Version     | Prerelease | IsLatestVersion | IsAbsoluteLatestVersion
    ///   ------------|------------|-----------------|------------------------
    ///   8.0.1-beta1 | true       | false           | false
    ///   8.0.3       | false      | false           | false
    ///   9.0.1       | false      | true            | false
    ///   9.0.2-beta1 | true       | false           | true
    /// 
    /// Suppose the very latest version is not prerelease. Notice the latest version is also the absolute latest
    /// version. In other words, a prerelease package cannot be a latest version but a non-prerelease package can be
    /// both the latest version and the absolute latest version.
    /// 
    ///   Version     | Prerelease | IsLatestVersion | IsAbsoluteLatestVersion
    ///   ------------|------------|-----------------|------------------------
    ///   8.0.1-beta1 | true       | false           | false
    ///   8.0.3       | false      | false           | false
    ///   9.0.1       | false      | true            | true
    /// 
    /// Suppose there are only prerelease versions. Notice there are no package that has IsLatestVersion as true.
    /// 
    ///   Version     | Prerelease | IsLatestVersion | IsAbsoluteLatestVersion
    ///   ------------|------------|-----------------|------------------------
    ///   8.0.1-beta1 | true       | false           | false
    ///   9.0.2-beta1 | true       | false           | true
    /// 
    /// </remarks>
    public enum SearchFilterType
    {
        /// <summary>
        /// Only select the latest stable version of a package per package ID. Given the server supports
        /// <see cref="IsAbsoluteLatestVersion"/>, a package that is <see cref="IsLatestVersion"/> should never be
        /// prerelease. Also, it does not make sense to look for a <see cref="IsLatestVersion"/> package when also
        /// including prerelease.
        /// </summary>
        /// <remarks>
        /// Suppose the available versions are:
        /// 
        /// Version     | IsLatestVersion | Prerelease
        /// ------------|-----------------|-----------
        /// 8.0.3       | false           | false
        /// 9.0.1       | true            | false
        /// 9.0.2-beta1 | false           | true
        /// </remarks>
        IsLatestVersion = 0,

        /// <summary>
        /// Only select the absolute latest version of a package per package ID. It does not make sense to look for a
        /// <see cref="IsAbsoluteLatestVersion"/> when excluding prerelease.
        /// </summary>
        /// <remarks>
        /// Suppose the available versions are:
        /// 
        /// Version     | IsAbsoluteLatestVersion | Prerelease
        /// ------------|-------------------------|-----------
        /// 8.0.3       | false                   | false
        /// 9.0.1       | false                   | false
        /// 9.0.2-beta1 | true                    | true
        /// </remarks>
        IsAbsoluteLatestVersion = 1
    }
}
