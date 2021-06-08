using System;

namespace deltaTimeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime time1 = DateTime.Now, time2 = DateTime.Now;
            float Checker = 1.5f;
            //double ticks = 0.5;
            // Here we find DeltaTime in while loop
            while (true)
            {
                // This is it, use it where you want, it is time between
                // two iterations of while loop
                time2 = DateTime.Now;
                //long dt = time2.Ticks - time1.Ticks;
                
                float deltaTime = (time2.Ticks - time1.Ticks) / 10000000f;
                if(deltaTime > Checker)
                {
                    Console.WriteLine(deltaTime);  // *float* output {0,2493331}
                    Console.WriteLine("Check : "+Checker); // *int* output {2493331}
                    Checker *= 0.95f;
                    time1 = time2;
                }
                
            }
        }
    }
}
