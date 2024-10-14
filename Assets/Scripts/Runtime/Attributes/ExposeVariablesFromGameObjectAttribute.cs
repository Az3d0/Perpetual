using UnityEngine;
using System;

namespace CodeGraph
{
    public class ExposeVariablesFromGameObjectAttribute : Attribute
    {
        private Type m_type;

        public Type Type => m_type;

        /// <summary>
        /// only displays the public fields of the given type of the assigned script
        /// </summary>
        /// <param name="type"></param>
        public ExposeVariablesFromGameObjectAttribute(Type type = null)
        {
            m_type = type;
        }
    }
}
