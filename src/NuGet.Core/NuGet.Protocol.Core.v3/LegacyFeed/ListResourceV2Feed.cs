// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Protocol.Core.Types;

namespace NuGet.Protocol
{
    public class ListResourceV2Feed : ListResource
    {
        private readonly IV2FeedParser _feedParser;

        public ListResourceV2Feed(IV2FeedParser feedParser)
        {
            _feedParser = feedParser;
        }

        public override Task<IEnumerable<IPackageSearchMetadata>> ListAsync(
            string searchTime,
            bool prerelease,
            bool allVersions,
            bool includeDelisted,
            CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
