using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTVechicleMove : MonoBehaviour
{
    [SerializeField] public Rigidbody rb {get; set;}
    private float gravityForce = 5f, dragOnGround = 8f;
    //Lähtölaskenta muuttaa Trueksi
    public bool imGrounded = false;
    public bool wallRiding = false;

    //Kerää näppäinkomennot
    private float speedInput, turnInput;

    [Header("Movement")]
    public float acceleration, reverse, handling;

    [Header("RayCast")]
    //asetetaan Ground Layer ja Wall
    [SerializeField] private LayerMask isGround;
    [SerializeField] private LayerMask isWall;
    //Säteen pituus 
    private float groundRayLength = 3.4f;


    [Header("Vechicle Components")]

    //Renkaiden ja tangon kääntyminen ja pyöriminen
    [SerializeField] private Transform BackWheel;
    [SerializeField] private Transform FrontWheel;
    [SerializeField] private Transform handleBar;
    private float maxWheelTurn = 30f;
    //Kallistuksen vakaukseen
    [SerializeField] private Transform bike;

    private void Start()
    {
        this.rb = GetComponent<Rigidbody>();

    }

    void Update()
    {
        //Kerätään inputit
        //Give speedInput     
        speedInput = 0;
        if (Input.GetAxis("Vertical") > 0)
        {
            speedInput = Input.GetAxis("Vertical") * acceleration * 150;
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            speedInput = Input.GetAxis("Vertical") * reverse * 50;
        }

        //GiveTurnInput
        turnInput = Input.GetAxis("Horizontal");

        //pyörän renkaiden pyöriminen
        FrontWheel.localRotation = Quaternion.Euler((-speedInput * 45 * Time.deltaTime), (turnInput * maxWheelTurn), -90); //-90 koska pivot 3ds maxissa väärin ja pyörän kääntyminen
        BackWheel.localRotation = Quaternion.Euler((-speedInput * 45 * Time.deltaTime), 0, -90);

        //Drifting effect
        transform.position = rb.transform.position;

    }

    private void FixedUpdate()
    {
        //katsotaan ollaanko seinällä ja jos ollaan niin rotaatio
        wallrotation(onaWall());
        //Jos ei olla seinällä,Pyörä vakautetaan
        if (!wallRiding)
        {
            stabilize();
        }
        //itse ajaminen ensin palautetaan drag
        rb.drag = dragOnGround;
        //ollaanko maassa
        imGrounded = groundCheck();
        //jos lähtölaskenta on loppunut ja ollaan maassa
        if (imGrounded || wallRiding)
        {
            //ajaminen
            GiveSpeedInput(speedInput);
            GiveTurnInput(turnInput);

        }
        else if (imGrounded == false)
        {
            IncreaseGravity();
        }


    }

    #region metodit
    private void GiveSpeedInput(float _speedInput)
    {
        if (Mathf.Abs(speedInput) > 0)
        {
            //rb.AddRelativeForce(transform.forward * speedInput,ForceMode.Force);
            rb.AddForce(transform.forward * speedInput);
        }

    }
    private void IncreaseGravity()
    {
        rb.drag = 0.1f;
        rb.AddForce(Vector3.up * -gravityForce * 15f);
    }

    private void GiveTurnInput(float _turnInput)
    {

        //Pyörän kääntyminen, vaatii myös verticaalista liikettä eli paikallaan ei voi kääntyä

        handleBar.localRotation = Quaternion.Euler(handleBar.localRotation.eulerAngles.x, (turnInput * maxWheelTurn / 2), handleBar.localRotation.eulerAngles.z); //-90 koska pivot 3ds maxissa väärin
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * handling * Time.deltaTime, 0f));
    }

    public bool groundCheck()
    {
        //Maasäde
        Ray ray = new Ray(transform.position,  transform.up * -1.0f);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, groundRayLength, (isGround | isWall)))
        {
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
            return true;
        }
        else
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 3.5f, Color.green);
            return false;
        }
    }
    //Ollaanko seinällä tarkastus
    public Tuple<bool, string> onaWall()
    {
        //Luodaan kaksi RaytaYksivasemmalle yksi oikealle
        Ray rayL = new Ray(transform.position, bike.transform.TransformVector(Vector3.left));
        Ray rayR = new Ray(transform.position, bike.transform.TransformVector(Vector3.right));
        RaycastHit hitL;
        RaycastHit hitR;
        if (Physics.Raycast(rayL, out hitL, 3.5f, isWall))
        {
            //luodaan tuple jossa ilmoitetaan että vasen seinä osuu
            Debug.DrawLine(rayL.origin, hitL.point, Color.red);
            var tupleL = new Tuple<bool, string>(true, "left");
            wallRiding = true;
            return tupleL;
        }
        else if (Physics.Raycast(rayR, out hitR, 3.5f, isWall))
        {
            //luodaan tuple jossa ilmoitetaan että oikea seinä osuu
            Debug.DrawLine(rayR.origin, hitR.point, Color.red);
            var tupleR = new Tuple<bool, string>(true, "right");
            wallRiding = true;
            return tupleR;
        }
        else
        {
            //ei osumaa
            Debug.DrawLine(rayL.origin, rayL.origin + rayL.direction * 1.5f, Color.green);
            Debug.DrawLine(rayR.origin, rayR.origin + rayR.direction * 1.5f, Color.green);
            wallRiding = false;
            return new Tuple<bool, string>(false, string.Empty);
        }



    }
    //Käännetään ajoneuvoa seinän mukaisesti otetaan vastaan tuple
    public void wallrotation(Tuple<bool, string> _amIOnAWall)
    {
        //ei osumaa seinään palataan takaisin
        if (_amIOnAWall.Item1 == false) { return; }
        //jos osutaan vasempaan seinään
        if (_amIOnAWall.Item1 == true && _amIOnAWall.Item2.Equals("left"))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, -20f * Input.GetAxis("Vertical")), Time.deltaTime * 8);
        }
        //osutaan oikeaan seinään
        else
        {   
            Quaternion initialRot = transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, initialRot * Quaternion.Euler(0, 0, 15f* Input.GetAxis("Vertical")), Time.deltaTime * 8);
        }

    }
    //pyörän vakain
    private void stabilize()
    {
        //korjaa 50 asteen kulmasta atm
        if (Vector3.Angle(Vector3.up, bike.up) < 90)
        {
            bike.rotation = Quaternion.Slerp(bike.rotation, Quaternion.Euler(0, bike.rotation.eulerAngles.y, 0), Time.deltaTime * 5);
        }
    }
    #endregion

}
