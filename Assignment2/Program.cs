// using System;
// using System.Threading;

// namespace MonteCarloSimulator
// {
//     public class MonteCarloSim
//     {
//         int N;  
//         int M;
//         bool isParallel;

//         double initialPrice;

//         double[,] results;

//         public MonteCarloSim(int N, int M, bool isParallel, double initPrice)
//         {
//             this.N = N;
//             this.M = M;
//             this.isParallel = isParallel;
//             this.initialPrice = initPrice;
//             results = new double[M, N];
//         }

//         public void Run()
//         {
//             if (!isParallel)
//             {
//                 // Sequential version
//                 for (int i = 0; i < M; i++)
//                     SimulatePath(i, 0, M);
//             }
//             else
//             {
//                 int numCores = Environment.ProcessorCount;
//                 int simsPerCore = M / numCores;
//                 Thread[] threads = new Thread[numCores];

//                 for (int c = 0; c < numCores; c++)
//                 {
//                     int start = c * simsPerCore;
//                     int count = (c == numCores - 1) ? (M - start) : simsPerCore;

//                     // Use ParameterizedThreadStart
//                     threads[c] = new Thread(new ParameterizedThreadStart(ThreadWorker));
//                     threads[c].Start(new ThreadParams(start, count));
//                 }

//                 // Wait for all threads
//                 for (int c = 0; c < numCores; c++)
//                     threads[c].Join();
//             }

//             Console.WriteLine("Simulation completed!");
//         }

//         private void ThreadWorker(object? obj)
//         {
//             ThreadParams p = (ThreadParams)obj!;
//             for (int i = 0; i < p.Count; i++)
//             {
//                 int globalIndex = p.Start + i;
//                 SimulatePath(globalIndex, p.Start, p.Count);
//             }
//         }

//         private void SimulatePath(int pathIndex, int start, int count)
//         {
//             // Your Monte Carlo path generation logic goes here.
//             Random rand = new Random(Guid.NewGuid().GetHashCode()); 
//             double price = initialPrice; // Starting price

//             for (int t = 0; t < N; t++)
//             {
//                 double shock = Math.Sqrt(-2.0 * Math.Log(rand.NextDouble())) *
//                                Math.Cos(2 * Math.PI * rand.NextDouble());
//                 price *= Math.Exp(0.05 - 0.5 * 0.2 * 0.2 + 0.2 * shock);
//                 results[pathIndex, t] = price;
//             }
//         }
//     }

//     public class ThreadParams
//     {
//         public int Start { get; }
//         public int Count { get; }

//         public ThreadParams(int start, int count)
//         {
//             Start = start;
//             Count = count;
//         }
//     }
// }
