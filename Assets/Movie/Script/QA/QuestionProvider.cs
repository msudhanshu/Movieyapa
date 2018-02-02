using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using SgUnity;
using System.Collections.Generic;
//using NUnit.Framework.Internal;

namespace SgUnity
{

    /**
	 * 
	 * It will have list of all 
	 * 
	 */

    public class QuestionProvider
    {

        /*
		1. private Dictionary<string,int> questionlevels;
		2. public List<QLevelItemModel> questionsWithLevel { get; set; }
		3. List<KeyValuePair<string,object>>
		4. Dictionary<string,List<object>>
		5. List<Tuple<string, object>>
		6. Lookup   //link:https://msdn.microsoft.com/en-us/library/bb460184.aspx
		*/

        private SgFirebaseDataInitialiser wrapper
        {
            get
            {
                return SgGameManager.Instance.wrapper;
            }
        }



        /** 
		* 1. get random effect , from the all effect list and based on current level , depending upon some random probablity distribution
        * 2. we may want to show new image or shown imaage based on. That will be conrolled from another probabilty distribution.
        * a) if its new level jump play. (new = 100%, old = 0 )
        * b) if he is replaying an old played level  ( new = 5% , old=95%)
        * c) if he failed last level and retrying to finish it... he might have failed at initial stage or at the later stage...
			 ( progress ( 0 to 100 %)
			   new = (100-----50)  , old = 100 - new
			   new = 100-progress/2 , 
			 )

		* 2.1 (if not 2) a) We will try to get any question less than this range 
		*  
		* 
		*     b) Or else when we can't find any question even in lesser range, then  at last we will try to get a question imidiate more than this range
		*    
		* 	   c) Or else just return any shown bloody question again or popup game finish popup or wait(or come again after some day) for more question.

		* 2.1.1 (if 2.1 b)
		* Choose effect again or no effect at all
		*
		*3 (if 2. and new question) : update the in game lists and firebase database user data
		*/



        private int level;
        private CarrierManager.CarrierReplayType replayType;
        private int progressIfRetry = 0;

        public int getQuestion(int level, CarrierManager.CarrierReplayType replayType, int lastProgressIfRetry)
        {
            this.replayType = replayType;
            this.level = level;
            this.progressIfRetry = lastProgressIfRetry;

            //TODO MANJEET HARDCODE
            //QEffectItemModel randEffect = GetRandomEffect();
            //int qid = getRandomQuestion(Mathf.Max(1, this.level - randEffect.max), Mathf.Max(1, this.level - randEffect.min));

            //wrapper.unSolvedQuestionsWithLevel.Find(
            //int qid = getRandomNewQuestion(0, 100);

            if (CarrierManager.GetInstance().testQuestion)
            {
                return CarrierManager.GetInstance().testQuestionId;
            }
            else
                return GetRandom(1, 25, true);
            // return qid;
        }

        //TODO WITH PROBABLITY DISTRUBUTION BASED ON LEVEL
        //DistributedProbabilityModel
        private QEffectItemModel GetRandomEffect()
        {
            List<QEffectItemModel> list = wrapper.qEffectLevel.FindAll(qeffect => level <= qeffect.min && level >= qeffect.level);
            if (list != null)
            {
                int randmax = list.Count;
                int rand = Random.Range(0, randmax);
                return list[rand];
            }
            return null;
        }

        private int getRandomQuestion(int minLevel, int maxLevel)
        {
            if (ShouldShowNewQuestion())
            {
                return getRandomNewQuestion(minLevel, maxLevel);
            }
            else
            {
                return getRandomOldQuestion(minLevel, maxLevel);
            }
        }

        //TODO: OPTIMIZATION SCOPE IF IT IS SORTED 
        private int getRandomNewQuestion(int minLevel, int maxLevel)
        {
            List<QLevelItemModel> l = wrapper.unSolvedQuestionsWithLevel.FindAll(qlevelItem => qlevelItem.level > minLevel && qlevelItem.level < maxLevel);
            QLevelItemModel q = l[GetRandom(0, l.Count)];
            return q.id;
        }

        //TODO : it should exclude even the one which has been seen in this level jump
        private int getRandomOldQuestion(int minLevel, int maxLevel)
        {
            List<QSolvedItemModel> l = wrapper.userSolvedQuestions.FindAll(qlevelItem => qlevelItem.level > minLevel && qlevelItem.level < maxLevel);
            QSolvedItemModel q = l[GetRandom(0, l.Count)];
            return q.id;
        }

        private bool ShouldShowNewQuestion()
        {
            bool isNew = false;
            switch (replayType)
            {
                case CarrierManager.CarrierReplayType.NEW:
                    isNew = isNewBasedOnProbabilityDistribution(95);
                    break;
                case CarrierManager.CarrierReplayType.REPLAY:
                    isNew = isNewBasedOnProbabilityDistribution(5);
                    break;
                case CarrierManager.CarrierReplayType.RETRY:
                    isNew = isNewBasedOnProbabilityDistribution(100 - (int)(progressIfRetry / 2.0f));
                    break;
            }
            return isNew;
        }

        private bool isNewBasedOnProbabilityDistribution(int probForNew)
        {
            int rand = Random.Range(0, 100);
            if (rand <= probForNew) return true;
            return false;
        }

        private int GetRandom(int min, int max, bool maxIncluded = false)
        {
            if (maxIncluded)
                return Random.Range(min, max + 1);
            else
                return Random.Range(min, max);

        }

    }
}