using System;
using System.Collections.Generic;

namespace Accenture.eviola.Tracking
{
    public interface IHasId { 
        int Id { get; set; }
    }

    public class ScoreWithId : IHasId{
        public int Id { get; set; }
        public float Score = 0;

        public ScoreWithId() { }
        public ScoreWithId(int id, float score) {
            Id = id;
            Score = score;
        }
    }

    public class TrackingUtils {
        /// <summary>
        /// order a list by id, assuming the elements implement IHasId
        /// </summary>
        public static void OrderById<T>(ref List<T> l) where T:IHasId {
            if (l == null) return;
            if (l.Count < 2) return;
            l.Sort((T a, T b) => {
                return a.Id.CompareTo(b.Id);
            });
        }

        /// <summary>
        /// given 2 list of elemnts implementing IHAsId, 
        /// where oldList is an old state and newList the current state,
        /// and given a cost function to determine likeliness that an element in oldList is the same of an element in newList,
        /// it returns a dictionary<int, int> so that the key is an index in newList and the value is an index in oldList and the 2 respective elements identify the same tracked object
        /// </summary>
        public static Dictionary<int, int> TrackFromOldFrame<T>(ref List<T> oldList, ref List<T> newList, Func<T, T, float> costFunc) where T:IHasId{
            Dictionary<int, int> newOldAssociations = new Dictionary<int, int>();
            if (oldList == null || newList == null || oldList.Count < 1 || newList.Count < 1) return newOldAssociations;
            OrderById(ref newList);
            Dictionary<int, List<ScoreWithId>> costMatrix = new Dictionary<int, List<ScoreWithId>>();
            
            foreach (T newElem in newList) {
                costMatrix[newElem.Id] = new List<ScoreWithId>();
                for (int o = 0; o < oldList.Count; o++) {
                    costMatrix[newElem.Id].Add(new ScoreWithId(oldList[o].Id, costFunc(newElem, oldList[o])));
                }
                costMatrix[newElem.Id].Sort((ScoreWithId a, ScoreWithId b) => { return a.Score.CompareTo(b.Score); });

                newOldAssociations[newElem.Id] = costMatrix[newElem.Id][0].Id;
            }
            //TODO: handle conflicts
            return newOldAssociations;
        }
    }
}