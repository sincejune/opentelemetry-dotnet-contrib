// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using OpenTelemetry.Instrumentation.SqlClient.Implementation;

namespace OpenTelemetry.Instrumentation.SqlClient.Tests;

using System.Collections.Generic;
using Xunit;

public class SqlCommenterTest
{
    [Theory]
    [InlineData("", "")]
    [InlineData("key1=value1", "/*key1=value1*/")]
    [InlineData("key1=value1,key2=value2", "/*key1=value1,key2=value2*/")]
    public void CreateComment_WithEncodedParams_ReturnsCommentString(string encodedParams, string expectedComment)
    {
        string comment = SqlCommenter.CreateComment(encodedParams);
        Assert.Equal(expectedComment, comment);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("key1", "value1", "key2", "value2", "key1=value1,key2=value2")]
    [InlineData("key with space", "value with space", "key+with+plus", "value+with+plus",
        "key+with+space=value+with+space,key%2bwith%2bplus=value%2bwith%2bplus")]
    public void EncodeParams_ReturnsEncodedString(string key1, string value1, string? key2 = null, string? value2 = null,
        string? expectedEncoded = null)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(key1)) parameters.Add(key1, value1);
        if (!string.IsNullOrEmpty(key2)) parameters.Add(key2, value2);

        string encoded = SqlCommenter.EncodeParams(parameters);
        Assert.Equal(expectedEncoded ?? "", encoded); // Handle null expectedEncoded
    }

    [Fact]
    public void CreateComment_WithSpecialCharacters_ReturnsCorrectCommentString()
    {
        Dictionary<string, string> params4 = new Dictionary<string, string> { { "key1", "value with /* comment */" } };
        string encoded = SqlCommenter.EncodeParams(params4);
        string comment = SqlCommenter.CreateComment(encoded);
        Assert.Equal("/*key1=value+with+%2f*+comment+*%2f*/", comment);
    }
}
