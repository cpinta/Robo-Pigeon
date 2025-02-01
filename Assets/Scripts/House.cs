using System.Collections.Generic;
using UnityEngine;

public enum HouseState
{
    Idle = 0,
    NewspaperOnSidewalk = 1,
    GettingNewspaper = 2,
    HasNewspaper = 3
}

public class House : MonoBehaviour
{
    Newspaper newspaper;
    [SerializeField] Transform trNewspaperSpot;
    [SerializeField] Transform trPerson;
    [SerializeField] Transform trPersonHand;
    [SerializeField] Transform trPathStart;

    HouseState state = HouseState.Idle;

    float afterHasPaperDelay = 3;
    float afterHasPaperDelayTimer = 0;

    float getAnotherNewspaperDelay = 4;
    float getAnotherNewspaperDelayTimer = 0;

    bool acceptingNewspapers = true;

    List<Transform> pathNodes = new List<Transform>();
    int currentNodeTarget = 0;
    float minNodeDistance = 0.1f;
    float personWalkSpeed = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Transform n = trPathStart;
        pathNodes.Add(n);
        while (n.transform.childCount != 0)
        {
            n = n.transform.GetChild(0).GetComponent<Transform>();
            pathNodes.Add(n);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(newspaper == null)
        {
            state = HouseState.Idle;
        }
        switch (state)
        {
            case HouseState.Idle:
                trPerson.position = transform.position;
                if(getAnotherNewspaperDelayTimer > 0)
                {
                    getAnotherNewspaperDelayTimer -= Time.deltaTime;
                    acceptingNewspapers = false;
                }
                else
                {
                    acceptingNewspapers = true;
                }
                break;
            case HouseState.NewspaperOnSidewalk:
                if (afterHasPaperDelayTimer > 0)
                {
                    afterHasPaperDelayTimer -= Time.deltaTime;
                }
                else
                {
                    GoGetNewspaper();
                }
                break;
            case HouseState.GettingNewspaper:
                float distance = Vector3.Distance(trPerson.position, pathNodes[currentNodeTarget].position);
                if(distance < minNodeDistance)
                {
                    currentNodeTarget++;
                    if(currentNodeTarget >= pathNodes.Count)
                    {
                        GotNewspaper();
                        break;
                    }
                }

                trPerson.position = Vector3.MoveTowards(trPerson.position, pathNodes[currentNodeTarget].position, personWalkSpeed * Time.deltaTime);

                break;
            case HouseState.HasNewspaper:
                distance = Vector3.Distance(trPerson.position, pathNodes[currentNodeTarget].position);
                if (distance < minNodeDistance)
                {
                    currentNodeTarget--;
                    if (currentNodeTarget < 0)
                    {
                        WentBackInside();
                        break;
                    }
                }

                trPerson.position = Vector3.MoveTowards(trPerson.position, pathNodes[currentNodeTarget].position, personWalkSpeed * Time.deltaTime);
                break;
        }
    }

    void GoGetNewspaper()
    {
        state = HouseState.GettingNewspaper;
    }

    void GotNewspaper()
    {
        state = HouseState.HasNewspaper;

        newspaper.transform.parent = trPersonHand;
        newspaper.transform.localPosition = Vector3.zero;
        currentNodeTarget = 3;
    }

    void WentBackInside()
    {
        state = HouseState.Idle;
        getAnotherNewspaperDelayTimer = getAnotherNewspaperDelay;

        trPerson.transform.localPosition = Vector3.zero;
        currentNodeTarget = 0;
        Destroy(newspaper);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!acceptingNewspapers)
        {
            return;
        }
        if(other.gameObject.tag == "Van")
        {
            Van van = other.gameObject.GetComponent<Van>();
            newspaper = van.ThrowNewspaper(trNewspaperSpot);
            state = HouseState.NewspaperOnSidewalk;
            afterHasPaperDelayTimer = afterHasPaperDelay;
            acceptingNewspapers = false;
            currentNodeTarget = 0;
        }
    }
}
