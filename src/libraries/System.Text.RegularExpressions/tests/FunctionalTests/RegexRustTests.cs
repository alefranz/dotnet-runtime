﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace System.Text.RegularExpressions.Tests
{
    /// <summary>
    /// These tests were ported from https://github.com/rust-lang/regex/tree/6ff285e37555d4adc52f24e97318681f8a5ecd48/tests
    /// </summary>
    public class RegexRustTests
    {
        public static IEnumerable<object[]> MatchStartAndEndPositions_MemberData()
        {
            foreach (RegexEngine engine in RegexHelpers.AvailableEngines)
            {
                (string Pattern, string Input, IEnumerable<(int, int)> MatchBoundaries)[] cases = MatchStartAndEndPositions_MemberData_Cases().ToArray();
                Regex[] regexes = RegexHelpers.GetRegexes(engine, cases.Select(c => (c.Pattern, (CultureInfo?)null, (RegexOptions?)RegexOptions.None, (TimeSpan?)null)).ToArray());
                for (int i = 0; i < regexes.Length; i++)
                {
                    yield return new object[] { regexes[i], cases[i].Input, cases[i].MatchBoundaries };
                }
            }
        }

        public static IEnumerable<(string Pattern, string Input, IEnumerable<(int, int)>? MatchBoundaries)> MatchStartAndEndPositions_MemberData_Cases()
        {
            yield return (@"^$", "", new[] { (0, 0) });
            yield return (@"^$^$^$", "", new[] { (0, 0) });
            yield return (@"^^^$$$", "", new[] { (0, 0) });
            yield return (@"$^", "", new[] { (0, 0) });
            yield return (@"(?:^$)*", "a\nb\nc", new[] { (0, 0), (1, 1), (2, 2), (3, 3), (4, 4), (5, 5) });
            yield return (@"(?:$^)*", "a\nb\nc", new[] { (0, 0), (1, 1), (2, 2), (3, 3), (4, 4), (5, 5) });
            yield return (@"", "", new[] { (0, 0) });
            yield return (@"", "abc", new[] { (0, 0), (1, 1), (2, 2), (3, 3) });
            yield return (@"()", "abc", new[] { (0, 0), (1, 1), (2, 2), (3, 3) });
            yield return (@"()*", "abc", new[] { (0, 0), (1, 1), (2, 2), (3, 3) });
            yield return (@"()+", "abc", new[] { (0, 0), (1, 1), (2, 2), (3, 3) });
            yield return (@"()?", "abc", new[] { (0, 0), (1, 1), (2, 2), (3, 3) });
            yield return (@"()()", "abc", new[] { (0, 0), (1, 1), (2, 2), (3, 3) });
            yield return (@"()+|z", "abc", new[] { (0, 0), (1, 1), (2, 2), (3, 3) });
            yield return (@"z|()+", "abc", new[] { (0, 0), (1, 1), (2, 2), (3, 3) });
            yield return (@"()+|b", "abc", new[] { (0, 0), (1, 1), (2, 2), (3, 3) });
            yield return (@"b|()+", "abc", new[] { (0, 0), (1, 2), (2, 2), (3, 3) });
            yield return (@"|b", "abc", new[] { (0, 0), (1, 1), (2, 2), (3, 3) });
            yield return (@"b|", "abc", new[] { (0, 0), (1, 2), (2, 2), (3, 3) });
            yield return (@"|z", "abc", new[] { (0, 0), (1, 1), (2, 2), (3, 3) });
            yield return (@"z|", "abc", new[] { (0, 0), (1, 1), (2, 2), (3, 3) });
            yield return (@"|", "abc", new[] { (0, 0), (1, 1), (2, 2), (3, 3) });
            yield return (@"||", "abc", new[] { (0, 0), (1, 1), (2, 2), (3, 3) });
            yield return (@"||z", "abc", new[] { (0, 0), (1, 1), (2, 2), (3, 3) });
            yield return (@"(?:)|b", "abc", new[] { (0, 0), (1, 1), (2, 2), (3, 3) });
            yield return (@"b|(?:)", "abc", new[] { (0, 0), (1, 2), (2, 2), (3, 3) });
            yield return (@"(?:|)", "abc", new[] { (0, 0), (1, 1), (2, 2), (3, 3) });
            yield return (@"(?:|)|z", "abc", new[] { (0, 0), (1, 1), (2, 2), (3, 3) });
            yield return (@"a(?:)|b", "abc", new[] { (0, 1), (1, 2) });
            yield return (@"a$", "a", new[] { (0, 1) });
            yield return (@"(?m)^[a-z]+$", "abc\ndef\nxyz", new[] { (0, 3), (4, 7), (8, 11) });
            yield return (@"(?m)^$", "abc\ndef\nxyz", new ValueTuple<int, int>[] { });
            yield return (@"(?m)^", "abc\ndef\nxyz", new[] { (0, 0), (4, 4), (8, 8) });
            yield return (@"(?m)$", "abc\ndef\nxyz", new[] { (3, 3), (7, 7), (11, 11) });
            yield return (@"(?m)^[a-z]", "abc\ndef\nxyz", new[] { (0, 1), (4, 5), (8, 9) });
            yield return (@"(?m)[a-z]^", "abc\ndef\nxyz", new ValueTuple<int, int>[] { });
            yield return (@"(?m)[a-z]$", "abc\ndef\nxyz", new[] { (2, 3), (6, 7), (10, 11) });
            yield return (@"(?m)$[a-z]", "abc\ndef\nxyz", new ValueTuple<int, int>[] { });
            yield return (@"(?m)^$", "", new[] { (0, 0) });
            yield return (@"(?m)(?:^$)*", "a\nb\nc", new[] { (0, 0), (1, 1), (2, 2), (3, 3), (4, 4), (5, 5) });
            yield return (@"(?m)(?:^|a)+", "a\naaa\n", new[] { (0, 0), (2, 2), (3, 5), (6, 6) });
            yield return (@"(?m)(?:^|a)*", "a\naaa\n", new[] { (0, 0), (1, 1), (2, 2), (3, 5), (5, 5), (6, 6) });
            yield return (@"(?m)(?:^[a-z])+", "abc\ndef\nxyz", new[] { (0, 1), (4, 5), (8, 9) });
            yield return (@"(?m)(?:^[a-z]{3}\n?)+", "abc\ndef\nxyz", new[] { (0, 11) });
            yield return (@"(?m)(?:^[a-z]{3}\n?)*", "abc\ndef\nxyz", new[] { (0, 11), (11, 11) });
            yield return (@"(?m)(?:\n?[a-z]{3}$)+", "abc\ndef\nxyz", new[] { (0, 11) });
            yield return (@"(?m)(?:\n?[a-z]{3}$)*", "abc\ndef\nxyz", new[] { (0, 11), (11, 11) });
            yield return (@"(?m)^*", "\naa\n", new[] { (0, 0), (1, 1), (2, 2), (3, 3), (4, 4) });
            yield return (@"(?m)^+", "\naa\n", new[] { (0, 0), (1, 1), (4, 4) });
            yield return (@"(?m)$*", "\naa\n", new[] { (0, 0), (1, 1), (2, 2), (3, 3), (4, 4) });
            yield return (@"(?m)$+", "\naa\n", new[] { (0, 0), (3, 3), (4, 4) });
            yield return (@"(?m)(?:$\n)+", "\n\naaa\n\n", new[] { (0, 2), (5, 7) });
            yield return (@"(?m)(?:$\n)*", "\n\naaa\n\n", new[] { (0, 2), (2, 2), (3, 3), (4, 4), (5, 7), (7, 7) });
            yield return (@"(?m)(?:$\n^)+", "\n\naaa\n\n", new[] { (0, 2), (5, 7) });
            yield return (@"(?m)(?:^|$)+", "\n\naaa\n\n", new[] { (0, 0), (1, 1), (2, 2), (5, 5), (6, 6), (7, 7) });
            yield return (@"\b", "a b c", new[] { (0, 0), (1, 1), (2, 2), (3, 3), (4, 4), (5, 5) });
            yield return (@"^a|b", "ba", new[] { (0, 1) });
            yield return (@"[0-9][0-9][0-9]000", "153.230000\n", new[] { (4, 10) });
            yield return (@"((?i)foo)|Bar", "foo Foo bar Bar", new[] { (0, 3), (4, 7), (12, 15) });
            yield return (@"()?01", "z?01", new[] { (2, 4) });
            yield return (@"\b", "", new ValueTuple<int, int>[] { });
            yield return (@"\b", "a", new[] { (0, 0), (1, 1) });
            yield return (@"\b", "ab", new[] { (0, 0), (2, 2) });
            yield return (@"^\b", "ab", new[] { (0, 0) });
            yield return (@"\b$", "ab", new[] { (2, 2) });
            yield return (@"^\b$", "ab", new ValueTuple<int, int>[] { });
            yield return (@"\bbar\b", "nobar bar foo bar", new[] { (6, 9), (14, 17) });
            yield return (@"a\b", "faoa x", new[] { (3, 4) });
            yield return (@"\bbar", "bar x", new[] { (0, 3) });
            yield return (@"\bbar", "foo\nbar x", new[] { (4, 7) });
            yield return (@"bar\b", "foobar", new[] { (3, 6) });
            yield return (@"bar\b", "foobar\nxxx", new[] { (3, 6) });
            yield return (@"(foo|bar|[A-Z])\b", "foo", new[] { (0, 3) });
            yield return (@"(foo|bar|[A-Z])\b", "foo\n", new[] { (0, 3) });
            yield return (@"\b(foo|bar|[A-Z])", "foo", new[] { (0, 3) });
            yield return (@"\b(foo|bar|[A-Z])\b", "X", new[] { (0, 1) });
            yield return (@"\b(foo|bar|[A-Z])\b", "XY", new ValueTuple<int, int>[] { });
            yield return (@"\b(foo|bar|[A-Z])\b", "bar", new[] { (0, 3) });
            yield return (@"\b(foo|bar|[A-Z])\b", "foo", new[] { (0, 3) });
            yield return (@"\b(foo|bar|[A-Z])\b", "foo\n", new[] { (0, 3) });
            yield return (@"\b(foo|bar|[A-Z])\b", "ffoo bbar N x", new[] { (10, 11) });
            yield return (@"\b(fo|foo)\b", "fo", new[] { (0, 2) });
            yield return (@"\b(fo|foo)\b", "foo", new[] { (0, 3) });
            yield return (@"\b\b", "", new ValueTuple<int, int>[] { });
            yield return (@"\b\b", "a", new[] { (0, 0), (1, 1) });
            yield return (@"\b$", "", new ValueTuple<int, int>[] { });
            yield return (@"\b$", "x", new[] { (1, 1) });
            yield return (@"\b$", "y x", new[] { (3, 3) });
            yield return (@"\b.$", "x", new[] { (0, 1) });
            yield return (@"^\b(fo|foo)\b", "fo", new[] { (0, 2) });
            yield return (@"^\b(fo|foo)\b", "foo", new[] { (0, 3) });
            yield return (@"^\b$", "", new ValueTuple<int, int>[] { });
            yield return (@"^\b$", "x", new ValueTuple<int, int>[] { });
            yield return (@"^\b.$", "x", new[] { (0, 1) });
            yield return (@"^\b.\b$", "x", new[] { (0, 1) });
            yield return (@"^^^^^\b$$$$$", "", new ValueTuple<int, int>[] { });
            yield return (@"^^^^^\b.$$$$$", "x", new[] { (0, 1) });
            yield return (@"^^^^^\b$$$$$", "x", new ValueTuple<int, int>[] { });
            yield return (@"^^^^^\b\b\b.\b\b\b$$$$$", "x", new[] { (0, 1) });
            yield return (@"\b.+\b", "$$abc$$", new[] { (2, 5) });
            yield return (@"\b", "a b c", new[] { (0, 0), (1, 1), (2, 2), (3, 3), (4, 4), (5, 5) });
            yield return (@"\Bfoo\B", "n foo xfoox that", new[] { (7, 10) });
            yield return (@"a\B", "faoa x", new[] { (1, 2) });
            yield return (@"\Bbar", "bar x", new ValueTuple<int, int>[] { });
            yield return (@"\Bbar", "foo\nbar x", new ValueTuple<int, int>[] { });
            yield return (@"bar\B", "foobar", new ValueTuple<int, int>[] { });
            yield return (@"bar\B", "foobar\nxxx", new ValueTuple<int, int>[] { });
            yield return (@"(foo|bar|[A-Z])\B", "foox", new[] { (0, 3) });
            yield return (@"(foo|bar|[A-Z])\B", "foo\n", new ValueTuple<int, int>[] { });
            yield return (@"\B", "", new[] { (0, 0) });
            yield return (@"\B", "x", new ValueTuple<int, int>[] { });
            yield return (@"\B(foo|bar|[A-Z])", "foo", new ValueTuple<int, int>[] { });
            yield return (@"\B(foo|bar|[A-Z])\B", "xXy", new[] { (1, 2) });
            yield return (@"\B(foo|bar|[A-Z])\B", "XY", new ValueTuple<int, int>[] { });
            yield return (@"\B(foo|bar|[A-Z])\B", "XYZ", new[] { (1, 2) });
            yield return (@"\B(foo|bar|[A-Z])\B", "abara", new[] { (1, 4) });
            yield return (@"\B(foo|bar|[A-Z])\B", "xfoo_", new[] { (1, 4) });
            yield return (@"\B(foo|bar|[A-Z])\B", "xfoo\n", new ValueTuple<int, int>[] { });
            yield return (@"\B(foo|bar|[A-Z])\B", "foo bar vNX", new[] { (9, 10) });
            yield return (@"\B(fo|foo)\B", "xfoo", new[] { (1, 3) });
            yield return (@"\B(foo|fo)\B", "xfooo", new[] { (1, 4) });
            yield return (@"\B\B", "", new[] { (0, 0) });
            yield return (@"\B\B", "x", new ValueTuple<int, int>[] { });
            yield return (@"\B$", "", new[] { (0, 0) });
            yield return (@"\B$", "x", new ValueTuple<int, int>[] { });
            yield return (@"\B$", "y x", new ValueTuple<int, int>[] { });
            yield return (@"\B.$", "x", new ValueTuple<int, int>[] { });
            yield return (@"^\B(fo|foo)\B", "fo", new ValueTuple<int, int>[] { });
            yield return (@"^\B(fo|foo)\B", "foo", new ValueTuple<int, int>[] { });
            yield return (@"^\B", "", new[] { (0, 0) });
            yield return (@"^\B", "x", new ValueTuple<int, int>[] { });
            yield return (@"^\B\B", "", new[] { (0, 0) });
            yield return (@"^\B\B", "x", new ValueTuple<int, int>[] { });
            yield return (@"^\B$", "", new[] { (0, 0) });
            yield return (@"^\B$", "x", new ValueTuple<int, int>[] { });
            yield return (@"^\B.$", "x", new ValueTuple<int, int>[] { });
            yield return (@"^\B.\B$", "x", new ValueTuple<int, int>[] { });
            yield return (@"^^^^^\B$$$$$", "", new[] { (0, 0) });
            yield return (@"^^^^^\B.$$$$$", "x", new ValueTuple<int, int>[] { });
            yield return (@"^^^^^\B$$$$$", "x", new ValueTuple<int, int>[] { });
            yield return (@"\bx\b", "\u00ABx", new[] { (1, 2) });
            yield return (@"\bx\b", "x\u00BB", new[] { (0, 1) });
            yield return (@"\bx\b", "\u00E1x\u00DF", new ValueTuple<int, int>[] { });
            yield return (@"\Bx\B", "\u00E1x\u00DF", new[] { (1, 2) });
            yield return (@" \b", " \u03B4", new[] { (0, 1) });
            yield return (@" \B", " \u03B4", new ValueTuple<int, int>[] { });
            yield return (@"\w+", "a\u03B4", new[] { (0, 2) });
            yield return (@"\d+", "1\u0968\u09699", new[] { (0, 4) });
            yield return (@"[^a]", "\u03B4", new[] { (0, 1) });
            yield return (@"a", "\xFFa", new ValueTuple<int, int>[] { });
            yield return (@"a", "a", new[] { (0, 1) });
            yield return (@"[-+]?[0-9]*\.?[0-9]+", "0.1", new[] { (0, 3) });
            yield return (@"[-+]?[0-9]*\.?[0-9]+", "0.1.2", new[] { (0, 3), (3, 5) });
            yield return (@"[-+]?[0-9]*\.?[0-9]+", "a1.2", new[] { (1, 4) });
            yield return (@"^[-+]?[0-9]*\.?[0-9]+$", "1.a", new ValueTuple<int, int>[] { });
            yield return (@"[^ac]", "acx", new[] { (2, 3) });
            yield return (@"[^a,]", "a,x", new[] { (2, 3) });
            yield return (@"[^,]", ",,x", new[] { (2, 3) });
            yield return (@"((?:.*)*?)=", "a=b", new[] { (0, 2) });
            yield return (@"((?:.?)*?)=", "a=b", new[] { (0, 2) });
            yield return (@"((?:.*)+?)=", "a=b", new[] { (0, 2) });
            yield return (@"((?:.?)+?)=", "a=b", new[] { (0, 2) });
            yield return (@"((?:.*){1,}?)=", "a=b", new[] { (0, 2) });
            yield return (@"((?:.*){1,2}?)=", "a=b", new[] { (0, 2) });
            yield return (@"((?:.*)*)=", "a=b", new[] { (0, 2) });
            yield return (@"((?:.?)*)=", "a=b", new[] { (0, 2) });
            yield return (@"((?:.*)+)=", "a=b", new[] { (0, 2) });
            yield return (@"((?:.?)+)=", "a=b", new[] { (0, 2) });
            yield return (@"((?:.*){1,})=", "a=b", new[] { (0, 2) });
            yield return (@"((?:.*){1,2})=", "a=b", new[] { (0, 2) });
            yield return (@"abracadabra$", "abracadabracadabra", new[] { (7, 18) });
            yield return (@"a...b", "abababbb", new[] { (2, 7) });
            yield return (@"XXXXXX", "..XXXXXX", new[] { (2, 8) });
            yield return (@"\)", "()", new[] { (1, 2) });
            yield return (@"a]", "a]a", new[] { (0, 2) });
            yield return (@"\}", "}", new[] { (0, 1) });
            yield return (@"\]", "]", new[] { (0, 1) });
            yield return (@"]", "]", new[] { (0, 1) });
            yield return (@"^a", "ax", new[] { (0, 1) });
            yield return (@"\^a", "a^a", new[] { (1, 3) });
            yield return (@"a\^", "a^", new[] { (0, 2) });
            yield return (@"a$", "aa", new[] { (1, 2) });
            yield return (@"a\$", "a$", new[] { (0, 2) });
            yield return (@"^$", "", new[] { (0, 0) });
            yield return (@"$^", "", new[] { (0, 0) });
            yield return (@"a($)", "aa", new[] { (1, 2) });
            yield return (@"a*(^a)", "aa", new[] { (0, 1) });
            yield return (@"(..)*(...)*", "a", new[] { (0, 0), (1, 1) });
            yield return (@"(..)*(...)*", "abcd", new[] { (0, 4), (4, 4) });
            yield return (@"(ab)c|abc", "abc", new[] { (0, 3) });
            yield return (@"a{0}b", "ab", new[] { (1, 2) });
            yield return (@"a*(a.|aa)", "aaaa", new[] { (0, 4) });
            yield return (@"(a|b)?.*", "b", new[] { (0, 1), (1, 1) });
            yield return (@"(a|b)c|a(b|c)", "ac", new[] { (0, 2) });
            yield return (@"(a|b)*c|(a|ab)*c", "abc", new[] { (0, 3) });
            yield return (@"(a|b)*c|(a|ab)*c", "xc", new[] { (1, 2) });
            yield return (@"a?(ab|ba)ab", "abab", new[] { (0, 4) });
            yield return (@"a?(ac{0}b|ba)ab", "abab", new[] { (0, 4) });
            yield return (@"ab|abab", "abbabab", new[] { (0, 2), (3, 5), (5, 7) });
            yield return (@"aba|bab|bba", "baaabbbaba", new[] { (5, 8) });
            yield return (@"aba|bab", "baaabbbaba", new[] { (6, 9) });
            yield return (@"ab|a", "xabc", new[] { (1, 3) });
            yield return (@"ab|a", "xxabc", new[] { (2, 4) });
            yield return (@"[^-]", "--a", new[] { (2, 3) });
            yield return (@"[a-]*", "--a", new[] { (0, 3), (3, 3) });
            yield return (@"[a-m-]*", "--amoma--", new[] { (0, 4), (4, 4), (5, 9), (9, 9) });
            yield return (@"[\p{Lu}]", "A", new[] { (0, 1) });
            yield return (@"[\p{Ll}]+", "`az{", new[] { (1, 3) });
            yield return (@"[\p{Lu}]+", "@AZ[", new[] { (1, 3) });
            yield return (@"xxx", "xxx", new[] { (0, 3) });
            yield return (@".*", "\u263A\u007F", new[] { (0, 2), (2, 2) });
            yield return (@"a*a*a*a*a*b", "aaaaaaaaab", new[] { (0, 10) });
            yield return (@"^", "", new[] { (0, 0) });
            yield return (@"$", "", new[] { (0, 0) });
            yield return (@"^$", "", new[] { (0, 0) });
            yield return (@"^a$", "a", new[] { (0, 1) });
            yield return (@"abc", "abc", new[] { (0, 3) });
            yield return (@"abc", "xabcy", new[] { (1, 4) });
            yield return (@"abc", "ababc", new[] { (2, 5) });
            yield return (@"ab*c", "abc", new[] { (0, 3) });
            yield return (@"ab*bc", "abc", new[] { (0, 3) });
            yield return (@"ab*bc", "abbc", new[] { (0, 4) });
            yield return (@"ab*bc", "abbbbc", new[] { (0, 6) });
            yield return (@"ab+bc", "abbc", new[] { (0, 4) });
            yield return (@"ab+bc", "abbbbc", new[] { (0, 6) });
            yield return (@"ab?bc", "abbc", new[] { (0, 4) });
            yield return (@"ab?bc", "abc", new[] { (0, 3) });
            yield return (@"ab?c", "abc", new[] { (0, 3) });
            yield return (@"^abc$", "abc", new[] { (0, 3) });
            yield return (@"^abc", "abcc", new[] { (0, 3) });
            yield return (@"abc$", "aabc", new[] { (1, 4) });
            yield return (@"^", "abc", new[] { (0, 0) });
            yield return (@"$", "abc", new[] { (3, 3) });
            yield return (@"a.c", "abc", new[] { (0, 3) });
            yield return (@"a.c", "axc", new[] { (0, 3) });
            yield return (@"a.*c", "axyzc", new[] { (0, 5) });
            yield return (@"a[bc]d", "abd", new[] { (0, 3) });
            yield return (@"a[b-d]e", "ace", new[] { (0, 3) });
            yield return (@"a[b-d]", "aac", new[] { (1, 3) });
            yield return (@"a[-b]", "a-", new[] { (0, 2) });
            yield return (@"a[b-]", "a-", new[] { (0, 2) });
            yield return (@"a]", "a]", new[] { (0, 2) });
            yield return (@"a[]]b", "a]b", new[] { (0, 3) });
            yield return (@"a[^bc]d", "aed", new[] { (0, 3) });
            yield return (@"a[^-b]c", "adc", new[] { (0, 3) });
            yield return (@"a[^]b]c", "adc", new[] { (0, 3) });
            yield return (@"ab|cd", "abc", new[] { (0, 2) });
            yield return (@"ab|cd", "abcd", new[] { (0, 2), (2, 4) });
            yield return (@"a\(b", "a(b", new[] { (0, 3) });
            yield return (@"a\(*b", "ab", new[] { (0, 2) });
            yield return (@"a\(*b", "a((b", new[] { (0, 4) });
            yield return (@"a+b+c", "aabbabc", new[] { (4, 7) });
            yield return (@"a*", "aaa", new[] { (0, 3), (3, 3) });
            yield return (@"(a*)*", "-", new[] { (0, 0), (1, 1) });
            yield return (@"(a*)+", "-", new[] { (0, 0), (1, 1) });
            yield return (@"(a*|b)*", "-", new[] { (0, 0), (1, 1) });
            yield return (@"(a+|b)*", "ab", new[] { (0, 2), (2, 2) });
            yield return (@"(a+|b)+", "ab", new[] { (0, 2) });
            yield return (@"(a+|b)?", "ab", new[] { (0, 1), (1, 2), (2, 2) });
            yield return (@"[^ab]*", "cde", new[] { (0, 3), (3, 3) });
            yield return (@"(^)*", "-", new[] { (0, 0), (1, 1) });
            yield return (@"a*", "", new[] { (0, 0) });
            yield return (@"([abc])*d", "abbbcd", new[] { (0, 6) });
            yield return (@"([abc])*bcd", "abcd", new[] { (0, 4) });
            yield return (@"a|b|c|d|e", "e", new[] { (0, 1) });
            yield return (@"(a|b|c|d|e)f", "ef", new[] { (0, 2) });
            yield return (@"((a*|b))*", "-", new[] { (0, 0), (1, 1) });
            yield return (@"abcd*efg", "abcdefg", new[] { (0, 7) });
            yield return (@"ab*", "xabyabbbz", new[] { (1, 3), (4, 8) });
            yield return (@"ab*", "xayabbbz", new[] { (1, 2), (3, 7) });
            yield return (@"(ab|cd)e", "abcde", new[] { (2, 5) });
            yield return (@"[abhgefdc]ij", "hij", new[] { (0, 3) });
            yield return (@"(a|b)c*d", "abcd", new[] { (1, 4) });
            yield return (@"(ab|ab*)bc", "abc", new[] { (0, 3) });
            yield return (@"a([bc]*)c*", "abc", new[] { (0, 3) });
            yield return (@"a[bcd]*dcdcde", "adcdcde", new[] { (0, 7) });
            yield return (@"(ab|a)b*c", "abc", new[] { (0, 3) });
            yield return (@"[A-Za-z_][A-Za-z0-9_]*", "alpha", new[] { (0, 5) });
            yield return (@"^a(bc+|b[eh])g|.h$", "abh", new[] { (1, 3) });
            yield return (@"abcd", "abcd", new[] { (0, 4) });
            yield return (@"a(bc)d", "abcd", new[] { (0, 4) });
            yield return ("a[\u263A-\u2665]?c", "a\u263bc", new[] { (0, 3) });
            yield return (@"a+(b|c)*d+", "aabcdd", new[] { (0, 6) });
            yield return (@"^.+$", "vivi", new[] { (0, 4) });
            yield return (@"^(.+)$", "vivi", new[] { (0, 4) });
            yield return (@".*(/XXX).*", "/XXX", new[] { (0, 4) });
            yield return (@".*(/000).*", "/000", new[] { (0, 4) });
            yield return (@".*(\\000).*", "\000", new ValueTuple<int, int>[] { });
            yield return (@"\\000", "\000", new ValueTuple<int, int>[] { });
            yield return (@"(a*)*", "a", new[] { (0, 1), (1, 1) });
            yield return (@"(a*)*", "x", new[] { (0, 0), (1, 1) });
            yield return (@"(a*)*", "aaaaaa", new[] { (0, 6), (6, 6) });
            yield return (@"(a*)*", "aaaaaax", new[] { (0, 6), (6, 6), (7, 7) });
            yield return (@"(a*)+", "a", new[] { (0, 1), (1, 1) });
            yield return (@"(a*)+", "x", new[] { (0, 0), (1, 1) });
            yield return (@"(a*)+", "aaaaaa", new[] { (0, 6), (6, 6) });
            yield return (@"(a*)+", "aaaaaax", new[] { (0, 6), (6, 6), (7, 7) });
            yield return (@"(a+)*", "a", new[] { (0, 1), (1, 1) });
            yield return (@"(a+)*", "x", new[] { (0, 0), (1, 1) });
            yield return (@"(a+)*", "aaaaaa", new[] { (0, 6), (6, 6) });
            yield return (@"(a+)*", "aaaaaax", new[] { (0, 6), (6, 6), (7, 7) });
            yield return (@"(a+)+", "a", new[] { (0, 1) });
            yield return (@"(a+)+", "x", new ValueTuple<int, int>[] { });
            yield return (@"(a+)+", "aaaaaa", new[] { (0, 6) });
            yield return (@"(a+)+", "aaaaaax", new[] { (0, 6) });
            yield return (@"([a]*)*", "a", new[] { (0, 1), (1, 1) });
            yield return (@"([a]*)*", "x", new[] { (0, 0), (1, 1) });
            yield return (@"([a]*)*", "aaaaaa", new[] { (0, 6), (6, 6) });
            yield return (@"([a]*)*", "aaaaaax", new[] { (0, 6), (6, 6), (7, 7) });
            yield return (@"([a]*)+", "a", new[] { (0, 1), (1, 1) });
            yield return (@"([a]*)+", "x", new[] { (0, 0), (1, 1) });
            yield return (@"([a]*)+", "aaaaaa", new[] { (0, 6), (6, 6) });
            yield return (@"([a]*)+", "aaaaaax", new[] { (0, 6), (6, 6), (7, 7) });
            yield return (@"([^b]*)*", "a", new[] { (0, 1), (1, 1) });
            yield return (@"([^b]*)*", "b", new[] { (0, 0), (1, 1) });
            yield return (@"([^b]*)*", "aaaaaa", new[] { (0, 6), (6, 6) });
            yield return (@"([ab]*)*", "a", new[] { (0, 1), (1, 1) });
            yield return (@"([ab]*)*", "aaaaaa", new[] { (0, 6), (6, 6) });
            yield return (@"([ab]*)*", "ababab", new[] { (0, 6), (6, 6) });
            yield return (@"([ab]*)*", "bababa", new[] { (0, 6), (6, 6) });
            yield return (@"([ab]*)*", "b", new[] { (0, 1), (1, 1) });
            yield return (@"([ab]*)*", "bbbbbb", new[] { (0, 6), (6, 6) });
            yield return (@"([^a]*)*", "b", new[] { (0, 1), (1, 1) });
            yield return (@"([^a]*)*", "bbbbbb", new[] { (0, 6), (6, 6) });
            yield return (@"([^a]*)*", "aaaaaa", new[] { (0, 0), (1, 1), (2, 2), (3, 3), (4, 4), (5, 5), (6, 6) });
            yield return (@"([^ab]*)*", "ababab", new[] { (0, 0), (1, 1), (2, 2), (3, 3), (4, 4), (5, 5), (6, 6) });
            yield return (@"((..)|(.))", "", new ValueTuple<int, int>[] { });
            yield return (@"((..)|(.))((..)|(.))", "", new ValueTuple<int, int>[] { });
            yield return (@"((..)|(.))((..)|(.))((..)|(.))", "", new ValueTuple<int, int>[] { });
            yield return (@"((..)|(.)){1}", "", new ValueTuple<int, int>[] { });
            yield return (@"((..)|(.)){2}", "", new ValueTuple<int, int>[] { });
            yield return (@"((..)|(.)){3}", "", new ValueTuple<int, int>[] { });
            yield return (@"((..)|(.))*", "", new[] { (0, 0) });
            yield return (@"((..)|(.))((..)|(.))", "a", new ValueTuple<int, int>[] { });
            yield return (@"((..)|(.))((..)|(.))((..)|(.))", "a", new ValueTuple<int, int>[] { });
            yield return (@"((..)|(.)){2}", "a", new ValueTuple<int, int>[] { });
            yield return (@"((..)|(.)){3}", "a", new ValueTuple<int, int>[] { });
            yield return (@"((..)|(.))((..)|(.))((..)|(.))", "aa", new ValueTuple<int, int>[] { });
            yield return (@"((..)|(.)){3}", "aa", new ValueTuple<int, int>[] { });
            yield return (@"(a|ab|c|bcd){4,}(d*)", "ababcd", new ValueTuple<int, int>[] { });
            yield return (@"(a|ab|c|bcd){4,10}(d*)", "ababcd", new ValueTuple<int, int>[] { });
            yield return (@"(ab|a|c|bcd){4,}(d*)", "ababcd", new ValueTuple<int, int>[] { });
            yield return (@"(ab|a|c|bcd){4,10}(d*)", "ababcd", new ValueTuple<int, int>[] { });
            yield return (@"^abc", "abc", new[] { (0, 3) });
            yield return (@"^abc", "zabc", new ValueTuple<int, int>[] { });
            yield return (@"abc", "xxxxxab", new ValueTuple<int, int>[] { });
            yield return (@"(?i)[^x]", "x", new ValueTuple<int, int>[] { });
            yield return (@"(?i)[^x]", "X", new ValueTuple<int, int>[] { });
            yield return (@"[[:word:]]", "_", new ValueTuple<int, int>[] { });
            yield return (@"ab?|$", "az", new[] { (0, 1), (2, 2) });
            yield return (@"^(.*?)(\n|\r\n?|$)", "ab\rcd", new[] { (0, 3) });
            yield return (@"z*azb", "azb", new[] { (0, 3) });
            yield return (@"(?i)\p{Ll}+", "\u039B\u0398\u0393\u0394\u03B1", new[] { (0, 5) });
            yield return (@"1|2|3|4|5|6|7|8|9|10|int", "int", new[] { (0, 3) });
            yield return (@"^a[[:^space:]]", "a ", new ValueTuple<int, int>[] { });
            yield return (@"^a[[:^space:]]", "foo boo a ", new ValueTuple<int, int>[] { });
            yield return (@"^-[a-z]", "r-f", new ValueTuple<int, int>[] { });
            yield return (@"(ABC|CDA|BC)X", "CDAX", new[] { (0, 4) });
            yield return (@"(aa$)?", "aaz", new[] { (0, 0), (1, 1), (2, 2), (3, 3) });
            yield return (@"ab??", "ab", new[] { (0, 1) });
            yield return (@".*abcd", "abcd", new[] { (0, 4) });
            yield return (@".*(?:abcd)+", "abcd", new[] { (0, 4) });
            yield return (@".*(?:abcd)+", "abcdabcd", new[] { (0, 8) });
            yield return (@".*(?:abcd)+", "abcdxabcd", new[] { (0, 9) });
            yield return (@".*x(?:abcd)+", "abcdxabcd", new[] { (0, 9) });
            yield return (@"[^abcd]*x(?:abcd)+", "abcdxabcd", new[] { (4, 9) });
            yield return (@".", "\xD4\xC2\x65\x2B\x0E\xFE", new[] { (0, 1), (1, 2), (2, 3), (3, 4), (4, 5), (5, 6) });
            yield return ("${2}\u00E4", "\xD4\xC2\x65\x2B\x0E\xFE", new ValueTuple<int, int>[] { });
            yield return ("\u2603", "\u2603", new[] { (0, 1) });
            yield return ("\u2603+", "\u2603", new[] { (0, 1) });
            yield return ("(?i)\u2603+", "\u2603", new[] { (0, 1) });
            yield return ("[\u2603\u2160]+", "\u2603", new[] { (0, 1) });
            yield return ("(?i)\u0394", "\u03B4", new[] { (0, 1) });
            yield return (@"\p{Lu}+", "\u039B\u0398\u0393\u0394\u03B1", new[] { (0, 4) });
            yield return (@"(?i)\p{Lu}+", "\u039B\u0398\u0393\u0394\u03B1", new[] { (0, 5) });
            yield return (@"\p{L}+", "\u039B\u0398\u0393\u0394\u03B1", new[] { (0, 5) });
            yield return (@"\p{Ll}+", "\u039B\u0398\u0393\u0394\u03B1", new[] { (4, 5) });
            yield return (@"\w+", "d\u03B4d", new[] { (0, 3) });
            yield return (@"\w+", "\u2961", new ValueTuple<int, int>[] { });
            yield return (@"\W+", "\u2961", new[] { (0, 1) });
            yield return (@"\d+", "1\u0968\u09699", new[] { (0, 4) });
            yield return (@"\d+", "\u2161", new ValueTuple<int, int>[] { });
            yield return (@"\D+", "\u2161", new[] { (0, 1) });
            yield return (@"\s+", "\u1680", new[] { (0, 1) });
            yield return (@"\s+", "\u2603", new ValueTuple<int, int>[] { });
            yield return (@"\S+", "\u2603", new[] { (0, 1) });
            yield return (@"\d\b", "6\u03B4", new ValueTuple<int, int>[] { });
            yield return (@"\d\b", "6\u1680", new[] { (0, 1) });
            yield return (@"\d\B", "6\u03B4", new[] { (0, 1) });
            yield return (@"\d\B", "6\u1680", new ValueTuple<int, int>[] { });
        }

        [Theory]
        [MemberData(nameof(MatchStartAndEndPositions_MemberData))]
        public void MatchStartAndEndPositions(Regex regex, string input, IEnumerable<(int, int)>? matchBoundaries)
        {
            Match match = regex.Match(input);
            foreach ((int start, int end) in matchBoundaries)
            {
                Assert.True(match.Success);
                Assert.Equal(start, match.Index);
                Assert.Equal(end, match.Index + match.Length);
                match = match.NextMatch();
            }
            Assert.False(match.Success);
        }
    }
}
