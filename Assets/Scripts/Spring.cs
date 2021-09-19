using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Spring : MonoBehaviour
{
    // Start is called before the first frame update

    public Vector3 Force;
    public Vector3 Acceleration;
    public Vector3 Velocity;
    public Vector3 PrePosition;
    public Vector3 Position;
    public float mass = 10f;
    public bool isFixed = false;
    public float NaturalLength = 1f;


    public List<Transform> StructParticle = new List<Transform>();
    public int StructParticleIndex = 0;

    public List<Transform> ShearParticle = new List<Transform>();
    public int ShearParticleIndex = 0;


    public Vector3 StructForce;
    public Vector3 ShearForce;
    public Vector3 gravityForce;
    public Vector3 dampingForce;

    public float ABSForce = 0;
    [SerializeField]
    public List<Edge> edges = new List<Edge>();


    private Vector3 mOffset;
    private float mZCoord;

    void Start()
    {
        
        
        StructParticleIndex = 0;
        Force = Vector3.zero;
        Acceleration = Vector3.zero;
        Velocity = Vector3.zero;
        Position = transform.position;
        PrePosition = transform.position;
        //Position += new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
    }

    void OnMouseDown()

    {
        isFixed = true;


        mZCoord = Camera.main.WorldToScreenPoint(

            gameObject.transform.position).z;



        // Store offset = gameobject world pos - mouse world pos

        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();

    }



    private Vector3 GetMouseAsWorldPoint()

    {

        // Pixel coordinates of mouse (x,y)

        Vector3 mousePoint = Input.mousePosition;



        // z coordinate of game object on screen

        mousePoint.z = mZCoord;



        // Convert it to world points

        return Camera.main.ScreenToWorldPoint(mousePoint);

    }



    void OnMouseDrag()

    {

        transform.position = GetMouseAsWorldPoint() + mOffset;

    }

    void OnMouseUp()
    {
        isFixed = false;
    }


}
