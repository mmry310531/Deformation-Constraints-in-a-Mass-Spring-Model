using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[System.Serializable] public struct Edge
{
    public float Length;
    public float StartLength;
    public Transform p1;
    public Transform p2;
    public bool SuperElongation;

};


public class Binding : MonoBehaviour
{


    public int row = 8;
    public int col = 16;
    public float Clock = 0;
    public float deltaTime = 1f;
    public int iteration = 10;

    [Header("Spring Attribute")]
    public float NaturalLength = 1f;
    public float DeformRate = 0.01f;
    public float stiffness = 5f;
    public Vector3 GotForce;
    public Vector3 ABSForce;
    public Transform[] p;
    public float dampingCoff = 1f;
    public bool AddShearForce = true;
    
    [SerializeField]
    public List<Edge> StructEdges = new List<Edge>();
    public List<Edge> ShearEdges = new List<Edge>();

    // Start is called before the first frame update
    void Start()
    {
        p = new Transform[row * col];
        InitStructParticle();
        
    }

    void FixedUpdate()
    {
        //BindingParticle();
        ComputeAllPosition();
        for(int i = 0; i < iteration; i++)
        {
            FixAllPosition();
        }
        FixAllVelocity();
        

        //SuperElasticPosition();
    }


    void InitStructParticle()
    {
        for (int i = 0; i < row; ++i)
        {
            for (int j = 0; j < col; ++j)
            {
                p[i * col + j] = transform.GetChild(i * col + j);
            }
        }

        for (int i = 0; i < row; ++i)
        {

            for (int j = 0; j < col; ++j)
            {
                if (j != col - 1)
                {
                    Edge edge;
                    edge.p1 = p[i * col + j];
                    edge.p2 = p[i * col + j+1];
                    edge.Length = (edge.p1.position - edge.p2.position).magnitude;
                    edge.StartLength = (edge.p1.position - edge.p2.position).magnitude;
                    edge.SuperElongation = false;
                    StructEdges.Add(edge);
                    p[i * col + j].GetComponent<Spring>().edges.Add(edge);
                    p[i * col + j + 1].GetComponent<Spring>().edges.Add(edge);
                }

                //
                // Structural particle
                //
                if (j == 0)
                {
                    //m2 = p[i * col + 1];
                    AddStructParticle(p[i * col + j], p[i * col + 1]);

                }
                else if (j == col - 1)
                {
                    //m2 = p[i * col + (j-1)];
                    AddStructParticle(p[i * col + j], p[i * col + (j - 1)]);
                }
                else
                {
                    //m2 = transform.GetChild(i * col + (j - 1));
                    AddStructParticle(p[i * col + j], p[i * col + (j - 1)]);
                    //m2 = transform.GetChild(i * col + (j + 1));
                    AddStructParticle(p[i * col + j], p[i * col + (j + 1)]);

                }

                // deal col mass
                if (i == 0)
                {
                    //m2 = transform.GetChild(1 * col + j);
                    AddStructParticle(p[i * col + j], p[(1 * col + j)]);
                }
                else if (i == row - 1)
                {
                    //m2 = transform.GetChild((i - 1) * col + j);
                    AddStructParticle(p[i * col + j], p[(i - 1) * col + j]);
                }
                else
                {
                    //m2 = transform.GetChild((i - 1) * col + j);
                    AddStructParticle(p[i * col + j], p[(i - 1) * col + j]);
                    // m2 = transform.GetChild((i + 1) * col + j);
                    AddStructParticle(p[i * col + j], p[(i + 1) * col + j]);
                }


                // Shear Particle
                if (AddShearForce)
                {
                    if (i == 0 && j == 0)
                    {
                        AddShearParticle(p[i * col + j], p[(i + 1) * col + (j + 1)]);
                    }
                    else if (i == 0 && (j == col - 1))
                    {
                        AddShearParticle(p[i * col + j], p[(i + 1) * col + (j - 1)]);
                    }
                    else if ((i == row - 1) && j == 0)
                    {
                        AddShearParticle(p[i * col + j], p[(i - 1) * col + (j + 1)]);
                    }
                    else if ((i == row - 1) && (j == col - 1))
                    {
                        AddShearParticle(p[i * col + j], p[(i - 1) * col + (j - 1)]);
                    }
                    else if (i == 0)
                    {
                        AddShearParticle(p[i * col + j], p[(i + 1) * col + (j + 1)]);
                        AddShearParticle(p[i * col + j], p[(i + 1) * col + (j - 1)]);
                    }
                    else if (i == row - 1)
                    {
                        AddShearParticle(p[i * col + j], p[(i - 1) * col + (j + 1)]);
                        AddShearParticle(p[i * col + j], p[(i - 1) * col + (j - 1)]);
                    }
                    else if (j == 0)
                    {
                        AddShearParticle(p[i * col + j], p[(i + 1) * col + (j + 1)]);
                        AddShearParticle(p[i * col + j], p[(i - 1) * col + (j + 1)]);
                    }
                    else if (j == col - 1)
                    {
                        AddShearParticle(p[i * col + j], p[(i + 1) * col + (j - 1)]);
                        AddShearParticle(p[i * col + j], p[(i - 1) * col + (j - 1)]);
                    }
                    else
                    {
                        AddShearParticle(p[i * col + j], p[(i + 1) * col + (j + 1)]);
                        AddShearParticle(p[i * col + j], p[(i - 1) * col + (j + 1)]);
                        AddShearParticle(p[i * col + j], p[(i + 1) * col + (j - 1)]);
                        AddShearParticle(p[i * col + j], p[(i - 1) * col + (j - 1)]);
                    }
                }
                

            }
        }
        //Debug.Log("row : " + row);
        //Debug.Log("col : " + col);

        for(int j = 0; j < col; j++)
        {
            for(int i = 0; i < row; i++)
            {
                if(i != row - 1)
                {
                    Edge edge;
                    edge.p1 = p[i * col + j];
                    edge.p2 = p[(i+1) * col + j];
                    edge.Length = (edge.p1.position - edge.p2.position).magnitude;
                    edge.StartLength = (edge.p1.position - edge.p2.position).magnitude;
                    edge.SuperElongation = false;
                    StructEdges.Add(edge);
                    p[i * col + j].GetComponent<Spring>().edges.Add(edge);
                    p[(i+1) * col].GetComponent<Spring>().edges.Add(edge);
                }
                
            }
        }
        if (AddShearForce)
        {
            for (int i = 0; i < row - 1; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (j != col - 1)
                    {
                        Edge edge;
                        edge.p1 = p[i * col + j];
                        edge.p2 = p[(i + 1) * col + (j + 1)];
                        edge.Length = (edge.p1.position - edge.p2.position).magnitude;
                        edge.StartLength = (edge.p1.position - edge.p2.position).magnitude;
                        edge.SuperElongation = false;
                        ShearEdges.Add(edge);
                        p[i * col + j].GetComponent<Spring>().edges.Add(edge);
                        p[(i + 1) * col + (j + 1)].GetComponent<Spring>().edges.Add(edge);
                    }
                    if (j != 0)
                    {
                        Edge edge;
                        edge.p1 = p[i * col + j];
                        edge.p2 = p[(i + 1) * col + (j - 1)];
                        edge.Length = (edge.p1.position - edge.p2.position).magnitude;
                        edge.StartLength = (edge.p1.position - edge.p2.position).magnitude;
                        edge.SuperElongation = false;
                        ShearEdges.Add(edge);
                        p[i * col + j].GetComponent<Spring>().edges.Add(edge);
                        p[(i + 1) * col + (j - 1)].GetComponent<Spring>().edges.Add(edge);
                    }

                }
            }
        }
        
    }



    //void BindingParticle()
    //{
        
    //    // second Force
    //    // third Force (no need)
    //    // Gravity();
    //    // Wind()
    //    //Damping();
    //}

    //void StructForce(Transform p)
    //{
    //    Spring SpringCS = p.GetComponent<Spring>();

    //    foreach(Transform p1 in SpringCS.StructParticle)
    //    {
    //        AddSpring(p, p1);
    //    }
    //}

    void AddSpring(Transform p1, Transform p2)
    {
        Vector3 Vab = p2.position - p1.position;
        //Debug.Log(Vab.magnitude);
        p1.GetComponent<Spring>().Force += stiffness * (Vab - NaturalLength * (Vab.normalized));
        
        //Debug.Log(m2);
    }

    void AddStructParticle(Transform p1, Transform p2)
    {
        Spring SpringCS = p1.GetComponent<Spring>();
        SpringCS.StructParticle.Add(p2);
        SpringCS.StructParticleIndex += 1;

    }

    void AddShearParticle(Transform p1, Transform p2)
    {
        Spring SpringCS = p1.GetComponent<Spring>();
        SpringCS.ShearParticle.Add(p2);
        SpringCS.ShearParticleIndex += 1;
    }
    void ComputeAllPosition()
    {
        for(int i = 0; i < row; ++i)
        {
            for(int j = 0; j < col; ++j)
            {
                UpdatePerPosition(p[i * col + j]);
            }
        }
    }

    void UpdatePerPosition(Transform p)
    {
        
        Spring SpringCS = p.GetComponent<Spring>();
        if (SpringCS.isFixed)
        {
            return;
        }


        SpringCS.Force = Vector3.zero;
        SpringCS.ABSForce = 0f;

        SpringCS.StructForce = Vector3.zero;
        SpringCS.ShearForce = Vector3.zero;
        SpringCS.gravityForce = Vector3.zero;
        SpringCS.dampingForce = Vector3.zero;

        //Struct
        foreach (Transform p1 in SpringCS.StructParticle)
        {
            if (p1 == null)
            {
                break;
            }
            //AddSpring(p, p1);
            Vector3 Vab = p1.position - p.position;
            //Debug.Log(Vab.magnitude);
            SpringCS.Force += stiffness * (Vab - NaturalLength * (Vab.normalized));
            SpringCS.StructForce += stiffness * (Vab - NaturalLength * (Vab.normalized));
            SpringCS.ABSForce += Mathf.Abs(SpringCS.Force.magnitude);
        }

        // Shear
        if (AddShearForce)
        {
            foreach (Transform p1 in SpringCS.ShearParticle)
            {
                if (p1 == null)
                {
                    break;
                }
                Vector3 Vab = p1.position - p.position;
                //Debug.Log(Vab.magnitude);
                SpringCS.Force += stiffness * (Vab - NaturalLength * (Vab.normalized));
                SpringCS.ShearForce += stiffness * (Vab - NaturalLength * (Vab.normalized));
                SpringCS.ABSForce += Mathf.Abs(SpringCS.Force.magnitude);
            }
        }



        //Damping
        SpringCS.Force -= SpringCS.Velocity * dampingCoff;
        SpringCS.dampingForce -= SpringCS.Velocity * dampingCoff;
        SpringCS.ABSForce += Mathf.Abs(SpringCS.Velocity.magnitude * dampingCoff);

        //Gravity
        SpringCS.Force += new Vector3(0, -9.81f, 0);
        SpringCS.gravityForce += new Vector3(0, -9.81f, 0);
        SpringCS.ABSForce += Mathf.Abs(-9.81f);


        SpringCS.PrePosition = SpringCS.Position;

        SpringCS.Acceleration = SpringCS.Force / SpringCS.mass;
        SpringCS.Velocity += SpringCS.Acceleration * Time.deltaTime  * deltaTime;
        SpringCS.Position += SpringCS.Velocity * Time.deltaTime  * deltaTime;
        //FixPosition(p);

        //SpringCS.Velocity = Vector3.zero;




        //p.transform.position = SpringCS.Position;

    }

    void FixAllPosition()
    {
        

        for(int i=0;i<StructEdges.Count;i++)
        {
            

            bool SuperElongation = false;
            Spring p1 = StructEdges[i].p1.GetComponent<Spring>();
            Spring p2 = StructEdges[i].p2.GetComponent<Spring>();

            Vector3 dir = (p2.Position - p1.Position);
            float dis = dir.magnitude;
            
            

            if (dis >= NaturalLength * (1+DeformRate)) 
            {
                SuperElongation = true;
                //Debug.Log("deform rate higher than expect");
                if (p1.isFixed && p2.isFixed)
                {
                    continue;
                }
                else if (p1.isFixed)
                {
                    StructEdges[i].p2.position = StructEdges[i].p1.position + (1+DeformRate) * NaturalLength * dir.normalized;
                    p2.Position = StructEdges[i].p2.position;
                    ;
                    
                }
                else if (p2.isFixed)
                {
                    StructEdges[i].p1.position = StructEdges[i].p2.position - (1 + DeformRate) * NaturalLength * dir.normalized;
                    p1.Position = StructEdges[i].p1.position;
                    ;

                }
                else
                {
                    //Vector3 temp = (edge.p1.GetComponent<Spring>().Position + edge.p2.GetComponent<Spring>().Position) / 2;
                    Vector3 middle = (p1.Position + p2.Position) / 2;
                    StructEdges[i].p1.position = middle - (1 + DeformRate) * NaturalLength * dir.normalized / 2;
                    StructEdges[i].p2.position = middle + (1 + DeformRate) * NaturalLength * dir.normalized / 2;

                    p1.Position = StructEdges[i].p1.position;
                    p2.Position = StructEdges[i].p2.position;
                    //edge.p1.position = edge.p1.GetComponent<Spring>().Position;
                    //edge.p2.position = edge.p2.GetComponent<Spring>().Position;

                }
            }
            else
            {
                SuperElongation = false;
                if (!p1.isFixed)
                {
                    StructEdges[i].p1.position = p1.Position;
                }
                if (!p2.isFixed)
                {
                    StructEdges[i].p2.position = p2.Position;
                }
                continue;
            }

            

            Edge edge = StructEdges[i];

            edge.Length = (StructEdges[i].p1.position - StructEdges[i].p2.position).magnitude;
            edge.SuperElongation = SuperElongation;
            StructEdges[i] = edge;
            //Debug.Log(edge.Length);
        }

        if (AddShearForce)
        {
            for (int i = 0; i < ShearEdges.Count; i++)
            {


                bool SuperElongation = false;
                Spring p1 = ShearEdges[i].p1.GetComponent<Spring>();
                Spring p2 = ShearEdges[i].p2.GetComponent<Spring>();

                Vector3 dir = (p2.Position - p1.Position);
                float dis = dir.magnitude;



                if (dis >= NaturalLength * (1 + DeformRate))
                {
                    SuperElongation = true;
                    //Debug.Log("deform rate higher than expect");
                    if (p1.isFixed && p2.isFixed)
                    {
                        continue;
                    }
                    else if (p1.isFixed)
                    {
                        ShearEdges[i].p2.position = ShearEdges[i].p1.position + (1 + DeformRate) * NaturalLength * dir.normalized * 1.414f;
                        p2.Position = ShearEdges[i].p2.position;
                        ;

                    }
                    else if (p2.isFixed)
                    {
                        ShearEdges[i].p1.position = ShearEdges[i].p2.position - (1 + DeformRate) * NaturalLength * dir.normalized * 1.414f;
                        p1.Position = ShearEdges[i].p1.position;
                        ;

                    }
                    else
                    {
                        //Vector3 temp = (edge.p1.GetComponent<Spring>().Position + edge.p2.GetComponent<Spring>().Position) / 2;
                        Vector3 middle = (p1.Position + p2.Position) / 2;
                        ShearEdges[i].p1.position = middle - (1 + DeformRate) * NaturalLength * dir.normalized / 2 * 1.414f;
                        ShearEdges[i].p2.position = middle + (1 + DeformRate) * NaturalLength * dir.normalized / 2 * 1.414f;

                        p1.Position = ShearEdges[i].p1.position;
                        p2.Position = ShearEdges[i].p2.position;
                        //edge.p1.position = edge.p1.GetComponent<Spring>().Position;
                        //edge.p2.position = edge.p2.GetComponent<Spring>().Position;

                    }
                }
                else
                {
                    SuperElongation = false;
                    if (!p1.isFixed)
                    {
                        ShearEdges[i].p1.position = p1.Position;
                    }
                    if (!p2.isFixed)
                    {
                        ShearEdges[i].p2.position = p2.Position;
                    }
                    continue;
                }



                Edge edge = ShearEdges[i];

                edge.Length = (ShearEdges[i].p1.position - ShearEdges[i].p2.position).magnitude;
                edge.SuperElongation = SuperElongation;
                ShearEdges[i] = edge;
                //Debug.Log(edge.Length);
            }
        }
        
    }

    void SuperElasticPosition()
    {
        foreach(Transform pp in p)
        {
            pp.position = pp.GetComponent<Spring>().Position;
        }
    }
    void FixAllVelocity()
    {
        Spring SpringCS;
        foreach (Transform pp in p)
        {
            SpringCS = pp.GetComponent<Spring>();
            SpringCS.Velocity = (SpringCS.Position - SpringCS.PrePosition) / (Time.deltaTime * deltaTime);
        }
    }



}
