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
    GameObject newspaper;
    [SerializeField] Transform trNewspaperSpot;
    [SerializeField] Transform trPerson;
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
                        state = HouseState.HasNewspaper;
                    }
                }

                trPerson.position = Vector3.MoveTowards(trPerson.position, pathNodes[currentNodeTarget].position, personWalkSpeed * Time.deltaTime);

                break;
            case HouseState.HasNewspaper:
                distance = Vector3.Distance(trPerson.position, pathNodes[currentNodeTarget].position);
                if (distance < minNodeDistance)
                {
                    currentNodeTarget++;
                    if (currentNodeTarget >= pathNodes.Count)
                    {
                        state = HouseState.HasNewspaper;
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
    }

    void WentBackInside()
    {
        getAnotherNewspaperDelayTimer = getAnotherNewspaperDelay;
        state = HouseState.Idle;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Van")
        {
            Van van = collision.gameObject.GetComponent<Van>();
            van.ThrowNewspaper(trNewspaperSpot.position);
            state = HouseState.NewspaperOnSidewalk;
            afterHasPaperDelayTimer = afterHasPaperDelay;
            acceptingNewspapers = false;
        }
    }
}
