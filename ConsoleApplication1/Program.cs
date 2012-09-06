using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1
{

    

    public interface ITest
    {
        string Name { get; set; }
    }

    public class In : ITest
    {
        public string Name { get; set; }

        public static implicit operator In(Out d)
        {
            return new In { Name = d.Name };
        }

        public static explicit operator Out(In d)
        {
            return new Out { Name = d.Name };
        }



    }
    public class Out : ITest
    {
        public string Name { get; set; }

    }

    class Program
    {

        static TOutput[] Map<TInput, TOutput>(Func<TInput, TOutput> mapFunction, TInput[] inputs)
        {
            return inputs.AsParallel()
                    .Select(v => mapFunction(v))
                    .ToArray();
        }

        static TValue Reduce<TValue>(TValue[] source, TValue seed, Func<TValue,TValue,TValue> reduceFunction)
        {
            return source.AsParallel()
                    .Aggregate(seed, (localResult, value) => reduceFunction(localResult, value),
                                    (overallResult, localResult) => reduceFunction(overallResult, localResult), overallResult => overallResult);
        }

        static IEnumerable<TOutput> MapReduce<TInput, TIntermediate, TKey, TOutput>(IEnumerable<TInput> source,
                                                                                    Func<TInput, IEnumerable<TIntermediate>> mapFunction,
                                                                                    Func<TIntermediate, TKey> groupFunction,
                                                                                    Func<IGrouping<TKey, TIntermediate>, TOutput> reduceFunction)
        {
            return source.AsParallel()
                        .SelectMany(mapFunction)
                        .GroupBy(groupFunction)
                        .Select(reduceFunction);
        }


        static void Main(string[] args)
        {
            List<string> words = new List<string> 
			{ "there", "is", "a", 
				"great", "house", "and", 
				"an", "amazing", "lake", 
				"there", "is", "a", 
				"computer", "running", "a", 
				"new", "query", "there", 
				"is", "a", "great", 
				"server", "ready", "to", 
				"process", 
				"map", "and", "reduce" };

            var dic = words.AsParallel().ToLookup(p => p, k => 1).ToDictionary(k => k, v => v);
            // Map
            // Generate a (word, 1) key, value pair 
            ILookup<string, int> map = words.AsParallel().ToLookup(p => p, k => 1);
            // End of Map

            // Reduce
            // Calculate the number of times a word appears and select the words that appear more than once
            var reduce = from IGrouping<string, int> wordMap
                         in map.AsParallel()
                         where wordMap.Count() > 1
                         select new { Word = wordMap.Key, Count = wordMap.Count() };
            // End of Reduce

            // Show each word and the number of times it appears
            foreach (var word in reduce)
                Console.WriteLine("Word: '{0}'; Count: {1}",
                    word.Word, word.Count);

            string k1 = "RiccardoSenzaVocali";

            var res = new string(k1.ToCharArray().Except(new[] { 'a', 'e', 'i', 'o', 'u' }).ToArray());
            Console.WriteLine(res);

            var @in = new In { Name = "Bugghina" };

            var @out = (Out)@in;
            //var out2 = (Out)@out;

            var in2 = (In)@out;

            Console.WriteLine(@out.Name);
        }
    }
}
