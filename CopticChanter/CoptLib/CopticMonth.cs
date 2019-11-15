using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoptLib
{
    public class CopticMonth : IComparable, IComparable<CopticMonth>, IEquatable<CopticMonth>
    {
        public Months_E EnglishName {
            get;
        }

        public string CopticName {
            get;
        }

        public int Number {
            get;
        }

        public CopticMonth(int num)
        {
            EnglishName = (Months_E)num;
            CopticName = CopticMonths[num];
            
            if (num > 13)
            {
                Number = num % 13;
            }
            else
            {
                Number = num;
            }
        }

        public static string[] CopticMonths = {
            "",
            "Th-oh-o-u-t-", // 1
            "P-a-o-p-i-", // 2
            "A-th-o-r-", // 3
            "Kh-o-i-a-k-", // 4
            "T-oh-b-i-", // 5
            "M-eh-sh-i-r-", // 6
            "P-a-r-eh-m-h-a-t-", // 7
            "Ph-a-r-m-o-th-i-", // 8
            "P-a-sh-a-n-s-", // 9
            "P-a-oh-`-n-i-", // 10
            "Eh-`-p-ee-p-", // 11
            "M-eh-s-oh-r-ee-", // 12
            "P-i-k-o-u-j-i- -n-`-a-`-b-o-t-" // 13
        };

        public enum Months_E
        {
            Tout = 1,
            Baba = 2,
            Hator = 3,
            Kiahk = 4,
            Toba = 5,
            Amshir = 6,
            Baramhat = 7,
            Baramouda = 8,
            Bashans = 9,
            Paona = 10,
            Epep = 11,
            Mesra = 12,
            Nasie = 13
        }

        int IComparable.CompareTo(object compare)
        {
            CopticMonth other;
            try
            {
                other = (CopticMonth)compare;
            }
            catch
            {
                return 1;
            }

            // If other is not a valid object reference, this instance is greater.
            if (other == null) return 1;

            if (Number > other.Number)
            {
                return -1;
            }
            else if (Number == other.Number)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        int IComparable<CopticMonth>.CompareTo(CopticMonth other)
        {
            // If other is not a valid object reference, this instance is greater.
            if (other == null) return 1;

            if (Number > other.Number)
            {
                return -1;
            }
            else if (Number == other.Number)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        bool IEquatable<CopticMonth>.Equals(CopticMonth other)
        {
            if (this == other)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Equals(Object obj)
        {
            CopticMonth other = obj as CopticMonth;
            if (other == null)
                return false;
            else
                return other.Equals(this);
        }

        public override int GetHashCode()
        {
            return this.Number.GetHashCode();
        }

        #region Operators
        public static bool operator >(CopticMonth t1, CopticMonth t2)
        {
            if (t1.Number > t2.Number)
                return true;
            else
                return false;
        }

        public static bool operator <(CopticMonth t1, CopticMonth t2)
        {
            if (t1.Number < t2.Number)
                return true;
            else
                return false;
        }

        public static bool operator >=(CopticMonth t1, CopticMonth t2)
        {
            if (t1.Number >= t2.Number)
                return true;
            else
                return false;
        }

        public static bool operator <=(CopticMonth t1, CopticMonth t2)
        {
            if (t1.Number <= t2.Number)
                return true;
            else
                return false;
        }

        public static bool operator ==(CopticMonth t1, CopticMonth t2)
        {
            if (t1.Number == t2.Number)
                return true;
            else
                return false;
        }

        public static bool operator !=(CopticMonth t1, CopticMonth t2)
        {
            if (t1.Number != t2.Number)
                return true;
            else
                return false;
        }

        public static CopticMonth operator +(CopticMonth t1, CopticMonth t2)
        {
            return new CopticMonth(t1.Number + t2.Number);
        }

        public static CopticMonth operator ++(CopticMonth t1)
        {
            return new CopticMonth(t1.Number + 1);
        }

        public static CopticMonth operator --(CopticMonth t1)
        {
            return new CopticMonth(t1.Number - 1);
        }

        public static CopticMonth operator -(CopticMonth t1, CopticMonth t2)
        {
            return new CopticMonth(t1.Number - t2.Number);
        }
        #endregion
    }
}
