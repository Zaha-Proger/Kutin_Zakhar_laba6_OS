﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muxanov_Kutin_laba6_OS
{
    public class OperationMemory
    {
        public static readonly List<Process> Processes = new List<Process>();
        public static readonly int size = 64;
        public static readonly int[] memory = new int[size];

        public OperationMemory()
        {
            for (int i = 0; i < size; i++)
            {
                memory[i] = -1;
            }
        }

        public static Process getProcByID(int id)
        {
            for (int i = 0; i < size; i++)
            {
                if (id == Processes[i].id) return Processes[i];
            }
            return null;
        }

        public void addProcess(Process newProcess)
        {
            Processes.Add(newProcess);
            int startIndex = findFreeSpace(newProcess);
            if (startIndex == -1)
            {
                startIndex = moveToShMemory(newProcess);
                if (startIndex == -1)
                {
                    Console.WriteLine("Not enough memory");
                    return;
                }
                for (int i = startIndex; i < startIndex + newProcess.size; i++)
                {
                    memory[i] = newProcess.id;
                }
            }
            else
            {
                for (int i = startIndex; i < startIndex + newProcess.size; i++)
                {
                    memory[i] = newProcess.id;
                }
            }
            Processes[Processes.Count-1].startIndex = startIndex;
            Processes[Processes.Count-1].typeOfMemory = 1;
            Processes[Processes.Count-1].state = "Running";
        }

        static public void addProcess(int num)
        {
            int startIndex = findFreeSpace(getProcByID(num));
            if (startIndex == -1)
            {
                startIndex = moveToShMemory(getProcByID(num));
                if (startIndex == -1)
                {
                    Console.WriteLine("Невозможно добавить новый процесс, недостаточно памяти");
                    return;
                }
                for (int i = startIndex; i < startIndex + getProcByID(num).size; i++)
                {
                    memory[i] = getProcByID(num) == null ? -1 : getProcByID(num).id;
                }
            }
            else
            {
                for (int i = startIndex; i < startIndex + getProcByID(num).size; i++)
                {
                    memory[i] = getProcByID(num) == null ? -1 : getProcByID(num).id;
                }
            }
            Processes[Processes.Count-1].startIndex = startIndex;
            Processes[Processes.Count-1].typeOfMemory = 1;
        }

        private static int findFreeSpace(Process newProcess)
        {
            int startIndex = -1;
            int freeMemorySize = 0;
            for (int i = 0; i < 64; i++)
            {
                if (memory[i] == -1)
                {
                    if (startIndex == -1)
                    {
                        startIndex = i;
                    }
                    freeMemorySize++;
                }
                else if (memory[i] != -1 && freeMemorySize < newProcess.size)
                {
                    freeMemorySize = 0;
                    startIndex = -1;
                }
            }
            return freeMemorySize >= newProcess.size ? startIndex : -1;
        }

        private static int moveToShMemory(Process newProcess)
        {
            int startIndex = -1;
            foreach (Process Process in Processes)
            {
                if (Process.state.Equals("Wait") && Process.typeOfMemory == 1)
                {
                    if (Process.size >= newProcess.size)
                    {
                        startIndex = Process.startIndex;
                        for (int i = startIndex; i < startIndex + Process.size; i++)
                        {
                            memory[i] = -1;
                        }
                        Process.typeOfMemory = 2;
                        Process.startIndex = -1;
                    }
                    else
                    {
                        int freeMemory = Process.size;
                        for (int i = Process.startIndex + Process.size; i < size; i++)
                        {
                            if (memory[i] == -1)
                            {
                                freeMemory += 1;
                            }
                            else
                            {
                                break;
                            }
                            if (freeMemory >= newProcess.size)
                            {
                                break;
                            }
                        }
                        if (freeMemory >= newProcess.size)
                        {
                            startIndex = Process.startIndex;
                            for (int i = startIndex; i < startIndex + Process.size; i++)
                            {
                                memory[i] = -1;
                            }
                            Process.typeOfMemory = 2;
                            Process.startIndex = -1;
                        }
                    }

                }

            }
            return startIndex;
        }

        public void deleteFromOpMemory(int id)
        {


            if (id <= Processes.Count && id >= 0)
            {
                foreach (Process Process in Processes)
                {
                    if (Process.id == id)
                    {
                        for (int i = Process.startIndex; i < Process.startIndex + Process.size; i++)
                        {
                            memory[i] = -1;
                        }
                    }
                }
                Processes.Remove(Processes.First<Process>(x => x.id == id));
            }
            //Iterator var2 = proceses.iterator();

            //while (true)
            //{
            //    Process process;
            //    do
            //    {
            //        if (!var2.hasNext())
            //        {
            //            Processes.Remove(Processes.First<Process>(x => {
            //                return x.id == id;
            //            });
            //            return;
            //        }

            //        procces = (Procces)var2.next();
            //    } while (procces.id != id);

            //    for (int i = procces.startIndex; i < procces.startIndex + procces.size; ++i)
            //    {
            //        memory[i] = -1;
            //    }
            //}
        }

        public void showStatusMemory()
        {
            Console.Clear();

            for (int i = 0; i < 55; i++) Console.Write("_");
            Console.WriteLine();
            Console.WriteLine("| id |    State    | St. index | Size | Type of memory |\n");
            //Console.WriteLine("| %2s | %10s | %2s | %2s | %2s |\n", "id", "State", "St. index", "Size", "Type of memory");
            for (int i = 0; i < 55; i++) Console.Write("-");
            Console.WriteLine();
            foreach (Process Process in Processes)
            {
                Console.WriteLine("|  {0} |    {1}    |    {2}     |  {3}  |        {4}       |\n", Process.id, Process.state, Process.startIndex, Process.size, Process.typeOfMemory);
                for (int i = 0; i < 55; i++) Console.Write("-");
                Console.WriteLine();
            }
            Console.WriteLine("Type of memory: 1 - RAM, 2 - ROM");
            this.countFreeSpace();
        }

        public void pauseProcess(int id)
        {
            if (id <= Processes.Count && id >= 0)
            {
                getProcByID(id).pauseProcess();
            }
        }

        public void countFreeSpace()
        {
            int line = 0;
            int maxline = 0;
            int all = 0;

            for (int i = 0; i < 64; ++i)
            {
                if (memory[i] == -1)
                {
                    ++line;
                    ++all;
                }

                if (memory[i] != -1)
                {
                    line = 0;
                }

                maxline = Math.Max(maxline, line);
            }

            Console.WriteLine("Free memory: " + all + "KB, " + maxline + "KB for one process");
        }
    }
}
