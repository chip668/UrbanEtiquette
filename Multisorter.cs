using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anzeige
{

    public class Multisorter
    {
        public static bool TestShellSort(int elementCount)
        {
            bool result = false;

            try
            {
                Random rnd = new Random();
                int[] data = new int[elementCount];

                for (int i = 0; i < elementCount; i++)
                {
                    data[i] = rnd.Next(0, 1000000);
                }

                Multisorter.ShellSort(data);

                bool sorted = true;

                for (int i = 1; i < data.Length; i++)
                {
                    if (data[i - 1] > data[i])
                    {
                        sorted = false;
                        break;
                    }
                }

                result = sorted;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Testfehler");
            }

            return result;
        }
        public static void ShellSort<T>(T[] array) where T : IComparable<T>
        {
            int n = array.Length;

            // Gap bestimmen
            for (int gap = n / 2; gap > 0; gap /= 2)
            {
                for (int i = gap; i < n; i++)
                {
                    T temp = array[i];
                    int j = i;

                    // Gap-InsertionSort
                    while (j >= gap && array[j - gap].CompareTo(temp) > 0)
                    {
                        array[j] = array[j - gap];
                        j -= gap;
                    }

                    array[j] = temp;
                }
            }
        }
    }
}
