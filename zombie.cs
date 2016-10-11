using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieTest
{
    class RecycleablePool
    {
        private static List<IRecycleable> pool = new List<IRecycleable>();

        public static void Return(IRecycleable obj)
        {
            Console.WriteLine("Pool::Return");

            pool.Add(obj);
        }
        public static IRecycleable Get()
        {
            Console.WriteLine("Pool::Get");

            var obj = pool[0];
            obj.Reset();
            pool.RemoveAt(0);

            return obj;
        }
    }

    interface IRecycleable
    {
        void Reset();
    }

    class Zombie : IRecycleable
    {
        public Zombie()
        {
            Console.WriteLine("Zombie::.ctor");
        }
        ~Zombie()
        {
            Console.WriteLine("Zomble::.dtor");

            GC.SuppressFinalize(this);
            RecycleablePool.Return(this);
        }

        public void Reset()
        {
            GC.ReRegisterForFinalize(this);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var holder = new List<IRecycleable>();

            for(int i = 0; i < 3; i++)
            {
                RecycleablePool.Return(new Zombie());
            }

            holder.Add(RecycleablePool.Get());
            holder.Add(RecycleablePool.Get());
            holder.Add(RecycleablePool.Get());

            holder.Clear();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.Write("EndProgram");
        }
    }
}
