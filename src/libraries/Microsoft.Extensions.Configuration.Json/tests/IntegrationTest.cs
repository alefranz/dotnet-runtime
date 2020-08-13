// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.Extensions.Configuration.Test;
using Xunit;

namespace Microsoft.Extensions.Configuration.Json.Test
{
    public class IntegrationTest
    {
        [Fact]
        public void EmptyObject_AddsEmptyString()
        {
            var json = @"{
                ""a"": ""b"",
                ""c"": {
                    ""d"": ""e""
                },
                ""f"": """",
                ""g"": null,
                ""h"": {},
                ""i"": {
                    ""k"": {}
                } 
            }";

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonStream(TestStreamHelpers.StringToStream(json));
            var configuration = configurationBuilder.Build();

            var firstLevelChildren = configuration.GetChildren();

            Assert.Collection(firstLevelChildren, new Action<IConfigurationSection>[] {
                x => Assert.Equal("b", x.Value)
            });
        }
    }
}
