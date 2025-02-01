using System.Collections.Generic;
using UnityEngine;

public enum HouseState
{
    Idle = 0,
    NewspaperOnSidewalk = 1,
    GettingNewspaper = 2,
    PickingUpNewspaper = 3,
    HasNewspaper = 4,
    Pissed = 5
}

public class House : MonoBehaviour
{
    Newspaper newspaper;
    [SerializeField] Transform trNewspaperSpot;
    [SerializeField] Transform trPerson;
    [SerializeField] char1 charPerson;
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

    float yellingTime = 3;
    float yellingTimer = 0;

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
                charPerson.SetIsWalking(false);
                charPerson.SetIsPickingUp(false);
                charPerson.SetIsYelling(false);
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
                charPerson.SetIsWalking(false);
                charPerson.SetIsPickingUp(false);
                charPerson.SetIsYelling(false);
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
                charPerson.SetIsWalking(true);
                charPerson.SetIsPickingUp(false);
                charPerson.SetIsYelling(false);
                trPerson.position = Vector3.MoveTowards(trPerson.position, pathNodes[currentNodeTarget].position, personWalkSpeed * Time.deltaTime);

                break;
            case HouseState.PickingUpNewspaper:
                charPerson.SetIsWalking(false);
                charPerson.SetIsPickingUp(true);
                charPerson.SetIsYelling(false);
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

                charPerson.SetIsWalking(true);
                charPerson.SetIsPickingUp(false);
                charPerson.SetIsYelling(false);
                trPerson.position = Vector3.MoveTowards(trPerson.position, pathNodes[currentNodeTarget].position, personWalkSpeed * Time.deltaTime);
                break;
            case HouseState.Pissed:
                charPerson.SetIsWalking(false);
                charPerson.SetIsPickingUp(false);
                charPerson.SetIsYelling(true);

                if (yellingTime > 0)
                {
                    yellingTimer -= Time.deltaTime;
                }
                else
                {
                    state = HouseState.Idle;
                }
                break;
        }
    }

    void GoGetNewspaper()
    {
        trPerson.LookAt(new Vector3(trNewspaperSpot.position.x, trPerson.position.y, trNewspaperSpot.position.z), Vector3.up);
        state = HouseState.GettingNewspaper;
    }

    void GotNewspaper()
    {
        state = HouseState.HasNewspaper;

        newspaper.transform.parent = trPersonHand;
        newspaper.transform.localPosition = Vector3.zero;
        currentNodeTarget = 3;
        trPerson.LookAt(new Vector3(transform.position.x, trPerson.position.y, transform.position.z), Vector3.up);
    }

    void WentBackInside()
    {
        state = HouseState.Idle;
        getAnotherNewspaperDelayTimer = getAnotherNewspaperDelay;

        trPerson.transform.localPosition = Vector3.zero;
        currentNodeTarget = 0;
        Destroy(newspaper);
    }

    void Pissed()
    {
        state = HouseState.Pissed;
        yellingTimer = yellingTime;
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
            newspaper.snatchedNewspaper.AddListener(Pissed);
            state = HouseState.NewspaperOnSidewalk;
            afterHasPaperDelayTimer = afterHasPaperDelay;
            acceptingNewspapers = false;
            currentNodeTarget = 0;
        }
    }
}
