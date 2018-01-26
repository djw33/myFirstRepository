/********************************************************************************
 * 
 *      Program 1: Lg Lg n 
 *      Daniel Weber, 9/19/2017
 * 
 *********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Lg Lg n!");
            while (true)
            {
                Console.Write("\nEnter n: ");
                long n = long.Parse(Console.ReadLine());
                long ans = lg(lg(n));
                Console.WriteLine("lg(lg({0})) = {1}.", n, ans);
            }
        }
        static long lg(long n)
        {
            int count = 0;
            while(n>1)
            {
                count = count + 1;
                n = n / 2;
            }
            return count;
        }
    }
}
