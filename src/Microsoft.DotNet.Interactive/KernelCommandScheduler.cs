// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.DotNet.Interactive.Commands;

namespace Microsoft.DotNet.Interactive;

public class KernelCommandScheduler : KernelScheduler<KernelCommand, KernelCommandResult>
{
    protected override bool ShouldRunPreemptively(
        KernelCommand current,
        KernelCommand incoming)
    {
        if (current is null)
        {
            return false;
        }

        if (incoming.Parent == current)
        {
            return true;
        }

        if (incoming.IsSelfOrDescendantOf(current))
        {
            return true;
        }

        if (incoming.IsSiblingOf(current))
        {
            return true;
        }

        return incoming.RoutingSlip.StartsWith(current.RoutingSlip);
    }
}