using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace GA
{
     public delegate double Evaluator(ref List<int> binaryEncoded, ref List<int> bestChromo);
     public delegate void GA_EventHandler(List<int> chromo, double bestFitness, double iteration);
     public delegate int Spinner(ref List<double> fitnesses);
     public delegate void Mutater(ref List<int> chromo);
     public delegate void Crosser(int p1, int p2, ref List<int> child1, ref List<int> child2);

     public enum GAType
     {
          BINARY,
          COMBO
     }

     public class ConicalGA
     {
          public event GA_EventHandler NewBest;

          Evaluator Eval = null;

          Crosser cross = null;
          Mutater mutate = null;
          Spinner spin = null;

          ManualResetEvent[] GADoneEvents = new ManualResetEvent[1];
          List<GAThread> GA_Threads = new List<GAThread>(1);
          int GAThreadcount = 1;

          public ConicalGA() { }

          public ConicalGA(int popSize, int chromoCount, GAType type)
          {
               m_xProp = 0.7;
               m_mutProp = 1.0 / chromoCount;
               m_size = chromoCount;
               m_type = type;
               m_population = new List<List<int>>(popSize);
               Initialize();
               SetupLambdas();
          }

          private void SetupLambdas()
          {
               cross = ((int p1, int p2, ref List<int> child1, ref List<int> child2) =>
               {
                    Cross(p1, p2, ref child1, ref child2);
               });

               mutate = ((ref List<int> chromo) =>
               {
                    Mutate(ref chromo);
               });

               spin = ((ref List<double> fitnesses) =>
               {
                    return Spin(ref fitnesses);
               });
          }

          public void SetEval(Evaluator function) { Eval = function; }

          public void Begin()
          {
               List<double> fitnesses = new List<double>();
               int iterationC = 0, BestIndex = 0, tmp = 0;
               double bestFitness = 0;
               List<int> bestChromo = new List<int>();
               int maxIt = 1000;

               using (StreamWriter sr = new StreamWriter("output.csv"))
               {
                    sr.WriteLine("Max Fitness, Min Fitness, Average Fitness");
                    while (iterationC < maxIt && !m_stop)
                    {
                         fitnesses.Clear();
                         Population.ForEach((List<int> chromo) =>
                              {
                                   fitnesses.Add(Eval(ref chromo, ref bestChromo));
                              });

                         tmp = 0;

                         bestFitness = fitnesses.Max();

                         sr.WriteLine("{0},{1},{2}", bestFitness, fitnesses.Min(), fitnesses.Average());

                         BestIndex = fitnesses.IndexOf(bestFitness);

                         if (bestChromo != Population[BestIndex])
                         {
                              bestChromo = Population[BestIndex];
                              if (NewBest != null)
                                   NewBest(bestChromo, bestFitness, ((double)iterationC / (double)maxIt));
                         }

                         if (bestFitness > 3e2)
                              m_stop = true;

                         Evolve(ref fitnesses, false);

                         iterationC++;

                    }
               }
          }

          bool m_stop = false;

          public void Stop()
          {
               m_stop = true;
          }

          void Evolve(ref List<double> fitnesses, bool useElites)
          {
               //here we're going to cross the children and return
               //spin wheel to get who survives

               try
               {
                    GA_Threads.Clear();
                    List<List<int>> cutPop = new List<List<int>>(Population.Count / GAThreadcount);
                    List<double> cutFitnesses = new List<double>(fitnesses.Count / GAThreadcount);
                    int step = Population.Count / GAThreadcount;
                    for (int i = 0; i < GAThreadcount; i++)
                    {
                         GADoneEvents[i] = new ManualResetEvent(false);
                         cutPop = new List<List<int>>(Population.GetRange(i * step, step));
                         cutFitnesses = new List<double>(fitnesses.GetRange(i * step, step));
                         GAThread gathr = new GAThread(GADoneEvents[i], cutPop, cutFitnesses, cross, mutate, spin, Eval);
                         GA_Threads.Add(gathr);
                         ThreadPool.QueueUserWorkItem(gathr.ThreadPoolCallback, i);
                    }

                    // Wait for all threads in pool to finish.
                    foreach (var e in GADoneEvents) e.WaitOne();

                    Population.Clear();
                    GA_Threads.ForEach((GAThread ga) => { Population.AddRange(ga.Result); });
               }
               catch (Exception e)
               {
                    string message = e.Message;
                    m_stop = true;
               }

               //List<int> chosen = new List<int>();
               //List<List<int>> children = new List<List<int>>();
               //List<List<int>> elites = new List<List<int>>();
               //List<int> child1, child2;
               //child1 = new List<int>();
               //child2 = new List<int>();

               //int i;

               //for (i = 0; i < Population.Count; i++)
               //     chosen.Add(Spin(ref fitnesses));

               //for (i = 1; i < Population.Count; i += 2)
               //{
               //     Cross(chosen[i - 1], chosen[i], ref child1, ref child2);
               //     children.Add(new List<int>(child1));
               //     children.Add(new List<int>(child2));
               //}

               //Population.Clear();
               //children.ForEach((List<int> chromos) =>
               //     {
               //          //Mutate(ref chromos);
               //          Population.Add(new List<int>(chromos));
               //     });
               //for (i = 0; i < Population.Count; i++)
               //     children.Add(new List<int>(Population[i]));
               //copy(&children[i].front(), &m_pop[i].front());

               //if (useElites)
               //{
               //     elites = GetElite(m_pop, fitnesses);

               //     for (i = 0; i < elites.size(); i++)
               //     {
               //          if (IsBinaryGA())
               //               mutate(&elites[i].front());
               //          else
               //               swap(&elites[i].front());

               //          copy(&elites[i].front(), &m_pop[i].front());
               //     }
               //}
          }

          #region Member Variables

          Random rand = new Random();

          List<List<int>> m_population;

          GAType m_type = GAType.BINARY;

          int m_size;

          double m_mutProp, m_xProp;

          #endregion Member Variables

          #region Properties

          /// <summary>
          /// GA Type
          /// </summary>
          internal GAType Type
          {
               get { return m_type; }
               set { m_type = value; }
          }

          /// <summary>
          /// Crossover Probability
          /// </summary>
          public double XProp
          {
               get { return m_xProp; }
               set { m_xProp = value; }
          }

          /// <summary>
          /// Mutation Probability
          /// </summary>
          public double MutProp
          {
               get { return m_mutProp; }
               set { m_mutProp = value; }
          }

          /// <summary>
          /// Population Size
          /// </summary>
          public int Size
          {
               get { return m_size; }
               set { m_size = value; }
          }

          /// <summary>
          /// Population
          /// </summary>
          public List<List<int>> Population
          {
               get { return m_population; }
               set { m_population = value; }
          }

          #endregion Properties

          #region Methods

          private void Initialize()
          {
               if (Type == GAType.BINARY)
               {
                    for (int i = 0; i < Population.Capacity; i++)
                    {
                         List<int> tmp = new List<int>();
                         for (int j = 0; j < m_size; j++)
                              tmp.Add(rand.Next() % 2);// randomly generate a chromosome of 1's and 0's
                         Population.Add(tmp);
                    }
               }
               else
               {
                    for (int i = 0; i < Population.Capacity; i++)
                         Population.Add(ComboInitialize());

               }

               
          }

          /// <summary>
          /// initialize combo chromosome
          /// </summary>
          /// <returns></returns>
          List<int> ComboInitialize()
          {
               List<int> numbers = new List<int>();
               int i, n, tmp;

               //srand(time(NULL));

               // Initialize the array
               for (i = 0; i < m_size; ++i)
                    numbers.Add(i);

               // Shuffle the array
               for (i = 0; i < m_size; ++i)
               {
                    n = rand.Next() % m_size;
                    tmp = numbers[n];
                    numbers[n] = numbers[i];
                    numbers[i] = tmp;
               }

               return numbers;

          }

          /// <summary>
          /// Get a random number between 0 and 1 (percentage)
          /// </summary>
          /// <returns></returns>
          double GetRandom()
          {
               return (double)(rand.Next(0, Int32.MaxValue) / (double)Int32.MaxValue);
          }

          /// <summary>
          /// randomly flip binary
          /// </summary>
          /// <param name="chance"></param>
          /// <param name="val"></param>
          /// <returns></returns>
          int Flip(double chance, int val)
          {
               return GetRandom() <= chance ? 1 - val : val;
          }

          /// <summary>
          /// mutate binary chromosome
          /// </summary>
          /// <param name="chromo"></param>
          void Mutate(ref List<int> chromo)
          {
               for (int i = 0; i < Size; i++)
                    chromo[i] = Flip(MutProp, chromo[i]);
          }

          /// <summary>
          /// swap values in a chromosome
          /// </summary>
          /// <param name="chromo"></param>
          void Swap(ref List<int> chromo)
          {
               int index, index2, tmp;

               index = (int)(Size * GetRandom());
               index2 = index + (int)((Size - index - 1) * GetRandom());

               if (index == index2)
               {
                    if (index2 + 1 < Size)
                         index2 += 1;
                    else if (index - 1 > 0)
                         index -= 1;
               }

               if (index > 0 && index < Size)
                    if (index2 > 0 && index2 < Size)
                    {
                         tmp = chromo[index2];
                         chromo[index2] = chromo[index];
                         chromo[index] = tmp;
                    }
          }

          ///// <summary>
          ///// create two new children from two parents
          ///// </summary>
          ///// <param name="parent1"></param>
          ///// <param name="parent2"></param>
          ///// <param name="parent1"></param>
          ///// <param name="parent2"></param>
          //void Cross(ref List<int> parent1, ref List<int> parent2, ref List<int> child1, ref List<int> child2)
          //{
          //     int index = (int)(Size * GetRandom()); // get random index for crossing

          //     if (index == 0)
          //          index += 1;
          //     else if (index == m_size)
          //          index -= 1;

          //     child1.Clear(); //tmp variable for storage
          //     child2.Clear(); //tmp variable for storage

          //     double rand = GetRandom(); // get random value

          //     if (rand >= XProp) // if greater than 0.7, then don't cross
          //     {
          //          child1 = parent1;
          //          child2 = parent2;
          //          Mutate(ref child1); // mutate
          //          Mutate(ref child2); // mutate
          //          return;
          //     }

          //     for (int i = 0; i < m_size; i++) // cross children with parents
          //     {
          //          if (index >= i)
          //          {
          //               child1.Add(parent1[i]);
          //               child2.Add(parent2[i]);
          //          }
          //          else
          //          {
          //               child1.Add(parent2[i]);
          //               child2.Add(parent1[i]);
          //          }
          //     }

          //     Mutate(ref child1); // mutate
          //     Mutate(ref child2); // mutate
          //}

          /// <summary>
          /// cross
          /// </summary>
          /// <param name="p1"></param>
          /// <param name="p2"></param>
          /// <param name="child1"></param>
          /// <param name="child2"></param>
          private void Cross(int p1, int p2, ref List<int> child1, ref List<int> child2)
          {
               int index = (int)(Size * GetRandom()); // get random index for crossing

               if (index == 0)
                    index += 1;
               else if (index == m_size)
                    index -= 1;

               child1.Clear(); //tmp variable for storage
               child2.Clear(); //tmp variable for storage

               double rand = GetRandom(); // get random value

               if (rand >= XProp) // if greater than 0.7, then don't cross
               {
                    child1 = new List<int>(Population[p1]);
                    child2 = new List<int>(Population[p2]);
                    Mutate(ref child1); // mutate
                    Mutate(ref child2); // mutate
                    return;
               }

               for (int i = 0; i < m_size; i++) // cross children with parents
               {
                    if (index >= i)
                    {
                         child1.Add(Population[p1][i]);
                         child2.Add(Population[p2][i]);
                    }
                    else
                    {
                         child1.Add(Population[p2][i]);
                         child2.Add(Population[p1][i]);
                    }
               }

               Mutate(ref child1); // mutate
               Mutate(ref child2); // mutate
          }

          /// <summary>
          /// Roulette wheel spin
          /// </summary>
          /// <param name="fitnesses"></param>
          /// <returns></returns>
          int Spin(ref List<double> fitnesses)
          {
               int i = 0;
               double count = 0;

               double sum = 0;

               fitnesses.ForEach((double d) => { sum += d; });

               List<double> normalized = new List<double>(fitnesses);

               for (i = 0; i < normalized.Count; i++)
                    normalized[i] /= sum;

               double val = GetRandom();

               for (i = 0; i < fitnesses.Count; i++)
               {
                    count += normalized[i];
                    if (count >= val)
                         return i;
               }

               return fitnesses.Count - 1;
          }

          #endregion Methods

     }
     /// <summary>
     /// This guy is used to calculate a piece of the mesh grid surface
     /// </summary>
     public class GAThread
     {
          /// <summary>
          /// Thread Done Event
          /// </summary>
          private ManualResetEvent _doneEvent;

          private Crosser _cross;
          private Mutater _mutate;
          private Spinner _spin;
          private Evaluator _eval;
          private List<List<int>> Pop;
          private List<double> fitnesses;

          /// <summary>
          /// get the resulting grid of points from the thread
          /// </summary>
          public List<List<int>> Result
          {
               get { return Pop; }
               set { Pop = value; }
          }

          public GAThread(ManualResetEvent doneEvent, List<List<int>> pop, List<double> fits, Crosser cross, Mutater mutate, Spinner spin)
          {
               fitnesses = new List<double>(fits);
               Pop = new List<List<int>>(pop);
               _doneEvent = doneEvent;
               _cross = cross;
               _mutate = mutate;
               _spin = spin;
          }

          public GAThread(ManualResetEvent manualResetEvent, List<List<int>> cutPop, List<double> cutFitnesses, Crosser cross, Mutater mutate, Spinner spin, Evaluator Eval)
          {
               // TODO: Complete member initialization
               fitnesses = new List<double>(cutFitnesses);
               Pop = new List<List<int>>(cutPop);
               _doneEvent = manualResetEvent;
               _cross = cross;
               _mutate = mutate;
               _spin = spin;
               _eval = Eval;
          }

          public void ThreadPoolCallback(Object threadContext)
          {
               int threadIndex = (int)threadContext;
               Evolve();
               _doneEvent.Set();
          }

          void Evolve()
          {
               List<int> chosen = new List<int>();
               List<List<int>> children = new List<List<int>>();
               //List<List<int>> elites = new List<List<int>>();
               List<int> child1, child2;
               child1 = new List<int>();
               child2 = new List<int>();
               //Dictionary<double, List<int>> f_c = new Dictionary<double, List<int>>(Pop.Count);
               //List<double> chFit = new List<double>();

               int i;

               for (i = 0; i < Pop.Count; i++)
                    chosen.Add(_spin(ref fitnesses));

               for (i = 1; i < Pop.Count; i += 2)
               {
                    _cross(chosen[i - 1], chosen[i], ref child1, ref child2);
                    children.Add(new List<int>(child1));
                    children.Add(new List<int>(child2));
               }

               //int count = 0;
               //fitnesses.ForEach((double fit) =>
               //{
               //     f_c.Add(fit, Pop[count++]);
               //});
               
               //List<int> dummy = new List<int>();

               //children.ForEach((List<int> chromo) =>
               //     {
               //          chFit.Add(_eval(ref chromo, ref dummy));
               //     });

               //count = 0;
               //chFit.ForEach((double fit) =>
               //{
               //     f_c.Add(fit, children[count++]);
               //});

               //var list = f_c.Keys.ToList();
               //list.Sort();

               //// Loop through keys.
               //Result.Clear();
               //for(i = 0 ; i < children.Count; i++)
               //     Result.Add(new List<int>(f_c[list[i]]));

               Result.Clear();
               children.ForEach((List<int> chromos) =>
               {
               //     //Mutate(ref chromos);
                    Result.Add(new List<int>(chromos));
               });
          }
     }

     public class GA_Event : EventArgs
     {
          delegate void GA_EventHandler(List<int> chromo);

          GA_Event() { }

          public GA_Event(List<int> chromo)
          {
               BestChromo = new List<int>(chromo);
          }

          List<int> m_bestChromo;

          public List<int> BestChromo
          {
               get { return m_bestChromo; }
               set { m_bestChromo = value; }
          }

     }
}
