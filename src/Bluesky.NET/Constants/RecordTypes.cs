﻿namespace Bluesky.NET.Constants;

public static class RecordTypes
{
    public const string NewPost = "app.bsky.feed.post";
    public const string Like = "app.bsky.feed.like";
    public const string Repost = "app.bsky.feed.repost";
    public const string StarterPack = "app.bsky.graph.starterpack";
    public const string Follow = "app.bsky.graph.follow";

    public static string ToStringType(this RecordType recordType) => recordType switch
    {
        RecordType.Reply => NewPost,
        RecordType.NewPost => NewPost,
        RecordType.Like => Like,
        RecordType.Repost => Repost,
        RecordType.StarterPack => StarterPack,
        RecordType.Follow => Follow,
        _ => string.Empty,
    };

    public static RecordType GetRecordType(this string recordTypeString) => recordTypeString switch
    {
        NewPost => RecordType.NewPost,
        Like => RecordType.Like,
        Repost => RecordType.Repost,
        StarterPack => RecordType.StarterPack,
        Follow => RecordType.Follow,
        _ => RecordType.Unknown,
    };
}

public enum RecordType
{
    Unknown,
    NewPost,
    Like,
    Repost,
    Reply,
    StarterPack,
    Follow
}
