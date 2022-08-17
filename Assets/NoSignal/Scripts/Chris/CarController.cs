using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {

	private float m_horizontalInput;
	private float m_verticalInput;
	private float m_steeringAngle;

	public GameObject vehicle;
	public WheelCollider frontDriverW, frontPassengerW;
	public WheelCollider rearDriverW, rearPassengerW;
	public Transform frontDriverT, frontPassengerT;
	public Transform rearDriverT, rearPassengerT;
	public float maxSteerAngle = 30;
	public float motorForce = 0.0f;
	public float breakForce;
	public bool isBreaking = false;
	public float speed;
	public float maxSpeed = 100;
	private bool speedLimit;
	[SerializeField] public AudioClip[] CarClips;
	private AudioSource audioSource;

	private InteractableCar interactableCar;

	// against flipping
	public Vector3 Try = new Vector3(0, -0.9f, 0);
	private Rigidbody rig;

    private void Awake()
    {
		audioSource = GetComponent<AudioSource>();
		interactableCar = GetComponent<InteractableCar>();
	}

	private void Start()
    {
		rig = GetComponent<Rigidbody>();
		rig.centerOfMass = Try;
		speed = rig.velocity.magnitude;
	}

    private void Update()
    {
		if (Input.GetKeyDown(KeyCode.E))
		{
			interactableCar.GetOutOfCar();
		}
	}

	public void GetInput()
	{
		m_horizontalInput = Input.GetAxis("Horizontal");
		m_verticalInput = Input.GetAxis("Vertical");
	}

	private void Steer()
	{
		m_steeringAngle = maxSteerAngle * m_horizontalInput;
		frontDriverW.steerAngle = m_steeringAngle;
		frontPassengerW.steerAngle = m_steeringAngle;
	}

	private void Accelerate()
	{
        if (isBreaking == false && motorForce <= maxSpeed)
        {
		frontDriverW.motorTorque = m_verticalInput * motorForce;
		frontPassengerW.motorTorque = m_verticalInput * motorForce;
		rearPassengerW.motorTorque = m_verticalInput * motorForce;
		rearDriverW.motorTorque = m_verticalInput * motorForce;
		Debug.Log("sam2");
        }

	}

	private void StopAccelerate()
    {
        if (m_verticalInput == 0)
        {
			speedLimit = true;
			isBreaking = true;

			frontDriverW.brakeTorque = 10000;
			frontPassengerW.brakeTorque = 10000;
			rearPassengerW.brakeTorque = 10000;
			rearDriverW.brakeTorque = 10000;


			Debug.Log("sam");
        }
        else
        {
			frontDriverW.brakeTorque = 0;
			frontPassengerW.brakeTorque = 0;
			rearPassengerW.brakeTorque = 0;
			rearDriverW.brakeTorque = 0;

			isBreaking = false;
        }


	}

	private void UpdateWheelPoses()
	{
		UpdateWheelPose(frontDriverW, frontDriverT);
		UpdateWheelPose(frontPassengerW, frontPassengerT);
		UpdateWheelPose(rearDriverW, rearDriverT);
		UpdateWheelPose(rearPassengerW, rearPassengerT);
	}

	private void UpdateWheelPose(WheelCollider _collider, Transform _transform)
	{
		Vector3 _pos = _transform.position;
		Quaternion _quat = _transform.rotation;

		_collider.GetWorldPose(out _pos, out _quat);

		_transform.position = _pos;
		_transform.rotation = _quat;
	}

	private void FixedUpdate()
	{
		GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity, 50.0f);
		Accelerate();
		GetInput();
		Steer();
		StopAccelerate();
		UpdateWheelPoses();
	}

	public void AudioPlay()
    {
		audioSource.Play();
		audioSource.volume = 1;
	}
	public void AudioStop()
    {
		audioSource.volume = 0;
	}

}


