using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Isometric.Core
{
    public class Research : IEnumerable<Research>
    {
        public string Name { get; set; }

        public float ResearchPointsRequired { get; set; }

        public Research[] Children { get; set; }

        public Research[] RequiredResearches { get; set; } = new Research[0];

        public Action<Player> EndAction { get; set; }



        /// <summary>
        /// testing ctor
        /// </summary>
        public Research() { }

        public Research(string name, float researchPointRequired, Research[] children)
        {
            Name = name;
            Children = children;
            ResearchPointsRequired = researchPointRequired;
        }

        public IEnumerable<Research> GetAllChildren()
        {
            var result = new List<Research>();

            foreach (var child in Children)
            {
                if (!result.Contains(child))
                {
                    result.Add(child);
                }

                result.AddRange(child.GetAllChildren().Where(c => !result.Contains(c)));
            }

            return result;
        }

        public bool Possible(IEnumerable<Research> researchedTechonologies, Research root)
        {
            var researchedTechnologiesArray = researchedTechonologies as Research[] ?? researchedTechonologies.ToArray();

            return !researchedTechnologiesArray.Contains(this)
                   && RequiredResearches.All(researchedTechnologiesArray.Contains)
                   && (this == root || researchedTechnologiesArray.Any(rt => rt.Children.Contains(this)));
        }



        public IEnumerator<Research> GetEnumerator()
        {
            return new List<Research>(GetAllChildren()) { this }.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}