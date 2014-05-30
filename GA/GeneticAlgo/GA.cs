using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using GeneticAlgo;
using System.Data;

namespace GA
{
    public delegate double Evaluator(ref List<int> binaryEncoded, ref List<int> bestChromo);
    public delegate void GA_EventHandler(List<int> chromo, double bestFitness, double iteration);
    public delegate int Spinner(ref List<double> fitnesses);
    public delegate void Mutater(ref List<int> chromo);
    public delegate void Crosser(int p1, int p2, ref List<int> child1, ref List<int> child2, ref List<List<int>> pop);


    public enum GAType
    {
        BINARY,
        COMBO
    }

    public class ConicalGA
    {
        public enum GA_Type
        {
            _2D,
            _3D
        };

        GA_Type m_dimension = GA_Type._2D;

        GA_DBDataSet database = new GA_DBDataSet();

        public event GA_EventHandler NewBest;

        Evaluator Eval = null;

        Crosser cross = null;
        Mutater mutate = null;
        Spinner spin = null;

        ManualResetEvent[] GADoneEvents;
        List<GAThread> GA_Threads;
        int GAThreadcount = 1;

        bool useCIGAR = false;

        public ConicalGA() { }

        public ConicalGA(int popSize, int chromoCount, GAType type, bool usecig, GA_Type dimension)
        {
            m_dimension = dimension;
            useCIGAR = usecig;
            m_xProp = 0.7;
            m_mutProp = 1.0 / chromoCount;
            m_size = chromoCount;
            m_type = type;
            m_population = new List<List<int>>(popSize);
            GA_Threads = new List<GAThread>(GAThreadcount);
            GADoneEvents = new ManualResetEvent[GAThreadcount];
            Initialize();
            SetupLambdas();
        }

        private void SetupLambdas()
        {
            cross = ((int p1, int p2, ref List<int> child1, ref List<int> child2, ref List<List<int>> pop) =>
            {
                Cross(p1, p2, ref child1, ref child2, ref pop);
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

        void AddToDB(ref List<int> chromosome, ref double fitness)
        {
            if (chromosome.Count == 0)
                return;

            GeneticAlgo.GA_DBDataSetTableAdapters.BestsTableAdapter regionTableAdapter = new GeneticAlgo.GA_DBDataSetTableAdapters.BestsTableAdapter();
            string chromoString = "";

            chromosome.ForEach(chromo => chromoString += chromo.ToString());
            regionTableAdapter.Insert((float)fitness, chromoString, m_size, m_dimension == GA_Type._2D ? "2D" : "3D");
        }

        public void SetEval(Evaluator function) { Eval = function; }

        public void Begin(string filepath, double stopFitnessValue, string trialType)
        {
            filepath = SetSmartFilePath(filepath, trialType);
            List<double> fitnesses = new List<double>();
            int iterationC = 0, BestIndex = 0, tmp = 0;
            double bestFitness = 0;
            double bestEverFitness = double.MaxValue;
            List<int> BestEverChromo = new List<int>();
            List<int> bestChromo = new List<int>();
            int maxIt = 1000;

            using (StreamWriter sr = new StreamWriter(filepath))
            {
                sr.WriteLine("Max Fitness, Min Fitness, Average Fitness");

                while (iterationC < maxIt && !m_stop)
                {
                    fitnesses.Clear();

                    Population.ForEach((List<int> chromo) => { fitnesses.Add(Eval(ref chromo, ref bestChromo)); });

                    //tmp = 0;

                    bestFitness = fitnesses.Max();

                    if (useCIGAR && iterationC == 0)
                        CheckForBestInDB(ref bestChromo, ref bestFitness, ref bestEverFitness, ref BestEverChromo);

                    if (bestFitness < bestEverFitness)
                    {
                        BestIndex = fitnesses.IndexOf(bestFitness);


                        //CheckBest(ref bestFitness, ref fitnesses, ref BestIndex);

                        if (bestChromo != Population[BestIndex])
                        {
                            bestChromo = Population[BestIndex];
                            if (NewBest != null)
                                NewBest(bestChromo, bestFitness, ((double)iterationC / (double)maxIt));
                        }

                        bestEverFitness = bestFitness;
                        BestEverChromo = new List<int>(bestChromo);
                    }
                    else
                    {
                        List<int> tmpc = null;
                        for (int i = 0; i < Population.Count; i++)
                        {
                             tmpc = new List<int>(BestEverChromo);
                             Mutate(ref tmpc, 0.05);
                             Population[i] = new List<int>(tmpc);
                        }

                        fitnesses.Clear();
                        Population.ForEach((List<int> chromo) => { fitnesses.Add(Eval(ref chromo, ref BestEverChromo)); });
                        bestFitness = fitnesses.Max();
                        if (bestEverFitness < bestFitness)
                        {
                             BestIndex = fitnesses.IndexOf(bestFitness);
                             bestChromo = Population[BestIndex];
                             bestEverFitness = bestFitness;
                        }

                        BestIndex = fitnesses.IndexOf(bestFitness);
                        Population[BestIndex] = BestEverChromo;
                        fitnesses[BestIndex] = bestEverFitness;
                        List<double> tmpFitnesses = new List<double>(fitnesses);
                        tmpFitnesses.Sort();
                        List<int> tmp2 = null;

                        //Initialize();

                        for (int i = 0; i < Population.Count * 0.25; i++)
                        {
                            tmp2 = new List<int>(BestEverChromo);
                            Mutate(ref tmp2, 0.5);
                            Population[fitnesses.IndexOf(tmpFitnesses[i])] = tmp2;
                            fitnesses[fitnesses.IndexOf(tmpFitnesses[i])] = Eval(ref tmp2, ref bestChromo);
                        }

                        //fitnesses.Clear();
                        //Population.ForEach((List<int> chromo) => { fitnesses.Add(Eval(ref chromo, ref bestChromo)); });

                        bestFitness = fitnesses.Max();

                        if (NewBest != null)
                            NewBest(null, bestFitness, ((double)iterationC / (double)maxIt));

                        CheckBest(ref bestFitness, ref fitnesses, ref BestIndex);

                        if (bestChromo != Population[BestIndex])
                        {
                             bestChromo = Population[BestIndex];
                             if (NewBest != null)
                                  NewBest(bestChromo, bestFitness, ((double)iterationC / (double)maxIt));
                        }
                    }

                    sr.WriteLine("{0},{1},{2}", bestFitness, fitnesses.Min(), fitnesses.Average());

                    BestIndex = fitnesses.IndexOf(bestFitness);

                    CheckBest(ref bestFitness, ref fitnesses, ref BestIndex);

                    if (bestChromo != Population[BestIndex])
                    {
                         bestChromo = Population[BestIndex];
                         if (NewBest != null)
                              NewBest(bestChromo, bestFitness, ((double)iterationC / (double)maxIt));
                    }

                    //CheckBest(bestFitness, ref BestIndex);

                    if (bestFitness >= stopFitnessValue)
                    {
                        if (useCIGAR)
                        AddToDB(ref bestChromo, ref bestFitness);
                        m_stop = true;
                    }

                    Evolve(ref fitnesses, false);

                    iterationC++;

                }

                if (iterationC == maxIt /*&& useCIGAR*/) // reached end without converging, save the result for anyway
                    AddToDB(ref bestChromo, ref bestFitness);
            }
        }

        List<int> Begin2( double stopFitnessValue, string trialType)
        {
            //filepath = SetSmartFilePath(filepath, trialType);
            List<double> fitnesses = new List<double>();
            int iterationC = 0, BestIndex = 0, tmp = 0;
            double bestFitness = 0;
            double bestEverFitness = 0;
            List<int> BestEverChromo = new List<int>();
            List<int> bestChromo = new List<int>();
            int maxIt = 1000;

            //using (StreamWriter sr = new StreamWriter(filepath))
            //{
            //sr.WriteLine("Max Fitness, Min Fitness, Average Fitness");

            while (iterationC < maxIt && !m_stop)
            {
                fitnesses.Clear();

                Population.ForEach((List<int> chromo) => { fitnesses.Add(Eval(ref chromo, ref bestChromo)); });

                //tmp = 0;

                bestFitness = fitnesses.Max();

                //if (useCIGAR && iterationC == 0)
                //    CheckForBestInDB(ref bestChromo, ref bestFitness, ref bestEverFitness, ref BestEverChromo);

                //if (bestEverFitness < bestFitness)
                //{
                BestIndex = fitnesses.IndexOf(bestFitness);


                //CheckBest(ref bestFitness, ref fitnesses, ref BestIndex);

                if (bestChromo != Population[BestIndex])
                {
                    bestChromo = Population[BestIndex];
                    if (NewBest != null)
                        NewBest(bestChromo, bestFitness, ((double)iterationC / (double)maxIt));
                }

                bestEverFitness = bestFitness;
                BestEverChromo = new List<int>(bestChromo);
                //}
                //else
                //{
                //List<int> tmpc = null;
                //for (int i = 0; i < Population.Count; i++)
                //{
                //     tmpc = new List<int>(BestEverChromo);
                //     Mutate(ref tmpc, 0.05);
                //     Population[i] = new List<int>(tmpc);
                //}

                //fitnesses.Clear();
                //Population.ForEach((List<int> chromo) => { fitnesses.Add(Eval(ref chromo, ref BestEverChromo)); });
                //bestFitness = fitnesses.Max();
                //if (bestEverFitness < bestFitness)
                //{
                //     BestIndex = fitnesses.IndexOf(bestFitness);
                //     bestChromo = Population[BestIndex];
                //     bestEverFitness = bestFitness;
                //}

                //BestIndex = fitnesses.IndexOf(bestFitness);
                //Population[BestIndex] = BestEverChromo;
                //fitnesses[BestIndex] = bestEverFitness;
                //List<double> tmpFitnesses = new List<double>(fitnesses);
                //tmpFitnesses.Sort();
                //List<int> tmp2 = null;

                ////Initialize();

                //for (int i = 0; i < Population.Count * 0.25; i++)
                //{
                //    tmp2 = new List<int>(BestEverChromo);
                //    Mutate(ref tmp2, 0.5);
                //    Population[fitnesses.IndexOf(tmpFitnesses[i])] = tmp2;
                //    fitnesses[fitnesses.IndexOf(tmpFitnesses[i])] = Eval(ref tmp2, ref bestChromo);
                //}

                ////fitnesses.Clear();
                ////Population.ForEach((List<int> chromo) => { fitnesses.Add(Eval(ref chromo, ref bestChromo)); });

                //bestFitness = fitnesses.Max();

                //if (NewBest != null)
                //    NewBest(null, bestFitness, ((double)iterationC / (double)maxIt));

                //CheckBest(ref bestFitness, ref fitnesses, ref BestIndex);

                //if (bestChromo != Population[BestIndex])
                //{
                //     bestChromo = Population[BestIndex];
                //     if (NewBest != null)
                //          NewBest(bestChromo, bestFitness, ((double)iterationC / (double)maxIt));
                //}
                //}

                //sr.WriteLine("{0},{1},{2}", bestFitness, fitnesses.Min(), fitnesses.Average());

                //BestIndex = fitnesses.IndexOf(bestFitness);

                //CheckBest(ref bestFitness, ref fitnesses, ref BestIndex);

                //if (bestChromo != Population[BestIndex])
                //{
                //     bestChromo = Population[BestIndex];
                //     if (NewBest != null)
                //          NewBest(bestChromo, bestFitness, ((double)iterationC / (double)maxIt));
                //}

                //CheckBest(bestFitness, ref BestIndex);

                if (bestFitness >= stopFitnessValue)
                {
                    //if (useCIGAR)
                    //AddToDB(ref bestChromo, ref bestFitness);
                    m_stop = true;
                }

                Evolve(ref fitnesses, false);

                iterationC++;

            }

            return bestChromo;

            //if (iterationC == maxIt /*&& useCIGAR*/) // reached end without converging, save the result for anyway
            //  AddToDB(ref bestChromo, ref bestFitness);
            //}
        }

        private string SetSmartFilePath(string filepath, string runType)
        {
            //2d/3d
            //  Gaussian/Parabolic/Cubic/Linear/etc.
            //      TrialName.csv
            string newname;
            string path = AutoName(m_dimension.ToString() + "\\" + runType + (useCIGAR ? "_CIG_ON" : "_CIG_OFF"), "Trial.csv", "Trial", out newname);
            return path;
        }

        static public string AutoName(string Directory, string Filename, string fixedName, out string newName)
        {
            int i = 0;
            while (File.Exists(Directory + "\\" + Filename))
            {
                if (i == 0)
                {
                    Filename = Filename.Replace(fixedName, fixedName + "1");
                    i = 1;
                }
                else
                {
                    Filename = Filename.Replace(i.ToString(), (i + 1).ToString());
                    i++;
                }
            }
            if (!System.IO.Directory.Exists(Directory))
                System.IO.Directory.CreateDirectory(Directory);

            newName = Filename;
            return Directory + "\\" + Filename;
        }

        private void CheckForBestInDB(ref List<int> currentBest, ref double fitness, ref double bestEverFitness, ref List<int> BestEverChromo)
        {
            List<List<int>> testChromos = new List<List<int>>();

            GeneticAlgo.GA_DBDataSetTableAdapters.BestsTableAdapter regionTableAdapter = new GeneticAlgo.GA_DBDataSetTableAdapters.BestsTableAdapter();
            GeneticAlgo.GA_DBDataSet.BestsDataTable table = null;
            if (m_dimension == GA_Type._2D)
                table = regionTableAdapter.GetDataBy2D();
            else
                table = regionTableAdapter.GetDataBy3D();

            string tmp = "";
            for (int i = 0; i < table.Rows.Count; i++)
            {
                List<int> chromo = new List<int>();
                if ((int)table.Rows[i][2] == m_size)
                {
                    tmp = table.Rows[i][0].ToString();
                    foreach (char c in tmp)
                    {
                        try { chromo.Add(Convert.ToInt32(c.ToString())); }
                        catch { }
                    }
                    if (chromo.Count == m_size)
                        testChromos.Add(new List<int>(chromo));
                }
            }

            List<double> fitnesses = new List<double>();
            List<int> best = new List<int>();

            if (testChromos.Count == 0)
                return;

            testChromos.ForEach((chromosome) => { if (chromosome.Count > 0) fitnesses.Add(Eval(ref chromosome, ref best)); });

            if (fitness < fitnesses.Max())
            {
                BestEverChromo = testChromos[fitnesses.IndexOf(fitnesses.Max())];
                bestEverFitness = fitnesses.Max();
            }
        }

        private void CheckBest(ref double bestFitness, ref List<double> fitnesses, ref int bestIndex)
        {
            if (IsEqual(bestFitness, oldBestFitness, BestChangeRange))
                bestRangeRepeatCount++;
            else
                oldBestFitness = bestFitness;

            if (bestRangeRepeatCount > RepeatLimit)
            {
                bestRangeRepeatCount = 0;
                if (bestFitness < 0.65)
                    AddNewPoint(ref bestFitness, ref fitnesses, ref bestIndex);
            }
        }

        private void AddNewPoint(ref double bestFitness, ref List<double> fitnesses, ref int bestIndex)
        {
            int tmp = 0;
            List<int> tmpArray = new List<int>();
            List<int> tmpBinary = new List<int>();
            List<int> pop = new List<int>();
            int count = 0;
            string strg;

            //pop = Population[0];
            //tmpBinary = new List<int>(pop.GetRange(70, 4));
            //count = 0;
            //tmp = (int)(16.0 * GetRandom());
            //strg = Convert.ToString(tmp, 2);
            //foreach (char c in strg) { tmpBinary[count++] = c.ToString() == "0" ? 0 : 1; }


            for (int i = 0; i < Population.Count; i++)
            {
                pop = Population[i];
                tmpBinary = new List<int>(pop.GetRange(56, 4));
                tmp = Convert.ToInt32(string.Join("", tmpBinary.ToArray()), 2);

                if (GetRandom() > 0.5)
                    tmp += 1;
                else
                    tmp -= 1;

                if (tmp > 15)
                    tmp -= 1;
                else if (tmp < 2)
                    tmp += 1;

                //ToString().PadLeft(4, '0')
                strg = Convert.ToString(tmp, 2);
                switch (strg.Length)
                {
                    case 1:
                        strg += "000";
                        break;
                    case 2:
                        strg += "00";
                        break;
                    case 3:
                        strg += "0";
                        break;
                }
                //for (int j = 0; j < 5 - strg.Length; j++)
                //     strg += "0";
                strg = ReverseString(strg);
                count = 0;
                //for (int j = tmpBinary.Count-1; j>=0; j--)
                //{
                //     if (strg.ToCharArray().Length > j)
                //         tmpBinary[j] = strg.ToCharArray()[j];
                //}
                foreach (char c in strg) { tmpBinary[count++] = c.ToString() == "0" ? 0 : 1; }

                tmpBinary.Reverse();
                //count = 0;
                //tmp = (int)(16.0 * GetRandom());
                //tmp = Convert.ToInt32().ToString(), 2);
                //strg = Convert.ToString(tmp, 2);
                //count = 0;
                //foreach (char c in strg) { tmpBinary[count++] = c.ToString() == "0" ? 0 : 1; }

                //Mutate(ref pop);
                //Population[i] = pop;

                //for (int j = 0; i < tmpBinary.Count; i++)
                //     tmpBinary[i] = Flip(1.0, tmpBinary[j]);

                count = 0;
                for (int j = 56; j < 56 + tmpBinary.Count; j++)
                    Population[i][j] = Flip(1.0, Population[i][j]);

            }

            List<double> tmpFitnesses = new List<double>();
            List<int> newBest = new List<int>();
            Population.ForEach((p) => { tmpFitnesses.Add(Eval(ref p, ref newBest)); });

            fitnesses = new List<double>(tmpFitnesses);

            bestFitness = fitnesses.Max();

            bestIndex = fitnesses.IndexOf(bestFitness);

            //if (bestChromo != Population[BestIndex])
            //{
            //     bestChromo = Population[bestIndex];
            //if (NewBest != null)
            //    NewBest(Population[bestIndex], bestFitness, (0));
            //}

        }

        public string ReverseString(string s)
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        double oldBestFitness = 0;
        int bestRangeRepeatCount = 0;
        double BestChangeRange = 0.1;
        int RepeatLimit = 100;

        bool IsEqual(double val1, double val2, double Tolerance)
        {
            return Math.Abs(val2 - val1) < Tolerance;
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
            //try
            //{
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
            //}
            //catch (Exception e)
            //{
            //    string message = e.Message;
            //    m_stop = true;
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
            Population.Clear();

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
            for (int i = 0; i < chromo.Count; i++)
                chromo[i] = Flip(MutProp, chromo[i]);
        }

        /// <summary>
        /// mutate binary chromosome
        /// </summary>
        /// <param name="chromo"></param>
        void Mutate(ref List<int> chromo, double chance)
        {
            for (int i = 0; i < chromo.Count; i++)
                chromo[i] = Flip(chance, chromo[i]);
        }

        /// <summary>
        /// swap values in a chromosome
        /// </summary>
        /// <param name="chromo"></param>
        void Swap(ref List<int> chromo)
        {
            int index, index2, tmp;
            int size = chromo.Count;

            index = (int)(size * GetRandom());
            index2 = index + (int)((size - index - 1) * GetRandom());

            if (index == index2)
            {
                if (index2 + 1 < size)
                    index2 += 1;
                else if (index - 1 > 0)
                    index -= 1;
            }

            if (index > 0 && index < size)
                if (index2 > 0 && index2 < size)
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
        private void Cross(int p1, int p2, ref List<int> child1, ref List<int> child2, ref List<List<int>> pop)
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
                child1 = new List<int>(pop[p1]);
                child2 = new List<int>(pop[p2]);
                Mutate(ref child1); // mutate
                Mutate(ref child2); // mutate
                return;
            }

            for (int i = 0; i < m_size; i++) // cross children with parents
            {
                if (index >= i)
                {
                    child1.Add(pop[p1][i]);
                    child2.Add(pop[p2][i]);
                }
                else
                {
                    child1.Add(pop[p2][i]);
                    child2.Add(pop[p1][i]);
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

            List<int> child1, child2;
            child1 = new List<int>();
            child2 = new List<int>();


            int i;

            for (i = 0; i < Pop.Count; i++)
                chosen.Add(_spin(ref fitnesses));

            for (i = 1; i < Pop.Count; i += 2)
            {
                _cross(chosen[i - 1], chosen[i], ref child1, ref child2, ref Pop);
                children.Add(new List<int>(child1));
                children.Add(new List<int>(child2));
            }

            #region Old
            //List<List<int>> elites = new List<List<int>>();
            //Dictionary<double, List<int>> f_c = new Dictionary<double, List<int>>(Pop.Count);
            //List<double> chFit = new List<double>();
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
            #endregion Old

            Result.Clear();
            children.ForEach((List<int> chromos) => { Result.Add(new List<int>(chromos)); });
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
