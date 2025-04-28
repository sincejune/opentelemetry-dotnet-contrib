// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

namespace OpenTelemetry.Instrumentation.SqlClient.Implementation;

using System.Collections.Generic;
using System.Web;

internal sealed class SqlCommenter
{
    public static string EncodeParams(Dictionary<string, string> parameters)
    {
        List<string> encodedParams = new List<string>();

        foreach (var kvp in parameters)
        {
            string encodedKey = HttpUtility.UrlEncode(kvp.Key);
            string encodedValue = HttpUtility.UrlEncode(kvp.Value);
            encodedParams.Add($"{encodedKey}={encodedValue}");
        }

        return string.Join(",", encodedParams);
    }

    public static string CreateComment(string encodedParams)
    {
        if (string.IsNullOrEmpty(encodedParams))
        {
            return ""; // Or "/* */"
        }

        return $"/*{encodedParams}*/";
    }
}
