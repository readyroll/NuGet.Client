// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace NuGet.Protocol.Core.Types
{
    public enum SearchFilterType
    {
        /// <summary>
        /// Only select the latest stable version of a package per package ID. Suppose the available versions are:
        /// 
        /// Version     | IsLatestVersion | Prerelease
        /// ------------|-----------------|-----------
        /// 8.0.3       | false           | false
        /// 9.0.1       | true            | false
        /// 9.0.2-beta1 | false           | true
        /// 
        /// Given the server supports <see cref="IsAbsoluteLatestVersion"/>, a package that is <see cref="IsLatestVersion"/>
        /// should never be prerelease. Also, it does not make sense to look for a <see cref="IsLatestVersion"/>
        /// package when also including prerelease.
        /// </summary>
        IsLatestVersion = 0,

        /// <summary>
        /// Only select the absolute latest version of a package per package ID. Suppose the available versions are:
        /// 
        /// Version     | IsAbsoluteLatestVersion | Prerelease
        /// ------------|-------------------------|-----------
        /// 8.0.3       | false                   | false
        /// 9.0.1       | false                   | false
        /// 9.0.2-beta1 | true                    | true
        /// 
        /// It does not make sense to look for a <see cref="IsAbsoluteLatestVersion"/> when excluding prerelease.
        /// </summary>
        IsAbsoluteLatestVersion = 1
    }
}
