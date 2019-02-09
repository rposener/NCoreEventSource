using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer
{
    public static class IEnumerableOfTExtensions
    {
        /// <summary>
        /// Runs a set of Tasks in Parallel using Partitioner to Limit Concurrency
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">Item to Iterate Over</param>
        /// <param name="dop">Degrees of Parallelism (how many concurrent)</param>
        /// <param name="body">Function to Execute</param>
        /// <returns></returns>
        public static Task ForEachAsync<T>(this IEnumerable<T> source, int dop, Func<T, Task> body)
        {
            return Task.WhenAll(
                from partition in Partitioner.Create(source).GetPartitions(dop)
                select Task.Run(async delegate {
                    using (partition)
                        while (partition.MoveNext())
                            await body(partition.Current);
                }));
        }
    }
}
