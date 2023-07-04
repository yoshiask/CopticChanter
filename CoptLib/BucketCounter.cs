using System.Collections.Generic;
using System.Linq;

namespace CoptLib;

public class BucketCounter<TBucket>
{
    private int? totalCount = null;
    private Dictionary<TBucket, int> _dict = new();

    public ICollection<TBucket> Buckets => _dict.Keys;

    public int Total
    {
        get => totalCount ?? _dict.Values.Sum();
        set => totalCount = value < 0 ? null : value;
    }

    /// <summary>
    /// Gets or sets the count of the specified <paramref name="bucket"/>.
    /// </summary>
    public int this[TBucket bucket]
    {
        get
        {
            EnsureExists(bucket);
            return _dict[bucket];
        }
        set
        {
            EnsureExists(bucket);
            _dict[bucket] = value;
        }
    }

    /// <summary>
    /// Increments the specified <paramref name="bucket"/>.
    /// </summary>
    /// <returns>The new count of the bucket.</returns>
    public void IncrementBucket(TBucket bucket) => ++this[bucket];

    /// <summary>
    /// Adds <paramref name="count"/> to the specified <paramref name="bucket"/>.
    /// </summary>
    /// <returns>The new count of the bucket.</returns>
    public int AddToBucket(TBucket bucket, int count) => this[bucket] += count;

    public double GetBucketPercent(TBucket bucket) => (double)this[bucket] / Total;

    public double GetTotalPercent() => (double)_dict.Values.Sum() / Total;

    public TBucket GetLargestBucket() => _dict.Keys.OrderByDescending(b => _dict[b]).FirstOrDefault();

    private bool EnsureExists(TBucket bucket)
    {
        if (_dict.ContainsKey(bucket))
            return false;
        
        _dict.Add(bucket, 0);
        return true;
    }
}