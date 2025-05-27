
namespace UnityUtils
{
    public class Preconditions
    {
        public static void CheckNotNull(object obj, string name)
        {
            if (obj == null)
            {
                throw new System.ArgumentNullException(name);
            }
        }
    }
}
