using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D myRigidbody;
    float moveSpeed =3;

    public float inputAccuracyBase;
    public GameObject prepareText, inputText, evilGuy;
    public float accuracyModifier;

    bool jumping;
    bool jumpInitiated;
    float jumpForce = 7;

    bool inputWindow;
    bool bounce;
    bool beingSlowed;

    //Key Sequence
    bool attemptStarted;
    bool attemptUsed;
    float sequenceInputTime = 1.5f;
    float timer;
    private float timePassed;
    float groundHeight;
    //Create key sequence
    KeyCode[] sequence = new KeyCode[]
    {
        KeyCode.T,
        KeyCode.R,
        KeyCode.W,
        KeyCode.E
    };
    int sequenceIndex = 0;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        //prepareText.SetActive(false);
        inputText.SetActive(false);
        animator = GetComponent<Animator>();
        myRigidbody = gameObject.GetComponent<Rigidbody2D>();
        groundHeight = transform.position.y;
        jumping = false;
    }

    // Update is called once per frame
    void Update()
    {
        

        if (attemptStarted == true)
        {
            timePassed += Time.deltaTime;
        }

        moveForward();

        if(transform.position.y > groundHeight)
        {
            jumping = true;
            moveSpeed = 4.5f;    
        }
        else
        {
            jumping = false;
            moveSpeed = 3;
        }
        
        if(jumping == false)
        {
            jump();
        }

        if (jumpInitiated && bounce)
        {
            myRigidbody.velocity += new Vector2(0, jumpForce);
            jumpInitiated = false;
            
        }
        if (beingSlowed)
        {
            moveSpeed = 1.5f;
        }

        animator.SetBool("Jumping", jumping);
    }


    void moveForward()
    {
        transform.position += new Vector3(moveSpeed*Time.deltaTime, 0);
    }

    void jump()
    {
        if (inputtingSequence())
        {
            if (rollDice())
            {
                jumpInitiated = true;
            }
            
        }
    }

    bool inputtingSequence()
    {
        if (inputWindow && attemptUsed == false)
        {
            if (Input.GetKeyDown(sequence[0])) //If input is the very first key in the sequence, assume user is trying to start sequence from the beginning, and reset timer
            {
                Debug.Log("Sequence Started");
                timePassed = 0;
                attemptStarted = true;
            }

            if (Input.GetKeyDown(sequence[sequenceIndex])) //If the key pressed is the next key we needed to press in the sequence, check that...
            {
                sequenceIndex += 1;
                if (sequenceIndex == sequence.Length) // sequenceIndex is equal to the length of the sequence array, because if it is, we have correctly done the sequence
                {
                    resetSequenceAttempt();
                    Debug.Log("Correct Sequence Entered");
                    attemptStarted = false;
                    attemptUsed = true;
                    return true; //return true, so we can do whatever we need to do if we are using either discrete or continuous input.
                }
            }
            else if (Input.anyKeyDown || timePassed > sequenceInputTime) //else, if we input any other key, or spend too long trying to input the sequence, start over.
            {
                Debug.Log("failed");
                resetSequenceAttempt();
                attemptStarted = false;
            }
        }
        return false;
    }

    void resetSequenceAttempt() //Reset all the stuff we need to check whether correct sequence has been typed.
    {
        sequenceIndex = 0;
        timePassed = 0;
    }

    bool rollDice()
    {
        if (Random.value < inputAccuracyBase + accuracyModifier)
        {
            return true;
        }

        return false;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "inputWindow")
        {
            simulatingStress();
            prepareText.SetActive(false);
            inputText.SetActive(true);
            inputWindow = true;
        }

        if(collision.tag == "bounce")
        {
            bounce = true;
        }

        if (collision.tag == "slow")
        {
            beingSlowed = true;
        }

    }



    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "inputWindow")
        {           
            inputWindow = false;
        }

        if (collision.tag == "bounce")
        {
            bounce = false;
            attemptUsed = false;
            prepareText.SetActive(true);
            inputText.SetActive(false);
        }

        if (collision.tag == "slow")
        {
            beingSlowed = false;
        }
    }

    void simulatingStress()
    {
        if(evilGuy.transform.position.x - transform.position.x < - 4)
        {
            accuracyModifier = 0;
        }
        else if(evilGuy.transform.position.x < transform.position.x)
        {
            accuracyModifier = Random.Range(4, 12)*2;
        }
        else if(evilGuy.transform.position.x - transform.position.x < 4)
        {
            accuracyModifier = Random.Range(12, 20)*2;
        }
        else
        {
            accuracyModifier = Random.Range(20, 30)*2;
        }
    }
}
