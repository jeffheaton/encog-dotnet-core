using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Loader.OpenQuant
{
    public interface IDataSeries : IEnumerable
        {
            // Methods
            void Add(DateTime datetime, object obj);
            void Clear();
            bool Contains(DateTime datetime);
            DateTime DateTimeAt(int index);
            void Flush();
            int IndexOf(DateTime datetime);
            int IndexOf(DateTime datetime, SearchOption option);
            void Remove(DateTime datetime);
            void RemoveAt(int index);
            void Update(DateTime datetime, object obj);
            void Update(int index, object obj);

            // Properties
            int Count { get; }
            string Description { get; set; }
            DateTime FirstDateTime { get; }
            object this[int index] { get; }
            object this[DateTime datetime] { get; set; }
            
            DateTime LastDateTime { get; }
            string Name { get; }
        }

 

 

    
}

