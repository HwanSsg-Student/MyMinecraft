using System;
using UnityEngine;


[Serializable]
public class ItemStack
{
    BlockType _blockType;
    int _count;
    int _maxCount;

    public BlockType BlockType 
    { 
        get { return _blockType; } 
        private set { } 
    }
    public int Count
    { 
        get { return _count; } 
        set { _count = value; } 
    }
    public int MaxCount 
    { 
        get { return _maxCount; } 
        private set { } 
    }
    public bool IsEmpty => _blockType == BlockType.None || Count <= 0;
    public int RemainingCapacity => Mathf.Max(0, MaxCount - Count);

    public ItemStack(BlockType blockType, int count, int maxStack)
    {
        _blockType = blockType;
        _maxCount = Math.Max(1, maxStack);
        _count = Math.Clamp(count, 0, MaxCount);
    }

    public int TryAdd(int amount)
    {
        if (amount <= 0 || IsEmpty) return 0;

        int add = Mathf.Min(amount, RemainingCapacity);
        _count += add;
        return add;
    }

    public int TryRemove(int amount)
    {
        if (amount <= 0 || IsEmpty) return 0;

        int rm = Mathf.Min(amount, _count);
        _count -= rm;
        return rm;
    }

    public ItemStack SplitItem(int amount)
    {
        var taken = TryRemove(amount);
        if(taken <= 0) return null;
        return new ItemStack(_blockType, taken, _maxCount);
    }

    public bool CanMerge(ItemStack other)
    {
        if (other == null) return false;
        if (_blockType != other._blockType) return false;
        if (IsEmpty || other.IsEmpty) return false;
        if (Count == MaxCount) return false;
        return true;
    }

    public int MergeFrom(ItemStack other)
    {
        if(!CanMerge(other)) return 0;

        int moved = TryAdd(other.Count);
        other.TryRemove(moved);
        return moved;
    }


}
