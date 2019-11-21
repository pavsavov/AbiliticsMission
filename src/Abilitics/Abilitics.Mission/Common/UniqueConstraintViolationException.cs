using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abilitics.Mission.Common
{
    public class UniqueConstraintViolationException : Exception
    {
        public UniqueConstraintViolationException()
        {
        }

        public UniqueConstraintViolationException(string message)
            : base(message)
        {
        }

        public UniqueConstraintViolationException(string message, Exception inner)
            : base(message, inner)
        {
            if(inner.Message.Contains("Violation of UNIQUE KEY constraint"))
            {
                Console.WriteLine(message);
                Console.ReadKey();
            }
            else
            {
                throw InnerException;
            }
        }
    }
}
