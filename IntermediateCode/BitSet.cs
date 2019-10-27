using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntermediateCode
{
    public class BitSet
    {
        protected HashSet<object> universe;
        private int[] bits;
        private int nInts;

        public BitSet(HashSet<object> universe)
        {
            this.universe = universe;
            nInts = (universe.Count + 8 * sizeof(int) - 1) / (8 * sizeof(int));
            bits = new int[nInts];
            // Inicialização com conjunto vazio
            for (int i = 0; i < nInts; i++)
                bits[i] = 0;
        }

        public IEnumerator<object> GetEnumerator()
        {
            for (int u = 0; u < universe.Count; u++)
                if (this[u])
                    yield return universe.ElementAt<object>(u);
        }

        public bool Add(object obj)
        {
            for (int pos = 0; pos < universe.Count; pos++)
            {
                // If one is null, but not both, return false.
                if ((universe.ElementAt<object>(pos) == null) || (obj == null))
                    return false;

                if (universe.ElementAt<object>(pos).GetType() != obj.GetType())
                    return false;

                if (universe.ElementAt<object>(pos).Equals(obj))
                {
                    if (this[pos])
                        return false;
                    this[pos] = true;
                    return true;
                }
            }

            return false;
        }

        public bool Remove(object obj)
        {
            for (int pos = 0; pos < universe.Count; pos++)
            {
                if (universe.ElementAt<object>(pos) == obj)
                {
                    if (!this[pos])
                        return false;
                    this[pos] = false;
                    return true;
                }
            }

            return false;
        }

        public int Count
        {
            get
            {
                int count = 0;

                for (int u = 0; u < universe.Count; u++)
                    if (this[u])
                        count++;
                return count;
            }
        }

        public static bool operator==(BitSet lhs, BitSet rhs)
        {
            if ((object)lhs == null && (object)rhs == null)
                return true;
            return lhs.Equals(rhs);
        }
        public static bool operator!=(BitSet s1, BitSet s2)
        { return !(s1 == s2); }

        /// <summary>
        /// Serves as a hash function for the Set type.
        /// </summary>
        /// <returns>an integer code for hashtables.</returns>
        public override int GetHashCode()
        {
            int hash = bits[0];

            for (int i = 1; i < nInts; i++)
                hash ^= bits[i];
            return hash;
        }

        /// <summary>
        /// Overrides the Equals method for Sets, based on the hashcodes of the objects.
        /// </summary>
        /// <param name="obj">an object to be compared to.</param>
        /// <returns>true if the two sets contain the same elements.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != typeof(BitSet))
                return false;
            if (((BitSet)obj).Universe != Universe)
                return false;

            for (int i = 0; i < nInts; i++)
                if (((BitSet)obj).bits[i] != bits[i])
                    return false;

            return true;
        }

        public static BitSet operator +(BitSet s1, BitSet s2)
        { return Union(s1, s2); }

        public static BitSet Union(BitSet s1, BitSet s2)
        {
            if (object.ReferenceEquals(s1.Universe, s2.Universe))
            {
                BitSet tmp = new BitSet(s1.Universe);

                for (int i = 0; i < s1.nInts; i++)
                    tmp.bits[i] = (int)s1.bits[i] | (int)s2.bits[i];
                return tmp;
            }
            return null;
        }

        public static BitSet operator *(BitSet s1, BitSet s2)
        { return Intersection(s1, s2); }

        public static BitSet Intersection(BitSet s1, BitSet s2)
        {
            if (object.ReferenceEquals(s1.Universe, s2.Universe))
            {
                BitSet tmp = new BitSet(s1.Universe);

                for (int i = 0; i < s1.nInts; i++)
                    tmp.bits[i] = (int)s1.bits[i] & (int)s2.bits[i];
                return tmp;
            }
            return null;
        }

        public static bool operator <=(BitSet s1, BitSet s2)
        { return Subset(s1, s2); }

        public static bool operator >=(BitSet s1, BitSet s2)
        { return Subset(s2, s1); }

        public static bool Subset(BitSet s1, BitSet s2)
        {
            if (object.ReferenceEquals(s1.Universe, s2.Universe))
            {
                for (int i = 0; i < s1.nInts; i++)
                    if ((s1.bits[i] & s2.bits[i]) != s1.bits[i])
                        return false;
                return true;
            }
            return false;
        }

        public static BitSet operator -(BitSet s1, BitSet s2)
        { return Subtract(s1, s2); }

        public static BitSet Subtract(BitSet s1, BitSet s2)
        {
            if (object.ReferenceEquals(s1.Universe, s2.Universe))
            {
                BitSet tmp = new BitSet(s1.Universe);

                for (int i = 0; i < s1.nInts; i++)
                    tmp.bits[i] = s1.bits[i] & ~s2.bits[i];
                return tmp;
            }
            return null;
        }

        public static BitSet operator !(BitSet s1)
        { return Complement(s1); }

        public static BitSet Complement(BitSet s1)
        {
            BitSet tmp = new BitSet(s1.Universe);

            for (int i = 0; i < s1.nInts; i++)
                tmp.bits[i] = ~s1.bits[i];
            return tmp;
        }

        public HashSet<object> Universe
        { get { return universe; } }

        public bool this[int i]
        { 
            get {
                int j = i / (8 * sizeof(int));
                int b = i % (8 * sizeof(int));
                return (bits[j] & (1 << b)) != 0;
            }
            set
            {
                int j = i / (8 * sizeof(int));
                int b = i % (8 * sizeof(int));
                if (value)
                    bits[j] |= (1 << b);
                else
                    bits[j] &= ~(1 << b);
            }
        }

        public void SetValue(BitArray val)
        {
            if (val.Length == universe.Count)
                for (int i = 0; i < universe.Count; i++)
                    this[i] = val[i];
        }

        public void SetToOnes()
        {
            for (int i = 0; i < nInts; i++)
                bits[i] = -1;
        }

        public void SetToZeros()
        {
            for (int i = 0; i < nInts; i++)
                bits[i] = 0;
        }

        public override string ToString()
        {
            string str;
            int u;
            int c;

            str = "{";
            for (c = u = 0; u < universe.Count; u++)
            {
                if (this[u])
                {
                    c++;
                    if (c < Count)
                        str += universe.ElementAt<object>(u).ToString() + ", ";
                    else
                        str += universe.ElementAt<object>(u).ToString();
                }
            }
            str += "}";

            return str;
        }

        public string ToBinary()
        { 
            string str = "";
            for (int i = 0; i < universe.Count; i++)
                if (this[i])
                    str += "1";
                else
                    str += "0";
            return str;
        }
    }

    public class BitPowerSet
    {
        protected HashSet<object> universe;
            
        public BitPowerSet(HashSet<object> domain)
        {
            universe = domain;
        }

        public BitSet this[BitArray b]
        {
            get
            {
                BitSet bs = new BitSet(universe);
                for (int i = 0; i < b.Count; i++)
                    bs[i] = b[i];
                return bs;
            }
        }

        public int Count
        { get { return (int) Math.Pow(2, universe.Count); } }

        public IEnumerator<object> GetEnumerator()
        {
            BitArray b = new BitArray(this.Count);
            b.SetAll(false);

            for (int u = 0; u < Count; u++)
            {
                int vaiUm = 1;

                for (int i = 0; i < b.Count; i++)
                {
                    int bit;
                    if (b[i]) bit = 1; else bit = 0;
                    int r = bit + vaiUm;

                    switch (r)
                    {
                        case 1:
                            vaiUm = 0;
                            b[i] = true;
                            break;
                        case 2:
                            vaiUm = 1;
                            b[i] = false;
                            break;
                        case 3:
                            vaiUm = 1;
                            b[i] = true;
                            break;
                    }
                }
                yield return this[b];
            }
        }

        public static bool operator ==(BitPowerSet lhs, BitPowerSet rhs)
        {
            if ((object)lhs == null && (object)rhs == null)
                return true;
            return lhs.Equals(rhs);
        }
        public static bool operator !=(BitPowerSet s1, BitPowerSet s2)
        { return !(s1 == s2); }

        /// <summary>
        /// Serves as a hash function for the Set type.
        /// </summary>
        /// <returns>an integer code for hashtables.</returns>
        public override int GetHashCode()
        {
            return universe.GetHashCode();
        }

        /// <summary>
        /// Overrides the Equals method for Sets, based on the hashcodes of the objects.
        /// </summary>
        /// <param name="obj">an object to be compared to.</param>
        /// <returns>true if the two sets contain the same elements.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != typeof(BitSet))
                return false;
            if (((BitSet)obj).Universe != Universe)
                return false;

            return true;
        }

        public HashSet<object> Universe
        { get { return universe; } }

        public override string ToString()
        {
            string str;

            str = "{";
            if (Count <= 32)
            {
                foreach (BitSet set in this)
                    str += set.ToString() + ",";
            }
            else 
            { 
                BitArray b = new BitArray(this.Count);

                for (int u = 0; u < 16; u++)
                {
                    b[0] = (((u >> 0) & 1) == 1);
                    b[1] = (((u >> 1) & 1) == 1);
                    b[2] = (((u >> 2) & 1) == 1);
                    b[3] = (((u >> 3) & 1) == 1);
                    str += this[b].ToString() + ",";
                }
                str += ", ..., ";
                b.SetAll(true);
                for (int u = 0; u < 16; u++)
                {
                    b[0] = (((u >> 0) & 1) == 1);
                    b[1] = (((u >> 1) & 1) == 1);
                    b[2] = (((u >> 2) & 1) == 1);
                    b[3] = (((u >> 3) & 1) == 1);
                    str += this[b].ToString() + ",";
                }
            }

            str += "}";

            return str;
        }
    }
}
