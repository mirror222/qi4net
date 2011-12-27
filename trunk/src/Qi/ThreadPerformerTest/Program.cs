﻿using System;
using System.Collections.Generic;
using System.Linq;
using Qi.Text;
using Qi.Threads;

namespace ThreadPerformerTest
{
    internal class Program
    {
        private static Int64 result;

        private static void Main(string[] args)
        {
            Console.WriteLine("is correct?");
            CheckCorrectAssignment();
            Console.WriteLine("Performer speed");
            CheckSpeed();
            Console.Read();
        }

        private static void CheckSpeed()
        {
            var max = new int[100 * 10000];

            DateTime dateTime = DateTime.Now;
            OnExecuteFunction(max);
            Console.WriteLine((DateTime.Now - dateTime).TotalMilliseconds);

            dateTime = DateTime.Now;

            max.AvgExecute(2, OnExecuteFunction);
            Console.WriteLine((DateTime.Now - dateTime).TotalMilliseconds);


            Console.Read();
        }

        private static void OnExecuteFunction_CheckSpeed(int[] s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                byte[] result = Guid.NewGuid().ToString().Sha512ASCII();
            }
        }

        private static void CheckCorrectAssignment()
        {
            int max = 840001;
            var ilist = new List<int>();
            int counter = 0;
            var random = new Random();
            IList<int> a = new List<int>();

            while (counter < max)
            {
                ilist.Add(random.Next(max));
                counter++;
            }

            DateTime dateTime = DateTime.Now;

            for (int i = 0; i < ilist.Count; i++)
            {
                result += ilist[i];
            }
            Console.WriteLine("Single thread result:" + result);
            Console.WriteLine((DateTime.Now - dateTime).TotalMilliseconds);

            dateTime = DateTime.Now;
            result = 0;

            ThreadHelper.AvgExecute(4, ilist.ToArray(), OnExecuteFunction);
            Console.WriteLine("4 thread result:" + result);
            Console.WriteLine((DateTime.Now - dateTime).TotalMilliseconds);
        }

        private static void OnExecuteFunction(int[] s)
        {
            Int64 partyResult = s.Aggregate<int, long>(0, (current, item) => current + item);
            result += partyResult;
        }
    }
}