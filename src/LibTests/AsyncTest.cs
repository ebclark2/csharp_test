using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace LibTests
{
    public class AsyncTest    
    {

        // Create several async tasks with a delay.
        // Await results and verify tasks did not execute in order created.
        // Verify final result is correct
        [Test]
        public async Task CountRace() {
            int start = 0;
            Func<Task<int>> nextInt = async () => { 
                await Task.Delay(100); 
                Console.WriteLine($"TID: {Thread.CurrentThread.ManagedThreadId}"); 
                return ++start; 
            };

            var tasks = new List<Task<int>>();
            for(int i=0; i<100; ++i) {
                tasks.Add(nextInt());
            }
            foreach (var t in tasks) {
                int n = await t;
                Console.WriteLine($"n: {n}");
            }
            Console.WriteLine($"start: {start}");
        }
    
        // Test captured local variable that goes out of scope
        public Func<Task<int>> createFuncWithCapturedLocalVariable() {
            int localInt = 0;
            return async () => { await Task.Delay(100); return ++localInt; };
        }

        [Test]
        public async Task VerifyOutOfScopeVariableCapture() {
            var func = createFuncWithCapturedLocalVariable();
            var tasks = new List<Task<int>>();
            for(int i=0; i<100; ++i) {
                tasks.Add(func());
            }
            foreach (var t in tasks) {
                int n = await t;
                Console.WriteLine($"n: {n}");
            }
        }

        [Test]
        public async Task VerifyOutOfOrderAwait() {
            Func<int, Task> delayedPrint = async (delay) => {
                await Task.Delay(delay);                
                Console.WriteLine($"{DateTime.Now}: Delay {delay} complete!");
            };

            Console.WriteLine($"{DateTime.Now} Await blocks");
            await delayedPrint(3000);
            await delayedPrint(2000);
            await delayedPrint(1000);

            Console.WriteLine($"{DateTime.Now} Create tasks then await, no block");
            Task t1 = delayedPrint(3000);
            Task t2 = delayedPrint(2000);
            Task t3 = delayedPrint(1000);

            await t1;
            await t2;
            await t3;

        }
    }

}
