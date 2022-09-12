using Microsoft.AspNetCore.Mvc;
using OneOf;

namespace WebApi.Properties;

public static class OneOfExtensions
{
    public static IActionResult MatchResponse<T0>(this OneOf<T0> oneOf, Func<T0, IActionResult>? t0 = null)
    {
        return oneOf.Match(
            t0 ?? ResponseMatcher.AsErrorResult
        );
    }

    public static IActionResult MatchResponse<T0, T1>(this OneOf<T0, T1> oneOf, Func<T0, IActionResult>? t0 = null,
        Func<T1, IActionResult>? t1 = null)
    {
        return oneOf.Match(
            t0 ?? ResponseMatcher.AsErrorResult,
            t1 ?? ResponseMatcher.AsErrorResult
        );
    }

    public static IActionResult MatchResponse<T0, T1, T2>(this OneOf<T0, T1, T2> oneOf,
        Func<T0, IActionResult>? t0 = null, Func<T1, IActionResult>? t1 = null, Func<T2, IActionResult>? t2 = null)
    {
        return oneOf.Match(
            t0 ?? ResponseMatcher.AsErrorResult,
            t1 ?? ResponseMatcher.AsErrorResult,
            t2 ?? ResponseMatcher.AsErrorResult
        );
    }

    public static IActionResult MatchResponse<T0, T1, T2, T3>(this OneOf<T0, T1, T2, T3> oneOf,
        Func<T0, IActionResult>? t0 = null, Func<T1, IActionResult>? t1 = null, Func<T2, IActionResult>? t2 = null,
        Func<T3, IActionResult>? t3 = null)
    {
        return oneOf.Match(
            t0 ?? ResponseMatcher.AsErrorResult,
            t1 ?? ResponseMatcher.AsErrorResult,
            t2 ?? ResponseMatcher.AsErrorResult,
            t3 ?? ResponseMatcher.AsErrorResult
        );
    }

    public static IActionResult MatchResponse<T0, T1, T2, T3, T4>(this OneOf<T0, T1, T2, T3, T4> oneOf,
        Func<T0, IActionResult>? t0 = null, Func<T1, IActionResult>? t1 = null, Func<T2, IActionResult>? t2 = null,
        Func<T3, IActionResult>? t3 = null, Func<T4, IActionResult>? t4 = null)
    {
        return oneOf.Match(
            t0 ?? ResponseMatcher.AsErrorResult,
            t1 ?? ResponseMatcher.AsErrorResult,
            t2 ?? ResponseMatcher.AsErrorResult,
            t3 ?? ResponseMatcher.AsErrorResult,
            t4 ?? ResponseMatcher.AsErrorResult
        );
    }

    public static IActionResult MatchResponse<T0, T1, T2, T3, T4, T5>(this OneOf<T0, T1, T2, T3, T4, T5> oneOf,
        Func<T0, IActionResult>? t0 = null, Func<T1, IActionResult>? t1 = null, Func<T2, IActionResult>? t2 = null,
        Func<T3, IActionResult>? t3 = null, Func<T4, IActionResult>? t4 = null, Func<T5, IActionResult>? t5 = null)
    {
        return oneOf.Match(
            t0 ?? ResponseMatcher.AsErrorResult,
            t1 ?? ResponseMatcher.AsErrorResult,
            t2 ?? ResponseMatcher.AsErrorResult,
            t3 ?? ResponseMatcher.AsErrorResult,
            t4 ?? ResponseMatcher.AsErrorResult,
            t5 ?? ResponseMatcher.AsErrorResult
        );
    }

    public static IActionResult MatchResponse<T0, T1, T2, T3, T4, T5, T6>(
        this OneOf<T0, T1, T2, T3, T4, T5, T6> oneOf, Func<T0, IActionResult>? t0 = null,
        Func<T1, IActionResult>? t1 = null, Func<T2, IActionResult>? t2 = null, Func<T3, IActionResult>? t3 = null,
        Func<T4, IActionResult>? t4 = null, Func<T5, IActionResult>? t5 = null, Func<T6, IActionResult>? t6 = null)
    {
        return oneOf.Match(
            t0 ?? ResponseMatcher.AsErrorResult,
            t1 ?? ResponseMatcher.AsErrorResult,
            t2 ?? ResponseMatcher.AsErrorResult,
            t3 ?? ResponseMatcher.AsErrorResult,
            t4 ?? ResponseMatcher.AsErrorResult,
            t5 ?? ResponseMatcher.AsErrorResult,
            t6 ?? ResponseMatcher.AsErrorResult
        );
    }

    public static IActionResult MatchResponse<T0, T1, T2, T3, T4, T5, T6, T7>(
        this OneOf<T0, T1, T2, T3, T4, T5, T6, T7> oneOf, Func<T0, IActionResult>? t0 = null,
        Func<T1, IActionResult>? t1 = null, Func<T2, IActionResult>? t2 = null, Func<T3, IActionResult>? t3 = null,
        Func<T4, IActionResult>? t4 = null, Func<T5, IActionResult>? t5 = null, Func<T6, IActionResult>? t6 = null,
        Func<T7, IActionResult>? t7 = null)
    {
        return oneOf.Match(
            t0 ?? ResponseMatcher.AsErrorResult,
            t1 ?? ResponseMatcher.AsErrorResult,
            t2 ?? ResponseMatcher.AsErrorResult,
            t3 ?? ResponseMatcher.AsErrorResult,
            t4 ?? ResponseMatcher.AsErrorResult,
            t5 ?? ResponseMatcher.AsErrorResult,
            t6 ?? ResponseMatcher.AsErrorResult,
            t7 ?? ResponseMatcher.AsErrorResult
        );
    }

    public static IActionResult MatchResponse<T0, T1, T2, T3, T4, T5, T6, T7, T8>(
        this OneOf<T0, T1, T2, T3, T4, T5, T6, T7, T8> oneOf, Func<T0, IActionResult>? t0 = null,
        Func<T1, IActionResult>? t1 = null, Func<T2, IActionResult>? t2 = null, Func<T3, IActionResult>? t3 = null,
        Func<T4, IActionResult>? t4 = null, Func<T5, IActionResult>? t5 = null, Func<T6, IActionResult>? t6 = null,
        Func<T7, IActionResult>? t7 = null, Func<T8, IActionResult>? t8 = null)
    {
        return oneOf.Match(
            t0 ?? ResponseMatcher.AsErrorResult,
            t1 ?? ResponseMatcher.AsErrorResult,
            t2 ?? ResponseMatcher.AsErrorResult,
            t3 ?? ResponseMatcher.AsErrorResult,
            t4 ?? ResponseMatcher.AsErrorResult,
            t5 ?? ResponseMatcher.AsErrorResult,
            t6 ?? ResponseMatcher.AsErrorResult,
            t7 ?? ResponseMatcher.AsErrorResult,
            t8 ?? ResponseMatcher.AsErrorResult
        );
    }
}