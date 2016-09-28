// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;

namespace NuGet.PackageManagement.VisualStudio
{
    public interface IProjectSystemProviderMetadata
    {
        Type ProjectType { get; }

        [DefaultValue(Int32.MaxValue)]
        int Order { get; }
    }
}
