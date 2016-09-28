using System;

namespace Straight.Core.Exceptions
{
    public class ModelNotFoundException : SystemException
    {
        public ModelNotFoundException(string message) : base(message)
        {
        }
    }
}