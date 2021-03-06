using System;
using System.Collections.Generic;

namespace HelloWorld
{
    // Message class
    class Message
    {
        public int Priority
        { get; set; }
        public double Trans
        { get; set; }
        public double Period
        { get; set; }
        public double Blocking
        { get; set; }
        public double Queueing
        { get; set; }
        public double Response
        { get; set; }
        public Message(string[] param)
        {
            int priority = int.Parse(param[0]);
            double trans = double.Parse(param[1]);
            double period = double.Parse(param[2]);
            Priority = priority;
            Trans = trans;
            Period = period;
            Blocking = 0D;
            Queueing = 0D;
            Response = 0D;
        }
    }
    class Program
    {
		// Method for response time computation
        static int Response_comp(List<Message> messages, int num, double tau)
        {
            for (int i = 0; i < num; ++i)
            {
				// Find Bi
                for (int j = 0; j < num; ++j)
                {
                    if (messages[j].Priority >= messages[i].Priority && messages[j].Trans > messages[i].Blocking)
                    {
                        messages[i].Blocking = messages[j].Trans;
                    }
                }
				// Set Qi = Bi
                messages[i].Queueing = messages[i].Blocking;
                while (true)
                {
					// Compute RHS
                    double rhs = messages[i].Blocking;
                    for (int j = 0; j < num; ++j)
                    {
                        if (messages[j].Priority < messages[i].Priority)
                        {
                            rhs += Math.Ceiling((messages[i].Queueing + tau) / messages[j].Period) * messages[j].Trans;
                        }
                    }
					// If RHS + Ci > Ti -> end the method with value 1
                    if (rhs + messages[i].Trans > messages[i].Period)
                    {
                        return 1;
                    }
					// If Qi == RHS -> break out of the loop
                    if (messages[i].Queueing == rhs)
                    {
                        break;
                    }
					// Otherwise -> set Qi = RHS, continue the loop
                    messages[i].Queueing = rhs;
                }
				// Compute Ri = Qi + Ci
                messages[i].Response = messages[i].Queueing + messages[i].Trans;
            }
			// End the method with value 0
            return 0;
        }
        static void Main(string[] args)
        {
			// File IO
            string[] lines = System.IO.File.ReadAllLines(args[0]);
            int num = int.Parse(lines[0]);
            double tau = double.Parse(lines[1]);
            List<Message> messages = new List<Message>();
            for (int i = 0; i < num; ++i)
            {
                string[] param = lines[i + 2].Trim().Split(" ");
                Message message = new Message(param);
                messages.Add(message);
            }
			// Execution
            if (Response_comp(messages, num, tau) == 0)
            {
				// Output the result
                foreach (Message message in messages)
                {
                    Console.WriteLine("{0:F}", message.Response);
                }
            }
            else
            {
				// If the system is not schedulable, output the error message
                Console.WriteLine("The system is not schedulable!");
            }
        }
    }
}
