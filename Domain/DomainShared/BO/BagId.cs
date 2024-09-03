using System;

namespace Archiv10.Domain.Shared.BO
{
    public class BagId
    {
        private readonly string _id;
        public BagId(string guid)
        {
            if (string.IsNullOrWhiteSpace(guid))
                throw new ArgumentException("id");

            this._id = guid;
        }

        public string Guid { get { return _id; } }
        public override string ToString()
        {
            return _id;
        }


        public static BagId CreateId()
        {
            return new BagId(System.Guid.NewGuid().ToString("D").ToLower());
        }
    }
}