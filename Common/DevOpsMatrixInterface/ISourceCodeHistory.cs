﻿
namespace DevOpsInterface
{
    public interface ISourceCodeHistory
    {
        int Id { get; }
        string Comment { get; }
        DateTime Timestamp { get; }
        string Owner { get; }

        List<ISourceCodeHistoryItem> Changes { get; }
        List<IMergeItemInfo> MergeInfo { get; }
    }
}