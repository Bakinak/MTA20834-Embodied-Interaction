using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D myRigidbody;
    public float moveSpeed;

    public float inputAccuracyBase;
    float accuracyModifier;

    bool jumping;
    bool jumpInitiated;
    public float jumpForce;

    //Key Sequence
    bool attemptStarted;
    float sequenceInputTime = 1.5f;
    float timer;
    float delayTime;
    float inputDelay;
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


    // Start is called before the first frame update
    void Start()
    {
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
        }
        else
        {
            jumping = false;
        }
        
        if(jumping == false)
        {
            jump();
        }

        if (jumpInitiated)
        {
            delayTime += Time.deltaTime;
            if(delayTime > inputDelay)
            {
                myRigidbody.velocity += new Vector2(0, jumpForce);
                delayTime = 0;
                jumpInitiated = false;
            }
        }
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
                return true; //return true, so we can do whatever we need to do if we are using either discrete or continuous input.
            }
        }
        else if (Input.anyKeyDown || timePassed > sequenceInputTime) //else, if we input any other key, or spend too long trying to input the sequence, start over.
        {
            Debug.Log("failed");
            resetSequenceAttempt();
            attemptStarted = false;
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
            inputDelay = Random.value;
            return true;
        }

        return false;
    }

}
