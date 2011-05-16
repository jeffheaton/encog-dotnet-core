namespace Encog.Util
{
    /// <summary>
    /// A pair of objects.
    /// </summary>
    /// <typeparam name="A_TYPE">The type of the first object.</typeparam>
    /// <typeparam name="B_TYPE">The type of the second object.</typeparam>
    public class ObjectPair<A_TYPE, B_TYPE>
    {
        /// <summary>
        /// The first object.
        /// </summary>
        private readonly A_TYPE a;

        /// <summary>
        /// The second object.
        /// </summary>
        private readonly B_TYPE b;

        /// <summary>
        /// Construct an object pair. 
        /// </summary>
        /// <param name="a">The first object.</param>
        /// <param name="b">The second object.</param>
        public ObjectPair(A_TYPE a, B_TYPE b)
        {
            this.a = a;
            this.b = b;
        }

        /// <summary>
        /// The first object.
        /// </summary>
        public A_TYPE A
        {
            get { return a; }
        }

        /// <summary>
        /// The second object.
        /// </summary>
        public B_TYPE B
        {
            get { return b; }
        }
    }
}